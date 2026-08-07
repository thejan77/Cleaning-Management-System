using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CleaningManagement_JobMaintenance : System.Web.UI.Page
{
  

    private string connStr = ConfigurationManager.ConnectionStrings["CmsConnectionString"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadTeamDropdown();
            LoadSupervisorDropdown();
            BindGrid();
        }
    }

    #region Dropdown Loading

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
            ddlTeam.Items.Insert(0, new ListItem("All teams", ""));
        }
    }


    private void LoadSupervisorDropdown()
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

            ddlSupervisor.DataSource = dt;
            ddlSupervisor.DataTextField = "Name";
            ddlSupervisor.DataValueField = "StaffID";
            ddlSupervisor.DataBind();
            ddlSupervisor.Items.Insert(0, new ListItem("All supervisors", ""));
        }
    }

    #endregion

    #region Filters

    protected void btnApply_Click(object sender, EventArgs e)
    {
        lblResultCount.Text =
            "Team: " + ddlTeam.SelectedValue +
            " | Supervisor: " + ddlSupervisor.SelectedValue +
            " | Status: " + ddlStatus.SelectedValue +
            " | From: " + txtFrom.Text +
            " | To: " + txtTo.Text;

        BindGrid();
    }

    protected void btnReset_Click(object sender, EventArgs e)
    {
        ddlTeam.SelectedValue = "";
        ddlSupervisor.SelectedValue = "";
        ddlStatus.SelectedValue = "";
        txtFrom.Text = "";
        txtTo.Text = "";
        BindGrid();
    }

    #endregion

    #region Grid Binding

    private void BindGrid()
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            string sql = @"
            SELECT
                r.RecordID,
                j.JobID,
                r.WorkDate,
                r.WorkTime,
                t.TeamName,
                st.Name AS SupervisorName,
                j.Status,

                ISNULL(
                    (
                        SELECT COUNT(*) 
                        FROM CmsStaff cs 
                        WHERE cs.TeamID = r.TeamID
                    ),0
                ) AS PeopleInvolved,

                r.WorkDetails

            FROM CmsJobMaintenanceRecord r

            INNER JOIN CmsJob j 
                ON r.JobID = j.JobID

            LEFT JOIN CmsTeam t 
                ON r.TeamID = t.TeamID

            LEFT JOIN CmsTeamSupervisor ts 
                ON r.TeamSupervisorID = ts.TeamSupervisorID

            LEFT JOIN CmsStaff st 
                ON ts.StaffID = st.StaffID


            WHERE 
                (@TeamID = '' OR r.TeamID = @TeamIDVal)

                AND

                (@SupervisorID = '' 
                 OR r.TeamSupervisorID IN
                    (
                        SELECT TeamSupervisorID
                        FROM CmsTeamSupervisor
                        WHERE StaffID = @SupervisorIDVal
                    )
                )

                AND

                (@Status = '' OR j.Status = @Status)

                AND

                (@FromDate = '' OR r.WorkDate >= @FromDateVal)

                AND

                (@ToDate = '' OR r.WorkDate <= @ToDateVal)


            ORDER BY 
                r.WorkDate DESC,
                r.WorkTime DESC";


            SqlCommand cmd = new SqlCommand(sql, con);


            // TEAM FILTER
            string teamFilter = ddlTeam.SelectedValue;

            cmd.Parameters.AddWithValue(
                "@TeamID",
                string.IsNullOrEmpty(teamFilter) ? "" : teamFilter
            );

            cmd.Parameters.AddWithValue(
                "@TeamIDVal",
                string.IsNullOrEmpty(teamFilter)
                ? 0
                : Convert.ToInt32(teamFilter)
            );


         
            string supervisorFilter = ddlSupervisor.SelectedValue;

            cmd.Parameters.AddWithValue(
                "@SupervisorID",
                string.IsNullOrEmpty(supervisorFilter) ? "" : supervisorFilter
            );

            cmd.Parameters.AddWithValue(
                "@SupervisorIDVal",
                string.IsNullOrEmpty(supervisorFilter)
                ? 0
                : Convert.ToInt32(supervisorFilter)
            );


  
            

            string statusFilter = ddlStatus.SelectedValue;

            if (statusFilter == "InProgress")
            {
                statusFilter = "In Progress";
            }

            cmd.Parameters.AddWithValue("@Status", statusFilter ?? "");
        


        
            string fromFilter = txtFrom.Text;

            cmd.Parameters.AddWithValue(
                "@FromDate",
                string.IsNullOrEmpty(fromFilter)
                ? ""
                : fromFilter
            );

            cmd.Parameters.AddWithValue(
                "@FromDateVal",
                string.IsNullOrEmpty(fromFilter)
                ? (object)DBNull.Value
                : Convert.ToDateTime(fromFilter)
            );


         
            string toFilter = txtTo.Text;

            cmd.Parameters.AddWithValue(
                "@ToDate",
                string.IsNullOrEmpty(toFilter)
                ? ""
                : toFilter
            );

            cmd.Parameters.AddWithValue(
                "@ToDateVal",
                string.IsNullOrEmpty(toFilter)
                ? (object)DBNull.Value
                : Convert.ToDateTime(toFilter)
            );


            con.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            DataTable dt = new DataTable();

            dt.Load(rdr);


            gvJobMaintenance.DataSource = dt;
            gvJobMaintenance.DataBind();


            lblResultCount.Text = string.Format(
                "Showing {0} record{1}",
                dt.Rows.Count,
                dt.Rows.Count == 1 ? "" : "s"
            );
        }
    }

    protected string GetStatusClass(string status)
    {
        switch (status)
        {
            case "Pending":
                return "status-Pending";

            case "In Progress":
            case "InProgress":
                return "status-InProgress";

            case "Completed":
                return "status-Completed";

            case "Cancelled":
                return "status-Cancelled";

            default:
                return "";
        }
    }

    protected void gvJobMaintenance_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.DataRow)
            return;

        object workTimeObj = DataBinder.Eval(e.Row.DataItem, "WorkTime");

        if (workTimeObj != DBNull.Value && workTimeObj != null)
        {
            TimeSpan ts = (TimeSpan)workTimeObj;
            e.Row.Cells[3].Text = DateTime.Today.Add(ts).ToString("h:mm tt");
        }
    }

    #endregion
}