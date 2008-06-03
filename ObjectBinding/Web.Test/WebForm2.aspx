<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>
<%@ Page language="c#" Codebehind="WebForm2.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.WebForm2" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <title>WebForm1</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
  </HEAD>
  <body MS_POSITIONING="GridLayout">
    <form id="Form1" method="post" runat="server">
      <remotion:BocTextValue id=DateOfBirthField style="Z-INDEX: 106; LEFT: 376px; POSITION: absolute; TOP: 192px" runat="server" 
        PropertyIdentifier="DateOfBirth" DataSource="<%# reflectionBusinessObjectDataSource1 %>">
      </remotion:BocTextValue>
      <remotion:SmartLabel id="BocPropertyLabel3" style="Z-INDEX: 107; LEFT: 64px; POSITION: absolute; TOP: 192px"
        runat="server" ForControl="DateOfBirthField"></remotion:SmartLabel>
      <cc1:BocTextValueValidator id="BocTextValueValidator1" style="Z-INDEX: 108; LEFT: 584px; POSITION: absolute; TOP: 200px"
        runat="server" ControlToValidate="DateOfBirthField"></cc1:BocTextValueValidator>
      <asp:button id="SaveButton" style="Z-INDEX: 103; LEFT: 48px; POSITION: absolute; TOP: 400px"
        runat="server" Width="80px" Text="Save"></asp:button>
    </form>
  </body>
</HTML>
