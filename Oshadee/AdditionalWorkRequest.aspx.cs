using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CleaningManagement_AdditionalWorkRequest : System.Web.UI.Page
{
    private string connStr = ConfigurationManager.ConnectionStrings["CmsConnectionString"].ConnectionString;

    private const string ROLE_NAME_STAFF = "Staff";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadFilterSectionDropdown();
            BindGrid();
        }
    }

    #region Request Code helpers (REQ-001 format)

    protected string FormatRequestCode(object requestId)
    {
        if (requestId == null || requestId == DBNull.Value) return "";
        int id;
        if (!int.TryParse(requestId.ToString(), out id)) return "";
        return "REQ-" + id.ToString("D3");
    }


    private string GetNextRequestCode()
    {
        using (SqlConnection con = new SqlConnection(connStr))
        using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(RequestID), 0) + 1 FROM CmsAdditionalWorkRequest", con))
        {
            con.Open();
            int nextId = Convert.ToInt32(cmd.ExecuteScalar());
            return "REQ-" + nextId.ToString("D3");
        }
    }

    #endregion

    #region Filters

    private void LoadFilterSectionDropdown()
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            string sql = "SELECT SectionID, SectionName FROM CmsSection ORDER BY SectionName";
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(rdr);

            ddlFilterSection.DataSource = dt;
            ddlFilterSection.DataTextField = "SectionName";
            ddlFilterSection.DataValueField = "SectionID";
            ddlFilterSection.DataBind();
            ddlFilterSection.Items.Insert(0, new ListItem("All Sections", ""));
        }
    }

    protected void ddlFilterSection_SelectedIndexChanged(object sender, EventArgs e) { BindGrid(); }
    protected void ddlFilterPriority_SelectedIndexChanged(object sender, EventArgs e) { BindGrid(); }
    protected void ddlFilterStatus_SelectedIndexChanged(object sender, EventArgs e) { BindGrid(); }
    protected void ddlFilterAssignType_SelectedIndexChanged(object sender, EventArgs e) { BindGrid(); }

    protected void btnClearFilters_Click(object sender, EventArgs e)
    {
        ddlFilterSection.SelectedValue = "";
        ddlFilterPriority.SelectedValue = "";
        ddlFilterStatus.SelectedValue = "";
        ddlFilterAssignType.SelectedValue = "";
        BindGrid();
    }

    #endregion

    #region Dropdown Loading

    private void LoadFormDropdowns()
    {
        LoadSectionDropdown();
        LoadTeamDropdown();
        LoadAssignedToDropdown();
        LoadSupervisorDropdowns();
        ResetTeamSupervisorDropdown();
    }

    private void LoadSectionDropdown()
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            string sql = "SELECT SectionID, SectionName FROM CmsSection ORDER BY SectionName";
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(rdr);

            ddlSection.DataSource = dt;
            ddlSection.DataTextField = "SectionName";
            ddlSection.DataValueField = "SectionID";
            ddlSection.DataBind();
            ddlSection.Items.Insert(0, new ListItem("-- Select Section --", ""));
        }
    }

    private void LoadTeamDropdown()
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            string sql = "SELECT TeamID, TeamName FROM CmsTeam ORDER BY TeamName";
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(rdr);

            ddlTeam.DataSource = dt;
            ddlTeam.DataTextField = "TeamName";
            ddlTeam.DataValueField = "TeamID";
            ddlTeam.DataBind();
            ddlTeam.Items.Insert(0, new ListItem("-- Select Team --", ""));
        }
    }

    private void LoadAssignedToDropdown()
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            string sql = @"
                SELECT s.StaffID, s.Name
                FROM CmsStaff s
                INNER JOIN CmsRole r ON s.RoleID = r.RoleID
                WHERE r.RoleName = @RoleName
                ORDER BY s.Name";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@RoleName", ROLE_NAME_STAFF);
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(rdr);

            ddlAssignedTo.DataSource = dt;
            ddlAssignedTo.DataTextField = "Name";
            ddlAssignedTo.DataValueField = "StaffID";
            ddlAssignedTo.DataBind();
            ddlAssignedTo.Items.Insert(0, new ListItem("-- Select Staff --", ""));
        }
    }

    private void LoadSupervisorDropdowns()
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            string sql = @"
                SELECT DISTINCT st.StaffID, st.Name
                FROM CmsTeamSupervisor ts
                INNER JOIN CmsStaff st ON ts.StaffID = st.StaffID
                ORDER BY st.Name";
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(rdr);

            ddlRequestedBy.DataSource = dt;
            ddlRequestedBy.DataTextField = "Name";
            ddlRequestedBy.DataValueField = "StaffID";
            ddlRequestedBy.DataBind();
            ddlRequestedBy.Items.Insert(0, new ListItem("-- Select Supervisor --", ""));

            ddlApprovedBy.DataSource = dt;
            ddlApprovedBy.DataTextField = "Name";
            ddlApprovedBy.DataValueField = "StaffID";
            ddlApprovedBy.DataBind();
            ddlApprovedBy.Items.Insert(0, new ListItem("-- Select Supervisor --", ""));
        }
    }

    private void LoadTeamSupervisorDropdown(int teamId, string selectedTeamSupervisorId)
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            string sql = @"
                SELECT ts.TeamSupervisorID, st.Name AS SupervisorName
                FROM CmsTeamSupervisor ts
                INNER JOIN CmsStaff st ON ts.StaffID = st.StaffID
                WHERE ts.TeamID = @TeamID
                ORDER BY st.Name";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@TeamID", teamId);
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(rdr);

            ddlTeamSupervisor.Items.Clear();
            ddlTeamSupervisor.DataSource = dt;
            ddlTeamSupervisor.DataTextField = "SupervisorName";
            ddlTeamSupervisor.DataValueField = "TeamSupervisorID";
            ddlTeamSupervisor.DataBind();
            ddlTeamSupervisor.Items.Insert(0, new ListItem("-- Select Supervisor --", ""));

            if (!string.IsNullOrEmpty(selectedTeamSupervisorId))
            {
                ListItem match = ddlTeamSupervisor.Items.FindByValue(selectedTeamSupervisorId);
                if (match != null) ddlTeamSupervisor.SelectedValue = selectedTeamSupervisorId;
            }
        }
    }

    private void ResetTeamSupervisorDropdown()
    {
        ddlTeamSupervisor.Items.Clear();
        ddlTeamSupervisor.Items.Add(new ListItem("-- Select Team First --", ""));
    }

    #endregion

    #region Grid Binding

    private void BindGrid()
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            string sql = @"
                SELECT
                    r.RequestID,
                    s.SectionName,
                    r.Description,
                    r.RequestType,
                    r.Priority,
                    r.Status,
                    r.TimeTaken,
                    reqBy.Name AS RequestedByName,
                    apprBy.Name AS ApprovedByName,
                    asgTo.Name AS AssignedToName,
                    t.TeamName AS TeamName,
                    r.RequestDate,
                    r.CompletedDate
                FROM CmsAdditionalWorkRequest r
                LEFT JOIN CmsSection s ON r.SectionID = s.SectionID
                LEFT JOIN CmsStaff reqBy ON r.RequestedByID = reqBy.StaffID
                LEFT JOIN CmsStaff apprBy ON r.ApprovedByID = apprBy.StaffID
                LEFT JOIN CmsStaff asgTo ON r.AssignedToID = asgTo.StaffID
                LEFT JOIN CmsTeam t ON r.TeamID = t.TeamID
                WHERE (@SectionID = '' OR r.SectionID = @SectionIDVal)
                  AND (@Priority = '' OR r.Priority = @Priority)
                  AND (@Status = '' OR r.Status = @Status)
                  AND (@AssignType = ''
                       OR (@AssignType = 'Individual' AND r.AssignedToID IS NOT NULL)
                       OR (@AssignType = 'Team' AND r.TeamID IS NOT NULL))
                ORDER BY r.RequestID DESC";

            SqlCommand cmd = new SqlCommand(sql, con);

            string sectionFilter = ddlFilterSection.SelectedValue;
            cmd.Parameters.AddWithValue("@SectionID", sectionFilter);
            cmd.Parameters.AddWithValue("@SectionIDVal", string.IsNullOrEmpty(sectionFilter) ? 0 : Convert.ToInt32(sectionFilter));
            cmd.Parameters.AddWithValue("@Priority", ddlFilterPriority.SelectedValue ?? "");
            cmd.Parameters.AddWithValue("@Status", ddlFilterStatus.SelectedValue ?? "");
            cmd.Parameters.AddWithValue("@AssignType", ddlFilterAssignType.SelectedValue ?? "");

            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(rdr);

            gvRequests.DataSource = dt;
            gvRequests.DataBind();
        }
    }

    #endregion

    #region Form show / hide (modal, same approach as FeedbackComplaint)

    protected void btnShowForm_Click(object sender, EventArgs e)
    {
        ClearForm();
        litFormTitle.Text = "Register New Work Request";
        LoadFormDropdowns();

        
        litRequestCode.Text = GetNextRequestCode();

        ScriptManager.RegisterStartupScript(this, GetType(), "openModal", "openAwrModal();", true);
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ClearForm();
    }

    private void ClearForm()
    {
        hfRequestID.Value = "0";
        litRequestCode.Text = "";
        txtDescription.Text = "";
        txtRequestType.Text = "";
        txtRemarks.Text = "";
        txtRequestDate.Text = "";
        txtCompletedDate.Text = "";
        txtTimeTakenOther.Text = "";
        ddlTimeTaken.SelectedValue = "";
        ddlPriority.SelectedValue = "Normal";
        ddlStatus.SelectedValue = "Pending";
        rbAssignIndividual.Checked = true;
        rbAssignTeam.Checked = false;
    }
    protected void ddlTeam_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(ddlTeam.SelectedValue))
        {
            LoadTeamSupervisorDropdown(Convert.ToInt32(ddlTeam.SelectedValue), null);
        }
        else
        {
            ResetTeamSupervisorDropdown();
        }


        ScriptManager.RegisterStartupScript(this, GetType(), "openModal", "openAwrModal();", true);
    }

    #endregion

    #region Save (Insert / Update)

    protected void btnSave_Click(object sender, EventArgs e)
    {
        bool isIndividual = rbAssignIndividual.Checked;

        if (string.IsNullOrWhiteSpace(txtDescription.Text) || string.IsNullOrEmpty(ddlSection.SelectedValue))
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "err",
                "alert('Section and Description are required.'); openAwrModal();", true);
            return;
        }

        if (isIndividual && string.IsNullOrEmpty(ddlAssignedTo.SelectedValue))
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "err",
                "alert('Please select a staff member to assign this request to.'); openAwrModal();", true);
            return;
        }

        if (!isIndividual && string.IsNullOrEmpty(ddlTeam.SelectedValue))
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "err",
                "alert('Please select a team to assign this request to.'); openAwrModal();", true);
            return;
        }

        if (string.IsNullOrEmpty(ddlRequestedBy.SelectedValue))
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "err",
                "alert('Requested By (Supervisor) is required.'); openAwrModal();", true);
            return;
        }

        object timeTakenValue;
        string timeError;

        if (!TryResolveTimeTaken(out timeTakenValue, out timeError))
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "err",
                "alert('" + timeError.Replace("'", "\\'") + "'); openAwrModal();", true);
            return;
        }

        int requestId = Convert.ToInt32(hfRequestID.Value);

        object assignedToParam = isIndividual ? (object)Convert.ToInt32(ddlAssignedTo.SelectedValue) : DBNull.Value;
        object teamParam = isIndividual ? DBNull.Value : (object)Convert.ToInt32(ddlTeam.SelectedValue);
        object teamSupervisorParam = isIndividual ? DBNull.Value : ToDbValueOrNull(ddlTeamSupervisor.SelectedValue);

        using (SqlConnection con = new SqlConnection(connStr))
        {
            con.Open();
            SqlCommand cmd;

            if (requestId == 0)
            {
                string sql = @"
                    INSERT INTO CmsAdditionalWorkRequest
                        (SectionID, ApprovedByID, AssignedToID, Description, TimeTaken, Status,
                         TeamID, RequestedByID, RequestType, Priority, RequestDate, CompletedDate, Remarks, TeamSupervisorID)
                    VALUES
                        (@SectionID, @ApprovedByID, @AssignedToID, @Description, @TimeTaken, @Status,
                         @TeamID, @RequestedByID, @RequestType, @Priority, @RequestDate, @CompletedDate, @Remarks, @TeamSupervisorID)";
                cmd = new SqlCommand(sql, con);
            }
            else
            {
                string sql = @"
                    UPDATE CmsAdditionalWorkRequest SET
                        SectionID = @SectionID,
                        ApprovedByID = @ApprovedByID,
                        AssignedToID = @AssignedToID,
                        Description = @Description,
                        TimeTaken = @TimeTaken,
                        Status = @Status,
                        TeamID = @TeamID,
                        RequestedByID = @RequestedByID,
                        RequestType = @RequestType,
                        Priority = @Priority,
                        RequestDate = @RequestDate,
                        CompletedDate = @CompletedDate,
                        Remarks = @Remarks,
                        TeamSupervisorID = @TeamSupervisorID
                    WHERE RequestID = @RequestID";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@RequestID", requestId);
            }

            cmd.Parameters.AddWithValue("@SectionID", Convert.ToInt32(ddlSection.SelectedValue));
            cmd.Parameters.AddWithValue("@ApprovedByID", ToDbValueOrNull(ddlApprovedBy.SelectedValue));
            cmd.Parameters.AddWithValue("@AssignedToID", assignedToParam);
            cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
            cmd.Parameters.AddWithValue("@TimeTaken", timeTakenValue);
            cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);
            cmd.Parameters.AddWithValue("@TeamID", teamParam);
            cmd.Parameters.AddWithValue("@RequestedByID", Convert.ToInt32(ddlRequestedBy.SelectedValue));
            cmd.Parameters.AddWithValue("@RequestType", string.IsNullOrWhiteSpace(txtRequestType.Text) ? (object)DBNull.Value : txtRequestType.Text.Trim());
            cmd.Parameters.AddWithValue("@Priority", ddlPriority.SelectedValue);
            cmd.Parameters.AddWithValue("@RequestDate",
                string.IsNullOrEmpty(txtRequestDate.Text) ? (object)DBNull.Value : Convert.ToDateTime(txtRequestDate.Text));
            cmd.Parameters.AddWithValue("@CompletedDate",
                string.IsNullOrEmpty(txtCompletedDate.Text) ? (object)DBNull.Value : Convert.ToDateTime(txtCompletedDate.Text));
            cmd.Parameters.AddWithValue("@Remarks", string.IsNullOrWhiteSpace(txtRemarks.Text) ? (object)DBNull.Value : txtRemarks.Text.Trim());
            cmd.Parameters.AddWithValue("@TeamSupervisorID", teamSupervisorParam);

            cmd.ExecuteNonQuery();
        }

        BindGrid();
        ClearForm();
        ScriptManager.RegisterStartupScript(this, GetType(), "closeModal", "closeAwrModal();", true);
    }

    private bool TryResolveTimeTaken(out object value, out string error)
    {
        error = "";
        string selected = ddlTimeTaken.SelectedValue;

        if (string.IsNullOrEmpty(selected))
        {
            value = DBNull.Value;
            return true;
        }

        if (selected == "other")
        {
            if (string.IsNullOrWhiteSpace(txtTimeTakenOther.Text))
            {
                value = null;
                error = "Please type the time taken in hours, or choose a preset value.";
                return false;
            }
            decimal customHours;

            if (!decimal.TryParse(txtTimeTakenOther.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out customHours))
            {
                value = null;
                error = "Time taken must be a valid number (hours).";
                return false;
            }
        }

        value = decimal.Parse(selected, CultureInfo.InvariantCulture);
        return true;
    }

    private object ToDbValueOrNull(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return DBNull.Value;
        return value;
    }

    #endregion

    #region Edit / Delete

    protected void gvRequests_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int requestId = Convert.ToInt32(e.CommandArgument);

        if (e.CommandName == "EditRequest")
        {
            LoadRequestForEdit(requestId);
            litFormTitle.Text = "Edit Work Request";
            ScriptManager.RegisterStartupScript(this, GetType(), "openModal", "openAwrModal();", true);
        }
        else if (e.CommandName == "DeleteRequest")
        {
            DeleteRequest(requestId);
            BindGrid();
        }
    }

    private void LoadRequestForEdit(int requestId)
    {
        LoadFormDropdowns();

        using (SqlConnection con = new SqlConnection(connStr))
        {
            string sql = "SELECT * FROM CmsAdditionalWorkRequest WHERE RequestID = @RequestID";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@RequestID", requestId);
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                hfRequestID.Value = rdr["RequestID"].ToString();

                litRequestCode.Text = FormatRequestCode(rdr["RequestID"]);

                ddlSection.SelectedValue = rdr["SectionID"].ToString();

                bool hasTeam = rdr["TeamID"] != DBNull.Value;
                rbAssignIndividual.Checked = !hasTeam;
                rbAssignTeam.Checked = hasTeam;

                if (hasTeam)
                {
                    int teamId = Convert.ToInt32(rdr["TeamID"]);
                    ddlTeam.SelectedValue = teamId.ToString();

                    string teamSupervisorId = rdr["TeamSupervisorID"] != DBNull.Value
                        ? rdr["TeamSupervisorID"].ToString()
                        : null;

                    LoadTeamSupervisorDropdown(teamId, teamSupervisorId);
                }
                else if (rdr["AssignedToID"] != DBNull.Value)
                {
                    ddlAssignedTo.SelectedValue = rdr["AssignedToID"].ToString();
                }

                ddlApprovedBy.SelectedValue = rdr["ApprovedByID"] != DBNull.Value ? rdr["ApprovedByID"].ToString() : "";
                ddlRequestedBy.SelectedValue = rdr["RequestedByID"] != DBNull.Value ? rdr["RequestedByID"].ToString() : "";

                txtDescription.Text = rdr["Description"].ToString();
                txtRequestType.Text = rdr["RequestType"] != DBNull.Value ? rdr["RequestType"].ToString() : "";
                txtRemarks.Text = rdr["Remarks"] != DBNull.Value ? rdr["Remarks"].ToString() : "";

                ddlPriority.SelectedValue = rdr["Priority"].ToString();
                ddlStatus.SelectedValue = rdr["Status"].ToString();

                if (rdr["TimeTaken"] != DBNull.Value)
                {
                    decimal hours = Convert.ToDecimal(rdr["TimeTaken"]);
                    string hoursStr = hours.ToString(CultureInfo.InvariantCulture);
                    ListItem match = ddlTimeTaken.Items.FindByValue(hoursStr);
                    if (match != null)
                    {
                        ddlTimeTaken.SelectedValue = hoursStr;
                    }
                    else
                    {
                        ddlTimeTaken.SelectedValue = "other";
                        txtTimeTakenOther.Text = hoursStr;
                    }
                }
                else
                {
                    ddlTimeTaken.SelectedValue = "";
                }

                txtRequestDate.Text = rdr["RequestDate"] != DBNull.Value
                    ? Convert.ToDateTime(rdr["RequestDate"]).ToString("yyyy-MM-dd") : "";
                txtCompletedDate.Text = rdr["CompletedDate"] != DBNull.Value
                    ? Convert.ToDateTime(rdr["CompletedDate"]).ToString("yyyy-MM-dd") : "";
            }
        }
    }

    private void DeleteRequest(int requestId)
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            string sql = "DELETE FROM CmsAdditionalWorkRequest WHERE RequestID = @RequestID";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@RequestID", requestId);
            con.Open();
            cmd.ExecuteNonQuery();
        }
    }

    #endregion
}