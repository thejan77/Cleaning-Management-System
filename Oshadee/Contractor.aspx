<%@ Page Title="Contractors" Language="C#" MasterPageFile="~/CmsMaster.master"
    AutoEventWireup="true" CodeFile="Contractor.aspx.cs" Inherits="CleaningManagement_Masters_Contractor" %>



<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<asp:ScriptManager ID="MasterScriptManager" runat="server" />
    <style>
        :root {
            --black: #0B0D12;
            --black-dark: #000000;
            --orange: #E8622D;
            --orange-dark: #CC5222;
            --bg-light: #F4F4F5;
            --gray-text: #64748B;
            --border-color: #E2E8F0;
            --sidebar-width: 250px;
        }

        /* ---------- Page heading ---------- */
        .cms-page-title {
            font-size: 22px;
            font-weight: 700;
            margin-bottom: 18px;
            color: var(--black);
        }

        /* ---------- Small orange trigger box ---------- */
        .add-job-box {
            border: 1.5px solid var(--orange);
            border-radius: 8px;
            background-color: #ffffff;
            color: var(--orange);
            padding: 10px 16px;
            font-size: 13.5px;
            font-weight: 600;
            cursor: pointer;
            display: inline-flex;
            align-items: center;
            gap: 8px;
            width: auto;
            transition: all 0.18s ease-in-out;
            margin-bottom: 20px;
        }

        .add-job-box:hover {
            background-color: var(--orange);
            color: #ffffff;
            border-color: var(--orange);
        }

        .add-job-box .plus-icon {
            font-size: 15px;
            font-weight: bold;
            line-height: 1;
        }

        /* ---------- Form panel (hidden until the orange box is clicked) ---------- */
        #pnlContractorFormWrapper {
            display: none;
            border: 1px solid var(--border-color);
            border-radius: 10px;
            padding: 24px 28px;
            margin-bottom: 26px;
            background-color: #ffffff;
            box-shadow: 0 2px 8px rgba(11,13,18,0.06);
        }

        .form-heading-row {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 18px;
            padding-bottom: 14px;
            border-bottom: 1px solid var(--border-color);
        }

        /* Two-column, justified form grid */
        .form-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            column-gap: 32px;
            row-gap: 16px;
        }

        .form-grid .form-row.full-width {
            grid-column: 1 / -1;
        }

        .form-row label {
            display: block;
            font-weight: 600;
            font-size: 13px;
            margin-bottom: 6px;
            color: var(--black);
        }

        .form-row .form-control {
            width: 100%;
            box-sizing: border-box;
            padding: 9px 12px;
            border: 1px solid var(--border-color);
            border-radius: 6px;
            font-size: 13.5px;
            color: var(--black);
            background-color: #fff;
        }

        .form-row .form-control:focus {
            outline: none;
            border-color: var(--orange);
        }

        .form-actions {
            margin-top: 22px;
            grid-column: 1 / -1;
        }

        .btn-save {
            background-color: var(--orange);
            border: none;
            color: #fff;
            padding: 10px 24px;
            border-radius: 6px;
            font-weight: 600;
            font-size: 13.5px;
            cursor: pointer;
        }

        .btn-save:hover {
            background-color: var(--orange-dark);
        }

        .btn-cancel {
            background-color: var(--bg-light);
            border: 1px solid var(--border-color);
            color: var(--black);
            padding: 10px 24px;
            border-radius: 6px;
            font-weight: 600;
            font-size: 13.5px;
            cursor: pointer;
            margin-left: 10px;
        }

        /* ---------- Registered contractors table ---------- */
        .job-history-title {
            font-size: 17px;
            font-weight: 700;
            margin: 26px 0 12px 0;
            color: var(--black);
        }

        .job-grid-wrapper {
            border: 1px solid var(--border-color);
            border-radius: 10px;
            overflow: hidden;
            background-color: #fff;
        }

        .job-grid {
            width: 100%;
            border-collapse: collapse;
        }

        .job-grid th {
            background-color: var(--black);
            color: #fff;
            padding: 12px 14px;
            text-align: left;
            font-size: 12.5px;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.3px;
        }

        .job-grid td {
            padding: 11px 14px;
            border-bottom: 1px solid var(--border-color);
            font-size: 13px;
            color: var(--black);
        }

        .job-grid tr:last-child td {
            border-bottom: none;
        }

        .job-grid tr:hover td {
            background-color: var(--bg-light);
        }

        .status-pill {
            padding: 3px 11px;
            border-radius: 12px;
            font-size: 11.5px;
            font-weight: 600;
            color: #fff;
            white-space: nowrap;
        }

        .status-active { background-color: #2E7D32; }
        .status-expired { background-color: #94A3B8; }
        .status-ongoing { background-color: var(--orange); }

        .edit-link {
            color: var(--orange);
            font-weight: 600;
            font-size: 13px;
            text-decoration: none;
        }

        .edit-link:hover {
            color: var(--orange-dark);
            text-decoration: underline;
        }

        .btn-update {
    background-color: #DC3545;
    border: none;
    color: white;
    padding: 10px 24px;
    border-radius: 6px;
    font-weight: 600;
    font-size: 13.5px;
    cursor: pointer;
}

.btn-update:hover {
    background-color: #B02A37;
}
    </style>

    <div class="cms-page-title">Contractors</div>

    <%-- Hidden field tracks which ContractorID is being edited. 0 = registering a new contractor --%>
    <asp:HiddenField ID="hdnContractorID" runat="server" Value="0" ClientIDMode="Static" />

    <%-- ================= Small orange trigger box ================= --%>
   <div id="addContractorBox" class="add-job-box" onclick="openNewContractor();">
        <span class="plus-icon">+</span>
        <span>Register New Contractor</span>
    </div>

    <%-- ================= Contractor register / edit form (two-column, justified) ================= --%>
    <div id="pnlContractorFormWrapper">

        <div class="form-heading-row">
            <asp:Label ID="lblFormHeading" runat="server" Text="Register New Contractor" Font-Bold="true" Font-Size="16px" ForeColor="#0B0D12" ClientIDMode="Static" />
            <asp:Label ID="lblMessage" runat="server" ClientIDMode="Static" />
        </div>

        <div class="form-grid">

             <div class="form-row">
        <label for="txtContractorID">Contractor ID</label>
        <asp:TextBox
            ID="txtContractorID"
            runat="server"
            CssClass="form-control"
            ReadOnly="true"
            ClientIDMode="Static" />
    </div>
            <div class="form-row">
                <label for="txtContractorName">Contractor / Company Name *</label>
                <asp:TextBox ID="txtContractorName" runat="server" CssClass="form-control" ClientIDMode="Static" />
            </div>

            <div class="form-row">
                <label for="txtContractCategory">Contract Category</label>
                <asp:TextBox ID="txtContractCategory" runat="server" CssClass="form-control"
                    placeholder="e.g. Pest Control, Lift Maintenance, Security" ClientIDMode="Static" />
            </div>

            <div class="form-row full-width">
                <label for="txtContactPerson">Contact Person</label>
                <asp:TextBox ID="txtContactPerson" runat="server" CssClass="form-control" ClientIDMode="Static" />
            </div>

            <div class="form-row">
                <label for="txtContractStartDate">Contract Start Date</label>
                <asp:TextBox ID="txtContractStartDate" runat="server" CssClass="form-control" TextMode="Date" ClientIDMode="Static" />
            </div>

            <div class="form-row">
                <label for="txtContractEndDate">Contract End Date</label>
                <asp:TextBox ID="txtContractEndDate" runat="server" CssClass="form-control" TextMode="Date" ClientIDMode="Static" />
            </div>

            <div class="form-actions">
                <asp:Button ID="btnSaveContractor" runat="server" 
    Text="Save Contractor" 
    CssClass="btn-save" 
    ClientIDMode="Static"
    OnClick="btnSaveContractor_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn-cancel" CausesValidation="false" OnClick="btnCancel_Click" />
            </div>

        </div>

    </div>

    <%-- ================= Registered contractors table ================= --%>
    <div class="job-history-title">Registered Contractors</div>

    <div class="job-grid-wrapper">
        <asp:GridView ID="gvContractors" runat="server" AutoGenerateColumns="false" CssClass="job-grid"
            GridLines="None" DataKeyNames="ContractorID" OnRowCommand="gvContractors_RowCommand"
            EmptyDataText="No contractors have been registered yet.">
            <Columns>
                
                <asp:TemplateField HeaderText="ID">
             <ItemTemplate>
               <%# "CON-" + Convert.ToInt32(Eval("ContractorID")).ToString("000") %>
             </ItemTemplate>
             </asp:TemplateField>
                <asp:BoundField DataField="ContractorName" HeaderText="Contractor / Company" />
                <asp:BoundField DataField="ContractCategory" HeaderText="Category" />
                <asp:BoundField DataField="ContactPerson" HeaderText="Contact Person" />
                <asp:BoundField DataField="ContractStartDate" HeaderText="Start Date" DataFormatString="{0:yyyy-MM-dd}" HtmlEncode="false" />
                <asp:BoundField DataField="ContractEndDate" HeaderText="End Date" DataFormatString="{0:yyyy-MM-dd}" HtmlEncode="false" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='status-pill status-<%# Eval("ContractStatus").ToString().ToLower() %>'>
                            <%# Eval("ContractStatus") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEdit" runat="server" CssClass="edit-link"
                            CommandName="EditContractor" CommandArgument='<%# Eval("ContractorID") %>'>Edit</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

<script type="text/javascript">

    function toggleContractorForm(show) {
        var panel = document.getElementById('pnlContractorFormWrapper');
        if (panel) {
            panel.style.display = show ? 'block' : 'none';
        }
    }


    function openNewContractor() {

        var contractorID = document.getElementById('hdnContractorID').value;

        // If editing an existing contractor
        if (contractorID != "0") {
            alert("Please cancel the current edit before registering a new contractor.");
            return;
        }

        toggleContractorForm(true);
    }

    </script>


</asp:Content>
