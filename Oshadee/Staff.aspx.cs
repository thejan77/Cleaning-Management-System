using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

public partial class CleaningManagement_Masters_Staff : System.Web.UI.Page
{
    private string ConnStr
    {
        get
        {
            return ConfigurationManager.ConnectionStrings["CmsConnectionString"].ConnectionString;
        }
    }


    // Stores selected Job Title tab
    private string SelectedJobTitle
    {
        get
        {
            return ViewState["SelectedJobTitle"] == null
                ? ""
                : ViewState["SelectedJobTitle"].ToString();
        }

        set
        {
            ViewState["SelectedJobTitle"] = value;
        }
    }


    // Stores Staff ID search value
    private string SearchStaffID
    {
        get
        {
            return ViewState["SearchStaffID"] == null
                ? ""
                : ViewState["SearchStaffID"].ToString();
        }

        set
        {
            ViewState["SearchStaffID"] = value;
        }
    }


    // Stores Staff Name search value
    private string SearchName
    {
        get
        {
            return ViewState["SearchName"] == null
                ? ""
                : ViewState["SearchName"].ToString();
        }

        set
        {
            ViewState["SearchName"] = value;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindRoleTabs();
            BindStaffGrid();
        }
    }



   
    private void BindRoleTabs()
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        {
            string query = @"
                SELECT DISTINCT JobTitle
                FROM CmsStaff
                WHERE JobTitle IS NOT NULL
                AND JobTitle <> ''
                ORDER BY JobTitle";


            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);


                    DataTable tabsTable = new DataTable();

                    tabsTable.Columns.Add(
                        "JobTitleValue",
                        typeof(string)
                    );

                    tabsTable.Columns.Add(
                        "IsSelected",
                        typeof(bool)
                    );


                    // All tab
                    tabsTable.Rows.Add(
                        "All",
                        SelectedJobTitle == ""
                    );


                    foreach (DataRow row in dt.Rows)
                    {
                        string jobTitle =
                            row["JobTitle"].ToString();


                        tabsTable.Rows.Add(
                            jobTitle,
                            jobTitle == SelectedJobTitle
                        );
                    }


                    rptRoleTabs.DataSource = tabsTable;
                    rptRoleTabs.DataBind();

                }
            }
        }
    }




    private void BindStaffGrid()
    {
        using (SqlConnection con = new SqlConnection(ConnStr))
        {

            string query = @"
                SELECT
                    st.StaffID,
                    st.Name,
                    st.ContactNumber,
                    st.JobTitle,
                    t.TeamName,
                    c.ContractorName

                FROM CmsStaff st

                LEFT JOIN CmsTeam t
                ON st.TeamID = t.TeamID

                LEFT JOIN CmsAmcContractor c
                ON st.ContractorID = c.ContractorID


                WHERE
                (@JobTitle = ''
                OR st.JobTitle = @JobTitle)

                AND (@StaffID = ''
                OR CAST(st.StaffID AS NVARCHAR(20)) LIKE '%' + @StaffID + '%')

                AND (@Name = ''
                OR st.Name LIKE '%' + @Name + '%')


                ORDER BY st.StaffID DESC";


            using (SqlCommand cmd = new SqlCommand(query, con))
            {

                cmd.Parameters.AddWithValue(
                    "@JobTitle",
                    SelectedJobTitle
                );

                cmd.Parameters.AddWithValue(
                    "@StaffID",
                    SearchStaffID
                );

                cmd.Parameters.AddWithValue(
                    "@Name",
                    SearchName
                );


                con.Open();


                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {

                    DataTable dt = new DataTable();

                    da.Fill(dt);


                    gvStaff.DataSource = dt;

                    gvStaff.DataBind();

                }
            }
        }
    }





    protected void rptRoleTabs_ItemCommand(
        object source,
        RepeaterCommandEventArgs e)
    {

        if (e.CommandName == "FilterRole")
        {

            string selected =
                e.CommandArgument.ToString();


            SelectedJobTitle =
                selected == "All"
                ? ""
                : selected;



            BindRoleTabs();

            BindStaffGrid();

        }

    }




    protected void btnSearchStaff_Click(object sender, EventArgs e)
    {
        
        SearchName = txtSearchName.Text.Trim();

        BindRoleTabs();
        BindStaffGrid();
    }

    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        txtSearchName.Text = "";
        SearchName = "";
        SelectedJobTitle = ""; 

        BindRoleTabs();
        BindStaffGrid();
    }
}