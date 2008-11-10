<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="EditStatefulAccessControlListControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.AccessControl.EditStatefulAccessControlListControl" %>
<%@ Register TagPrefix="securityManager" Assembly="Remotion.SecurityManager.Clients.Web" Namespace="Remotion.SecurityManager.Clients.Web.Classes" %>
<%@ Register TagPrefix="securityManager" Src="EditStateCombinationControl.ascx" TagName="EditStateCombinationControl" %>
<%@ Register TagPrefix="securityManager" Src="EditAccessControlEntryControl.ascx" TagName="EditAccessControlEntryControl" %>

<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.AccessControl.StatefulAccessControlList, Remotion.SecurityManager" />
<table class="accessControlList">
  <tr>
  <td class="accessControlListTitleCell" colspan="2">
    <h2 ID="AccessControlListTitle" runat="server">###</h2>
    <div class="accessControlListButtons">
      <remotion:WebButton ID="NewStateCombinationButton" runat="server" Text="$res:NewStateCombinationButton" OnClick="NewStateCombinationButton_Click" CausesValidation="false" />
      <remotion:WebButton ID="NewAccessControlEntryButton" runat="server" Text="$res:NewAccessControlEntryButton" OnClick="NewAccessControlEntryButton_Click" CausesValidation="false" />
      <remotion:WebButton ID="DeleteAccessControlListButton" runat="server" Text="$res:DeleteAccessControlListButton" OnClick="DeleteAccessControlListButton_Click" CausesValidation="false" />
    </div>
  </td>
  </tr>
  <tr>
    <td class="stateCombinationsContainer">
      <div id="StateCombinationControls" runat="server" class="stateCombinationsContainer"><%-- 
        <securityManager:ObjectBoundRepeater ID="StateCombinationsRepeater" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="StateCombinations">
          <HeaderTemplate><table><tr><td></HeaderTemplate>
          <SeparatorTemplate></td></tr><tr><td></SeparatorTemplate>
          <FooterTemplate></td></tr></table></FooterTemplate>
          <ItemTemplate><securityManager:EditStateCombinationControl id="EditStateCombinationControl" runat="server"/></ItemTemplate>
        </securityManager:ObjectBoundRepeater>
        --%></div>
      <asp:CustomValidator ID="MissingStateCombinationsValidator" runat="server" ErrorMessage="###" OnServerValidate="MissingStateCombinationsValidator_ServerValidate" />
    </td>
    <td class="accessControlEntriesContainer">
      <div id="AccessControlEntryControls" runat="server" class="accessControlEntriesContainer"><%-- 
        <securityManager:ObjectBoundRepeater ID="AccessControlEntriesRepeater" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="AccessControlEntries">
          <HeaderTemplate><table><tr><td></HeaderTemplate>
          <SeparatorTemplate></td></tr><tr><td></SeparatorTemplate>
          <FooterTemplate></td></tr></table></FooterTemplate>
          <ItemTemplate><securityManager:EditAccessControlEntryControl id="EditAccessControlEntryControl" runat="server"/></ItemTemplate>
        </securityManager:ObjectBoundRepeater>
        --%></div>
   </td>
  </tr>
</table>

