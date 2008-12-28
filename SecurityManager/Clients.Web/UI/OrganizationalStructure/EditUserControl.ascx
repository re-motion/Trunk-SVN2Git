<%-- This file is part of re-strict (www.re-motion.org)
 % Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
 % 
 % This program is free software; you can redistribute it and/or modify
 % it under the terms of the GNU Affero General Public License version 3.0 
 % as published by the Free Software Foundation.
 % 
 % This program is distributed in the hope that it will be useful, 
 % but WITHOUT ANY WARRANTY; without even the implied warranty of 
 % MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 % GNU Affero General Public License for more details.
 % 
 % You should have received a copy of the GNU Affero General Public License
 % along with this program; if not, see http://www.gnu.org/licenses.
 % 
 % Additional permissions are listed in the file re-motion_exceptions.txt.
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditUserControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure.EditUserControl" %>

<remotion:FormGridManager ID="FormGridManager" runat="server" ValidatorVisibility="HideValidators" />
<remotion:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.OrganizationalStructure.User, Remotion.SecurityManager" />
<asp:ScriptManagerProxy runat="server" />
<table id="FormGrid" runat="server" cellpadding="0" cellspacing="0">
  <tr class="underlinedMarkerCellRow">
    <td class="formGridTitleCell" style="white-space: nowrap;" colspan="2">
      <remotion:SmartLabel runat="server" id="UserLabel" Text="###" />
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue runat="server" ID="UserNameField" DataSourceControl="CurrentObject" PropertyIdentifier="UserName"></remotion:BocTextValue>    
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue ID="TitleField" runat="server" DataSourceControl="CurrentObject"
        PropertyIdentifier="Title">
        <TextBoxStyle MaxLength="100" />
      </remotion:BocTextValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue ID="FirstnameField" runat="server" DataSourceControl="CurrentObject"
        PropertyIdentifier="FirstName">
        <TextBoxStyle MaxLength="100" />
      </remotion:BocTextValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue ID="LastNameField" runat="server" DataSourceControl="CurrentObject"
        PropertyIdentifier="LastName">
        <TextBoxStyle MaxLength="100" />
      </remotion:BocTextValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocReferenceValue ID="OwningGroupField" runat="server" DataSourceControl="CurrentObject"
        PropertyIdentifier="OwningGroup">
        <PersistedCommand>
          <remotion:BocCommand />
        </PersistedCommand>
      </remotion:BocReferenceValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocList ID="RolesList" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Roles" OnMenuItemClick="RolesList_MenuItemClick" Selection="Multiple" ShowEmptyListMessage="True" ShowEmptyListReadOnlyMode="True">
        <FixedColumns>
          <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="Group" />
          <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="Position" />
        </FixedColumns>
        <ListMenuItems>
          <remotion:BocMenuItem ItemID="NewItem" Text="$res:New">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Show="EditMode" />
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="EditItem" RequiredSelection="ExactlyOne" Text="$res:Edit">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Show="EditMode" />
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="DeleteItem" RequiredSelection="OneOrMore" Text="$res:Delete">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Show="EditMode" />
            </PersistedCommand>
          </remotion:BocMenuItem>
        </ListMenuItems>
      </remotion:BocList>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocList ID="SubstitutedByList" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="SubstitutedBy" OnMenuItemClick="SubstitutedByList_MenuItemClick" OnEditableRowChangesCanceled="SubstitutedByList_EditableRowChangesCanceled" OnEditableRowChangesSaved="SubstitutedByList_EditableRowChangesSaved" Selection="Multiple" ShowEmptyListMessage="True" ShowEmptyListReadOnlyMode="True">
        <FixedColumns>
          <remotion:BocRowEditModeColumnDefinition Width="40px"/>
          <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="SubstitutingUser" Width="30%" />
          <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="SubstitutedRole" Width="30%" />
          <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="BeginDate" Width="15%" />
          <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="EndDate" Width="15%" />          
          <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="IsEnabled" Width="10%" />
        </FixedColumns>
        <ListMenuItems>
          <remotion:BocMenuItem ItemID="NewItem" Text="$res:New">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Show="EditMode" />
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="DeleteItem" RequiredSelection="OneOrMore" Text="$res:Delete">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Show="EditMode" />
            </PersistedCommand>
          </remotion:BocMenuItem>
        </ListMenuItems>
      </remotion:BocList>
    </td>
  </tr>
</table>
