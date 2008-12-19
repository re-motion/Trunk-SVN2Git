<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>


<%@ Page language="c#" Codebehind="PersonsForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.PersonsForm" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>Persons Form</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
    <meta content=C# name=CODE_LANGUAGE>
    <meta content=JavaScript name=vs_defaultClientScript>
    <meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
    <remotion:htmlheadcontents runat="server" id="HtmlHeadContents"></remotion:htmlheadcontents>
  </head>
  <body>
    <form id=Form method=post runat="server"><h1>Persons Form</h1>
      <table id=FormGrid runat="server">
        <tr>
          <td colSpan=2>Persons</td></tr>
        <tr>
          <td></td>
          <td><remotion:BocList id="PersonList" runat="server" PropertyIdentifier="" DataSourceControl="CurrentObject" ShowAllProperties="True" >
<fixedcolumns>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="DisplayName">
<persistedcommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
</FixedColumns></remotion:BocList></td></tr>
          </table>
      <p><asp:button id="PostBackButton" runat="server" Text="Post Back"></asp:button></p>
      <p><remotion:formgridmanager id=FormGridManager runat="server" visible="true"></remotion:formgridmanager><remotion:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" /></p></form>

  </body>
</html>
