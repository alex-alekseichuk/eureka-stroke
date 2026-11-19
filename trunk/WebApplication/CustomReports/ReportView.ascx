<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportView.ascx.cs" Inherits="WebApplication.CustomReports.ReportView" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<style type="text/css">
.report_Size_Fix
{
    height:1180px !important;
}
.report_Size_Fix iframe
{
    height:1100px !important;
}
</style>
<form runat="server">

    Reporting Source:
    <asp:DropDownList runat="server" ID="ddlReportingSources" 
        DataSourceID="dsReportingSources" AutoPostBack="true"
        DataValueField="Name" DataTextField="Name" />
    <asp:ObjectDataSource runat="server" ID="dsReportingSources"
        TypeName="WebApplication.CustomReports.ReportingSourcesDataSource" 
        SelectMethod="GetReportingSources"></asp:ObjectDataSource>
    <asp:CheckBox runat="server" ID="chkAllReportingSource" Text="All" />
    <asp:Label runat="server" ID="lblReportDescription" style="margin-left:30px;"></asp:Label>
    <rsweb:ReportViewer ID="rvControl" runat="server" ProcessingMode="Remote" 
        Height="500px" Width="100%" ShowBackButton="True" CssClass="report_Size_Fix" 
        EnableViewState="True">
    </rsweb:ReportViewer>
</form>


