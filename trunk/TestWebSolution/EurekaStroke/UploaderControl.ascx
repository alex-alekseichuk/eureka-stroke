<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UploaderControl.ascx.cs" Inherits="WebApplication.UploaderControl" %>

<script type="text/javascript" src="<%= ResolveUrl("~/js/ajax_upload.js") %>"></script>
<script type="text/javascript" src="<%= ResolveUrl("~/js/uploader.js") %>"></script>

<h2>File Upload</h2>

<div id="boxError" class="boxError"></div>
<div id="boxMessage" class="boxMessage"></div>

<script type="text/javascript"><!--
    function beforeFacilityDataDownload() {
        jQuery('#<%=hidFacilityID.ClientID %>').val(jQuery('#selFacility').val());
    }
//--></script>

<form name="form" action="" method="POST" enctype="multipart/form-data" id="formUpload" runat="server">
    <div class="modes">
        <div class="modes_menu">
            <label><input type="radio" name="mode" id="mode_existing" class="mode" value="existing" /> Existing Facility</label>
            <label><input type="radio" name="mode" id="mode_new" class="mode" value="new" /> New Facility</label>
        </div>

        <div id="divMode_existing" class="divMode">
            <table class="form">
            <tr>
                <th>Select Facility:</th>
                <td>
                    <asp:HiddenField ID="hidFacilityID" runat="server" />
                    <select id="selFacility">
                        <asp:Repeater ID="repFacilities" runat="server" DataSourceID="FacilityList">
                            <ItemTemplate>
                                <option value="<%# Eval("ID") %>"><%# Eval("Name") %></option>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:ObjectDataSource ID="FacilityList" runat="server" 
                                TypeName="Eureka.Stroke.UploaderService.UploaderService" 
                                SelectMethod="GetFacilities">
		                        <SelectParameters>
			                        <asp:Parameter Name="queryString" ConvertEmptyStringToNull="true" DefaultValue="" />
		                        </SelectParameters>
                        </asp:ObjectDataSource>
                    </select>
                    <asp:Button ID="btnDownloadFacilityData" runat="server" 
                        Text="Download Facility Data" onclick="btnDownloadFacilityData_Click"
                        OnClientClick="beforeFacilityDataDownload();" />
                </td>
            </tr>
            </table>
        </div>
        <div id="divMode_new" class="divMode">
            <table class="form">
            <tr>
                <th>New Facility Name:</th>
                <td><input name="txtFacilityName" id="txtFacilityName" /></td>
            </tr>
            <tr>
                <th>Format Map:</th>
                <td>
                    <select id="selFormat">
                        <asp:Repeater ID="repFormats" runat="server" DataSourceID="FormatList">
                            <ItemTemplate>
                                <option value="<%# Eval("ID") %>"><%# Eval("Name") %></option>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:ObjectDataSource ID="FormatList" runat="server" 
                                TypeName="WebApplication.FormatManagerDataSource" 
                                SelectMethod="GetFormats">
                        </asp:ObjectDataSource>
                    </select>
                </td>
            </tr>
            </table>
        </div>
    </div>       
    
    <div>
        Select File: <input type="file" name="source" id="fileSource" />
    </div>

    <div class="buttons">
        <input type="button" id="btnUpload" value="Upload" />
        <span id="upload_animation" style="display:none;">
            <img id="Img1" runat="server" src="~/img/ajax-loader.gif" alt="Uploading..." />
        </span>
    </div>
    
    <asp:Button ID="btnDownloadStrokeData" runat="server" 
        Text="Download Stroke Data" onclick="btnDownloadStrokeData_Click" />

</form>

<div id="UploadLog"></div>

<script type="text/javascript"><!--
    jQuery(function() {
        // init the uploader component
        jQuery.EurekaStroke.Uploader.init(
            {
                Items: <%=jsonUploadLog%>
            });
    });
//--></script>

<script type="text/html" id="tmplUploadLog">
<@ if (Items.length > 0) { @>
<table class="grid" style="width:100%;">
    <tr>
        <th>Uploaded Filename</th>
        <th>Upload Date/Time</th>
        <th>Error Count</th>
        <th>Correction Count</th>
        <th>Changes Applied</th>
        <th>Status</th>
    </tr>
  <@ for ( var i = 0; i < Items.length; i++ ) { @>
    <tr>
        <td><@=Items[i].Filename@></td>
        <td><@=Items[i].UploadDate@></td>
        <td>
            <@ if (Items[i].ErrorCount > 0) { @>
                <a href="AuditReports.aspx?rid=1&uid=<@=Items[i].ID@>"><@=Items[i].ErrorCount@></a>
            <@ } else { @>
                <@=Items[i].ErrorCount@>
            <@ } @>
        </td>
        <td>
            <@ if (Items[i].CorrectionsCount > 0) { @>
                <a href="AuditReports.aspx?rid=2&uid=<@=Items[i].ID@>"><@=Items[i].CorrectionsCount@></a>
            <@ } else { @>
                <@=Items[i].CorrectionsCount@>
            <@ } @>
        </td>
        <td><@=Items[i].ChangesCount@></td>
        <td><@=jQuery.EurekaStroke.Uploader.Statuses[Items[i].Status]@></td>
    </tr>
  <@ } @>
</table>
<@ } else { @>
    No Upload Log items.
<@ } @>
</script>
