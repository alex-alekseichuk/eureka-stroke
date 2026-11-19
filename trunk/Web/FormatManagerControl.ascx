<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FormatManagerControl.ascx.cs"
    Inherits="FormatManagerControl" %>
<%@ Import Namespace="Eureka.Stroke"%>
<%@ Register Assembly="System.Web.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
    Namespace="System.Web.UI.WebControls" TagPrefix="asp" %>
<%@ Register TagPrefix="common" Namespace="Common" %>
<%@ Register TagPrefix="EurekaStroke" TagName="GridViewPager" Src="GridViewPager.ascx" %>

<h2>Format Manager</h2>
<form id="Form1" runat="server">
<asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="<%$ AppSettings: AsyncPostBackTimeout %>">
</asp:ScriptManager>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <div style="margin-bottom:10px;">
            <asp:Label id="boxError" runat="server" CssClass="boxError" Style="display:block;"></asp:Label>
            <asp:Label id="boxMessage" runat="server" CssClass="boxMessage" Style="display:block;"></asp:Label>
        </div>

        <script type="text/javascript"><!--
            function requestEndHandler(sender, args) {
                // init all tooltips on the page
                jQuery("a.tooltip, span.tooltip").tooltip({
                    showURL: false,
                    simple: true,
                    track: true,
                    fade: 250
                });
                if (args.get_error()) {
                    jQuery('.boxError').html('Connection Error<br /><br />' + args.get_error().description);
                    args.set_errorHandled(true);
                }
            }
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(requestEndHandler);
        //--></script>

        <div style="float: left;">
            Format:
            <asp:DropDownList ID="ddlFormat" runat="server" DataTextField="Name" DataValueField="ID"
                DataSourceID="dsFormatsList" OnSelectedIndexChanged="ddlFormat_SelectedIndexChanged"
                AutoPostBack="true" OnDataBound="ddlFormat_DataBound" >
            </asp:DropDownList>
            <asp:ObjectDataSource ID="dsFormatsList" runat="server" SelectMethod="GetFormats" OnSelected="ddlFormat_Selected"
                TypeName="FormatManagerDataSource"></asp:ObjectDataSource>
                
            <asp:LinkButton ID="btnDeleteFormat" OnClick="btnDeleteFormat_Click" runat="server" Tooltip="Delete New Format" >
	            <asp:Image runat="server" ImageUrl="~/img/actions/delete.gif" AlternativeText="Delete" />
            </asp:LinkButton>

            <span class="tooltip" title="Here you can shoose Alien Format you are going to map to Eureka Format.<br />You can Add New Alien Format as well.">
                <asp:Image ID="Image1" runat="server" ImageUrl="~/img/actions/info.gif" AlternativeText="Info" />
            </span>
        </div>

        <div style="float:left;margin-left:15px;">
            <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                <ProgressTemplate>
                    <img id="Img1" runat="server" src="~/img/ajax-loader.gif" alt="Uploading..." />
                </ProgressTemplate>
            </asp:UpdateProgress>
        </div>
        <div style="float: right;">
            New Format:
            <asp:TextBox ID="txtNewFormat" runat="server" />

            <asp:LinkButton ID="btnAddFormat" OnClick="btnAddFormat_Click" runat="server" Tooltip="Add New Format" >
	            <asp:Image runat="server" ImageUrl="~/img/actions/add.gif" AlternativeText="Add" />
            </asp:LinkButton>
        </div>
        <div style="clear: both;">
        </div>
        
        <div id="divFields">
            <h3>
                Fields Mapping
                <span class="tooltip" title="There is a grid with all Eureka Fields we save in the database.<br />Choose specified field to provide mapping for it.<br />You can add or modify existing fields.">
                    <asp:Image ID="Image2" runat="server" ImageUrl="~/img/actions/info.gif" AlternativeText="Info" />
                </span>
            </h3>
            <common:HeaderGrid ID="gridFields" runat="server" DataSourceID="dsEurekaFields" AutoGenerateColumns="false"
                AllowPaging="True" PageSize="10"
                CssClass="grid" Style="width:100%;" ShowFooter="true" ShowFooterWhenEmpty="true"
                OnRowDataBound="gridFields_RowDataBound"
                OnSelectedIndexChanged="gridFields_SelectedIndexChanged"
                OnRowEditing="gridFields_RowEditing"
                OnRowDeleting="gridFields_RowDeleting"
                OnRowCreated="gridFields_RowCreated"
                OnDataBound="gridFields_DataBound"
                EmptyDataText="No Eureka Fields." DataKeyNames="ID">
                <SelectedRowStyle BackColor="#FDFFB8" />
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton Text="" CommandName="Select" CausesValidation="false" runat="server" ID="btSelect" />
                            <asp:LinkButton Tooltip="Edit" CommandName="Edit" CausesValidation="false" runat="server" ID="btEdit">
	                            <asp:Image runat="server" ImageUrl="~/img/actions/edit.gif" AlternativeText="Edit" />
                            </asp:LinkButton>
                            <asp:LinkButton Tooltip="Delete" CommandName="Delete" CausesValidation="false" runat="server" ID="btDelete">
	                            <asp:Image runat="server" ImageUrl="~/img/actions/delete.gif" AlternativeText="Delete" />
                            </asp:LinkButton>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:LinkButton Tooltip="Update" CommandName="Update" CausesValidation="true" runat="server" ID="btUpdate" >
	                            <asp:Image runat="server" ImageUrl="~/img/actions/update.gif" AlternativeText="Update" />
                            </asp:LinkButton>
                            <asp:LinkButton Tooltip="Cancel" CommandName="Cancel" CausesValidation="false" runat="server" ID="btCancel" >
	                            <asp:Image runat="server" ImageUrl="~/img/actions/cancel.gif" AlternativeText="Cancel" />
                            </asp:LinkButton>
                        </EditItemTemplate>
                        <FooterTemplate>
                            <asp:LinkButton Tooltip="Add" CommandName="Insert" CausesValidation="true" runat="server" ID="btInsert" OnClick="btInsert_Click">
	                            <asp:Image runat="server" ImageUrl="~/img/actions/add.gif" AlternativeText="Add" />
                            </asp:LinkButton>
                        </FooterTemplate>
                    </asp:TemplateField>                            
                    <asp:TemplateField HeaderText="Type">
                        <ItemTemplate>
                            <%# FieldType.Types[(short)Eval("FieldType")]%>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList ID="ddlTypeUpdate" runat="server" 
                                DataSourceID="dsFieldTypesList" DataTextField="Value" DataValueField="Key"
                                SelectedValue='<%# Bind("FieldType") %>'></asp:DropDownList>
                            <asp:Label ID="labelTypeUpdate" runat="server" />
                        </EditItemTemplate>
                        <FooterTemplate>
                            <asp:DropDownList ID="ddlTypeInsert" runat="server" 
                                DataSourceID="dsFieldTypesList" DataTextField="Value" DataValueField="Key"></asp:DropDownList>
                        </FooterTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>Eureka Field</HeaderTemplate>
                        <ItemTemplate>
                           <asp:Label ID="lblEurekaFieldNameValue" Text='<%# Eval("Name") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtEurekaFieldNameUpdate" runat="server" Text='<%# Bind("Name") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <FooterTemplate>
                              <asp:TextBox ID="txtEurekaFieldNameInsert" runat="server" Text="" ></asp:TextBox>
                        </FooterTemplate>
                    </asp:TemplateField>                        
                    <asp:TemplateField>
                        <HeaderTemplate>Eureka Field Description</HeaderTemplate>
                        <ItemTemplate>
                           <asp:Label ID="lblEurekaFieldDescriptionValue" Text='<%# Utils.Truncate((string)Eval("Description"), 100) %>' ToolTip='<%# Eval("Description") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                              <asp:TextBox ID="txtEurekaFieldDescriptionUpdate" runat="server" Text='<%# Bind("Description") %>' TextMode="MultiLine" CssClass="eureka_desc"></asp:TextBox>
                        </EditItemTemplate>
                        <FooterTemplate>
                              <asp:TextBox ID="txtEurekaFieldDescriptionInsert" runat="server" Text="" TextMode="MultiLine" CssClass="eureka_desc"></asp:TextBox>
                        </FooterTemplate>
                    </asp:TemplateField>                        
                </Columns>
                <PagerTemplate>
                    <EurekaStroke:GridViewPager runat="server" />
                </PagerTemplate>
            </common:HeaderGrid>
            <asp:ObjectDataSource ID="dsEurekaFields" runat="server" TypeName="FormatManagerDataSource"
                SelectMethod="GetEurekaFields" OnSelected="gridFields_Selected"
                DeleteMethod="DeleteEurekaField" OnDeleted="gridFields_RowDeleted"
                UpdateMethod="UpdateEurekaField" OnUpdating="gridFields_RowUpdating" OnUpdated="gridFields_RowUpdated"
                InsertMethod="InsertEurekaField" OnInserting="gridFields_RowInserting" OnInserted="gridFields_RowInserted">
                <SelectParameters>
                    <asp:ControlParameter ControlID="ddlFormat" DbType="Int32" DefaultValue="0" Name="formatID"
                        PropertyName="SelectedValue" />
                </SelectParameters>
                <InsertParameters>
                    <asp:Parameter Name="Type" Type="Int16" />
                    <asp:Parameter Name="Name" Type="String" />
                    <asp:Parameter Name="Description" Type="String" />
                    <asp:Parameter Name="RowIndex" Type="Int32" Direction="Output" />
                </InsertParameters>
            </asp:ObjectDataSource>
            <asp:ObjectDataSource ID="dsFieldTypesList" runat="server" SelectMethod="GetFieldTypes" 
                TypeName="FormatManagerDataSource"></asp:ObjectDataSource>
        </div>
        
        <asp:Panel id="panelFieldDetails" runat="server" Visible="false" style="margin-top: 20px;">
            <div id="divAlienFields" style="float:left;">
                <h3>
                    Alien Fields
                    <span class="tooltip" title="Eureka Field may be mapped to one or several Alien Format Fields.">
                        <asp:Image ID="Image3" runat="server" ImageUrl="~/img/actions/info.gif" AlternativeText="Info" />
                    </span>
                </h3>
                <common:HeaderGrid ID="gridAlienFields" runat="server"
                    DataSourceID="dsAlienFields" AutoGenerateColumns="false" DataKeyNames="ID"
                    CssClass="grid" ShowHeaderWhenEmpty="true" ShowFooter="true" ShowFooterWhenEmpty="true"
                    OnRowCreated="gridAlienFields_RowCreated"
                    OnRowDataBound="gridAlienFields_RowDataBound"
                    OnRowEditing="gridAlienFields_RowEditing"
                    EmptyDataText="No alien fields mapped.">
                    <Columns>
                        <asp:TemplateField ItemStyle-Width="50">
                            <ItemTemplate>
                                <asp:LinkButton Tooltip="Edit" CommandName="Edit" CausesValidation="false" runat="server" ID="btEdit">
	                                <asp:Image runat="server" ImageUrl="~/img/actions/edit.gif" AlternativeText="Edit" />
                                </asp:LinkButton>
                                <asp:LinkButton Tooltip="Delete" CommandName="Delete" CausesValidation="false" runat="server" ID="btDelete">
	                                <asp:Image runat="server" ImageUrl="~/img/actions/delete.gif" AlternativeText="Delete" />
                                </asp:LinkButton>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:LinkButton Tooltip="Update" CommandName="Update" CausesValidation="true" runat="server" ID="btUpdate" >
	                                <asp:Image runat="server" ImageUrl="~/img/actions/update.gif" AlternativeText="Update" />
                                </asp:LinkButton>
                                <asp:LinkButton Tooltip="Cancel" CommandName="Cancel" CausesValidation="false" runat="server" ID="btCancel" >
	                                <asp:Image runat="server" ImageUrl="~/img/actions/cancel.gif" AlternativeText="Cancel" />
                                </asp:LinkButton>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton Tooltip="Add" CommandName="Insert" CausesValidation="true" runat="server" ID="btInsert" OnClick="btInsertAlien_Click">
	                                <asp:Image runat="server" ImageUrl="~/img/actions/add.gif" AlternativeText="Add" />
                                </asp:LinkButton>
                            </FooterTemplate>
                        </asp:TemplateField>                            
                        <asp:TemplateField>
                            <HeaderTemplate>Alien Field</HeaderTemplate>
                            <ItemTemplate>
                               <asp:Label ID="lblAlienFieldValue" Text='<%# Eval("Name") %>' runat="server"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                  <asp:TextBox ID="txtAlienFieldUpdate" runat="server" CssClass="small_grid" Text='<%# Bind("Name") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                  <asp:TextBox ID="txtAlienFieldInsert" runat="server" CssClass="small_grid" Text="" ></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>                        
                        <asp:TemplateField>
                            <HeaderTemplate>Regexp</HeaderTemplate>
                            <ItemTemplate>
                               <asp:Label ID="lblRegexpValue" Text='<%# Eval("RegularExpression") %>' runat="server"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                  <asp:TextBox ID="txtRegexpUpdate" runat="server" CssClass="small_grid" Text='<%# Bind("RegularExpression") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                  <asp:TextBox ID="txtRegexpInsert" runat="server" CssClass="small_grid" Text="" ></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>                        
                    </Columns>
                </common:HeaderGrid>
            </div>
            <asp:ObjectDataSource ID="dsAlienFields" runat="server"
                SelectMethod="GetAlienFields" OnSelected="gridAlienFields_Selected"
                DeleteMethod="DeleteAlienField" OnDeleted="gridAlienFields_RowDeleted"
                UpdateMethod="UpdateAlienField" OnUpdating="gridAlienFields_RowUpdating" OnUpdated="gridAlienFields_RowUpdated"
                InsertMethod="InsertAlienField" OnInserting="gridAlienFields_RowInserting" OnInserted="gridAlienFields_RowInserted"
                TypeName="FormatManagerDataSource">
                <SelectParameters>
                    <asp:ControlParameter ControlID="ddlFormat" DbType="Int32" DefaultValue="0" Name="formatID"
                        PropertyName="SelectedValue" />
                    <asp:ControlParameter ControlID="gridFields" DbType="Int32" Name="eurekaFieldID" DefaultValue="0" PropertyName="SelectedValue" />
                </SelectParameters>
                <InsertParameters>
                    <asp:ControlParameter ControlID="ddlFormat" DbType="Int32" DefaultValue="0" Name="formatID"
                        PropertyName="SelectedValue" />
                    <asp:ControlParameter ControlID="gridFields" DbType="Int32" Name="eurekaFieldID" DefaultValue="0" PropertyName="SelectedValue" />
                    <asp:Parameter Name="Name" Type="String" />
                    <asp:Parameter Name="RegularExpression" Type="String" />
                </InsertParameters>
            </asp:ObjectDataSource>

            <div id="divValues" style="float:right;">
                <h3>
                    Values Mapping
                    <span class="tooltip" title="Here you can customize mapping for specified values">
                        <asp:Image ID="Image4" runat="server" ImageUrl="~/img/actions/info.gif" AlternativeText="Info" />
                    </span>
                </h3>
                <common:HeaderGrid ID="gridValues" runat="server"
                    DataSourceID="dsValuesList" AutoGenerateColumns="false" DataKeyNames="EurekaID,AlienID"
                    CssClass="grid" ShowHeaderWhenEmpty="true" ShowFooter="true" ShowFooterWhenEmpty="true"
                    OnRowCreated="gridValues_RowCreated"
                    OnRowDataBound="gridValues_RowDataBound"
                    OnRowEditing="gridValues_RowEditing"
                    EmptyDataText="No values mapping.">
                    <Columns>
                        <asp:TemplateField ItemStyle-Width="50">
                            <ItemTemplate>
                                <asp:LinkButton Tooltip="Edit" CommandName="Edit" CausesValidation="false" runat="server" ID="btEdit">
	                                <asp:Image runat="server" ImageUrl="~/img/actions/edit.gif" AlternativeText="Edit" />
                                </asp:LinkButton>
                                <asp:LinkButton Tooltip="Delete" CommandName="Delete" CausesValidation="false" runat="server" ID="btDelete">
	                                <asp:Image runat="server" ImageUrl="~/img/actions/delete.gif" AlternativeText="Delete" />
                                </asp:LinkButton>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:LinkButton Tooltip="Update" CommandName="Update" CausesValidation="true" runat="server" ID="btUpdate" >
	                                <asp:Image runat="server" ImageUrl="~/img/actions/update.gif" AlternativeText="Update" />
                                </asp:LinkButton>
                                <asp:LinkButton Tooltip="Cancel" CommandName="Cancel" CausesValidation="false" runat="server" ID="btCancel" >
	                                <asp:Image runat="server" ImageUrl="~/img/actions/cancel.gif" AlternativeText="Cancel" />
                                </asp:LinkButton>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton Tooltip="Add" CommandName="Insert" CausesValidation="true" runat="server" ID="btInsert" OnClick="btInsertValue_Click">
	                                <asp:Image runat="server" ImageUrl="~/img/actions/add.gif" AlternativeText="Add" />
                                </asp:LinkButton>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>Eureka Value</HeaderTemplate>
                            <ItemTemplate>
                               <asp:Label ID="lblEurekaValueValue" Text='<%# Eval("EurekaValue") %>' runat="server"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                  <asp:TextBox ID="txtEurekaValueUpdate" runat="server" CssClass="small_grid" Text='<%# Bind("EurekaValue") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                  <asp:TextBox ID="txtEurekaValueInsert" runat="server" CssClass="small_grid" Text="" ></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>Alien Value</HeaderTemplate>
                            <ItemTemplate>
                               <asp:Label ID="lblAlienValueValue" Text='<%# Eval("AlienValue") %>' runat="server"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                  <asp:TextBox ID="txtAlienValueUpdate" runat="server" CssClass="small_grid" Text='<%# Bind("AlienValue") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                  <asp:TextBox ID="txtAlienValueInsert" runat="server" CssClass="small_grid" Text="" ></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>                        
                        <asp:TemplateField>
                            <HeaderTemplate>Eureka Description</HeaderTemplate>
                            <ItemTemplate>
                               <asp:Label ID="lblEurekaDescriptionValue" Text='<%# Utils.Truncate((string)Eval("EurekaDescription"), 20) %>' ToolTip='<%# Eval("EurekaDescription") %>' runat="server"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                  <asp:TextBox ID="txtEurekaDescriptionUpdate" runat="server" CssClass="small_grid" Text='<%# Bind("EurekaDescription") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                  <asp:TextBox ID="txtEurekaDescriptionInsert" runat="server" CssClass="small_grid" Text="" ></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>                        
                        <asp:TemplateField>
                            <HeaderTemplate>Alien Definition</HeaderTemplate>
                            <ItemTemplate>
                               <asp:Label ID="lblAlienDescriptionValue" Text='<%# Utils.Truncate((string)Eval("AlienDescription"), 20) %>' ToolTip='<%# Eval("AlienDescription") %>' runat="server"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                  <asp:TextBox ID="txtAlienDescriptionUpdate" runat="server" CssClass="small_grid" Text='<%# Bind("AlienDescription") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                  <asp:TextBox ID="txtAlienDescriptionInsert" runat="server" CssClass="small_grid" Text="" ></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>                        
                    </Columns>
                </common:HeaderGrid>
            </div>
            <asp:ObjectDataSource ID="dsValuesList" runat="server"
                SelectMethod="GetEurekaValues" OnSelected="gridValues_Selected"
                DeleteMethod="DeleteEurekaValue" OnDeleted="gridValues_RowDeleted"
                UpdateMethod="UpdateEurekaValue" OnUpdating="gridValues_RowUpdating" OnUpdated="gridValues_RowUpdated"
                InsertMethod="InsertEurekaValue" OnInserting="gridValues_RowInserting" OnInserted="gridValues_RowInserted"
                TypeName="FormatManagerDataSource">
                <SelectParameters>
                    <asp:ControlParameter ControlID="ddlFormat" DbType="Int32" DefaultValue="0" Name="formatID"
                        PropertyName="SelectedValue" />
                    <asp:ControlParameter ControlID="gridFields" DbType="Int32" Name="eurekaFieldID" DefaultValue="0" PropertyName="SelectedValue" />
                </SelectParameters>
                <InsertParameters>
                    <asp:ControlParameter ControlID="ddlFormat" DbType="Int32" DefaultValue="0" Name="formatID"
                        PropertyName="SelectedValue" />
                    <asp:ControlParameter ControlID="gridFields" DbType="Int32" Name="eurekaFieldID" DefaultValue="0" PropertyName="SelectedValue" />
                    <asp:Parameter Name="EurekaValue" Type="String" />
                    <asp:Parameter Name="AlienValue" Type="String" />
                    <asp:Parameter Name="EurekaDescription" Type="String" />
                    <asp:Parameter Name="AlienDescription" Type="String" />
                </InsertParameters>
                <UpdateParameters>
                    <asp:ControlParameter ControlID="ddlFormat" DbType="Int32" DefaultValue="0" Name="formatID"
                        PropertyName="SelectedValue" />
                    <asp:ControlParameter ControlID="gridFields" DbType="Int32" Name="eurekaFieldID" DefaultValue="0" PropertyName="SelectedValue" />
                </UpdateParameters>
            </asp:ObjectDataSource>

            <div style="clear:both;"></div>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
</form>
