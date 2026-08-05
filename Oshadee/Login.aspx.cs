using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;

public partial class Login : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
           
            if (Session["UserID"] != null && Session["UserRole"] != null)
            {
                RedirectToDashboard(Session["UserRole"].ToString());
            }
        }
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        string username = txtUsername.Text.Trim();
        string password = txtPassword.Text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowError("Please enter both username and password.");
            return;
        }

        string connStr = ConfigurationManager.ConnectionStrings["CmsConnectionString"].ConnectionString;

        string passwordHash = HashPassword(password);

      
        const string query = @"
            SELECT ua.UserID,
                   ISNULL(st.Name, ua.Username) AS FullName,
                   r.RoleName
            FROM CmsUserAccount ua
            INNER JOIN CmsRole r ON r.RoleID = ua.RoleID
            LEFT JOIN CmsStaff st ON st.StaffID = ua.StaffID
            WHERE ua.Username = @Username
              AND ua.PasswordHash = @PasswordHash
              AND ua.IsActive = 1;";

        try
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 100).Value = username;
                cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value = passwordHash;

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int userId = Convert.ToInt32(reader["UserID"]);

                        Session["UserID"] = userId;
                        Session["UserFullName"] = reader["FullName"].ToString();
                        Session["UserRole"] = reader["RoleName"].ToString();

                        reader.Close();
                        UpdateLastLogin(connStr, userId);

                        if (chkRemember.Checked)
                        {
                            
                        }

                        RedirectToDashboard(Session["UserRole"].ToString());
                    }
                    else
                    {
                        ShowError("Invalid username or password.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ShowError("Unable to log in right now. Please try again later.");
            
        }
    }

    protected void lnkForgot_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/ForgotPassword.aspx");
    }

    private void RedirectToDashboard(string role)
    {
        switch (role)
        {
            case "Admin":
                Response.Redirect("~/DashboardAdmin.aspx");
                break;
            case "Supervisor":
                Response.Redirect("~/Supervisor/SupervisorDashboard.aspx");
                break;
            case "Staff":
                Response.Redirect("~/Staff/StaffDashboard.aspx");
                break;
            case "Contractor":
                Response.Redirect("~/Contractor/ContractorDashboard.aspx");
                break;
            default:
                ShowError("Your account role is not recognized. Contact an administrator.");
                break;
        }
    }

    private void ShowError(string message)
    {
        lblMessage.Text = message;
        lblMessage.Visible = true;
    }

    private static void UpdateLastLogin(string connStr, int userId)
    {
        const string sql = "UPDATE CmsUserAccount SET LastLogin = GETDATE() WHERE UserID = @UserID;";
        using (var conn = new SqlConnection(connStr))
        using (var cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@UserID", userId);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

   
    private static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.Unicode.GetBytes(password));
            var sb = new StringBuilder();
            foreach (byte b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}