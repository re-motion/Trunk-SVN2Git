<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditPositionControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure.EditPositionControl" %>

<remotion:FormGridManager ID="FormGridManager" runat="server" ValidatorVisibility="HideValidators" />
<remotion:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.OrganizationalStructure.Position, Remotion.SecurityManager" />
<table id="FormGrid" runat="server" cellpadding="0" cellspacing="0">
  <tr class="underlinedMarkerCellRow">
    <td class="formGridTitleCell" style="white-space: nowrap;" colspan="2">
      <remotion:SmartLabel runat="server" id="PositionLabel" Text="###" />
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
      <remotion:BocEnumValue runat="server" ID="DelegationField" DataSourceControl="CurrentObject" PropertyIdentifier="Delegation" />
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocList ID="GroupTypesList" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="GroupTypes" OnMenuItemClick="GroupTypesList_MenuItemClick" Selection="Multiple" ShowEmptyListMessage="True" ShowEmptyListReadOnlyMode="True">
        <FixedColumns>
          <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="GroupType">
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
</table>
