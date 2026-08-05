using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CleaningManagement_CleaningSchedule : System.Web.UI.Page
{
    private string ConnStr
    {
        get { return ConfigurationManager.ConnectionStrings["CmsConnectionString"].ConnectionString; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindSectionDropDown();
            BindGrid();
        }
    }

    private void BindGrid()
    {
        const string sql = @"
            SELECT cs.ScheduleID, cs.Frequency, cs.StartDate, cs.RepeatTime,
                   s.SectionName, l.LocationName
            FROM CmsCleaningSchedule cs
            INNER JOIN CmsSection s ON cs.SectionID = s.SectionID
            INNER JOIN CmsLocation l ON s.LocationID = l.LocationID
            ORDER BY cs.StartDate DESC";

        using (SqlConnection conn = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
        {
            DataTable dt = new DataTable();
            da.Fill(dt);
            gvSchedule.DataSource = dt;
            gvSchedule.DataBind();
        }
    }

    private void BindSectionDropDown()
    {
        const string sql = @"
            SELECT s.SectionID, s.SectionName + ' - ' + l.LocationName AS DisplayName
            FROM CmsSection s
            INNER JOIN CmsLocation l ON s.LocationID = l.LocationID
            ORDER BY l.LocationName, s.SectionName";

        using (SqlConnection conn = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
        {
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlSection.DataSource = dt;
            ddlSection.DataBind();
        }
    }

    protected string GetFrequencyBadgeClass(string frequency)
    {
        switch (frequency)
        {
            case "Daily": return "badge badge-info";
            case "Weekly": return "badge badge-warning";
            case "Monthly": return "badge badge-primary";
            default: return "badge badge-secondary";
        }
    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        hfScheduleID.Value = "0";
        litModalTitle.Text = "New Cleaning Schedule";
        ddlSection.ClearSelection();
        ddlFrequency.SelectedIndex = 0;
        txtStartDate.Text = string.Empty;
        txtRepeatTime.Text = string.Empty;
        ShowModal();
    }

    protected void gvSchedule_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int scheduleId = Convert.ToInt32(e.CommandArgument);

        if (e.CommandName == "EditSchedule")
        {
            LoadScheduleForEdit(scheduleId);
        }
        else if (e.CommandName == "DeleteSchedule")
        {
            DeleteSchedule(scheduleId);
            BindGrid();
        }
    }

    protected void gvSchedule_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        // Reserved for future row-level formatting.
    }

    private void LoadScheduleForEdit(int scheduleId)
    {
        const string sql = @"
            SELECT SectionID, Frequency, StartDate, RepeatTime
            FROM CmsCleaningSchedule
            WHERE ScheduleID = @ScheduleID";

        using (SqlConnection conn = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@ScheduleID", scheduleId);
            conn.Open();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    hfScheduleID.Value = scheduleId.ToString();
                    litModalTitle.Text = "Edit Cleaning Schedule";
                    ddlSection.SelectedValue = reader["SectionID"].ToString();
                    ddlFrequency.SelectedValue = reader["Frequency"].ToString();
                    txtStartDate.Text = Convert.ToDateTime(reader["StartDate"]).ToString("yyyy-MM-dd");
                    txtRepeatTime.Text = reader["RepeatTime"] == DBNull.Value
                        ? string.Empty
                        : DateTime.Today.Add((TimeSpan)reader["RepeatTime"]).ToString("HH:mm");
                }
            }
        }

        ShowModal();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (ddlSection.SelectedIndex < 0 || string.IsNullOrWhiteSpace(txtStartDate.Text))
        {
            ShowMessage("Please select a section and start date.", false);
            ShowModal();
            return;
        }

        int scheduleId = Convert.ToInt32(hfScheduleID.Value);
        int sectionId = Convert.ToInt32(ddlSection.SelectedValue);
        string frequency = ddlFrequency.SelectedValue;
        DateTime startDate = Convert.ToDateTime(txtStartDate.Text);
        object repeatTime = string.IsNullOrWhiteSpace(txtRepeatTime.Text)
            ? (object)DBNull.Value
            : TimeSpan.Parse(txtRepeatTime.Text);

        string sql = scheduleId == 0
            ? @"INSERT INTO CmsCleaningSchedule (SectionID, Frequency, StartDate, RepeatTime)
               VALUES (@SectionID, @Frequency, @StartDate, @RepeatTime)"
            : @"UPDATE CmsCleaningSchedule
               SET SectionID = @SectionID, Frequency = @Frequency, StartDate = @StartDate, RepeatTime = @RepeatTime
               WHERE ScheduleID = @ScheduleID";

        using (SqlConnection conn = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@SectionID", sectionId);
            cmd.Parameters.AddWithValue("@Frequency", frequency);
            cmd.Parameters.AddWithValue("@StartDate", startDate);
            cmd.Parameters.AddWithValue("@RepeatTime", repeatTime);
            if (scheduleId != 0)
                cmd.Parameters.AddWithValue("@ScheduleID", scheduleId);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        ShowMessage(scheduleId == 0 ? "Cleaning schedule created." : "Cleaning schedule updated.", true);
        BindGrid();
    }

    private void DeleteSchedule(int scheduleId)
    {
        const string sql = "DELETE FROM CmsCleaningSchedule WHERE ScheduleID = @ScheduleID";

        using (SqlConnection conn = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@ScheduleID", scheduleId);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        ShowMessage("Cleaning schedule deleted.", true);
    }

    private void ShowModal()
    {
        ScriptManager.RegisterStartupScript(this, GetType(), "showScheduleModal",
            "$('#scheduleModal').modal('show');", true);
    }

    private void ShowMessage(string message, bool success)
    {
        lblMessage.Text = message;
        lblMessage.CssClass = success ? "alert alert-success" : "alert alert-danger";
        lblMessage.Visible = true;
    }
}