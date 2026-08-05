using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CleaningManagement_Notification : System.Web.UI.Page
{


    private string connStr = ConfigurationManager.ConnectionStrings["CmsConnectionString"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindGrid();
        }
    }

    #region Filters

    protected void btnApply_Click(object sender, EventArgs e)
    {
        BindGrid();
    }

    protected void btnReset_Click(object sender, EventArgs e)
    {
        ddlChannel.SelectedValue = "";
        ddlStatus.SelectedValue = "";
        ddlRecipientType.SelectedValue = "";
        txtFrom.Text = "";
        txtTo.Text = "";
        chkComplaintsOnly.Checked = false;
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
                    n.NotificationID,
                    n.RecipientID,
                    n.RecipientType,
                    CASE WHEN n.RecipientType = 'Staff' THEN st.Name
                         ELSE CAST(n.RecipientID AS NVARCHAR(20))
                    END AS RecipientName,
                    n.RelatedEntityType,
                    n.RelatedEntityID,
                    n.Channel,
                    n.Message,
                    n.SentDate,
                    n.Status
                FROM CmsNotification n
                LEFT JOIN CmsStaff st ON n.RecipientType = 'Staff' AND n.RecipientID = st.StaffID
                WHERE (@Channel = '' OR n.Channel = @Channel)
                  AND (@Status = '' OR n.Status = @Status)
                  AND (@RecipientType = '' OR n.RecipientType = @RecipientType)
                  AND (@FromDate = '' OR n.SentDate >= @FromDateVal)
                  AND (@ToDate = '' OR n.SentDate <= @ToDateVal)
                  AND (@ComplaintsOnly = 0 OR n.RelatedEntityType = 'Feedback')
                ORDER BY n.SentDate DESC";

            SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@Channel", ddlChannel.SelectedValue ?? "");
            cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue ?? "");
            cmd.Parameters.AddWithValue("@RecipientType", ddlRecipientType.SelectedValue ?? "");

            string fromFilter = txtFrom.Text;
            cmd.Parameters.AddWithValue("@FromDate", fromFilter);
            cmd.Parameters.AddWithValue("@FromDateVal", string.IsNullOrEmpty(fromFilter) ? (object)DBNull.Value : Convert.ToDateTime(fromFilter));

            string toFilter = txtTo.Text;
            cmd.Parameters.AddWithValue("@ToDate", toFilter);
          
            cmd.Parameters.AddWithValue("@ToDateVal", string.IsNullOrEmpty(toFilter) ? (object)DBNull.Value : Convert.ToDateTime(toFilter).AddDays(1).AddSeconds(-1));

            cmd.Parameters.AddWithValue("@ComplaintsOnly", chkComplaintsOnly.Checked ? 1 : 0);

            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(rdr);

            gvNotifications.DataSource = dt;
            gvNotifications.DataBind();

            lblResultCount.Text = string.Format("Showing {0} record{1}",
                dt.Rows.Count, dt.Rows.Count == 1 ? "" : "s");

            BindStats(dt);
        }
    }

    private void BindStats(DataTable dt)
    {
        int total = dt.Rows.Count;
        int pending = 0, sent = 0, failed = 0;

        foreach (DataRow row in dt.Rows)
        {
            string status = row["Status"].ToString();
            if (status == "Pending") pending++;
            else if (status == "Sent") sent++;
            else if (status == "Failed") failed++;
        }

        litTotal.Text = total.ToString();
        litPending.Text = pending.ToString();
        litSent.Text = sent.ToString();
        litFailed.Text = failed.ToString();
    }

    #endregion
}