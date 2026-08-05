using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CleaningManagement_Apps_CleaningJobs : System.Web.UI.Page
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
            BindTeamDropDown();
            BindStaffDropDown();
            BindJobsGrid();
        }
    }

    private void BindSectionDropDown()
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        {
            string query = "SELECT SectionID, SectionName FROM CmsSection ORDER BY SectionName";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlSection.DataSource = dt;
                    ddlSection.DataTextField = "SectionName";
                    ddlSection.DataValueField = "SectionID";
                    ddlSection.DataBind();

                    ddlSection.Items.Insert(0, new ListItem("-- Select Section --", "0"));
                }
            }
        }
    }


    private void BindTeamDropDown()
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        {
            string query = "SELECT TeamID, TeamName FROM CmsTeam ORDER BY TeamName";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlTeam.DataSource = dt;
                    ddlTeam.DataTextField = "TeamName";
                    ddlTeam.DataValueField = "TeamID";
                    ddlTeam.DataBind();

                    ddlTeam.Items.Insert(0, new ListItem("-- Select Team --", "0"));
                }
            }
        }
    }

  
    private void BindStaffDropDown()
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        {
            string query = "SELECT StaffID, Name FROM CmsStaff ORDER BY Name";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlStaff.DataSource = dt;
                    ddlStaff.DataTextField = "Name";
                    ddlStaff.DataValueField = "StaffID";
                    ddlStaff.DataBind();

                    ddlStaff.Items.Insert(0, new ListItem("-- Select Staff --", "0"));
                }
            }
        }
    }


    private void BindJobsGrid()
    {
        string query = @"
        SELECT 
            j.JobID,
            s.SectionName,
            j.JobType,
            j.Description,
            j.ScheduledDate,
            j.ExpectedCompletionDate,
            ISNULL(t.TeamName, st.Name) AS AssignedTo,
            j.Status
        FROM CmsJob j
        INNER JOIN CmsSection s 
            ON j.SectionID = s.SectionID
        LEFT JOIN CmsTeam t 
            ON j.TeamID = t.TeamID
        LEFT JOIN CmsStaff st 
            ON j.StaffID = st.StaffID
        ORDER BY j.JobID DESC";

        DataTable dt = new DataTable();

        using (SqlConnection con = new SqlConnection(ConnStr))
        {
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
        }

        gvJobs.DataSource = dt;
        gvJobs.DataBind();
    }


    private void BindJobsGridFromDatabase()
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        {
            string query = @"
                SELECT
                    j.JobID,
                    s.SectionName,
                    j.JobType,
                    j.Description,
                    j.ScheduledDate,
                    j.ExpectedCompletionDate,
                    j.Status,
                    CASE
                        WHEN j.AssignmentType = 'Team' THEN t.TeamName
                        WHEN j.AssignmentType = 'Individual' THEN st.Name
                        ELSE NULL
                    END AS AssignedTo
                FROM CmsJob j
                INNER JOIN CmsSection s ON j.SectionID = s.SectionID
                LEFT JOIN CmsTeam t ON j.TeamID = t.TeamID
                LEFT JOIN CmsStaff st ON j.StaffID = st.StaffID
                ORDER BY j.JobID DESC";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvJobs.DataSource = dt;
                    gvJobs.DataBind();
                }
            }
        }
    }


    protected void btnSaveJob_Click(object sender, EventArgs e)
    {
        int sectionId = Convert.ToInt32(ddlSection.SelectedValue);

        if (sectionId == 0)
        {
            lblMessage.Text = "Please select a section before saving.";
            lblMessage.ForeColor = System.Drawing.Color.Red;
            return;
        }

        string jobType = ddlJobType.SelectedValue;
        string description = txtDescription.Text.Trim();
        string status = ddlStatus.SelectedValue;

        string assignmentType = ddlAssignmentType.SelectedValue; // "", "Team", or "Individual"

        object teamId = DBNull.Value;
        object staffId = DBNull.Value;

        if (assignmentType == "Team")
        {
            int selectedTeamId = Convert.ToInt32(ddlTeam.SelectedValue);
            if (selectedTeamId == 0)
            {
                lblMessage.Text = "Please select a team to assign this job to.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }
            teamId = selectedTeamId;
        }
        else if (assignmentType == "Individual")
        {
            int selectedStaffId = Convert.ToInt32(ddlStaff.SelectedValue);
            if (selectedStaffId == 0)
            {
                lblMessage.Text = "Please select a staff member to assign this job to.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }
            staffId = selectedStaffId;
        }

        object assignmentTypeValue = string.IsNullOrEmpty(assignmentType) ? (object)DBNull.Value : assignmentType;

        object scheduledDate = string.IsNullOrEmpty(txtScheduledDate.Text)
            ? (object)DBNull.Value
            : Convert.ToDateTime(txtScheduledDate.Text);

        object expectedCompletionDate = string.IsNullOrEmpty(txtExpectedCompletionDate.Text)
            ? (object)DBNull.Value
            : Convert.ToDateTime(txtExpectedCompletionDate.Text);

        int jobId = Convert.ToInt32(hdnJobID.Value);

        using (SqlConnection con = new SqlConnection(ConnStr))
        {
            SqlCommand cmd;

            if (jobId == 0)
            {
                string insertQuery = @"
                    INSERT INTO CmsJob
                        (SectionID, JobType, Description, ScheduledDate, ExpectedCompletionDate, Status, AssignmentType, TeamID, StaffID)
                    VALUES
                        (@SectionID, @JobType, @Description, @ScheduledDate, @ExpectedCompletionDate, @Status, @AssignmentType, @TeamID, @StaffID)";

                cmd = new SqlCommand(insertQuery, con);
            }
            else
            {
                string updateQuery = @"
                    UPDATE CmsJob
                    SET SectionID = @SectionID,
                        JobType = @JobType,
                        Description = @Description,
                        ScheduledDate = @ScheduledDate,
                        ExpectedCompletionDate = @ExpectedCompletionDate,
                        Status = @Status,
                        AssignmentType = @AssignmentType,
                        TeamID = @TeamID,
                        StaffID = @StaffID
                    WHERE JobID = @JobID";

                cmd = new SqlCommand(updateQuery, con);
                cmd.Parameters.AddWithValue("@JobID", jobId);
            }

            cmd.Parameters.AddWithValue("@SectionID", sectionId);
            cmd.Parameters.AddWithValue("@JobType", jobType);
            cmd.Parameters.AddWithValue("@Description",
                string.IsNullOrEmpty(description) ? (object)DBNull.Value : description);
            cmd.Parameters.AddWithValue("@ScheduledDate", scheduledDate);
            cmd.Parameters.AddWithValue("@ExpectedCompletionDate", expectedCompletionDate);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@AssignmentType", assignmentTypeValue);
            cmd.Parameters.AddWithValue("@TeamID", teamId);
            cmd.Parameters.AddWithValue("@StaffID", staffId);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        ResetForm();
        BindJobsGrid();

        lblMessage.ForeColor = System.Drawing.Color.Green;
        lblMessage.Text = (jobId == 0) ? "Job created successfully." : "Job updated successfully.";


        ScriptManager.RegisterStartupScript(this, this.GetType(), "hideFormAfterSave",
            "toggleJobForm(false);", true);
    }


    
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ResetForm();

        lblMessage.Text = "";

        ScriptManager.RegisterStartupScript(this, this.GetType(),
            "hideFormOnCancel",
            "toggleJobForm(false);", true);
    }
    protected void gvJobs_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "EditJob")
        {
            int jobId = Convert.ToInt32(e.CommandArgument);

            LoadJobForEdit(jobId);

            btnSaveJob.Text = "Update Job";
            btnSaveJob.CssClass = "btn-update";

            ScriptManager.RegisterStartupScript(this, this.GetType(),
                "showFormForEdit",
                "toggleJobForm(true); toggleAssignmentFields();", true);
        }
    }
    private void LoadJobForEdit(int jobId)
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        {
            string query = @"
                SELECT JobID, SectionID, JobType, Description, ScheduledDate, ExpectedCompletionDate, Status,
                       AssignmentType, TeamID, StaffID
                FROM CmsJob
                WHERE JobID = @JobID";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@JobID", jobId);
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        hdnJobID.Value = reader["JobID"].ToString();
                        ddlSection.SelectedValue = reader["SectionID"].ToString();
                        ddlJobType.SelectedValue = reader["JobType"].ToString();
                        txtDescription.Text = reader["Description"] == DBNull.Value ? "" : reader["Description"].ToString();
                        txtScheduledDate.Text = reader["ScheduledDate"] == DBNull.Value
                            ? "" : Convert.ToDateTime(reader["ScheduledDate"]).ToString("yyyy-MM-dd");
                        txtExpectedCompletionDate.Text = reader["ExpectedCompletionDate"] == DBNull.Value
                            ? "" : Convert.ToDateTime(reader["ExpectedCompletionDate"]).ToString("yyyy-MM-dd");
                        ddlStatus.SelectedValue = reader["Status"].ToString();
 
                        string assignmentType = reader["AssignmentType"] == DBNull.Value ? "" : reader["AssignmentType"].ToString();
                        ddlAssignmentType.SelectedValue = assignmentType;

                        if (assignmentType == "Team" && reader["TeamID"] != DBNull.Value)
                        {
                            ddlTeam.SelectedValue = reader["TeamID"].ToString();
                        }
                        else if (assignmentType == "Individual" && reader["StaffID"] != DBNull.Value)
                        {
                            ddlStaff.SelectedValue = reader["StaffID"].ToString();
                        }

                        lblFormHeading.Text = "Edit Job #" + jobId;
                    }
                }
            }
        }
    }

  
       private void ResetForm()
    {
        hdnJobID.Value = "0";

        ddlSection.SelectedIndex = 0;
        ddlJobType.SelectedIndex = 0;
        txtDescription.Text = "";
        txtScheduledDate.Text = "";
        txtExpectedCompletionDate.Text = "";
        ddlStatus.SelectedIndex = 0;
        ddlAssignmentType.SelectedIndex = 0;
        ddlTeam.SelectedIndex = 0;
        ddlStaff.SelectedIndex = 0;

        lblFormHeading.Text = "Create New Job";

        btnSaveJob.Text = "Create Job";
        btnSaveJob.CssClass = "btn-create";
    }
}