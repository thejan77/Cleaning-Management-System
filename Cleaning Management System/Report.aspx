<%@ Page Title="Reports" Language="C#" MasterPageFile="~/CmsMaster.master" AutoEventWireup="true" CodeFile="Report.aspx.cs" Inherits="Reports" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .cj-wrap {
            --black: #0B0D12;
            --orange: #E8622D;
            --orange-dark: #CC5222;
            --bg-light: #F4F4F5;
            --gray-text: #64748B;
            --border-color: #E2E8F0;
            font-family: 'Segoe UI', Arial, sans-serif;
        }

        .cj-title { font-size: 22px; font-weight: 700; color: var(--black); margin: 0; }
        .cj-subtitle { color: var(--gray-text); font-size: 14px; margin-top: 4px; margin-bottom: 22px; }

        .cj-filters {
            background: #fff; border: 1px solid var(--border-color); border-radius: 10px;
            padding: 18px 20px; display: grid;
            grid-template-columns: repeat(4,1fr) auto; gap: 16px; align-items: end; margin-bottom: 22px;
        }

        .cj-filter-label { display: block; text-transform: uppercase; font-size: 11px; font-weight: 600; color: var(--gray-text); margin-bottom: 6px; }

        .cj-filters select, .cj-filters input[type=text] {
            width: 100%; padding: 9px 10px; border: 1px solid var(--border-color); border-radius: 6px; font-size: 14px; background: #fff; box-sizing: border-box;
        }

        .cj-clear-btn {
            background: #fff; border: 1px solid var(--border-color); color: var(--black);
            padding: 9px 16px; border-radius: 6px; font-weight: 600; cursor: pointer; height: 40px;
        }

        .cj-tabs { display: flex; gap: 0; margin-bottom: 20px; border-bottom: 1px solid var(--border-color); }

        .cj-tab-btn {
            padding: 10px 4px; margin-right: 26px; font-size: 14px; font-weight: 700;
            color: var(--gray-text); background: none; border: none; border-bottom: 3px solid transparent;
            cursor: pointer; text-decoration: none !important;
        }

        .cj-tab-btn.active { color: var(--orange-dark); border-bottom-color: var(--orange); }

        .cj-section-title { display: flex; align-items: center; gap: 10px; margin: 0 0 14px; }
        .cj-section-title .cj-dot { width: 8px; height: 8px; border-radius: 50%; background: var(--orange); }
        .cj-section-title h3 { margin: 0; font-size: 16px; font-weight: 700; }

        .cj-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(260px, 1fr)); gap: 16px; margin-bottom: 8px; }

        .cj-card { background: #fff; border: 1px solid var(--border-color); border-radius: 12px; padding: 16px 18px; display: flex; flex-direction: column; gap: 14px; }
        .cj-card-title { font-size: 14px; font-weight: 700; margin: 0; color: var(--black); }

        .cj-card-footer { display: flex; gap: 8px; padding-top: 12px; border-top: 1px solid var(--border-color); }

        .cj-mini-btn {
            flex: 1; text-align: center; border: 1px solid var(--border-color); border-radius: 7px;
            padding: 8px 6px; font-size: 12px; font-weight: 700; cursor: pointer; background: #fff;
            color: var(--black); text-decoration: none !important;
        }
        .cj-mini-btn.pdf { color: var(--orange-dark); border-color: #F0C4AE; background: #FDF3EE; }
        .cj-mini-btn.xls { color: #15803D; border-color: #BBE8CC; background: #F0FBF4; }

        .cj-msg { margin-bottom: 16px; font-size: 13px; }
        .cj-msg.err { color: #DC2626; }
    </style>

    <div class="cj-wrap">

        <h1 class="cj-title">Reports</h1>
        <div class="cj-subtitle">Generate, preview and export operational and maintenance reports</div>

        <asp:Literal ID="litMessage" runat="server" />

        <div class="cj-filters">
            <div>
                <span class="cj-filter-label">Section</span>
                <asp:DropDownList ID="ddlFilterSection" runat="server" />
            </div>
            <div>
                <span class="cj-filter-label">Machine</span>
                <asp:DropDownList ID="ddlFilterMachine" runat="server" />
            </div>
            <div>
                <span class="cj-filter-label">From Date</span>
                <asp:TextBox ID="txtFromDate" runat="server" TextMode="Date" />
            </div>
            <div>
                <span class="cj-filter-label">To Date</span>
                <asp:TextBox ID="txtToDate" runat="server" TextMode="Date" />
            </div>
            <asp:LinkButton ID="btnClearFilters" runat="server" CssClass="cj-clear-btn" OnClick="btnClearFilters_Click" CausesValidation="false">Clear</asp:LinkButton>
        </div>

        <div class="cj-tabs">
            <asp:LinkButton ID="btnTabCleaning" runat="server" OnClick="btnTabCleaning_Click" CausesValidation="false">Cleaning Operations</asp:LinkButton>
            <asp:LinkButton ID="btnTabMachine" runat="server" OnClick="btnTabMachine_Click" CausesValidation="false">Machine &amp; Maintenance</asp:LinkButton>
        </div>

        <asp:Panel ID="pnlCleaningOps" runat="server">
            <div class="cj-section-title"><span class="cj-dot"></span><h3>Cleaning Operations</h3></div>
            <div class="cj-grid">

                <div class="cj-card">
                    <p class="cj-card-title">Daily Cleaning Report</p>
                    <div class="cj-card-footer">
                        <asp:LinkButton runat="server" CssClass="cj-mini-btn pdf" CommandName="DailyCleaning" CommandArgument="PDF" OnCommand="Report_Command" CausesValidation="false">PDF</asp:LinkButton>
                        <asp:LinkButton runat="server" CssClass="cj-mini-btn xls" CommandName="DailyCleaning" CommandArgument="EXCEL" OnCommand="Report_Command" CausesValidation="false">Excel</asp:LinkButton>
                    </div>
                </div>

                <div class="cj-card">
                    <p class="cj-card-title">Weekly Cleaning Report</p>
                    <div class="cj-card-footer">
                        <asp:LinkButton runat="server" CssClass="cj-mini-btn pdf" CommandName="WeeklyCleaning" CommandArgument="PDF" OnCommand="Report_Command" CausesValidation="false">PDF</asp:LinkButton>
                        <asp:LinkButton runat="server" CssClass="cj-mini-btn xls" CommandName="WeeklyCleaning" CommandArgument="EXCEL" OnCommand="Report_Command" CausesValidation="false">Excel</asp:LinkButton>
                    </div>
                </div>

                <div class="cj-card">
                    <p class="cj-card-title">Monthly Cleaning Report</p>
                    <div class="cj-card-footer">
                        <asp:LinkButton runat="server" CssClass="cj-mini-btn pdf" CommandName="MonthlyCleaning" CommandArgument="PDF" OnCommand="Report_Command" CausesValidation="false">PDF</asp:LinkButton>
                        <asp:LinkButton runat="server" CssClass="cj-mini-btn xls" CommandName="MonthlyCleaning" CommandArgument="EXCEL" OnCommand="Report_Command" CausesValidation="false">Excel</asp:LinkButton>
                    </div>
                </div>

                <div class="cj-card">
                    <p class="cj-card-title">Additional Work Request Report</p>
                    <div class="cj-card-footer">
                        <asp:LinkButton runat="server" CssClass="cj-mini-btn pdf" CommandName="AdditionalWorkRequest" CommandArgument="PDF" OnCommand="Report_Command" CausesValidation="false">PDF</asp:LinkButton>
                        <asp:LinkButton runat="server" CssClass="cj-mini-btn xls" CommandName="AdditionalWorkRequest" CommandArgument="EXCEL" OnCommand="Report_Command" CausesValidation="false">Excel</asp:LinkButton>
                    </div>
                </div>

            </div>
        </asp:Panel>

        <asp:Panel ID="pnlMachineMaint" runat="server">
            <div class="cj-section-title"><span class="cj-dot"></span><h3>Machine &amp; Maintenance</h3></div>
            <div class="cj-grid">

                <div class="cj-card">
                    <p class="cj-card-title">Machine Maintenance History</p>
                    <div class="cj-card-footer">
                        <asp:LinkButton runat="server" CssClass="cj-mini-btn pdf" CommandName="MachineMaintenanceHistory" CommandArgument="PDF" OnCommand="Report_Command" CausesValidation="false">PDF</asp:LinkButton>
                        <asp:LinkButton runat="server" CssClass="cj-mini-btn xls" CommandName="MachineMaintenanceHistory" CommandArgument="EXCEL" OnCommand="Report_Command" CausesValidation="false">Excel</asp:LinkButton>
                    </div>
                </div>

                <div class="cj-card">
                    <p class="cj-card-title">Breakdown History</p>
                    <div class="cj-card-footer">
                        <asp:LinkButton runat="server" CssClass="cj-mini-btn pdf" CommandName="BreakdownHistory" CommandArgument="PDF" OnCommand="Report_Command" CausesValidation="false">PDF</asp:LinkButton>
                        <asp:LinkButton runat="server" CssClass="cj-mini-btn xls" CommandName="BreakdownHistory" CommandArgument="EXCEL" OnCommand="Report_Command" CausesValidation="false">Excel</asp:LinkButton>
                    </div>
                </div>

                <div class="cj-card">
                    <p class="cj-card-title">Repair Details</p>
                    <div class="cj-card-footer">
                        <asp:LinkButton runat="server" CssClass="cj-mini-btn pdf" CommandName="RepairDetails" CommandArgument="PDF" OnCommand="Report_Command" CausesValidation="false">PDF</asp:LinkButton>
                        <asp:LinkButton runat="server" CssClass="cj-mini-btn xls" CommandName="RepairDetails" CommandArgument="EXCEL" OnCommand="Report_Command" CausesValidation="false">Excel</asp:LinkButton>
                    </div>
                </div>

            </div>
        </asp:Panel>

    </div>

</asp:Content>