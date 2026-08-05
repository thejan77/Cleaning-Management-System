<%@ Page Title="Feedback &amp; Complaints" Language="C#" MasterPageFile="~/CmsMaster.Master" AutoEventWireup="true" CodeFile="Feedback.aspx.cs" Inherits="CleaningManagement_FeedbackComplaint" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
       
        :root {
            --black: #0B0D12;
            --orange: #E8622D;
            --orange-dark: #CC5222;
            --bg-light: #F4F4F5;
            --gray-text: #64748B;
            --border-color: #E2E8F0;
        }

        .fc-page {
            font-family: 'Segoe UI', Arial, sans-serif;
        }

        .fc-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
        }

            .fc-header h2 {
                color: var(--black);
                font-size: 22px;
                font-weight: 700;
                margin: 0;
            }

            .fc-header p {
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

        .fc-summary-row {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 16px;
            margin-bottom: 20px;
        }

        .fc-summary-card {
            background: #fff;
            border-radius: 12px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.06);
            padding: 16px 18px;
        }

            .fc-summary-card .label {
                font-size: 12px;
                color: var(--gray-text);
                text-transform: uppercase;
                letter-spacing: .3px;
                font-weight: 600;
            }

            .fc-summary-card .value {
                font-size: 24px;
                font-weight: 700;
                color: var(--black);
                margin-top: 6px;
            }

            .fc-summary-card.accent .value {
                color: var(--orange);
            }


        .fc-filter-card {
            background: #fff;
            border-radius: 12px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.06);
            padding: 18px 20px;
            margin-bottom: 20px;
        }

        .fc-filter-grid {
            display: grid;
            grid-template-columns: repeat(4, 1fr) auto;
            gap: 14px;
            align-items: end;
        }

        .fc-filter-item label {
            display: block;
            font-size: 12px;
            font-weight: 600;
            color: var(--gray-text);
            margin-bottom: 6px;
            text-transform: uppercase;
            letter-spacing: .3px;
        }

        .fc-filter-item select, .fc-filter-item input[type=text] {
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

            .fc-filter-item select:focus, .fc-filter-item input:focus {
                outline: none;
                border-color: var(--orange);
            }

        /* Ensures Clear button sits on the same row/level as the dropdowns */
        .fc-filter-item .fc-clear-btn {
            width: 100%;
            height: 38px;
            box-sizing: border-box;
            display: inline-flex;
            align-items: center;
            justify-content: center;
        }

        .fc-table-card {
            background: #fff;
            border-radius: 12px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.06);
            overflow: hidden;
            overflow-x: auto;
        }

        .fc-grid {
            width: 100%;
            border-collapse: collapse;
        }

            .fc-grid th {
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

            .fc-grid td {
                padding: 12px 14px;
                font-size: 13px;
                color: var(--black);
                border-bottom: 1px solid var(--border-color);
                vertical-align: middle;
            }

            .fc-grid tr:hover td {
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

        .pill-open {
            background: #FEF3C7;
            color: #92400E;
        }

        .pill-inprogress {
            background: #DBEAFE;
            color: #1E40AF;
        }

        .pill-resolved {
            background: #D1FAE5;
            color: #065F46;
        }

        .pill-closed {
            background: #F1F5F9;
            color: #334155;
        }

        .pill-rejected {
            background: #FEE2E2;
            color: #991B1B;
        }

        .pill-complaint {
            background: #FEE2E2;
            color: #991B1B;
        }

        .pill-feedback {
            background: #E0F2FE;
            color: #075985;
        }

        .pill-suggestion {
            background: #EDE9FE;
            color: #5B21B6;
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

        .fc-modal-overlay {
            display: none;
            position: fixed;
            inset: 0;
            background: rgba(11,13,18,0.55);
            z-index: 1000;
            align-items: center;
            justify-content: center;
        }

            .fc-modal-overlay.active {
                display: flex;
            }

        .fc-modal {
            background: #fff;
            width: 680px;
            max-width: 92%;
            max-height: 90vh;
            overflow-y: auto;
            border-radius: 14px;
            box-shadow: 0 20px 50px rgba(0,0,0,0.3);
        }

        .fc-modal-header {
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

            .fc-modal-header h3 {
                margin: 0;
                font-size: 17px;
                color: var(--black);
                font-weight: 700;
            }

        .fc-modal-close {
            background: none;
            border: none;
            font-size: 20px;
            color: var(--gray-text);
            cursor: pointer;
        }

            .fc-modal-close:hover {
                color: var(--black);
            }

        .fc-modal-body {
            padding: 20px 24px;
        }

        .fc-form-row {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 16px;
            margin-bottom: 16px;
        }

        .fc-form-group.full {
            grid-column: 1 / -1;
        }

        .fc-form-group label {
            display: block;
            font-size: 12px;
            font-weight: 600;
            color: var(--black);
            margin-bottom: 6px;
            text-transform: uppercase;
            letter-spacing: .3px;
        }

        .fc-form-group select, .fc-form-group input[type=text], .fc-form-group input[type=date], .fc-form-group textarea {
            width: 100%;
            padding: 10px 12px;
            border: 1px solid var(--border-color);
            border-radius: 8px;
            font-size: 13px;
            color: var(--black);
            box-sizing: border-box;
            background: #fff;
        }

            .fc-form-group select:focus, .fc-form-group input:focus, .fc-form-group textarea:focus {
                outline: none;
                border-color: var(--orange);
            }

        /* Locked Feedback ID display - same visual pattern as AWR's awr-code-display */
        .fc-code-display {
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

        .fc-section-title {
            font-size: 12px;
            font-weight: 700;
            color: var(--orange-dark);
            text-transform: uppercase;
            letter-spacing: .4px;
            margin: 4px 0 12px 0;
            padding-top: 8px;
            border-top: 1px dashed var(--border-color);
        }

        .fc-modal-body > .fc-section-title:first-child {
            border-top: none;
            padding-top: 0;
        }

        .fc-modal-footer {
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

        .fc-hidden {
            display: none !important;
        }
    </style>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">


    <div class="fc-header">
        <div>
            <h2>Feedback &amp; Complaints</h2>
            <p>Log, track and resolve feedback, complaints and suggestions from staff and clients</p>
        </div>
        <asp:LinkButton ID="btnAddFeedback" runat="server" CssClass="btn-orange" OnClick="btnAddFeedback_Click">
            + Add Feedback / Complaint
        </asp:LinkButton>
    </div>

 
    <div class="fc-summary-row">
        <div class="fc-summary-card accent">
            <div class="label">Open</div>
            <asp:Label ID="lblCountOpen" runat="server" CssClass="value" Text="0" />
        </div>
        <div class="fc-summary-card">
            <div class="label">In Progress</div>
            <asp:Label ID="lblCountInProgress" runat="server" CssClass="value" Text="0" />
        </div>
        <div class="fc-summary-card">
            <div class="label">Resolved (This Month)</div>
            <asp:Label ID="lblCountResolved" runat="server" CssClass="value" Text="0" />
        </div>
        <div class="fc-summary-card">
            <div class="label">Complaints (Total)</div>
            <asp:Label ID="lblCountComplaints" runat="server" CssClass="value" Text="0" />
        </div>
    </div>


    <div class="fc-filter-card">
        <div class="fc-filter-grid">
            <div class="fc-filter-item">
                <label>Section</label>
                <asp:DropDownList ID="ddlFilterSection" runat="server" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlFilterSection_SelectedIndexChanged">
                    <asp:ListItem Text="All Sections" Value="" />
                </asp:DropDownList>
            </div>
            <div class="fc-filter-item">
                <label>Type</label>
                <asp:DropDownList ID="ddlFilterType" runat="server" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlFilterType_SelectedIndexChanged">
                    <asp:ListItem Text="All Types" Value="" />
                    <asp:ListItem Text="Complaint" Value="Complaint" />
                    <asp:ListItem Text="Feedback" Value="Feedback" />
                    <asp:ListItem Text="Suggestion" Value="Suggestion" />
                </asp:DropDownList>
            </div>
            <div class="fc-filter-item">
                <label>Status</label>
                <asp:DropDownList ID="ddlFilterStatus" runat="server" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlFilterStatus_SelectedIndexChanged">
                    <asp:ListItem Text="All Status" Value="" />
                    <asp:ListItem Text="Open" Value="Open" />
                    <asp:ListItem Text="In Progress" Value="In Progress" />
                    <asp:ListItem Text="Resolved" Value="Resolved" />
                    <asp:ListItem Text="Rejected" Value="Rejected" />
                    <asp:ListItem Text="Closed" Value="Closed" />
                </asp:DropDownList>
            </div>
            <div class="fc-filter-item">
                <label>Submitted By</label>
                <asp:DropDownList ID="ddlFilterSubmittedByType" runat="server" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlFilterSubmittedByType_SelectedIndexChanged">
                    <asp:ListItem Text="All" Value="" />
                    <asp:ListItem Text="Client" Value="Client" />
                    <asp:ListItem Text="Staff" Value="Staff" />
                    <asp:ListItem Text="Visitor" Value="Visitor" />
                    <asp:ListItem Text="Contractor" Value="Contractor" />
                </asp:DropDownList>
            </div>
            <div class="fc-filter-item">
                <label>&nbsp;</label>
                <asp:LinkButton ID="btnClearFilters" runat="server" CssClass="btn-outline fc-clear-btn"
                    OnClick="btnClearFilters_Click">Clear</asp:LinkButton>
            </div>
        </div>
    </div>

    <div class="fc-table-card">
        <asp:GridView ID="gvFeedback" runat="server" CssClass="fc-grid" AutoGenerateColumns="false"
            DataKeyNames="FeedbackID" GridLines="None" OnRowCommand="gvFeedback_RowCommand"
            OnRowDataBound="gvFeedback_RowDataBound" EmptyDataText="No feedback or complaints found.">
            <Columns>
                <asp:TemplateField HeaderText="Feedback ID">
                    <ItemTemplate>
                        <%# FormatFeedbackCode(Eval("FeedbackID")) %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="SectionName" HeaderText="Section" />

                <asp:TemplateField HeaderText="Type">
                    <ItemTemplate>
                        <asp:Label ID="lblType" runat="server" CssClass="pill" Text='<%# Eval("Type") %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="Description" HeaderText="Description" />
                <asp:BoundField DataField="SubmittedByType" HeaderText="Submitted By" />
                <asp:BoundField DataField="SubmittedByName" HeaderText="Name" />

                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <asp:Label ID="lblStatus" runat="server" CssClass="pill" Text='<%# Eval("Status") %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="SubmittedDate" HeaderText="Submitted" DataFormatString="{0:dd MMM yyyy}" />
                <asp:BoundField DataField="ResolvedDate" HeaderText="Resolved" DataFormatString="{0:dd MMM yyyy}" />

                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEdit" runat="server" CssClass="action-link" CommandName="EditFeedback"
                            CommandArgument='<%# Eval("FeedbackID") %>'>Edit</asp:LinkButton>
                        <asp:LinkButton ID="lnkDelete" runat="server" CssClass="action-link danger" CommandName="DeleteFeedback"
                            CommandArgument='<%# Eval("FeedbackID") %>'
                            OnClientClick="return confirm('Delete this feedback / complaint record?');">Delete</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

  
    <div id="fcModalOverlay" class="fc-modal-overlay">
        <div class="fc-modal">
            <div class="fc-modal-header">
                <h3 id="fcModalTitle" runat="server">Add Feedback / Complaint</h3>
                <button type="button" class="fc-modal-close" onclick="closeFcModal();">✕</button>
            </div>

            <div class="fc-modal-body">
                <asp:HiddenField ID="hfFeedbackID" runat="server" Value="0" />

                <div class="fc-section-title">Details</div>

                <div class="fc-form-row">
                    <div class="fc-form-group">
                        <label>Feedback ID</label>
                        <span class="fc-code-display"><asp:Literal ID="litFeedbackCode" runat="server" /></span>
                    </div>
                    <div class="fc-form-group">
                        <label>Section</label>
                        <asp:DropDownList ID="ddlSection" runat="server" ClientIDMode="Static">
                            <asp:ListItem Text="-- Select Section --" Value="" />
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="fc-form-row">
                    <div class="fc-form-group">
                        <label>Location (Optional)</label>
                        <asp:DropDownList ID="ddlLocation" runat="server" ClientIDMode="Static">
                            <asp:ListItem Text="-- Select Location --" Value="" />
                        </asp:DropDownList>
                    </div>
                    <div class="fc-form-group">
                        <label>Type</label>
                        <asp:DropDownList ID="ddlType" runat="server" ClientIDMode="Static">
                            <asp:ListItem Text="Complaint" Value="Complaint" />
                            <asp:ListItem Text="Feedback" Value="Feedback" />
                            <asp:ListItem Text="Suggestion" Value="Suggestion" />
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="fc-form-row">
                    <div class="fc-form-group">
                        <label>Status</label>
                        <asp:DropDownList ID="ddlStatus" runat="server" ClientIDMode="Static" onchange="toggleResolvedDate();">
                            <asp:ListItem Text="Open" Value="Open" Selected="True" />
                            <asp:ListItem Text="In Progress" Value="In Progress" />
                            <asp:ListItem Text="Resolved" Value="Resolved" />
                            <asp:ListItem Text="Rejected" Value="Rejected" />
                            <asp:ListItem Text="Closed" Value="Closed" />
                        </asp:DropDownList>
                    </div>
                    <div class="fc-form-group">
                        <label>&nbsp;</label>
                    </div>
                </div>

                <div class="fc-form-row">
                    <div class="fc-form-group full">
                        <label>Description</label>
                        <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="3" ClientIDMode="Static" />
                    </div>
                </div>

              
                <div class="fc-section-title">Submitted By</div>
                <div class="fc-form-row">
                    <div class="fc-form-group">
                        <label>Submitted By Type</label>
                        <asp:DropDownList ID="ddlSubmittedByType" runat="server" ClientIDMode="Static" onchange="toggleSubmittedBy();">
                            <asp:ListItem Text="Client" Value="Client" Selected="True" />
                            <asp:ListItem Text="Staff" Value="Staff" />
                            <asp:ListItem Text="Visitor" Value="Visitor" />
                            <asp:ListItem Text="Contractor" Value="Contractor" />
                        </asp:DropDownList>
                    </div>
                    <div class="fc-form-group" id="rowSubmittedByStaff">
                        <label>Submitted By (Staff)</label>
                        <asp:DropDownList ID="ddlSubmittedByStaff" runat="server" ClientIDMode="Static">
                            <asp:ListItem Text="-- Select Staff --" Value="" />
                        </asp:DropDownList>
                    </div>
                </div>

             
                <div class="fc-section-title">Resolution</div>
                <div class="fc-form-row">
                    <div class="fc-form-group">
                        <label>Submitted Date</label>
                        <asp:TextBox ID="txtSubmittedDate" runat="server" TextMode="Date" ClientIDMode="Static" />
                    </div>
                    <div class="fc-form-group" id="rowResolvedDate">
                        <label>Resolved Date</label>
                        <asp:TextBox ID="txtResolvedDate" runat="server" TextMode="Date" ClientIDMode="Static" />
                    </div>
                </div>

            </div>

            <div class="fc-modal-footer">
                <button type="button" class="btn-outline" onclick="closeFcModal();">Cancel</button>
                <asp:LinkButton ID="btnSaveFeedback" runat="server" CssClass="btn-orange" OnClick="btnSaveFeedback_Click">
                    Save Record
                </asp:LinkButton>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function openFcModal() {
            document.getElementById('fcModalOverlay').classList.add('active');
            toggleSubmittedBy();
            toggleResolvedDate();
        }

        function closeFcModal() {
            document.getElementById('fcModalOverlay').classList.remove('active');
        }

        function toggleSubmittedBy() {
            var type = document.getElementById('ddlSubmittedByType').value;
            var row = document.getElementById('rowSubmittedByStaff');
            if (type === 'Staff') {
                row.classList.remove('fc-hidden');
            } else {
                row.classList.add('fc-hidden');
            }
        }

        function toggleResolvedDate() {
            var status = document.getElementById('ddlStatus').value;
            var row = document.getElementById('rowResolvedDate');
            if (status === 'Resolved' || status === 'Closed') {
                row.classList.remove('fc-hidden');
            } else {
                row.classList.add('fc-hidden');
            }
        }
    </script>


</asp:Content>