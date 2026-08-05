using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CleaningManagement_Masters_Team : System.Web.UI.Page
{
    private string ConnStr
    {
        get { return ConfigurationManager.ConnectionStrings["CmsConnectionString"].ConnectionString; }
    }

    private int CurrentUserID
    {
        get { return Session["UserID"] != null ? Convert.ToInt32(Session["UserID"]) : 0; }
    }

    private bool isAdmin = false;
    private bool isSupervisor = false;
    private int supervisorTeamID = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["UserID"] == null || Session["UserRole"] == null)
        {
            Response.Redirect("~/Login.aspx");
            return;
        }

        string role = Session["UserRole"].ToString();

        if (role != "Admin" && role != "Supervisor")
        {
            Response.Redirect("~/Login.aspx");
            return;
        }

        isAdmin = (role == "Admin");
        isSupervisor = (role == "Supervisor");

        if (isSupervisor)
        {
            SetupSupervisorView();
        }
        else
        {
            SetupAdminView();
        }
    }

    private void SetupAdminView()
    {
        pnlTeamAdminSection.Visible = true;
        lblPageTitle.Text = "Teams";

        if (!IsPostBack)
        {
            LoadSupervisorDropdown();
            BindTeamsGrid();
        }
    }

    private void SetupSupervisorView()
    {
        pnlTeamAdminSection.Visible = false;
        lblPageTitle.Text = "My Team";

        supervisorTeamID = GetSupervisorTeamID(CurrentUserID);

        if (supervisorTeamID == 0)
        {
            pnlNoTeamAssigned.Visible = true;
            pnlTeamWorkspace.Visible = false;
            lblSelectedTeamName.Text = "";
            ScriptManager.RegisterStartupScript(this, GetType(),
                "showMembersNoTeam", "toggleMembersSection(true);", true);
            return;
        }

        pnlNoTeamAssigned.Visible = false;
        pnlTeamWorkspace.Visible = true;

        if (!IsPostBack)
        {
            hdnSelectedTeamID.Value = supervisorTeamID.ToString();
            lblSelectedTeamName.Text = GetTeamName(supervisorTeamID);

            LoadMemberRoleDropdown();
            //Supervisor cannot assign Supervisor job title to member
            ddlJobTitle.Items.Remove(ddlJobTitle.Items.FindByValue("Supervisor"));

            //Remove Supervisor system role from role dropdown
            ListItem supervisorRole = ddlMemberRole.Items.FindByValue(
                GetRoleIDByName("Supervisor").ToString() );
            if (supervisorRole != null)
                ddlMemberRole.Items.Remove(supervisorRole);

            LoadMemberContractorDropdown();

            ResetMemberForm();
            BindStaffGrid(supervisorTeamID);
            BindSectionDropdown();
            BindAssignedSectionsGrid(supervisorTeamID);
        }

        ScriptManager.RegisterStartupScript(this, GetType(),
            "showMembersForSupervisor", "toggleMembersSection(true);", true);
    }

    private int GetSupervisorTeamID(int userId)
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetSupervisorTeamID", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userId);
            con.Open();
            object result = cmd.ExecuteScalar();
            return (result == null || result == DBNull.Value)
                ? 0 : Convert.ToInt32(result);
        }
    }

    private int GetRoleIDByName(string roleName)
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetRoleIDByName", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RoleName", roleName);
            con.Open();
            object result = cmd.ExecuteScalar();
            return result == null ? 0 : Convert.ToInt32(result);
        }
    }

    private string GetTeamName(int teamId)
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetTeamName", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TeamID", teamId);
            con.Open();
            object result = cmd.ExecuteScalar();
            return result == null ? "" : result.ToString();
        }
    }

    private bool CanActOnTeam(int teamId)
    {
        if (isAdmin) return true;
        if (isSupervisor) return teamId != 0 && teamId == supervisorTeamID;
        return false;
    }

  
    // TEAM MASTER — grid, save, cancel, edit (Admin only)
    

    private void BindTeamsGrid()
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetTeams", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            gvTeams.DataSource = dt;
            gvTeams.DataBind();
        }
    }

    protected void btnSaveTeam_Click(object sender, EventArgs e)
    {
        if (!isAdmin) return;

        string teamName = txtTeamName.Text.Trim();

        if (string.IsNullOrEmpty(teamName))
        {
            ShowMessage("Please enter a Team Name before saving.", false);
            ScriptManager.RegisterStartupScript(this, GetType(),
                "keepOpen", "toggleTeamForm(true);", true);
            return;
        }

        int teamId = Convert.ToInt32(hdnTeamID.Value);
        string description = txtDescription.Text.Trim();
        short active = Convert.ToInt16(ddlActive.SelectedValue);
        int supervisorId = Convert.ToInt32(ddlSupervisor.SelectedValue);

        using (SqlConnection con = new SqlConnection(ConnStr))
        {
            SqlCommand cmd;

            if (teamId == 0)
            {
                cmd = new SqlCommand("SP_CMS_InsertTeam", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TeamName", teamName);
                cmd.Parameters.AddWithValue("@Description",
                    string.IsNullOrEmpty(description) ? (object)DBNull.Value : description);
                cmd.Parameters.AddWithValue("@Active", active);
                cmd.Parameters.AddWithValue("@SupervisorID", 
                 supervisorId == 0 ? (object)DBNull.Value : supervisorId);
                cmd.Parameters.AddWithValue("@CreatedBy", CurrentUserID);
            }
            else
            {
                cmd = new SqlCommand("SP_CMS_UpdateTeam", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TeamID", teamId);
                cmd.Parameters.AddWithValue("@TeamName", teamName);
                cmd.Parameters.AddWithValue("@Description",
                    string.IsNullOrEmpty(description) ? (object)DBNull.Value : description);
                cmd.Parameters.AddWithValue("@Active", active);
                cmd.Parameters.AddWithValue("@SupervisorID",
                 supervisorId == 0 ? (Object)DBNull.Value : supervisorId);
                cmd.Parameters.AddWithValue("@UpdatedBy", CurrentUserID);
            }

            con.Open();
            int result = Convert.ToInt32(cmd.ExecuteScalar());

            if (result == -1)
            {
                ShowMessage("A team with this name already exists.", false);
                ScriptManager.RegisterStartupScript(this, GetType(),
                    "keepOpen", "toggleTeamForm(true);", true);
                return;
            }
        }

        string msg = teamId == 0
            ? "Team added successfully."
            : "Team updated successfully.";

        ResetTeamForm();
        BindTeamsGrid();
        ShowMessage(msg, true);

        ScriptManager.RegisterStartupScript(this, GetType(),
            "hideForm", "toggleTeamForm(false);", true);
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        if (!isAdmin) return;

        ResetTeamForm();
        ScriptManager.RegisterStartupScript(this, GetType(),
            "hideForm", "toggleTeamForm(false);", true);
    }

    protected void gvTeams_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (!isAdmin) return;

        if (e.CommandName == "EditTeam")
        {
            int teamId = Convert.ToInt32(e.CommandArgument);
            LoadTeamForEdit(teamId);
            btnSaveTeam.Text = "Update Team";
            btnSaveTeam.CssClass = "btn-update";
            ScriptManager.RegisterStartupScript(this, GetType(),
                "showForm", "toggleTeamForm(true);", true);
        }
        else if (e.CommandName == "ManageMembers")
        {
            string[] parts = e.CommandArgument.ToString().Split('|');
            int teamId = Convert.ToInt32(parts[0]);
            string teamName = parts.Length > 1 ? parts[1] : "";

            hdnSelectedTeamID.Value = teamId.ToString();
            lblSelectedTeamName.Text = teamName;

            pnlNoTeamAssigned.Visible = false;
            pnlTeamWorkspace.Visible = true;

            LoadSupervisorDropdown();
            LoadMemberRoleDropdown();
            LoadMemberContractorDropdown();

            ResetMemberForm();
            BindStaffGrid(teamId);
            BindSectionDropdown();
            BindAssignedSectionsGrid(teamId);
            lblAreaMessage.Text = "";

            ScriptManager.RegisterStartupScript(this, GetType(),
                "showMembers", "toggleMembersSection(true); toggleMemberForm(false);", true);
        }
    }

    private void LoadTeamForEdit(int teamId)
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetTeamByID", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TeamID", teamId);
            con.Open();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    hdnTeamID.Value = reader["TeamID"].ToString();
                    txtTeamName.Text = reader["TeamName"].ToString();
                    txtDescription.Text = reader["Description"] == DBNull.Value
                        ? "" : reader["Description"].ToString();
                    ddlActive.SelectedValue = reader["Active"].ToString();
                    ddlSupervisor.SelectedValue = reader["SupervisorID"] == DBNull.Value
                        ? "0" : reader["SupervisorID"].ToString();
                    lblFormHeading.Text = "Edit Team #" + teamId;
                }
            }
        }
    }

    private void ResetTeamForm()
    {
        hdnTeamID.Value = "0";
        txtTeamName.Text = "";
        txtDescription.Text = "";
        ddlActive.SelectedValue = "0";
        lblFormHeading.Text = "Add New Team";
        btnSaveTeam.Text = "Save Team";
        btnSaveTeam.CssClass = "btn-save";
        lblMessage.Text = "";
    }

    private void ShowMessage(string message, bool success)
    {
        lblMessage.Text = message;
        lblMessage.ForeColor = success
            ? System.Drawing.Color.Green
            : System.Drawing.Color.Red;
    }

    
    // TEAM MEMBERS (CmsStaff)


    private void LoadMemberRoleDropdown()
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetAllRoles", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            ddlMemberRole.DataSource = dt;
            ddlMemberRole.DataTextField = "RoleName";
            ddlMemberRole.DataValueField = "RoleID";
            ddlMemberRole.DataBind();
            ddlMemberRole.Items.Insert(0, new ListItem("-- Select --", "0"));
        }
    }

    private void LoadSupervisorDropdown()
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetSupervisorUsers", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlSupervisor.DataSource = dt;
           ddlSupervisor.DataTextField = "DisplayName";
           ddlSupervisor.DataValueField = "UserID";
            ddlSupervisor.DataBind();
           ddlSupervisor.Items.Insert(0,
                new ListItem("-- None --", "0"));
        }
    }

    private void LoadMemberContractorDropdown()
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetAllContractors", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            ddlMemberContractor.DataSource = dt;
            ddlMemberContractor.DataTextField = "ContractorName";
            ddlMemberContractor.DataValueField = "ContractorID";
            ddlMemberContractor.DataBind();
            ddlMemberContractor.Items.Insert(0, new ListItem("-- None (Internal Staff) --", "0"));
        }
    }

    private void BindStaffGrid(int teamId)
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetStaffByTeam", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TeamID", teamId);
            con.Open();

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            gvStaff.DataSource = dt;
            gvStaff.DataBind();
        }
    }

    protected void btnSaveMember_Click(object sender, EventArgs e)
    {
        int teamId = Convert.ToInt32(hdnSelectedTeamID.Value);

        if (!CanActOnTeam(teamId)) return;

        string name = txtStaffName.Text.Trim();
        string jobTitle = ddlJobTitle.SelectedValue;
        string contactNumber = txtContactNumber.Text.Trim();
        int roleId = Convert.ToInt32(ddlMemberRole.SelectedValue);
        int contractorId = Convert.ToInt32(ddlMemberContractor.SelectedValue);
        int staffId = Convert.ToInt32(hdnStaffID.Value);

        //Block supervisor from assigning Supervisor job title or role
        if (isSupervisor)
        {
            if (jobTitle == "Supervisor")
            {
                ShowMemberMessage("You cannot assign the Supervisor role to a member.", false);
                ScriptManager.RegisterStartupScript(this, GetType(),
                    "keepMemberOpen", "toggleMembersSection(true); toggleMemberForm(true);", true);
                return;
            }

            int supervisorRoleID = GetRoleIDByName("Supervisor");
            if (roleId != 0 && roleId == supervisorRoleID)
            {
                ShowMemberMessage("You cannot assign the Supervisor system role to a member.", false);
                ScriptManager.RegisterStartupScript(this, GetType(),
                    "keepMemberOpen", "toggleMembersSection(true); toggleMemberForm(true);", true);
                return;
            }
        }

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(jobTitle))
        {
            ShowMemberMessage("Please enter a Name and select a Role before saving.", false);
            ScriptManager.RegisterStartupScript(this, GetType(),
                "keepMemberOpen", "toggleMembersSection(true); toggleMemberForm(true);", true);
            return;
        }

        

        using (SqlConnection con = new SqlConnection(ConnStr))
        {
            SqlCommand cmd;

            if (staffId == 0)
            {
                cmd = new SqlCommand("SP_CMS_InsertStaff", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TeamID", teamId);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@ContactNumber",
                    string.IsNullOrEmpty(contactNumber) ? (object)DBNull.Value : contactNumber);
                cmd.Parameters.AddWithValue("@JobTitle", jobTitle);
                cmd.Parameters.AddWithValue("@ContractorID",
                    contractorId == 0 ? (object)DBNull.Value : contractorId);
                cmd.Parameters.AddWithValue("@RoleID",
                    roleId == 0 ? (object)DBNull.Value : roleId);
                cmd.Parameters.AddWithValue("@UpdatedBy", CurrentUserID);
            }
            else
            {
                cmd = new SqlCommand("SP_CMS_UpdateStaff", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StaffID", staffId);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@ContactNumber",
                    string.IsNullOrEmpty(contactNumber) ? (object)DBNull.Value : contactNumber);
                cmd.Parameters.AddWithValue("@JobTitle", jobTitle);
                cmd.Parameters.AddWithValue("@ContractorID",
                    contractorId == 0 ? (object)DBNull.Value : contractorId);
                cmd.Parameters.AddWithValue("@RoleID",
                    roleId == 0 ? (object)DBNull.Value : roleId);
                cmd.Parameters.AddWithValue("@UpdatedBy", CurrentUserID);
                // NOTE: @TeamID intentionally NOT passed here.
                // SP_CMS_UpdateStaff treats a missing/NULL @TeamID as
                // "leave TeamID unchanged" — Team.aspx never reassigns teams.
            }

            con.Open();
            cmd.ExecuteNonQuery();
        }

        string msg = staffId == 0
            ? "Member added successfully."
            : "Member details updated successfully.";

        ResetMemberForm();
        BindStaffGrid(teamId);
        ShowMemberMessage(msg, true);

        ScriptManager.RegisterStartupScript(this, GetType(),
            "hideMemberForm", "toggleMembersSection(true); toggleMemberForm(false);", true);
    }

    protected void btnCancelMember_Click(object sender, EventArgs e)
    {
        int teamId = Convert.ToInt32(hdnSelectedTeamID.Value);

        ResetMemberForm();
        if (CanActOnTeam(teamId)) BindStaffGrid(teamId);

        ScriptManager.RegisterStartupScript(this, GetType(),
            "hideMemberFormOnCancel", "toggleMembersSection(true); toggleMemberForm(false);", true);
    }

    protected void gvStaff_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int teamId = Convert.ToInt32(hdnSelectedTeamID.Value);
        if (!CanActOnTeam(teamId)) return;

        if (e.CommandName == "EditMember")
        {
            int staffId = Convert.ToInt32(e.CommandArgument);
            LoadMemberForEdit(staffId);
            btnSaveMember.Text = "Update Member";
            btnSaveMember.CssClass = "btn-update";

            ScriptManager.RegisterStartupScript(this, GetType(),
                "showMemberFormEdit", "toggleMembersSection(true); toggleMemberForm(true);", true);
        }
        else if (e.CommandName == "RemoveMember")
        {
            int staffId = Convert.ToInt32(e.CommandArgument);
            DeleteMember(staffId);

            ResetMemberForm();
            BindStaffGrid(teamId);
            ShowMemberMessage("Member removed from team.", true);

            ScriptManager.RegisterStartupScript(this, GetType(),
                "afterRemove", "toggleMembersSection(true); toggleMemberForm(false);", true);
        }
    }

    private void LoadMemberForEdit(int staffId)
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetStaffByID", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@StaffID", staffId);
            con.Open();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    hdnStaffID.Value = reader["StaffID"].ToString();
                    txtStaffName.Text = reader["Name"].ToString();
                    txtContactNumber.Text = reader["ContactNumber"] == DBNull.Value
                        ? "" : reader["ContactNumber"].ToString();
                    ddlJobTitle.SelectedValue = reader["JobTitle"].ToString();
                    ddlMemberRole.SelectedValue = reader["RoleID"] == DBNull.Value
                        ? "0" : reader["RoleID"].ToString();
                    ddlMemberContractor.SelectedValue = reader["ContractorID"] == DBNull.Value
                        ? "0" : reader["ContractorID"].ToString();

                    lblMemberFormHeading.Text = "Edit Member #" + staffId;
                }
            }
        }
    }

    private void DeleteMember(int staffId)
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_DeleteStaff", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@StaffID", staffId);
            con.Open();
            cmd.ExecuteNonQuery();
        }
    }

    private void ResetMemberForm()
    {
        hdnStaffID.Value = "0";
        txtStaffName.Text = "";
        txtContactNumber.Text = "";
        ddlJobTitle.SelectedValue = "";
        ddlMemberRole.SelectedValue = "0";
        ddlMemberContractor.SelectedValue = "0";

        lblMemberFormHeading.Text = "Add Team Member";
        btnSaveMember.Text = "Save Member";
        btnSaveMember.CssClass = "btn-save";
        lblMemberMessage.Text = "";
    }

    private void ShowMemberMessage(string message, bool success)
    {
        lblMemberMessage.Text = message;
        lblMemberMessage.ForeColor = success
            ? System.Drawing.Color.Green
            : System.Drawing.Color.Red;
    }

    
    // TEAM AND SECTION ASSIGNMENT (CmsTeamSection)
    

    private void BindSectionDropdown()
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetAllSections", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            ddlAssignSection.DataSource = dt;
            ddlAssignSection.DataTextField = "SectionName";
            ddlAssignSection.DataValueField = "SectionID";
            ddlAssignSection.DataBind();
            ddlAssignSection.Items.Insert(0, new ListItem("-- Select Section --", "0"));
        }
    }

    private void BindAssignedSectionsGrid(int teamId)
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetAssignedSections", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TeamID", teamId);
            con.Open();

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            gvAssignedSections.DataSource = dt;
            gvAssignedSections.DataBind();
        }
    }

    protected void btnAssignSection_Click(object sender, EventArgs e)
    {
        int teamId = Convert.ToInt32(hdnSelectedTeamID.Value);
        if (!CanActOnTeam(teamId)) return;

        int sectionId = Convert.ToInt32(ddlAssignSection.SelectedValue);

        if (sectionId == 0)
        {
            lblAreaMessage.ForeColor = System.Drawing.Color.Red;
            lblAreaMessage.Text = "Please select a section to assign.";
            ScriptManager.RegisterStartupScript(this, GetType(),
                "keepMembersOpen", "toggleMembersSection(true);", true);
            return;
        }

        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_AssignSectionToTeam", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TeamID", teamId);
            cmd.Parameters.AddWithValue("@SectionID", sectionId);
            con.Open();

            int result = Convert.ToInt32(cmd.ExecuteScalar());

            if (result == -1)
            {
                lblAreaMessage.ForeColor = System.Drawing.Color.Red;
                lblAreaMessage.Text = "This section is already assigned to the team.";
            }
            else
            {
                lblAreaMessage.ForeColor = System.Drawing.Color.Green;
                lblAreaMessage.Text = "Section assigned successfully.";
            }
        }

        BindAssignedSectionsGrid(teamId);
        ddlAssignSection.SelectedValue = "0";

        ScriptManager.RegisterStartupScript(this, GetType(),
            "keepMembersOpenAfterAssign", "toggleMembersSection(true);", true);
    }

    protected void gvAssignedSections_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int teamId = Convert.ToInt32(hdnSelectedTeamID.Value);
        if (!CanActOnTeam(teamId)) return;

        if (e.CommandName == "UnassignSection")
        {
            int assignmentId = Convert.ToInt32(e.CommandArgument);

            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand("SP_CMS_UnassignSectionFromTeam", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AssignmentID", assignmentId);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            BindAssignedSectionsGrid(teamId);
            lblAreaMessage.ForeColor = System.Drawing.Color.Green;
            lblAreaMessage.Text = "Section unassigned.";

            ScriptManager.RegisterStartupScript(this, GetType(),
                "keepMembersOpenAfterUnassign", "toggleMembersSection(true);", true);
        }
    }
}