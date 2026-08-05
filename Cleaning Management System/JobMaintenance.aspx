<%@ Page Language="C#" AutoEventWireup="true" CodeFile="JobMaintenance.aspx.cs" Inherits="CleaningManagement_JobMaintenance" MasterPageFile="~/CmsMaster.master" %>

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

        .jmr-page-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 18px;
            flex-wrap: wrap;
            gap: 12px;
        }

        .jmr-page-title {
            font-size: 22px;
            font-weight: 700;
            color: var(--black);
            margin: 0;
        }

        .jmr-page-subtitle {
            font-size: 13.5px;
            color: var(--gray-text);
            margin-top: 4px;
            max-width: 640px;
        }

        .jmr-filter-bar {
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

        .jmr-filter-group {
            display: flex;
            flex-direction: column;
            min-width: 170px;
        }

            .jmr-filter-group label {
                font-size: 11.5px;
                font-weight: 700;
                text-transform: uppercase;
                letter-spacing: 0.4px;
                color: var(--gray-text);
                margin-bottom: 6px;
            }

        .jmr-select, .jmr-input {
            border: 1px solid var(--border-color);
            border-radius: 8px;
            padding: 10px 12px;
            font-size: 14px;
            color: var(--black);
            width: 100%;
            box-sizing: border-box;
        }

        .jmr-btn-apply {
            background: var(--orange);
            border: 1px solid var(--orange);
            color: #fff;
            font-weight: 700;
            padding: 10px 20px;
            border-radius: 8px;
            cursor: pointer;
            height: 40px;
            white-space: nowrap;
        }

            .jmr-btn-apply:hover {
                background: var(--orange-dark);
            }

        .jmr-btn-clear {
            background: #fff;
            border: 1px solid var(--border-color);
            color: var(--black);
            font-weight: 600;
            padding: 10px 18px;
            border-radius: 8px;
            cursor: pointer;
            height: 40px;
            white-space: nowrap;
        }

            .jmr-btn-clear:hover {
                background: var(--bg-light);
            }

        .jmr-table-wrap {
            background: #fff;
            border-radius: 10px;
            overflow: hidden;
            box-shadow: 0 1px 3px rgba(0,0,0,0.08);
        }

        table.jmr-grid {
            width: 100%;
            border-collapse: collapse;
            font-size: 13.5px;
        }

            table.jmr-grid th {
                background: var(--black);
                color: #fff;
                text-transform: uppercase;
                font-size: 11.5px;
                letter-spacing: 0.4px;
                padding: 12px 14px;
                text-align: left;
                white-space: nowrap;
            }

            table.jmr-grid td {
                padding: 12px 14px;
                border-bottom: 1px solid var(--border-color);
                color: var(--black);
                vertical-align: middle;
            }

            table.jmr-grid tr:last-child td {
                border-bottom: none;
            }

            table.jmr-grid tr:hover td {
                background: var(--bg-light);
            }

        .jmr-rec-id {
            font-weight: 600;
            color: var(--gray-text);
        }

        .jmr-job-tag {
            display: inline-block;
            font-size: 11.5px;
            font-weight: 700;
            background: var(--bg-light);
            border: 1px solid var(--border-color);
            padding: 2px 8px;
            border-radius: 6px;
        }

        .jmr-details-cell {
            max-width: 280px;
            color: var(--gray-text);
            line-height: 1.4;
        }

        .jmr-badge {
            display: inline-block;
            padding: 4px 12px;
            border-radius: 999px;
            font-size: 12px;
            font-weight: 600;
            color: #fff;
            white-space: nowrap;
        }

        .status-Pending {
            background: #F0A93A;
        }

        .status-InProgress {
            background: #3B82F6;
        }

        .status-Completed {
            background: #16A34A;
        }

        .jmr-table-footer {
            padding: 12px 18px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            font-size: 12.5px;
            color: var(--gray-text);
            border-top: 1px solid var(--border-color);
        }
    </style>

    <div class="jmr-page-header">
        <div>
            <div class="jmr-page-title">Job Maintenance Records</div>
            <div class="jmr-page-subtitle">Work details and maintenance logs for every completed and ongoing cleaning activity</div>
        </div>
    </div>

    <div class="jmr-filter-bar">
        <div class="jmr-filter-group">
            <label>Team</label>
            <asp:DropDownList ID="ddlTeam" runat="server" CssClass="jmr-select">
                <asp:ListItem Text="All teams" Value="" />
            </asp:DropDownList>
        </div>

        <div class="jmr-filter-group">
            <label>Supervisor</label>
            <asp:DropDownList ID="ddlSupervisor" runat="server" CssClass="jmr-select">
                <asp:ListItem Text="All supervisors" Value="" />
            </asp:DropDownList>
        </div>

        <div class="jmr-filter-group">
            <label>Status</label>
            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="jmr-select">
                <asp:ListItem Text="All statuses" Value="" />
                <asp:ListItem Text="Pending" Value="Pending" />
                <asp:ListItem Text="In Progress" Value="InProgress" />
                <asp:ListItem Text="Completed" Value="Completed" />
            </asp:DropDownList>
        </div>

        <div class="jmr-filter-group">
            <label>Work Date From</label>
            <asp:TextBox ID="txtFrom" runat="server" TextMode="Date" CssClass="jmr-input" />
        </div>

        <div class="jmr-filter-group">
            <label>Work Date To</label>
            <asp:TextBox ID="txtTo" runat="server" TextMode="Date" CssClass="jmr-input" />
        </div>

        <asp:Button ID="btnApply" runat="server" CssClass="jmr-btn-apply" Text="Filter"
            OnClick="btnApply_Click" CausesValidation="false" />

        <asp:Button ID="btnReset" runat="server" CssClass="jmr-btn-clear" Text="Clear"
            OnClick="btnReset_Click" CausesValidation="false" />
    </div>

    <div class="jmr-table-wrap">
        <asp:GridView ID="gvJobMaintenance" runat="server" CssClass="jmr-grid" AutoGenerateColumns="false"
            GridLines="None" DataKeyNames="RecordID" OnRowDataBound="gvJobMaintenance_RowDataBound">
            <Columns>
                <asp:TemplateField HeaderText="Record">
                    <ItemTemplate>
                        <span class="jmr-rec-id">#<%# Eval("RecordID") %></span>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Job">
                    <ItemTemplate>
                        <span class="jmr-job-tag">JOB-<%# Eval("JobID") %></span>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="WorkDate" HeaderText="Date" DataFormatString="{0:dd MMM yyyy}" HtmlEncode="false" />
                <asp:BoundField DataField="WorkTime" HeaderText="Time" />
                <asp:BoundField DataField="TeamName" HeaderText="Assigned Team" />
                <asp:BoundField DataField="SupervisorName" HeaderText="Supervisor" />
                <asp:BoundField DataField="PeopleInvolved" HeaderText="People" />

                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='jmr-badge status-<%# Eval("Status") %>'><%# Eval("Status") %></span>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Work Details">
                    <ItemTemplate>
                        <div class="jmr-details-cell"><%# Eval("WorkDetails") %></div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <div style="padding: 20px; color: #64748B;">No maintenance records match these filters.</div>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>

    <div class="jmr-table-footer">
        <asp:Label ID="lblResultCount" runat="server" Text="Showing 0 records" />
    
    </div>

</asp:Content>
