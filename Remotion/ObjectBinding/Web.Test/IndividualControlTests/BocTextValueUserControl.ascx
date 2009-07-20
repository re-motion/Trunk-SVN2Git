<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
 % Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
 %
 % The re-motion Core Framework is free software; you can redistribute it 
 % and/or modify it under the terms of the GNU Lesser General Public License 
 % version 3.0 as published by the Free Software Foundation.
 %
 % re-motion is distributed in the hope that it will be useful, 
 % but WITHOUT ANY WARRANTY; without even the implied warranty of 
 % MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 % GNU Lesser General Public License for more details.
 %
 % You should have received a copy of the GNU Lesser General Public License
 % along with re-motion; if not, see http://www.gnu.org/licenses.
--%>



<%@ Control Language="c#" AutoEventWireup="false" Codebehind="BocTextValueUserControl.ascx.cs" Inherits="OBWTest.IndividualControlTests.BocTextValueUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<div style="BORDER-RIGHT: black thin solid; BORDER-TOP: black thin solid; BORDER-LEFT: black thin solid; BORDER-BOTTOM: black thin solid; BACKGROUND-COLOR: #ffff99" runat="server" visible="false" ID="NonVisualControls">
<remotion:formgridmanager id=FormGridManager runat="server"/><remotion:BindableObjectDataSourceControl id=CurrentObject runat="server" Type="Remotion.ObjectBinding.Sample::Person"/></div>
<table id=FormGrid runat="server">
  <tr>
    <td colSpan=4>Person</td></tr>
  <tr>
    <td></td>
    <td><remotion:boctextvalue id=FirstNameField runat="server" TextBoxStyle-AutoPostBack="true" Width="150px" PropertyIdentifier="FirstName" required="True" datasourcecontrol="CurrentObject"></remotion:boctextvalue></td>
    <td>
      <p>bound, required=true</p></td>
    <td style="WIDTH: 20%"><asp:label id=FirstNameFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:boctextvalue id=ReadOnlyFirstNameField runat="server" Width="150px" datasourcecontrol="CurrentObject" PropertyIdentifier="FirstName" ReadOnly="True"></remotion:boctextvalue></td>
    <td>
      <p>bound, read-only</p></td>
    <td style="WIDTH: 20%"><asp:label id=ReadOnlyFirstNameFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:boctextvalue id=UnboundFirstNameField runat="server" Width="150px"></remotion:boctextvalue></td>
    <td>
      <p>unbound, value not set, list-box, 
      required=false</p></td>
    <td style="WIDTH: 20%"><asp:label id=UnboundFirstNameFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:boctextvalue id=UnboundReadOnlyFirstNameField runat="server" Width="150px" ReadOnly="True"></remotion:boctextvalue></td>
    <td>
      <p>unbound, value set, read only</p></td>
    <td style="WIDTH: 20%"><asp:label id=UnboundReadOnlyFirstNameFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:boctextvalue id=IncomeField runat="server" Width="150px" datasourcecontrol="CurrentObject" PropertyIdentifier="Income" ReadOnly="True" format="c"></remotion:boctextvalue></td>
    <td>
      <p>&nbsp;</p></td>
    <td style="WIDTH: 20%"><asp:label id=Label1 runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:boctextvalue id=HeightField runat="server" Width="150px" datasourcecontrol="CurrentObject" PropertyIdentifier="Height">
<textboxstyle maxlength="3">
</TextBoxStyle>
</remotion:boctextvalue></td>
    <td>
      <p>&nbsp;</p></td>
    <td style="WIDTH: 20%"><asp:label id=Label4 runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:boctextvalue id=DateOfBirthField runat="server" Width="150px" datasourcecontrol="CurrentObject" PropertyIdentifier="DateOfBirth"></remotion:boctextvalue></td>
    <td>
      <p>&nbsp;</p></td>
    <td style="WIDTH: 20%"><asp:label id=Label2 runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:boctextvalue id=DateOfDeathField runat="server" Width="150px" datasourcecontrol="CurrentObject" PropertyIdentifier="DateOfDeath"></remotion:boctextvalue></td>
    <td>
      <p>&nbsp;</p></td>
    <td style="WIDTH: 20%"><asp:label id=Label3 runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:boctextvalue id=DisabledFirstNameField runat="server" Width="150px" datasourcecontrol="CurrentObject" PropertyIdentifier="FirstName" enabled="false"></remotion:boctextvalue></td>
    <td>
      <p>disabled, bound, required=true</p></td>
    <td style="WIDTH: 20%"><asp:label id=DisabledFirstNameFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:boctextvalue id=DisabledReadOnlyFirstNameField runat="server" Width="150px" datasourcecontrol="CurrentObject" PropertyIdentifier="FirstName" ReadOnly="True" enabled="false"></remotion:boctextvalue></td>
    <td>
      <p>disabled, bound, read-only</p></td>
    <td style="WIDTH: 20%"><asp:label id=DisabledReadOnlyFirstNameFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:boctextvalue id=DisabledUnboundFirstNameField runat="server" Width="150px" enabled="false"></remotion:boctextvalue></td>
    <td>
      <p>disabled, unbound, value set, list-box, 
      required=false</p></td>
    <td style="WIDTH: 20%"><asp:label id=DisabledUnboundFirstNameFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:boctextvalue id=DisabledUnboundReadOnlyFirstNameField runat="server" Width="150px" ReadOnly="True" enabled="false"></remotion:boctextvalue></td>
    <td>
      <p>disabled, unbound, value set, read only</p></td>
    <td style="WIDTH: 20%"><asp:label id=DisabledUnboundReadOnlyFirstNameFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:boctextvalue id="BocTextValue1" runat="server" ValueType="Int32" required="True"></remotion:boctextvalue></td>
    <td></td>
    <td style="WIDTH: 20%"></td></tr>
  <tr>
    <td></td>
    <td><remotion:boctextvalue id="BocTextValue2" runat="server" ValueType="Double"></remotion:boctextvalue></td>
    <td></td>
    <td style="WIDTH: 20%"></td></tr></table>
<p><remotion:webbutton id=FirstNameTestSetNullButton runat="server" Text="FirstName Set Null" width="220px"/><remotion:webbutton id=FirstNameTestSetNewValueButton runat="server" Text="FirstName Set New Value" width="220px"/></p>
<p>FirstName Field Text Changed: <asp:label id=FirstNameFieldTextChangedLabel runat="server" enableviewstate="False">#</asp:label></p>
<p><br /><remotion:webbutton id=ReadOnlyFirstNameTestSetNullButton runat="server" Text="Read Only FirstName Set Null" width="220px"/><remotion:webbutton id=ReadOnlyFirstNameTestSetNewValueButton runat="server" Text="Read Only FirstName Set New Value" width="220px"/></p>
