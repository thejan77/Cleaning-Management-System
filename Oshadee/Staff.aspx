<%@ Page Title="Staff" Language="C#" MasterPageFile="~/CmsMaster.master"
    AutoEventWireup="true" CodeFile="Staff.aspx.cs" Inherits="CleaningManagement_Masters_Staff" %>



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

        
        .tabs-filter-row {
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 12px;
            margin: 26px 0 14px 0;
        }

        .role-tabs {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
        }

        .role-tab {
            border: 1px solid var(--border-color);
            background-color: #ffffff;
            color: var(--gray-text);
            padding: 7px 16px;
            border-radius: 20px;
            font-size: 13px;
            font-weight: 600;
            cursor: pointer;
            text-decoration: none;
        }

        .role-tab:hover {
            border-color: var(--orange);
            color: var(--orange);
        }

        .role-tab.active {
            background-color: var(--orange);
            border-color: var(--orange);
            color: #ffffff;
        }

        .staff-search-box {
            display: flex;
            align-items: center;
            gap: 8px;
            margin-left: auto;
        }

        .staff-search-box .form-control {
            width: 130px;
            box-sizing: border-box;
            padding: 8px 10px;
            border: 1px solid var(--border-color);
            border-radius: 6px;
            font-size: 13px;
            color: var(--black);
            background-color: #fff;
        }

        .staff-search-box .form-control:focus {
            outline: none;
            border-color: var(--orange);
        }

        .btn-search {
            background-color: var(--orange);
            border: none;
            color: #fff;
            padding: 8px 18px;
            border-radius: 6px;
            font-weight: 600;
            font-size: 13px;
            cursor: pointer;
        }

        .btn-search:hover {
            background-color: var(--orange-dark);
        }

        /* ---------- Staff table ---------- */
        .job-history-title {
            font-size: 22px;
            font-weight: 700;
            color: var(--black);
            margin-bottom: 10px;
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

        .btn-refresh {
    background-color: #D8DBDF;
    border: 1px solid #C7CBD1;
    color: var(--black);
    padding: 8px 18px;
    border-radius: 6px;
    font-weight: 600;
    font-size: 13px;
    cursor: pointer;
}

.btn-refresh:hover {
    background-color: #C7CBD1;
    color: var(--black);
}
    </style>

    <!-- ================= Role Tabs + Search Filter (same row, search pinned right) ================= -->

        <div class="job-history-title">
        Staff Records
    </div>


    <div class="tabs-filter-row">

        <div class="role-tabs">
            <asp:Repeater ID="rptRoleTabs" runat="server"
                OnItemCommand="rptRoleTabs_ItemCommand">

                <ItemTemplate>

                    <asp:LinkButton ID="lnkRoleTab" runat="server"
                        CssClass='<%# "role-tab" + (Eval("IsSelected").ToString() == "True" ? " active" : "") %>'
                        CommandName="FilterRole"
                        CommandArgument='<%# Eval("JobTitleValue") %>'
                        Text='<%# Eval("JobTitleValue") %>' />

                </ItemTemplate>

            </asp:Repeater>
        </div>

        <div class="staff-search-box">

            <asp:TextBox ID="txtSearchName" runat="server"
                CssClass="form-control"
                placeholder="Staff Name">
            </asp:TextBox>

            <asp:Button ID="btnSearchStaff" runat="server"
                Text="Search"
                CssClass="btn-search"
                OnClick="btnSearchStaff_Click" />

            <asp:Button ID="btnRefresh" runat="server"
               Text="Refresh"
               CssClass="btn-refresh"
               OnClick="btnRefresh_Click" />

        </div>

    </div>


    <!-- ================= Staff Records ================= -->



    <div class="job-grid-wrapper">

        <asp:GridView ID="gvStaff" runat="server"
            AutoGenerateColumns="false"
            CssClass="job-grid"
            GridLines="None"
            DataKeyNames="StaffID"
            EmptyDataText="No staff members found.">

            <Columns>

                <asp:TemplateField HeaderText="Staff ID">
                    <ItemTemplate>
                        <%# "STF-" + Convert.ToInt32(Eval("StaffID")).ToString("000") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField
                    DataField="Name"
                    HeaderText="Staff Name" />

                <asp:BoundField
                    DataField="ContactNumber"
                    HeaderText="Contact Number" />

                <asp:BoundField
                    DataField="JobTitle"
                    HeaderText="Job Title" />

                <asp:BoundField
                    DataField="TeamName"
                    HeaderText="Team" />

                <asp:BoundField
                    DataField="ContractorName"
                    HeaderText="Contractor" />

            </Columns>

        </asp:GridView>

    </div>

</asp:Content>
