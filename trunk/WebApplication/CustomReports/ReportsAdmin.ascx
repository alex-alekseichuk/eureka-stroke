<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportsAdmin.ascx.cs" Inherits="WebApplication.CustomReports.ReportsAdmin" %>
<%@ Import Namespace="WebApplication" %>
<form runat="server">
<div style="float:left;" class="Area">
    <asp:ListView ID="listFolders" runat="server" DataSourceID="dsFolders" DataKeyNames="ID"
        InsertItemPosition="LastItem"
        OnSelectedIndexChanged="listFolders_SelectedIndexChanged">
        <EmptyDataTemplate>
            No Categories.
        </EmptyDataTemplate>
        <ItemTemplate>
            <tr class="Item">
                <td>
                    <asp:ImageButton ID="btnSelect" CommandName="Select" ImageUrl="~/img/actions/folder.gif" AlternateText="Select" runat="server" ToolTip="Select Category" />
                    <% if(Util.IsAdmin()){%>
                    <asp:ImageButton ID="btnEdit" CommandName="Edit" ImageUrl="~/img/actions/edit.gif" AlternateText="Edit" runat="server" ToolTip="Edit Category" />
                    <asp:ImageButton ID="btnDelete" CommandName="Delete" ImageUrl="~/img/actions/delete.gif" AlternateText="Delete" runat="server" OnClientClick="return confirm('Are you sure to delete this folder and all its reports?');" ToolTip="Delete Category" />
                    <% } %>
                </td>
                <td>
                    <% if (Util.IsAdmin())
                       {%>
                    <asp:CheckBox runat="server" ID="chkEnabled" Checked='<%# Eval("Enabled") %>' Enabled="false" ToolTip="Active" />
                    <%}%>
                    <asp:LinkButton runat="server" ID="btnSelect2" CommandName="Select">
                        <asp:Label runat="server" ID="lblName" Text='<%# Eval("Name") %>'></asp:Label>
                    </asp:LinkButton>
                </td>
            </tr>
        </ItemTemplate>
        <SelectedItemTemplate>
            <tr class="Select">
                <td>
                    <asp:ImageButton ID="btnSelect" CommandName="Select" ImageUrl="~/img/actions/folder.gif" AlternateText="Select" runat="server" ToolTip="Select Category" />
                    <% if (Util.IsAdmin()){%>
                    <asp:ImageButton ID="btnEdit" CommandName="Edit" ImageUrl="~/img/actions/edit.gif" AlternateText="Edit" runat="server" ToolTip="Edit Category" />
                    <asp:ImageButton ID="btnDelete" CommandName="Delete" ImageUrl="~/img/actions/delete.gif" AlternateText="Delete" runat="server" OnClientClick="return confirm('Are you sure to delete this folder and all its reports?');" ToolTip="Delete Category" />
                    <%}%>
                </td>
                <td>
                    <% if (Util.IsAdmin()){%>
                    <asp:CheckBox runat="server" ID="chkEnabled" Checked='<%# Eval("Enabled") %>' Enabled="false"  ToolTip="Active" />
                    <%}%>
                    <asp:Label runat="server" ID="lblName" Text='<%# Eval("Name") %>'></asp:Label>
                </td>
                
            </tr>
        </SelectedItemTemplate>
        <EditItemTemplate>
            <tr class="Select">
                <td>
                    <asp:ImageButton ID="btnUpdate" CommandName="Update" ImageUrl="~/img/actions/update.gif" AlternateText="Update" runat="server" ToolTip="Update Category" />
                    <asp:ImageButton ID="btnCancel" CommandName="Cancel" ImageUrl="~/img/actions/cancel.gif" AlternateText="Cancel" runat="server" ToolTip="Cancel" />
                </td>
                <td>
                    <asp:CheckBox runat="server" ID="chkEnabled" Checked='<%# Bind("Enabled") %>'  ToolTip="Active" />
                    <asp:TextBox ID="txtName" runat="server" Text='<%# Bind("Name") %>'></asp:TextBox>
                </td>
            </tr>
        </EditItemTemplate>
        <InsertItemTemplate>
            <tr class="Item">
                <td>
                    <asp:ImageButton ID="btnInsert" CommandName="Insert" ImageUrl="~/img/actions/add.gif" AlternateText="Add" runat="server" ToolTip="Add Category" />
                </td>
                <td>
                    <asp:CheckBox runat="server" ID="chkEnabled" Checked='<%# Bind("Enabled") %>' ToolTip="Active" />
                    <asp:TextBox ID="txtName" runat="server" Text='<%# Bind("Name") %>'></asp:TextBox>
                </td>
            </tr>
        </InsertItemTemplate>
        <LayoutTemplate>
            <h3 style="float:left;">Categories</h3>
            <div style="float:right;">
                <asp:Button ID="btnSort" runat="server" Text="Sort" OnClick="btnSort_Click" />
            </div>
            <div style="clear:both;"></div>
            <table class="Items">
            <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
            </table>
        </LayoutTemplate>
    </asp:ListView>
    
    <asp:ObjectDataSource ID="dsFolders" runat="server"
        TypeName="WebApplication.CustomReports.FoldersDataSource" 
        SelectMethod="GetFolders"
        UpdateMethod="UpdateFolder"
        InsertMethod="InsertFolder"
        DeleteMethod="DeleteFolder">
    </asp:ObjectDataSource>
</div>

<asp:PlaceHolder id="phReports" runat="server" Visible="false">
<div style="float:left;">
    <div class="Area" id="divDownloadTemplate" runat="server">
        <a href='Download.aspx' ><img id="Img1" src="~/img/actions/download.gif" runat="server" /> Download Template</a>
    </div>
    <div class="Area">
    <asp:ListView ID="listReports" runat="server" DataSourceID="dsReports" DataKeyNames="ID" InsertItemPosition="LastItem">
        <EmptyDataTemplate>
            No Reports
        </EmptyDataTemplate>
        <ItemTemplate>
            <tr><td>
            <div class="Item">
                <div style='float:left;width:<%=Util.IsAdmin()?60:20%>px;'>
                    <a href='ViewReport.aspx?reportID=<%# Eval("ID")%>' title="View Report"><img src="~/img/actions/report.gif" runat="server" alt="View" title="View Report" /></a>
                    <% if (Util.IsAdmin())
                       {%>
                    <a href='Download.aspx?reportID=<%# Eval("ID")%>' title="Download .rdl file" ><img src="~/img/actions/download.gif" runat="server" alt=".rdl file" title="Download .rdl file" /></a>
                    <asp:ImageButton ID="btnDelete" CommandName="Delete" ImageUrl="~/img/actions/delete.gif" AlternateText="Delete" runat="server" OnClientClick="return confirm('Are you sure to delete this report?');" ToolTip="Delete Report" />
                    <%
                       }%>
                </div>
                <div style="float:left;">
                    <% if (Util.IsAdmin())
                       {%>
                    <asp:CheckBox runat="server" ID="chkEnabled" Checked='<%# Eval("Enabled") %>' Enabled="false" ToolTip="Active" />
                    <%
                       }%>
                </div>
                <div style="float:left;">
                    <% if (Util.IsAdmin())
                       {%>
                    <asp:LinkButton runat="server" ID="btnEdit" CommandName="Edit">
                        <asp:Label runat="server" ID="lblName" Text='<%# Eval("Name") %>' ToolTip='<%# Eval("Description") %>'></asp:Label>
                    </asp:LinkButton>
                    <%
                       } else {%>
                       <a href='ViewReport.aspx?reportID=<%# Eval("ID")%>' ><asp:Label runat="server" ID="Label1" Text='<%# Eval("Name") %>' ToolTip='<%# Eval("Description") %>'></asp:Label></a>
                    <%
                       }%>
                </div>
                <div style="clear:both;"></div>
            </div>
            </td></tr>
        </ItemTemplate>
        <InsertItemTemplate>
            <tr><td>
            <div class="Item" style="height:140px">
                <div style="float:left;width:60px;">
                    <asp:ImageButton ID="btnInsert" CommandName="Insert" ImageUrl="~/img/actions/add.gif" AlternateText="Add" runat="server" ImageAlign="AbsMiddle" ToolTip="Add Report" />
                </div>
                <div style="float:left;">
                    <table>
                        <tr>
                            <td style="width:100px;">Report Name:</td>
                            <td><asp:TextBox ID="txtName" runat="server" Text='<%# Bind("Name") %>' Width="300"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td><asp:CheckBox runat="server" ID="chkEnabled" Checked='<%# Bind("Enabled") %>' ToolTip="Active" Text="Active" /></td>
                        </tr>
                        <tr>
                            <td style="vertical-align:middle">Description:</td>
                            <td>
                                <asp:TextBox ID="txtDescription" runat="server" Text='<%# Bind("Description") %>' TextMode="MultiLine" Width="300" Height="50"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>.rdl file:</td>
                            <td>
                                <asp:FileUpload ID="fileInsertReport" runat="server" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div style="clear:both;"></div>
            </div>
            </td></tr>
        </InsertItemTemplate>
        <EditItemTemplate>
            <tr><td>
            <div class="ItemSelect" style="height:140px">
                <div style="float:left;width:60px;">
                    <asp:ImageButton ID="btnUpdate" CommandName="Update" ImageUrl="~/img/actions/update.gif" AlternateText="Update" runat="server" ImageAlign="AbsMiddle" ToolTip="Update Report" />
                    <asp:ImageButton ID="btnCancel" CommandName="Cancel" ImageUrl="~/img/actions/cancel.gif" AlternateText="Cancel" runat="server" ImageAlign="AbsMiddle" ToolTip="Cancel" />
                </div>
                <div style="float:left;">
                
                    <table>
                        <tr>
                            <td style="width:100px;">Report Name:</td>
                            <td><asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Name") %>' Width="300"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td><asp:CheckBox runat="server" ID="CheckBox1" Checked='<%# Bind("Enabled") %>' ToolTip="Active" Text="Active" /></td>
                        </tr>
                        <tr>
                            <td style="vertical-align:middle">Description:</td>
                            <td>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Description") %>' TextMode="MultiLine" Width="300" Height="50"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>.rdl file:</td>
                            <td>
                                <asp:FileUpload ID="fileUpdateReport" runat="server" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div style="clear:both;"></div>
            </div>
            </td></tr>
        </EditItemTemplate>
        <LayoutTemplate>
            <h3>Reports</h3>
            <table>
                <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
            </table>
        </LayoutTemplate>
    </asp:ListView>
    </div>
    <asp:ObjectDataSource ID="dsReports" runat="server"
        TypeName="WebApplication.CustomReports.ReportsDataSource" 
        SelectMethod="GetReports"
        InsertMethod="InsertReport" OnInserting="listReports_Inserting"
        UpdateMethod="UpdateReport" OnUpdating="listReports_Updating"
        DeleteMethod="DeleteReport">
        <SelectParameters>
            <asp:ControlParameter Name="folderID" Type="Int32" DefaultValue="0" ControlID="listFolders" PropertyName="SelectedDataKey.Values[&quot;ID&quot;]" />
        </SelectParameters>
        <InsertParameters>
            <asp:ControlParameter Name="folderID" Type="Int32" DefaultValue="0" ControlID="listFolders" PropertyName="SelectedDataKey.Values[&quot;ID&quot;]" />
            <asp:Parameter Name="tempPath" Type="String" DefaultValue="" />
        </InsertParameters>
        <UpdateParameters>
            <asp:Parameter Name="tempPath" Type="String" DefaultValue="" />
        </UpdateParameters>
    </asp:ObjectDataSource>
</div>
</asp:PlaceHolder>

<div style="clear:both;"></div>

</form>
