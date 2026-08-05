using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CleaningManagement_FeedbackComplaint : System.Web.UI.Page
{
    private string connStr = ConfigurationManager.ConnectionStrings["CmsConnectionString"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadSectionDropdowns();
            LoadLocationDropdown();
            LoadStaffDropdown();
            LoadSummaryCounts();
            BindGrid();
        }
    }

    #region Code Formatting

    protected string FormatFeedbackCode(object id)
    {
        if (id == null || id == DBNull.Value)
            return "";

        int val = Convert.ToInt32(id);
        return "FED-" + val.ToString("D3");
    }

    private int GetNextFeedbackId()
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            string sql = "SELECT ISNULL(MAX(FeedbackID), 0) + 1 FROM CmsFeedback";
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            object result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }
    }

    #endregion

    #region Dropdown Loading

    private void LoadSectionDropdowns()
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

            ddlSection.DataSource = dt;
            ddlSection.DataTextField = "SectionName";
            ddlSection.DataValueField = "SectionID";
            ddlSection.DataBind();
            ddlSection.Items.Insert(0, new ListItem("-- Select Section --", ""));
        }
    }

    private void LoadLocationDropdown()
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            string sql = "SELECT LocationID, LocationName FROM CmsLocation ORDER BY LocationName";
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(rdr);

            ddlLocation.DataSource = dt;
            ddlLocation.DataTextField = "LocationName";
            ddlLocation.DataValueField = "LocationID";
            ddlLocation.DataBind();
            ddlLocation.Items.Insert(0, new ListItem("-- Select Location --", ""));
        }
    }

    private void LoadStaffDropdown()
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            string sql = "SELECT StaffID, Name FROM CmsStaff ORDER BY Name";
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(rdr);

            ddlSubmittedByStaff.DataSource = dt;
            ddlSubmittedByStaff.DataTextField = "Name";
            ddlSubmittedByStaff.DataValueField = "StaffID";
            ddlSubmittedByStaff.DataBind();
            ddlSubmittedByStaff.Items.Insert(0, new ListItem("-- Select Staff --", ""));
        }
    }

    #endregion

    #region Summary Counts

    private void LoadSummaryCounts()
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            con.Open();
            lblCountOpen.Text = GetScalarCount(con, "SELECT COUNT(*) FROM CmsFeedback WHERE Status = 'Open'");
            lblCountInProgress.Text = GetScalarCount(con, "SELECT COUNT(*) FROM CmsFeedback WHERE Status = 'In Progress'");
            lblCountResolved.Text = GetScalarCount(con,
                "SELECT COUNT(*) FROM CmsFeedback WHERE Status = 'Resolved' AND MONTH(ResolvedDate) = MONTH(GETDATE()) AND YEAR(ResolvedDate) = YEAR(GETDATE())");
            lblCountComplaints.Text = GetScalarCount(con, "SELECT COUNT(*) FROM CmsFeedback WHERE Type = 'Complaint'");
        }
    }

    private string GetScalarCount(SqlConnection con, string sql)
    {
        SqlCommand cmd = new SqlCommand(sql, con);
        object result = cmd.ExecuteScalar();
        return result != null ? result.ToString() : "0";
    }

    #endregion

    #region Grid Binding

    private void BindGrid()
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            string sql = @"
                SELECT 
                    f.FeedbackID,
                    sec.SectionName,
                    f.Type,
                    f.Description,
                    f.SubmittedByType,
                    ISNULL(s.Name, f.SubmittedByType) AS SubmittedByName,
                    f.Status,
                    f.SubmittedDate,
                    f.ResolvedDate,
                    f.SectionID,
                    f.LocationID,
                    f.SubmittedByID
                FROM CmsFeedback f
                INNER JOIN CmsSection sec ON f.SectionID = sec.SectionID
                LEFT JOIN CmsStaff s ON f.SubmittedByID = s.StaffID AND f.SubmittedByType = 'Staff'
                WHERE (@SectionID = '' OR f.SectionID = @SectionIDVal)
                  AND (@Type = '' OR f.Type = @Type)
                  AND (@Status = '' OR f.Status = @Status)
                  AND (@SubmittedByType = '' OR f.SubmittedByType = @SubmittedByType)
                ORDER BY f.SubmittedDate DESC, f.FeedbackID DESC";

            SqlCommand cmd = new SqlCommand(sql, con);

            string sectionFilter = ddlFilterSection.SelectedValue;
            cmd.Parameters.AddWithValue("@SectionID", sectionFilter);
            cmd.Parameters.AddWithValue("@SectionIDVal", string.IsNullOrEmpty(sectionFilter) ? 0 : Convert.ToInt32(sectionFilter));
            cmd.Parameters.AddWithValue("@Type", ddlFilterType.SelectedValue ?? "");
            cmd.Parameters.AddWithValue("@Status", ddlFilterStatus.SelectedValue ?? "");
            cmd.Parameters.AddWithValue("@SubmittedByType", ddlFilterSubmittedByType.SelectedValue ?? "");

            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(rdr);

            gvFeedback.DataSource = dt;
            gvFeedback.DataBind();
        }

        LoadSummaryCounts();
    }

    protected void gvFeedback_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblStatus = (Label)e.Row.FindControl("lblStatus");
            if (lblStatus != null)
            {
                lblStatus.CssClass = "pill " + GetStatusPillClass(lblStatus.Text.Trim());
            }

            Label lblType = (Label)e.Row.FindControl("lblType");
            if (lblType != null)
            {
                lblType.CssClass = "pill " + GetTypePillClass(lblType.Text.Trim());
            }
        }
    }

    private string GetStatusPillClass(string status)
    {
        switch (status)
        {
            case "Open": return "pill-open";
            case "In Progress": return "pill-inprogress";
            case "Resolved": return "pill-resolved";
            case "Rejected": return "pill-rejected";
            case "Closed": return "pill-closed";
            default: return "pill-open";
        }
    }

    private string GetTypePillClass(string type)
    {
        switch (type)
        {
            case "Complaint": return "pill-complaint";
            case "Feedback": return "pill-feedback";
            case "Suggestion": return "pill-suggestion";
            default: return "pill-feedback";
        }
    }

    #endregion

    #region Filters

    protected void ddlFilterSection_SelectedIndexChanged(object sender, EventArgs e) { BindGrid(); }
    protected void ddlFilterType_SelectedIndexChanged(object sender, EventArgs e) { BindGrid(); }
    protected void ddlFilterStatus_SelectedIndexChanged(object sender, EventArgs e) { BindGrid(); }
    protected void ddlFilterSubmittedByType_SelectedIndexChanged(object sender, EventArgs e) { BindGrid(); }

    protected void btnClearFilters_Click(object sender, EventArgs e)
    {
        ddlFilterSection.SelectedValue = "";
        ddlFilterType.SelectedValue = "";
        ddlFilterStatus.SelectedValue = "";
        ddlFilterSubmittedByType.SelectedValue = "";
        BindGrid();
    }

    #endregion

    #region Add / Edit / Delete

    protected void btnAddFeedback_Click(object sender, EventArgs e)
    {
        ClearForm();
        fcModalTitle.InnerText = "Add Feedback / Complaint";
        ScriptManager.RegisterStartupScript(this, GetType(), "openModal", "openFcModal();", true);
    }

    protected void gvFeedback_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int feedbackId = Convert.ToInt32(e.CommandArgument);

        if (e.CommandName == "EditFeedback")
        {
            LoadRecordIntoForm(feedbackId);
            fcModalTitle.InnerText = "Edit Feedback / Complaint";
            ScriptManager.RegisterStartupScript(this, GetType(), "openModal", "openFcModal();", true);
        }
        else if (e.CommandName == "DeleteFeedback")
        {
            DeleteRecord(feedbackId);
            BindGrid();
        }
    }

    private void ClearForm()
    {
        hfFeedbackID.Value = "0";
        litFeedbackCode.Text = FormatFeedbackCode(GetNextFeedbackId());
        ddlSection.SelectedValue = "";
        ddlLocation.SelectedValue = "";
        ddlType.SelectedValue = "Complaint";
        ddlStatus.SelectedValue = "Open";
        txtDescription.Text = "";
        ddlSubmittedByType.SelectedValue = "Client";
        ddlSubmittedByStaff.SelectedValue = "";
        txtSubmittedDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        txtResolvedDate.Text = "";
    }

    private void LoadRecordIntoForm(int feedbackId)
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            string sql = "SELECT * FROM CmsFeedback WHERE FeedbackID = @id";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", feedbackId);
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                hfFeedbackID.Value = feedbackId.ToString();
                litFeedbackCode.Text = FormatFeedbackCode(feedbackId);

                ddlSection.SelectedValue = rdr["SectionID"].ToString();
                ddlLocation.SelectedValue = rdr["LocationID"] != DBNull.Value ? rdr["LocationID"].ToString() : "";
                ddlType.SelectedValue = rdr["Type"] != DBNull.Value ? rdr["Type"].ToString() : "Complaint";
                ddlStatus.SelectedValue = rdr["Status"].ToString();
                txtDescription.Text = rdr["Description"].ToString();

                ddlSubmittedByType.SelectedValue = rdr["SubmittedByType"].ToString();
                ddlSubmittedByStaff.SelectedValue = rdr["SubmittedByID"] != DBNull.Value ? rdr["SubmittedByID"].ToString() : "";

                txtSubmittedDate.Text = rdr["SubmittedDate"] != DBNull.Value
                    ? Convert.ToDateTime(rdr["SubmittedDate"]).ToString("yyyy-MM-dd") : "";
                txtResolvedDate.Text = rdr["ResolvedDate"] != DBNull.Value
                    ? Convert.ToDateTime(rdr["ResolvedDate"]).ToString("yyyy-MM-dd") : "";
            }
        }
    }

    protected void btnSaveFeedback_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(ddlSection.SelectedValue))
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "err",
                "alert('Please select a section.'); openFcModal();", true);
            return;
        }

        if (string.IsNullOrEmpty(txtDescription.Text.Trim()))
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "err",
                "alert('Please enter a description.'); openFcModal();", true);
            return;
        }

        int feedbackId = Convert.ToInt32(hfFeedbackID.Value);

        bool isStaffSubmitter = ddlSubmittedByType.SelectedValue == "Staff";
        object submittedByIdParam = (isStaffSubmitter && !string.IsNullOrEmpty(ddlSubmittedByStaff.SelectedValue))
            ? (object)Convert.ToInt32(ddlSubmittedByStaff.SelectedValue)
            : DBNull.Value;

        using (SqlConnection con = new SqlConnection(connStr))
        {
            con.Open();
            SqlCommand cmd;

            if (feedbackId == 0)
            {
                string sql = @"
                    INSERT INTO CmsFeedback
                        (SectionID, SubmittedByID, SubmittedByType, Type, Description, Status, SubmittedDate, ResolvedDate, LocationID)
                    VALUES
                        (@SectionID, @SubmittedByID, @SubmittedByType, @Type, @Description, @Status, @SubmittedDate, @ResolvedDate, @LocationID)";
                cmd = new SqlCommand(sql, con);
            }
            else
            {
                string sql = @"
                    UPDATE CmsFeedback SET
                        SectionID = @SectionID,
                        SubmittedByID = @SubmittedByID,
                        SubmittedByType = @SubmittedByType,
                        Type = @Type,
                        Description = @Description,
                        Status = @Status,
                        SubmittedDate = @SubmittedDate,
                        ResolvedDate = @ResolvedDate,
                        LocationID = @LocationID
                    WHERE FeedbackID = @FeedbackID";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@FeedbackID", feedbackId);
            }

            cmd.Parameters.AddWithValue("@SectionID", Convert.ToInt32(ddlSection.SelectedValue));
            cmd.Parameters.AddWithValue("@SubmittedByID", submittedByIdParam);
            cmd.Parameters.AddWithValue("@SubmittedByType", ddlSubmittedByType.SelectedValue);
            cmd.Parameters.AddWithValue("@Type", ddlType.SelectedValue);
            cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
            cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);
            cmd.Parameters.AddWithValue("@SubmittedDate",
                string.IsNullOrEmpty(txtSubmittedDate.Text) ? (object)DateTime.Now : Convert.ToDateTime(txtSubmittedDate.Text));
            cmd.Parameters.AddWithValue("@ResolvedDate",
                string.IsNullOrEmpty(txtResolvedDate.Text) ? (object)DBNull.Value : Convert.ToDateTime(txtResolvedDate.Text));
            cmd.Parameters.AddWithValue("@LocationID",
                string.IsNullOrEmpty(ddlLocation.SelectedValue) ? (object)DBNull.Value : Convert.ToInt32(ddlLocation.SelectedValue));

            cmd.ExecuteNonQuery();
        }

        BindGrid();
        ScriptManager.RegisterStartupScript(this, GetType(), "closeModal", "closeFcModal();", true);
    }

    private void DeleteRecord(int feedbackId)
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            string sql = "DELETE FROM CmsFeedback WHERE FeedbackID = @id";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", feedbackId);
            con.Open();
            cmd.ExecuteNonQuery();
        }
    }

    #endregion
}