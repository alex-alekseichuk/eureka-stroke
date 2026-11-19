<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FormatManager.aspx.cs" Inherits="WebApplication.FormatManager" %>

<%@ Register TagPrefix="EurekaStroke" TagName="FormatManagerControl" Src="FormatManagerControl.ascx" %>
<%@ Register TagPrefix="EurekaStroke" TagName="Header" Src="Header.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>FMS - Eureka Stroke</title>

    <link href="<%= ResolveUrl("~/img/main.css") %>" rel="stylesheet" type="text/css"/>
    
<!--[if lte IE 7]>  
<link rel="stylesheet" type="text/css" href="<%= ResolveUrl("~/img/ie6.css") %>" />  
<![endif]-->  
    
    <script type="text/javascript" src="<%= ResolveUrl("~/js/tmpl.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/js/jquery-1.4.2.min.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/js/jquery.tooltip.pack.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/js/init.js") %>"></script>
</head>
<body>
    <EurekaStroke:Header ID="Header1" Runat="server" />
    <div>
        <EurekaStroke:FormatManagerControl ID="FormatManagerControl" Runat="server" />
    </div>
</body>
</html>
