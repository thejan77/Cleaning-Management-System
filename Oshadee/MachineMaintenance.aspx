<%@ Page Title="Machine Maintenance" Language="C#" MasterPageFile="~/CmsMaster.Master" AutoEventWireup="true" CodeFile="MachineMaintenance.aspx.cs" Inherits="CleaningManagement_MachineMaintenance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <style>
        :root {
            --black: #0B0D12;
            --black-dark: #000000;
            --orange: #E8622D;
            --orange-dark: #CC5222;
            --bg-light: #F4F4F5;
            --gray-text: #64748B;
            --border-color: #E2E8F0;
        }

        .mm-page {
            font-family: 'Segoe UI', Arial, sans-serif;
        }

        .mm-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
        }

            .mm-header h2 {
                color: var(--black);
                font-size: 22px;
                font-weight: 700;
                margin: 0;
            }

            .mm-header p {
                color: var(--gray-text);
                font-size: 13px;
                margin: 4px 0 0 0;
            }

        .btn-orange {
            background: var(--orange);
            color: #fff;
            border: none;
            padding: 10px 20px;
            border-radius: 8px;
            font-weight: 600;
            font-size: 14px;
            cursor: pointer;
            text-decoration: none;
        }

            .btn-orange:hover {
                background: var(--orange-dark);
            }

        .btn-outline {
            background: #fff;
            color: var(--black);
            border: 1px solid var(--border-color);
            padding: 10px 18px;
            border-radius: 8px;
            font-weight: 600;
            font-size: 14px;
            cursor: pointer;
        }

            .btn-outline:hover {
                background: var(--bg-light);
            }


        .mm-summary-row {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 16px;
            margin-bottom: 20px;
        }

        .mm-summary-card {
            background: #fff;
            border: 1px solid var(--border-color);
            border-radius: 10px;
            padding: 18px 20px;
        }

            .mm-summary-card .label {
                font-size: 12px;
                color: var(--gray-text);
                text-transform: uppercase;
                letter-spacing: .3px;
                font-weight: 600;
            }

            .mm-summary-card .value {
                font-size: 24px;
                font-weight: 700;
                color: var(--black);
                margin-top: 6px;
            }

            .mm-summary-card.accent .value {
                color: var(--orange);
            }

        .mm-filter-card {
            background: #fff;
            border: 1px solid var(--border-color);
            border-radius: 10px;
            padding: 18px 20px;
            margin-bottom: 20px;
        }

        .mm-filter-grid {
            display: grid;
            grid-template-columns: repeat(4, 1fr) auto;
            gap: 14px;
            align-items: end;
        }

        .mm-filter-item label {
            display: block;
            font-size: 12px;
            font-weight: 600;
            color: var(--gray-text);
            margin-bottom: 6px;
            text-transform: uppercase;
            letter-spacing: .3px;
        }

        .mm-filter-item select,
        .mm-filter-item input[type=text] {
            width: 100%;
            padding: 9px 10px;
            border: 1px solid var(--border-color);
            border-radius: 8px;
            font-size: 13px;
            color: var(--black);
            background: #fff;
            box-sizing: border-box;
            height: 38px;
        }

            .mm-filter-item select:focus,
            .mm-filter-item input:focus {
                outline: none;
                border-color: var(--orange);
            }

    
        .mm-filter-item .mm-clear-btn {
            width: 100%;
            height: 38px;
            box-sizing: border-box;
            display: inline-flex;
            align-items: center;
            justify-content: center;
        }


        .mm-table-card {
            background: #fff;
            border: 1px solid var(--border-color);
            border-radius: 10px;
            overflow: hidden;
            overflow-x: auto;
        }

        .mm-grid {
            width: 100%;
            border-collapse: collapse;
        }

            .mm-grid th {
                background: var(--black);
                color: #fff;
                font-size: 12px;
                text-transform: uppercase;
                letter-spacing: .3px;
                padding: 12px 14px;
                text-align: left;
                font-weight: 600;
                white-space: nowrap;
            }

            .mm-grid td {
                padding: 12px 14px;
                font-size: 13px;
                color: var(--black);
                border-bottom: 1px solid var(--border-color);
                vertical-align: middle;
            }

            .mm-grid tr:hover td {
                background: var(--bg-light);
            }


        .pill {
            display: inline-block;
            padding: 4px 12px;
            border-radius: 999px;
            font-size: 11px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: .3px;
        }

        .pill-pending {
            background: #FEF3C7;
            color: #92400E;
        }

        .pill-inprogress {
            background: #DBEAFE;
            color: #1E40AF;
        }

        .pill-completed {
            background: #D1FAE5;
            color: #065F46;
        }

        .pill-cancelled {
            background: #FEE2E2;
            color: #991B1B;
        }

        .pill-overdue {
            background: #FEE2E2;
            color: #991B1B;
        }

        .pill-scheduled {
            background: #F1F5F9;
            color: #334155;
        }

        .pill-breakdown {
            background: #FEE2E2;
            color: #991B1B;
        }

        .action-link {
            color: var(--orange);
            font-weight: 600;
            font-size: 12px;
            text-decoration: none;
            margin-right: 12px;
            cursor: pointer;
        }

            .action-link:hover {
                text-decoration: underline;
            }

            .action-link.danger {
                color: #B91C1C;
            }


        .mm-modal-overlay {
            display: none;
            position: fixed;
            inset: 0;
            background: rgba(11,13,18,0.55);
            z-index: 1000;
            align-items: center;
            justify-content: center;
        }

            .mm-modal-overlay.active {
                display: flex;
            }

        .mm-modal {
            background: #fff;
            width: 680px;
            max-width: 92%;
            max-height: 90vh;
            overflow-y: auto;
            border-radius: 14px;
            box-shadow: 0 20px 50px rgba(0,0,0,0.3);
        }

        .mm-modal-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 18px 24px;
            border-bottom: 1px solid var(--border-color);
            position: sticky;
            top: 0;
            background: #fff;
            border-radius: 14px 14px 0 0;
        }

            .mm-modal-header h3 {
                margin: 0;
                font-size: 17px;
                color: var(--black);
                font-weight: 700;
            }

        .mm-modal-close {
            background: none;
            border: none;
            font-size: 20px;
            color: var(--gray-text);
            cursor: pointer;
        }

            .mm-modal-close:hover {
                color: var(--black);
            }

        .mm-modal-body {
            padding: 20px 24px;
        }

        .mm-form-row {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 16px;
            margin-bottom: 16px;
        }

        .mm-form-group.full {
            grid-column: 1 / -1;
        }

        .mm-form-group label {
            display: block;
            font-size: 12px;
            font-weight: 600;
            color: var(--black);
            margin-bottom: 6px;
            text-transform: uppercase;
            letter-spacing: .3px;
        }

        .mm-form-group select,
        .mm-form-group input[type=text],
        .mm-form-group input[type=date],
        .mm-form-group input[type=number],
        .mm-form-group textarea {
            width: 100%;
            padding: 10px 12px;
            border: 1px solid var(--border-color);
            border-radius: 8px;
            font-size: 13px;
            color: var(--black);
            box-sizing: border-box;
            background: #fff;
        }


        .mm-form-group select:focus,
        .mm-form-group input:focus,
        .mm-form-group textarea:focus {
            outline: none;
            border-color: var(--orange);
        }

     
        .mm-code-display {
            display: block;
            padding: 10px 12px;
            border: 1px solid var(--border-color);
            border-radius: 8px;
            background: var(--bg-light);
            color: var(--black);
            font-size: 13px;
            font-weight: 600;
            box-sizing: border-box;
        }

        .mm-section-title {
            font-size: 12px;
            font-weight: 700;
            color: var(--orange-dark);
            text-transform: uppercase;
            letter-spacing: .4px;
            margin: 4px 0 12px 0;
            padding-top: 8px;
            border-top: 1px dashed var(--border-color);
        }

        .mm-modal-body > .mm-section-title:first-child {
            border-top: none;
            padding-top: 0;
        }

        .mm-modal-footer {
            padding: 16px 24px;
            border-top: 1px solid var(--border-color);
            display: flex;
            justify-content: flex-end;
            gap: 10px;
            position: sticky;
            bottom: 0;
            background: #fff;
            border-radius: 0 0 14px 14px;
        }

        .mm-hidden {
            display: none !important;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">


    <div class="mm-header">
        <div>
            <h2>Machine Maintenance</h2>
            <p>Track scheduled servicing and breakdown repairs for cleaning machines</p>
        </div>
        <asp:LinkButton ID="btnAddMaintenance" runat="server" CssClass="btn-orange" OnClick="btnAddMaintenance_Click">
            + Add Maintenance Record
        </asp:LinkButton>
    </div>

    <div class="mm-summary-row">
        <div class="mm-summary-card">
            <div class="label">Pending</div>
            <asp:Label ID="lblCountPending" runat="server" CssClass="value" Text="0" />
        </div>
        <div class="mm-summary-card accent">
            <div class="label">In Progress</div>
            <asp:Label ID="lblCountInProgress" runat="server" CssClass="value" Text="0" />
        </div>
        <div class="mm-summary-card">
            <div class="label">Completed (This Month)</div>
            <asp:Label ID="lblCountCompleted" runat="server" CssClass="value" Text="0" />
        </div>
        <div class="mm-summary-card">
            <div class="label">Breakdown Reports</div>
            <asp:Label ID="lblCountBreakdown" runat="server" CssClass="value" Text="0" />
        </div>
    </div>


    <div class="mm-filter-card">
        <div class="mm-filter-grid">
            <div class="mm-filter-item">
                <label>Machine</label>
                <asp:DropDownList ID="ddlFilterMachine" runat="server" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlFilterMachine_SelectedIndexChanged">
                    <asp:ListItem Text="All Machines" Value="" />
                </asp:DropDownList>
            </div>
            <div class="mm-filter-item">
                <label>Record Type</label>
                <asp:DropDownList ID="ddlFilterRecordType" runat="server" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlFilterRecordType_SelectedIndexChanged">
                    <asp:ListItem Text="All Types" Value="" />
                    <asp:ListItem Text="Scheduled" Value="Scheduled" />
                    <asp:ListItem Text="Breakdown" Value="Breakdown" />
                </asp:DropDownList>
            </div>
            <div class="mm-filter-item">
                <label>Status</label>
                <asp:DropDownList ID="ddlFilterStatus" runat="server" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlFilterStatus_SelectedIndexChanged">
                    <asp:ListItem Text="All Status" Value="" />
                    <asp:ListItem Text="Pending" Value="Pending" />
                    <asp:ListItem Text="In Progress" Value="In Progress" />
                    <asp:ListItem Text="Completed" Value="Completed" />
                    <asp:ListItem Text="Cancelled" Value="Cancelled" />
                </asp:DropDownList>
            </div>
            <div class="mm-filter-item">
                <label>Maintenance Type</label>
                <asp:DropDownList ID="ddlFilterMaintenanceType" runat="server" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlFilterMaintenanceType_SelectedIndexChanged">
                    <asp:ListItem Text="All" Value="" />
                    <asp:ListItem Text="Preventive" Value="Preventive" />
                    <asp:ListItem Text="Corrective" Value="Corrective" />
                    <asp:ListItem Text="Inspection" Value="Inspection" />
                    <asp:ListItem Text="Calibration" Value="Calibration" />
                </asp:DropDownList>
            </div>
            <div class="mm-filter-item">
                <label>&nbsp;</label>
                <asp:LinkButton ID="btnClearFilters" runat="server" CssClass="btn-outline mm-clear-btn"
                    OnClick="btnClearFilters_Click">Clear</asp:LinkButton>
            </div>
        </div>
    </div>


    <div class="mm-table-card">
        <asp:GridView ID="gvMaintenance" runat="server" CssClass="mm-grid" AutoGenerateColumns="false"
            DataKeyNames="MaintenanceID" GridLines="None" OnRowCommand="gvMaintenance_RowCommand"
            OnRowDataBound="gvMaintenance_RowDataBound" EmptyDataText="No maintenance records found.">
            <Columns>
                <asp:TemplateField HeaderText="Machine ID">
                    <ItemTemplate>
                        <%# FormatMaintenanceCode(Eval("MaintenanceID")) %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="MachineName" HeaderText="Machine" />

                <asp:TemplateField HeaderText="Record Type">
                    <ItemTemplate>
                        <asp:Label ID="lblRecordType" runat="server" CssClass="pill" Text='<%# Eval("RecordType") %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="MaintenanceType" HeaderText="Type" />
                <asp:BoundField DataField="ScheduledDate" HeaderText="Scheduled" DataFormatString="{0:dd MMM yyyy}" />
                <asp:BoundField DataField="AssignedToName" HeaderText="Assigned To" />
                <asp:BoundField DataField="Cost" HeaderText="Cost (LKR)" DataFormatString="{0:N2}" />

                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <asp:Label ID="lblStatus" runat="server" CssClass="pill" Text='<%# Eval("Status") %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEdit" runat="server" CssClass="action-link" CommandName="EditMaintenance"
                            CommandArgument='<%# Eval("MaintenanceID") %>'>Edit</asp:LinkButton>
                        <asp:LinkButton ID="lnkDelete" runat="server" CssClass="action-link danger" CommandName="DeleteMaintenance"
                            CommandArgument='<%# Eval("MaintenanceID") %>'
                            OnClientClick="return confirm('Delete this maintenance record?');">Delete</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <div id="mmModalOverlay" class="mm-modal-overlay">
        <div class="mm-modal">
            <div class="mm-modal-header">
                <h3 id="mmModalTitle" runat="server">Add Maintenance Record</h3>
                <button type="button" class="mm-modal-close" onclick="closeMmModal();">✕</button>
            </div>

            <div class="mm-modal-body">
                <asp:HiddenField ID="hfMaintenanceID" runat="server" Value="0" />

                <div class="mm-section-title">Machine &amp; Record Info</div>

                <div class="mm-form-row">
                    <div class="mm-form-group">
                        <label>Machine ID</label>
                        <span class="mm-code-display"><asp:Literal ID="litMaintenanceCode" runat="server" /></span>
                    </div>
                    <div class="mm-form-group">
                        <label>Machine</label>
                        <asp:DropDownList ID="ddlMachine" runat="server" ClientIDMode="Static">
                            <asp:ListItem Text="-- Select Machine --" Value="" />
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="mm-form-row">
                    <div class="mm-form-group">
                        <label>Record Type</label>
                        <asp:DropDownList ID="ddlRecordType" runat="server" ClientIDMode="Static" onchange="toggleRecordType();">
                            <asp:ListItem Text="Scheduled" Value="Scheduled" />
                            <asp:ListItem Text="Breakdown" Value="Breakdown" />
                        </asp:DropDownList>
                    </div>
                    <div class="mm-form-group">
                        <label>Maintenance Type</label>
                        <asp:DropDownList ID="ddlMaintenanceType" runat="server" ClientIDMode="Static">
                            <asp:ListItem Text="Preventive" Value="Preventive" />
                            <asp:ListItem Text="Corrective" Value="Corrective" />
                            <asp:ListItem Text="Inspection" Value="Inspection" />
                            <asp:ListItem Text="Calibration" Value="Calibration" />
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="mm-form-row">
                    <div class="mm-form-group">
                        <label>Status</label>
                        <asp:DropDownList ID="ddlStatus" runat="server" ClientIDMode="Static">
                            <asp:ListItem Text="Pending" Value="Pending" Selected="True" />
                            <asp:ListItem Text="In Progress" Value="In Progress" />
                            <asp:ListItem Text="Completed" Value="Completed" />
                            <asp:ListItem Text="Cancelled" Value="Cancelled" />
                        </asp:DropDownList>
                    </div>
                    <div class="mm-form-group">
                        <label>&nbsp;</label>
                    </div>
                </div>

                <div class="mm-form-row">
                    <div class="mm-form-group full">
                        <label>Issue Description</label>
                        <asp:TextBox ID="txtIssueDescription" runat="server" TextMode="MultiLine" Rows="2" ClientIDMode="Static" />
                    </div>
                </div>


                <div id="rowBreakdownDate" class="mm-form-row">
                    <div class="mm-form-group">
                        <label>Breakdown Date</label>
                        <asp:TextBox ID="txtBreakdownDate" runat="server" TextMode="DateTimeLocal" ClientIDMode="Static" />
                    </div>
                    <div class="mm-form-group">
                        <label>Scheduled Date</label>
                        <asp:TextBox ID="txtScheduledDate" runat="server" TextMode="Date" ClientIDMode="Static" />
                    </div>
                </div>

                <div id="rowRepairDetails" class="mm-form-row">
                    <div class="mm-form-group full">
                        <label>Repair Details</label>
                        <asp:TextBox ID="txtRepairDetails" runat="server" TextMode="MultiLine" Rows="2" ClientIDMode="Static" />
                    </div>
                </div>


                <div class="mm-section-title">Assignment &amp; Schedule</div>

                <div class="mm-form-row">
                    <div class="mm-form-group">
                        <label>Assigned To (Staff)</label>
                        <asp:DropDownList ID="ddlAssignedTo" runat="server" ClientIDMode="Static">
                            <asp:ListItem Text="-- Select Staff --" Value="" />
                        </asp:DropDownList>
                    </div>
                    <div class="mm-form-group">
                        <label>Completed Date</label>
                        <asp:TextBox ID="txtCompletedDate" runat="server" TextMode="Date" ClientIDMode="Static" />
                    </div>
                </div>

                <div class="mm-form-row">
                    <div class="mm-form-group">
                        <label>Cost (LKR)</label>
                        <asp:TextBox ID="txtCost" runat="server" TextMode="Number" ClientIDMode="Static" />
                    </div>
                    <div class="mm-form-group">
                        <label>&nbsp;</label>
                    </div>
                </div>

                <div class="mm-form-row">
                    <div class="mm-form-group full">
                        <label>Remarks</label>
                        <asp:TextBox ID="txtRemarks" runat="server" TextMode="MultiLine" Rows="2" ClientIDMode="Static" />
                    </div>
                </div>

            </div>

            <div class="mm-modal-footer">
                <button type="button" class="btn-outline" onclick="closeMmModal();">Cancel</button>
                <asp:LinkButton ID="btnSaveMaintenance" runat="server" CssClass="btn-orange" OnClick="btnSaveMaintenance_Click">
                    Save Record
                </asp:LinkButton>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function openMmModal() {
            document.getElementById('mmModalOverlay').classList.add('active');
            toggleRecordType();
        }

        function closeMmModal() {
            document.getElementById('mmModalOverlay').classList.remove('active');
        }

        function toggleRecordType() {
            var recordType = document.getElementById('ddlRecordType').value;
            var breakdownRow = document.getElementById('rowBreakdownDate');
            var repairRow = document.getElementById('rowRepairDetails');

            if (recordType === 'Breakdown') {
                breakdownRow.classList.remove('mm-hidden');
                repairRow.classList.remove('mm-hidden');
            } else {
                breakdownRow.classList.add('mm-hidden');
                repairRow.classList.add('mm-hidden');
            }
        }
    </script>

</asp:Content>