<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>




<%@ Page language="c#" Codebehind="CompleteBocTestForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.CompleteBocForm" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>CompleteBocTest: Form, No UserControl</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><remotion:htmlheadcontents id=HtmlHeadContents runat="server"></remotion:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server">
<h1>CompleteBocTest: Form, No UserControl</h1>
<p>
<table id=FormGrid runat="server">
  <tr>
    <td colSpan=2><remotion:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" datasourcecontrol="CurrentObject" ReadOnly="True"></remotion:boctextvalue>&nbsp;<remotion:boctextvalue id=LastNameField runat="server" PropertyIdentifier="LastName" datasourcecontrol="CurrentObject" ReadOnly="True"></remotion:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:boctextvalue id=TextField runat="server" datasourcecontrol="CurrentObject" propertyidentifier="FirstName" errormessage="Fehler">
<textboxstyle textmode="SingleLine" autopostback="True">
</TextBoxStyle></remotion:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocmultilinetextvalue id=MultilineTextField runat="server" datasourcecontrol="CurrentObject" propertyidentifier="CV" DESIGNTIMEDRAGDROP="37" errormessage="Fehler">
<textboxstyle textmode="MultiLine" autopostback="True">
</TextBoxStyle></remotion:bocmultilinetextvalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocdatetimevalue id=DateTimeField runat="server" datasourcecontrol="CurrentObject" propertyidentifier="DateOfBirth" errormessage="Fehler">
<datetextboxstyle autopostback="True">
</DateTextBoxStyle></remotion:bocdatetimevalue></td></tr>
  <tr>
    <td style="HEIGHT: 18px"></td>
    <td style="HEIGHT: 18px"><remotion:bocenumvalue id=EnumField runat="server" datasourcecontrol="CurrentObject" propertyidentifier="MarriageStatus" errormessage="Fehler">
<listcontrolstyle autopostback="True">
</ListControlStyle></remotion:bocenumvalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocreferencevalue id=ReferenceField runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Partner" errormessage="Fehler">
<dropdownliststyle autopostback="True">
</DropDownListStyle>

<persistedcommand>
<remotion:BocCommand Type="None"></remotion:BocCommand>
</PersistedCommand></remotion:bocreferencevalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocbooleanvalue id=BooleanField runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Deceased" errormessage="Fehler" autopostback="True"></remotion:bocbooleanvalue></td></tr>
  <tr>
    <td></td>
    <td></td></tr>
  <tr>
    <td colSpan=2><remotion:boclist id=ListField runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Jobs" showsortingorder="True" alwaysshowpageinfo="True" enableselection="True">
<fixedcolumns>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="Title">
<persistedcommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="StartDate">
<persistedcommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
</FixedColumns></remotion:boclist></td></tr></table></p>
<p><remotion:formgridmanager id=FormGridManager runat="server" 
visible="true"></remotion:formgridmanager><remotion:BindableObjectDataSourceControl 
id=CurrentObject runat="server" 
Type="Remotion.ObjectBinding.Sample::Person" /></p>
<p><asp:button id=SaveButton runat="server" Text="Save" Width="80px"></asp:button><asp:button id=PostBackButton runat="server" Text="Post Back"></asp:button></p></form>
  </body>
</html>
