<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AdditionalWorkRequest.aspx.cs" Inherits="CleaningManagement_AdditionalWorkRequest" MasterPageFile="~/CmsMaster.master" %>

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

        .awr-page-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 18px;
            flex-wrap: wrap;
            gap: 12px;
        }

        .awr-page-title {
            font-size: 22px;
            font-weight: 700;
            color: var(--black);
            margin: 0;
        }

        .awr-page-subtitle {
            font-size: 13.5px;
            color: var(--gray-text);
            margin-top: 4px;
        }

        .awr-toggle-btn {
            background: var(--orange);
            border: none;
            color: #fff;
            font-weight: 600;
            padding: 11px 18px;
            border-radius: 8px;
            cursor: pointer;
            white-space: nowrap;
        }

            .awr-toggle-btn:hover {
                background: var(--orange-dark);
            }

        .awr-filter-bar {
            background: #fff;
            border-radius: 10px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.08);
            padding: 18px 20px;
            margin-bottom: 20px;
            display: flex;
            align-items: flex-end;
            gap: 20px;
            flex-wrap: wrap;
        }

        .awr-filter-group {
            display: flex;
            flex-direction: column;
            min-width: 170px;
        }

            .awr-filter-group label {
                font-size: 11.5px;
                font-weight: 700;
                text-transform: uppercase;
                letter-spacing: 0.4px;
                color: var(--gray-text);
                margin-bottom: 6px;
            }

        .awr-btn-clear {
            background: #fff;
            border: 1px solid var(--border-color);
            color: var(--black);
            font-weight: 600;
            padding: 10px 18px;
            border-radius: 8px;
            cursor: pointer;
            height: 40px;
        }

            .awr-btn-clear:hover {
                background: var(--bg-light);
            }

        .awr-table-wrap {
            background: #fff;
            border-radius: 10px;
            overflow: hidden;
            box-shadow: 0 1px 3px rgba(0,0,0,0.08);
        }

        table.awr-grid {
            width: 100%;
            border-collapse: collapse;
            font-size: 13.5px;
        }

            table.awr-grid th {
                background: var(--black);
                color: #fff;
                text-transform: uppercase;
                font-size: 11.5px;
                letter-spacing: 0.4px;
                padding: 12px 14px;
                text-align: left;
            }

            table.awr-grid td {
                padding: 12px 14px;
                border-bottom: 1px solid var(--border-color);
                color: var(--black);
                vertical-align: middle;
            }

            table.awr-grid tr:last-child td {
                border-bottom: none;
            }

        .awr-pill {
            display: inline-block;
            padding: 4px 12px;
            border-radius: 20px;
            font-size: 11px;
            font-weight: 700;
            text-transform: uppercase;
        }

        .awr-pill-pending {
            background: #FEF3C7;
            color: #92400E;
        }

        .awr-pill-inprogress {
            background: #DBEAFE;
            color: #1E40AF;
        }

        .awr-pill-completed {
            background: #DCFCE7;
            color: #166534;
        }

        .awr-pill-rejected {
            background: #FEE2E2;
            color: #991B1B;
        }

        .awr-priority {
            font-size: 12px;
            font-weight: 600;
            padding: 3px 10px;
            border-radius: 6px;
        }

        .priority-low {
            background: #EEF2F7;
            color: #475569;
        }

        .priority-normal {
            background: #E8F1FF;
            color: #2563EB;
        }

        .priority-high {
            background: #FFF1E6;
            color: #C2410C;
        }

        .priority-urgent {
            background: #FDE2E1;
            color: #B91C1C;
        }

        .awr-action-link {
            color: var(--orange);
            font-weight: 600;
            text-decoration: none;
            margin-right: 12px;
            cursor: pointer;
        }

            .awr-action-link.delete {
                color: #DC2626;
            }

            .awr-action-link:hover {
                text-decoration: underline;
            }

        .awr-modal-overlay {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(11,13,18,0.55);
            z-index: 1000;
            align-items: flex-start;
            justify-content: center;
            overflow-y: auto;
            padding: 40px 16px;
        }

            .awr-modal-overlay.open {
                display: flex;
            }

        .awr-modal-box {
            background: #fff;
            border-radius: 12px;
            width: 100%;
            max-width: 900px;
            max-height: 90vh;
            overflow-y: auto;
        }

        .awr-modal-head {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 18px 24px;
            border-bottom: 1px solid var(--border-color);
        }

        .awr-modal-title {
            margin: 0;
            font-size: 17px;
            font-weight: 700;
        }

        .awr-modal-body {
            padding: 20px 24px;
        }

        .awr-section-label {
            text-transform: uppercase;
            font-size: 12px;
            font-weight: 700;
            color: var(--orange);
            letter-spacing: .04em;
            margin: 0 0 16px 0;
            padding-bottom: 10px;
            border-bottom: 1px solid var(--border-color);
        }

        .awr-form-row {
            display: flex;
            gap: 18px 24px;
            margin-bottom: 20px;
            padding: 0;
        }

        .awr-modal-close {
            background: none;
            border: none;
            font-size: 20px;
            cursor: pointer;
            color: var(--gray-text);
        }

        .awr-modal-title {
            font-size: 17px;
            font-weight: 700;
            color: var(--black);
            margin: 0;
        }

        .awr-modal-foot {
            padding: 18px 26px;
            border-top: 1px solid var(--border-color);
            display: flex;
            justify-content: flex-end;
            gap: 12px;
        }

        .awr-form-row {
            display: flex;
            gap: 24px;
            margin-bottom: 18px;
            flex-wrap: wrap;
            padding: 0;
        }

        .awr-form-group {
            flex: 1;
            min-width: 220px;
            display: flex;
            flex-direction: column;
        }

            .awr-form-group label {
                font-size: 13px;
                font-weight: 600;
                color: var(--black);
                margin-bottom: 6px;
            }

            .awr-form-group .req {
                color: var(--orange);
            }

        .awr-input, .awr-select, .awr-textarea {
            border: 1px solid var(--border-color);
            border-radius: 8px;
            padding: 10px 12px;
            font-size: 14px;
            color: var(--black);
            width: 100%;
            box-sizing: border-box;
        }

        .awr-textarea {
            resize: vertical;
            min-height: 70px;
        }

        .awr-code-display {
            display: block;
            padding: 10px 12px;
            border: 1px solid var(--border-color);
            border-radius: 8px;
            background: var(--bg-light);
            color: var(--black);
            font-size: 14px;
        }

        .awr-btn-save {
            background: var(--orange);
            border: 1px solid var(--orange);
            color: #fff;
            padding: 11px 22px;
            border-radius: 8px;
            font-weight: 700;
            font-size: 14px;
            cursor: pointer;
            height: 42px;
        }

        .awr-btn-cancel {
            background: #fff;
            border: 1px solid var(--border-color);
            color: var(--black);
            padding: 11px 22px;
            border-radius: 8px;
            font-weight: 700;
            font-size: 14px;
            cursor: pointer;
            height: 42px;
        }

        .awr-section-title {
            margin: 0 0 18px 0;
            padding-bottom: 12px;
            border-bottom: 1px solid #E5E7EB;
            color: #E8622D;
            font-size: 12px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: .04em;
        }

        .awr-section-label {
            text-transform: uppercase;
            font-size: 12px;
            font-weight: 700;
            color: var(--orange);
            letter-spacing: .04em;
            margin: 0 0 16px 0;
            padding-bottom: 10px;
            border-bottom: 1px solid var(--border-color);
        }

        .awr-modal-body {
            padding: 20px 28px;
        }

        .awr-btn-save:hover {
            background: var(--orange-dark);
        }

        .awr-radio-group {
            display: flex;
            gap: 22px;
            align-items: center;
            padding: 10px 0 2px 0;
        }

            .awr-radio-group label {
                font-weight: 500;
                font-size: 14px;
                display: flex;
                align-items: center;
                gap: 6px;
                cursor: pointer;
            }
    </style>

    <div class="awr-page-header">
        <div>
            <div class="awr-page-title">Additional Work Requests</div>
            <div class="awr-page-subtitle">Log, assign and track extra cleaning work requested outside the regular schedule</div>
        </div>
        <asp:Button ID="btnShowForm" runat="server" CssClass="awr-toggle-btn"
            Text="+ Register New Request" OnClick="btnShowForm_Click" CausesValidation="false" />
    </div>

    <div class="awr-filter-bar">
        <div class="awr-filter-group">
            <label>Section</label>
            <asp:DropDownList ID="ddlFilterSection" runat="server" CssClass="awr-select" AutoPostBack="true"
                OnSelectedIndexChanged="ddlFilterSection_SelectedIndexChanged">
                <asp:ListItem Text="All Sections" Value="" />
            </asp:DropDownList>
        </div>
        <div class="awr-filter-group">
            <label>Priority</label>
            <asp:DropDownList ID="ddlFilterPriority" runat="server" CssClass="awr-select" AutoPostBack="true"
                OnSelectedIndexChanged="ddlFilterPriority_SelectedIndexChanged">
                <asp:ListItem Text="All Priorities" Value="" />
                <asp:ListItem Text="Low" Value="Low" />
                <asp:ListItem Text="Normal" Value="Normal" />
                <asp:ListItem Text="High" Value="High" />
                <asp:ListItem Text="Urgent" Value="Urgent" />
            </asp:DropDownList>
        </div>
        <div class="awr-filter-group">
            <label>Status</label>
            <asp:DropDownList ID="ddlFilterStatus" runat="server" CssClass="awr-select" AutoPostBack="true"
                OnSelectedIndexChanged="ddlFilterStatus_SelectedIndexChanged">
                <asp:ListItem Text="All Status" Value="" />
                <asp:ListItem Text="Pending" Value="Pending" />
                <asp:ListItem Text="In Progress" Value="In Progress" />
                <asp:ListItem Text="Completed" Value="Completed" />
                <asp:ListItem Text="Rejected" Value="Rejected" />
            </asp:DropDownList>
        </div>
        <div class="awr-filter-group">
            <label>Assigned To</label>
            <asp:DropDownList ID="ddlFilterAssignType" runat="server" CssClass="awr-select" AutoPostBack="true"
                OnSelectedIndexChanged="ddlFilterAssignType_SelectedIndexChanged">
                <asp:ListItem Text="All" Value="" />
                <asp:ListItem Text="Individual Staff" Value="Individual" />
                <asp:ListItem Text="Team" Value="Team" />
            </asp:DropDownList>
        </div>
        <asp:Button ID="btnClearFilters" runat="server" CssClass="awr-btn-clear" Text="Clear"
            OnClick="btnClearFilters_Click" CausesValidation="false" />
    </div>

    <div class="awr-table-wrap">
        <asp:GridView ID="gvRequests" runat="server" CssClass="awr-grid" AutoGenerateColumns="false"
            GridLines="None" DataKeyNames="RequestID" OnRowCommand="gvRequests_RowCommand">
            <Columns>
                <asp:TemplateField HeaderText="Request ID">
                    <ItemTemplate>
                        <%# FormatRequestCode(Eval("RequestID")) %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="SectionName" HeaderText="Section" />
                <asp:BoundField DataField="Description" HeaderText="Description" />
                <asp:TemplateField HeaderText="Assigned To">
                    <ItemTemplate>
                        <%# Eval("AssignedToName") != null && Eval("AssignedToName").ToString() != ""
                              ? Eval("AssignedToName")
                              : (Eval("TeamName") != null && Eval("TeamName").ToString() != "" ? "Team: " + Eval("TeamName") : "-") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Priority">
                    <ItemTemplate>
                        <span class='awr-priority priority-<%# Eval("Priority").ToString().ToLower() %>'>
                            <%# Eval("Priority") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="RequestedByName" HeaderText="Requested By" />
                <asp:BoundField DataField="ApprovedByName" HeaderText="Approved By" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='awr-pill awr-pill-<%# Eval("Status").ToString().Replace(" ", "").ToLower() %>'>
                            <%# Eval("Status") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="TimeTaken" HeaderText="Time Taken (h)" />
                <asp:BoundField DataField="RequestDate" HeaderText="Request Date" DataFormatString="{0:yyyy-MM-dd}" HtmlEncode="false" />
                <asp:BoundField DataField="CompletedDate" HeaderText="Completed Date" DataFormatString="{0:yyyy-MM-dd}" HtmlEncode="false" />
                <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEdit" runat="server" CssClass="awr-action-link" CommandName="EditRequest"
                            CommandArgument='<%# Eval("RequestID") %>' Text="Edit" CausesValidation="false" />
                        <asp:LinkButton ID="lnkDelete" runat="server" CssClass="awr-action-link delete" CommandName="DeleteRequest"
                            CommandArgument='<%# Eval("RequestID") %>' Text="Delete" CausesValidation="false"
                            OnClientClick="return confirm('Are you sure you want to delete this request?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <div style="padding: 20px; color: #64748B;">No additional work requests found.</div>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>

    <div id="awrModalOverlay" class="awr-modal-overlay">
        <div class="awr-modal-box">
            <div class="awr-modal-head">
                <h2 class="awr-modal-title">
                    <asp:Literal ID="litFormTitle" runat="server" Text="Register New Work Request" />
                </h2>

                <asp:LinkButton ID="btnCloseX" runat="server" CssClass="awr-modal-close"
                    OnClick="btnCancel_Click" CausesValidation="false">
        ✕
                </asp:LinkButton>
            </div>

            <div class="awr-modal-body">

                <asp:HiddenField ID="hfRequestID" runat="server" Value="0" />
                <p class="awr-section-label">REQUEST INFO</p>
                <div class="awr-form-row">
                    <div class="awr-form-group">
                        <label>REQUEST ID</label>
                        <span class="awr-code-display"><asp:Literal ID="litRequestCode" runat="server" Text="" /></span>
                    </div>
                    <div class="awr-form-group">
                        <label>SECTION <span class="req">*</span></label>
                        <asp:DropDownList ID="ddlSection" runat="server" CssClass="awr-select" DataTextField="SectionName" DataValueField="SectionID">
                            <asp:ListItem Text="-- Select Section --" Value="" />
                        </asp:DropDownList>
                    </div>
                    <div class="awr-form-group">
                        <label>REQUEST TYPE</label>
                        <asp:TextBox ID="txtRequestType" runat="server" CssClass="awr-input" MaxLength="100" placeholder="e.g. Repair, Special Cleaning" />
                    </div>
                </div>

                <div class="awr-form-row">
                    <div class="awr-form-group" style="flex: 2 1 100%;">
                        <label>DESCRIPTION <span class="req">*</span></label>
                        <asp:TextBox ID="txtDescription" runat="server" CssClass="awr-textarea" TextMode="MultiLine" MaxLength="255" />
                    </div>
                </div>

               
                <p class="awr-section-label">ASSIGNMENT</p>

                <div class="awr-form-row" style="margin-bottom: 4px;">
                    <div class="awr-form-group">
                        <label>ASSIGN WORK TO <span class="req">*</span></label>
                        <div class="awr-radio-group">
                            <label>
                                <asp:RadioButton ID="rbAssignIndividual" runat="server" GroupName="AssignType" Checked="true" onclick="awrToggleAssignType();" />
                                Individual Staff</label>
                            <label>
                                <asp:RadioButton ID="rbAssignTeam" runat="server" GroupName="AssignType" onclick="awrToggleAssignType();" />
                                Team</label>
                        </div>
                    </div>
                </div>

                <div class="awr-form-row" id="rowAssignIndividual" runat="server">
                    <div class="awr-form-group">
                        <label>ASSIGNED TO (STAFF / CLEANER) <span class="req">*</span></label>
                        <asp:DropDownList ID="ddlAssignedTo" runat="server" CssClass="awr-select" DataTextField="Name" DataValueField="StaffID">
                            <asp:ListItem Text="-- Select Staff --" Value="" />
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="awr-form-row" id="rowAssignTeam" runat="server">
                    <div class="awr-form-group">
                        <label>TEAM<span class="req">*</span></label>
                        <asp:DropDownList ID="ddlTeam" runat="server" CssClass="awr-select" DataTextField="TeamName" DataValueField="TeamID"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlTeam_SelectedIndexChanged" CausesValidation="false">
                            <asp:ListItem Text="-- Select Team --" Value="" />
                        </asp:DropDownList>
                    </div>
                    <div class="awr-form-group">
                        <label>TEAM SUPERVISOR</label>

                        <asp:DropDownList ID="ddlTeamSupervisor" runat="server" CssClass="awr-select" DataTextField="SupervisorName" DataValueField="TeamSupervisorID">
                            <asp:ListItem Text="-- Select Team First --" Value="" />
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="awr-form-row">
                    <div class="awr-form-group">
                        <label>REQUESTED BY  <span class="req">*</span></label>
                        <asp:DropDownList ID="ddlRequestedBy" runat="server" CssClass="awr-select" DataTextField="Name" DataValueField="StaffID">
                            <asp:ListItem Text="-- Select Supervisor --" Value="" />
                        </asp:DropDownList>
                    </div>
                    <div class="awr-form-group">
                        <label>APPROVED BY</label>
                        <asp:DropDownList ID="ddlApprovedBy" runat="server" CssClass="awr-select" DataTextField="Name" DataValueField="StaffID">
                            <asp:ListItem Text="-- Select Supervisor --" Value="" />
                        </asp:DropDownList>
                    </div>
                </div>
                <p class="awr-section-label">REQUEST STATUS</p>
                <div class="awr-form-row">
                    <div class="awr-form-group">
                        <label>PRIORITY</label>
                        <asp:DropDownList ID="ddlPriority" runat="server" CssClass="awr-select">
                            <asp:ListItem Text="Low" Value="Low" />
                            <asp:ListItem Text="Normal" Value="Normal" Selected="True" />
                            <asp:ListItem Text="High" Value="High" />
                            <asp:ListItem Text="Urgent" Value="Urgent" />
                        </asp:DropDownList>
                    </div>
                    <div class="awr-form-group">
                        <label>STATUS</label>

                        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="awr-select">
                            <asp:ListItem Text="Pending" Value="Pending" Selected="True" />
                            <asp:ListItem Text="In Progress" Value="In Progress" />
                            <asp:ListItem Text="Completed" Value="Completed" />
                            <asp:ListItem Text="Rejected" Value="Rejected" />
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="awr-form-row">
                    <div class="awr-form-group">
                        <label>REQUEST DATE</label>
                        <asp:TextBox ID="txtRequestDate" runat="server" CssClass="awr-input" TextMode="Date" />
                    </div>
                    <div class="awr-form-group">
                        <label>COMPLETED DATE</label>
                        <asp:TextBox ID="txtCompletedDate" runat="server" CssClass="awr-input" TextMode="Date" />
                    </div>
                </div>

                <div class="awr-form-row">
                    <div class="awr-form-group">
                        <label>TIME TAKEN</label>
                        <asp:DropDownList ID="ddlTimeTaken" runat="server" CssClass="awr-select" onchange="awrToggleTimeOther();">
                            <asp:ListItem Text="-- Select Time Taken --" Value="" />
                            <asp:ListItem Text="15 mins" Value="0.25" />
                            <asp:ListItem Text="30 mins" Value="0.5" />
                            <asp:ListItem Text="45 mins" Value="0.75" />
                            <asp:ListItem Text="1 hour" Value="1" />
                            <asp:ListItem Text="1 hour 30 mins" Value="1.5" />
                            <asp:ListItem Text="2 hours" Value="2" />
                            <asp:ListItem Text="2 hours 30 mins" Value="2.5" />
                            <asp:ListItem Text="3 hours" Value="3" />
                            <asp:ListItem Text="3 hours 30 mins" Value="3.5" />
                            <asp:ListItem Text="4 hours" Value="4" />
                            <asp:ListItem Text="Other (type manually)" Value="other" />
                        </asp:DropDownList>
                    </div>
                    <div class="awr-form-group" id="rowTimeOther" runat="server">
                        <label>Enter Time (hours, e.g. 4.5)</label>
                        <asp:TextBox ID="txtTimeTakenOther" runat="server" CssClass="awr-input" TextMode="Number" step="0.01" />
                    </div>
                    <div class="awr-form-group" style="flex: 2 1 100%;">
                        <label>Remarks</label>
                        <asp:TextBox ID="txtRemarks" runat="server" CssClass="awr-input" MaxLength="255" />
                    </div>
                </div>
            </div>
            <div class="awr-modal-foot">
                <asp:Button ID="btnCancel" runat="server" CssClass="awr-btn-cancel"
                    Text="Cancel" CausesValidation="false"
                    OnClick="btnCancel_Click" />

                <asp:Button ID="btnSave" runat="server" CssClass="awr-btn-save"
                    Text="Save Request" OnClick="btnSave_Click" />
            </div>
        </div>
    <script type="text/javascript">
        function openAwrModal() {
            document.getElementById('awrModalOverlay').classList.add('open');
            awrToggleAssignType();
            awrToggleTimeOther();
        }

        function closeAwrModal() {
            document.getElementById('awrModalOverlay').classList.remove('open');
        }

        function awrToggleAssignType() {
            var individual = document.getElementById('<%= rbAssignIndividual.ClientID %>').checked;
            document.getElementById('<%= rowAssignIndividual.ClientID %>').style.display = individual ? '' : 'none';
            document.getElementById('<%= rowAssignTeam.ClientID %>').style.display = individual ? 'none' : '';
        }

        function awrToggleTimeOther() {
            var ddl = document.getElementById('<%= ddlTimeTaken.ClientID %>');
            var otherRow = document.getElementById('<%= rowTimeOther.ClientID %>');
            otherRow.style.display = (ddl.value === 'other') ? '' : 'none';
        }
    </script>

</asp:Content>

