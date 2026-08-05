<%@ Page Title="Supervisor Dashboard" Language="C#" MasterPageFile="~/CmsMaster.master"
    AutoEventWireup="true" CodeFile="DashboardSupervisor.aspx.cs" Inherits="DashboardSupervisor" %>

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
            margin-bottom: 4px;
            color: var(--black);
        }

        .welcome-sub {
            font-size: 14px;
            color: var(--gray-text);
            margin-bottom: 28px;
        }

        .stats-row {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
            gap: 20px;
            margin-bottom: 30px;
        }

        .stat-card {
            border: 1px solid var(--border-color);
            border-radius: 12px;
            background-color: #ffffff;
            padding: 22px 24px;
            box-shadow: 0 2px 8px rgba(11,13,18,0.05);
            display: flex;
            align-items: center;
            gap: 16px;
        }

        .stat-icon-box {
            width: 46px;
            height: 46px;
            border-radius: 10px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 20px;
            font-weight: 700;
            color: #ffffff;
            flex-shrink: 0;
        }

        .icon-members { background-color: var(--orange); }
        .icon-areas   { background-color: var(--black); }

        .stat-value {
            font-size: 26px;
            font-weight: 700;
            color: var(--black);
            line-height: 1;
        }

        .stat-label {
            font-size: 12.5px;
            color: var(--gray-text);
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.3px;
            margin-top: 4px;
        }

        .team-info-card {
            border: 1px solid var(--border-color);
            border-radius: 12px;
            background-color: #ffffff;
            padding: 24px 26px;
            box-shadow: 0 2px 8px rgba(11,13,18,0.05);
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 16px;
        }

        .team-info-title {
            font-size: 12.5px;
            color: var(--gray-text);
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.3px;
            margin-bottom: 4px;
        }

        .team-info-name {
            font-size: 19px;
            font-weight: 700;
            color: var(--black);
        }

        .btn-go-team {
            background-color: var(--orange);
            border: none;
            color: #fff;
            padding: 11px 22px;
            border-radius: 6px;
            font-weight: 600;
            font-size: 13.5px;
            text-decoration: none;
            display: inline-block;
            transition: background-color 0.18s ease-in-out;
        }

        .btn-go-team:hover {
            background-color: var(--orange-dark);
            color: #fff;
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
        Welcome, <asp:Label ID="lblSupervisorName" runat="server" ClientIDMode="Static" />
    </div>
    <div class="welcome-sub">Here's a quick look at your team today.</div>

    <asp:Panel ID="pnlNoTeam" runat="server" CssClass="no-team-msg" Visible="false">
        Your account isn't linked to a team yet. Contact an administrator.
    </asp:Panel>

    <asp:Panel ID="pnlDashboard" runat="server">

        <div class="stats-row">

            <div class="stat-card">
                <div class="stat-icon-box icon-members">&#128101;</div>
                <div>
                    <div class="stat-value">
                        <asp:Label ID="lblMemberCount" runat="server" ClientIDMode="Static" />
                    </div>
                    <div class="stat-label">Team Members</div>
                </div>
            </div>

            <div class="stat-card">
                <div class="stat-icon-box icon-areas">&#128204;</div>
                <div>
                    <div class="stat-value">
                        <asp:Label ID="lblAreaCount" runat="server" ClientIDMode="Static" />
                    </div>
                    <div class="stat-label">Assigned Areas</div>
                </div>
            </div>

        </div>

        <div class="team-info-card">
            <div>
                <div class="team-info-title">Your Team</div>
                <div class="team-info-name">
                    <asp:Label ID="lblTeamName" runat="server" ClientIDMode="Static" />
                </div>
                <div class="team-info-title" style="margin-top:10px;">Supervisor</div>
                <div class="team-info-name" style="font-size:15px;">
                    <asp:Label ID="lblSupervisorAssigned" runat="server" ClientIDMode="Static" />
                </div>
            </div>
            <a class="btn-go-team" href="Team.aspx" runat="server">Go to My Team &rarr;</a>
        </div>

    </asp:Panel>

</asp:Content>