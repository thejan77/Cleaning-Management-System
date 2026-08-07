using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CleaningSchedule : Page
{
    private string connStr = ConfigurationManager.ConnectionStrings["CmsConnectionString"].ConnectionString;

    private string ActiveView
    {
        get { return ViewState["ActiveView"] == null ? "Table" : ViewState["ActiveView"].ToString(); }
        set { ViewState["ActiveView"] = value; }
    }

    private string ActiveTab
    {
        get { return ViewState["ActiveTab"] == null ? "Daily" : ViewState["ActiveTab"].ToString(); }
        set { ViewState["ActiveTab"] = value; }
    }

    private int MonthOffset
    {
        get { return ViewState["MonthOffset"] == null ? 0 : (int)ViewState["MonthOffset"]; }
        set { ViewState["MonthOffset"] = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindSectionLookup();
            BindStats();
            ApplyViewHighlight();
            ApplyTabHighlight();
            BindActiveView();
            CloseModal();
        }
    }

    #region Lookups

    private void BindSectionLookup()
    {
        using (var con = new SqlConnection(connStr))
        {
            con.Open();

            BindDropDown(con, "SELECT SectionID, SectionName FROM CmsSection ORDER BY SectionName",
                ddlSection, "SectionID", "SectionName", " Select Section ");

            BindDropDown(con, "SELECT SectionID, SectionName FROM CmsSection ORDER BY SectionName",
                ddlFilterSection, "SectionID", "SectionName", "All Sections");
        }
    }

    private void BindDropDown(SqlConnection con, string sql, DropDownList ddl, string valueField, string textField, string placeholder)
    {
        using (var cmd = new SqlCommand(sql, con))
        using (var da = new SqlDataAdapter(cmd))
        {
            var dt = new DataTable();
            da.Fill(dt);
            ddl.Items.Clear();

            ddl.DataSource = dt;
            ddl.DataTextField = textField;
            ddl.DataValueField = valueField;
            ddl.DataBind();

            ddl.Items.Insert(0, new ListItem(placeholder, ""));
        }
    }

    #endregion

    #region Stats

    private void BindStats()
    {
        using (var con = new SqlConnection(connStr))
        using (var cmd = new SqlCommand(@"
            SELECT
                SUM(CASE WHEN Frequency = 'Daily' THEN 1 ELSE 0 END) AS DailyCount,
                SUM(CASE WHEN Frequency = 'Weekly' THEN 1 ELSE 0 END) AS WeeklyCount,
                SUM(CASE WHEN Frequency = 'Monthly' THEN 1 ELSE 0 END) AS MonthlyCount,
                SUM(CASE WHEN Status = 'Active' THEN 1 ELSE 0 END) AS ActiveCount
            FROM CmsCleaningSchedule", con))
        {
            con.Open();
            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    litDailyCount.Text = rdr["DailyCount"] == DBNull.Value ? "0" : rdr["DailyCount"].ToString();
                    litWeeklyCount.Text = rdr["WeeklyCount"] == DBNull.Value ? "0" : rdr["WeeklyCount"].ToString();
                    litMonthlyCount.Text = rdr["MonthlyCount"] == DBNull.Value ? "0" : rdr["MonthlyCount"].ToString();
                    litActiveCount.Text = rdr["ActiveCount"] == DBNull.Value ? "0" : rdr["ActiveCount"].ToString();
                }
            }
        }
    }

    protected string GetStatusPillClass(string status)
    {
        switch (status)
        {
            case "Active": return "sch-pill-active";
            case "Inactive": return "sch-pill-inactive";
            case "Completed": return "sch-pill-completed";
            default: return "sch-pill-active";
        }
    }

    
    protected string FormatScheduleCode(int scheduleId)
    {
        return "SCH-" + scheduleId.ToString("D3");
    }

    #endregion

    #region View toggle (Table / Calendar)

    protected void View_Click(object sender, EventArgs e)
    {
        var btn = (LinkButton)sender;
        ActiveView = btn.CommandArgument;
        ApplyViewHighlight();
        BindActiveView();
    }

    private void ApplyViewHighlight()
    {
        btnViewTable.CssClass = "sch-view-btn" + (ActiveView == "Table" ? " active" : "");
        btnViewCalendar.CssClass = "sch-view-btn" + (ActiveView == "Calendar" ? " active" : "");

        pnlCalendarSection.Visible = ActiveView == "Calendar";
        pnlTableView.Visible = ActiveView == "Table";

        pnlFilters.CssClass = "sch-filters" + (ActiveView == "Table" ? "" : " no-freq");
    }

    #endregion

    #region Tabs + Filters

    protected void Tab_Click(object sender, EventArgs e)
    {
        var btn = (LinkButton)sender;
        ActiveTab = btn.CommandArgument;
        ApplyTabHighlight();
        BindActiveView();
    }

    private void ApplyTabHighlight()
    {
        tabDaily.CssClass = "sch-tab" + (ActiveTab == "Daily" ? " active" : "");
        tabWeekly.CssClass = "sch-tab" + (ActiveTab == "Weekly" ? " active" : "");
        tabMonthly.CssClass = "sch-tab" + (ActiveTab == "Monthly" ? " active" : "");
    }

    protected void Filter_Changed(object sender, EventArgs e)
    {
        BindActiveView();
    }

    protected void btnClearFilters_Click(object sender, EventArgs e)
    {
        ddlFilterSection.SelectedValue = "";
        ddlFilterStatus.SelectedValue = "";
        ddlFilterFrequency.SelectedValue = "";
        ActiveTab = "Daily";
        ApplyTabHighlight();
        BindActiveView();
    }

    #endregion

    #region View dispatch

    private void BindActiveView()
    {
        if (ActiveView == "Table")
        {
            BindTableView();
            return;
        }

        switch (ActiveTab)
        {
            case "Weekly":
                BindWeeklyView();
                break;
            case "Monthly":
                BindMonthlyView();
                break;
            default:
                BindDailyView();
                break;
        }
    }

    private void ShowOnlyPanel(string which)
    {
        pnlDailyView.Visible = which == "Daily";
        pnlWeeklyView.Visible = which == "Weekly";
        pnlMonthlyView.Visible = which == "Monthly";
    }

    #endregion

    #region Table view

    private void BindTableView()
    {
        const string sql = @"
    SELECT cs.ScheduleID, cs.CleaningType, cs.Frequency, cs.RepeatTime, cs.StartDate, cs.Status, sec.SectionName
    FROM CmsCleaningSchedule cs
    JOIN CmsSection sec ON sec.SectionID = cs.SectionID
    WHERE (@SectionID = '' OR cs.SectionID = @SectionID)
      AND (@Status = '' OR cs.Status = @Status)
      AND (@Frequency = '' OR cs.Frequency = @Frequency)
    ORDER BY cs.ScheduleID DESC";

        var dt = new DataTable();
        using (var con = new SqlConnection(connStr))
        using (var cmd = new SqlCommand(sql, con))
        {
            cmd.Parameters.AddWithValue("@SectionID", ddlFilterSection.SelectedValue ?? "");
            cmd.Parameters.AddWithValue("@Status", ddlFilterStatus.SelectedValue ?? "");
            cmd.Parameters.AddWithValue("@Frequency", ddlFilterFrequency.SelectedValue ?? "");
            using (var da = new SqlDataAdapter(cmd))
            {
                con.Open();
                da.Fill(dt);
            }
        }

        if (dt.Rows.Count == 0)
        {
            litScheduleTable.Text = "<div class='sch-empty-msg'>No cleaning schedules found.</div>";
            return;
        }

        var sb = new StringBuilder();
        sb.Append("<div class='sch-table-wrap'><table class='sch-table'><thead><tr>");
        sb.Append("<th>Schedule ID</th><th>Section</th><th>Cleaning Type</th><th>Frequency</th><th>Time</th><th>Start Date</th><th>Status</th><th>Actions</th>");
        sb.Append("</tr></thead><tbody>");

        foreach (DataRow row in dt.Rows)
        {
            int scheduleId = Convert.ToInt32(row["ScheduleID"]);
            string section = row["SectionName"].ToString();
            string type = row["CleaningType"] == DBNull.Value ? "General Cleaning" : row["CleaningType"].ToString();
            string frequency = row["Frequency"].ToString();
            string status = row["Status"].ToString();
            string statusClass = GetStatusPillClass(status);
            string timeDisplay = row["RepeatTime"] == DBNull.Value
                ? "--:--"
                : DateTime.Today.Add((TimeSpan)row["RepeatTime"]).ToString("hh:mm tt");
            string startDate = row["StartDate"] == DBNull.Value
                ? "--"
                : Convert.ToDateTime(row["StartDate"]).ToString("dd MMM yyyy");

            sb.Append("<tr>");
            sb.Append("<td>").Append(FormatScheduleCode(scheduleId)).Append("</td>");
            sb.Append("<td>").Append(Server.HtmlEncode(section)).Append("</td>");
            sb.Append("<td>").Append(Server.HtmlEncode(type)).Append("</td>");
            sb.Append("<td><span class='sch-freq-badge'>").Append(Server.HtmlEncode(frequency)).Append("</span></td>");
            sb.Append("<td>").Append(timeDisplay).Append("</td>");
            sb.Append("<td>").Append(startDate).Append("</td>");
            sb.Append("<td><span class='sch-pill ").Append(statusClass).Append("'>").Append(Server.HtmlEncode(status)).Append("</span></td>");
            sb.Append("<td class='sch-row-actions'>");
            sb.Append("<a href='javascript:void(0)' onclick=\"editSchedule(").Append(scheduleId).Append(")\">Edit</a>");
            sb.Append("<a href='javascript:void(0)' onclick=\"deleteSchedule(").Append(scheduleId).Append(")\">Delete</a>");
            sb.Append("</td>");
            sb.Append("</tr>");
        }

        sb.Append("</tbody></table></div>");
        litScheduleTable.Text = sb.ToString();
    }

    #endregion

    #region Daily view

    private void BindDailyView()
    {
        ShowOnlyPanel("Daily");

        litDailyDayNum.Text = DateTime.Today.Day.ToString();
        litDailyDayName.Text = DateTime.Today.ToString("ddd, MMM").ToUpper();

        const string sql = @"
            SELECT cs.ScheduleID, cs.CleaningType, cs.RepeatTime, cs.Status, sec.SectionName
            FROM CmsCleaningSchedule cs
            JOIN CmsSection sec ON sec.SectionID = cs.SectionID
            WHERE cs.Frequency = 'Daily'
              AND (@SectionID = '' OR cs.SectionID = @SectionID)
              AND (@Status = '' OR cs.Status = @Status)
            ORDER BY cs.RepeatTime ASC";

        var dt = new DataTable();
        using (var con = new SqlConnection(connStr))
        using (var cmd = new SqlCommand(sql, con))
        {
            cmd.Parameters.AddWithValue("@SectionID", ddlFilterSection.SelectedValue ?? "");
            cmd.Parameters.AddWithValue("@Status", ddlFilterStatus.SelectedValue ?? "");
            using (var da = new SqlDataAdapter(cmd))
            {
                con.Open();
                da.Fill(dt);
            }
        }

        litDailySummary.Text = dt.Rows.Count + " daily cleaning job" + (dt.Rows.Count == 1 ? "" : "s");

        if (dt.Rows.Count == 0)
        {
            litDailyTimeline.Text = "<div class='sch-empty-msg'>No daily cleaning schedules found.</div>";
            return;
        }

        string[] colorClasses = { "blue", "purple", "amber" };
        var sb = new StringBuilder();
        int idx = 0;

        foreach (DataRow row in dt.Rows)
        {
            int scheduleId = Convert.ToInt32(row["ScheduleID"]);
            string section = row["SectionName"].ToString();
            string type = row["CleaningType"] == DBNull.Value ? "General Cleaning" : row["CleaningType"].ToString();
            string status = row["Status"].ToString();
            string timeDisplay = row["RepeatTime"] == DBNull.Value
                ? "--:--"
                : DateTime.Today.Add((TimeSpan)row["RepeatTime"]).ToString("hh:mm tt");
            string colorClass = colorClasses[idx % colorClasses.Length];

            sb.Append("<div class='sch-time-row'>");
            sb.Append("<div class='sch-time-label'>").Append(timeDisplay).Append("</div>");
            sb.Append("<div class='sch-job-card ").Append(colorClass).Append("' onclick=\"editSchedule(").Append(scheduleId).Append(")\">");
            sb.Append("<div class='sch-jc-time'>").Append(timeDisplay).Append(" &middot; ").Append(FormatScheduleCode(scheduleId)).Append("</div>");
            sb.Append("<div class='sch-jc-name'>").Append(Server.HtmlEncode(section)).Append(" &mdash; ").Append(Server.HtmlEncode(type)).Append("</div>");
            sb.Append("<div class='sch-jc-foot'>");
            sb.Append("<span class='sch-jc-status'>").Append(Server.HtmlEncode(status)).Append("</span>");
            sb.Append("<span class='sch-jc-actions'>");
            sb.Append("<a href='javascript:void(0)' onclick=\"event.stopPropagation(); editSchedule(").Append(scheduleId).Append(")\">Edit</a>");
            sb.Append("<a href='javascript:void(0)' onclick=\"event.stopPropagation(); deleteSchedule(").Append(scheduleId).Append(")\">Delete</a>");
            sb.Append("</span>");
            sb.Append("</div></div></div>");

            idx++;
        }

        litDailyTimeline.Text = sb.ToString();
    }

    #endregion

    #region Weekly view

    private void BindWeeklyView()
    {
        ShowOnlyPanel("Weekly");

        const string sql = @"
            SELECT cs.ScheduleID, cs.CleaningType, cs.RepeatTime, cs.StartDate, cs.Status, sec.SectionName
            FROM CmsCleaningSchedule cs
            JOIN CmsSection sec ON sec.SectionID = cs.SectionID
            WHERE cs.Frequency = 'Weekly'
              AND (@SectionID = '' OR cs.SectionID = @SectionID)
              AND (@Status = '' OR cs.Status = @Status)
            ORDER BY cs.RepeatTime ASC";

        var dt = new DataTable();
        using (var con = new SqlConnection(connStr))
        using (var cmd = new SqlCommand(sql, con))
        {
            cmd.Parameters.AddWithValue("@SectionID", ddlFilterSection.SelectedValue ?? "");
            cmd.Parameters.AddWithValue("@Status", ddlFilterStatus.SelectedValue ?? "");
            using (var da = new SqlDataAdapter(cmd))
            {
                con.Open();
                da.Fill(dt);
            }
        }

        if (dt.Rows.Count == 0)
        {
            litWeeklyGrid.Text = "<div class='sch-empty-msg'>No weekly cleaning schedules found.</div>";
            return;
        }

        var byDay = new Dictionary<DayOfWeek, List<DataRow>>();
        foreach (DayOfWeek d in Enum.GetValues(typeof(DayOfWeek)))
            byDay[d] = new List<DataRow>();

        foreach (DataRow row in dt.Rows)
        {
            DateTime start = Convert.ToDateTime(row["StartDate"]);
            byDay[start.DayOfWeek].Add(row);
        }

        DayOfWeek[] weekOrder = { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
                                  DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday };

        var sb = new StringBuilder();
        sb.Append("<div class='sch-week-grid'>");

        foreach (var day in weekOrder)
            sb.Append("<div class='sch-week-head'>").Append(day.ToString()).Append("</div>");

        foreach (var day in weekOrder)
        {
            sb.Append("<div class='sch-week-col'>");
            var rows = byDay[day];

            if (rows.Count == 0)
            {
                sb.Append("<div class='sch-week-empty'>&nbsp;</div>");
            }
            else
            {
                foreach (var row in rows)
                {
                    int scheduleId = Convert.ToInt32(row["ScheduleID"]);
                    string section = row["SectionName"].ToString();
                    string type = row["CleaningType"] == DBNull.Value ? "General Cleaning" : row["CleaningType"].ToString();
                    string status = row["Status"].ToString();
                    string statusClass = GetStatusPillClass(status);
                    string timeDisplay = row["RepeatTime"] == DBNull.Value
                        ? "--:--"
                        : DateTime.Today.Add((TimeSpan)row["RepeatTime"]).ToString("hh:mm tt");

                    sb.Append("<div class='sch-job-chip' onclick=\"editSchedule(").Append(scheduleId).Append(")\">");
                    sb.Append("<div class='sch-chip-time'>").Append(timeDisplay).Append(" &middot; ").Append(FormatScheduleCode(scheduleId)).Append("</div>");
                    sb.Append("<div class='sch-chip-name'>").Append(Server.HtmlEncode(section)).Append("</div>");
                    sb.Append("<div class='sch-chip-type'>").Append(Server.HtmlEncode(type)).Append("</div>");
                    sb.Append("<span class='sch-chip-status sch-pill ").Append(statusClass).Append("'>").Append(Server.HtmlEncode(status)).Append("</span>");
                    sb.Append("</div>");
                }
            }

            sb.Append("</div>");
        }

        sb.Append("</div>");
        litWeeklyGrid.Text = sb.ToString();
    }

    #endregion

    #region Monthly view

    protected void btnPrevMonth_Click(object sender, EventArgs e)
    {
        MonthOffset -= 1;
        BindActiveView();
    }

    protected void btnNextMonth_Click(object sender, EventArgs e)
    {
        MonthOffset += 1;
        BindActiveView();
    }

    private void BindMonthlyView()
    {
        ShowOnlyPanel("Monthly");
        DateTime displayMonth = DateTime.Today.AddMonths(MonthOffset);
        litMonthTitle.Text = displayMonth.ToString("MMMM yyyy").ToUpper() + " CLEANING PLAN";

        DateTime monthStart = new DateTime(displayMonth.Year, displayMonth.Month, 1);
        DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);

        const string sql = @"
    SELECT cs.ScheduleID, cs.CleaningType, cs.RepeatTime, cs.StartDate, cs.Status, sec.SectionName
    FROM CmsCleaningSchedule cs
    JOIN CmsSection sec ON sec.SectionID = cs.SectionID
    WHERE cs.Frequency = 'Monthly'
      AND cs.StartDate <= @MonthEnd
      AND (cs.EndDate IS NULL OR cs.EndDate >= @MonthStart)
      AND (@SectionID = '' OR cs.SectionID = @SectionID)
      AND (@Status = '' OR cs.Status = @Status)";

        var dt = new DataTable();
        using (var con = new SqlConnection(connStr))
        using (var cmd = new SqlCommand(sql, con))
        {
            cmd.Parameters.AddWithValue("@MonthStart", monthStart);
            cmd.Parameters.AddWithValue("@MonthEnd", monthEnd);
            cmd.Parameters.AddWithValue("@SectionID", ddlFilterSection.SelectedValue ?? "");
            cmd.Parameters.AddWithValue("@Status", ddlFilterStatus.SelectedValue ?? "");
            using (var da = new SqlDataAdapter(cmd))
            {
                con.Open();
                da.Fill(dt);
            }
        }

        var byDayOfMonth = new Dictionary<int, List<DataRow>>();
        foreach (DataRow row in dt.Rows)
        {
            DateTime start = Convert.ToDateTime(row["StartDate"]);
            int dayOfMonth = start.Day;
            if (!byDayOfMonth.ContainsKey(dayOfMonth))
                byDayOfMonth[dayOfMonth] = new List<DataRow>();
            byDayOfMonth[dayOfMonth].Add(row);
        }

        int daysInMonth = DateTime.DaysInMonth(displayMonth.Year, displayMonth.Month);
        DateTime firstOfMonth = new DateTime(displayMonth.Year, displayMonth.Month, 1);
        // 0 = Monday .. 6 = Sunday
        int firstDayIndex = ((int)firstOfMonth.DayOfWeek + 6) % 7;

        var sb = new StringBuilder();
        sb.Append("<div class='sch-month-grid'>");

        string[] dowLabels = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
        foreach (var lbl in dowLabels)
            sb.Append("<div class='sch-month-dow'>").Append(lbl).Append("</div>");

        for (int i = 0; i < firstDayIndex; i++)
            sb.Append("<div class='sch-month-cell blank'></div>");

        for (int day = 1; day <= daysInMonth; day++)
        {
            sb.Append("<div class='sch-month-cell'>");
            sb.Append("<div class='sch-month-daynum'>").Append(day).Append("</div>");

            if (byDayOfMonth.ContainsKey(day))
            {
                foreach (var row in byDayOfMonth[day])
                {
                    int scheduleId = Convert.ToInt32(row["ScheduleID"]);
                    string section = row["SectionName"].ToString();
                    string type = row["CleaningType"] == DBNull.Value ? "" : row["CleaningType"].ToString();
                    string timeDisplay = row["RepeatTime"] == DBNull.Value
                        ? ""
                        : DateTime.Today.Add((TimeSpan)row["RepeatTime"]).ToString("hh:mm tt");

                    sb.Append("<div class='sch-month-job' onclick=\"editSchedule(").Append(scheduleId).Append(")\">");
                    sb.Append("<div class='sch-month-jname'>").Append(Server.HtmlEncode(type)).Append("</div>");
                    sb.Append("<div class='sch-month-jtime'>").Append(timeDisplay).Append(" &middot; ").Append(Server.HtmlEncode(section)).Append("</div>");
                    sb.Append("<div class='sch-month-jcode'>").Append(FormatScheduleCode(scheduleId)).Append("</div>");
                    sb.Append("</div>");
                }
            }

            sb.Append("</div>");
        }

        int totalCells = firstDayIndex + daysInMonth;
        int remainder = totalCells % 7;
        if (remainder != 0)
        {
            int trailing = 7 - remainder;
            for (int i = 0; i < trailing; i++)
                sb.Append("<div class='sch-month-cell blank'></div>");
        }

        sb.Append("</div>");

        litMonthlyGrid.Text = dt.Rows.Count == 0
            ? "<div class='sch-empty-msg'>No monthly cleaning schedules found.</div>" + sb.ToString()
            : sb.ToString();
    }

    #endregion

    #region Add / Edit / Delete

    protected void btnOpenAdd_Click(object sender, EventArgs e)
    {
        ResetForm();
        litModalTitle.Text = "Add Cleaning Schedule";
        OpenModal();
    }

    protected void lnkProxyEdit_Click(object sender, EventArgs e)
    {
        int scheduleId = Convert.ToInt32(hfActionScheduleID.Value);
        LoadScheduleIntoForm(scheduleId);
        litModalTitle.Text = "Edit Cleaning Schedule";
        OpenModal();
    }

    protected void lnkProxyDelete_Click(object sender, EventArgs e)
    {
        int scheduleId = Convert.ToInt32(hfActionScheduleID.Value);
        DeleteSchedule(scheduleId);
        BindStats();
        BindActiveView();
    }

    private void LoadScheduleIntoForm(int scheduleId)
    {
        const string sql = @"SELECT * FROM CmsCleaningSchedule WHERE ScheduleID = @ScheduleID";
        using (var con = new SqlConnection(connStr))
        using (var cmd = new SqlCommand(sql, con))
        {
            cmd.Parameters.AddWithValue("@ScheduleID", scheduleId);
            con.Open();
            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    hfScheduleID.Value = rdr["ScheduleID"].ToString();
                    txtScheduleCode.Text = FormatScheduleCode(scheduleId);
                    ddlSection.SelectedValue = rdr["SectionID"].ToString();
                    ddlCleaningType.SelectedValue = rdr["CleaningType"] == DBNull.Value ? "" : rdr["CleaningType"].ToString();
                    ddlFrequency.SelectedValue = rdr["Frequency"].ToString();
                    ddlStatus.SelectedValue = rdr["Status"] == DBNull.Value ? "Active" : rdr["Status"].ToString();
                    txtDescription.Text = rdr["Description"] == DBNull.Value ? "" : rdr["Description"].ToString();

                    txtStartDate.Text = Convert.ToDateTime(rdr["StartDate"]).ToString("yyyy-MM-dd");
                    txtEndDate.Text = rdr["EndDate"] == DBNull.Value ? ""
                        : Convert.ToDateTime(rdr["EndDate"]).ToString("yyyy-MM-dd");
                    txtRepeatTime.Text = rdr["RepeatTime"] == DBNull.Value ? ""
                        : ((TimeSpan)rdr["RepeatTime"]).ToString(@"hh\:mm");
                }
            }
        }
    }

    protected void btnSaveRecord_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) { OpenModal(); return; }

        int scheduleId = Convert.ToInt32(hfScheduleID.Value);

        int currentUserId = Session["UserID"] != null ? Convert.ToInt32(Session["UserID"]) : 1;

        try
        {
            using (var con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd;

                if (scheduleId == 0)
                {
                    cmd = new SqlCommand(@"
                        INSERT INTO CmsCleaningSchedule
                            (SectionID, Frequency, StartDate, RepeatTime, CleaningType, EndDate, Status,
                             Description, CreatedDate, CreatedBy)
                        VALUES
                            (@SectionID, @Frequency, @StartDate, @RepeatTime, @CleaningType, @EndDate, @Status,
                             @Description, GETDATE(), @CreatedBy)", con);
                    cmd.Parameters.AddWithValue("@CreatedBy", currentUserId);
                }
                else
                {
                    cmd = new SqlCommand(@"
                        UPDATE CmsCleaningSchedule SET
                            SectionID = @SectionID, Frequency = @Frequency, StartDate = @StartDate,
                            RepeatTime = @RepeatTime, CleaningType = @CleaningType, EndDate = @EndDate,
                            Status = @Status, Description = @Description
                        WHERE ScheduleID = @ScheduleID", con);
                    cmd.Parameters.AddWithValue("@ScheduleID", scheduleId);
                }

                cmd.Parameters.AddWithValue("@SectionID", Convert.ToInt32(ddlSection.SelectedValue));
                cmd.Parameters.AddWithValue("@Frequency", ddlFrequency.SelectedValue);
                cmd.Parameters.AddWithValue("@StartDate", Convert.ToDateTime(txtStartDate.Text));
                cmd.Parameters.AddWithValue("@RepeatTime", string.IsNullOrEmpty(txtRepeatTime.Text) ? (object)DBNull.Value : TimeSpan.Parse(txtRepeatTime.Text));
                cmd.Parameters.AddWithValue("@CleaningType", string.IsNullOrEmpty(ddlCleaningType.SelectedValue) ? (object)DBNull.Value : ddlCleaningType.SelectedValue);
                cmd.Parameters.AddWithValue("@EndDate", string.IsNullOrEmpty(txtEndDate.Text) ? (object)DBNull.Value : Convert.ToDateTime(txtEndDate.Text));
                cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);
                cmd.Parameters.AddWithValue("@Description", string.IsNullOrEmpty(txtDescription.Text) ? (object)DBNull.Value : txtDescription.Text.Trim());

                using (cmd) { cmd.ExecuteNonQuery(); }
            }

            CloseModal();
            BindStats();
            BindActiveView();
        }
        catch (Exception ex)
        {
            litError.Text = "<div style='color:#DC2626; margin-top:8px; font-size:13px;'>Could not save the record: " + ex.Message + "</div>";
            OpenModal();
        }
    }

    private void DeleteSchedule(int scheduleId)
    {
        using (var con = new SqlConnection(connStr))
        using (var cmd = new SqlCommand("DELETE FROM CmsCleaningSchedule WHERE ScheduleID = @ScheduleID", con))
        {
            cmd.Parameters.AddWithValue("@ScheduleID", scheduleId);
            con.Open();
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SqlException)
            {
            }
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        CloseModal();
    }

    private string GetNextScheduleCode()
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            con.Open();

            SqlCommand cmd = new SqlCommand(
                "SELECT ISNULL(MAX(ScheduleID),0) + 1 FROM CmsCleaningSchedule",
                con);

            int nextId = Convert.ToInt32(cmd.ExecuteScalar());

            return "SCH-" + nextId.ToString("D3");
        }
    }

    private void ResetForm()
    {
        hfScheduleID.Value = "0";
        txtScheduleCode.Text = GetNextScheduleCode();
        ddlSection.SelectedValue = "";
        ddlCleaningType.SelectedValue = "";
        ddlFrequency.SelectedValue = "Daily";
        ddlStatus.SelectedValue = "Active";
        txtDescription.Text = "";
        txtStartDate.Text = "";
        txtEndDate.Text = "";
        txtRepeatTime.Text = "";
        litError.Text = "";
    }

    #endregion

    #region Modal open/close

    private void OpenModal()
    {
        pnlModalOverlay.CssClass = "sch-modal-overlay show";
    }

    private void CloseModal()
    {
        pnlModalOverlay.CssClass = "sch-modal-overlay";
        ResetForm();
    }

    #endregion
}