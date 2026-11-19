<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UsersAdmin.ascx.cs" Inherits="WebApplication.CustomReports.UsersAdmin" %>
    
<form id="Form1" runat="server">

<div style="float:left;" class="Area">
    <asp:ListView ID="listUsers" runat="server" DataSourceID="dsUsers" DataKeyNames="ID"
        OnSelectedIndexChanged="listUsers_SelectedIndexChanged">
        <EmptyDataTemplate>
            No Users
        </EmptyDataTemplate>
        <ItemTemplate>
            <tr class="Item">
            <td>
                <asp:ImageButton ID="btnSelect" CommandName="Select" ImageUrl="~/img/actions/user.gif" AlternateText="Select" runat="server" ImageAlign="AbsMiddle" ToolTip="Select User" />
            </td>
            <td>
                    <asp:LinkButton runat="server" ID="btnSelect2" CommandName="Select">
                        <asp:Label runat="server" ID="lblName" Text='<%# Eval("UserName") %>'></asp:Label>
                    </asp:LinkButton>
            </td>
            </tr>
        </ItemTemplate>
        <SelectedItemTemplate>
            <tr class="Select">
            <td>
                    <asp:ImageButton ID="btnSelect" CommandName="Select" ImageUrl="~/img/actions/user.gif" AlternateText="Select" runat="server" ImageAlign="AbsMiddle" ToolTip="Select User" />
            </td>
            <td>
                    <asp:Label runat="server" ID="lblName" Text='<%# Eval("UserName") %>'></asp:Label>
            </td>
            </tr>
        </SelectedItemTemplate>
        <LayoutTemplate>
            <h3>Users</h3>
            <table class="Items">
                <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
            </table>
        </LayoutTemplate>
    </asp:ListView>
    
    <asp:ObjectDataSource ID="dsUsers" runat="server"
        TypeName="WebApplication.CustomReports.UsersDataSource" 
        SelectMethod="GetUsers">
    </asp:ObjectDataSource>
</div>

<asp:PlaceHolder id="phUserPrivileges" runat="server" Visible="false">
    <div style="float:left;" class="Area">
    <asp:ListView ID="listUserObjects" runat="server" DataSourceID="dsUserObjects" DataKeyNames="ID,Type,Name">
        <EmptyDataTemplate>
            No privileges
        </EmptyDataTemplate>
        <ItemTemplate>
            <tr class="Item">
            <td>
                    <img src='<%# "~/img/actions/"+Eval("TypeName")+".gif" %>' runat="server" title='<%# Eval("TypeName") %>' />
                    <asp:ImageButton ID="btnDelete" CommandName="Delete" ImageUrl="~/img/actions/delete.gif" AlternateText="Delete" runat="server" OnClientClick="return confirm('Are you sure to revoke permisions on this object?');" ToolTip="Revoke privileges" />
            </td>
            <td>
                    <asp:Label runat="server" ID="lblName" Text='<%# Eval("Name") %>' ToolTip='<%# Eval("TypeName") %>'></asp:Label>
            </td>
            </tr>
        </ItemTemplate>
        <LayoutTemplate>
            <h3>Granted Access</h3>
            <table class="Items">
                <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
            </table>
        </LayoutTemplate>
    </asp:ListView>
    </div>
    <asp:ObjectDataSource ID="dsUserObjects" runat="server"
        TypeName="WebApplication.CustomReports.UserObjectsDataSource" 
        SelectMethod="GetUserObjects"
        DeleteMethod="DeleteUserObject" OnDeleted="listUserObjects_Deleted">
        <SelectParameters>
            <asp:ControlParameter Name="userID" Type="Int32" DefaultValue="0" ControlID="listUsers" PropertyName="SelectedDataKey.Values[&quot;ID&quot;]" />
        </SelectParameters>
        <DeleteParameters>
            <asp:ControlParameter Name="userID" Type="Int32" DefaultValue="0" ControlID="listUsers" PropertyName="SelectedDataKey.Values[&quot;ID&quot;]" />
        </DeleteParameters>
    </asp:ObjectDataSource>
    
    <div style="float:left;" id="container">

    <ul>
        <li><a href="#section-folders">Categories</a></li>
        <li><a href="#section-reports">Reports</a></li>
        <li><a href="#section-facilities">Facilities</a></li>
        <li><a href="#section-reporting-sources">Reporting Sources</a></li>
    </ul>

    <div class="Area" id="section-folders">
    <asp:ListView ID="listOtherFolders" runat="server" DataSourceID="dsOtherFolders" DataKeyNames="ID" OnSelectedIndexChanged="listOtherFolders_SelectedIndexChanged">
        <EmptyDataTemplate>
            No Categories
        </EmptyDataTemplate>
        <ItemTemplate>
            <tr class="Item">
            <td>
                    <asp:ImageButton ID="btnSelect" CommandName="Select" ImageUrl="~/img/actions/folder.gif" AlternateText="Grant" runat="server" ImageAlign="AbsMiddle" ToolTip="Grant privileges" />
            </td>
            <td>
                    <asp:LinkButton runat="server" ID="btnSelect2" CommandName="Select" ToolTip="Grant privileges">
                        <asp:Label runat="server" ID="Label1" Text='<%# Eval("Name") %>'></asp:Label>
                    </asp:LinkButton>
            </td>
            </tr>
        </ItemTemplate>
        <LayoutTemplate>
            <h3>Available Categories</h3>
            <table class="Items">
                <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
            </table>
        </LayoutTemplate>
    </asp:ListView>
    <asp:ObjectDataSource ID="dsOtherFolders" runat="server"
        TypeName="WebApplication.CustomReports.UserObjectsDataSource" 
        SelectMethod="GetOtherFolders">
        <SelectParameters>
            <asp:ControlParameter Name="userID" Type="Int32" DefaultValue="0" ControlID="listUsers" PropertyName="SelectedDataKey.Values[&quot;ID&quot;]" />
        </SelectParameters>
    </asp:ObjectDataSource>
    </div>

    <div class="Area" id="section-reports">
    <asp:ListView ID="listOtherReports" runat="server" DataSourceID="dsOtherReports" DataKeyNames="ID" OnSelectedIndexChanged="listOtherReports_SelectedIndexChanged">
        <EmptyDataTemplate>
            No Reports
        </EmptyDataTemplate>
        <ItemTemplate>
            <tr class="Item">
            <td>
                    <asp:ImageButton ID="btnSelect" CommandName="Select" ImageUrl="~/img/actions/report.gif" AlternateText="Grant" runat="server" ImageAlign="AbsMiddle" ToolTip="Grant privileges" />
            </td>
            <td>
                    <asp:LinkButton runat="server" ID="btnSelect2" CommandName="Select" ToolTip="Grant privileges">
                        <asp:Label runat="server" ID="Label1" Text='<%# Eval("Name") %>'></asp:Label>
                    </asp:LinkButton>
            </td>
            </tr>
        </ItemTemplate>
        <LayoutTemplate>
            <h3>Available Reports</h3>
            <table class="Items">
                <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
            </table>
        </LayoutTemplate>
    </asp:ListView>
    <asp:ObjectDataSource ID="dsOtherReports" runat="server"
        TypeName="WebApplication.CustomReports.UserObjectsDataSource" 
        SelectMethod="GetOtherReports">
        <SelectParameters>
            <asp:ControlParameter Name="userID" Type="Int32" DefaultValue="0" ControlID="listUsers" PropertyName="SelectedDataKey.Values[&quot;ID&quot;]" />
        </SelectParameters>
    </asp:ObjectDataSource>
    </div>

    <div class="Area" id="section-facilities">
    <asp:ListView ID="listOtherFacilities" runat="server" DataSourceID="dsOtherFacilities" DataKeyNames="ID" OnSelectedIndexChanged="listOtherFacilities_SelectedIndexChanged">
        <EmptyDataTemplate>
            No Facilities
        </EmptyDataTemplate>
        <ItemTemplate>
            <tr class="Item">
            <td>
                    <asp:ImageButton ID="btnSelect" CommandName="Select" ImageUrl="~/img/actions/facility.gif" AlternateText="Grant" runat="server" ImageAlign="AbsMiddle" ToolTip="Grant privileges" />
            </td>
            <td>
                    <asp:LinkButton runat="server" ID="btnSelect2" CommandName="Select" ToolTip="Grant privileges">
                        <asp:Label runat="server" ID="Label1" Text='<%# Eval("Name") %>'></asp:Label>
                    </asp:LinkButton>
            </td>
            </tr>
        </ItemTemplate>
        <LayoutTemplate>
            <h3>Available Facilities</h3>
            <table class="Items">
                <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
            </table>
        </LayoutTemplate>
    </asp:ListView>
    <asp:ObjectDataSource ID="dsOtherFacilities" runat="server"
        TypeName="WebApplication.CustomReports.UserObjectsDataSource" 
        SelectMethod="GetOtherFacilities">
        <SelectParameters>
            <asp:ControlParameter Name="userID" Type="Int32" DefaultValue="0" ControlID="listUsers" PropertyName="SelectedDataKey.Values[&quot;ID&quot;]" />
        </SelectParameters>
    </asp:ObjectDataSource>
    </div>

    <div class="Area" id="section-reporting-sources">
    <asp:ListView ID="listOtherReportingSources" runat="server" DataSourceID="dsOtherReportingSources" DataKeyNames="Name" OnSelectedIndexChanged="listOtherReportingSources_SelectedIndexChanged">
        <EmptyDataTemplate>
            No Reporting Sources
        </EmptyDataTemplate>
        <ItemTemplate>
            <tr class="Item">
            <td>
                    <asp:ImageButton ID="btnSelect" CommandName="Select" ImageUrl="~/img/actions/reporting-source.gif" AlternateText="Grant" runat="server" ImageAlign="AbsMiddle" ToolTip="Grant privileges" />
            </td>
            <td>
                    <asp:LinkButton runat="server" ID="btnSelect2" CommandName="Select" ToolTip="Grant privileges">
                        <asp:Label runat="server" ID="Label1" Text='<%# Eval("Name") %>'></asp:Label>
                    </asp:LinkButton>
            </td>
            </tr>
        </ItemTemplate>
        <LayoutTemplate>
            <h3>Available Reporting Sources</h3>
            <table class="Items">
                <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
            </table>
        </LayoutTemplate>
    </asp:ListView>
    <asp:ObjectDataSource ID="dsOtherReportingSources" runat="server"
        TypeName="WebApplication.CustomReports.UserObjectsDataSource" 
        SelectMethod="GetOtherReportingSources">
        <SelectParameters>
            <asp:ControlParameter Name="userID" Type="Int32" DefaultValue="0" ControlID="listUsers" PropertyName="SelectedDataKey.Values[&quot;ID&quot;]" />
        </SelectParameters>
    </asp:ObjectDataSource>
    </div>

    </div>    
</asp:PlaceHolder>

<div style="clear:both;"></div>

</form>
<script type="text/javascript"><!--
    jQuery(function() {
        jQuery('#container').tabs({ cookie: { expires: 30} });
    });
//--></script>