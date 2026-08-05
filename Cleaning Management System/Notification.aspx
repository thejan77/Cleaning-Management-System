<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Notification.aspx.cs" Inherits="CleaningManagement_Notification" MasterPageFile="~/CmsMaster.master" %>

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

        .ntf-page-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 18px;
            flex-wrap: wrap;
            gap: 12px;
        }

        .ntf-page-title {
            font-size: 22px;
            font-weight: 700;
            color: var(--black);
            margin: 0;
        }

        .ntf-page-subtitle {
            font-size: 13.5px;
            color: var(--gray-text);
            margin-top: 4px;
            max-width: 640px;
        }

        
        .ntf-stats {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 14px;
            margin-bottom: 20px;
        }

        .ntf-stat-card {
            background: #fff;
            border-radius: 10px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.08);
            padding: 14px 16px;
        }

        .ntf-stat-label {
            font-size: 11px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.4px;
            color: var(--gray-text);
        }

        .ntf-stat-value {
            font-size: 24px;
            font-weight: 700;
            color: var(--black);
            margin-top: 6px;
        }

        .ntf-filter-bar {
            background: #fff;
            border-radius: 10px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.08);
            padding: 18px 20px;
            margin-bottom: 20px;
        }

        .ntf-filter-row {
            display: grid;
            grid-template-columns: repeat(5, 1fr);
            gap: 16px;
            margin-bottom: 16px;
        }

        .ntf-filter-group {
            display: flex;
            flex-direction: column;
            min-width: 0;
        }

            .ntf-filter-group label {
                font-size: 11.5px;
                font-weight: 700;
                text-transform: uppercase;
                letter-spacing: 0.4px;
                color: var(--gray-text);
                margin-bottom: 6px;
            }

        .ntf-select, .ntf-input {
            border: 1px solid var(--border-color);
            border-radius: 8px;
            padding: 10px 12px;
            font-size: 14px;
            color: var(--black);
            width: 100%;
            box-sizing: border-box;
        }

        .ntf-checkbox-row {
            margin-bottom: 16px;
        }

        .ntf-checkbox-group {
            display: flex;
            align-items: center;
            gap: 8px;
        }

            .ntf-checkbox-group label {
                font-size: 13.5px;
                font-weight: 600;
                color: var(--black);
                cursor: pointer;
            }

        .ntf-button-row {
            display: flex;
            gap: 10px;
        }

        .ntf-btn-apply {
            background: var(--orange);
            border: 1px solid var(--orange);
            color: #fff;
            font-weight: 700;
            padding: 10px 24px;
            border-radius: 8px;
            cursor: pointer;
            height: 40px;
            white-space: nowrap;
        }

            .ntf-btn-apply:hover {
                background: var(--orange-dark);
            }

        .ntf-btn-clear {
            background: #fff;
            border: 1px solid var(--border-color);
            color: var(--black);
            font-weight: 600;
            padding: 10px 20px;
            border-radius: 8px;
            cursor: pointer;
            height: 40px;
            white-space: nowrap;
        }

            .ntf-btn-clear:hover {
                background: var(--bg-light);
            }

        .ntf-table-wrap {
            background: #fff;
            border-radius: 10px;
            overflow: hidden;
            box-shadow: 0 1px 3px rgba(0,0,0,0.08);
        }

        table.ntf-grid {
            width: 100%;
            border-collapse: collapse;
            font-size: 13.5px;
        }

            table.ntf-grid th {
                background: var(--black);
                color: #fff;
                text-transform: uppercase;
                font-size: 11.5px;
                letter-spacing: 0.4px;
                padding: 12px 14px;
                text-align: left;
                white-space: nowrap;
            }

            table.ntf-grid td {
                padding: 12px 14px;
                border-bottom: 1px solid var(--border-color);
                color: var(--black);
                vertical-align: middle;
            }

            table.ntf-grid tr:last-child td {
                border-bottom: none;
            }

            table.ntf-grid tr:hover td {
                background: var(--bg-light);
            }

        .ntf-id {
            font-weight: 600;
            color: var(--gray-text);
        }

        .ntf-entity-tag {
            display: inline-block;
            font-size: 11.5px;
            font-weight: 700;
            background: var(--bg-light);
            border: 1px solid var(--border-color);
            padding: 2px 8px;
            border-radius: 6px;
            white-space: nowrap;
        }

        .ntf-message-cell {
            max-width: 300px;
            color: var(--gray-text);
            line-height: 1.4;
        }

       
        .ntf-channel {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            font-size: 12px;
            font-weight: 700;
            padding: 4px 11px;
            border-radius: 999px;
        }

        .channel-Email {
            background: #E8F1FF;
            color: #2563EB;
        }

        .channel-SMS {
            background: #FFF1E6;
            color: #C2410C;
        }

     
        .ntf-badge {
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

        .status-Sent {
            background: #16A34A;
        }

        .status-Failed {
            background: #DC2626;
        }

        .ntf-table-footer {
            padding: 12px 18px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            font-size: 12.5px;
            color: var(--gray-text);
            border-top: 1px solid var(--border-color);
        }
    </style>

    <div class="ntf-page-header">
        <div>
            <div class="ntf-page-title">Alerts &amp; Notifications</div>
            <div class="ntf-page-subtitle">Automatic email and SMS alerts sent for new or unresolved complaints and feedback</div>
        </div>
    </div>

    <div class="ntf-stats">
        <div class="ntf-stat-card">
            <div class="ntf-stat-label">Total Alerts</div>
            <div class="ntf-stat-value"><asp:Literal ID="litTotal" runat="server" Text="0" /></div>
        </div>
        <div class="ntf-stat-card">
            <div class="ntf-stat-label">Pending</div>
            <div class="ntf-stat-value"><asp:Literal ID="litPending" runat="server" Text="0" /></div>
        </div>
        <div class="ntf-stat-card">
            <div class="ntf-stat-label">Sent</div>
            <div class="ntf-stat-value"><asp:Literal ID="litSent" runat="server" Text="0" /></div>
        </div>
        <div class="ntf-stat-card">
            <div class="ntf-stat-label">Failed</div>
            <div class="ntf-stat-value"><asp:Literal ID="litFailed" runat="server" Text="0" /></div>
        </div>
    </div>

    <div class="ntf-filter-bar">
        <div class="ntf-filter-row">
            <div class="ntf-filter-group">
                <label>Channel</label>
                <asp:DropDownList ID="ddlChannel" runat="server" CssClass="ntf-select">
                    <asp:ListItem Text="All Channels" Value="" />
                    <asp:ListItem Text="Email" Value="Email" />
                    <asp:ListItem Text="SMS" Value="SMS" />
                </asp:DropDownList>
            </div>

            <div class="ntf-filter-group">
                <label>Status</label>
                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="ntf-select">
                    <asp:ListItem Text="All Status" Value="" />
                    <asp:ListItem Text="Pending" Value="Pending" />
                    <asp:ListItem Text="Sent" Value="Sent" />
                    <asp:ListItem Text="Failed" Value="Failed" />
                </asp:DropDownList>
            </div>

            <div class="ntf-filter-group">
                <label>Recipient Type</label>
                <asp:DropDownList ID="ddlRecipientType" runat="server" CssClass="ntf-select">
                    <asp:ListItem Text="All Recipients" Value="" />
                    <asp:ListItem Text="Staff" Value="Staff" />
                    <asp:ListItem Text="Client" Value="Client" />
                    <asp:ListItem Text="Contractor" Value="Contractor" />
                </asp:DropDownList>
            </div>

            <div class="ntf-filter-group">
                <label>Sent From</label>
                <asp:TextBox ID="txtFrom" runat="server" TextMode="Date" CssClass="ntf-input" />
            </div>

            <div class="ntf-filter-group">
                <label>Sent To</label>
                <asp:TextBox ID="txtTo" runat="server" TextMode="Date" CssClass="ntf-input" />
            </div>
        </div>

        <div class="ntf-checkbox-row">
            <div class="ntf-checkbox-group">
                <asp:CheckBox ID="chkComplaintsOnly" runat="server" />
                <label for="<%= chkComplaintsOnly.ClientID %>">Complaint alerts only</label>
            </div>
        </div>

        <div class="ntf-button-row">
            <asp:Button ID="btnApply" runat="server" CssClass="ntf-btn-apply" Text="Filter"
                OnClick="btnApply_Click" CausesValidation="false" />

            <asp:Button ID="btnReset" runat="server" CssClass="ntf-btn-clear" Text="Clear"
                OnClick="btnReset_Click" CausesValidation="false" />
        </div>
    </div>

    <div class="ntf-table-wrap">
        <asp:GridView ID="gvNotifications" runat="server" CssClass="ntf-grid" AutoGenerateColumns="false"
            GridLines="None" DataKeyNames="NotificationID">
            <Columns>
                <asp:TemplateField HeaderText="ID">
                    <ItemTemplate>
                        <span class="ntf-id">#<%# Eval("NotificationID") %></span>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Channel">
                    <ItemTemplate>
                        <span class='ntf-channel channel-<%# Eval("Channel") %>'><%# Eval("Channel") %></span>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="RecipientType" HeaderText="Recipient Type" />
                <asp:BoundField DataField="RecipientName" HeaderText="Recipient" />

                <asp:TemplateField HeaderText="Related To">
                    <ItemTemplate>
                        <span class="ntf-entity-tag"><%# Eval("RelatedEntityType") %>-<%# Eval("RelatedEntityID") %></span>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Message">
                    <ItemTemplate>
                        <div class="ntf-message-cell"><%# Eval("Message") %></div>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="SentDate" HeaderText="Sent" DataFormatString="{0:dd MMM yyyy HH:mm}" HtmlEncode="false" />

                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='ntf-badge status-<%# Eval("Status") %>'><%# Eval("Status") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <div style="padding: 20px; color: #64748B;">No alerts match these filters.</div>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>

    <div class="ntf-table-footer">
        <asp:Label ID="lblResultCount" runat="server" Text="Showing 0 records" />
       
    </div>

</asp:Content>

