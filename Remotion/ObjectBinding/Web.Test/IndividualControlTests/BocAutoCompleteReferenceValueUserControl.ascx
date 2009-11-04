<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
 % Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
 %
 % The re-motion Core Framework is free software; you can redistribute it 
 % and/or modify it under the terms of the GNU Lesser General Public License 
 % as published by the Free Software Foundation; either version 2.1 of the 
 % License, or (at your option) any later version.
 %
 % re-motion is distributed in the hope that it will be useful, 
 % but WITHOUT ANY WARRANTY; without even the implied warranty of 
 % MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 % GNU Lesser General Public License for more details.
 %
 % You should have received a copy of the GNU Lesser General Public License
 % along with re-motion; if not, see http://www.gnu.org/licenses.
--%>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="BocAutoCompleteReferenceValueUserControl.ascx.cs" Inherits="OBWTest.IndividualControlTests.BocAutoCompleteReferenceValueUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>


<div style="BORDER-RIGHT: black thin solid; BORDER-TOP: black thin solid; BORDER-LEFT: black thin solid; BORDER-BOTTOM: black thin solid; BACKGROUND-COLOR: #ffff99" runat="server" visible="false" ID="NonVisualControls">
<remotion:formgridmanager id=FormGridManager runat="server"/>
<remotion:BindableObjectDataSourceControl id=CurrentObject runat="server" Type="Remotion.ObjectBinding.Sample::Person"/></div>

<table id=FormGrid runat="server">
  <tr>
    <td colSpan=4><remotion:boctextvalue id=FirstNameField runat="server" readonly="True" datasourcecontrol="CurrentObject" PropertyIdentifier="FirstName">
</remotion:boctextvalue>&nbsp;<remotion:boctextvalue id=LastNameField runat="server" readonly="True" datasourcecontrol="CurrentObject" PropertyIdentifier="LastName"></remotion:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocAutoCompleteReferenceValue id=PartnerField runat="server" ServiceMethod="GetPersonList" ServicePath="~/IndividualControlTests/AutoCompleteService.asmx" 
    TextBoxStyle-AutoPostBack="true" readonly="False" datasourcecontrol="CurrentObject" propertyidentifier="Partner" width="100%"
    CompletionInterval="50" SuggestionInterval="2000" CompletionSetCount="5">
<persistedcommand>
<remotion:BocCommand Type="Event"></remotion:BocCommand>
</PersistedCommand>

<optionsmenuitems>
<remotion:BocMenuItem Text="intern">
<persistedcommand>
<remotion:BocMenuItemCommand Type="Href" HrefCommand-Href="~/startForm.aspx"></remotion:BocMenuItemCommand>
</PersistedCommand>
</remotion:BocMenuItem>
<remotion:BocMenuItem Text="extern">
<persistedcommand>
<remotion:BocMenuItemCommand Type="Href" HrefCommand-Target="_blank" HrefCommand-Href="~/startForm.aspx"></remotion:BocMenuItemCommand>
</PersistedCommand>
</remotion:BocMenuItem>
</OptionsMenuItems>

<labelstyle cssclass="class">
</LabelStyle></remotion:bocAutoCompleteReferenceValue></td>
    <td>bound</td>
    <td style="WIDTH: 20%"><asp:label id=PartnerFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocAutoCompleteReferenceValue id=ReadOnlyPartnerField runat="server" ServiceMethod="GetPersonList" ServicePath="~/IndividualControlTests/AutoCompleteService.asmx" readonly="True" datasourcecontrol="CurrentObject" propertyidentifier="Partner" width="100%" >
<persistedcommand>
<remotion:BocCommand WxeFunctionCommand-Parameters="id" WxeFunctionCommand-TypeName="OBWTest.ViewPersonDetailsWxeFunction,OBWTest" Type="Event"></remotion:BocCommand>
</PersistedCommand>

<labelstyle cssclass="class">
</LabelStyle></remotion:bocAutoCompleteReferenceValue></td>
    <td>bound, read-only</td>
    <td style="WIDTH: 20%"><asp:label id=ReadOnlyPartnerFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocAutoCompleteReferenceValue id=UnboundPartnerField runat="server" ServiceMethod="GetPersonList" ServicePath="~/IndividualControlTests/AutoCompleteService.asmx" required="True" hasvalueembeddedinsideoptionsmenu="False" width="250px" showoptionsmenu="False" >
<persistedcommand>
<remotion:boccommand Type="Event"></remotion:boccommand>
</PersistedCommand></remotion:bocAutoCompleteReferenceValue></td>
    <td>
       unbound, value not set</td>
    <td style="WIDTH: 20%"><asp:label id=UnboundPartnerFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocAutoCompleteReferenceValue id=UnboundReadOnlyPartnerField runat="server" ServiceMethod="GetPersonList" ServicePath="~/IndividualControlTests/AutoCompleteService.asmx" readonly="True" enableicon="False" hasvalueembeddedinsideoptionsmenu="False" width="250px">
<persistedcommand>
<remotion:boccommand Type="Event"></remotion:boccommand>
</PersistedCommand>

</remotion:bocAutoCompleteReferenceValue></td>
    <td>
      unbound, value set, read only</td>
    <td style="WIDTH: 20%"><asp:label id=UnboundReadOnlyPartnerFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocAutoCompleteReferenceValue id=DisabledPartnerField runat="server" ServiceMethod="GetPersonList" ServicePath="~/IndividualControlTests/AutoCompleteService.asmx" readonly="False" datasourcecontrol="CurrentObject" propertyidentifier="Partner" hasvalueembeddedinsideoptionsmenu="True" enabled="False">

<persistedcommand>
<remotion:boccommand Type="Event"></remotion:boccommand>
</PersistedCommand></remotion:bocAutoCompleteReferenceValue></td>
    <td>disabled, bound</td>
    <td style="WIDTH: 20%"><asp:label id="DisabledPartnerFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocAutoCompleteReferenceValue id=DisabledReadOnlyPartnerField runat="server" ServiceMethod="GetPersonList" ServicePath="~/IndividualControlTests/AutoCompleteService.asmx" readonly="True" datasourcecontrol="CurrentObject" propertyidentifier="Partner" enabled="False">

<persistedcommand>
<remotion:boccommand WxeFunctionCommand-Parameters="id" WxeFunctionCommand-TypeName="OBWTest.ViewPersonDetailsWxeFunction,OBWTest" Type="WxeFunction"></remotion:boccommand>
</PersistedCommand></remotion:bocAutoCompleteReferenceValue></td>
    <td>disabled, bound, read-only</td>
    <td style="WIDTH: 20%"><asp:label id="DisabledReadOnlyPartnerFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocAutoCompleteReferenceValue id=DisabledUnboundPartnerField runat="server" ServiceMethod="GetPersonList" ServicePath="~/IndividualControlTests/AutoCompleteService.asmx" required="True" enabled="False" width="250px" >
<persistedcommand>
<remotion:boccommand></remotion:boccommand>
</PersistedCommand></remotion:bocAutoCompleteReferenceValue></td>
    <td>
       disabled, unbound, value set</td>
    <td style="WIDTH: 20%"><asp:label id="DisabledUnboundPartnerFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td></tr>
  <tr>
    <td></td>
    <td><remotion:bocAutoCompleteReferenceValue id=DisabledUnboundReadOnlyPartnerField runat="server" ServiceMethod="GetPersonList" ServicePath="~/IndividualControlTests/AutoCompleteService.asmx" readonly="True" enableicon="False" enabled="False" width="250px" >
<persistedcommand>
<remotion:boccommand></remotion:boccommand>
</PersistedCommand>

</remotion:bocAutoCompleteReferenceValue></td>
    <td>
      disabled, unbound, value set, read only</td>
    <td style="WIDTH: 20%"><asp:label id="DisabledUnboundReadOnlyPartnerFieldValueLabel" runat="server" enableviewstate="False">#</asp:label></td></tr></table>
<p>Partner Command Click: <asp:label id="PartnerCommandClickLabel" runat="server" enableviewstate="False">#</asp:label></p>
<p>Partner Selection Changed: <asp:label id=PartnerFieldSelectionChangedLabel runat="server" enableviewstate="False">#</asp:label></p>
<p>Partner Menu Click: <asp:label id=PartnerFieldMenuClickEventArgsLabel runat="server" enableviewstate="False">#</asp:label></p>
<p><br /><remotion:webbutton id=PartnerTestSetNullButton runat="server" width="220px" Text="Partner Set Null"/><remotion:webbutton id=PartnerTestSetNewItemButton runat="server" width="220px" Text="Partner Set New Item"/></p>
<p><remotion:webbutton id=ReadOnlyPartnerTestSetNullButton runat="server" width="220px" Text="Read Only Partner Set Null"/><remotion:webbutton id=ReadOnlyPartnerTestSetNewItemButton runat="server" width="220px" Text="Read Only Partner Set New Item"/></p>
