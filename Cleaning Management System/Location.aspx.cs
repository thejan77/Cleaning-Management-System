using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CleaningManagement_Masters_Location : System.Web.UI.Page
{
    private string ConnStr
    {
        get { return ConfigurationManager.ConnectionStrings["CmsConnectionString"].ConnectionString; }
    }

    private int CurrentUserID
    {
        get { return Session["UserID"] != null ? Convert.ToInt32(Session["UserID"]) : 0; }
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
            BindLocationsGrid();
        }
    }

    // ── Load grid ──
    private void BindLocationsGrid()
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetLocations", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            gvLocations.DataSource = dt;
            gvLocations.DataBind();
        }
    }

    // ── Save / Update ──
    protected void btnSaveLocation_Click(object sender, EventArgs e)
    {
        string locationName = txtLocationName.Text.Trim();

        if (string.IsNullOrEmpty(locationName))
        {
            ShowMessage("Please enter a Location Name before saving.", false);
            ScriptManager.RegisterStartupScript(this, GetType(),
                "keepOpen", "toggleLocationForm(true);", true);
            return;
        }

        int locationId = Convert.ToInt32(hdnLocationID.Value);
        string description = txtDescription.Text.Trim();
        short active = Convert.ToInt16(ddlActive.SelectedValue);

        using (SqlConnection con = new SqlConnection(ConnStr))
        {
            SqlCommand cmd;

            if (locationId == 0)
            {
                cmd = new SqlCommand("SP_CMS_InsertLocation", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@LocationName", locationName);
                cmd.Parameters.AddWithValue("@Description",
                    string.IsNullOrEmpty(description)
                        ? (object)DBNull.Value : description);
                cmd.Parameters.AddWithValue("@Active", active);
                cmd.Parameters.AddWithValue("@CreatedBy", CurrentUserID);
            }
            else
            {
                cmd = new SqlCommand("SP_CMS_UpdateLocation", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@LocationID", locationId);
                cmd.Parameters.AddWithValue("@LocationName", locationName);
                cmd.Parameters.AddWithValue("@Description",
                    string.IsNullOrEmpty(description)
                        ? (object)DBNull.Value : description);
                cmd.Parameters.AddWithValue("@Active", active);
                cmd.Parameters.AddWithValue("@UpdatedBy", CurrentUserID);
            }

            con.Open();
            int result = Convert.ToInt32(cmd.ExecuteScalar());

            if (result == -1)
            {
                ShowMessage("A location with this name already exists.", false);
                ScriptManager.RegisterStartupScript(this, GetType(),
                    "keepOpen", "toggleLocationForm(true);", true);
                return;
            }
        }

        string msg = locationId == 0
            ? "Location added successfully."
            : "Location updated successfully.";

        ResetForm();
        BindLocationsGrid();
        ShowMessage(msg, true);

        ScriptManager.RegisterStartupScript(this, GetType(),
            "hideForm", "toggleLocationForm(false);", true);
    }

    // ── Cancel ──
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ResetForm();
        ScriptManager.RegisterStartupScript(this, GetType(),
            "hideForm", "toggleLocationForm(false);", true);
    }

    // ── Edit row ──
    protected void gvLocations_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "RemoveLocation")
        {
            int locationId = Convert.ToInt32(e.CommandArgument);
            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand("SP_CMS_DeleteLocation", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@LocationID", locationId);
                con.Open();
                int result = Convert.ToInt32(cmd.ExecuteScalar());
                if (result == -1)
                {
                    ShowMessage("Cannot remove — sections exist under this location. Remove sections first.", false);
                    return;
                }
            }
            BindLocationsGrid();
            ShowMessage("Location removed.", true);
        }
        else if (e.CommandName == "EditLocation")
        {
            int locationId = Convert.ToInt32(e.CommandArgument);
            LoadLocationForEdit(locationId);
            btnSaveLocation.Text = "Update Location";
            btnSaveLocation.CssClass = "btn-update";
            ScriptManager.RegisterStartupScript(this, GetType(),
                "showForm", "toggleLocationForm(true);", true);
        }
    }

    private void LoadLocationForEdit(int locationId)
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand("SP_CMS_GetLocationByID", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@LocationID", locationId);
            con.Open();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    hdnLocationID.Value = reader["LocationID"].ToString();
                    txtLocationName.Text = reader["LocationName"].ToString();
                    txtDescription.Text = reader["Description"] == DBNull.Value
                        ? "" : reader["Description"].ToString();
                    ddlActive.SelectedValue = reader["Active"].ToString();
                    lblFormHeading.Text = "Edit Location #" + locationId;
                }
            }
        }
    }

    // ── Helpers ──
    private void ResetForm()
    {
        hdnLocationID.Value = "0";
        txtLocationName.Text = "";
        txtDescription.Text = "";
        ddlActive.SelectedValue = "1";
        lblFormHeading.Text = "Add New Location";
        btnSaveLocation.Text = "Save Location";
        btnSaveLocation.CssClass = "btn-save";
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