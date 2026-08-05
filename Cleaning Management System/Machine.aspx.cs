using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CleaningManagement_Masters_Machine : System.Web.UI.Page
{
    private string ConnStr
    {
        get { return ConfigurationManager.ConnectionStrings["CmsConnectionString"].ConnectionString; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        //  Auth guard: Admin only
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
            LoadSectionDropdown();
            LoadContractorDropdown();
            BindMachinesGrid();
        }
    }

    private void LoadSectionDropdown()
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetAllSections", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            ddlSection.DataSource = dt;
            ddlSection.DataTextField = "SectionName";
            ddlSection.DataValueField = "SectionID";
            ddlSection.DataBind();
            ddlSection.Items.Insert(0, new ListItem("-- Select Section --", "0"));
        }
    }

    private void LoadContractorDropdown()
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetAllContractors", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();

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

    private void BindMachinesGrid()
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetMachines", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            gvMachines.DataSource = dt;
            gvMachines.DataBind();
        }
    }

    protected void btnSaveMachine_Click(object sender, EventArgs e)
    {
        string machineName = txtMachineName.Text.Trim();
        int sectionId = Convert.ToInt32(ddlSection.SelectedValue);

        if (string.IsNullOrEmpty(machineName) || sectionId == 0)
        {
            ShowMessage("Please enter a Machine Name and select a Section.", false);
            ScriptManager.RegisterStartupScript(this, GetType(),
                "keepOpen", "toggleMachineForm(true);", true);
            return;
        }

        string machineType = txtMachineType.Text.Trim();
        string serialNumber = txtSerialNumber.Text.Trim();
        int contractorId = Convert.ToInt32(ddlContractor.SelectedValue);
        string machineStatus = ddlMachineStatus.SelectedValue;
        string description = txtDescription.Text.Trim();

        object purchaseDate = string.IsNullOrEmpty(txtPurchaseDate.Text)
            ? (object)DBNull.Value
            : Convert.ToDateTime(txtPurchaseDate.Text);

        int machineId = Convert.ToInt32(hdnMachineID.Value);

        using (SqlConnection con = new SqlConnection(ConnStr))
        {
            SqlCommand cmd;

            if (machineId == 0)
            {
                cmd = new SqlCommand("SP_CMS_InsertMachine", con);
                cmd.CommandType = CommandType.StoredProcedure;
            }
            else
            {
                cmd = new SqlCommand("SP_CMS_UpdateMachine", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MachineID", machineId);
            }

            cmd.Parameters.AddWithValue("@SectionID", sectionId);
            cmd.Parameters.AddWithValue("@MachineName", machineName);
            cmd.Parameters.AddWithValue("@MachineType",
                string.IsNullOrEmpty(machineType) ? (object)DBNull.Value : machineType);
            cmd.Parameters.AddWithValue("@SerialNumber",
                string.IsNullOrEmpty(serialNumber) ? (object)DBNull.Value : serialNumber);
            cmd.Parameters.AddWithValue("@ContractorID",
                contractorId == 0 ? (object)DBNull.Value : contractorId);
            cmd.Parameters.AddWithValue("@MachineStatus", machineStatus);
            cmd.Parameters.AddWithValue("@PurchaseDate", purchaseDate);
            cmd.Parameters.AddWithValue("@Description",
                string.IsNullOrEmpty(description) ? (object)DBNull.Value : description);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        string msg = machineId == 0
            ? "Machine added successfully."
            : "Machine updated successfully.";

        ResetForm();
        BindMachinesGrid();
        ShowMessage(msg, true);

        ScriptManager.RegisterStartupScript(this, GetType(),
            "hideForm", "toggleMachineForm(false);", true);
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ResetForm();
        ScriptManager.RegisterStartupScript(this, GetType(),
            "hideForm", "toggleMachineForm(false);", true);
    }

    protected void gvMachines_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "EditMachine")
        {
            int machineId = Convert.ToInt32(e.CommandArgument);
            LoadMachineForEdit(machineId);
            btnSaveMachine.Text = "Update Machine";
            btnSaveMachine.CssClass = "btn-update";

            ScriptManager.RegisterStartupScript(this, GetType(),
                "showForm", "toggleMachineForm(true);", true);
        }
        else if (e.CommandName == "RemoveMachine")
        {
            int machineId = Convert.ToInt32(e.CommandArgument);

            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand("SP_CMS_DeleteMachine", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MachineID", machineId);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            BindMachinesGrid();
            ShowMessage("Machine removed.", true);
        }
    }

    private void LoadMachineForEdit(int machineId)
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetMachineByID", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MachineID", machineId);
            con.Open();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    hdnMachineID.Value = reader["MachineID"].ToString();
                    txtMachineName.Text = reader["MachineName"].ToString();
                    ddlSection.SelectedValue = reader["SectionID"].ToString();
                    txtMachineType.Text = reader["MachineType"] == DBNull.Value
                        ? "" : reader["MachineType"].ToString();
                    txtSerialNumber.Text = reader["SerialNumber"] == DBNull.Value
                        ? "" : reader["SerialNumber"].ToString();
                    ddlContractor.SelectedValue = reader["ContractorID"] == DBNull.Value
                        ? "0" : reader["ContractorID"].ToString();
                    ddlMachineStatus.SelectedValue = reader["MachineStatus"].ToString();
                    txtPurchaseDate.Text = reader["PurchaseDate"] == DBNull.Value
                        ? "" : Convert.ToDateTime(reader["PurchaseDate"]).ToString("yyyy-MM-dd");
                    txtDescription.Text = reader["Description"] == DBNull.Value
                        ? "" : reader["Description"].ToString();

                    lblFormHeading.Text = "Edit Machine #" + machineId;
                }
            }
        }
    }

    private void ResetForm()
    {
        hdnMachineID.Value = "0";
        txtMachineName.Text = "";
        ddlSection.SelectedValue = "0";
        txtMachineType.Text = "";
        txtSerialNumber.Text = "";
        ddlContractor.SelectedValue = "0";
        ddlMachineStatus.SelectedValue = "Active";
        txtPurchaseDate.Text = "";
        txtDescription.Text = "";

        lblFormHeading.Text = "Add New Machine";
        btnSaveMachine.Text = "Save Machine";
        btnSaveMachine.CssClass = "btn-save";
        lblMessage.Text = "";
    }

    private void ShowMessage(string message, bool success)
    {
        lblMessage.Text = message;
        lblMessage.ForeColor = success
            ? System.Drawing.Color.Green
            : System.Drawing.Color.Red;
    }
}