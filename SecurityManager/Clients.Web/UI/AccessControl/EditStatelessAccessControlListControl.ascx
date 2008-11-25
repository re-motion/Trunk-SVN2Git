<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="EditStatelessAccessControlListControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.AccessControl.EditStatelessAccessControlListControl" %>
<%@ Register TagPrefix="securityManager" Assembly="Remotion.SecurityManager.Clients.Web" Namespace="Remotion.SecurityManager.Clients.Web.Classes" %>
<%@ Register TagPrefix="securityManager" Src="EditAccessControlEntryControl.ascx" TagName="EditAccessControlEntryControl" %>

<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.AccessControl.StatelessAccessControlList, Remotion.SecurityManager" />
<table class="accessControlList">
  <tr>
  <td class="accessControlListTitleCell" colspan="2">
    <h2 ID="StatelessAccessControlListTitle" runat="server">###</h2>
    <div class="accessControlListButtons">
      <remotion:WebButton ID="NewAccessControlEntryButton" runat="server" Text="$res:NewAccessControlEntryButton" OnClick="NewAccessControlEntryButton_Click" CausesValidation="false" />
      <remotion:WebButton ID="DeleteAccessControlListButton" runat="server" Text="$res:DeleteAccessControlListButton" OnClick="DeleteAccessControlListButton_Click" CausesValidation="false" />
    </div>
  </td>
  </tr>
  <tr>
    <td class="stateCombinationsContainer">
    </td>
    <td class="accessControlEntriesContainer">
      <asp:PlaceHolder id="AccessControlEntryControls" runat="server" />
   </td>
  </tr>
</table>

