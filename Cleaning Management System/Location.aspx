<%@ Page Title="Locations" Language="C#" MasterPageFile="~/CmsMaster.master"
    AutoEventWireup="true" CodeFile="Location.aspx.cs" Inherits="CleaningManagement_Masters_Location" %>

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
            transition: all 0.18s ease-in-out;
            margin-bottom: 20px;
        }

        .add-job-box:hover {
            background-color: var(--orange);
            color: #ffffff;
        }

        .add-job-box .plus-icon {
            font-size: 15px;
            font-weight: bold;
        }

        #pnlLocationFormWrapper {
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

        .btn-save:hover { background-color: var(--orange-dark); }

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

        .btn-update {
            background-color: #DC3545;
            border: none;
            color: white;
            padding: 10px 24px;
            border-radius: 6px;
            font-weight: 600;
            font-size: 13.5px;
            cursor: pointer;
        }

        .btn-update:hover { background-color: #B02A37; }

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

        .job-grid tr:last-child td { border-bottom: none; }
        .job-grid tr:hover td { background-color: var(--bg-light); }

        .status-pill {
            padding: 3px 11px;
            border-radius: 12px;
            font-size: 11.5px;
            font-weight: 600;
            color: #fff;
        }

        .status-active   { background-color: #2E7D32; }
        .status-inactive { background-color: #94A3B8; }

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
    </style>

    <div class="cms-page-title">Locations</div>

    <asp:HiddenField ID="hdnLocationID" runat="server" Value="0" ClientIDMode="Static" />

    <%-- ── Trigger button ── --%>
    <div id="addLocationBox" class="add-job-box" onclick="openNewLocation();">
        <span class="plus-icon">+</span>
        <span>Add New Location</span>
    </div>

    <%-- ── Form panel ── --%>
    <div id="pnlLocationFormWrapper">

        <div class="form-heading-row">
            <asp:Label ID="lblFormHeading" runat="server"
                Text="Add New Location"
                Font-Bold="true" Font-Size="16px"
                ForeColor="#0B0D12" ClientIDMode="Static" />
            <asp:Label ID="lblMessage" runat="server" ClientIDMode="Static" />
        </div>

        <div class="form-grid">

            <div class="form-row">
                <label for="txtLocationName">Location Name *</label>
                <asp:TextBox ID="txtLocationName" runat="server"
                    CssClass="form-control"
                    placeholder="e.g. Main Building, Car Park, Tower Block"
                    ClientIDMode="Static" />
            </div>

            <div class="form-row">
                <label for="ddlActive">Status</label>
                <asp:DropDownList ID="ddlActive" runat="server"
                    CssClass="form-control" ClientIDMode="Static">
                    <asp:ListItem Value="1">Active</asp:ListItem>
                    <asp:ListItem Value="0">Inactive</asp:ListItem>
                </asp:DropDownList>
            </div>

            <div class="form-row full-width">
                <label for="txtDescription">Description</label>
                <asp:TextBox ID="txtDescription" runat="server"
                    CssClass="form-control"
                    TextMode="MultiLine" Rows="3"
                    placeholder="Optional — brief description of this location"
                    ClientIDMode="Static" />
            </div>

            <div class="form-actions">
                <asp:Button ID="btnSaveLocation" runat="server"
                    Text="Save Location"
                    CssClass="btn-save"
                    ClientIDMode="Static"
                    OnClick="btnSaveLocation_Click" />
                <asp:Button ID="btnCancel" runat="server"
                    Text="Cancel"
                    CssClass="btn-cancel"
                    CausesValidation="false"
                    OnClick="btnCancel_Click" />
            </div>

        </div>
    </div>

    <%-- ── Grid ── --%>
    <div class="job-history-title">Registered Locations</div>

    <div class="job-grid-wrapper">
        <asp:GridView ID="gvLocations" runat="server"
            AutoGenerateColumns="false"
            CssClass="job-grid"
            GridLines="None"
            DataKeyNames="LocationID"
            OnRowCommand="gvLocations_RowCommand"
            EmptyDataText="No locations have been added yet.">
            <Columns>
                <asp:BoundField DataField="LocationID"   HeaderText="ID" />
                <asp:BoundField DataField="LocationName" HeaderText="Location Name" />
                <asp:BoundField DataField="Description"  HeaderText="Description" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='status-pill status-<%# Convert.ToInt16(Eval("Active")) == 1 ? "active" : "inactive" %>'>
                            <%# Convert.ToInt16(Eval("Active")) == 1 ? "Active" : "Inactive" %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="CreatedDate" HeaderText="Created"
                    DataFormatString="{0:yyyy-MM-dd}" HtmlEncode="false" />
                <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEdit" runat="server"
                            CssClass="edit-link"
                            CommandName="EditLocation"
                            CommandArgument='<%# Eval("LocationID") %>'>Edit</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <script type="text/javascript">
        function toggleLocationForm(show) {
            var panel = document.getElementById('pnlLocationFormWrapper');
            if (panel) panel.style.display = show ? 'block' : 'none';
        }

        function openNewLocation() {
            var locationID = document.getElementById('hdnLocationID').value;
            if (locationID != "0") {
                alert("Please cancel the current edit before adding a new location.");
                return;
            }
            toggleLocationForm(true);
        }
    </script>

</asp:Content>