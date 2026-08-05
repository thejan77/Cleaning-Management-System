using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CleaningManagement_Masters_UserAccount : System.Web.UI.Page
{
    private string ConnStr
    {
        get { return ConfigurationManager.ConnectionStrings["CmsConnectionString"].ConnectionString; }
    }

    private const string DEFAULT_PASSWORD = "Reset@2026";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["UserID"] == null || Session["UserRole"] == null ||
            Session["UserRole"].ToString() != "Admin")
        {
            Response.Redirect("~/Login.aspx");
            return;
        }

        if (!IsPostBack)
        {
            LoadRoleDropdown();
            LoadStaffDropdown();
            LoadContractorDropdown();
            BindUsersGrid();
        }
    }

    // Dropdowns 
    private void LoadRoleDropdown()
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetAllRoles", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlRole.DataSource = dt;
            ddlRole.DataTextField = "RoleName";
            ddlRole.DataValueField = "RoleID";
            ddlRole.DataBind();
            ddlRole.Items.Insert(0, new ListItem("-- Select Role --", "0"));
        }
    }

    private void LoadStaffDropdown()
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetStaffList", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlStaff.DataSource = dt;
            ddlStaff.DataTextField = "Name";
            ddlStaff.DataValueField = "StaffID";
            ddlStaff.DataBind();
            ddlStaff.Items.Insert(0, new ListItem("-- None --", "0"));
        }
    }

    private void LoadContractorDropdown()
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetAllContractors", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlContractor.DataSource = dt;
            ddlContractor.DataTextField = "ContractorName";
            ddlContractor.DataValueField = "ContractorID";
            ddlContractor.DataBind();
            ddlContractor.Items.Insert(0, new ListItem("-- None --", "0"));
        }
    }

    // Grid 
    private void BindUsersGrid()
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetUserAccounts", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            gvUsers.DataSource = dt;
            gvUsers.DataBind();
        }
    }

    // Save Or Update 
    protected void btnSaveUser_Click(object sender, EventArgs e)
    {
        string username = txtUsername.Text.Trim();
        string password = txtPassword.Text.Trim();
        int roleId = Convert.ToInt32(ddlRole.SelectedValue);
        int userId = Convert.ToInt32(hdnUserID.Value);
        int staffId = Convert.ToInt32(ddlStaff.SelectedValue);
        int contractorId = Convert.ToInt32(ddlContractor.SelectedValue);
        bool isActive = ddlIsActive.SelectedValue == "1";
        string fullName = txtName.Text.Trim();
        string email = txtEmail.Text.Trim();
        string contactNo = txtContactNumber.Text.Trim();

        if (string.IsNullOrEmpty(username) || roleId == 0)
        {
            ShowMessage("Please enter a Username and select a Role.", false);
            ScriptManager.RegisterStartupScript(this, GetType(),
                "keepOpen", "toggleUserForm(true);", true);
            return;
        }

        if (userId == 0 && string.IsNullOrEmpty(password))
        {
            ShowMessage("Please enter a Password for the new account.", false);
            ScriptManager.RegisterStartupScript(this, GetType(),
                "keepOpen", "toggleUserForm(true);", true);
            return;
        }

        using (SqlConnection con = new SqlConnection(ConnStr))
        {
            con.Open();
            SqlCommand cmd;
            int result;

            if (userId == 0)
            {
                // Auto-create staff record if Supervisor role selected
                // and no existing staff record was manually linked
                if (staffId == 0)
                {
                    string roleName = GetRoleNameByID(roleId, con);
                    if (roleName == "Supervisor")
                    {
                        staffId = AutoCreateSupervisorStaff(
                            string.IsNullOrEmpty(fullName) ? username : fullName,
                            contactNo, con);
                    }
                }

                string hash = HashPassword(password);
                cmd = new SqlCommand("SP_CMS_InsertUser", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@PasswordHash", hash);
                cmd.Parameters.AddWithValue("@RoleID", roleId);
                cmd.Parameters.AddWithValue("@StaffID",
                    staffId == 0 ? (object)DBNull.Value : staffId);
                cmd.Parameters.AddWithValue("@ContractorID",
                    contractorId == 0 ? (object)DBNull.Value : contractorId);
                cmd.Parameters.AddWithValue("@Name",
                    string.IsNullOrEmpty(fullName)
                        ? (object)DBNull.Value : fullName);
                cmd.Parameters.AddWithValue("@Email",
                    string.IsNullOrEmpty(email)
                        ? (object)DBNull.Value : email);
                cmd.Parameters.AddWithValue("@ContactNumber",
                    string.IsNullOrEmpty(contactNo)
                        ? (object)DBNull.Value : contactNo);
                cmd.Parameters.AddWithValue("@IsActive", isActive);

                result = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                cmd = new SqlCommand("SP_CMS_UpdateUser", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@RoleID", roleId);
                cmd.Parameters.AddWithValue("@StaffID",
                    staffId == 0 ? (object)DBNull.Value : staffId);
                cmd.Parameters.AddWithValue("@ContractorID",
                    contractorId == 0 ? (object)DBNull.Value : contractorId);
                cmd.Parameters.AddWithValue("@Name",
                    string.IsNullOrEmpty(fullName)
                        ? (object)DBNull.Value : fullName);
                cmd.Parameters.AddWithValue("@Email",
                    string.IsNullOrEmpty(email)
                        ? (object)DBNull.Value : email);
                cmd.Parameters.AddWithValue("@ContactNumber",
                    string.IsNullOrEmpty(contactNo)
                        ? (object)DBNull.Value : contactNo);
                cmd.Parameters.AddWithValue("@IsActive", isActive);

                result = Convert.ToInt32(cmd.ExecuteScalar());

                // Update password if provided
                if (!string.IsNullOrEmpty(password))
                {
                    string hash = HashPassword(password);
                    using (SqlCommand pwdCmd =
                        new SqlCommand("SP_CMS_ResetPassword", con))
                    {
                        pwdCmd.CommandType = CommandType.StoredProcedure;
                        pwdCmd.Parameters.AddWithValue("@UserID", userId);
                        pwdCmd.Parameters.AddWithValue("@PasswordHash", hash);
                        pwdCmd.ExecuteNonQuery();
                    }
                }
            }

            if (result == -1)
            {
                ShowMessage("A user with this username already exists.", false);
                ScriptManager.RegisterStartupScript(this, GetType(),
                    "keepOpen", "toggleUserForm(true);", true);
                return;
            }
        }

        string msg = userId == 0
            ? "User account created successfully."
            : "User account updated successfully.";

        ResetForm();
        BindUsersGrid();
        ShowMessage(msg, true);
        ScriptManager.RegisterStartupScript(this, GetType(),
            "hideForm", "toggleUserForm(false);", true);
    }

    // Cancel 
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ResetForm();
        ScriptManager.RegisterStartupScript(this, GetType(),
            "hideForm", "toggleUserForm(false);", true);
    }

    // Grid row commands 
    protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "EditUser")
        {
            int userId = Convert.ToInt32(e.CommandArgument);
            LoadUserForEdit(userId);
            btnSaveUser.Text = "Update User";
            btnSaveUser.CssClass = "btn-update";
            ScriptManager.RegisterStartupScript(this, GetType(),
                "showForm", "toggleUserForm(true);", true);
        }
        else if (e.CommandName == "ResetPwd")
        {
            int userId = Convert.ToInt32(e.CommandArgument);
            string hash = HashPassword(DEFAULT_PASSWORD);

            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand("SP_CMS_ResetPassword", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@PasswordHash", hash);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            BindUsersGrid();
            ShowMessage("Password reset to default: " + DEFAULT_PASSWORD, true);
        }
        else if (e.CommandName == "ToggleActive")
        {
            string[] parts = e.CommandArgument.ToString().Split('|');
            int userId = Convert.ToInt32(parts[0]);
            bool currentVal = Convert.ToBoolean(parts[1]);
            bool newVal = !currentVal;

            if (userId == Convert.ToInt32(Session["UserID"]))
            {
                ShowMessage("You cannot disable your own account.", false);
                return;
            }

            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand("SP_CMS_ToggleUserActive", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@IsActive", newVal);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            BindUsersGrid();
            ShowMessage(newVal ? "Account enabled." : "Account disabled.", true);
        }
    }

    // Load for edit 
    private void LoadUserForEdit(int userId)
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetUserByID", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userId);
            con.Open();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    hdnUserID.Value = reader["UserID"].ToString();
                    txtUsername.Text = reader["Username"].ToString();
                    txtPassword.Text = "";
                    txtName.Text = reader["Name"] == DBNull.Value
                        ? "" : reader["Name"].ToString();
                    txtEmail.Text = reader["Email"] == DBNull.Value
                        ? "" : reader["Email"].ToString();
                    txtContactNumber.Text = reader["ContactNumber"] == DBNull.Value
                        ? "" : reader["ContactNumber"].ToString();
                    ddlRole.SelectedValue = reader["RoleID"].ToString();
                    ddlIsActive.SelectedValue =
                        Convert.ToBoolean(reader["IsActive"]) ? "1" : "0";
                    ddlStaff.SelectedValue = reader["StaffID"] == DBNull.Value
                        ? "0" : reader["StaffID"].ToString();
                    ddlContractor.SelectedValue = reader["ContractorID"] == DBNull.Value
                        ? "0" : reader["ContractorID"].ToString();
                    lblFormHeading.Text = "Edit User #" + userId;
                }
            }
        }
    }

    // Reset form
    private void ResetForm()
    {
        hdnUserID.Value = "0";
        txtUsername.Text = "";
        txtPassword.Text = "";
        txtName.Text = "";
        txtEmail.Text = "";
        txtContactNumber.Text = "";
        ddlRole.SelectedValue = "0";
        ddlIsActive.SelectedValue = "1";
        ddlStaff.SelectedValue = "0";
        ddlContractor.SelectedValue = "0";
        lblFormHeading.Text = "Add New User";
        btnSaveUser.Text = "Save User";
        btnSaveUser.CssClass = "btn-save";
        lblMessage.Text = "";
    }

    private void ShowMessage(string message, bool success)
    {
        lblMessage.Text = message;
        lblMessage.ForeColor = success
            ? System.Drawing.Color.Green
            : System.Drawing.Color.Red;
    }

    // SP helpers 
    private string GetRoleNameByID(int roleId, SqlConnection con)
    {
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetRoleNameByID", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RoleID", roleId);
            object result = cmd.ExecuteScalar();
            return result == null ? "" : result.ToString();
        }
    }

    private int AutoCreateSupervisorStaff(
        string name, string contactNo, SqlConnection con)
    {
        using (SqlCommand cmd = new SqlCommand(
            "SP_CMS_AutoCreateSupervisorStaff", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@ContactNumber",
                string.IsNullOrEmpty(contactNo)
                    ? (object)DBNull.Value : contactNo);
            object result = cmd.ExecuteScalar();
            return result == null ? 0 : Convert.ToInt32(result);
        }
    }

    // Hash matches Login.aspx (UTF-16) 
    private static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(
                Encoding.Unicode.GetBytes(password));
            var sb = new StringBuilder();
            foreach (byte b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}