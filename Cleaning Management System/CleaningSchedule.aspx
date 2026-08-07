<%@ Page Title="Cleaning Schedule" Language="C#" MasterPageFile="~/CmsMaster.master" AutoEventWireup="true" CodeFile="CleaningSchedule.aspx.cs" Inherits="CleaningSchedule" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .sch-wrap {
            --black: #0B0D12;
            --black-dark: #000000;
            --orange: #E8622D;
            --orange-dark: #CC5222;
            --bg-light: #F4F4F5;
            --gray-text: #64748B;
            --border-color: #E2E8F0;
            --orange-light: #FDECD8;
            --orange-soft: #FFF6F1;
            --orange-border: #F5C7A8;
            --orange-text: #CC5222;

            .sch-wrap {
                padding: 0;
                margin: 0;
                background: transparent;
                border: none;
                box-shadow: none;
            }

            .sch-header-row {
                display: flex;
                justify-content: space-between;
                align-items: flex-start;
                margin-bottom: 24px;
                gap: 16px;
                flex-wrap: wrap;
            }

            .sch-title {
                font-size: 22px;
                font-weight: 700;
                color: var(--black);
                margin: 0;
            }

            .sch-subtitle {
                color: var(--gray-text);
                font-size: 13px;
                margin-top: 4px;
            }

            .sch-header-actions {
                display: flex;
                align-items: center;
                gap: 10px;
            }
            .sch-view-toggle {
                display: flex;
                background: #fff;
                border: 1px solid var(--border-color);
                border-radius: 8px;
                padding: 3px;
                gap: 2px;
            }

            .sch-view-btn {
                border: none;
                background: transparent;
                width: 38px;
                height: 34px;
                border-radius: 6px;
                display: inline-flex;
                align-items: center;
                justify-content: center;
                cursor: pointer;
                color: var(--gray-text);
                text-decoration: none;
                vertical-align: middle;
            }

                .sch-view-btn svg {
                    width: 18px;
                    height: 18px;
                    pointer-events: none;
                }

                .sch-view-btn:hover {
                    color: var(--black);
                    background: var(--bg-light);
                    text-decoration: none;
                }

                .sch-view-btn.active {
                    background: var(--black);
                    color: #fff;
                }

            .sch-btn-primary {
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

                .sch-btn-primary:hover {
                    background: var(--orange-dark);
                    color: #fff;
                }

            .sch-stats {
                display: grid;
                grid-template-columns: repeat(4,1fr);
                gap: 14px;
                margin-bottom: 20px;
            }

            .sch-stat-card {
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

            .sch-stat-label {
                text-transform: uppercase;
                font-size: 11px;
                letter-spacing: .04em;
                color: var(--black);
                font-weight: 600;
            }

            .sch-stat-value {
                font-size: 24px;
                font-weight: 700;
                color: var(--black);
                margin-top: 4px;
            }

                .sch-stat-value.orange {
                    color: var(--orange);
                }

            .sch-tabs {
                display: flex;
                gap: 8px;
                margin-bottom: 20px;
                background: #fff;
                border: 1px solid var(--border-color);
                border-radius: 10px;
                padding: 6px;
                width: fit-content;
            }

            .sch-tab {
                border: none;
                background: transparent;
                padding: 10px 22px;
                border-radius: 8px;
                font-weight: 600;
                font-size: 13px;
                text-transform: uppercase;
                letter-spacing: .03em;
                color: var(--gray-text);
                cursor: pointer;
                text-decoration: none;
            }

                .sch-tab:hover {
                    color: var(--black);
                    text-decoration: none;
                }

                .sch-tab.active {
                    background: var(--black);
                    color: #fff;
                }
.sch-filters {
    background: #fff;
    border: 1px solid var(--border-color);
    border-radius: 12px;
    padding: 18px 20px;
    display: grid;
    grid-template-columns: 230px 230px 230px auto;
    gap: 12px;
    align-items: end;
    justify-content: start;
    margin-bottom: 20px;
}


.sch-filters.no-freq {
    grid-template-columns: 230px 230px auto;
    justify-content: start;
    gap: 12px;
}

.sch-filters.no-freq .sch-filter-freq {
    display: none;
}

.sch-filter-label {
    display: block;
    text-transform: uppercase;
    font-size: 11px;
    font-weight: 600;
    color: var(--gray-text);
    margin-bottom: 6px;
}

.sch-filters select {
    width: 230px;
    height: 40px;
    padding: 9px 10px;
    border: 1px solid var(--border-color);
    border-radius: 6px;
    font-size: 14px;
    background: #fff;
    box-sizing: border-box;
}

.sch-clear-btn {
    background: #fff;
    border: 1px solid var(--border-color);
    color: var(--black);
    padding: 0 18px;
    border-radius: 6px;
    font-weight: 600;
    cursor: pointer;
    height: 40px;
    white-space: nowrap;
    text-decoration: none !important;
}

.sch-clear-btn:hover {
    background: var(--bg-light);
    border-color: var(--orange);
    color: var(--orange);
    text-decoration: none !important;
}

            .sch-pill {
                display: inline-block;
                padding: 4px 12px;
                border-radius: 20px;
                font-size: 11px;
                font-weight: 700;
                text-transform: uppercase;
            }

            .sch-pill-active {
                background: #DCFCE7;
                color: #166534;
            }

            .sch-pill-inactive {
                background: #F1F5F9;
                color: #334155;
            }

            .sch-pill-completed {
                background: #E0E7FF;
                color: #3730A3;
            }

            .sch-empty-msg {
                background: #fff;
                border: 1px solid var(--border-color);
                border-radius: 12px;
                padding: 40px;
                text-align: center;
                color: var(--gray-text);
                font-size: 14px;
            }
            
            .sch-table-wrap {
                background: #fff;
                border-radius: 12px;
                overflow: hidden;
                box-shadow: 0 2px 8px rgba(0,0,0,0.05);
            }

            table.sch-table {
                width: 100%;
                border-collapse: collapse;
                font-size: 13.5px;
            }

                table.sch-table thead {
                    background: var(--black);
                }

                table.sch-table th {
                    color: #fff;
                    text-align: left;
                    padding: 13px 16px;
                    font-size: 11.5px;
                    text-transform: uppercase;
                    letter-spacing: .04em;
                    font-weight: 600;
                }

                    table.sch-table th:first-child,
                    table.sch-table td:first-child {
                        width: 110px;
                        white-space: nowrap;
                    }

                table.sch-table td {
                    padding: 13px 16px;
                    border-bottom: 1px solid var(--border-color);
                    color: var(--black);
                }

                table.sch-table tbody tr:hover {
                    background: var(--bg-light);
                }

                table.sch-table tbody tr:last-child td {
                    border-bottom: none;
                }

            .sch-freq-badge {
                display: inline-block;
                font-size: 11px;
                font-weight: 700;
                padding: 3px 10px;
                border-radius: 6px;
                background: var(--orange-soft);
                color: var(--orange-dark);
            }

            .sch-row-actions a {
                color: var(--black);
                font-size: 12px;
                font-weight: 700;
                text-decoration: none;
                margin-right: 14px;
            }

                .sch-row-actions a:hover {
                    color: var(--orange);
                }

                .sch-row-actions a:last-child {
                    margin-right: 0;
                }
        
            .sch-daily-wrap {
                display: flex;
                gap: 24px;
            }

            .sch-day-badge {
                width: 140px;
                flex-shrink: 0;
                background: #fff;
                border: 1px solid var(--border-color);
                border-radius: 14px;
                padding: 20px 16px;
                text-align: center;
                height: max-content;
            }

                .sch-day-badge .num {
                    font-size: 40px;
                    font-weight: 800;
                    line-height: 1;
                    color: var(--black);
                }

                .sch-day-badge .name {
                    font-size: 12px;
                    font-weight: 700;
                    color: var(--gray-text);
                    letter-spacing: .5px;
                    margin-top: 4px;
                }

            .sch-agenda {
                flex: 1;
            }

            .sch-agenda-head h2 {
                margin: 0 0 2px 0;
                font-size: 19px;
                color: var(--black);
            }

            .sch-agenda-head p {
                margin: 0 0 16px 0;
                color: var(--gray-text);
                font-size: 13px;
            }

            .sch-timeline {
                position: relative;
                padding-left: 56px;
            }

            .sch-time-row {
                position: relative;
                margin-bottom: 16px;
            }

            .sch-time-label {
                position: absolute;
                left: -56px;
                top: 14px;
                width: 46px;
                text-align: right;
                font-size: 11.5px;
                color: var(--gray-text);
                font-weight: 600;
            }

            .sch-time-row::before {
                content: '';
                position: absolute;
                left: -16px;
                top: 18px;
                width: 8px;
                height: 8px;
                border-radius: 50%;
                background: var(--border-color);
            }
            

            .sch-job-card {
                background: #fff;
                border: 1px solid var(--border-color);
                border-left: 4px solid var(--orange);
                border-radius: 10px;
                padding: 12px 14px;
                margin-bottom: 12px;
                cursor: pointer;
                transition: all .2s ease;
            }

                .sch-job-card:hover {
                    background: #FDECD8;
                    border-color: var(--orange);
                }

            .sch-jc-time {
                font-size: 11px;
                font-weight: 700;
                color: var(--orange-dark);
                margin-bottom: 4px;
            }

            .sch-jc-name {
                font-size: 14px;
                font-weight: 700;
                color: var(--black);
                margin-bottom: 4px;
            }

            .sch-jc-type {
                font-size: 12px;
                color: var(--gray-text);
                margin-bottom: 8px;
            }

            .sch-jc-foot {
                display: flex;
                justify-content: space-between;
                align-items: center;
                margin-top: 6px;
            }

            .sch-jc-status {
                display: inline-block;
                background: #D1FAE5;
                color: #065F46;
                font-size: 10px;
                font-weight: 700;
                padding: 3px 10px;
                border-radius: 20px;
            }

            .sch-jc-actions a {
                color: var(--black);
                font-size: 11px;
                font-weight: 700;
                text-decoration: none;
                margin-left: 12px;
            }

                .sch-jc-actions a:hover {
                    color: var(--orange);
                    text-decoration: underline;
                }
            .sch-weekly-wrap {
                background: #fff;
                border: 1px solid var(--border-color);
                border-radius: 12px;
                padding: 20px;
            }

            .sch-week-grid {
                display: grid;
                grid-template-columns: repeat(7, 1fr);
                border-top: 1px solid var(--border-color);
                border-left: 1px solid var(--border-color);
            }

            .sch-week-head {
                background: var(--bg-light);
                text-align: center;
                padding: 10px;
                font-size: 12px;
                font-weight: 700;
                color: var(--black);
                border-right: 1px solid var(--border-color);
                border-bottom: 1px solid var(--border-color);
            }

            .sch-week-col {
                border-right: 1px solid var(--border-color);
                border-bottom: 1px solid var(--border-color);
                min-height: 160px;
                padding: 8px;
            }

            .sch-week-empty {
                color: var(--border-color);
            }

            .sch-job-chip {
                background: var(--bg-light);
                border-left: 3px solid var(--orange);
                border-radius: 6px;
                padding: 8px 10px;
                font-size: 12px;
                cursor: pointer;
                margin-bottom: 6px;
            }

                .sch-job-chip:hover {
                    background: #FDECD8;
                }

            .sch-chip-time {
                font-size: 11px;
                font-weight: 700;
                color: var(--orange-dark);
                margin-bottom: 2px;
            }

            .sch-chip-name {
                font-weight: 700;
                margin-bottom: 2px;
                color: var(--black);
            }

            .sch-chip-type {
                color: var(--gray-text);
                margin-bottom: 4px;
            }

            .sch-chip-status {
                display: inline-block;
                font-size: 10px;
                font-weight: 700;
                padding: 2px 8px;
                border-radius: 20px;
            }
           
            .sch-monthly-wrap {
                background: #fff;
                border: 1px solid var(--border-color);
                border-radius: 12px;
                padding: 20px;
            }

            .sch-month-nav {
                display: flex;
                justify-content: center;
                align-items: center;
                gap: 16px;
                margin-bottom: 18px;
            }

            .sch-month-nav-btn {
                border: 1px solid var(--border-color);
                background: #fff;
                border-radius: 8px;
                width: 32px;
                height: 32px;
                cursor: pointer;
                font-size: 14px;
                color: var(--black);
            }

            .sch-month-title {
                font-weight: 700;
                font-size: 15px;
                color: var(--black);
                letter-spacing: .5px;
            }

            .sch-month-grid {
                display: grid;
                grid-template-columns: repeat(7, 1fr);
                border-top: 1px solid var(--border-color);
                border-left: 1px solid var(--border-color);
            }

            .sch-month-dow {
                background: var(--bg-light);
                text-align: center;
                padding: 8px;
                font-size: 11px;
                font-weight: 700;
                color: var(--gray-text);
                border-right: 1px solid var(--border-color);
                border-bottom: 1px solid var(--border-color);
            }

            .sch-month-cell {
                border-right: 1px solid var(--border-color);
                border-bottom: 1px solid var(--border-color);
                min-height: 92px;
                padding: 6px;
            }

                .sch-month-cell.blank {
                    background: #FAFAFB;
                }

            .sch-month-daynum {
                font-size: 12px;
                font-weight: 700;
                color: var(--gray-text);
                margin-bottom: 4px;
            }

            .sch-month-job {
                background: var(--amber-bg);
                border-radius: 5px;
                padding: 4px 6px;
                font-size: 10.5px;
                margin-bottom: 3px;
                cursor: pointer;
            }

                .sch-month-job:hover {
                    background: #FBD9BB;
                }

            .sch-month-jname {
                font-weight: 700;
                color: var(--orange-dark);
            }

            .sch-month-jtime {
                color: var(--gray-text);
                font-size: 10px;
            }
           
            .sch-modal-overlay {
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

                .sch-modal-overlay.show {
                    display: flex;
                }

            .sch-modal {
                background: #fff;
                width: 680px;
                max-width: 92%;
                max-height: 90vh;
                overflow-y: auto;
                border-radius: 14px;
                box-shadow: 0 20px 50px rgba(0,0,0,0.3);
            }

            .sch-modal-head {
                display: flex;
                justify-content: space-between;
                align-items: center;
                padding: 18px 24px;
                border-bottom: 1px solid var(--border-color);
            }

            .sch-modal-title {
                font-size: 17px;
                font-weight: 700;
                color: var(--black);
                margin: 0;
            }

            .sch-modal-close {
                background: none;
                border: none;
                font-size: 20px;
                cursor: pointer;
                color: var(--gray-text);
                text-decoration: none !important;
            }

                .sch-modal-close:hover,
                .sch-modal-close:focus,
                .sch-modal-close:visited {
                    color: var(--gray-text);
                    text-decoration: none !important;
                }

            .sch-modal-body {
                padding: 20px 24px;
            }

            .sch-section-label {
                text-transform: uppercase;
                font-size: 12px;
                font-weight: 700;
                color: var(--orange);
                letter-spacing: .04em;
                margin: 0 0 16px 0;
                padding-bottom: 10px;
                border-bottom: 1px solid var(--border-color);
            }

            .sch-form-grid {
                display: grid;
                grid-template-columns: 1fr 1fr;
                gap: 18px 24px;
                margin-bottom: 24px;
            }

                .sch-form-grid.full {
                    grid-template-columns: 1fr;
                }

            .sch-field label {
                display: block;
                text-transform: uppercase;
                font-size: 11px;
                font-weight: 700;
                color: var(--black);
                margin-bottom: 6px;
            }

            .sch-field select, .sch-field input[type=text], .sch-field input[type=date], .sch-field input[type=time], .sch-field textarea {
                width: 100%;
                padding: 10px 12px;
                border: 1px solid var(--border-color);
                border-radius: 8px;
                font-size: 14px;
                box-sizing: border-box;
            }

            .sch-field textarea {
                resize: vertical;
                min-height: 70px;
            }

            .sch-modal-foot {
                padding: 18px 26px;
                border-top: 1px solid var(--border-color);
                display: flex;
                justify-content: flex-end;
                gap: 12px;
            }

            .sch-btn-cancel {
                background: #fff;
                border: 1px solid var(--border-color);
                color: var(--black);
                padding: 10px 18px;
                border-radius: 8px;
                font-weight: 600;
                font-size: 14px;
                cursor: pointer;
                text-decoration: none !important;
            }

                .sch-btn-cancel:hover,
                .sch-btn-cancel:focus,
                .sch-btn-cancel:visited {
                    color: var(--black);
                    text-decoration: none !important;
                }

            .sch-req {
                color: #DC2626;
            }

            .sch-locked-field {
                background: var(--bg-light) !important;
                color: var(--gray-text) !important;
                cursor: not-allowed !important;
                font-weight: 600;
                letter-spacing: .03em;
            }

            .sch-month-jcode {
                color: var(--gray-text);
                font-size: 9.5px;
                margin-top: 1px;
            }
    </style>

    <script type="text/javascript">
        function editSchedule(id) {
            document.getElementById('<%= hfActionScheduleID.ClientID %>').value = id;
            __doPostBack('<%= lnkProxyEdit.UniqueID %>', '');
        }

        function deleteSchedule(id) {
            if (!confirm('Delete this cleaning schedule? This cannot be undone.')) return;
            document.getElementById('<%= hfActionScheduleID.ClientID %>').value = id;
            __doPostBack('<%= lnkProxyDelete.UniqueID %>', '');
        }
    </script>

    <div class="sch-wrap">

        <div class="sch-header-row">
            <div>
                <h1 class="sch-title">Cleaning Schedule</h1>
                <div class="sch-subtitle">Plan and manage recurring daily, weekly and monthly cleaning schedules</div>
            </div>
            <div class="sch-header-actions">
                <div class="sch-view-toggle">
                    <asp:LinkButton ID="btnViewTable" runat="server" CssClass="sch-view-btn active" CommandArgument="Table"
                        OnClick="View_Click" CausesValidation="false" ToolTip="Table View">
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                            <rect x="3" y="4" width="18" height="16" rx="2"></rect>
                            <line x1="3" y1="10" x2="21" y2="10"></line>
                            <line x1="9" y1="10" x2="9" y2="20"></line>
                        </svg>
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnViewCalendar" runat="server" CssClass="sch-view-btn" CommandArgument="Calendar"
                        OnClick="View_Click" CausesValidation="false" ToolTip="Calendar View">
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                            <rect x="3" y="5" width="18" height="16" rx="2"></rect>
                            <line x1="3" y1="10" x2="21" y2="10"></line>
                            <line x1="8" y1="2" x2="8" y2="6"></line>
                            <line x1="16" y1="2" x2="16" y2="6"></line>
                        </svg>
                    </asp:LinkButton>
                </div>
                <asp:LinkButton ID="btnOpenAdd" runat="server" CssClass="sch-btn-primary" OnClick="btnOpenAdd_Click">+ Add Cleaning Schedule</asp:LinkButton>
            </div>
        </div>

        <div class="sch-stats">
            <div class="sch-stat-card">
                <div class="sch-stat-label">Daily Schedules</div>
                <div class="sch-stat-value">
                    <asp:Literal ID="litDailyCount" runat="server" Text="0" />
                </div>
            </div>
            <div class="sch-stat-card">
                <div class="sch-stat-label">Weekly Schedules</div>
                <div class="sch-stat-value">
                    <asp:Literal ID="litWeeklyCount" runat="server" Text="0" />
                </div>
            </div>
            <div class="sch-stat-card">
                <div class="sch-stat-label">Monthly Schedules</div>
                <div class="sch-stat-value">
                    <asp:Literal ID="litMonthlyCount" runat="server" Text="0" />
                </div>
            </div>
            <div class="sch-stat-card">
                <div class="sch-stat-label">Total Active</div>
                <div class="sch-stat-value orange">
                    <asp:Literal ID="litActiveCount" runat="server" Text="0" />
                </div>
            </div>
        </div>

        <asp:Panel ID="pnlFilters" runat="server" CssClass="sch-filters">
            <div>
                <span class="sch-filter-label">Section</span>
                <asp:DropDownList ID="ddlFilterSection" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed">
                    <asp:ListItem Text="All Sections" Value="" />
                </asp:DropDownList>
            </div>
            <div>
                <span class="sch-filter-label">Status</span>
                <asp:DropDownList ID="ddlFilterStatus" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed">
                    <asp:ListItem Text="All Status" Value="" />
                    <asp:ListItem Text="Active" Value="Active" />
                    <asp:ListItem Text="Inactive" Value="Inactive" />
                    <asp:ListItem Text="Completed" Value="Completed" />
                </asp:DropDownList>
            </div>
            <div class="sch-filter-freq">
                <span class="sch-filter-label">Frequency</span>
                <asp:DropDownList ID="ddlFilterFrequency" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed">
                    <asp:ListItem Text="All Frequencies" Value="" />
                    <asp:ListItem Text="Daily" Value="Daily" />
                    <asp:ListItem Text="Weekly" Value="Weekly" />
                    <asp:ListItem Text="Monthly" Value="Monthly" />
                </asp:DropDownList>
            </div>
            <asp:LinkButton ID="btnClearFilters" runat="server" CssClass="sch-clear-btn" OnClick="btnClearFilters_Click">Clear</asp:LinkButton>
        </asp:Panel>

        <%-- ===================== TABLE VIEW (default) ===================== --%>
        <asp:Panel ID="pnlTableView" runat="server">
            <asp:Literal ID="litScheduleTable" runat="server" />
        </asp:Panel>

        <%-- ===================== CALENDAR VIEW ===================== --%>
        <asp:Panel ID="pnlCalendarSection" runat="server" Visible="false">

            <div class="sch-tabs">
                <asp:LinkButton ID="tabDaily" runat="server" CssClass="sch-tab" CommandArgument="Daily" OnClick="Tab_Click">Daily</asp:LinkButton>
                <asp:LinkButton ID="tabWeekly" runat="server" CssClass="sch-tab" CommandArgument="Weekly" OnClick="Tab_Click">Weekly</asp:LinkButton>
                <asp:LinkButton ID="tabMonthly" runat="server" CssClass="sch-tab" CommandArgument="Monthly" OnClick="Tab_Click">Monthly</asp:LinkButton>
            </div>

            <asp:Panel ID="pnlDailyView" runat="server" CssClass="sch-daily-wrap">
                <div class="sch-day-badge">
                    <div class="num">
                        <asp:Literal ID="litDailyDayNum" runat="server" />
                    </div>
                    <div class="name">
                        <asp:Literal ID="litDailyDayName" runat="server" />
                    </div>
                </div>
                <div class="sch-agenda">
                    <div class="sch-agenda-head">
                        <h2>Today</h2>
                        <p>
                            <asp:Literal ID="litDailySummary" runat="server" />
                        </p>
                    </div>
                    <div class="sch-timeline">
                        <asp:Literal ID="litDailyTimeline" runat="server" />
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlWeeklyView" runat="server" CssClass="sch-weekly-wrap" Visible="false">
                <asp:Literal ID="litWeeklyGrid" runat="server" />
            </asp:Panel>

            <asp:Panel ID="pnlMonthlyView" runat="server" CssClass="sch-monthly-wrap" Visible="false">
                <div class="sch-month-nav">
                    <asp:LinkButton ID="btnPrevMonth" runat="server" CssClass="sch-month-nav-btn" OnClick="btnPrevMonth_Click" CausesValidation="false">&#8249;</asp:LinkButton>
                    <div class="sch-month-title">
                        <asp:Literal ID="litMonthTitle" runat="server" />
                    </div>
                    <asp:LinkButton ID="btnNextMonth" runat="server" CssClass="sch-month-nav-btn" OnClick="btnNextMonth_Click" CausesValidation="false">&#8250;</asp:LinkButton>
                </div>
                <asp:Literal ID="litMonthlyGrid" runat="server" />
            </asp:Panel>

        </asp:Panel>

        <%-- Hidden proxies so raw HTML cards/chips/rows can trigger server-side Edit/Delete --%>
        <asp:HiddenField ID="hfActionScheduleID" runat="server" />
        <asp:LinkButton ID="lnkProxyEdit" runat="server" Style="display: none" OnClick="lnkProxyEdit_Click" CausesValidation="false">Edit</asp:LinkButton>
        <asp:LinkButton ID="lnkProxyDelete" runat="server" Style="display: none" OnClick="lnkProxyDelete_Click" CausesValidation="false">Delete</asp:LinkButton>

        <asp:Panel ID="pnlModalOverlay" runat="server" CssClass="sch-modal-overlay">
            <div class="sch-modal">
                <div class="sch-modal-head">
                    <h2 class="sch-modal-title">
                        <asp:Literal ID="litModalTitle" runat="server" Text="Add Cleaning Schedule" /></h2>
                    <asp:LinkButton ID="btnCloseX" runat="server" CssClass="sch-modal-close" OnClick="btnCancel_Click" CausesValidation="false">✕</asp:LinkButton>
                </div>

                <div class="sch-modal-body">
                    <asp:HiddenField ID="hfScheduleID" runat="server" Value="0" />

                    <p class="sch-section-label">Schedule Info</p>
                    <div class="sch-form-grid full">
                        <div class="sch-field">
                            <label>Schedule ID</label>
                            <asp:TextBox ID="txtScheduleCode" runat="server" ReadOnly="true" TabIndex="-1" CssClass="sch-locked-field" /> 
                        </div>
                    </div>
                    <div class="sch-form-grid">
                        <div class="sch-field">
                            <label>Section <span class="sch-req">*</span></label>
                            <asp:DropDownList ID="ddlSection" runat="server" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlSection" InitialValue=""
                                ErrorMessage="Section is required" ValidationGroup="CleaningSchedule" Display="Dynamic" ForeColor="#DC2626" Font-Size="12px" />
                        </div>
                        <div class="sch-field">
                            <label>Cleaning Type</label>
                            <asp:DropDownList ID="ddlCleaningType" runat="server">
                                <asp:ListItem Text=" Select Type " Value="" />
                                <asp:ListItem Text="General Cleaning" />
                                <asp:ListItem Text="Deep Cleaning" />
                                <asp:ListItem Text="Window Cleaning" />
                                <asp:ListItem Text="Restroom Cleaning" />
                                <asp:ListItem Text="Floor Care" />
                                <asp:ListItem Text="Other" />
                            </asp:DropDownList>
                        </div>
                        <div class="sch-field">
                            <label>Frequency <span class="sch-req">*</span></label>
                            <asp:DropDownList ID="ddlFrequency" runat="server">
                                <asp:ListItem Text="Daily" />
                                <asp:ListItem Text="Weekly" />
                                <asp:ListItem Text="Monthly" />
                            </asp:DropDownList>
                        </div>
                        <div class="sch-field">
                            <label>Status</label>
                            <asp:DropDownList ID="ddlStatus" runat="server">
                                <asp:ListItem Text="Active" />
                                <asp:ListItem Text="Inactive" />
                                <asp:ListItem Text="Completed" />
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="sch-form-grid full">
                        <div class="sch-field">
                            <label>Description</label>
                            <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="3" MaxLength="255" />
                        </div>
                    </div>

                    <p class="sch-section-label">Timing</p>
                    <div class="sch-form-grid">
                        <div class="sch-field">
                            <label>Start Date <span class="sch-req">*</span></label>
                            <asp:TextBox ID="txtStartDate" runat="server" TextMode="Date" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtStartDate"
                                ErrorMessage="Start date is required" ValidationGroup="CleaningSchedule" Display="Dynamic" ForeColor="#DC2626" Font-Size="12px" />
                        </div>
                        <div class="sch-field">
                            <label>End Date (optional)</label>
                            <asp:TextBox ID="txtEndDate" runat="server" TextMode="Date" />
                        </div>
                        <div class="sch-field">
                            <label>Repeat Time (optional)</label>
                            <asp:TextBox ID="txtRepeatTime" runat="server" TextMode="Time" />
                        </div>
                    </div>

                    <asp:Literal ID="litError" runat="server" />
                </div>

                <div class="sch-modal-foot">
                    <asp:LinkButton ID="btnCancel" runat="server" CssClass="sch-btn-cancel" OnClick="btnCancel_Click" CausesValidation="false">Cancel</asp:LinkButton>
                    <asp:LinkButton ID="btnSaveRecord" runat="server" CssClass="sch-btn-primary" OnClick="btnSaveRecord_Click" ValidationGroup="CleaningSchedule">Save Record</asp:LinkButton>
                </div>
            </div>
        </asp:Panel>

    </div>

</asp:Content>
