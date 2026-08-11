using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CleaningManagement_Masters_Contractor : System.Web.UI.Page
{
    private string ConnStr
    {
        get { return ConfigurationManager.ConnectionStrings["CmsConnectionString"].ConnectionString; }
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
            BindContractorsGrid();
        }
    }

    // Grid
    private void BindContractorsGrid()
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetContractors", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            // Calculate contract status client-side
            dt.Columns.Add("ContractStatus", typeof(string));
            foreach (DataRow row in dt.Rows)
            {
                if (row["ContractEndDate"] == DBNull.Value)
                    row["ContractStatus"] = "Ongoing";
                else
                {
                    DateTime endDate = Convert.ToDateTime(row["ContractEndDate"]);
                    row["ContractStatus"] = endDate.Date >= DateTime.Today
                        ? "Active" : "Expired";
                }
            }

            gvContractors.DataSource = dt;
            gvContractors.DataBind();
        }
    }

    //  Save / Update 
    protected void btnSaveContractor_Click(object sender, EventArgs e)
    {
        string contractorName = txtContractorName.Text.Trim();

        if (string.IsNullOrEmpty(contractorName))
        {
            ShowMessage("Please enter a contractor / company name before saving.", false);
            ScriptManager.RegisterStartupScript(this, GetType(),
                "keepOpen", "toggleContractorForm(true);", true);
            return;
        }

        string contractCategory = txtContractCategory.Text.Trim();
        string contactPerson = txtContactPerson.Text.Trim();
        int contractorId = Convert.ToInt32(hdnContractorID.Value);

        DateTime? startDt = null;
        DateTime? endDt = null;

        if (!string.IsNullOrEmpty(txtContractStartDate.Text))
        {
            DateTime parsedStart;
            if (DateTime.TryParse(txtContractStartDate.Text, out parsedStart))
                startDt = parsedStart;
            else
            {
                ShowMessage("Invalid Contract Start Date format.", false);
                ScriptManager.RegisterStartupScript(this, GetType(), "keepOpen", "toggleContractorForm(true);", true);
                return;
            }
        }

        if (!string.IsNullOrEmpty(txtContractEndDate.Text))
        {
            DateTime parsedEnd;
            if (DateTime.TryParse(txtContractEndDate.Text, out parsedEnd))
                endDt = parsedEnd;
            else
            {
                ShowMessage("Invalid Contract End Date format.", false);
                ScriptManager.RegisterStartupScript(this, GetType(), "keepOpen", "toggleContractorForm(true);", true);
                return;
            }
        }

        if (startDt.HasValue && endDt.HasValue && endDt.Value < startDt.Value)
        {
            ShowMessage("Contract End Date cannot be earlier than Contract Start Date.", false);
            ScriptManager.RegisterStartupScript(this, GetType(), "keepOpen", "toggleContractorForm(true);", true);
            return;
        }

        object startDate = startDt.HasValue ? (object)startDt.Value : DBNull.Value;
        object endDate = endDt.HasValue ? (object)endDt.Value : DBNull.Value;

        using (SqlConnection con = new SqlConnection(ConnStr))
        {
            SqlCommand cmd;

            if (contractorId == 0)
            {
                cmd = new SqlCommand("SP_CMS_InsertContractor", con);
                cmd.CommandType = CommandType.StoredProcedure;
            }
            else
            {
                cmd = new SqlCommand("SP_CMS_UpdateContractor", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ContractorID", contractorId);
            }

            cmd.Parameters.AddWithValue("@ContractorName", contractorName);
            cmd.Parameters.AddWithValue("@ContractCategory",
                string.IsNullOrEmpty(contractCategory)
                    ? (object)DBNull.Value : contractCategory);
            cmd.Parameters.AddWithValue("@ContactPerson",
                string.IsNullOrEmpty(contactPerson)
                    ? (object)DBNull.Value : contactPerson);
            cmd.Parameters.AddWithValue("@ContractStartDate", startDate);
            cmd.Parameters.AddWithValue("@ContractEndDate", endDate);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        string msg = contractorId == 0
            ? "Contractor registered successfully."
            : "Contractor details updated successfully.";

        ResetForm();
        BindContractorsGrid();
        ShowMessage(msg, true);

        ScriptManager.RegisterStartupScript(this, GetType(),
            "hideForm", "toggleContractorForm(false);", true);
    }

    // Cancel 
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ResetForm();
        ScriptManager.RegisterStartupScript(this, GetType(),
            "hideForm", "toggleContractorForm(false);", true);
    }

    // Grid row command 
    protected void gvContractors_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "EditContractor")
        {
            int contractorId = Convert.ToInt32(e.CommandArgument);
            LoadContractorForEdit(contractorId);
            btnSaveContractor.Text = "Update Contractor";
            btnSaveContractor.CssClass = "btn-update";
            ScriptManager.RegisterStartupScript(this, GetType(),
                "showForm", "toggleContractorForm(true);", true);
        }
        else if (e.CommandName == "RemoveContractor")
        {
            int contractorId = Convert.ToInt32(e.CommandArgument);
            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand("SP_CMS_DeleteContractor", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ContractorID", contractorId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            BindContractorsGrid();
            ShowMessage("Contractor removed.", true);
        }
    }

    //  Load for edit 
    private void LoadContractorForEdit(int contractorId)
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetContractorByID", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ContractorID", contractorId);
            con.Open();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    hdnContractorID.Value = reader["ContractorID"].ToString();
                    txtContractorName.Text = reader["ContractorName"].ToString();
                    txtContractCategory.Text = reader["ContractCategory"] == DBNull.Value
                        ? "" : reader["ContractCategory"].ToString();
                    txtContactPerson.Text = reader["ContactPerson"] == DBNull.Value
                        ? "" : reader["ContactPerson"].ToString();
                    txtContractStartDate.Text = reader["ContractStartDate"] == DBNull.Value
                        ? "" : Convert.ToDateTime(reader["ContractStartDate"])
                            .ToString("yyyy-MM-dd");
                    txtContractEndDate.Text = reader["ContractEndDate"] == DBNull.Value
                        ? "" : Convert.ToDateTime(reader["ContractEndDate"])
                            .ToString("yyyy-MM-dd");
                    lblFormHeading.Text = "Edit Contractor #" + contractorId;
                }
            }
        }
    }

    // Reset form 
    private void ResetForm()
    {
        hdnContractorID.Value = "0";
        txtContractorName.Text = "";
        txtContractCategory.Text = "";
        txtContactPerson.Text = "";
        txtContractStartDate.Text = "";
        txtContractEndDate.Text = "";
        lblFormHeading.Text = "Register New Contractor";
        btnSaveContractor.Text = "Save Contractor";
        btnSaveContractor.CssClass = "btn-save";
    }

    //  Helper 
    private void ShowMessage(string message, bool success)
    {
        lblMessage.Text = message;
        lblMessage.ForeColor = success
            ? System.Drawing.Color.Green
            : System.Drawing.Color.Red;
    }
}