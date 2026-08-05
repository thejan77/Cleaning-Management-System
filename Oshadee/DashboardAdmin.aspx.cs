using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;

public partial class DashboardAdmin : Page
{
    private static string ConnStr
    {
        get { return ConfigurationManager.ConnectionStrings["CmsConnectionString"].ConnectionString; }
    }


    public string JobsOverviewLabelsJson { get; private set; }
    public string TotalJobsSeriesJson { get; private set; }
    public string CompletedJobsSeriesJson { get; private set; }
    public string PendingJobsSeriesJson { get; private set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["UserID"] == null)
        {
            Response.Redirect("~/CleaningManagement/Login.aspx");
            return;
        }

        if (!IsPostBack)
        {
            LoadGreeting();
            LoadKpiCards();
            LoadJobsOverviewChart();
            LoadScheduleGrid();
            LoadRecentActivity();
        }
    }

    private void LoadGreeting()
    {
        litUserName.Text = Server.HtmlEncode(
            Session["UserFullName"] != null ? Session["UserFullName"].ToString() : "Admin");
    }

    private void LoadKpiCards()
    {
        var counts = GetKpiCountsInternal();
        litTotalJobs.Text = counts["TotalJobs"].ToString();
        litPendingJobs.Text = counts["PendingJobs"].ToString();
        litCompletedToday.Text = counts["CompletedToday"].ToString();
        litBreakdowns.Text = counts["Breakdowns"].ToString();
        litFeedbacks.Text = counts["Feedbacks"].ToString();
    }

    private static Dictionary<string, int> GetKpiCountsInternal()
    {
        const string sql = @"
    SELECT
        (SELECT COUNT(*) FROM CmsJob
            WHERE CAST(ScheduledDate AS DATE) = CAST(GETDATE() AS DATE)) AS TotalJobs,

        (SELECT COUNT(*) FROM CmsJob
            WHERE CAST(ScheduledDate AS DATE) = CAST(GETDATE() AS DATE)
            AND Status = 'Pending') AS PendingJobs,

        (SELECT COUNT(*) FROM CmsJob
            WHERE CAST(ScheduledDate AS DATE) = CAST(GETDATE() AS DATE)
            AND Status = 'Completed') AS CompletedToday,

        (SELECT COUNT(*) FROM CmsMachineMaintenance
            WHERE CAST(ScheduledDate AS DATE) = CAST(GETDATE() AS DATE)
            AND RecordType = 'Breakdown'
            AND Status <> 'Completed') AS Breakdowns,

        (SELECT COUNT(*) FROM CmsFeedback
            WHERE CAST(SubmittedDate AS DATE) = CAST(GETDATE() AS DATE)
            AND Status = 'Open') AS Feedbacks;";

        var result = new Dictionary<string, int>();

        using (var conn = new SqlConnection(ConnStr))
        using (var cmd = new SqlCommand(sql, conn))
        {
            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    result["TotalJobs"] = Convert.ToInt32(reader["TotalJobs"]);
                    result["PendingJobs"] = Convert.ToInt32(reader["PendingJobs"]);
                    result["CompletedToday"] = Convert.ToInt32(reader["CompletedToday"]);
                    result["Breakdowns"] = Convert.ToInt32(reader["Breakdowns"]);
                    result["Feedbacks"] = Convert.ToInt32(reader["Feedbacks"]);
                }
            }
        }

        return result;
    }

    private void LoadJobsOverviewChart()
    {
        var labels = new List<string>();
        var totals = new List<int>();
        var completed = new List<int>();
        var pending = new List<int>();

        const string sql = @"
            SELECT
                COUNT(*)                                             AS TotalCount,
                SUM(CASE WHEN Status = 'Completed' THEN 1 ELSE 0 END) AS CompletedCount,
                SUM(CASE WHEN Status = 'Pending'   THEN 1 ELSE 0 END) AS PendingCount
            FROM CmsJob
            WHERE CAST(ScheduledDate AS DATE) = @Day;";

        using (var conn = new SqlConnection(ConnStr))
        {
            conn.Open();
            for (int i = 6; i >= 0; i--)
            {
                DateTime day = DateTime.Today.AddDays(-i);
                labels.Add(day.ToString("dd MMM"));

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Day", day.Date);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            totals.Add(reader["TotalCount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TotalCount"]));
                            completed.Add(reader["CompletedCount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CompletedCount"]));
                            pending.Add(reader["PendingCount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PendingCount"]));
                        }
                        else
                        {
                            totals.Add(0);
                            completed.Add(0);
                            pending.Add(0);
                        }
                    }
                }
            }
        }

        JobsOverviewLabelsJson = ToJsonStringArray(labels);
        TotalJobsSeriesJson = ToJsonIntArray(totals);
        CompletedJobsSeriesJson = ToJsonIntArray(completed);
        PendingJobsSeriesJson = ToJsonIntArray(pending);
    }

    private void LoadScheduleGrid()
    {
        const string sql = @"
        SELECT
            ISNULL(CONVERT(VARCHAR(8), cs.RepeatTime, 108), '-') AS ScheduledTime,
            sec.SectionName                                      AS SectionName,
            loc.LocationName                                     AS LocationName,
            ISNULL(t.TeamName, st.Name)                          AS AssignedTo,
            j.Status                                             AS Status
        FROM CmsJob j
        INNER JOIN CmsSection sec 
            ON sec.SectionID = j.SectionID

        INNER JOIN CmsLocation loc 
            ON loc.LocationID = sec.LocationID

        LEFT JOIN CmsCleaningSchedule cs 
            ON cs.ScheduleID = j.ScheduleID

        LEFT JOIN CmsTeam t 
            ON t.TeamID = j.TeamID

        LEFT JOIN CmsStaff st 
            ON st.StaffID = j.StaffID

        WHERE CAST(j.ScheduledDate AS DATE) = CAST(GETDATE() AS DATE)

        ORDER BY cs.RepeatTime;";

        using (var conn = new SqlConnection(ConnStr))
        using (var cmd = new SqlCommand(sql, conn))
        using (var adapter = new SqlDataAdapter(cmd))
        {
            var table = new DataTable();
            adapter.Fill(table);

            gvSchedule.DataSource = table;
            gvSchedule.DataBind();
        }
    }

    private void LoadRecentActivity()
    {
       
        const string sql = @"
            SELECT TOP 10
                al.ActionType,
                al.TargetEntityType,
                al.TargetEntityID,
                al.Details,
                al.Timestamp,
                ISNULL(ua.Username, 'System') AS ActorName
            FROM CmsAuditLog al
            LEFT JOIN CmsUserAccount ua ON ua.UserID = al.UserID
            ORDER BY al.Timestamp DESC;";

        using (var conn = new SqlConnection(ConnStr))
        using (var cmd = new SqlCommand(sql, conn))
        using (var adapter = new SqlDataAdapter(cmd))
        {
            var table = new DataTable();
            adapter.Fill(table);
            rptActivity.DataSource = table;
            rptActivity.DataBind();
        }
    }

    public string GetStatusCssClass(string status)
    {
        if (string.IsNullOrEmpty(status)) return "cms-status-open";

        switch (status.Trim().ToLowerInvariant())
        {
            case "completed":
                return "cms-status-completed";
            case "pending":
                return "cms-status-pending";
            case "in progress":
                return "cms-status-inprogress";
            default:
                return "cms-status-open";
        }
    }


    public string GetActivityIconClass(string actionType)
    {
        if (string.IsNullOrEmpty(actionType)) return "other";

        switch (actionType.Trim().ToLowerInvariant())
        {
            case "insert":
            case "create":
            case "added":
                return "create";
            case "update":
            case "edit":
            case "status change":
                return "update";
            case "delete":
            case "remove":
                return "delete";
            default:
                return "other";
        }
    }

    public string GetActivityIconFa(string actionType)
    {
        switch (GetActivityIconClass(actionType))
        {
            case "create": return "fa-solid fa-plus";
            case "update": return "fa-solid fa-pen";
            case "delete": return "fa-solid fa-trash";
            default: return "fa-solid fa-circle-info";
        }
    }

    public string GetActivityVerb(string actionType)
    {
        switch (GetActivityIconClass(actionType))
        {
            case "create": return "created";
            case "update": return "updated";
            case "delete": return "removed";
            default: return "acted on";
        }
    }

    public string GetRelativeTime(DateTime timestamp)
    {
        TimeSpan diff = DateTime.Now - timestamp;

        if (diff.TotalSeconds < 60) return "just now";
        if (diff.TotalMinutes < 60) return (int)diff.TotalMinutes + " min ago";
        if (diff.TotalHours < 24) return (int)diff.TotalHours + " hr ago";
        if (diff.TotalDays < 7) return (int)diff.TotalDays + " day(s) ago";

        return timestamp.ToString("dd MMM, hh:mm tt");
    }

    private static string ToJsonIntArray(List<int> values)
    {
        var sb = new StringBuilder();
        sb.Append("[");
        for (int i = 0; i < values.Count; i++)
        {
            if (i > 0) sb.Append(",");
            sb.Append(values[i]);
        }
        sb.Append("]");
        return sb.ToString();
    }

    private static string ToJsonStringArray(List<string> values)
    {
        var sb = new StringBuilder();
        sb.Append("[");
        for (int i = 0; i < values.Count; i++)
        {
            if (i > 0) sb.Append(",");
            sb.Append("\"").Append(JsonEscape(values[i])).Append("\"");
        }
        sb.Append("]");
        return sb.ToString();
    }

    private static string JsonEscape(string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}