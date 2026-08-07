using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;

public partial class Reports : Page
{
    private string connStr = ConfigurationManager.ConnectionStrings["CmsConnectionString"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindLookups();
            SetActiveTab("Cleaning");
        }
    }

    #region Lookups

    private void BindLookups()
    {
        using (var con = new SqlConnection(connStr))
        {
            con.Open();

            BindDropDown(con, "SELECT SectionID, SectionName FROM CmsSection ORDER BY SectionName",
                ddlFilterSection, "SectionID", "SectionName", "All Sections");

            BindDropDown(con, "SELECT MachineID, MachineName FROM CmsMachine ORDER BY MachineName",
                ddlFilterMachine, "MachineID", "MachineName", "All Machines");
        }
    }

    private void BindDropDown(SqlConnection con, string sql, DropDownList ddl, string valueField, string textField, string placeholder)
    {
        using (var cmd = new SqlCommand(sql, con))
        using (var da = new SqlDataAdapter(cmd))
        {
            var dt = new DataTable();
            da.Fill(dt);
            ddl.Items.Clear();
            ddl.DataSource = dt;
            ddl.DataTextField = textField;
            ddl.DataValueField = valueField;
            ddl.DataBind();
            ddl.Items.Insert(0, new System.Web.UI.WebControls.ListItem(placeholder, ""));
        }
    }

    #endregion

    #region Tabs

    private void SetActiveTab(string tab)
    {
        ViewState["ActiveTab"] = tab;
        bool isCleaning = tab == "Cleaning";
        pnlCleaningOps.Visible = isCleaning;
        pnlMachineMaint.Visible = !isCleaning;
        btnTabCleaning.CssClass = "cj-tab-btn" + (isCleaning ? " active" : "");
        btnTabMachine.CssClass = "cj-tab-btn" + (!isCleaning ? " active" : "");
    }

    protected void btnTabCleaning_Click(object sender, EventArgs e)
    {
        SetActiveTab("Cleaning");
    }

    protected void btnTabMachine_Click(object sender, EventArgs e)
    {
        SetActiveTab("Machine");
    }

    #endregion

    #region Filters

    protected void btnClearFilters_Click(object sender, EventArgs e)
    {
        ddlFilterSection.SelectedValue = "";
        ddlFilterMachine.SelectedValue = "";
        txtFromDate.Text = "";
        txtToDate.Text = "";
        SetActiveTab((string)(ViewState["ActiveTab"] ?? "Cleaning"));
    }

    private DateTime GetFromDate(string reportKey)
    {
        if (!string.IsNullOrWhiteSpace(txtFromDate.Text))
            return Convert.ToDateTime(txtFromDate.Text);

        var today = DateTime.Today;
        switch (reportKey)
        {
            case "WeeklyCleaning":
                return today.AddDays(-(int)today.DayOfWeek);
            case "MonthlyCleaning":
                return new DateTime(today.Year, today.Month, 1);
            default:
                return today.AddMonths(-1);
        }
    }

    private DateTime GetToDate()
    {
        if (!string.IsNullOrWhiteSpace(txtToDate.Text))
            return Convert.ToDateTime(txtToDate.Text);
        return DateTime.Today;
    }

    #endregion

    #region Report Command

    protected void Report_Command(object sender, CommandEventArgs e)
    {
        string reportKey = e.CommandName;
        string format = e.CommandArgument.ToString();

        try
        {
            string title;
            DataTable dt = GetReportData(reportKey, out title);

            if (dt.Rows.Count == 0)
            {
                litMessage.Text = "<div class='cj-msg err'>No data found for the selected filters.</div>";
                return;
            }

            string fileNameBase = reportKey + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm");

            if (format == "PDF")
                SendPdf(dt, title, fileNameBase + ".pdf");
            else
                SendExcel(dt, title, fileNameBase + ".xlsx");
        }
        catch (Exception ex)
        {
            litMessage.Text = "<div class='cj-msg err'>Could not generate report: " + ex.Message + "</div>";
        }
    }

    #endregion

    #region Report Queries

    private DataTable GetReportData(string reportKey, out string title)
    {
        int sectionId;
        int.TryParse(ddlFilterSection.SelectedValue, out sectionId);
        int machineId;
        int.TryParse(ddlFilterMachine.SelectedValue, out machineId);

        DataTable dt = new DataTable();
        string sql;

        using (var con = new SqlConnection(connStr))
        using (var cmd = new SqlCommand())
        {
            cmd.Connection = con;

            switch (reportKey)
            {
                case "DailyCleaning":
                    title = "Daily Cleaning Report";
                    DateTime dayDate = GetFromDate(reportKey);
                    sql = @"SELECT sec.SectionName AS Section, j.CleaningType AS [Cleaning Type],
                                   j.ScheduledDate AS [Scheduled Date], j.Status, j.Priority,
                                   CASE WHEN j.AssignmentType = 'Staff' THEN st.Name
                                        WHEN j.AssignmentType = 'Team' THEN tm.TeamName
                                        ELSE '-' END AS [Assigned To]
                            FROM CmsJob j
                            JOIN CmsSection sec ON sec.SectionID = j.SectionID
                            LEFT JOIN CmsTeam tm ON tm.TeamID = j.TeamID
                            LEFT JOIN CmsStaff st ON st.StaffID = j.StaffID
                            WHERE j.ScheduledDate = @Date
                              AND (@SectionID = 0 OR j.SectionID = @SectionID)
                            ORDER BY sec.SectionName";
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@Date", dayDate.Date);
                    cmd.Parameters.AddWithValue("@SectionID", sectionId);
                    break;

                case "WeeklyCleaning":
                    title = "Weekly Cleaning Report";
                    sql = @"SELECT sec.SectionName AS Section, j.CleaningType AS [Cleaning Type],
                                   j.ScheduledDate AS [Scheduled Date], j.Status, j.Priority,
                                   CASE WHEN j.AssignmentType = 'Staff' THEN st.Name
                                        WHEN j.AssignmentType = 'Team' THEN tm.TeamName
                                        ELSE '-' END AS [Assigned To]
                            FROM CmsJob j
                            JOIN CmsSection sec ON sec.SectionID = j.SectionID
                            LEFT JOIN CmsTeam tm ON tm.TeamID = j.TeamID
                            LEFT JOIN CmsStaff st ON st.StaffID = j.StaffID
                            WHERE j.ScheduledDate BETWEEN @From AND @To
                              AND (@SectionID = 0 OR j.SectionID = @SectionID)
                            ORDER BY j.ScheduledDate";
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@From", GetFromDate(reportKey));
                    cmd.Parameters.AddWithValue("@To", GetToDate());
                    cmd.Parameters.AddWithValue("@SectionID", sectionId);
                    break;

                case "MonthlyCleaning":
                    title = "Monthly Cleaning Report";
                    sql = @"SELECT sec.SectionName AS Section, j.CleaningType AS [Cleaning Type],
                                   j.ScheduledDate AS [Scheduled Date], j.Status, j.Priority,
                                   CASE WHEN j.AssignmentType = 'Staff' THEN st.Name
                                        WHEN j.AssignmentType = 'Team' THEN tm.TeamName
                                        ELSE '-' END AS [Assigned To]
                            FROM CmsJob j
                            JOIN CmsSection sec ON sec.SectionID = j.SectionID
                            LEFT JOIN CmsTeam tm ON tm.TeamID = j.TeamID
                            LEFT JOIN CmsStaff st ON st.StaffID = j.StaffID
                            WHERE j.ScheduledDate BETWEEN @From AND @To
                              AND (@SectionID = 0 OR j.SectionID = @SectionID)
                            ORDER BY j.ScheduledDate";
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@From", GetFromDate(reportKey));
                    cmd.Parameters.AddWithValue("@To", GetToDate());
                    cmd.Parameters.AddWithValue("@SectionID", sectionId);
                    break;

                case "AdditionalWorkRequest":
                    title = "Additional Work Request Report";
                    sql = @"SELECT sec.SectionName AS Section, r.Description, r.RequestType AS [Request Type],
                                   r.Priority, r.Status, r.RequestDate AS [Request Date],
                                   r.CompletedDate AS [Completed Date], r.Cost,
                                   req.Name AS [Requested By], asg.Name AS [Assigned To], tm.TeamName AS Team
                            FROM CmsAdditionalWorkRequest r
                            JOIN CmsSection sec ON sec.SectionID = r.SectionID
                            LEFT JOIN CmsStaff req ON req.StaffID = r.RequestedByID
                            LEFT JOIN CmsStaff asg ON asg.StaffID = r.AssignedToID
                            LEFT JOIN CmsTeam tm ON tm.TeamID = r.TeamID
                            WHERE r.RequestDate BETWEEN @From AND @To
                              AND (@SectionID = 0 OR r.SectionID = @SectionID)
                            ORDER BY r.RequestDate DESC";
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@From", GetFromDate(reportKey));
                    cmd.Parameters.AddWithValue("@To", GetToDate());
                    cmd.Parameters.AddWithValue("@SectionID", sectionId);
                    break;

                case "MachineMaintenanceHistory":
                    title = "Machine Maintenance History";
                    sql = @"SELECT m.MachineName AS Machine, sec.SectionName AS Section,
                                   mm.MaintenanceType AS [Maintenance Type], mm.Status,
                                   mm.ScheduledDate AS [Scheduled Date], mm.CompletedDate AS [Completed Date],
                                   mm.IssueDescription AS Issue, mm.Cost, mm.Remarks
                            FROM CmsMachineMaintenance mm
                            JOIN CmsMachine m ON m.MachineID = mm.MachineID
                            JOIN CmsSection sec ON sec.SectionID = m.SectionID
                            WHERE (@MachineID = 0 OR mm.MachineID = @MachineID)
                              AND (mm.ScheduledDate BETWEEN @From AND @To OR mm.ScheduledDate IS NULL)
                            ORDER BY mm.ScheduledDate DESC";
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@From", GetFromDate(reportKey));
                    cmd.Parameters.AddWithValue("@To", GetToDate());
                    cmd.Parameters.AddWithValue("@MachineID", machineId);
                    break;

                case "BreakdownHistory":
                    title = "Breakdown History";
                    sql = @"SELECT m.MachineName AS Machine, sec.SectionName AS Section,
                                   b.BreakdownDate AS [Breakdown Date], st.Name AS [Responsible Staff]
                            FROM CmsBreakdown b
                            JOIN CmsMachine m ON m.MachineID = b.MachineID
                            JOIN CmsSection sec ON sec.SectionID = m.SectionID
                            LEFT JOIN CmsStaff st ON st.StaffID = b.ResponsibleStaffID
                            WHERE (@MachineID = 0 OR b.MachineID = @MachineID)
                              AND b.BreakdownDate BETWEEN @From AND @To
                            ORDER BY b.BreakdownDate DESC";
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@From", GetFromDate(reportKey));
                    cmd.Parameters.AddWithValue("@To", GetToDate());
                    cmd.Parameters.AddWithValue("@MachineID", machineId);
                    break;

                case "RepairDetails":
                    title = "Repair Details";
                    sql = @"SELECT m.MachineName AS Machine, sec.SectionName AS Section,
                                   b.BreakdownDate AS [Breakdown Date], st.Name AS [Responsible Staff],
                                   b.RepairDetails AS [Repair Details]
                            FROM CmsBreakdown b
                            JOIN CmsMachine m ON m.MachineID = b.MachineID
                            JOIN CmsSection sec ON sec.SectionID = m.SectionID
                            LEFT JOIN CmsStaff st ON st.StaffID = b.ResponsibleStaffID
                            WHERE (@MachineID = 0 OR b.MachineID = @MachineID)
                              AND b.BreakdownDate BETWEEN @From AND @To
                              AND b.RepairDetails IS NOT NULL
                            ORDER BY b.BreakdownDate DESC";
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@From", GetFromDate(reportKey));
                    cmd.Parameters.AddWithValue("@To", GetToDate());
                    cmd.Parameters.AddWithValue("@MachineID", machineId);
                    break;

                default:
                    title = "Report";
                    return dt;
            }

            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }
        }

        return dt;
    }

    #endregion

    #region Export Helpers

    private void SendPdf(DataTable dt, string title, string fileName)
    {
        byte[] pdfBytes = GeneratePdf(dt, title);
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
        Response.BinaryWrite(pdfBytes);
        Response.End();
    }

    private void SendExcel(DataTable dt, string title, string fileName)
    {
        byte[] xlsBytes = GenerateExcel(dt, title);
        Response.Clear();
        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
        Response.BinaryWrite(xlsBytes);
        Response.End();
    }

    private byte[] GeneratePdf(DataTable dt, string title)
    {
        using (var ms = new MemoryStream())
        {
            Document doc = new Document(PageSize.A4.Rotate(), 24, 24, 40, 30);
            PdfWriter.GetInstance(doc, ms);
            doc.Open();

            doc.Add(new Paragraph(title, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16)));
            doc.Add(new Paragraph("Generated on " + DateTime.Now.ToString("dd MMM yyyy HH:mm"),
                FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.GRAY)));
            doc.Add(Chunk.NEWLINE);

            PdfPTable table = new PdfPTable(dt.Columns.Count) { WidthPercentage = 100 };

            foreach (DataColumn col in dt.Columns)
            {
                PdfPCell cell = new PdfPCell(new Phrase(col.ColumnName, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.WHITE)))
                {
                    BackgroundColor = new BaseColor(11, 13, 18),
                    Padding = 6
                };
                table.AddCell(cell);
            }

            foreach (DataRow row in dt.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(item == null || item == DBNull.Value ? "-" : item.ToString(),
                        FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                    {
                        Padding = 5
                    };
                    table.AddCell(cell);
                }
            }

            doc.Add(table);
            doc.Close();
            return ms.ToArray();
        }
    }

    private byte[] GenerateExcel(DataTable dt, string title)
    {
        string sheetName = title.Length > 31
            ? title.Substring(0, 31)
            : title;

        using (var wb = new XLWorkbook())
        {
            var ws = wb.Worksheets.Add(sheetName);

            ws.Cell(1, 1).InsertTable(dt);

            ws.Row(1).Style.Font.Bold = true;

            ws.Columns().AdjustToContents();

            using (var ms = new MemoryStream())
            {
                wb.SaveAs(ms);
                return ms.ToArray();
            }
        }
    }

    #endregion
}