using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CleaningJob : Page
{

    private string connStr = ConfigurationManager.ConnectionStrings["CmsConnectionString"].ConnectionString;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindLookups();
            BindStats();
            BindGrid();
            CloseModal();
        }
    }

    #region Job Code helpers (JB-001 format)


    protected string FormatJobCode(object jobId)
    {
        if (jobId == null || jobId == DBNull.Value) return "";
        int id;
        if (!int.TryParse(jobId.ToString(), out id)) return "";
        return "JB-" + id.ToString("D3");
    }

    private string GetNextJobCode()
    {
        using (var con = new SqlConnection(connStr))
        using (var cmd = new SqlCommand("SELECT ISNULL(MAX(JobID), 0) + 1 FROM CmsJob", con))
        {
            con.Open();
            int nextId = Convert.ToInt32(cmd.ExecuteScalar());
            return "JB-" + nextId.ToString("D3");
        }
    }

    #endregion

    #region Lookups (dropdown data sources)

    private void BindLookups()
    {
        using (var con = new SqlConnection(connStr))
        {
            con.Open();

            BindDropDown(con, "SELECT SectionID, SectionName FROM CmsSection ORDER BY SectionName",
                ddlSection, "SectionID", "SectionName", " Select Section ");

            BindDropDown(con, "SELECT SectionID, SectionName FROM CmsSection ORDER BY SectionName",
                ddlFilterSection, "SectionID", "SectionName", "All Sections");

            BindDropDown(con, "SELECT TeamID, TeamName FROM CmsTeam WHERE Active = 1 ORDER BY TeamName",
                ddlTeam, "TeamID", "TeamName", " Select Team ");

            BindDropDown(con, "SELECT StaffID, Name FROM CmsStaff WHERE RoleID = 3 ORDER BY Name",
                ddlStaff, "StaffID", "Name", "Select Staff ");

            BindDropDown(con,
                @"SELECT ts.TeamSupervisorID, s.Name + ' (' + t.TeamName + ')' AS DisplayName
                  FROM CmsTeamSupervisor ts
                  JOIN CmsStaff s ON s.StaffID = ts.StaffID
                  JOIN CmsTeam t ON t.TeamID = ts.TeamID 
                  ORDER BY s.Name",
                ddlTeamSupervisor, "TeamSupervisorID", "DisplayName", " Select Supervisor ");

           
            BindDropDown(con,
     @"SELECT 
          cs.ScheduleID,
          'SCH-' + RIGHT('000' + CONVERT(varchar, cs.ScheduleID), 3)
          + ' - '
          + sec.SectionName AS DisplayName
      FROM CmsCleaningSchedule cs
      INNER JOIN CmsSection sec 
          ON sec.SectionID = cs.SectionID
      WHERE cs.Status = 'Active'
      ORDER BY cs.ScheduleID DESC",
     ddlSchedule, "ScheduleID", "DisplayName", "None ");


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

    #region Stats + Grid



    private void BindScheduleDropDown(SqlConnection con)
    {
    
        string sql = @"
    SELECT
        cs.ScheduleID,
        'SCH-' + RIGHT('000' + CONVERT(varchar, cs.ScheduleID), 3)
        + ' - '
        + sec.SectionName AS DisplayName
    FROM CmsCleaningSchedule cs
    INNER JOIN CmsSection sec 
        ON sec.SectionID = cs.SectionID
    WHERE cs.Status = 'Active'
    ORDER BY cs.ScheduleID DESC";

        using (SqlCommand cmd = new SqlCommand(sql, con))
        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
        {
            DataTable dt = new DataTable();
            da.Fill(dt);

            ddlSchedule.DataSource = dt;
            ddlSchedule.DataTextField = "DisplayName";
            ddlSchedule.DataValueField = "ScheduleID";
            ddlSchedule.DataBind();

            ddlSchedule.Items.Insert(0, new ListItem("-- None --", ""));
        }
    }

    private void LockScheduleFields(bool locked)
    {
        ddlSection.Enabled = !locked;
        ddlCleaningType.Enabled = !locked;
        ddlFrequency.Enabled = !locked;

        Response.Write("LOCK STATUS: " + locked);
    }
    private void BindStats()
    {
        using (var con = new SqlConnection(connStr))
        using (var cmd = new SqlCommand(@"
            SELECT
                SUM(CASE WHEN Status = 'Pending' THEN 1 ELSE 0 END) AS PendingCount,
                SUM(CASE WHEN Status = 'In Progress' THEN 1 ELSE 0 END) AS InProgressCount,
                SUM(CASE WHEN Status = 'Completed' AND MONTH(ExpectedCompletionDate) = MONTH(GETDATE())
                          AND YEAR(ExpectedCompletionDate) = YEAR(GETDATE()) THEN 1 ELSE 0 END) AS CompletedThisMonth,
                COUNT(*) AS TotalCount
            FROM CmsJob", con))
        {
            con.Open();
            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    litPending.Text = rdr["PendingCount"] == DBNull.Value ? "0" : rdr["PendingCount"].ToString();
                    litInProgress.Text = rdr["InProgressCount"] == DBNull.Value ? "0" : rdr["InProgressCount"].ToString();
                    litCompleted.Text = rdr["CompletedThisMonth"] == DBNull.Value ? "0" : rdr["CompletedThisMonth"].ToString();
                    litTotal.Text = rdr["TotalCount"].ToString();
                }
            }
        }
    }

    private void BindGrid()
    {
        var sql = @"
            SELECT j.JobID, j.Description, j.CleaningType, j.ScheduledDate, j.Status, j.Priority,
                   sec.SectionName,
                   CASE WHEN j.AssignmentType = 'Staff' THEN st.Name
                        WHEN j.AssignmentType = 'Team' THEN tm.TeamName
                        ELSE '-' END AS AssignedToDisplay
            FROM CmsJob j
            JOIN CmsSection sec ON sec.SectionID = j.SectionID
            LEFT JOIN CmsTeam tm ON tm.TeamID = j.TeamID
            LEFT JOIN CmsStaff st ON st.StaffID = j.StaffID
            WHERE (@SectionID = '' OR j.SectionID = @SectionID)
              AND (@Status = '' OR j.Status = @Status)
              AND (@Priority = '' OR j.Priority = @Priority)
              AND (@AssignmentType = '' OR j.AssignmentType = @AssignmentType)
            ORDER BY j.ScheduledDate DESC, j.JobID DESC";

        using (var con = new SqlConnection(connStr))
        using (var cmd = new SqlCommand(sql, con))
        {
            cmd.Parameters.AddWithValue("@SectionID", ddlFilterSection.SelectedValue ?? "");
            cmd.Parameters.AddWithValue("@Status", ddlFilterStatus.SelectedValue ?? "");
            cmd.Parameters.AddWithValue("@Priority", ddlFilterPriority.SelectedValue ?? "");
            cmd.Parameters.AddWithValue("@AssignmentType", ddlFilterAssignment.SelectedValue ?? "");

            using (var da = new SqlDataAdapter(cmd))
            {
                var dt = new DataTable();
                con.Open();
                da.Fill(dt);

                rptJobs.DataSource = dt;
                rptJobs.DataBind();
                phEmpty.Visible = dt.Rows.Count == 0;
            }
        }
    }

    protected string GetStatusPillClass(string status)
    {
        switch (status)
        {
            case "Pending": return "cj-pill-pending";
            case "In Progress": return "cj-pill-progress";
            case "Completed": return "cj-pill-completed";
            case "Cancelled": return "cj-pill-cancelled";
            default: return "cj-pill-pending";
        }
    }

    protected string GetPriorityPillClass(string priority)
    {
        switch (priority)
        {
            case "Low": return "cj-pill-low";
            case "Normal": return "cj-pill-normal";
            case "High": return "cj-pill-high";
            case "Urgent": return "cj-pill-urgent";
            default: return "cj-pill-normal";
        }
    }

    #endregion

    #region Filters

    protected void Filter_Changed(object sender, EventArgs e)
    {
        BindGrid();
    }

    protected void btnClearFilters_Click(object sender, EventArgs e)
    {
        ddlFilterSection.SelectedValue = "";
        ddlFilterStatus.SelectedValue = "";
        ddlFilterPriority.SelectedValue = "";
        ddlFilterAssignment.SelectedValue = "";
        BindGrid();
    }

    #endregion

    #region Add / Edit / Delete

    protected void btnOpenAdd_Click(object sender, EventArgs e)
    {
        ResetForm();

        using (SqlConnection con = new SqlConnection(connStr))
        {
            con.Open();
            BindScheduleDropDown(con);
        }

       
        litJobCode.Text = GetNextJobCode();

        litModalTitle.Text = "Add Cleaning Job";
        OpenModal();
    }
    protected void rptJobs_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int jobId = Convert.ToInt32(e.CommandArgument);

        litError.Text = "Edit clicked JobID = " + jobId;

        if (e.CommandName == "EditJob")
        {
            LoadJobIntoForm(jobId);
            litModalTitle.Text = "Edit Cleaning Job";
            OpenModal();
        }
        else if (e.CommandName == "DeleteJob")
        {
            DeleteJob(jobId);
            BindStats();
            BindGrid();
        }
    }
    private void SetDropDownValueSafe(DropDownList ddl, object value)
    {
        if (value == DBNull.Value || value == null)
            return;

        ListItem item = ddl.Items.FindByValue(value.ToString());

        if (item != null)
        {
            ddl.ClearSelection();
            item.Selected = true;
        }
    }
    private void LoadJobIntoForm(int jobId)
    {
        BindLookups();
        const string sql = @"SELECT * FROM CmsJob WHERE JobID = @JobID";

        using (var con = new SqlConnection(connStr))
        using (var cmd = new SqlCommand(sql, con))
        {
            cmd.Parameters.AddWithValue("@JobID", jobId);
            con.Open();

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {

                    hfJobID.Value = rdr["JobID"].ToString();

                    litJobCode.Text = FormatJobCode(rdr["JobID"]);

                    if (rdr["ScheduleID"] != DBNull.Value)
                    {
                        LockScheduleFields(true);
                    }
                    else
                    {
                        LockScheduleFields(false);
                    }

                    SetDropDownValueSafe(ddlSection, rdr["SectionID"]);

                    SetDropDownValueSafe(ddlCleaningType, rdr["CleaningType"]);

                    SetDropDownValueSafe(ddlPriority, rdr["Priority"]);

                    SetDropDownValueSafe(ddlStatus, rdr["Status"]);

                    SetDropDownValueSafe(ddlFrequency, rdr["Frequency"]);

                    SetDropDownValueSafe(ddlAssignmentType, rdr["AssignmentType"]);


                    SetDropDownValueSafe(ddlTeam, rdr["TeamID"]);


                    if (rdr["TeamID"] != DBNull.Value)
                    {
                        LoadTeamSupervisors(rdr["TeamID"].ToString());

                        if (rdr["TeamSupervisorID"] != DBNull.Value)
                        {
                            SetDropDownValueSafe(
                                ddlTeamSupervisor,
                                rdr["TeamSupervisorID"]
                            );
                        }
                    }


                    SetDropDownValueSafe(ddlStaff, rdr["StaffID"]);

                    SetDropDownValueSafe(ddlSchedule, rdr["ScheduleID"]);






                    txtDescription.Text =
                        rdr["Description"] == DBNull.Value
                        ? ""
                        : rdr["Description"].ToString();


                    txtScheduledDate.Text =
                        rdr["ScheduledDate"] == DBNull.Value
                        ? ""
                        : Convert.ToDateTime(rdr["ScheduledDate"])
                            .ToString("yyyy-MM-dd");


                    txtExpectedCompletionDate.Text =
                        rdr["ExpectedCompletionDate"] == DBNull.Value
                        ? ""
                        : Convert.ToDateTime(rdr["ExpectedCompletionDate"])
                            .ToString("yyyy-MM-dd");
                }
            }
        }
    }

    private void LoadTeamSupervisors(string teamId)
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            string sql = @"
        SELECT 
            ts.TeamSupervisorID,
            s.Name + ' (' + t.TeamName + ')' AS DisplayName
        FROM CmsTeamSupervisor ts
        INNER JOIN CmsStaff s 
            ON s.StaffID = ts.StaffID
        INNER JOIN CmsTeam t
            ON t.TeamID = ts.TeamID
        WHERE ts.TeamID = @TeamID";

            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@TeamID", teamId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            da.Fill(dt);

            ddlTeamSupervisor.Items.Clear();

            ddlTeamSupervisor.DataSource = dt;
            ddlTeamSupervisor.DataTextField = "DisplayName";
            ddlTeamSupervisor.DataValueField = "TeamSupervisorID";
            ddlTeamSupervisor.DataBind();

            ddlTeamSupervisor.Items.Insert(
                0,
                new ListItem("-- Select Supervisor --", "")
            );
        }
    }

    protected void ddlSchedule_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(ddlSchedule.SelectedValue))
        {
            int scheduleId = Convert.ToInt32(ddlSchedule.SelectedValue);

            using (var con = new SqlConnection(connStr))
            using (var cmd = new SqlCommand(
                @"SELECT 
                SectionID, 
                CleaningType, 
                Frequency, 
                StartDate, 
                Description 
              FROM CmsCleaningSchedule 
              WHERE ScheduleID = @ScheduleID", con))
            {
                cmd.Parameters.AddWithValue("@ScheduleID", scheduleId);

                con.Open();

                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        
                        SetDropDownValueSafe(ddlSection, rdr["SectionID"]);

                        SetDropDownValueSafe(
                            ddlCleaningType,
                            rdr["CleaningType"]
                        );

                        SetDropDownValueSafe(
                            ddlFrequency,
                            rdr["Frequency"]
                        );


                        if (rdr["StartDate"] != DBNull.Value)
                        {
                            txtScheduledDate.Text =
                                Convert.ToDateTime(rdr["StartDate"])
                                .ToString("yyyy-MM-dd");
                        }


                     
                        txtDescription.Text =
                            rdr["Description"] == DBNull.Value
                            ? ""
                            : rdr["Description"].ToString();
                    }
                }
            }


           
            LockScheduleFields(true);


            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "lockScheduleFields",
                "lockScheduleFields(true);",
                true
            );
        }
        else
        {
            LockScheduleFields(false);


            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "unlockScheduleFields",
                "lockScheduleFields(false);",
                true
            );
        }


        OpenModal();
    }


    protected void btnSaveRecord_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) { OpenModal(); return; }

        int jobId;
        int.TryParse(hfJobID.Value, out jobId);

        litError.Text = "JobID = " + jobId;

        int currentUserId = 1;

        try
        {
            using (var con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmd;

                if (jobId == 0)
                {
                    cmd = new SqlCommand(@"
                        INSERT INTO CmsJob
                            (SectionID, ScheduleID, Description, ScheduledDate, ExpectedCompletionDate,
                             Status, AssignmentType, TeamID, StaffID, TeamSupervisorID, CleaningType, Priority,
                             CreatedDate, CreatedBy, Frequency)
                        VALUES
                            (@SectionID, @ScheduleID, @Description, @ScheduledDate, @ExpectedCompletionDate,
                             @Status, @AssignmentType, @TeamID, @StaffID, @TeamSupervisorID, @CleaningType, @Priority,
                             GETDATE(), @CreatedBy, @Frequency)", con);
                    cmd.Parameters.AddWithValue("@CreatedBy", currentUserId);
                }
                else
                {
                    cmd = new SqlCommand(@"
                        UPDATE CmsJob SET
                            SectionID = @SectionID, ScheduleID = @ScheduleID,
                            Description = @Description, ScheduledDate = @ScheduledDate,
                            ExpectedCompletionDate = @ExpectedCompletionDate, Status = @Status,
                            AssignmentType = @AssignmentType, TeamID = @TeamID, StaffID = @StaffID,
                            TeamSupervisorID = @TeamSupervisorID, CleaningType = @CleaningType,
                            Priority = @Priority, Frequency = @Frequency
                        WHERE JobID = @JobID", con);
                    cmd.Parameters.AddWithValue("@JobID", jobId);
                }

                bool isTeam = ddlAssignmentType.SelectedValue == "Team";

                cmd.Parameters.AddWithValue("@SectionID", Convert.ToInt32(ddlSection.SelectedValue));
                cmd.Parameters.AddWithValue("@ScheduleID", string.IsNullOrEmpty(ddlSchedule.SelectedValue) ? (object)DBNull.Value : Convert.ToInt32(ddlSchedule.SelectedValue));

                cmd.Parameters.AddWithValue("@Description", string.IsNullOrEmpty(txtDescription.Text) ? (object)DBNull.Value : txtDescription.Text.Trim());
                cmd.Parameters.AddWithValue("@ScheduledDate", string.IsNullOrEmpty(txtScheduledDate.Text) ? (object)DBNull.Value : Convert.ToDateTime(txtScheduledDate.Text));
                cmd.Parameters.AddWithValue("@ExpectedCompletionDate", string.IsNullOrEmpty(txtExpectedCompletionDate.Text) ? (object)DBNull.Value : Convert.ToDateTime(txtExpectedCompletionDate.Text));
                cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);
                cmd.Parameters.AddWithValue("@AssignmentType", ddlAssignmentType.SelectedValue);
                cmd.Parameters.AddWithValue("@TeamID", isTeam && !string.IsNullOrEmpty(ddlTeam.SelectedValue) ? (object)Convert.ToInt32(ddlTeam.SelectedValue) : DBNull.Value);
                cmd.Parameters.AddWithValue("@StaffID", !isTeam && !string.IsNullOrEmpty(ddlStaff.SelectedValue) ? (object)Convert.ToInt32(ddlStaff.SelectedValue) : DBNull.Value);
                cmd.Parameters.AddWithValue("@TeamSupervisorID", string.IsNullOrEmpty(ddlTeamSupervisor.SelectedValue) ? (object)DBNull.Value : Convert.ToInt32(ddlTeamSupervisor.SelectedValue));
                cmd.Parameters.AddWithValue("@CleaningType", string.IsNullOrEmpty(ddlCleaningType.SelectedValue) ? (object)DBNull.Value : ddlCleaningType.SelectedValue);
                cmd.Parameters.AddWithValue("@Priority", ddlPriority.SelectedValue);
                cmd.Parameters.AddWithValue("@Frequency", ddlFrequency.SelectedValue);

                using (cmd) { cmd.ExecuteNonQuery(); }
            }

            CloseModal();
            BindStats();
            BindGrid();
        }
        catch (Exception ex)
        {
            litError.Text = "<div style='color:#DC2626; margin-top:8px; font-size:13px;'>Could not save the record: " + ex.Message + "</div>";
            OpenModal();
        }
    }

    private void DeleteJob(int jobId)
    {
        using (var con = new SqlConnection(connStr))
        using (var cmd = new SqlCommand("DELETE FROM CmsJob WHERE JobID = @JobID", con))
        {
            cmd.Parameters.AddWithValue("@JobID", jobId);
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

    private void ResetForm()
    {
        hfJobID.Value = "0";
        litJobCode.Text = "";

        ddlSection.ClearSelection();
        ddlCleaningType.ClearSelection();
        ddlPriority.ClearSelection();
        ddlStatus.ClearSelection();
        ddlFrequency.ClearSelection();
        ddlAssignmentType.ClearSelection();

        ddlTeam.ClearSelection();
        ddlTeamSupervisor.ClearSelection();
        ddlStaff.ClearSelection();

        txtDescription.Text = "";
        txtScheduledDate.Text = "";
        txtExpectedCompletionDate.Text = "";

        LockScheduleFields(false);
    }

    #endregion

    #region Modal open/close (CSS-class toggle, no JS framework required)

    private void OpenModal()
    {
        pnlModalOverlay.CssClass = "cj-modal-overlay show";
    }

    private void CloseModal()
    {
        pnlModalOverlay.CssClass = "cj-modal-overlay";
        ResetForm();
    }

    #endregion
}