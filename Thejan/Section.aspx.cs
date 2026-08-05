using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CleaningManagement_Masters_Section : System.Web.UI.Page
{
    private string ConnStr
    {
        get { return ConfigurationManager.ConnectionStrings["CmsConnectionString"].ConnectionString; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadLocationDropdown();
            BindSectionsGrid();
        }
    }

    private void LoadLocationDropdown()
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetActiveLocations", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            ddlLocation.DataSource = dt;
            ddlLocation.DataTextField = "LocationName";
            ddlLocation.DataValueField = "LocationID";
            ddlLocation.DataBind();
            ddlLocation.Items.Insert(0, new ListItem("-- Select --", "0"));
        }
    }

    private void BindSectionsGrid()
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetSections", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            gvSections.DataSource = dt;
            gvSections.DataBind();
        }
    }

    protected void btnSaveSection_Click(object sender, EventArgs e)
    {
        int locationId = Convert.ToInt32(ddlLocation.SelectedValue);
        string sectionType = ddlSectionType.SelectedValue;
        string sectionName = txtSectionName.Text.Trim();
        string description = txtDescription.Text.Trim();

        if (locationId == 0 || string.IsNullOrEmpty(sectionType) || string.IsNullOrEmpty(sectionName))
        {
            ShowMessage("Please select a Location, a Section Type, and enter a Section Name.", false);
            ScriptManager.RegisterStartupScript(this, GetType(),
                "keepOpen", "toggleSectionForm(true);", true);
            return;
        }

        int sectionId = Convert.ToInt32(hdnSectionID.Value);

        using (SqlConnection con = new SqlConnection(ConnStr))
        {
            SqlCommand cmd;

            if (sectionId == 0)
            {
                cmd = new SqlCommand("SP_CMS_InsertSection", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@LocationID", locationId);
                cmd.Parameters.AddWithValue("@SectionName", sectionName);
                cmd.Parameters.AddWithValue("@SectionType", sectionType);
                cmd.Parameters.AddWithValue("@Description",
                    string.IsNullOrEmpty(description) ? (object)DBNull.Value : description);
            }
            else
            {
                cmd = new SqlCommand("SP_CMS_UpdateSection", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SectionID", sectionId);
                cmd.Parameters.AddWithValue("@LocationID", locationId);
                cmd.Parameters.AddWithValue("@SectionName", sectionName);
                cmd.Parameters.AddWithValue("@SectionType", sectionType);
                cmd.Parameters.AddWithValue("@Description",
                    string.IsNullOrEmpty(description) ? (object)DBNull.Value : description);
            }

            con.Open();
            cmd.ExecuteNonQuery();
        }

        string msg = sectionId == 0
            ? "Section added successfully."
            : "Section updated successfully.";

        ResetForm();
        BindSectionsGrid();
        ShowMessage(msg, true);

        ScriptManager.RegisterStartupScript(this, GetType(),
            "hideForm", "toggleSectionForm(false);", true);
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ResetForm();
        ScriptManager.RegisterStartupScript(this, GetType(),
            "hideForm", "toggleSectionForm(false);", true);
    }

    protected void gvSections_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "EditSection")
        {
            int sectionId = Convert.ToInt32(e.CommandArgument);
            LoadSectionForEdit(sectionId);
            btnSaveSection.Text = "Update Section";
            btnSaveSection.CssClass = "btn-update";
            ScriptManager.RegisterStartupScript(this, GetType(),
                "showForm", "toggleSectionForm(true);", true);
        }
    }

    private void LoadSectionForEdit(int sectionId)
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetSectionByID", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SectionID", sectionId);
            con.Open();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    hdnSectionID.Value = reader["SectionID"].ToString();

                    string locationId = reader["LocationID"].ToString();
                    if (ddlLocation.Items.FindByValue(locationId) == null)
                    {
                        ddlLocation.Items.Insert(1, new ListItem("(Inactive) Location #" + locationId, locationId));
                    }
                    ddlLocation.SelectedValue = locationId;

                    ddlSectionType.SelectedValue = reader["SectionType"].ToString();
                    txtSectionName.Text = reader["SectionName"].ToString();
                    txtDescription.Text = reader["Description"] == DBNull.Value
                        ? "" : reader["Description"].ToString();

                    lblFormHeading.Text = "Edit Section #" + sectionId;
                }
            }
        }
    }

    private void ResetForm()
    {
        hdnSectionID.Value = "0";
        ddlLocation.SelectedValue = "0";
        ddlSectionType.SelectedValue = "";
        txtSectionName.Text = "";
        txtDescription.Text = "";

        lblFormHeading.Text = "Add New Section";
        btnSaveSection.Text = "Save Section";
        btnSaveSection.CssClass = "btn-save";
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