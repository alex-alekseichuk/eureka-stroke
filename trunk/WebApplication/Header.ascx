<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Header.ascx.cs" Inherits="WebApplication.Header" %>
<%@ Import Namespace="WebApplication" %>
    <h1>Eureka Stroke</h1>
    <ul class="main_menu">
        <li><a href="~/Uploader.aspx" runat="server">File Uploader</a></li>
        <li><a href="~/FormatManager.aspx" runat="server">Format Manager</a></li>
        <li><a href="~/AuditReports.aspx" runat="server">Reports</a></li>
        <li><a id="A1" href="~/CustomReports/AdminReports.aspx" runat="server">More Reports</a></li>
        <%
            if (Util.IsAdmin()) {
        %>
            <li><a id="A2" href="~/CustomReports/AdminUsers.aspx" runat="server">Users</a></li>
        <%
            }
        %>
    </ul>
    <div style="clear:both;"></div>
