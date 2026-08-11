<%@ Page Title="Machines" Language="C#" MasterPageFile="~/CmsMaster.master"
    AutoEventWireup="true" CodeFile="Machine.aspx.cs" Inherits="CleaningManagement_Masters_Machine" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        :root {
            --black: #0B0D12;
            --orange: #E8622D;
            --orange-dark: #CC5222;
            --bg-light: #F4F4F5;
            --gray-text: #64748B;
            --border-color: #E2E8F0;
        }

        .cms-page-title {
            font-size: 22px;
            font-weight: 700;
            margin-bottom: 18px;
            color: var(--black);
        }

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
            transition: all 0.18s ease-in-out;
            margin-bottom: 20px;
        }

        .add-job-box:hover {
            background-color: var(--orange);
            color: #ffffff;
        }

        .add-job-box .plus-icon {
            font-size: 15px;
            font-weight: bold;
        }

        #pnlMachineFormWrapper {
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

        .btn-save:hover { background-color: var(--orange-dark); }

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

        .btn-update:hover { background-color: #B02A37; }

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

        .job-grid tr:last-child td { border-bottom: none; }
        .job-grid tr:hover td { background-color: var(--bg-light); }

        .status-pill {
            padding: 3px 11px;
            border-radius: 12px;
            font-size: 11.5px;
            font-weight: 600;
            color: #fff;
            white-space: nowrap;
        }

        .status-active            { background-color: #2E7D32; }
        .status-undermaintenance  { background-color: var(--orange); }
        .status-broken            { background-color: #DC3545; }

        .edit-link {
            color: var(--orange);
            font-weight: 600;
            font-size: 13px;
            text-decoration: none;
            margin-right: 12px;
        }

        .edit-link:hover {
            color: var(--orange-dark);
            text-decoration: underline;
        }

        .remove-link {
            color: #DC3545;
            font-weight: 600;
            font-size: 13px;
            text-decoration: none;
        }

        .remove-link:hover {
            color: #B02A37;
            text-decoration: underline;
        }
    </style>

    <div class="cms-page-title">Machines</div>

    <asp:HiddenField ID="hdnMachineID" runat="server" Value="0" ClientIDMode="Static" />

    <div id="addMachineBox" class="add-job-box" onclick="openNewMachine();">
        <span class="plus-icon">+</span>
        <span>Add New Machine</span>
    </div>

    <div id="pnlMachineFormWrapper">

        <div class="form-heading-row">
            <asp:Label ID="lblFormHeading" runat="server"
                Text="Add New Machine"
                Font-Bold="true" Font-Size="16px"
                ForeColor="#0B0D12" ClientIDMode="Static" />
            <asp:Label ID="lblMessage" runat="server" ClientIDMode="Static" />
        </div>

        <div class="form-grid">

            <div class="form-row">
                <label for="txtMachineName">Machine Name *</label>
                <asp:TextBox ID="txtMachineName" runat="server"
                    CssClass="form-control"
                    placeholder="e.g. Floor Scrubber #3"
                    ClientIDMode="Static" />
            </div>

            <div class="form-row">
                <label for="ddlSection">Section *</label>
                <asp:DropDownList ID="ddlSection" runat="server"
                    CssClass="form-control" ClientIDMode="Static" />
            </div>

            <div class="form-row">
                <label for="txtMachineType">Machine Type</label>
                <asp:TextBox ID="txtMachineType" runat="server"
                    CssClass="form-control"
                    placeholder="e.g. Vacuum Cleaner, Pressure Washer"
                    ClientIDMode="Static" />
            </div>

            <div class="form-row">
                <label for="txtSerialNumber">Serial Number</label>
                <asp:TextBox ID="txtSerialNumber" runat="server"
                    CssClass="form-control" ClientIDMode="Static" />
            </div>

            <div class="form-row">
                <label for="ddlContractor">Maintained By (Contractor)</label>
                <asp:DropDownList ID="ddlContractor" runat="server"
                    CssClass="form-control" ClientIDMode="Static" />
            </div>

            <div class="form-row">
                <label for="ddlMachineStatus">Status *</label>
                <asp:DropDownList ID="ddlMachineStatus" runat="server"
                    CssClass="form-control" ClientIDMode="Static">
                    <asp:ListItem Value="Active">Active</asp:ListItem>
                    <asp:ListItem Value="Under Maintenance">Under Maintenance</asp:ListItem>
                    <asp:ListItem Value="Broken">Broken</asp:ListItem>
                </asp:DropDownList>
            </div>

            <div class="form-row">
                <label for="txtPurchaseDate">Purchase Date</label>
                <asp:TextBox ID="txtPurchaseDate" runat="server"
                    CssClass="form-control" TextMode="Date" ClientIDMode="Static" />
            </div>

            <div class="form-row full-width">
                <label for="txtDescription">Description</label>
                <asp:TextBox ID="txtDescription" runat="server"
                    CssClass="form-control"
                    TextMode="MultiLine" Rows="3"
                    placeholder="Optional — brief description or notes"
                    ClientIDMode="Static" />
            </div>

            <div class="form-actions">
                <asp:Button ID="btnSaveMachine" runat="server"
                    Text="Save Machine"
                    CssClass="btn-save"
                    ClientIDMode="Static"
                    OnClick="btnSaveMachine_Click" />
                <asp:Button ID="btnCancel" runat="server"
                    Text="Cancel"
                    CssClass="btn-cancel"
                    CausesValidation="false"
                    OnClick="btnCancel_Click" />
            </div>

        </div>
    </div>

    <div class="job-history-title">Registered Machines</div>

    <div class="job-grid-wrapper">
        <asp:GridView ID="gvMachines" runat="server"
            AutoGenerateColumns="false"
            CssClass="job-grid"
            GridLines="None"
            DataKeyNames="MachineID"
            OnRowCommand="gvMachines_RowCommand"
            EmptyDataText="No machines have been registered yet.">
            <Columns>
                <asp:BoundField DataField="MachineID" HeaderText="ID" />
                <asp:BoundField DataField="MachineName" HeaderText="Machine" />
                <asp:BoundField DataField="MachineType" HeaderText="Type" />
                <asp:BoundField DataField="LocationName" HeaderText="Location" />
                <asp:BoundField DataField="SectionName" HeaderText="Section" />
                <asp:BoundField DataField="SerialNumber" HeaderText="Serial No." />
                <asp:BoundField DataField="ContractorName" HeaderText="Contractor" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='status-pill status-<%# Eval("MachineStatus").ToString().ToLower().Replace(" ", "") %>'>
                            <%# Eval("MachineStatus") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="PurchaseDate" HeaderText="Purchased"
                    DataFormatString="{0:yyyy-MM-dd}" HtmlEncode="false" />
                <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEdit" runat="server"
                            CssClass="edit-link"
                            CommandName="EditMachine"
                            CommandArgument='<%# Eval("MachineID") %>'>Edit</asp:LinkButton>
                        <asp:LinkButton ID="lnkRemove" runat="server"
                            CssClass="remove-link"
                            CommandName="RemoveMachine"
                            CommandArgument='<%# Eval("MachineID") %>'
                            OnClientClick='<%# "return confirm(\"Remove " + Eval("MachineName") + "?\");" %>'>Remove</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <script type="text/javascript">
        function toggleMachineForm(show) {
            var panel = document.getElementById('pnlMachineFormWrapper');
            if (panel) panel.style.display = show ? 'block' : 'none';
        }

        function openNewMachine() {
            var machineID = document.getElementById('hdnMachineID').value;
            if (machineID != "0") {
                alert("Please cancel the current edit before adding a new machine.");
                return;
            }
            toggleMachineForm(true);
        }
    </script>

</asp:Content>