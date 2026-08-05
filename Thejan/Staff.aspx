<%@ Page Title="Staff" Language="C#" MasterPageFile="~/CmsMaster.master"
    AutoEventWireup="true" CodeFile="Staff.aspx.cs" Inherits="CleaningManagement_Masters_Staff" %>

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

        #pnlStaffFormWrapper {
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

        .filter-bar {
            display: flex;
            gap: 16px;
            align-items: flex-end;
            flex-wrap: wrap;
            border: 1px solid var(--border-color);
            border-radius: 10px;
            padding: 18px 20px;
            margin-bottom: 20px;
            background-color: #ffffff;
        }

        .filter-item {
            display: flex;
            flex-direction: column;
            min-width: 180px;
        }

        .filter-item label {
            font-size: 12px;
            font-weight: 600;
            color: var(--gray-text);
            margin-bottom: 6px;
            text-transform: uppercase;
            letter-spacing: 0.3px;
        }

        .filter-item .form-control {
            padding: 8px 10px;
            border: 1px solid var(--border-color);
            border-radius: 6px;
            font-size: 13px;
        }

        .btn-search {
            background-color: var(--black);
            border: none;
            color: #fff;
            padding: 9px 20px;
            border-radius: 6px;
            font-weight: 600;
            font-size: 13px;
            cursor: pointer;
        }

        .btn-search:hover { background-color: #000; }

        .btn-clear {
            background-color: var(--bg-light);
            border: 1px solid var(--border-color);
            color: var(--black);
            padding: 9px 20px;
            border-radius: 6px;
            font-weight: 600;
            font-size: 13px;
            cursor: pointer;
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

        .role-pill {
            padding: 3px 11px;
            border-radius: 12px;
            font-size: 11.5px;
            font-weight: 600;
            color: #fff;
            white-space: nowrap;
        }

        .role-supervisor { background-color: var(--orange); }
        .role-other       { background-color: var(--gray-text); }

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

    <div class="cms-page-title">Staff</div>

    <asp:HiddenField ID="hdnStaffID" runat="server" Value="0" ClientIDMode="Static" />

    <div id="addStaffBox" class="add-job-box" onclick="openNewStaff();">
        <span class="plus-icon">+</span>
        <span>Add New Staff</span>
    </div>

    <div id="pnlStaffFormWrapper">

        <div class="form-heading-row">
            <asp:Label ID="lblFormHeading" runat="server"
                Text="Add New Staff"
                Font-Bold="true" Font-Size="16px"
                ForeColor="#0B0D12" ClientIDMode="Static" />
            <asp:Label ID="lblMessage" runat="server" ClientIDMode="Static" />
        </div>

        <div class="form-grid">

            <div class="form-row">
                <label for="txtStaffName">Name *</label>
                <asp:TextBox ID="txtStaffName" runat="server"
                    CssClass="form-control" ClientIDMode="Static" />
            </div>

            <div class="form-row">
                <label for="ddlTeam">Team</label>
                <asp:DropDownList ID="ddlTeam" runat="server"
                    CssClass="form-control" ClientIDMode="Static" />
            </div>

            <div class="form-row">
                <label for="ddlJobTitle">Role *</label>
                <asp:DropDownList ID="ddlJobTitle" runat="server"
                    CssClass="form-control" ClientIDMode="Static">
                    <asp:ListItem Value="">-- Select --</asp:ListItem>
                    <asp:ListItem Value="Supervisor">Supervisor</asp:ListItem>
                    <asp:ListItem Value="Cleaner">Cleaner</asp:ListItem>
                    <asp:ListItem Value="Team Leader">Team Leader</asp:ListItem>
                    <asp:ListItem Value="Helper">Helper</asp:ListItem>
                </asp:DropDownList>
            </div>

            <div class="form-row">
                <label for="txtContactNumber">Contact Number</label>
                <asp:TextBox ID="txtContactNumber" runat="server"
                    CssClass="form-control"
                    placeholder="e.g. 077XXXXXXX"
                    ClientIDMode="Static" />
            </div>

            <div class="form-row">
                <label for="ddlStaffRole">System Role</label>
                <asp:DropDownList ID="ddlStaffRole" runat="server"
                    CssClass="form-control" ClientIDMode="Static" />
            </div>

            <div class="form-row">
                <label for="ddlStaffContractor">Supplied By (Contractor)</label>
                <asp:DropDownList ID="ddlStaffContractor" runat="server"
                    CssClass="form-control" ClientIDMode="Static" />
            </div>

            <div class="form-actions">
                <asp:Button ID="btnSaveStaff" runat="server"
                    Text="Save Staff"
                    CssClass="btn-save"
                    ClientIDMode="Static"
                    OnClick="btnSaveStaff_Click" />
                <asp:Button ID="btnCancel" runat="server"
                    Text="Cancel"
                    CssClass="btn-cancel"
                    CausesValidation="false"
                    OnClick="btnCancel_Click" />
            </div>

        </div>
    </div>

    <div class="filter-bar">
        <div class="filter-item">
            <label for="ddlFilterTeam">Team</label>
            <asp:DropDownList ID="ddlFilterTeam" runat="server" CssClass="form-control" ClientIDMode="Static" />
        </div>
        <div class="filter-item">
            <label for="ddlFilterRole">Role</label>
            <asp:DropDownList ID="ddlFilterRole" runat="server" CssClass="form-control" ClientIDMode="Static">
                <asp:ListItem Value="">All Roles</asp:ListItem>
                <asp:ListItem Value="Supervisor">Supervisor</asp:ListItem>
                <asp:ListItem Value="Cleaner">Cleaner</asp:ListItem>
                <asp:ListItem Value="Team Leader">Team Leader</asp:ListItem>
                <asp:ListItem Value="Helper">Helper</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="filter-item">
            <label for="txtFilterName">Search Name</label>
            <asp:TextBox ID="txtFilterName" runat="server" CssClass="form-control" ClientIDMode="Static" placeholder="e.g. Perera" />
        </div>
        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn-search" OnClick="btnSearch_Click" />
        <asp:Button ID="btnClearFilter" runat="server" Text="Clear" CssClass="btn-clear" CausesValidation="false" OnClick="btnClearFilter_Click" />
    </div>

    <div class="job-grid-wrapper">
        <asp:GridView ID="gvStaff" runat="server"
            AutoGenerateColumns="false"
            CssClass="job-grid"
            GridLines="None"
            DataKeyNames="StaffID"
            OnRowCommand="gvStaff_RowCommand"
            EmptyDataText="No staff found matching these filters.">
            <Columns>
                <asp:BoundField DataField="StaffID" HeaderText="ID" />
                <asp:BoundField DataField="Name" HeaderText="Name" />
                <asp:BoundField DataField="TeamName" HeaderText="Team" />
                <asp:TemplateField HeaderText="Role">
                    <ItemTemplate>
                        <span class='role-pill <%# Eval("JobTitle").ToString() == "Supervisor" ? "role-supervisor" : "role-other" %>'>
                            <%# Eval("JobTitle") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="ContactNumber" HeaderText="Contact Number" />
                <asp:BoundField DataField="RoleName" HeaderText="System Role" />
                <asp:BoundField DataField="ContractorName" HeaderText="Contractor" />
                <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEdit" runat="server"
                            CssClass="edit-link"
                            CommandName="EditStaff"
                            CommandArgument='<%# Eval("StaffID") %>'>Edit</asp:LinkButton>
                        <asp:LinkButton ID="lnkRemove" runat="server"
                            CssClass="remove-link"
                            CommandName="RemoveStaff"
                            CommandArgument='<%# Eval("StaffID") %>'
                            OnClientClick='<%# "return confirm(\"Remove " + Eval("Name") + " entirely?\");" %>'>Remove</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <script type="text/javascript">
        function toggleStaffForm(show) {
            var panel = document.getElementById('pnlStaffFormWrapper');
            if (panel) panel.style.display = show ? 'block' : 'none';
        }

        function openNewStaff() {
            var staffID = document.getElementById('hdnStaffID').value;
            if (staffID != "0") {
                alert("Please cancel the current edit before adding a new staff member.");
                return;
            }
            toggleStaffForm(true);
        }
    </script>

</asp:Content>