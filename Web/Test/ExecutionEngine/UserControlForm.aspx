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

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title></title>
</head>
<body>
  <form id="TheForm" runat="server">
  <div>
    <a href="ShowUserControl.wxe">Restart</a>
    <p>
      Last postback on page: <asp:Label ID="PageLabel" runat="server" /><br />
      <remotion:WebButton ID="PageButton" runat="server" Text="Postback to Page" onclick="PageButton_Click"/>
    </p>
  </div>
  </form>
</body>
</html>
