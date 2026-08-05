using System;
using System.Web.UI;

public partial class CmsMaster : MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
       
        if (Session["UserID"] == null || Session["UserRole"] == null)
        {
            Response.Redirect("~/Login.aspx");
            return;
        }

        string role = Session["UserRole"].ToString();

        litUserName.Text = Session["UserFullName"] != null ? Session["UserFullName"].ToString() : "User";
        litUserRole.Text = role;
        litToday.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy");

        phMasters.Visible = (role == "Admin");
        phApplications.Visible = (role == "Admin" || role == "Supervisor");
        phStaffMenu.Visible = (role == "Staff");
        phContractorMenu.Visible = (role == "Contractor");
        phReports.Visible = (role == "Admin" || role == "Supervisor");

        if (!IsPostBack)
        {
            LoadNotificationCount();
        }
    }

    private void LoadNotificationCount()
    {
       
        lblNotifCount.Visible = false;
    }

    protected void lnkLogout_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Session.Abandon();
        Response.Redirect("~/Login.aspx");
    }
}