<%-- This file is part of re-strict (www.re-motion.org)
 % Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditGroupTypeControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure.EditGroupTypeControl" %>

<remotion:FormGridManager ID="FormGridManager" runat="server" ValidatorVisibility="HideValidators" />
<remotion:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.OrganizationalStructure.GroupType, Remotion.SecurityManager" />
<table id="FormGrid" runat="server" cellpadding="0" cellspacing="0">
  <tr class="underlinedMarkerCellRow">
    <td class="formGridTitleCell" style="white-space: nowrap;" colspan="2">
      <remotion:SmartLabel runat="server" id="GroupTypeLabel" Text="###" />
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue ID="NameField" runat="server" DataSourceControl="CurrentObject"
        PropertyIdentifier="Name">
        <TextBoxStyle MaxLength="100" />
      </remotion:BocTextValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocList ID="GroupsList" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Groups" OnMenuItemClick="GroupsList_MenuItemClick" Selection="Multiple" ShowEmptyListMessage="true" ShowEmptyListReadOnlyMode="true">
        <FixedColumns>
          <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="DisplayName">
            <PersistedCommand>
              <remotion:BocListItemCommand />
            </PersistedCommand>
          </remotion:BocSimpleColumnDefinition>
        </FixedColumns>
        <ListMenuItems>
          <remotion:BocMenuItem ItemID="AddItem" Text="$res:Add">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Show="EditMode" />
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="RemoveItem" RequiredSelection="OneOrMore" Text="$res:Remove">
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
      <remotion:BocList ID="PositionsList" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Positions" OnMenuItemClick="PositionsList_MenuItemClick" Selection="Multiple">
        <FixedColumns>
          <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="Position">
            <PersistedCommand>
              <remotion:BocListItemCommand />
            </PersistedCommand>
          </remotion:BocSimpleColumnDefinition>
        </FixedColumns>
        <ListMenuItems>
          <remotion:BocMenuItem ItemID="NewItem" Text="$res:New">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Show="EditMode" />
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="EditItem" RequiredSelection="OneOrMore" Text="$res:Edit">
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
