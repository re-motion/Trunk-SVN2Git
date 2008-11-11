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
<remotion:FormGridManager ID="FormGridManager" runat="server" ShowHelpProviders="False" ShowRequiredMarkers="False" />
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
      <table cellpadding="0" cellspacing="0">
        <tr>
          <td><remotion:BocEnumValue ID="TenantConditionField" runat="server" PropertyIdentifier="TenantCondition" DataSourceControl="CurrentObject" OnSelectionChanged="TenantField_SelectionChanged" Width="20em" >
            <ListControlStyle AutoPostBack="True"/>
          </remotion:BocEnumValue></td>
          <td>
            <remotion:BocReferenceValue ID="SpecificTenantField" runat="server" PropertyIdentifier="SpecificTenant" DataSourceControl="CurrentObject" Required="True" >
              <PersistedCommand>
                <remotion:BocCommand />
              </PersistedCommand>
            </remotion:BocReferenceValue>
          </td>
        </tr>
      </table>
    </td>
  </tr>
  <tr>
    <td><remotion:SmartLabel ID="SpecificAbstractRoleLabel" runat="server" ForControl="SpecificAbstractRoleField"/></td>
    <td><remotion:BocReferenceValue ID="SpecificAbstractRoleField" runat="server" PropertyIdentifier="SpecificAbstractRole" DataSourceControl="CurrentObject" >
      <PersistedCommand>
        <remotion:BocCommand />
      </PersistedCommand>
      <DropDownListStyle AutoPostBack="True" />
    </remotion:BocReferenceValue></td>
 </tr>
  <tr>
    <td><remotion:SmartLabel ID="SpecificPositionLabel" runat="server" ForControl="SpecificPositionField"/></td>
    <td>
      <table cellpadding="0" cellspacing="0">
        <tr>
          <td>
            <remotion:BocReferenceValue ID="SpecificPositionField" runat="server" PropertyIdentifier="SpecificPosition" DataSourceControl="CurrentObject" OnSelectionChanged="SpecificPositionField_SelectionChanged" >
              <PersistedCommand>
                <remotion:BocCommand />
              </PersistedCommand>
              <DropDownListStyle AutoPostBack="True" />
            </remotion:BocReferenceValue>
          </td>
          <td>&nbsp;<asp:label id="SpecificPositionAndGroupLinkingLabel" runat="server" Text="###" />&nbsp;</td>
          <td><remotion:BocEnumValue ID="GroupConditionField" runat="server" PropertyIdentifier="GroupCondition" DataSourceControl="CurrentObject" width="20em">
            <ListControlStyle AutoPostBack="True"/>
          </remotion:BocEnumValue></td>
        </tr>
      </table>
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


