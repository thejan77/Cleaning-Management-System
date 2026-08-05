<%@ Page Title="User Accounts" Language="C#" MasterPageFile="~/CmsMaster.master"
    AutoEventWireup="true" CodeFile="UserAccount.aspx.cs"
    Inherits="CleaningManagement_Masters_UserAccount" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<style>
    :root {
        --black: #0B0D12; --orange: #E8622D; --orange-dark: #CC5222;
        --bg-light: #F4F4F5; --gray-text: #64748B; --border-color: #E2E8F0;
    }
    .cms-page-title { font-size:22px; font-weight:700; margin-bottom:18px; color:var(--black); }
    .add-job-box {
        border:1.5px solid var(--orange); border-radius:8px; background:#fff;
        color:var(--orange); padding:10px 16px; font-size:13.5px; font-weight:600;
        cursor:pointer; display:inline-flex; align-items:center; gap:8px;
        transition:all 0.18s ease-in-out; margin-bottom:20px;
    }
    .add-job-box:hover { background:var(--orange); color:#fff; }
    .add-job-box .plus-icon { font-size:15px; font-weight:bold; }
    #pnlUserFormWrapper {
        display:none; border:1px solid var(--border-color); border-radius:10px;
        padding:24px 28px; margin-bottom:26px; background:#fff;
        box-shadow:0 2px 8px rgba(11,13,18,0.06);
    }
    .form-heading-row {
        display:flex; justify-content:space-between; align-items:center;
        margin-bottom:18px; padding-bottom:14px; border-bottom:1px solid var(--border-color);
    }
    .form-grid { display:grid; grid-template-columns:1fr 1fr; column-gap:32px; row-gap:16px; }
    .form-grid .form-row.full-width { grid-column:1/-1; }
    .form-row label { display:block; font-weight:600; font-size:13px; margin-bottom:6px; color:var(--black); }
    .form-row .form-control {
        width:100%; box-sizing:border-box; padding:9px 12px;
        border:1px solid var(--border-color); border-radius:6px;
        font-size:13.5px; color:var(--black); background:#fff;
    }
    .form-row .form-control:focus { outline:none; border-color:var(--orange); }
    .form-actions { margin-top:22px; grid-column:1/-1; }
    .btn-save {
        background:var(--orange); border:none; color:#fff;
        padding:10px 24px; border-radius:6px; font-weight:600; font-size:13.5px; cursor:pointer;
    }
    .btn-save:hover { background:var(--orange-dark); }
    .btn-cancel {
        background:var(--bg-light); border:1px solid var(--border-color); color:var(--black);
        padding:10px 24px; border-radius:6px; font-weight:600; font-size:13.5px;
        cursor:pointer; margin-left:10px;
    }
    .btn-update { background:#DC3545; border:none; color:#fff; padding:10px 24px; border-radius:6px; font-weight:600; font-size:13.5px; cursor:pointer; }
    .btn-update:hover { background:#B02A37; }
    .job-history-title { font-size:17px; font-weight:700; margin:26px 0 12px 0; color:var(--black); }
    .job-grid-wrapper { border:1px solid var(--border-color); border-radius:10px; overflow:hidden; background:#fff; }
    .job-grid { width:100%; border-collapse:collapse; }
    .job-grid th { background:var(--black); color:#fff; padding:12px 14px; text-align:left; font-size:12.5px; font-weight:600; text-transform:uppercase; letter-spacing:0.3px; }
    .job-grid td { padding:11px 14px; border-bottom:1px solid var(--border-color); font-size:13px; color:var(--black); }
    .job-grid tr:last-child td { border-bottom:none; }
    .job-grid tr:hover td { background:var(--bg-light); }
    .status-pill { padding:3px 11px; border-radius:12px; font-size:11.5px; font-weight:600; color:#fff; }
    .status-active   { background:#2E7D32; }
    .status-inactive { background:#94A3B8; }
    .role-pill { padding:3px 11px; border-radius:12px; font-size:11.5px; font-weight:600; color:#fff; background:var(--gray-text); }
    .role-admin      { background:var(--black); }
    .role-supervisor { background:var(--orange); }
    .role-staff      { background:#1565C0; }
    .edit-link   { color:var(--orange); font-weight:600; font-size:13px; text-decoration:none; margin-right:10px; }
    .edit-link:hover { color:var(--orange-dark); text-decoration:underline; }
    .reset-link  { color:#1565C0; font-weight:600; font-size:13px; text-decoration:none; margin-right:10px; }
    .reset-link:hover { text-decoration:underline; }
    .disable-link { color:#DC3545; font-weight:600; font-size:13px; text-decoration:none; }
    .disable-link:hover { text-decoration:underline; }
    .enable-link  { color:#2E7D32; font-weight:600; font-size:13px; text-decoration:none; }
    .enable-link:hover { text-decoration:underline; }
    .hint-text { font-size:11.5px; color:var(--gray-text); margin-top:4px; }
</style>

<div class="cms-page-title">User Accounts</div>

<asp:HiddenField ID="hdnUserID" runat="server" Value="0" ClientIDMode="Static" />

<div id="addUserBox" class="add-job-box" onclick="openNewUser();">
    <span class="plus-icon">+</span>
    <span>Add New User</span>
</div>

<div id="pnlUserFormWrapper">
    <div class="form-heading-row">
        <asp:Label ID="lblFormHeading" runat="server" Text="Add New User"
            Font-Bold="true" Font-Size="16px" ForeColor="#0B0D12" ClientIDMode="Static" />
        <asp:Label ID="lblMessage" runat="server" ClientIDMode="Static" />
    </div>

    <div class="form-grid">

        <div class="form-row">
            <label for="txtUsername">Username *</label>
            <asp:TextBox ID="txtUsername" runat="server"
                CssClass="form-control" ClientIDMode="Static" />
        </div>

        <div class="form-row">
            <label for="txtPassword">Password *</label>
            <asp:TextBox ID="txtPassword" runat="server"
                CssClass="form-control" TextMode="Password" ClientIDMode="Static" />
            <div class="hint-text" id="passwordHint">Leave blank when editing to keep current password.</div>
        </div>

        <div class="form-row">
            <label for="ddlRole">Role *</label>
            <asp:DropDownList ID="ddlRole" runat="server"
                CssClass="form-control" ClientIDMode="Static" />
        </div>

        <div class="form-row">
            <label for="ddlIsActive">Status</label>
            <asp:DropDownList ID="ddlIsActive" runat="server"
                CssClass="form-control" ClientIDMode="Static">
                <asp:ListItem Value="1">Active</asp:ListItem>
                <asp:ListItem Value="0">Inactive</asp:ListItem>
            </asp:DropDownList>
        </div>

        <div class="form-row">
            <label for="txtName">Full Name</label>
            <asp:TextBox ID="txtName" runat="server"
                CssClass="form-control" ClientIDMode="Static" />
        </div>

        <div class="form-row">
            <label for="txtEmail">Email</label>
            <asp:TextBox ID="txtEmail" runat="server"
                CssClass="form-control" ClientIDMode="Static" />
        </div>

        <div class="form-row">
            <label for="txtContactNumber">Contact Number</label>
            <asp:TextBox ID="txtContactNumber" runat="server"
                CssClass="form-control" ClientIDMode="Static" />
        </div>

        <div class="form-row">
            <label for="ddlStaff">Link to Staff (optional)</label>
            <asp:DropDownList ID="ddlStaff" runat="server"
                CssClass="form-control" ClientIDMode="Static" />
        </div>

        <div class="form-row">
            <label for="ddlContractor">Link to Contractor (optional)</label>
            <asp:DropDownList ID="ddlContractor" runat="server"
                CssClass="form-control" ClientIDMode="Static" />
        </div>

        <div class="form-actions">
            <asp:Button ID="btnSaveUser" runat="server"
                Text="Save User" CssClass="btn-save"
                ClientIDMode="Static" OnClick="btnSaveUser_Click" />
            <asp:Button ID="btnCancel" runat="server"
                Text="Cancel" CssClass="btn-cancel"
                CausesValidation="false" OnClick="btnCancel_Click" />
        </div>

    </div>
</div>

<div class="job-history-title">User Accounts</div>

<div class="job-grid-wrapper">
    <asp:GridView ID="gvUsers" runat="server"
        AutoGenerateColumns="false" CssClass="job-grid"
        GridLines="None" DataKeyNames="UserID"
        OnRowCommand="gvUsers_RowCommand"
        EmptyDataText="No user accounts found.">
        <Columns>
            <asp:BoundField DataField="UserID"   HeaderText="ID" />
            <asp:BoundField DataField="Username" HeaderText="Username" />
            <asp:BoundField DataField="Name"     HeaderText="Full Name" />
            <asp:BoundField DataField="Email"    HeaderText="Email" />
            <asp:BoundField DataField="ContactNumber" HeaderText="Contact" />
            <asp:TemplateField HeaderText="Role">
                <ItemTemplate>
                    <span class='role-pill role-<%# Eval("RoleName").ToString().ToLower() %>'>
                        <%# Eval("RoleName") %>
                    </span>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="StaffName"      HeaderText="Staff" />
            <asp:BoundField DataField="ContractorName" HeaderText="Contractor" />
            <asp:BoundField DataField="LastLogin" HeaderText="Last Login"
                DataFormatString="{0:yyyy-MM-dd HH:mm}" HtmlEncode="false" />
            <asp:TemplateField HeaderText="Status">
                <ItemTemplate>
                    <span class='status-pill status-<%# Convert.ToBoolean(Eval("IsActive")) ? "active" : "inactive" %>'>
                        <%# Convert.ToBoolean(Eval("IsActive")) ? "Active" : "Inactive" %>
                    </span>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Actions">
                <ItemTemplate>
                    <asp:LinkButton ID="lnkEdit" runat="server"
                        CssClass="edit-link" CommandName="EditUser"
                        CommandArgument='<%# Eval("UserID") %>'>Edit</asp:LinkButton>
                    <asp:LinkButton ID="lnkReset" runat="server"
                        CssClass="reset-link" CommandName="ResetPwd"
                        CommandArgument='<%# Eval("UserID") %>'
                        OnClientClick='<%# "return confirm(\"Reset password for " + Eval("Username") + " to default?\");" %>'>Reset Pwd</asp:LinkButton>
                    <asp:LinkButton ID="lnkToggle" runat="server"
                        CssClass='<%# Convert.ToBoolean(Eval("IsActive")) ? "disable-link" : "enable-link" %>'
                        CommandName="ToggleActive"
                        CommandArgument='<%# Eval("UserID") + "|" + Eval("IsActive") %>'
                        OnClientClick='<%# "return confirm(\"" + (Convert.ToBoolean(Eval("IsActive")) ? "Disable" : "Enable") + " this account?\");" %>'>
                        <%# Convert.ToBoolean(Eval("IsActive")) ? "Disable" : "Enable" %>
                    </asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</div>

<script type="text/javascript">
function toggleUserForm(show) {
    var panel = document.getElementById('pnlUserFormWrapper');
    if (panel) panel.style.display = show ? 'block' : 'none';
}

function openNewUser() {
    var userID = document.getElementById('hdnUserID').value;
    if (userID != "0") {
        alert("Please cancel the current edit before adding a new user.");
        return;
    }
    toggleUserForm(true);
    }
</script>

</asp:Content>