<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="EditAccessControlEntryControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.AccessControl.EditAccessControlEntryControl" %>
<%@ Register TagPrefix="securityManager" Assembly="Remotion.SecurityManager.Clients.Web" Namespace="Remotion.SecurityManager.Clients.Web.Classes" %>
<%@ Register TagPrefix="securityManager" Src="EditPermissionControl.ascx" TagName="EditPermissionControl" %>

<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.AccessControl.AccessControlEntry, Remotion.SecurityManager" />
<remotion:FormGridManager ID="FormGridManager" runat="server" ShowHelpProviders="False" />
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server" />

<tr>

<asp:PlaceHolder ID="CollapsedView" runat="server">
<asp:PlaceHolder ID="CollapsedAccessControlInformation" runat="server" />
<td>
  <remotion:WebButton ID="ExpandAccessControlEntryButton" runat="server" CssClass="imageButton"
    OnClick="ExpandAccessControlEntryButton_Click" CausesValidation="false" RequiresSynchronousPostBack="False" />
</td>
</asp:PlaceHolder>

<asp:PlaceHolder ID="ExpandedView" runat="server">
<td id="ExpandedCell" runat="server">
<table id="FormGrid" runat="server" class="accessControlEntryExpanded">
  <tr>
    <td class="accessControlEntryTitleCell" colspan="2">
      <div class="accessControlEntryButtons">
        <remotion:WebButton ID="DeleteAccessControlEntryButton" runat="server" CssClass="imageButton"
          OnClick="DeleteAccessControlEntryButton_Click" CausesValidation="false" RequiresSynchronousPostBack="True" />
        <remotion:WebButton ID="CollapseAccessControlEntryButton" runat="server" CssClass="imageButton"
          OnClick="CollapseAccessControlEntryButton_Click" CausesValidation="false" RequiresSynchronousPostBack="False" />
      </div>
    </td>
  </tr>
  <tr>
    <td><remotion:SmartLabel ID="TenantLabel" runat="server" ForControl="TenantConditionField"/></td>
    <td>
      <remotion:BocEnumValue ID="TenantConditionField" runat="server" PropertyIdentifier="TenantCondition" DataSourceControl="CurrentObject" OnSelectionChanged="TenantConditionField_SelectionChanged" Width="33%" Style="display:block; float:left; margin-top:0.2em" >
        <ListControlStyle AutoPostBack="True"/>
      </remotion:BocEnumValue>
      <remotion:BocReferenceValue ID="SpecificTenantField" runat="server" PropertyIdentifier="SpecificTenant" DataSourceControl="CurrentObject" Required="True" OnSelectionChanged="SpecificTenantField_SelectionChanged" Width="33%" Style="display:block; float:left; margin-top:0.05em">
        <DropDownListStyle AutoPostBack="true" />
      </remotion:BocReferenceValue>
      <remotion:BocEnumValue ID="TenantHierarchyConditionField" runat="server" PropertyIdentifier="TenantHierarchyCondition" DataSourceControl="CurrentObject" Required="true" Width="33%" Style="display:block; float:left; margin-top:0.2em"/>
    </td>
  </tr>
  <tr>
    <td><remotion:SmartLabel ID="GroupConditionLabel" runat="server" ForControl="GroupConditionField"/></td>
    <td>
      <remotion:BocEnumValue ID="GroupConditionField" runat="server" PropertyIdentifier="GroupCondition" DataSourceControl="CurrentObject"  OnSelectionChanged="GroupConditionField_SelectionChanged" Width="33%" Style="display:block; float:left; margin-top:0.2em">
        <ListControlStyle AutoPostBack="True"/>
      </remotion:BocEnumValue>
      <remotion:BocAutoCompleteReferenceValue ID="SpecificGroupField" runat="server" PropertyIdentifier="SpecificGroup" DataSourceControl="CurrentObject" Required="true" Width="33%" Style="display:block; float:left; margin-top:0.2em"/>
      <remotion:BocEnumValue ID="GroupHierarchyConditionField" runat="server" PropertyIdentifier="GroupHierarchyCondition" DataSourceControl="CurrentObject" Required="true" Width="33%" Style="display:block; float:left; margin-top:0.2em"/>
      <remotion:BocReferenceValue ID="SpecificGroupTypeField" runat="server" PropertyIdentifier="SpecificGroupType" DataSourceControl="CurrentObject" Required="true" Width="33%" Style="display:block; float:left; margin-top:0.05em"/>
    </td>      
  </tr>
  <tr>
    <td><remotion:SmartLabel ID="UserConditionLabel" runat="server" ForControl="UserConditionField"/></td>
    <td>
      <remotion:BocEnumValue ID="UserConditionField" runat="server" PropertyIdentifier="UserCondition" DataSourceControl="CurrentObject"  OnSelectionChanged="UserConditionField_SelectionChanged" Width="33%" Style="display:block; float:left; margin-top:0.2em">
        <ListControlStyle AutoPostBack="True"/>
      </remotion:BocEnumValue>
      <remotion:BocAutoCompleteReferenceValue ID="SpecificUserField" runat="server" PropertyIdentifier="SpecificUser" DataSourceControl="CurrentObject" Required="true" Width="33%" Style="display:block; float:left; margin-top:0.2em"/>
      <remotion:BocReferenceValue ID="SpecificPositionField" runat="server" PropertyIdentifier="SpecificPosition" DataSourceControl="CurrentObject" Required="true" Width="33%" Style="display:block; float:left; margin-top:0.05em"/>
    </td>
  </tr>
  <tr>
    <td><remotion:SmartLabel ID="SpecificAbstractRoleLabel" runat="server" ForControl="SpecificAbstractRoleField"/></td>
    <td>
      <remotion:BocReferenceValue ID="SpecificAbstractRoleField" runat="server" PropertyIdentifier="SpecificAbstractRole" DataSourceControl="CurrentObject" Width="33%">
      <DropDownListStyle AutoPostBack="True" />
        </remotion:BocReferenceValue>
    </td>
  </tr>
  <tr>
    <td>
      <remotion:FormGridLabel ID="PermissionsLabel" runat="server" Text="###" />
      <remotion:BocBooleanValue ID="AllPermissionsField" runat="server" ShowDescription="false" AutoPostBack="true" Width="1em" OnCheckedChanged="AllPermissionsField_CheckedChange"/>
    </td>
    <td><asp:PlaceHolder ID="PermissionsPlaceHolder" runat="server" /></td>
  </tr>
</table>
</td>
</asp:PlaceHolder>

</tr>