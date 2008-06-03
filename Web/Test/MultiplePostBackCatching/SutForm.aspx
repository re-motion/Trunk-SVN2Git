<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>
<%@ Page language="c#" Codebehind="SutForm.aspx.cs" AutoEventWireup="True" Inherits="Remotion.Web.Test.MultiplePostBackCatching.SutForm" smartNavigation="False"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
  <head id="Head" runat="server">
    <title>MultiplePostbackCatcherForm</title>
    <remotion:HtmlHeadContents ID="HtmlHeadContents" runat="server" />
<script type="text/javascript">
  function ChangeAutoPostbackListSelection()
  {
    document.MyForm.AutoPostbackList.fireEvent ('onChange','');
  } 
</script>
</head>
<body>
<form id="MyForm" runat="server">
<asp:PlaceHolder ID="SutPlaceHolder" runat="server" />
<table style="WIDTH: 100%; HEIGHT: 100%">
  <tr>
    <td style="VERTICAL-ALIGN: top">
<a href="mpc.wxe?Parameter=Garbage;">Hyperlink</a><br/>
<input type="text" onkeyup="ChangeAutoPostbackListSelection(); return false;" />
<input type="button" value="Select value" onclick="ChangeAutoPostbackListSelection();  return false;" />
      <asp:DropDownList ID="AutoPostbackList" runat="server" AutoPostBack="True" EnableViewState="false">
        <asp:ListItem Value="1" Text="1" Selected="True" />
        <asp:ListItem Value="2" Text="2" />
        <asp:ListItem Value="3" Text="3" />
      </asp:DropDownList></td></tr></table>
</form>
  </body>
</html>
