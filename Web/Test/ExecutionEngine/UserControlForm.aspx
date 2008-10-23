<%@ Register Src="FirstControl.ascx" TagName="FirstControl" TagPrefix="uc1" %>
<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserControlForm.aspx.cs" Inherits="Remotion.Web.Test.ExecutionEngine.UserControlForm" %>

<%@ Register TagPrefix="webTest" TagName="FirstControl" Src="FirstControl.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title></title>
  <style>
    div
    {
      border: solid 1px black;
      margin-bottom: .5em;
      padding: .5em;
    }
  </style>
</head>
<body>
  <form id="TheForm" runat="server">
  <a href="ShowUserControl.wxe">Restart</a>
  <div>
    Page
    <p>
      Last postback on page:
      <asp:Label ID="PageLabel" runat="server" /><br />
      <remotion:WebButton ID="PageButton" runat="server" Text="Postback to Page" OnClick="PageButton_Click" />
      <remotion:WebButton ID="ExecuteSecondUserControlButton" runat="server" Text="Execute Second User Control" OnClick="ExecuteSecondUserControlButton_Click" />
      <br />
      ViewState:
      <asp:Label ID="ViewStateLabel" runat="server" /><br />
      ControlState:
      <asp:Label ID="ControlStateLabel" runat="server" />
    </p>
  </div>
  <asp:PlaceHolder ID="FirstControlPlaceHoder" runat="server" />
  <webTest:FirstControl ID="FirstControl" runat="server" />
  <div>
    Stack:
    <p>
      <asp:Label ID="StackLabel" runat="server" />
    </p>
  </div>
  </form>
</body>
</html>
