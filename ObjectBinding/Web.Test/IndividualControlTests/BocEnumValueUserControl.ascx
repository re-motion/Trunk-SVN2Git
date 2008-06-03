<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>




<%@ Control Language="c#" AutoEventWireup="false" Codebehind="BocEnumValueUserControl.ascx.cs" Inherits="OBWTest.IndividualControlTests.BocEnumValueUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<div style="BORDER-RIGHT: black thin solid; BORDER-TOP: black thin solid; BORDER-LEFT: black thin solid; BORDER-BOTTOM: black thin solid; BACKGROUND-COLOR: #ffff99" runat="server" visible="false" ID="NonVisualControls">
<remotion:formgridmanager id=FormGridManager runat="server"/><remotion:BindableObjectDataSourceControl id=CurrentObject runat="server" Type="Remotion.ObjectBinding.Sample::Person"/></div>
<table id=FormGrid runat="server">
  <tr>
    <td colSpan=4><remotion:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" datasourcecontrol="CurrentObject" readonly="True"></remotion:boctextvalue>&nbsp;<remotion:boctextvalue id=LastNameField runat="server" PropertyIdentifier="LastName" datasourcecontrol="CurrentObject" readonly="True"></remotion:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocenumvalue id=GenderField runat="server" PropertyIdentifier="Gender" datasourcecontrol="CurrentObject" nullitemerrormessage="Eingabe erforderlich">
<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal" >
</ListControlStyle>
</remotion:bocenumvalue></td>
    <td>
      <p>bound, radio buttons, required=true</p></td>
    <td style="WIDTH: 20%"><asp:label id=GenderFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocenumvalue id=ReadOnlyGenderField runat="server" PropertyIdentifier="Gender" datasourcecontrol="CurrentObject" ReadOnly="True" Required="True" width="150px">
<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal" >
</ListControlStyle>
</remotion:bocenumvalue></td>
    <td>
      <p>bound, read-only</p></td>
    <td style="WIDTH: 20%"><asp:label id=ReadOnlyGenderFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocenumvalue id=MarriageStatusField runat="server" PropertyIdentifier="MarriageStatus" datasourcecontrol="CurrentObject" required="False">
<listcontrolstyle radiobuttonlistrepeatdirection="Horizontal" >
</ListControlStyle>
            </remotion:bocenumvalue></td>
    <td>
      <p>bound, drop-down, required=false</p></td>
    <td style="WIDTH: 20%"><asp:label id=MarriageStatusFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocenumvalue id=UnboundMarriageStatusField runat="server" >
<listcontrolstyle listboxrows="2" controltype="ListBox" >
</ListControlStyle>
            </remotion:bocenumvalue></td>
    <td>
      <p>unbound, value not set, list-box, 
    required=true</p></td>
    <td style="WIDTH: 20%"><asp:label id=UnboundMarriageStatusFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocenumvalue id=UnboundReadOnlyMarriageStatusField runat="server" ReadOnly="True">
              <listcontrolstyle radionbuttonlistrepeatlayout="Table" controltype="ListBox"></listcontrolstyle>
            </remotion:bocenumvalue></td>
    <td>
      <p>unbound, value set, read only</p></td>
    <td style="WIDTH: 20%"><asp:label id=UnboundReadOnlyMarriageStatusFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocenumvalue id=DeceasedAsEnumField runat="server" PropertyIdentifier="Deceased" datasourcecontrol="CurrentObject" required="False">
              <listcontrolstyle radiobuttonlistrepeatdirection="Horizontal"></ListControlStyle>
            </remotion:bocenumvalue></td>
    <td>deceased (bool) as enum</td>
    <td style="WIDTH: 20%"><asp:label id=DeceasedAsEnumFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocenumvalue id=DisabledGenderField runat="server" PropertyIdentifier="Gender" datasourcecontrol="CurrentObject" nullitemerrormessage="Eingabe erforderlich" enabled=false>
<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal">
</ListControlStyle>
</remotion:bocenumvalue></td>
    <td>
      <p>disabled, bound, radio buttons, required=true</p></td>
    <td style="WIDTH: 20%"><asp:label id=DisabledGenderFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocenumvalue id=DisabledReadOnlyGenderField runat="server" PropertyIdentifier="Gender" datasourcecontrol="CurrentObject" ReadOnly="True" Required="True" enabled=false>
<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal">
</ListControlStyle>
</remotion:bocenumvalue></td>
    <td>
      <p>disabled, bound, read-only</p></td>
    <td style="WIDTH: 20%"><asp:label id=DisabledReadOnlyGenderFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocenumvalue id=DisabledMarriageStatusField runat="server" PropertyIdentifier="MarriageStatus" datasourcecontrol="CurrentObject" required="False" enabled=false>
              <listcontrolstyle radionbuttonlistrepeatlayout="Table" radiobuttonlistrepeatdirection="Horizontal"></listcontrolstyle>
            </remotion:bocenumvalue></td>
    <td>
      <p>disabled, bound, drop-down, required=false</p></td>
    <td style="WIDTH: 20%"><asp:label id=DisabledMarriageStatusFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocenumvalue id=DisabledUnboundMarriageStatusField runat="server" enabled=false>
              <listcontrolstyle radiobuttonlisttextalign="Right" listboxrows="2" radionbuttonlistrepeatlayout="Table"
                controltype="ListBox" radiobuttonlistrepeatdirection="Vertical"></listcontrolstyle>
            </remotion:bocenumvalue></td>
    <td>
      <p> disabled, unbound, value&nbsp;set, list-box, 
    required=true</p></td>
    <td style="WIDTH: 20%"><asp:label id=DisabledUnboundMarriageStatusFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocenumvalue id=DisabledUnboundReadOnlyMarriageStatusField runat="server" ReadOnly="True" enabled=false>
              <listcontrolstyle radionbuttonlistrepeatlayout="Table" controltype="ListBox"></listcontrolstyle>
            </remotion:bocenumvalue></td>
    <td>
      <p>disabled, unbound, value set, read only</p></td>
    <td style="WIDTH: 20%"><asp:label id=DisabledUnboundReadOnlyMarriageStatusFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td>Instance Enum</td>
    <td><remotion:bocenumvalue id="InstanceEnumField" runat="server">
            </remotion:bocenumvalue></td>
    <td>
      <p> unboud</p></td>
    <td style="WIDTH: 20%"><asp:label id="InstanceEnumFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
    </table>
<p><br>Gender Selection Changed: <asp:label id=GenderFieldSelectionChangedLabel runat="server" EnableViewState="False">#</asp:label></p>
<p><remotion:webbutton id=GenderTestSetNullButton runat="server" Text="Gender Set Null" width="165px"/><remotion:webbutton id=GenderTestSetDisabledGenderButton runat="server" Text="Gender Set Disabled Gender" width="165px"/><remotion:webbutton id=GenderTestSetMarriedButton runat="server" Text="Gender Set Married" width="165px"/></p>
<p><br><remotion:webbutton id=ReadOnlyGenderTestSetNullButton runat="server" Text="Read Only Gender Set Null" width="220px"/><remotion:webbutton id=ReadOnlyGenderTestSetNewItemButton runat="server" Text="Read Only Gender Set Female" width="220px"/></p>
