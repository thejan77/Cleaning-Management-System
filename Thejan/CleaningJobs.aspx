<%@ Page Title="Cleaning Jobs" Language="C#" MasterPageFile="~/CmsMaster.master"
    AutoEventWireup="true" EnableEventValidation="false"
    CodeFile="CleaningJobs.aspx.cs" Inherits="CleaningManagement_Apps_CleaningJobs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        :root {
            --black: #0B0D12;
            --black-dark: #000000;
            --orange: #E8622D;
            --orange-dark: #CC5222;
            --bg-light: #F4F4F5;
            --gray-text: #64748B;
            --border-color: #E2E8F0;
            --sidebar-width: 250px;
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
            width: auto;
            transition: all 0.18s ease-in-out;
            margin-bottom: 20px;
        }

        .add-job-box:hover {
            background-color: var(--orange);
            color: #ffffff;
            border-color: var(--orange);
        }

        .add-job-box .plus-icon {
            font-size: 15px;
            font-weight: bold;
            line-height: 1;
        }

        #pnlJobFormWrapper {
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

        .btn-save:hover {
            background-color: var(--orange-dark);
        }

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

        .job-grid tr:last-child td {
            border-bottom: none;
        }

        .job-grid tr:hover td {
            background-color: var(--bg-light);
        }

        .status-pill {
            padding: 3px 11px;
            border-radius: 12px;
            font-size: 11.5px;
            font-weight: 600;
            color: #fff;
            white-space: nowrap;
        }

        .status-pending { background-color: var(--gray-text); }
        .status-inprogress { background-color: var(--orange); }
        .status-completed { background-color: #2E7D32; }
        .status-cancelled { background-color: #94A3B8; }

        .edit-link {
            color: var(--orange);
            font-weight: 600;
            font-size: 13px;
            text-decoration: none;
        }

        .edit-link:hover {
            color: var(--orange-dark);
            text-decoration: underline;
        }

        .btn-update {
    background-color: #DC3545;
    border: none;
    color: #fff;
    padding: 10px 24px;
    border-radius: 6px;
    font-weight: 600;
    font-size: 13.5px;
    cursor: pointer;
}

.btn-update:hover {
    background-color: #B02A37;
}
    </style>

    <div class="cms-page-title">Cleaning Jobs</div>

    <asp:HiddenField ID="hdnJobID" runat="server" Value="0" ClientIDMode="Static" />

 
    <div id="addJobBox" class="add-job-box" onclick="toggleJobForm(true);">
        <span class="plus-icon">+</span>
        <span>Add New Cleaning Job</span>
    </div>

   
    <div id="pnlJobFormWrapper">

        <div class="form-heading-row">
            <asp:Label ID="lblFormHeading" runat="server" Text="Create New Job" Font-Bold="true" Font-Size="16px" ForeColor="#0B0D12" ClientIDMode="Static" />
            <asp:Label ID="lblMessage" runat="server" ClientIDMode="Static" />
        </div>

        <div class="form-grid">

            <div class="form-row">
                <label for="ddlSection">Section *</label>
                <asp:DropDownList ID="ddlSection" runat="server" CssClass="form-control" ClientIDMode="Static" />
            </div>

            <div class="form-row">
                <label for="ddlJobType">Job Type *</label>
                <asp:DropDownList ID="ddlJobType" runat="server" CssClass="form-control" ClientIDMode="Static">
                    <asp:ListItem Text="Cleaning" Value="Cleaning" />
                    <asp:ListItem Text="Pest Control" Value="Pest Control" />
                    <asp:ListItem Text="Maintenance" Value="Maintenance" />
                </asp:DropDownList>
            </div>

            <div class="form-row">
                <label for="ddlAssignmentType">Assignment Type *</label>
                <asp:DropDownList ID="ddlAssignmentType" runat="server" CssClass="form-control" ClientIDMode="Static" onchange="toggleAssignmentFields();">
                    <asp:ListItem Text="-- Select --" Value="" />
                    <asp:ListItem Text="Team" Value="Team" />
                    <asp:ListItem Text="Individual" Value="Individual" />
                </asp:DropDownList>
            </div>

            <div class="form-row" id="assignTargetCell">
                <div id="teamAssignRow" style="display:none;">
                    <label for="ddlTeam">Assign Team</label>
                    <asp:DropDownList ID="ddlTeam" runat="server" CssClass="form-control" ClientIDMode="Static" />
                </div>
                <div id="staffAssignRow" style="display:none;">
                    <label for="ddlStaff">Assign Staff</label>
                    <asp:DropDownList ID="ddlStaff" runat="server" CssClass="form-control" ClientIDMode="Static" />
                </div>
            </div>

            <div class="form-row">
                <label for="txtScheduledDate">Scheduled Date</label>
                <asp:TextBox ID="txtScheduledDate" runat="server" CssClass="form-control" TextMode="Date" ClientIDMode="Static" />
            </div>

            <div class="form-row">
                <label for="txtExpectedCompletionDate">Expected Completion Date</label>
                <asp:TextBox ID="txtExpectedCompletionDate" runat="server" CssClass="form-control" TextMode="Date" ClientIDMode="Static" />
            </div>

            <div class="form-row">
                <label for="ddlStatus">Status</label>
                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control" ClientIDMode="Static">
                    <asp:ListItem Text="Pending" Value="Pending" />
                    <asp:ListItem Text="In Progress" Value="In Progress" />
                    <asp:ListItem Text="Completed" Value="Completed" />
                    <asp:ListItem Text="Cancelled" Value="Cancelled" />
                </asp:DropDownList>
            </div>

            <div class="form-row full-width">
                <label for="txtDescription">Description</label>
                <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" ClientIDMode="Static" />
            </div>

            <div class="form-actions">
                <asp:Button ID="btnSaveJob" runat="server" Text="Save Job" CssClass="btn-save" OnClick="btnSaveJob_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn-cancel" CausesValidation="false" OnClick="btnCancel_Click" />
            </div>

        </div>

    </div>

  
    <div class="job-history-title">Recent Cleaning Jobs</div>

    <div class="job-grid-wrapper">
        <asp:GridView ID="gvJobs" runat="server" AutoGenerateColumns="false" CssClass="job-grid"
            GridLines="None" DataKeyNames="JobID" OnRowCommand="gvJobs_RowCommand"
            EmptyDataText="No jobs have been created yet.">
            <Columns>
                <asp:BoundField DataField="JobID" HeaderText="Job ID" />
                <asp:BoundField DataField="SectionName" HeaderText="Section" />
                <asp:BoundField DataField="JobType" HeaderText="Job Type" />
                <asp:BoundField DataField="Description" HeaderText="Description" />
                <asp:BoundField DataField="ScheduledDate" HeaderText="Scheduled Date" DataFormatString="{0:yyyy-MM-dd}" HtmlEncode="false" />
                <asp:BoundField DataField="ExpectedCompletionDate" HeaderText="Expected Completion" DataFormatString="{0:yyyy-MM-dd}" HtmlEncode="false" />
                <asp:BoundField DataField="AssignedTo" HeaderText="Assigned To" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='status-pill status-<%# ((string)Eval("Status")).Replace(" ", "").ToLower() %>'>
                            <%# Eval("Status") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEdit" runat="server" CssClass="edit-link"
                            CommandName="EditJob" CommandArgument='<%# Eval("JobID") %>'>Edit</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <script type="text/javascript">
      
        function toggleJobForm(show) {
            var panel = document.getElementById('pnlJobFormWrapper');
            if (panel) {
                panel.style.display = show ? 'block' : 'none';
            }
        }
        <a href="CleaningJobs.aspx">CleaningJobs.aspx</a>
       
        function toggleAssignmentFields() {
            var type = document.getElementById('ddlAssignmentType').value;
            var teamRow = document.getElementById('teamAssignRow');
            var staffRow = document.getElementById('staffAssignRow');

            teamRow.style.display = (type === 'Team') ? 'block' : 'none';
            staffRow.style.display = (type === 'Individual') ? 'block' : 'none';
        }
    </script>

</asp:Content>
