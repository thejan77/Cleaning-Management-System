<%@ Page Title="Teams" Language="C#" MasterPageFile="~/CmsMaster.master"
    AutoEventWireup="true" CodeFile="Team.aspx.cs" Inherits="CleaningManagement_Masters_Team" %>

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

        #pnlTeamFormWrapper,
        #pnlMemberFormWrapper {
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

        .form-row {
            margin-bottom: 0;
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
        }

        .status-active   { background-color: #2E7D32; }
        .status-inactive { background-color: #94A3B8; }

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

        .members-link {
            color: var(--black);
            font-weight: 600;
            font-size: 13px;
            text-decoration: underline;
        }

        .members-link:hover { color: var(--orange); }

        #pnlMembersSection {
            display: none;
            margin-top: 10px;
        }

        .members-section-title {
            font-size: 19px;
            font-weight: 700;
            color: var(--black);
            margin-bottom: 4px;
        }

        .members-section-sub {
            font-size: 13px;
            color: var(--gray-text);
            margin-bottom: 18px;
        }

        .assign-row {
            display: flex;
            gap: 10px;
            align-items: flex-end;
            margin-bottom: 10px;
        }

        .assign-row .form-row {
            flex: 1;
        }

        .no-team-msg {
            border: 1px solid var(--border-color);
            border-radius: 10px;
            padding: 20px;
            background-color: var(--bg-light);
            color: var(--gray-text);
            font-size: 13.5px;
        }
    </style>

    <div class="cms-page-title">
        <asp:Label ID="lblPageTitle" runat="server" Text="Teams" ClientIDMode="Static" />
    </div>

    <asp:HiddenField ID="hdnTeamID" runat="server" Value="0" ClientIDMode="Static" />
    <asp:HiddenField ID="hdnSelectedTeamID" runat="server" Value="0" ClientIDMode="Static" />
    <asp:HiddenField ID="hdnStaffID" runat="server" Value="0" ClientIDMode="Static" />

    <%-- TEAM MASTER SECTION (Admin only)  --%>

    <asp:Panel ID="pnlTeamAdminSection" runat="server">

        <div id="addTeamBox" class="add-job-box" onclick="openNewTeam();">
            <span class="plus-icon">+</span>
            <span>Add New Team</span>
        </div>

        <div id="pnlTeamFormWrapper">

            <div class="form-heading-row">
                <asp:Label ID="lblFormHeading" runat="server"
                    Text="Add New Team"
                    Font-Bold="true" Font-Size="16px"
                    ForeColor="#0B0D12" ClientIDMode="Static" />
                <asp:Label ID="lblMessage" runat="server" ClientIDMode="Static" />
            </div>

            <div class="form-grid">

                <div class="form-row">
                    <label for="txtTeamName">Team Name *</label>
                    <asp:TextBox ID="txtTeamName" runat="server"
                        CssClass="form-control"
                        placeholder="e.g. Morning Shift Team A, Washroom Crew"
                        ClientIDMode="Static" />
                </div>

                <div class="form-row">
                    <label for="ddlActive">Status</label>
                    <asp:DropDownList ID="ddlActive" runat="server"
                        CssClass="form-control" ClientIDMode="Static">
                        <asp:ListItem Value="1">Active</asp:ListItem>
                        <asp:ListItem Value="0">Inactive</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="form-row">
                    <label for="ddlSupervisor">Supervisor</label>
                    <asp:DropDownList ID="ddlSupervisor" runat="server"
                        CssClass="form-control" ClientIDMode="Static" />
                </div>
               

                <div class="form-row full-width">
                    <label for="txtDescription">Description</label>
                    <asp:TextBox ID="txtDescription" runat="server"
                        CssClass="form-control"
                        TextMode="MultiLine" Rows="3"
                        placeholder="Optional — brief description of this team"
                        ClientIDMode="Static" />
                </div>

                <div class="form-actions">
                    <asp:Button ID="btnSaveTeam" runat="server"
                        Text="Save Team"
                        CssClass="btn-save"
                        ClientIDMode="Static"
                        OnClick="btnSaveTeam_Click" />
                    <asp:Button ID="btnCancel" runat="server"
                        Text="Cancel"
                        CssClass="btn-cancel"
                        CausesValidation="false"
                        OnClick="btnCancel_Click" />
                </div>

            </div>
        </div>

        <div class="job-history-title">Registered Teams</div>

        <div class="job-grid-wrapper">
            <asp:GridView ID="gvTeams" runat="server"
                AutoGenerateColumns="false"
                CssClass="job-grid"
                GridLines="None"
                DataKeyNames="TeamID"
                OnRowCommand="gvTeams_RowCommand"
                EmptyDataText="No teams have been added yet.">
                <Columns>
                    <asp:BoundField DataField="TeamID"   HeaderText="ID" />
                    <asp:BoundField DataField="TeamName" HeaderText="Team Name" />
                    <asp:BoundField DataField="Description" HeaderText="Description" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class='status-pill status-<%# Convert.ToInt16(Eval("Active")) == 1 ? "active" : "inactive" %>'>
                                <%# Convert.ToInt16(Eval("Active")) == 1 ? "Active" : "Inactive" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                     <asp:BoundField DataField="SupervisorName" HeaderText="Supervisor" />
                    <asp:BoundField DataField="CreatedDate" HeaderText="Created"
                        DataFormatString="{0:yyyy-MM-dd}" HtmlEncode="false" />
                  <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkEdit" runat="server"
                                CssClass="edit-link"
                                CommandName="EditTeam"
                                CommandArgument='<%# Eval("TeamID") %>'>Edit</asp:LinkButton>
                            <asp:LinkButton ID="lnkMembers" runat="server"
                                CssClass="members-link"
                                CommandName="ManageMembers"
                                CommandArgument='<%# Eval("TeamID") + "|" + Eval("TeamName") %>'>Manage Members</asp:LinkButton>
                            <asp:LinkButton ID="lnkRemove" runat="server"
                                CssClass="remove-link"
                                CommandName="RemoveTeam"
                                CommandArgument='<%# Eval("TeamID") %>'
                                OnClientClick='<%# "return confirm(\"Remove team " + Eval("TeamName") + "? This will unlink all members.\");" %>'>Remove</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>

    </asp:Panel>

    <%-- TEAM MEMBERS AND AREAS SECTION  --%>

    <div id="pnlMembersSection">

        <div class="members-section-title">
            Team Members — <asp:Label ID="lblSelectedTeamName" runat="server" ClientIDMode="Static" />
        </div>
        <div class="members-section-sub">
            Add, edit, or remove members and supervisors assigned to this team.
        </div>

        <asp:Panel ID="pnlNoTeamAssigned" runat="server" CssClass="no-team-msg" Visible="false">
            Your account isn't linked to a team yet. Contact an administrator.
        </asp:Panel>

        <asp:Panel ID="pnlTeamWorkspace" runat="server">

            <div id="addMemberBox" class="add-job-box" onclick="openNewMember();">
                <span class="plus-icon">+</span>
                <span>Add Member / Supervisor</span>
            </div>

            <div id="pnlMemberFormWrapper">

                <div class="form-heading-row">
                    <asp:Label ID="lblMemberFormHeading" runat="server"
                        Text="Add Team Member"
                        Font-Bold="true" Font-Size="16px"
                        ForeColor="#0B0D12" ClientIDMode="Static" />
                    <asp:Label ID="lblMemberMessage" runat="server" ClientIDMode="Static" />
                </div>

                <div class="form-grid">

                    <div class="form-row">
                        <label for="txtStaffName">Name *</label>
                        <asp:TextBox ID="txtStaffName" runat="server"
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
                        <label for="ddlMemberRole">System Role</label>
                        <asp:DropDownList ID="ddlMemberRole" runat="server"
                            CssClass="form-control" ClientIDMode="Static" />
                    </div>

                    <div class="form-row">
                        <label for="ddlMemberContractor">Supplied By (Contractor)</label>
                        <asp:DropDownList ID="ddlMemberContractor" runat="server"
                            CssClass="form-control" ClientIDMode="Static" />
                    </div>

                    <div class="form-row full-width">
                        <label for="txtContactNumber">Contact Number</label>
                        <asp:TextBox ID="txtContactNumber" runat="server"
                            CssClass="form-control"
                            placeholder="e.g. 077XXXXXXX"
                            ClientIDMode="Static" />
                    </div>

                    <div class="form-actions">
                        <asp:Button ID="btnSaveMember" runat="server"
                            Text="Save Member"
                            CssClass="btn-save"
                            ClientIDMode="Static"
                            OnClick="btnSaveMember_Click" />
                        <asp:Button ID="btnCancelMember" runat="server"
                            Text="Cancel"
                            CssClass="btn-cancel"
                            CausesValidation="false"
                            OnClick="btnCancelMember_Click" />
                    </div>

                </div>
            </div>

            <div class="job-grid-wrapper">
                <asp:GridView ID="gvStaff" runat="server"
                    AutoGenerateColumns="false"
                    CssClass="job-grid"
                    GridLines="None"
                    DataKeyNames="StaffID"
                    OnRowCommand="gvStaff_RowCommand"
                    EmptyDataText="No members have been added to this team yet.">
                    <Columns>
                        <asp:BoundField DataField="StaffID" HeaderText="ID" />
                        <asp:BoundField DataField="Name" HeaderText="Name" />
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
                                <asp:LinkButton ID="lnkEditMember" runat="server"
                                    CssClass="edit-link"
                                    CommandName="EditMember"
                                    CommandArgument='<%# Eval("StaffID") %>'>Edit</asp:LinkButton>
                                <asp:LinkButton ID="lnkRemoveMember" runat="server"
                                    CssClass="remove-link"
                                    CommandName="RemoveMember"
                                    CommandArgument='<%# Eval("StaffID") %>'
                                    OnClientClick='<%# "return confirm(\"Remove " + Eval("Name") + " from this team?\");" %>'>Remove</asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

            <div class="job-history-title">Assigned Cleaning Areas</div>

            <div class="assign-row">
                <div class="form-row">
                    <label for="ddlAssignSection">Section</label>
                    <asp:DropDownList ID="ddlAssignSection" runat="server"
                        CssClass="form-control" ClientIDMode="Static" />
                </div>
                <asp:Button ID="btnAssignSection" runat="server"
                    Text="Assign Area"
                    CssClass="btn-save"
                    ClientIDMode="Static"
                    OnClick="btnAssignSection_Click" />
            </div>
            <asp:Label ID="lblAreaMessage" runat="server" ClientIDMode="Static" Font-Size="12.5px" /><br /><br />

            <div class="job-grid-wrapper">
                <asp:GridView ID="gvAssignedSections" runat="server"
                    AutoGenerateColumns="false"
                    CssClass="job-grid"
                    GridLines="None"
                    DataKeyNames="AssignmentID"
                    OnRowCommand="gvAssignedSections_RowCommand"
                    EmptyDataText="No cleaning areas assigned to this team yet.">
                    <Columns>
                        <asp:BoundField DataField="LocationName" HeaderText="Location" />
                        <asp:BoundField DataField="SectionName" HeaderText="Section" />
                        <asp:BoundField DataField="SectionType" HeaderText="Type" />
                        <asp:BoundField DataField="AssignedDate" HeaderText="Assigned On"
                            DataFormatString="{0:yyyy-MM-dd}" HtmlEncode="false" />
                        <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkUnassign" runat="server"
                                    CssClass="remove-link"
                                    CommandName="UnassignSection"
                                    CommandArgument='<%# Eval("AssignmentID") %>'
                                    OnClientClick='<%# "return confirm(\"Remove " + Eval("SectionName") + " from this team\x27s areas?\");" %>'>Unassign</asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

        </asp:Panel>

    </div>

    <script type="text/javascript">
        function toggleTeamForm(show) {
            var panel = document.getElementById('pnlTeamFormWrapper');
            if (panel) panel.style.display = show ? 'block' : 'none';
        }

        function openNewTeam() {
            var teamID = document.getElementById('hdnTeamID').value;
            if (teamID != "0") {
                alert("Please cancel the current edit before adding a new team.");
                return;
            }
            toggleTeamForm(true);
        }

        function toggleMembersSection(show) {
            var panel = document.getElementById('pnlMembersSection');
            if (panel) panel.style.display = show ? 'block' : 'none';
        }

        function toggleMemberForm(show) {
            var panel = document.getElementById('pnlMemberFormWrapper');
            if (panel) panel.style.display = show ? 'block' : 'none';
        }

        function openNewMember() {
            var staffID = document.getElementById('hdnStaffID').value;
            if (staffID != "0") {
                alert("Please cancel the current edit before adding a new member.");
                return;
            }
            toggleMemberForm(true);
        }
    </script>

</asp:Content>