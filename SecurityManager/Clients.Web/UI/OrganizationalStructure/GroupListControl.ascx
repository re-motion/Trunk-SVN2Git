<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroupListControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure.GroupListControl" %>

<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.OrganizationalStructure.Group, Remotion.SecurityManager" />
<remotion:FormGridManager ID="FormGridManager" runat="server" ValidatorVisibility="HideValidators" />
<table cellpadding="0" cellspacing="0" style="width: 100%; height: 100%;">
  <tr>
    <td style="height: 100%;">
      <table id="FormGrid" runat="server" cellpadding="0" cellspacing="0" style="width: 100%; height: 100%;">
        <tr class="underlinedMarkerCellRow">
          <td class="formGridTitleCell" style="white-space: nowrap;">
            <remotion:SmartLabel runat="server" id="GroupListLabel" Text="###"/>
          </td>
          <td style="DISPLAY: none;WIDTH: 100%"></td>
        </tr>
        <tr>
          <td style="height: 100%; vertical-align: top;">
            <remotion:BocList ID="GroupList" runat="server" DataSourceControl="CurrentObject" OnListItemCommandClick="GroupList_ListItemCommandClick" ShowEmptyListMessage="True" ShowEmptyListReadOnlyMode="True">
              <FixedColumns>
                <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="DisplayName">
                  <PersistedCommand>
                    <remotion:BocListItemCommand Type="Event" />
                  </PersistedCommand>
                </remotion:BocSimpleColumnDefinition>
              </FixedColumns>
            </remotion:BocList>
          </td>
        </tr>
      </table>
    </td>
  </tr>
  <tr>
    <td style="padding-left: 5px; padding-top: 5px;">
      <remotion:WebButton ID="NewGroupButton" runat="server" OnClick="NewGroupButton_Click" />
    </td>
  </tr>
</table>
