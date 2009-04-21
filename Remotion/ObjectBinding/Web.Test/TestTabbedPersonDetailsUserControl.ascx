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


<%@ Control Language="c#" AutoEventWireup="false" Codebehind="TestTabbedPersonDetailsUserControl.ascx.cs" Inherits="OBWTest.TestTabbedPersonDetailsUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>


<table id="FormGrid" runat="server" style="MARGIN-TOP: 0%">
  <tr>
    <td></td>
    <td><remotion:boctextvalue id="LastNameField" required="true" runat="server" propertyidentifier="LastName" datasourcecontrol="CurrentObject">
<textboxstyle textmode="SingleLine" autopostback="True">
</TextBoxStyle></remotion:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:boctextvalue id="FirstNameField" runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject"></remotion:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocdatetimevalue id="DateOfBirthField" runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="CurrentObject" >
<datetimetextboxstyle autopostback="True">
</DateTimeTextBoxStyle></remotion:bocdatetimevalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocbooleanvalue id="DeceasedField" runat="server" propertyidentifier="Deceased" datasourcecontrol="CurrentObject" nullitemerrormessage="Eingabe erforderlich" autopostback="True"></remotion:bocbooleanvalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocdatetimevalue id="DateOfDeathField" runat="server" propertyidentifier="DateOfDeath" datasourcecontrol="CurrentObject" ></remotion:bocdatetimevalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocenumvalue id="MarriageStatusField" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject" nullitemerrormessage="Eingabe erforderlich">
<listcontrolstyle autopostback="True">
</ListControlStyle></remotion:bocenumvalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocreferencevalue id="PartnerField" runat="server" propertyidentifier="Partner" datasourcecontrol="CurrentObject" nullitemerrormessage="Eingabe erforderlich">
<dropdownliststyle autopostback="True">
</DropDownListStyle>

<persistedcommand>
<remotion:BocCommand WxeFunctionCommand-Parameters="id" WxeFunctionCommand-TypeName="OBWTest.ViewPersonDetailsWxeFunction,OBWTest" Type="WxeFunction"></remotion:BocCommand>
</PersistedCommand></remotion:bocreferencevalue></td></tr>
</table>
<p><remotion:formgridmanager id="FormGridManager" runat="server" visible="true"></remotion:formgridmanager><remotion:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" /></p>
