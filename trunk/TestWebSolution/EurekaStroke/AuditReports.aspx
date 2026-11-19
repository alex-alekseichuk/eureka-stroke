<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AuditReports.aspx.cs" Inherits="WebApplication.AuditReports" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ Register Assembly="System.Web.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
    Namespace="System.Web.UI.WebControls" TagPrefix="asp" %>
<%@ Register TagPrefix="EurekaStroke" TagName="Header" Src="Header.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Uploader - Eureka Stroke</title>
    <link href="<%= ResolveUrl("~/img/main.css") %>" rel="stylesheet" type="text/css" />
    <!--[if lte IE 7]>  
<link rel="stylesheet" type="text/css" href="<%= ResolveUrl("~/img/ie6.css") %>" />  
<![endif]-->

    <script type="text/javascript" src="<%= ResolveUrl("~/js/tmpl.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/js/jquery-1.4.2.min.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/js/jquery.tooltip.pack.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/js/init.js") %>"></script>
</head>
<body>
    <EurekaStroke:Header ID="Header1" runat="server" />
    <div>
        <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        Select Upload Log: &nbsp;
        <asp:DropDownList ID="drpUploadLog" runat="server" OnSelectedIndexChanged="drpUploadLog_SelectedIndexChanged"
            AppendDataBoundItems="True" AutoPostBack="True">
            <asp:ListItem Selected="True" Value="0">(All)</asp:ListItem>
        </asp:DropDownList>
        <asp:Button ID="btnErrorReport" runat="server" Text="Run Error Report" OnClick="btnErrorReport_Click" />
        &nbsp;
        <asp:Button ID="btnChangesReport" runat="server" Text="Run Changes Report" OnClick="btnChangesReport_Click" />
        &nbsp;
        <asp:Button ID="btnDataDictionary" runat="server" 
            Text="Run Data Dictionary Report" onclick="btnDataDictionary_Click"  />
        <br />
        
                <rsweb:ReportViewer ID="rptViewer" runat="server" Font-Names="Verdana" Font-Size="8pt"
                    Height="400px" Width="100%" OnLoad="rptViewer_Load" 
                    ProcessingMode="Local" AsyncRendering="True" SizeToReportContent="True">
                </rsweb:ReportViewer>
                
        </form>
    </div>
</body>
</html>
