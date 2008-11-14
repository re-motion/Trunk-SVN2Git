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
<asp:UpdatePanel runat="server">
<ContentTemplate>
<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.AccessControl.AccessControlEntry, Remotion.SecurityManager" />
<remotion:FormGridManager ID="FormGridManager" runat="server" ShowHelpProviders="False" />
<asp:ScriptManagerProxy runat="server" />
<table id="FormGrid" runat="server" class="accessControlEntry">
  <tr>
    <td class="accessControlEntryTitleCell" colspan="2">
      <h3 ID="AccessControlEntryTitle" runat="server">###</h3>
      <div class="accessControlEntryButtons">
        <remotion:WebButton ID="DeleteAccessControlEntryButton" runat="server" Text="$res:DeleteAccessControlEntryButton" OnClick="DeleteAccessControlEntryButton_Click" CausesValidation="false" />
      </div>
    </td>
  </tr>
  <tr>
    <td><remotion:SmartLabel ID="TenantLabel" runat="server" ForControl="TenantConditionField"/></td>
    <td>
      <remotion:BocEnumValue ID="TenantConditionField" runat="server" PropertyIdentifier="TenantCondition" DataSourceControl="CurrentObject" OnSelectionChanged="TenantField_SelectionChanged" Width="20em" Style="display:block; float:left; margin-top:0.2em" >
        <ListControlStyle AutoPostBack="True"/>
      </remotion:BocEnumValue>
      <remotion:BocReferenceValue ID="SpecificTenantField" runat="server" PropertyIdentifier="SpecificTenant" DataSourceControl="CurrentObject" Required="True" OnSelectionChanged="SpecificTenantField_SelectionChanged" Style="display:block; float:left; margin-top:0.05em">
        <DropDownListStyle AutoPostBack="true" />
      </remotion:BocReferenceValue>
    </td>
  </tr>
  <tr>
    <td><remotion:SmartLabel ID="GroupConditionLabel" runat="server" ForControl="GroupConditionField"/></td>
    <td>
      <remotion:BocEnumValue ID="GroupConditionField" runat="server" PropertyIdentifier="GroupCondition" DataSourceControl="CurrentObject"  OnSelectionChanged="GroupConditionField_SelectionChanged" Width="14em" Style="display:block; float:left; margin-top:0.05em">
        <ListControlStyle AutoPostBack="True"/>
      </remotion:BocEnumValue>
      <remotion:BocAutoCompleteReferenceValue ID="SpecificGroupField" runat="server" PropertyIdentifier="SpecificGroup" DataSourceControl="CurrentObject" Required="true" Style="display:block; float:left; margin-bottom:0.2em"/>
      <remotion:BocEnumValue ID="GroupHierarchyConditionField" runat="server" PropertyIdentifier="GroupHierarchyCondition" DataSourceControl="CurrentObject" Required="true" Style="display:block; float:left; margin-bottom:0.2em"/>
      <remotion:BocReferenceValue ID="SpecificGroupTypeField" runat="server" PropertyIdentifier="SpecificGroupType" DataSourceControl="CurrentObject" Required="true" Style="display:block; float:left; margin-bottom:0.2em"/>
    </td>      
  </tr>
  <tr>
    <td><remotion:SmartLabel ID="UserConditionLabel" runat="server" ForControl="UserConditionField"/></td>
    <td>
      <remotion:BocEnumValue ID="UserConditionField" runat="server" PropertyIdentifier="UserCondition" DataSourceControl="CurrentObject"  OnSelectionChanged="UserConditionField_SelectionChanged" Width="14em" Style="display:block; float:left; margin-top:0.05em">
        <ListControlStyle AutoPostBack="True"/>
      </remotion:BocEnumValue>
      <remotion:BocAutoCompleteReferenceValue ID="SpecificUserField" runat="server" PropertyIdentifier="SpecificUser" DataSourceControl="CurrentObject" Required="true" Style="display:block; float:left; margin-bottom:0.2em"/>
      <remotion:BocReferenceValue ID="SpecificPositionField" runat="server" PropertyIdentifier="SpecificPosition" DataSourceControl="CurrentObject" Style="display:block; float:left;"/>
    </td>
  </tr>
  <tr>
    <td><remotion:SmartLabel ID="SpecificAbstractRoleLabel" runat="server" ForControl="SpecificAbstractRoleField"/></td>
    <td>
      <remotion:BocReferenceValue ID="SpecificAbstractRoleField" runat="server" PropertyIdentifier="SpecificAbstractRole" DataSourceControl="CurrentObject" >
      <DropDownListStyle AutoPostBack="True" />
        </remotion:BocReferenceValue>
    </td>
  </tr>
  <tr>
    <td><remotion:FormGridLabel ID="PermissionsLabel" runat="server" Text="###" /></td>
    <td>
      <asp:PlaceHolder ID="PermissionsPlaceHolder" runat="server" />
      <%--
      <securityManager:ObjectBoundRepeater ID="PermissionsRepeater" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Permissions">
        <HeaderTemplate><ul class="permissionsList"><li class="permissionsList"></HeaderTemplate>
        <SeparatorTemplate></li><li class="permissionsList"></SeparatorTemplate>
        <FooterTemplate></li></ul></FooterTemplate>
        <ItemTemplate><securityManager:EditPermissionControl id="EditPermissionControl" runat="server"/></ItemTemplate>
      </securityManager:ObjectBoundRepeater>
      --%>
    </td>
  </tr>
</table>
</ContentTemplate>
</asp:UpdatePanel>