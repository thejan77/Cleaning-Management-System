using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

public partial class DashboardSupervisor : System.Web.UI.Page
{
    private string ConnStr
    {
        get { return ConfigurationManager.ConnectionStrings["CmsConnectionString"].ConnectionString; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        // Auth guard: must be logged in as Supervisor 
        if (Session["UserID"] == null || Session["UserRole"] == null)
        {
            Response.Redirect("~/Login.aspx");
            return;
        }

        if (Session["UserRole"].ToString() != "Supervisor")
        {
            Response.Redirect("~/Login.aspx");
            return;
        }

        int userId = Convert.ToInt32(Session["UserID"]);
        lblSupervisorName.Text = Session["UserFullName"] != null
            ? Session["UserFullName"].ToString()
            : "Supervisor";

        if (!IsPostBack)
        {
            LoadDashboard(userId);
        }
    }

    private void LoadDashboard(int userId)
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetSupervisorDashboard", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userId);
            con.Open();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    pnlNoTeam.Visible = false;
                    pnlDashboard.Visible = true;

                    lblTeamName.Text = reader["TeamName"].ToString();
                    lblSupervisorAssigned.Text = reader["SupervisorName"].ToString();
                    lblMemberCount.Text = reader["MemberCount"].ToString();
                    lblAreaCount.Text = reader["AreaCount"].ToString();
                }
                else
                {
                    // No row returned = account not linked to any team
                    pnlNoTeam.Visible = true;
                    pnlDashboard.Visible = false;
                }
            }
        }
    }
}