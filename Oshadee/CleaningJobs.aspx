<%@ Page Title="Cleaning Jobs" Language="C#" MasterPageFile="~/CmsMaster.master" AutoEventWireup="true" CodeFile="CleaningJobs.aspx.cs" Inherits="CleaningJob" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .cj-wrap {
            --black: #0B0D12;
            --black-dark: #000000;
            --orange: #E8622D;
            --orange-dark: #CC5222;
            --bg-light: #F4F4F5;
            --gray-text: #64748B;
            --border-color: #E2E8F0;
            font-family: 'Segoe UI', Arial, sans-serif;
        }

        .cj-header-row {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 24px;
        }

        .cj-title {
            font-size: 22px;
            font-weight: 700;
            color: var(--black);
            margin: 0;
        }

        .cj-subtitle {
            color: var(--gray-text);
            font-size: 14px;
            margin-top: 4px;
        }

        .cj-btn-primary {
            background: var(--orange);
            color: #fff;
            border: none;
            padding: 12px 20px;
            border-radius: 8px;
            font-weight: 600;
            font-size: 14px;
            cursor: pointer;
            text-decoration: none;
        }

            .cj-btn-primary:hover {
                background: var(--orange-dark);
                color: #fff;
            }

        .cj-stats {
            display: grid;
            grid-template-columns: repeat(4,1fr);
            gap: 14px;
            margin-bottom: 20px;
        }

        .cj-stat-card {
            background: #fff;
            border: none;
            border-radius: 14px;
            padding: 14px 16px;
            height: 85px;
            display: flex;
            flex-direction: column;
            justify-content: center;
            box-shadow: 0 2px 8px rgba(0,0,0,0.05);
        }

        .cj-stat-label {
            text-transform: uppercase;
            font-size: 11px;
            letter-spacing: .04em;
            color: var(--gray-text);
            font-weight: 600;
        }

        .cj-stat-value {
            font-size: 24px;
            font-weight: 700;
            color: var(--black);
            margin-top: 4px;
        }

            .cj-stat-value.orange {
                color: var(--orange);
            }

        .cj-filters {
            background: #fff;
            border: 1px solid var(--border-color);
            border-radius: 10px;
            padding: 18px 20px;
            display: grid;
            grid-template-columns: repeat(4,1fr) auto;
            gap: 16px;
            align-items: end;
            margin-bottom: 20px;
        }

        .cj-filter-label {
            display: block;
            text-transform: uppercase;
            font-size: 11px;
            font-weight: 600;
            color: var(--gray-text);
            margin-bottom: 6px;
        }

        .cj-filters select {
            width: 100%;
            padding: 9px 10px;
            border: 1px solid var(--border-color);
            border-radius: 6px;
            font-size: 14px;
            background: #fff;
        }

        .cj-clear-btn {
            background: #fff;
            border: 1px solid var(--border-color);
            color: var(--black);
            padding: 9px 16px;
            border-radius: 6px;
            font-weight: 600;
            cursor: pointer;
            height: 40px;
        }

        .cj-table-wrap {
            background: #fff;
            border: 1px solid var(--border-color);
            border-radius: 10px;
            overflow: hidden;
        }

        table.cj-table {
            width: 100%;
            border-collapse: collapse;
        }

            table.cj-table thead tr {
                background: var(--black);
            }

            table.cj-table thead th {
                color: #fff;
                text-align: left;
                font-size: 12px;
                text-transform: uppercase;
                letter-spacing: .03em;
                padding: 14px 16px;
                font-weight: 600;
            }

            table.cj-table tbody td {
                padding: 14px 16px;
                border-bottom: 1px solid var(--border-color);
                font-size: 14px;
                color: var(--black);
            }

            table.cj-table tbody tr:last-child td {
                border-bottom: none;
            }

            table.cj-table tbody tr:hover {
                background: #FAFAFA;
            }


        .cj-pill {
            display: inline-block;
            padding: 4px 12px;
            border-radius: 20px;
            font-size: 11px;
            font-weight: 700;
            text-transform: uppercase;
        }

        .cj-pill-pending {
            background: #FEF3C7;
            color: #92400E;
        }

        .cj-pill-progress {
            background: #DBEAFE;
            color: #1E40AF;
        }

        .cj-pill-completed {
            background: #DCFCE7;
            color: #166534;
        }

        .cj-pill-cancelled {
            background: #FEE2E2;
            color: #991B1B;
        }

        .cj-pill-low {
            background: #F1F5F9;
            color: #334155;
        }

        .cj-pill-normal {
            background: #E0E7FF;
            color: #3730A3;
        }

        .cj-pill-high {
            background: #FFEDD5;
            color: #9A3412;
        }

        .cj-pill-urgent {
            background: #FEE2E2;
            color: #991B1B;
        }

        .cj-action-link {
            color: var(--orange);
            font-weight: 600;
            text-decoration: none;
            margin-right: 14px;
            font-size: 13px;
        }

            .cj-action-link:hover {
                text-decoration: underline;
            }

        .cj-action-delete {
            color: #B91C1C;
            font-weight: 600;
            text-decoration: none;
            font-size: 13px;
        }

            .cj-action-delete:hover {
                text-decoration: underline;
            }

        .cj-empty-row td {
            text-align: center;
            color: var(--gray-text);
            padding: 28px;
        }

        .cj-job-code {
            font-weight: 400;
            color: var(--black);
        }

        .cj-job-code-display {
            display: block;
            padding: 10px 12px;
            border: 1px solid var(--border-color);
            border-radius: 8px;
            background: var(--bg-light);
            font-weight: 700;
            color: var(--black);
            font-size: 14px;
        }


        .cj-modal-overlay {
            display: none;
            position: fixed;
            inset: 0;
            background: rgba(11,13,18,.55);
            z-index: 1000;
            align-items: flex-start;
            justify-content: center;
            padding: 40px 16px;
            overflow-y: auto;
        }

            .cj-modal-overlay.show {
                display: flex;
            }

        .cj-modal {
            background: #fff;
            width: 680px;
            max-width: 92%;
            max-height: 90vh;
            overflow-y: auto;
            border-radius: 14px;
            box-shadow: 0 20px 50px rgba(0,0,0,0.3);
        }

        .cj-modal-head {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 20px 24px;
            border-bottom: 1px solid var(--border-color);
        }

        .cj-modal-close {
            background: none;
            border: none;
            font-size: 20px;
            cursor: pointer;
            color: var(--gray-text);
            text-decoration: none !important;
        }

        .cj-modal-close {
            background: none;
            border: none;
            font-size: 20px;
            cursor: pointer;
            color: var(--gray-text);
        }

        .cj-modal-body {
            padding: 24px 26px;
        }

        .cj-section-label {
            text-transform: uppercase;
            font-size: 12px;
            font-weight: 700;
            color: var(--orange);
            letter-spacing: .04em;
            margin: 0 0 16px 0;
            padding-bottom: 10px;
            border-bottom: 1px solid var(--border-color);
        }

        .cj-form-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 18px 24px;
            margin-bottom: 24px;
        }

            .cj-form-grid.full {
                grid-template-columns: 1fr;
            }

        .cj-field label {
            display: block;
            text-transform: uppercase;
            font-size: 11px;
            font-weight: 700;
            color: var(--black);
            margin-bottom: 6px;
        }

        .cj-field select, .cj-field input[type=text], .cj-field input[type=date], .cj-field input[type=number], .cj-field textarea {
            width: 100%;
            padding: 10px 12px;
            border: 1px solid var(--border-color);
            border-radius: 8px;
            font-size: 14px;
            box-sizing: border-box;
        }

        .cj-field textarea {
            resize: vertical;
            min-height: 70px;
        }

        .cj-modal-foot {
            padding: 18px 24px;
            border-top: 1px solid var(--border-color);
            display: flex;
            justify-content: flex-end;
            gap: 12px;
        }

        .cj-btn-cancel {
            background: #fff;
            border: 1px solid var(--border-color);
            color: var(--black);
            padding: 11px 20px;
            border-radius: 8px;
            font-weight: 600;
            cursor: pointer;
            text-decoration: none !important;
        }

            .cj-btn-cancel:hover,
            .cj-btn-cancel:focus,
            .cj-btn-cancel:visited {
                text-decoration: none !important;
                color: var(--black);
            }
    </style>

    <div class="cj-wrap">

        <div class="cj-header-row">
            <div>
                <h1 class="cj-title">Cleaning Jobs</h1>
                <div class="cj-subtitle">Create, assign and track cleaning jobs across all sections</div>
            </div>
            <asp:LinkButton ID="btnOpenAdd" runat="server" CssClass="cj-btn-primary" OnClick="btnOpenAdd_Click">+ Add Cleaning Job</asp:LinkButton>
        </div>

        <div class="cj-stats">
            <div class="cj-stat-card">
                <div class="cj-stat-label">Pending</div>
                <div class="cj-stat-value orange">
                    <asp:Literal ID="litPending" runat="server" Text="0" />
                </div>
            </div>
            <div class="cj-stat-card">
                <div class="cj-stat-label">In Progress</div>
                <div class="cj-stat-value">
                    <asp:Literal ID="litInProgress" runat="server" Text="0" />
                </div>
            </div>
            <div class="cj-stat-card">
                <div class="cj-stat-label">Completed (This Month)</div>
                <div class="cj-stat-value">
                    <asp:Literal ID="litCompleted" runat="server" Text="0" />
                </div>
            </div>
            <div class="cj-stat-card">
                <div class="cj-stat-label">Total Jobs</div>
                <div class="cj-stat-value">
                    <asp:Literal ID="litTotal" runat="server" Text="0" />
                </div>
            </div>
        </div>

        <div class="cj-filters">
            <div>
                <span class="cj-filter-label">Section</span>
                <asp:DropDownList ID="ddlFilterSection" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed">
                    <asp:ListItem Text="All Sections" Value="" />
                </asp:DropDownList>
            </div>
            <div>
                <span class="cj-filter-label">Status</span>
                <asp:DropDownList ID="ddlFilterStatus" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed">
                    <asp:ListItem Text="All Status" Value="" />
                    <asp:ListItem Text="Pending" Value="Pending" />
                    <asp:ListItem Text="In Progress" Value="In Progress" />
                    <asp:ListItem Text="Completed" Value="Completed" />
                    <asp:ListItem Text="Cancelled" Value="Cancelled" />
                </asp:DropDownList>
            </div>
            <div>
                <span class="cj-filter-label">Priority</span>
                <asp:DropDownList ID="ddlFilterPriority" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed">
                    <asp:ListItem Text="All Priority" Value="" />
                    <asp:ListItem Text="Low" Value="Low" />
                    <asp:ListItem Text="Normal" Value="Normal" />
                    <asp:ListItem Text="High" Value="High" />
                    <asp:ListItem Text="Urgent" Value="Urgent" />
                </asp:DropDownList>
            </div>
            <div>
                <span class="cj-filter-label">Assigned To</span>
                <asp:DropDownList ID="ddlFilterAssignment" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed">
                    <asp:ListItem Text="All" Value="" />
                    <asp:ListItem Text="Team" Value="Team" />
                    <asp:ListItem Text="Staff" Value="Staff" />
                </asp:DropDownList>
            </div>
            <asp:LinkButton ID="btnClearFilters" runat="server" CssClass="cj-clear-btn" OnClick="btnClearFilters_Click">Clear</asp:LinkButton>
        </div>

        <div class="cj-table-wrap">
            <table class="cj-table">
                <thead>
                    <tr>

                        <th>Job Code</th>
                        <th>Section</th>
                        <th>Description</th>
                        <th>Cleaning Type</th>
                        <th>Scheduled Date</th>
                        <th>Assigned To</th>
                        <th>Status</th>
                        <th>Priority</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptJobs" runat="server" OnItemCommand="rptJobs_ItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td><span class="cj-job-code"><%# FormatJobCode(Eval("JobID")) %></span></td>
                                <td><%# Eval("SectionName") %></td>
                                <td><%# Eval("Description") %></td>
                                <td><%# Eval("CleaningType") %></td>
                                <td><%# Eval("ScheduledDate", "{0:dd MMM yyyy}") %></td>
                                <td><%# Eval("AssignedToDisplay") %></td>
                                <td><span class='cj-pill <%# GetStatusPillClass(Eval("Status").ToString()) %>'><%# Eval("Status") %></span></td>
                                <td><span class='cj-pill <%# GetPriorityPillClass(Eval("Priority").ToString()) %>'><%# Eval("Priority") %></span></td>
                                <td>
                                    <asp:LinkButton runat="server" CssClass="cj-action-link" CommandName="EditJob" CommandArgument='<%# Eval("JobID") %>'>Edit</asp:LinkButton>
                                    <asp:LinkButton runat="server" CssClass="cj-action-delete" CommandName="DeleteJob" CommandArgument='<%# Eval("JobID") %>'
                                        OnClientClick='<%# "return confirm(\x27Delete this cleaning job? This cannot be undone.\x27);" %>'>Delete</asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:PlaceHolder ID="phEmpty" runat="server" Visible="false">
                        <tr class="cj-empty-row">
                            <td colspan="10">No cleaning jobs found.</td>
                        </tr>
                    </asp:PlaceHolder>
                </tbody>
            </table>
        </div>


        <asp:Panel ID="pnlModalOverlay" runat="server" CssClass="cj-modal-overlay">
            <div class="cj-modal">
                <div class="cj-modal-head">
                    <h2 class="cj-modal-title">
                        <asp:Literal ID="litModalTitle" runat="server" Text="Add Cleaning Job" /></h2>
                    <asp:LinkButton ID="btnCloseX" runat="server" CssClass="cj-modal-close" OnClick="btnCancel_Click" CausesValidation="false">✕</asp:LinkButton>
                </div>

                <div class="cj-modal-body">
                    <asp:HiddenField ID="hfJobID" runat="server" Value="0" />

                    <p class="cj-section-label">Job Info</p>
                    <div class="cj-form-grid">
                        <div class="cj-field">
                            <label>Job ID</label>
                            <span class="cj-job-code-display">
                                <asp:Literal ID="litJobCode" runat="server" Text="" /></span>
                        </div>
                        <div class="cj-field">
                            <label>Section <span class="cj-req">*</span></label>
                            <asp:DropDownList ID="ddlSection" runat="server" Enabled="true" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlSection" InitialValue=""
                                ErrorMessage="Section is required" ValidationGroup="CleaningJob" Display="Dynamic" ForeColor="#DC2626" Font-Size="12px" />
                        </div>
                        <div class="cj-field">
                            <label>Cleaning Type</label>
                            <asp:DropDownList ID="ddlCleaningType" runat="server" Enabled="true">
                                <asp:ListItem Text="Select Type " Value="" />
                                <asp:ListItem Text="General Cleaning" />
                                <asp:ListItem Text="Deep Cleaning" />
                                <asp:ListItem Text="Window Cleaning" />
                                <asp:ListItem Text="Restroom Cleaning" />
                                <asp:ListItem Text="Floor Care" />
                                <asp:ListItem Text="Other" />
                            </asp:DropDownList>
                        </div>
                        <div class="cj-field">
                            <label>Priority</label>
                            <asp:DropDownList ID="ddlPriority" runat="server">
                                <asp:ListItem Text="Low" />
                                <asp:ListItem Text="Normal" Selected="True" />
                                <asp:ListItem Text="High" />
                                <asp:ListItem Text="Urgent" />
                            </asp:DropDownList>
                        </div>
                        <div class="cj-field">
                            <label>Status <span class="cj-req">*</span></label>
                            <asp:DropDownList ID="ddlStatus" runat="server">
                                <asp:ListItem Text="Pending" />
                                <asp:ListItem Text="In Progress" />
                                <asp:ListItem Text="Completed" />
                                <asp:ListItem Text="Cancelled" />
                            </asp:DropDownList>
                        </div>
                        <div class="cj-field">
                            <label>Frequency <span class="cj-req">*</span></label>
                            <asp:DropDownList ID="ddlFrequency" runat="server" Enabled="true">
                                <asp:ListItem Text="One Time" />
                                <asp:ListItem Text="Daily" />
                                <asp:ListItem Text="Weekly" />
                                <asp:ListItem Text="Monthly" />
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="cj-form-grid full">
                        <div class="cj-field">
                            <label>Description</label>
                            <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="3" MaxLength="255" />
                        </div>
                    </div>

                    <p class="cj-section-label">Assignment &amp; Schedule</p>
                    <div class="cj-form-grid">
                        <div class="cj-field">
                            <label>Assignment Type</label>
                            <asp:DropDownList ID="ddlAssignmentType" runat="server" onchange="cjToggleAssignment(this.value)">
                                <asp:ListItem Text="Team" />
                                <asp:ListItem Text="Staff" />
                            </asp:DropDownList>
                        </div>
                        <div class="cj-field">
                            <label>Team Supervisor</label>
                            <asp:DropDownList ID="ddlTeamSupervisor" runat="server">
                                <asp:ListItem Text="Select Supervisor " Value="" />
                            </asp:DropDownList>
                        </div>

                        <asp:Panel ID="pnlTeam" runat="server" CssClass="cj-field">
                            <label>Team</label>
                            <asp:DropDownList ID="ddlTeam" runat="server">
                                <asp:ListItem Text="Select Team " Value="" />
                            </asp:DropDownList>
                        </asp:Panel>
                        <asp:Panel ID="pnlStaff" runat="server" CssClass="cj-field">
                            <label>Staff</label>
                            <asp:DropDownList ID="ddlStaff" runat="server">
                                <asp:ListItem Text=" Select Staff " Value="" />
                            </asp:DropDownList>
                        </asp:Panel>

                        <div class="cj-field">
                            <label>Scheduled Date <span class="cj-req">*</span></label>
                            <asp:TextBox ID="txtScheduledDate" runat="server" TextMode="Date" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtScheduledDate"
                                ErrorMessage="Scheduled date is required" ValidationGroup="CleaningJob" Display="Dynamic" ForeColor="#DC2626" Font-Size="12px" />
                        </div>
                        <div class="cj-field">
                            <label>Expected Completion Date</label>
                            <asp:TextBox ID="txtExpectedCompletionDate" runat="server" TextMode="Date" />
                        </div>

                        <div class="cj-field">
                            <label>Linked Schedule (optional)</label>
                            <asp:DropDownList ID="ddlSchedule"
                                runat="server"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlSchedule_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>

                    </div>


                    <asp:Literal ID="litError" runat="server" />
                </div>

                <div class="cj-modal-foot">
                    <asp:LinkButton ID="btnCancel" runat="server" CssClass="cj-btn-cancel" OnClick="btnCancel_Click" CausesValidation="false">Cancel</asp:LinkButton>
                    <asp:LinkButton ID="btnSaveRecord" runat="server" CssClass="cj-btn-primary" OnClick="btnSaveRecord_Click" ValidationGroup="CleaningJob">Save Record</asp:LinkButton>
                </div>
      
            </div>
        </asp:Panel>

    </div>

    <script>
        function cjToggleAssignment(val) {
            var teamField = document.getElementById('<%= pnlTeam.ClientID %>');
            var staffField = document.getElementById('<%= pnlStaff.ClientID %>');
            if (!teamField || !staffField) return;
            if (val === 'Staff') { teamField.style.display = 'none'; staffField.style.display = 'block'; }
            else { teamField.style.display = 'block'; staffField.style.display = 'none'; }
        }

        document.addEventListener('DOMContentLoaded', function () {
            var ddl = document.getElementById('<%= ddlAssignmentType.ClientID %>');
            if (ddl) cjToggleAssignment(ddl.value);
        });

        function lockScheduleFields(lock) {

            document.getElementById('<%= ddlSection.ClientID %>').disabled = lock;

            document.getElementById('<%= ddlCleaningType.ClientID %>').disabled = lock;

            document.getElementById('<%= ddlFrequency.ClientID %>').disabled = lock;
        }
    </script>

</asp:Content>
