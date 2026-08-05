<%@ Page Title="Cleaning Schedule" Language="C#" MasterPageFile="~/CleaningManagement/CmsMaster.master" AutoEventWireup="true" CodeFile="CleaningSchedule.aspx.cs" Inherits="CleaningManagement_CleaningSchedule" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="d-flex justify-content-between align-items-center mb-3">
        <div>
            <h2 class="mb-0">Cleaning Schedule</h2>
            <p class="text-muted mb-0">Set up recurring cleaning tasks by section and frequency.</p>
        </div>
        <asp:LinkButton ID="btnNew" runat="server" CssClass="btn btn-warning text-white"
            OnClick="btnNew_Click">
            <i class="fa fa-plus"></i> New Schedule
        </asp:LinkButton>
    </div>

    <asp:Label ID="lblMessage" runat="server" CssClass="alert" Visible="false" />

    <div class="card">
        <div class="card-body p-0">
            <asp:GridView ID="gvSchedule" runat="server" AutoGenerateColumns="false"
                CssClass="table table-hover mb-0" GridLines="None"
                DataKeyNames="ScheduleID"
                OnRowCommand="gvSchedule_RowCommand"
                OnRowDataBound="gvSchedule_RowDataBound"
                EmptyDataText="No cleaning schedules have been created yet.">
                <Columns>
                    <asp:BoundField DataField="SectionName" HeaderText="Section" />
                    <asp:BoundField DataField="LocationName" HeaderText="Location" />
                    <asp:TemplateField HeaderText="Frequency">
                        <ItemTemplate>
                            <span class='<%# GetFrequencyBadgeClass(Eval("Frequency").ToString()) %>'>
                                <%# Eval("Frequency") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="StartDate" HeaderText="Start Date" DataFormatString="{0:dd MMM yyyy}" />
                    <asp:BoundField DataField="RepeatTime" HeaderText="Repeat Time" />
                    <asp:TemplateField HeaderText="">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="EditSchedule"
                                CommandArgument='<%# Eval("ScheduleID") %>' CssClass="text-secondary mr-2">
                                <i class="fa fa-edit"></i>
                            </asp:LinkButton>
                            <asp:LinkButton runat="server" CommandName="DeleteSchedule"
                                CommandArgument='<%# Eval("ScheduleID") %>' CssClass="text-danger"
                                OnClientClick="return confirm('Delete this cleaning schedule?');">
                                <i class="fa fa-trash"></i>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <!-- Add / Edit Modal -->
    <div class="modal fade" id="scheduleModal" tabindex="-1" role="dialog" runat="server" clientidmode="Static">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title"><asp:Literal ID="litModalTitle" runat="server" Text="New Cleaning Schedule" /></h5>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <asp:HiddenField ID="hfScheduleID" runat="server" Value="0" />

                    <div class="form-group">
                        <label>Section</label>
                        <asp:DropDownList ID="ddlSection" runat="server" CssClass="form-control"
                            DataTextField="DisplayName" DataValueField="SectionID" />
                    </div>

                    <div class="form-group">
                        <label>Frequency</label>
                        <asp:DropDownList ID="ddlFrequency" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Daily" Value="Daily" />
                            <asp:ListItem Text="Weekly" Value="Weekly" />
                            <asp:ListItem Text="Monthly" Value="Monthly" />
                        </asp:DropDownList>
                    </div>

                    <div class="form-row">
                        <div class="form-group col-md-6">
                            <label>Start Date</label>
                            <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" TextMode="Date" />
                        </div>
                        <div class="form-group col-md-6">
                            <label>Repeat Time</label>
                            <asp:TextBox ID="txtRepeatTime" runat="server" CssClass="form-control" TextMode="Time" />
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-warning text-white btn-block"
                        OnClick="btnSave_Click">Save Schedule</asp:LinkButton>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
