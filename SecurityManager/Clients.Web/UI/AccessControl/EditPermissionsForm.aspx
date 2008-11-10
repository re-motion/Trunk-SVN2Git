<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>
<%@ Page Language="C#" AutoEventWireup="true" Codebehind="EditPermissionsForm.aspx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.AccessControl.EditPermissionsForm"
  MasterPageFile="../SecurityManagerMasterPage.Master" %>

<%@ Register TagPrefix="securityManager" Assembly="Remotion.SecurityManager.Clients.Web" Namespace="Remotion.SecurityManager.Clients.Web.Classes" %>
<%@ Register TagPrefix="securityManager" Src="../ErrorMessageControl.ascx" TagName="ErrorMessageControl" %>

<asp:Content ID="ActualHeaderControlsPlaceHolder" runat="server" ContentPlaceHolderID="HeaderControlsPlaceHolder">
  <remotion:BindableObjectDataSourceControl ID="CurrentObjectHeaderControls" runat="server" Type="Remotion.SecurityManager.Domain.Metadata.SecurableClassDefinition, Remotion.SecurityManager" Mode="Read" />
  <h1><remotion:BocTextValue ID="NameField" runat="server" DataSourceControl="CurrentObjectHeaderControls" PropertyIdentifier="DisplayName" /></h1>
</asp:Content>
<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder" />
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.Metadata.SecurableClassDefinition, Remotion.SecurityManager" />
  <asp:CustomValidator ID="DuplicateStateCombinationsValidator" runat="server" ErrorMessage="<%$ res:DuplicateStateCombinationsValidatorErrorMessage %>" OnServerValidate="DuplicateStateCombinationsValidator_ServerValidate"/>
  <asp:PlaceHolder ID="AccessControlListsPlaceHolder" runat="server"/>
  <%-- 
  <securityManager:ObjectBoundRepeater ID="AccessControlListsRepeater" runat="server" PropertyIdentifier="AccessControlLists">
    <HeaderTemplate><div class="accessControlList"></HeaderTemplate>
    <SeparatorTemplate></div><div class="accessControlList"></SeparatorTemplate>
    <FooterTemplate></div></FooterTemplate>
    <ItemTemplate><securityManager:EditAccessControlListControl id="EditAccessControlListControl" runat="server"/></ItemTemplate>
  </securityManager:ObjectBoundRepeater>
  --%>
</asp:Content>
<asp:Content ID="ActualBottomControlsPlaceHolder" runat="server" ContentPlaceHolderID="BottomControlsPlaceHolder">
  <remotion:WebButton ID="SaveButton" runat="server" Text="$res:Save" OnClick="SaveButton_Click" CausesValidation="false" />
  <remotion:WebButton ID="CancelButton" runat="server" Text="$res:Cancel" Style="margin-left: 1em;" OnClick="CancelButton_Click" CausesValidation="false" />
  <remotion:WebButton ID="NewStatefulAccessControlListButton" runat="server" Text="$res:NewStatefulAccessControlListButton" Style="margin-left: 1em;" OnClick="NewStatefulAccessControlListButton_Click" CausesValidation="False" />
  <remotion:WebButton ID="NewStatelessAccessControlListButton" runat="server" Text="$res:NewStatelessAccessControlListButton" Style="margin-left: 1em;" OnClick="NewStatelessAccessControlListButton_Click" CausesValidation="False" />
</asp:Content>
