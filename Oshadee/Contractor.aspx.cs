using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CleaningManagement_Masters_Contractor : System.Web.UI.Page
{
    // Reads the same connection string used across the CMS (web.config -> CmsConnectionString)
    private string ConnStr
    {
        get { return ConfigurationManager.ConnectionStrings["CmsConnectionString"].ConnectionString; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            GenerateNextContractorID();
            BindContractorsGrid();
        }
    }

    private void BindContractorsGrid()
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        {
            string query = @"
                SELECT
                    ContractorID,
                    ContractorName,
                    ContractCategory,
                    ContactPerson,
                    ContractStartDate,
                    ContractEndDate
                FROM CmsAmcContractor
                ORDER BY ContractorID DESC";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dt.Columns.Add("ContractStatus", typeof(string));

                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["ContractEndDate"] == DBNull.Value)
                        {
                            row["ContractStatus"] = "Ongoing";
                        }
                        else
                        {
                            DateTime endDate = Convert.ToDateTime(row["ContractEndDate"]);
                            row["ContractStatus"] = (endDate.Date >= DateTime.Today) ? "Active" : "Expired";
                        }
                    }

                    gvContractors.DataSource = dt;
                    gvContractors.DataBind();
                }
            }
        }
    }

 
    private void GenerateNextContractorID()
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        {
            string query = "SELECT ISNULL(MAX(ContractorID),0)+1 FROM CmsAmcContractor";
            SqlCommand cmd = new SqlCommand(query, con);

            con.Open();

            int nextID = Convert.ToInt32(cmd.ExecuteScalar());

            txtContractorID.Text = "CON-" + nextID.ToString("000");
        }
    }



    protected void btnSaveContractor_Click(object sender, EventArgs e)
    {
        string contractorName = txtContractorName.Text.Trim();

        if (string.IsNullOrEmpty(contractorName))
        {
            lblMessage.Text = "Please enter a contractor / company name before saving.";
            lblMessage.ForeColor = System.Drawing.Color.Red;
            return;
        }

        string contractCategory = txtContractCategory.Text.Trim();
        string contactPerson = txtContactPerson.Text.Trim();

        object startDate = string.IsNullOrEmpty(txtContractStartDate.Text)
            ? (object)DBNull.Value
            : Convert.ToDateTime(txtContractStartDate.Text);

        object endDate = string.IsNullOrEmpty(txtContractEndDate.Text)
            ? (object)DBNull.Value
            : Convert.ToDateTime(txtContractEndDate.Text);

        int contractorId = Convert.ToInt32(hdnContractorID.Value);

        using (SqlConnection con = new SqlConnection(ConnStr))
        {
            SqlCommand cmd;

            if (contractorId == 0)
            {
                // ---------- INSERT new contractor ----------
                string insertQuery = @"
                    INSERT INTO CmsAmcContractor
                        (ContractorName, ContractCategory, ContactPerson, ContractStartDate, ContractEndDate)
                    VALUES
                        (@ContractorName, @ContractCategory, @ContactPerson, @ContractStartDate, @ContractEndDate)";

                cmd = new SqlCommand(insertQuery, con);
            }
            else
            {
                // ---------- UPDATE existing contractor ----------
                string updateQuery = @"
                    UPDATE CmsAmcContractor
                    SET ContractorName = @ContractorName,
                        ContractCategory = @ContractCategory,
                        ContactPerson = @ContactPerson,
                        ContractStartDate = @ContractStartDate,
                        ContractEndDate = @ContractEndDate
                    WHERE ContractorID = @ContractorID";

                cmd = new SqlCommand(updateQuery, con);
                cmd.Parameters.AddWithValue("@ContractorID", contractorId);
            }

            cmd.Parameters.AddWithValue("@ContractorName", contractorName);
            cmd.Parameters.AddWithValue("@ContractCategory",
                string.IsNullOrEmpty(contractCategory) ? (object)DBNull.Value : contractCategory);
            cmd.Parameters.AddWithValue("@ContactPerson",
                string.IsNullOrEmpty(contactPerson) ? (object)DBNull.Value : contactPerson);
            cmd.Parameters.AddWithValue("@ContractStartDate", startDate);
            cmd.Parameters.AddWithValue("@ContractEndDate", endDate);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        ResetForm();
        BindContractorsGrid();

        lblMessage.ForeColor = System.Drawing.Color.Green;
        lblMessage.Text = (contractorId == 0) ? "Contractor registered successfully." : "Contractor details updated successfully.";

        ClientScript.RegisterStartupScript(
      this.GetType(),
      "hideFormAfterSave",
      "toggleStaffForm(false);",
      true);
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ResetForm();

        btnSaveContractor.Text = "Register Contractor";
        btnSaveContractor.CssClass = "btn-save";

        ScriptManager.RegisterStartupScript(
            this,
            this.GetType(),
            "hideFormOnCancel",
            "toggleContractorForm(false);",
            true);
    }

    protected void gvContractors_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "EditContractor")
        {
            int contractorId = Convert.ToInt32(e.CommandArgument);

            LoadContractorForEdit(contractorId);


            btnSaveContractor.Text = "Update Contractor";
            btnSaveContractor.CssClass = "btn-update";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "showFormForEdit",
                "toggleContractorForm(true);", true);
        }
    }

    private void LoadContractorForEdit(int contractorId)
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        {
            string query = @"
            SELECT ContractorID, ContractorName, ContractCategory, ContactPerson, ContractStartDate, ContractEndDate
            FROM CmsAmcContractor
            WHERE ContractorID = @ContractorID";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@ContractorID", contractorId);
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        hdnContractorID.Value = reader["ContractorID"].ToString();

                      
                        int id = Convert.ToInt32(reader["ContractorID"]);
                        txtContractorID.Text = "CON-" + id.ToString("000");

                        txtContractorName.Text = reader["ContractorName"].ToString();
                        txtContractCategory.Text = reader["ContractCategory"] == DBNull.Value ? "" : reader["ContractCategory"].ToString();
                        txtContactPerson.Text = reader["ContactPerson"] == DBNull.Value ? "" : reader["ContactPerson"].ToString();
                        txtContractStartDate.Text = reader["ContractStartDate"] == DBNull.Value
                            ? "" : Convert.ToDateTime(reader["ContractStartDate"]).ToString("yyyy-MM-dd");
                        txtContractEndDate.Text = reader["ContractEndDate"] == DBNull.Value
                            ? "" : Convert.ToDateTime(reader["ContractEndDate"]).ToString("yyyy-MM-dd");

                        lblFormHeading.Text = "Edit Contractor #" + contractorId;
                    }
                }
            }
        }
    }


    private void ResetForm()
    {
        hdnContractorID.Value = "0";

        txtContractorName.Text = "";
        txtContractCategory.Text = "";
        txtContactPerson.Text = "";
        txtContractStartDate.Text = "";
        txtContractEndDate.Text = "";

        lblFormHeading.Text = "Register New Contractor";

        // Reset button back to orange Save mode
        btnSaveContractor.Text = "Save Contractor";
        btnSaveContractor.CssClass = "btn-save";
    }
}