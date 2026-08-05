using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CleaningManagement_Masters_Staff : System.Web.UI.Page
{
    private string ConnStr
    {
        get { return ConfigurationManager.ConnectionStrings["CmsConnectionString"].ConnectionString; }
    }

    private int CurrentUserID
    {
        get { return Session["UserID"] != null ? Convert.ToInt32(Session["UserID"]) : 0; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["UserID"] == null || Session["UserRole"] == null)
        {
            Response.Redirect("~/Login.aspx");
            return;
        }

        if (Session["UserRole"].ToString() != "Admin")
        {
            Response.Redirect("~/Login.aspx");
            return;
        }

        if (!IsPostBack)
        {
            LoadTeamDropdown(ddlTeam, false);
            LoadTeamDropdown(ddlFilterTeam, true);
            LoadRoleDropdown();
            LoadContractorDropdown();
            BindStaffGrid(0, "", "");
        }
    }

    private void LoadTeamDropdown(DropDownList ddl, bool isFilter)
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetAllTeamsDropdown", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            ddl.DataSource = dt;
            ddl.DataTextField = "TeamName";
            ddl.DataValueField = "TeamID";
            ddl.DataBind();

            ddl.Items.Insert(0, new ListItem(isFilter ? "All Teams" : "-- None (No Team) --", "0"));
        }
    }

    private void LoadRoleDropdown()
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetAllRoles", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            ddlStaffRole.DataSource = dt;
            ddlStaffRole.DataTextField = "RoleName";
            ddlStaffRole.DataValueField = "RoleID";
            ddlStaffRole.DataBind();
            ddlStaffRole.Items.Insert(0, new ListItem("-- Select --", "0"));
        }
    }

    private void LoadContractorDropdown()
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetAllContractors", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            ddlStaffContractor.DataSource = dt;
            ddlStaffContractor.DataTextField = "ContractorName";
            ddlStaffContractor.DataValueField = "ContractorID";
            ddlStaffContractor.DataBind();
            ddlStaffContractor.Items.Insert(0, new ListItem("-- None --", "0"));
        }
    }

    private void BindStaffGrid(int teamId, string jobTitle, string searchName)
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetAllStaff", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TeamID", teamId);
            cmd.Parameters.AddWithValue("@JobTitle", string.IsNullOrEmpty(jobTitle) ? (object)DBNull.Value : jobTitle);
            cmd.Parameters.AddWithValue("@SearchName", string.IsNullOrEmpty(searchName) ? (object)DBNull.Value : searchName);
            con.Open();

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            gvStaff.DataSource = dt;
            gvStaff.DataBind();
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        int teamId = Convert.ToInt32(ddlFilterTeam.SelectedValue);
        string jobTitle = ddlFilterRole.SelectedValue;
        string searchName = txtFilterName.Text.Trim();

        BindStaffGrid(teamId, jobTitle, searchName);
    }

    protected void btnClearFilter_Click(object sender, EventArgs e)
    {
        ddlFilterTeam.SelectedValue = "0";
        ddlFilterRole.SelectedValue = "";
        txtFilterName.Text = "";

        BindStaffGrid(0, "", "");
    }

    protected void btnSaveStaff_Click(object sender, EventArgs e)
    {
        string name = txtStaffName.Text.Trim();
        int teamId = Convert.ToInt32(ddlTeam.SelectedValue);
        string jobTitle = ddlJobTitle.SelectedValue;
        string contactNumber = txtContactNumber.Text.Trim();
        int roleId = Convert.ToInt32(ddlStaffRole.SelectedValue);
        int contractorId = Convert.ToInt32(ddlStaffContractor.SelectedValue);

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(jobTitle))
        {
            ShowMessage("Please enter a Name and select a Role.", false);
            ScriptManager.RegisterStartupScript(this, GetType(),
                "keepOpen", "toggleStaffForm(true);", true);
            return;
        }

        if (teamId == 0 && contractorId == 0)
        {
            ShowMessage("Please select either a Team or a Contractor for this staff member.", false);
            ScriptManager.RegisterStartupScript(this, GetType(),
                "keepOpen", "toggleStaffForm(true);", true);
            return;
        }

        int staffId = Convert.ToInt32(hdnStaffID.Value);

        using (SqlConnection con = new SqlConnection(ConnStr))
        {
            SqlCommand cmd;

            if (staffId == 0)
            {
                cmd = new SqlCommand("SP_CMS_InsertStaff", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TeamID", teamId == 0 ? (object)DBNull.Value : teamId);
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
                cmd.Parameters.AddWithValue("@TeamID", teamId == 0 ? (object)DBNull.Value : teamId);
                cmd.Parameters.AddWithValue("@ContractorID",
                    contractorId == 0 ? (object)DBNull.Value : contractorId);
                cmd.Parameters.AddWithValue("@RoleID",
                    roleId == 0 ? (object)DBNull.Value : roleId);
                cmd.Parameters.AddWithValue("@UpdatedBy", CurrentUserID);
            }

            con.Open();
            cmd.ExecuteNonQuery();
        }

        string msg = staffId == 0
            ? "Staff member added successfully."
            : "Staff member updated successfully.";

        ResetForm();
        BindStaffGrid(
            Convert.ToInt32(ddlFilterTeam.SelectedValue),
            ddlFilterRole.SelectedValue,
            txtFilterName.Text.Trim());
        ShowMessage(msg, true);

        ScriptManager.RegisterStartupScript(this, GetType(),
            "hideForm", "toggleStaffForm(false);", true);
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ResetForm();
        ScriptManager.RegisterStartupScript(this, GetType(),
            "hideForm", "toggleStaffForm(false);", true);
    }

    protected void gvStaff_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "EditStaff")
        {
            int staffId = Convert.ToInt32(e.CommandArgument);
            LoadStaffForEdit(staffId);
            btnSaveStaff.Text = "Update Staff";
            btnSaveStaff.CssClass = "btn-update";

            ScriptManager.RegisterStartupScript(this, GetType(),
                "showForm", "toggleStaffForm(true);", true);
        }
        else if (e.CommandName == "RemoveStaff")
        {
            int staffId = Convert.ToInt32(e.CommandArgument);

            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand("SP_CMS_DeleteStaff", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StaffID", staffId);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            BindStaffGrid(
                Convert.ToInt32(ddlFilterTeam.SelectedValue),
                ddlFilterRole.SelectedValue,
                txtFilterName.Text.Trim());
            ShowMessage("Staff member removed.", true);
        }
    }

    private void LoadStaffForEdit(int staffId)
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetStaffById", con))
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
                    ddlTeam.SelectedValue = reader["TeamID"] == DBNull.Value
                        ? "0" : reader["TeamID"].ToString();
                    ddlStaffRole.SelectedValue = reader["RoleID"] == DBNull.Value
                        ? "0" : reader["RoleID"].ToString();
                    ddlStaffContractor.SelectedValue = reader["ContractorID"] == DBNull.Value
                        ? "0" : reader["ContractorID"].ToString();

                    lblFormHeading.Text = "Edit Staff #" + staffId;
                }
            }
        }
    }

    private void ResetForm()
    {
        hdnStaffID.Value = "0";
        txtStaffName.Text = "";
        txtContactNumber.Text = "";
        ddlJobTitle.SelectedValue = "";
        ddlTeam.SelectedValue = "0";
        ddlStaffRole.SelectedValue = "0";
        ddlStaffContractor.SelectedValue = "0";

        lblFormHeading.Text = "Add New Staff";
        btnSaveStaff.Text = "Save Staff";
        btnSaveStaff.CssClass = "btn-save";
        lblMessage.Text = "";
    }

    private void ShowMessage(string message, bool success)
    {
        lblMessage.Text = message;
        lblMessage.ForeColor = success
            ? System.Drawing.Color.Green
            : System.Drawing.Color.Red;
    }
}