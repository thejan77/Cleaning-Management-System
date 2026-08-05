using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CleaningManagement_MachineMaintenance : System.Web.UI.Page
{
    private string connStr = ConfigurationManager.ConnectionStrings["CmsConnectionString"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadMachineDropdowns();
            LoadStaffDropdown();
            LoadSummaryCounts();
            BindGrid();
        }
    }

    #region Code Formatting

 
    protected string FormatMaintenanceCode(object id)
    {
        if (id == null || id == DBNull.Value)
            return "";

        int val = Convert.ToInt32(id);
        return "MCH-" + val.ToString("D3");
    }

    private int GetNextMaintenanceId()
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            string sql = "SELECT ISNULL(MAX(MaintenanceID), 0) + 1 FROM CmsMachineMaintenance";
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            object result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }
    }

    #endregion

    #region Dropdown Loading

    private void LoadMachineDropdowns()
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            string sql = "SELECT MachineID, MachineName FROM CmsMachine ORDER BY MachineName";
            SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(rdr);

            ddlFilterMachine.DataSource = dt;
            ddlFilterMachine.DataTextField = "MachineName";
            ddlFilterMachine.DataValueField = "MachineID";
            ddlFilterMachine.DataBind();
            ddlFilterMachine.Items.Insert(0, new ListItem("All Machines", ""));

            ddlMachine.DataSource = dt;
            ddlMachine.DataTextField = "MachineName";
            ddlMachine.DataValueField = "MachineID";
            ddlMachine.DataBind();
            ddlMachine.Items.Insert(0, new ListItem("-- Select Machine --", ""));
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

            ddlAssignedTo.DataSource = dt;
            ddlAssignedTo.DataTextField = "Name";
            ddlAssignedTo.DataValueField = "StaffID";
            ddlAssignedTo.DataBind();
            ddlAssignedTo.Items.Insert(0, new ListItem("-- Select Staff --", ""));
        }
    }

    #endregion

    #region Summary Counts

    private void LoadSummaryCounts()
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            con.Open();

            lblCountPending.Text = GetScalarCount(con, "SELECT COUNT(*) FROM CmsMachineMaintenance WHERE Status = 'Pending'");
            lblCountInProgress.Text = GetScalarCount(con, "SELECT COUNT(*) FROM CmsMachineMaintenance WHERE Status = 'In Progress'");
            lblCountCompleted.Text = GetScalarCount(con,
                "SELECT COUNT(*) FROM CmsMachineMaintenance WHERE Status = 'Completed' AND MONTH(CompletedDate) = MONTH(GETDATE()) AND YEAR(CompletedDate) = YEAR(GETDATE())");
            lblCountBreakdown.Text = GetScalarCount(con, "SELECT COUNT(*) FROM CmsMachineMaintenance WHERE RecordType = 'Breakdown'");
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
                    mm.MaintenanceID,
                    m.MachineName,
                    mm.RecordType,
                    mm.MaintenanceType,
                    mm.ScheduledDate,
                    s.Name AS AssignedToName,
                    mm.Cost,
                    mm.Status,
                    mm.MachineID,
                    mm.AssignedToID
                FROM CmsMachineMaintenance mm
                INNER JOIN CmsMachine m ON mm.MachineID = m.MachineID
                LEFT JOIN CmsStaff s ON mm.AssignedToID = s.StaffID
                WHERE (@MachineID = '' OR mm.MachineID = @MachineIDVal)
                  AND (@RecordType = '' OR mm.RecordType = @RecordType)
                  AND (@Status = '' OR mm.Status = @Status)
                  AND (@MaintenanceType = '' OR mm.MaintenanceType = @MaintenanceType)
                ORDER BY mm.ScheduledDate DESC, mm.MaintenanceID DESC";

            SqlCommand cmd = new SqlCommand(sql, con);

            string machineFilter = ddlFilterMachine.SelectedValue;
            cmd.Parameters.AddWithValue("@MachineID", machineFilter);
            cmd.Parameters.AddWithValue("@MachineIDVal", string.IsNullOrEmpty(machineFilter) ? 0 : Convert.ToInt32(machineFilter));
            cmd.Parameters.AddWithValue("@RecordType", ddlFilterRecordType.SelectedValue ?? "");
            cmd.Parameters.AddWithValue("@Status", ddlFilterStatus.SelectedValue ?? "");
            cmd.Parameters.AddWithValue("@MaintenanceType", ddlFilterMaintenanceType.SelectedValue ?? "");

            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(rdr);

            gvMaintenance.DataSource = dt;
            gvMaintenance.DataBind();
        }

        LoadSummaryCounts();
    }

    protected void gvMaintenance_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblStatus = (Label)e.Row.FindControl("lblStatus");
            if (lblStatus != null)
            {
                string status = lblStatus.Text.Trim();
                lblStatus.CssClass = "pill " + GetStatusPillClass(status);
            }

            Label lblRecordType = (Label)e.Row.FindControl("lblRecordType");
            if (lblRecordType != null)
            {
                string recordType = lblRecordType.Text.Trim();
                lblRecordType.CssClass = "pill " + (recordType == "Breakdown" ? "pill-breakdown" : "pill-scheduled");
            }
        }
    }

    private string GetStatusPillClass(string status)
    {
        switch (status)
        {
            case "Pending": return "pill-pending";
            case "In Progress": return "pill-inprogress";
            case "Completed": return "pill-completed";
            case "Cancelled": return "pill-cancelled";
            default: return "pill-pending";
        }
    }

    #endregion

    #region Filters

    protected void ddlFilterMachine_SelectedIndexChanged(object sender, EventArgs e) { BindGrid(); }
    protected void ddlFilterRecordType_SelectedIndexChanged(object sender, EventArgs e) { BindGrid(); }
    protected void ddlFilterStatus_SelectedIndexChanged(object sender, EventArgs e) { BindGrid(); }
    protected void ddlFilterMaintenanceType_SelectedIndexChanged(object sender, EventArgs e) { BindGrid(); }

    protected void btnClearFilters_Click(object sender, EventArgs e)
    {
        ddlFilterMachine.SelectedValue = "";
        ddlFilterRecordType.SelectedValue = "";
        ddlFilterStatus.SelectedValue = "";
        ddlFilterMaintenanceType.SelectedValue = "";
        BindGrid();
    }

    #endregion

    #region Add / Edit / Delete

    protected void btnAddMaintenance_Click(object sender, EventArgs e)
    {
        ClearForm();
        mmModalTitle.InnerText = "Add Maintenance Record";
        ScriptManager.RegisterStartupScript(this, GetType(), "openModal", "openMmModal();", true);
    }

    protected void gvMaintenance_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int maintenanceId = Convert.ToInt32(e.CommandArgument);

        if (e.CommandName == "EditMaintenance")
        {
            LoadRecordIntoForm(maintenanceId);
            mmModalTitle.InnerText = "Edit Maintenance Record";
            ScriptManager.RegisterStartupScript(this, GetType(), "openModal", "openMmModal();", true);
        }
        else if (e.CommandName == "DeleteMaintenance")
        {
            DeleteRecord(maintenanceId);
            BindGrid();
        }
    }

    private void ClearForm()
    {
        hfMaintenanceID.Value = "0";
        litMaintenanceCode.Text = FormatMaintenanceCode(GetNextMaintenanceId());
        ddlMachine.SelectedValue = "";
        ddlRecordType.SelectedValue = "Scheduled";
        ddlMaintenanceType.SelectedValue = "Preventive";
        ddlStatus.SelectedValue = "Pending";
        txtIssueDescription.Text = "";
        txtBreakdownDate.Text = "";
        txtScheduledDate.Text = "";
        txtRepairDetails.Text = "";
        ddlAssignedTo.SelectedValue = "";
        txtCompletedDate.Text = "";
        txtCost.Text = "";
        txtRemarks.Text = "";
    }

    private void LoadRecordIntoForm(int maintenanceId)
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            string sql = "SELECT * FROM CmsMachineMaintenance WHERE MaintenanceID = @id";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", maintenanceId);
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                hfMaintenanceID.Value = maintenanceId.ToString();
                litMaintenanceCode.Text = FormatMaintenanceCode(maintenanceId);

                ddlMachine.SelectedValue = rdr["MachineID"].ToString();
                ddlRecordType.SelectedValue = rdr["RecordType"] != DBNull.Value ? rdr["RecordType"].ToString() : "Scheduled";
                ddlMaintenanceType.SelectedValue = rdr["MaintenanceType"] != DBNull.Value ? rdr["MaintenanceType"].ToString() : "Preventive";
                ddlStatus.SelectedValue = rdr["Status"].ToString();
                txtIssueDescription.Text = rdr["IssueDescription"] != DBNull.Value ? rdr["IssueDescription"].ToString() : "";

                txtBreakdownDate.Text = rdr["BreakdownDate"] != DBNull.Value
                    ? Convert.ToDateTime(rdr["BreakdownDate"]).ToString("yyyy-MM-ddTHH:mm") : "";
                txtScheduledDate.Text = rdr["ScheduledDate"] != DBNull.Value
                    ? Convert.ToDateTime(rdr["ScheduledDate"]).ToString("yyyy-MM-dd") : "";
                txtRepairDetails.Text = rdr["RepairDetails"] != DBNull.Value ? rdr["RepairDetails"].ToString() : "";

                ddlAssignedTo.SelectedValue = rdr["AssignedToID"] != DBNull.Value ? rdr["AssignedToID"].ToString() : "";
                txtCompletedDate.Text = rdr["CompletedDate"] != DBNull.Value
                    ? Convert.ToDateTime(rdr["CompletedDate"]).ToString("yyyy-MM-dd") : "";
                txtCost.Text = rdr["Cost"] != DBNull.Value ? rdr["Cost"].ToString() : "";
                txtRemarks.Text = rdr["Remarks"] != DBNull.Value ? rdr["Remarks"].ToString() : "";
            }
        }
    }

    protected void btnSaveMaintenance_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(ddlMachine.SelectedValue))
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "err",
                "alert('Please select a machine.'); openMmModal();", true);
            return;
        }
        if (ddlRecordType.SelectedValue == "Maintenance" &&
    string.IsNullOrEmpty(txtScheduledDate.Text))
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "err",
                "alert('Please select Scheduled Date for maintenance.'); openMmModal();", true);
            return;
        }

        int maintenanceId = Convert.ToInt32(hfMaintenanceID.Value);

        using (SqlConnection con = new SqlConnection(connStr))
        {
            con.Open();
            SqlCommand cmd;

            if (maintenanceId == 0)
            {
                string sql = @"
                    INSERT INTO CmsMachineMaintenance
                        (MachineID, ScheduledDate, Status, RecordType, MaintenanceType, CompletedDate,
                         IssueDescription, AssignedToID, Cost, Remarks, CreatedDate, CreatedBy, BreakdownDate, RepairDetails)
                    VALUES
                        (@MachineID, @ScheduledDate, @Status, @RecordType, @MaintenanceType, @CompletedDate,
                         @IssueDescription, @AssignedToID, @Cost, @Remarks, GETDATE(), @CreatedBy, @BreakdownDate, @RepairDetails)";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@CreatedBy", GetCurrentUserId());
            }
            else
            {
                string sql = @"
                    UPDATE CmsMachineMaintenance SET
                        MachineID = @MachineID,
                        ScheduledDate = @ScheduledDate,
                        Status = @Status,
                        RecordType = @RecordType,
                        MaintenanceType = @MaintenanceType,
                        CompletedDate = @CompletedDate,
                        IssueDescription = @IssueDescription,
                        AssignedToID = @AssignedToID,
                        Cost = @Cost,
                        Remarks = @Remarks,
                        BreakdownDate = @BreakdownDate,
                        RepairDetails = @RepairDetails
                    WHERE MaintenanceID = @MaintenanceID";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaintenanceID", maintenanceId);
            }

            cmd.Parameters.AddWithValue("@MachineID", Convert.ToInt32(ddlMachine.SelectedValue));
            cmd.Parameters.AddWithValue("@ScheduledDate",
                string.IsNullOrEmpty(txtScheduledDate.Text) ? (object)DBNull.Value : Convert.ToDateTime(txtScheduledDate.Text));
            cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);
            cmd.Parameters.AddWithValue("@RecordType", ddlRecordType.SelectedValue);
            cmd.Parameters.AddWithValue("@MaintenanceType", ddlMaintenanceType.SelectedValue);
            cmd.Parameters.AddWithValue("@CompletedDate",
                string.IsNullOrEmpty(txtCompletedDate.Text) ? (object)DBNull.Value : Convert.ToDateTime(txtCompletedDate.Text));
            cmd.Parameters.AddWithValue("@IssueDescription",
                string.IsNullOrEmpty(txtIssueDescription.Text) ? (object)DBNull.Value : txtIssueDescription.Text);
            cmd.Parameters.AddWithValue("@AssignedToID",
                string.IsNullOrEmpty(ddlAssignedTo.SelectedValue) ? (object)DBNull.Value : Convert.ToInt32(ddlAssignedTo.SelectedValue));
            cmd.Parameters.AddWithValue("@Cost",
                string.IsNullOrEmpty(txtCost.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtCost.Text));
            cmd.Parameters.AddWithValue("@Remarks",
                string.IsNullOrEmpty(txtRemarks.Text) ? (object)DBNull.Value : txtRemarks.Text);
            cmd.Parameters.AddWithValue("@BreakdownDate",
                string.IsNullOrEmpty(txtBreakdownDate.Text) ? (object)DBNull.Value : Convert.ToDateTime(txtBreakdownDate.Text));
            cmd.Parameters.AddWithValue("@RepairDetails",
                string.IsNullOrEmpty(txtRepairDetails.Text) ? (object)DBNull.Value : txtRepairDetails.Text);

            cmd.ExecuteNonQuery();
        }

        BindGrid();
        ScriptManager.RegisterStartupScript(this, GetType(), "closeModal", "closeMmModal();", true);
    }

    private void DeleteRecord(int maintenanceId)
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            string sql = "DELETE FROM CmsMachineMaintenance WHERE MaintenanceID = @id";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", maintenanceId);
            con.Open();
            cmd.ExecuteNonQuery();
        }
    }

    private int GetCurrentUserId()
    {
        if (Session["UserID"] != null)
        {
            return Convert.ToInt32(Session["UserID"]);
        }
        return 0;
    }

    #endregion
}