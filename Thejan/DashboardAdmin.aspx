<%@ Page Title="" Language="C#" MasterPageFile="~/CmsMaster.master" AutoEventWireup="true"
    CodeFile="DashboardAdmin.aspx.cs" Inherits="DashboardAdmin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Admin Dashboard</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitleContent" runat="server">Dashboard</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">


    <meta http-equiv="refresh" content="30" />

    <style type="text/css">
        .cms-greeting{ margin-bottom:20px; }
        .cms-greeting h2{ font-size:22px; font-weight:700; color:#1A1A1A; margin:0 0 4px 0; }
        .cms-greeting p{ font-size:13.5px; color:#6B7280; margin:0; }
    </style>
    <div class="cms-greeting">
        <h2>Welcome, <asp:Literal ID="litUserName" runat="server" Text="Admin" />!</h2>
        <p>Here's what's happening with your cleaning operations today.</p>
   
    </div>


    <style type="text/css">
        .cms-kpi-grid{
            display:grid;
            grid-template-columns:repeat(auto-fit, minmax(200px, 1fr));
            gap:14px;
            margin-bottom:22px;
        }
        .cms-kpi-card{
            background:#FFFFFF;
            border:1px solid #E5E7EB;
            border-radius:10px;
            padding:12px 16px;
        }
        .cms-kpi-top{
            display:flex;
            align-items:center;
            gap:8px;
            margin-bottom:8px;
        }
        .cms-kpi-icon{
            width:26px;
            height:26px;
            border-radius:7px;
            display:flex;
            align-items:center;
            justify-content:center;
            color:#FFFFFF;
            font-size:12px;
            flex-shrink:0;
        }
        .cms-kpi-label{
            font-size:12.5px;
            font-weight:600;
            color:#6B7280;
        }
        .cms-kpi-value{
            font-size:22px;
            font-weight:700;
            color:#1A1A1A;
            line-height:1;
            margin-bottom:3px;
        }
        .cms-kpi-sub{
            font-size:11.5px;
            color:#9CA3AF;
        }
    </style>

    <!--Cards-->
    <div class="cms-kpi-grid">
        <div class="cms-kpi-card">
            <div class="cms-kpi-top">
                <div class="cms-kpi-icon" style="background:#E8622D;"><i class="fa-solid fa-clipboard-list"></i></div>
                <div class="cms-kpi-label">Total Jobs</div>
            </div>
            <div class="cms-kpi-value"><asp:Literal ID="litTotalJobs" runat="server" Text="0" /></div>
            <div class="cms-kpi-sub">Today's jobs</div>
        </div>
        <div class="cms-kpi-card">
            <div class="cms-kpi-top">
                <div class="cms-kpi-icon" style="background:#B7791F;"><i class="fa-regular fa-clock"></i></div>
                <div class="cms-kpi-label">Pending Jobs</div>
            </div>
            <div class="cms-kpi-value"><asp:Literal ID="litPendingJobs" runat="server" Text="0" /></div>
            <div class="cms-kpi-sub">Awaiting completion today</div>
        </div>
        <div class="cms-kpi-card">
            <div class="cms-kpi-top">
                <div class="cms-kpi-icon" style="background:#1F9D55;"><i class="fa-solid fa-check"></i></div>
                <div class="cms-kpi-label">Completed Jobs</div>
            </div>
            <div class="cms-kpi-value"><asp:Literal ID="litCompletedToday" runat="server" Text="0" /></div>
            <div class="cms-kpi-sub">Finished today</div>
        </div>
        <div class="cms-kpi-card">
            <div class="cms-kpi-top">
                <div class="cms-kpi-icon" style="background:#D64545;"><i class="fa-solid fa-triangle-exclamation"></i></div>
                <div class="cms-kpi-label">Machine Breakdowns</div>
            </div>
            <div class="cms-kpi-value"><asp:Literal ID="litBreakdowns" runat="server" Text="0" /></div>
            <div class="cms-kpi-sub">Open today</div>
        </div>
        <div class="cms-kpi-card">
            <div class="cms-kpi-top">
                <div class="cms-kpi-icon" style="background:#1A1A1A;"><i class="fa-solid fa-comment-dots"></i></div>
                <div class="cms-kpi-label">Feedbacks</div>
            </div>
            <div class="cms-kpi-value"><asp:Literal ID="litFeedbacks" runat="server" Text="0" /></div>
            <div class="cms-kpi-sub">New today</div>
        </div>
    </div>

    <!-- charts -->
    <div class="cms-grid-1" style="margin-bottom: 18px;">
        <div class="cms-card">
            <h3>Jobs Overview (This Week)</h3>
            <canvas id="chartJobsOverview" height="90"></canvas>
        </div>
    </div>

    <!-- Tables -->
    <div class="cms-grid-2">
        <div class="cms-card">
            <h3>Schedule Overview (Today)</h3>
            <asp:GridView ID="gvSchedule" runat="server" CssClass="cms-table" AutoGenerateColumns="false"
                GridLines="None" EmptyDataText="No jobs scheduled for today.">
                <Columns>
                    <asp:BoundField DataField="ScheduledTime" HeaderText="Time" />
                    <asp:BoundField DataField="SectionName" HeaderText="Section" />
                    <asp:BoundField DataField="LocationName" HeaderText="Location" />
                    <asp:BoundField DataField="AssignedTo" HeaderText="Team / Staff" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class='cms-status-pill <%# GetStatusCssClass(Eval("Status").ToString()) %>'>
                                <%# Eval("Status") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <div style="text-align:center; margin-top: 16px;">
                <a href="~/Apps/CleaningSchedule.aspx" runat="server" class="cms-btn-link">View Full Schedule</a>
            </div>
        </div>

        <div class="cms-card">
            <h3>Recent Activity (Audit Log)</h3>
            <asp:Repeater ID="rptActivity" runat="server">
                <ItemTemplate>
                    <div style="display:flex; gap:12px; padding: 10px 0; border-bottom: 1px solid #F3F4F6;">
                        <div style="flex-shrink:0; width:34px; height:34px; border-radius:50%; background:#FDEDE5; color:#CC5222; display:flex; align-items:center; justify-content:center;">
                            <i class="fa-solid fa-circle-info"></i>
                        </div>
                        <div style="flex:1;">
                            <div style="font-size:13px; font-weight:700; color:#1A1A1A;">
                                <%# Eval("ActionType") %> - <%# Eval("TargetEntityType") %> #<%# Eval("TargetEntityID") %></div>
                            <div style="font-size:12px; color:#6B7280;"><%# Eval("Details") %></div>
                        </div>
                        <div style="font-size:12px; color:#6B7280; white-space:nowrap;">
                            <%# Eval("Timestamp", "{0:hh:mm tt}") %>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <div style="text-align:center; margin-top: 16px;">
                <a href="~/Apps/Notifications.aspx" runat="server" class="cms-btn-link">View All Activities</a>
            </div>
        </div>
    </div>

    <!-- Chart Script -->
    <script>
        document.addEventListener('DOMContentLoaded', function () {


            var jobsLabels = <%= JobsOverviewLabelsJson %>;

            new Chart(document.getElementById('chartJobsOverview'), {
                type: 'line',
                data: {
                    labels: jobsLabels,
                    datasets: [
                        { label: 'Total', data: <%= TotalJobsSeriesJson %>, borderColor: '#E8622D', backgroundColor: 'rgba(232,98,45,0.08)', tension: 0.35 },
                        { label: 'Completed', data: <%= CompletedJobsSeriesJson %>, borderColor: '#1F9D55', backgroundColor: 'rgba(31,157,85,0.08)', tension: 0.35 },
                        { label: 'Pending', data: <%= PendingJobsSeriesJson %>, borderColor: '#B7791F', backgroundColor: 'rgba(183,121,31,0.08)', tension: 0.35 }
                    ]
                },
                options: { plugins: { legend: { position: 'bottom' } }, responsive: true }
            });
        });
    </script>

</asp:Content>
