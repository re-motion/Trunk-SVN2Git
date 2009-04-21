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
<%@ Control Language="c#" AutoEventWireup="True" Codebehind="BocAutoCompleteReferenceValueUserControl.ascx.cs" Inherits="OBWTest.IndividualControlTests.BocAutoCompleteReferenceValueUserControl"
  TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<remotion:FormGridManager ID="FormGridManager" runat="server" />
<remotion:BindableObjectDataSourceControl id=CurrentObject runat="server" Type="Remotion.ObjectBinding.Sample::Person"/>
<table id="FormGrid" runat="server">
  <tr>
    <td colspan="4">
      <remotion:BocTextValue ID="FirstNameField" runat="server" ReadOnly="True" DataSourceControl="CurrentObject" PropertyIdentifier="FirstName">
      </remotion:BocTextValue>
      &nbsp;<remotion:BocTextValue ID="LastNameField" runat="server" ReadOnly="True" DataSourceControl="CurrentObject" PropertyIdentifier="LastName">
      </remotion:BocTextValue>
    </td>
  </tr>
  <tr>
    <td>
    </td>
    <td>
      <remotion:BocAutoCompleteReferenceValue ID="PartnerField" runat="server" ReadOnly="False" DataSourceControl="CurrentObject" PropertyIdentifier="Partner"
        Width="75%" ServiceMethod="GetPersonList" ServicePath="~/IndividualControlTests/AutoCompleteService.asmx" args="Test">
        <TextBoxStyle AutoPostBack="True" />
      </remotion:BocAutoCompleteReferenceValue>
    </td>
    <td>
      bound, 75%</td>
    <td style="width: 20%">
      <asp:Label ID="PartnerFieldValueLabel" runat="server" EnableViewState="False">#</asp:Label></td>
  </tr>
  <tr>
    <td><br /><br /><br /><br /><br /><br />
    </td>
    <td>
      <remotion:BocAutoCompleteReferenceValue ID="ReadOnlyPartnerField" runat="server" ReadOnly="True" DataSourceControl="CurrentObject" PropertyIdentifier="Partner"
        Width="75%">
      </remotion:BocAutoCompleteReferenceValue>
    </td>
    <td>
      bound, read-only, 75%</td>
    <td style="width: 20%">
      <asp:Label ID="ReadOnlyPartnerFieldValueLabel" runat="server" EnableViewState="False">#</asp:Label></td>
  </tr>
  <tr>
    <td>
    </td>
    <td>
      <remotion:BocAutoCompleteReferenceValue ID="UnboundPartnerField" runat="server" Required="True" Width="15em" ServiceMethod="GetPersonList" 
      ServicePath="~/IndividualControlTests/AutoCompleteService.asmx">
      </remotion:BocAutoCompleteReferenceValue>
    </td>
    <td>
      <p>
        unbound, value not set, 15em</p>
    </td>
    <td style="width: 20%">
      <asp:Label ID="UnboundPartnerFieldValueLabel" runat="server" EnableViewState="False">#</asp:Label></td>
  </tr>
  <tr>
    <td>
    </td>
    <td>
      <remotion:BocAutoCompleteReferenceValue ID="UnboundReadOnlyPartnerField" runat="server" ReadOnly="True" Width="15em">
      </remotion:BocAutoCompleteReferenceValue>
    </td>
    <td>
      <p>
        unbound, value set, read only, 15em</p>
    </td>
    <td style="width: 20%">
      <asp:Label ID="UnboundReadOnlyPartnerFieldValueLabel" runat="server" EnableViewState="False">#</asp:Label></td>
  </tr>
  <tr>
    <td>
    </td>
    <td>
      <remotion:BocAutoCompleteReferenceValue ID="DisabledPartnerField" runat="server" ReadOnly="False" DataSourceControl="CurrentObject" PropertyIdentifier="Partner"
        Enabled="False" Width="75%" >
      </remotion:BocAutoCompleteReferenceValue>
    </td>
    <td>
      disabled, bound, 75%</td>
    <td style="width: 20%">
      <asp:Label ID="DisabledPartnerFieldValueLabel" runat="server" EnableViewState="False" >#</asp:Label></td>
  </tr>
  <tr>
    <td>
    </td>
    <td>
      <remotion:BocAutoCompleteReferenceValue ID="DisabledReadOnlyPartnerField" runat="server" ReadOnly="True" DataSourceControl="CurrentObject" PropertyIdentifier="Partner"
        Enabled="False">
      </remotion:BocAutoCompleteReferenceValue>
    </td>
    <td>
      disabled, bound, read-only</td>
    <td style="width: 20%">
      <asp:Label ID="DisabledReadOnlyPartnerFieldValueLabel" runat="server" EnableViewState="False">#</asp:Label></td>
  </tr>
  <tr>
    <td>
    </td>
    <td>
      <remotion:BocAutoCompleteReferenceValue ID="DisabledUnboundPartnerField" runat="server" Required="True" Enabled="False" Width="15em">
      </remotion:BocAutoCompleteReferenceValue>
    </td>
    <td>
      <p>
        disabled, unbound, value set, 15em</p>
    </td>
    <td style="width: 20%">
      <asp:Label ID="DisabledUnboundPartnerFieldValueLabel" runat="server" EnableViewState="False">#</asp:Label></td>
  </tr>
  <tr>
    <td>
    </td>
    <td>
      <remotion:BocAutoCompleteReferenceValue ID="DisabledUnboundReadOnlyPartnerField" runat="server" ReadOnly="True" Enabled="False" Width="15em">
      </remotion:BocAutoCompleteReferenceValue>
    </td>
    <td>
      <p>
        disabled, unbound, value set, read only, 15em</p>
    </td>
    <td style="width: 20%">
      <asp:Label ID="DisabledUnboundReadOnlyPartnerFieldValueLabel" runat="server" EnableViewState="False">#</asp:Label></td>
  </tr>
</table>
<p>
  Partner Command Click:
  <asp:Label ID="PartnerCommandClickLabel" runat="server" EnableViewState="False">#</asp:Label></p>
<p>
  Partner Selection Changed:
  <asp:Label ID="PartnerFieldSelectionChangedLabel" runat="server" EnableViewState="False">#</asp:Label></p>
<p>
  Partner Menu Click:
  <asp:Label ID="PartnerFieldMenuClickEventArgsLabel" runat="server" EnableViewState="False">#</asp:Label></p>
<p>
  <br>
  <remotion:WebButton ID="PartnerTestSetNullButton" runat="server" Text="Partner Set Null" />
  <remotion:WebButton ID="PartnerTestSetNewItemButton" runat="server" Text="Partner Set New Item" />
</p>
<p>
  <remotion:WebButton ID="ReadOnlyPartnerTestSetNullButton" runat="server" Text="Read Only Partner Set Null" />
  <remotion:WebButton ID="ReadOnlyPartnerTestSetNewItemButton" runat="server" Text="Read Only Partner Set New Item" />
</p>
