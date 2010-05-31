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
<%@ Page Language="C#" AutoEventWireup="true" Codebehind="EditPermissionsForm.aspx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.AccessControl.EditPermissionsForm"
  MasterPageFile="../SecurityManagerMasterPage.Master" %>

<%@ Register TagPrefix="securityManager" Assembly="Remotion.SecurityManager.Clients.Web" Namespace="Remotion.SecurityManager.Clients.Web.Classes" %>
<%@ Register TagPrefix="securityManager" Src="../ErrorMessageControl.ascx" TagName="ErrorMessageControl" %>

<asp:Content ID="ActualHeaderControlsPlaceHolder" runat="server" ContentPlaceHolderID="HeaderControlsPlaceHolder">
  <remotion:BindableObjectDataSourceControl ID="CurrentObjectHeaderControls" runat="server" Type="Remotion.SecurityManager.Domain.Metadata.SecurableClassDefinition, Remotion.SecurityManager" Mode="Read" />
  <h1 ID="TitleLabel" runat="server" class="pageTitle">###</h1>
</asp:Content>
<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder" />
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.Metadata.SecurableClassDefinition, Remotion.SecurityManager" />
  <asp:CustomValidator ID="DuplicateStateCombinationsValidator" runat="server" ErrorMessage="<%$ res:DuplicateStateCombinationsValidatorErrorMessage %>" OnServerValidate="DuplicateStateCombinationsValidator_ServerValidate" EnableClientScript="false"/>
  <asp:PlaceHolder ID="AccessControlListsPlaceHolder" runat="server"/>
</asp:Content>
<asp:Content ID="ActualBottomControlsPlaceHolder" runat="server" ContentPlaceHolderID="BottomControlsPlaceHolder">
  <asp:UpdatePanel ID="BottomControlsUpdatePanel" runat="server" UpdateMode="Always" RenderMode="Inline">
    <ContentTemplate>
      <remotion:WebButton ID="SaveButton" runat="server" Text="$res:Save" OnClick="SaveButton_Click" CausesValidation="false" RequiresSynchronousPostBack="true" />
      <remotion:WebButton ID="CancelButton" runat="server" Text="$res:Cancel" OnClick="CancelButton_Click" CausesValidation="false" RequiresSynchronousPostBack="true" />
      <remotion:WebButton ID="NewStatefulAccessControlListButton" runat="server" Text="$res:NewStatefulAccessControlListButton:Text" OnClick="NewStatefulAccessControlListButton_Click" CausesValidation="False" RequiresSynchronousPostBack="true"/>
      <remotion:WebButton ID="NewStatelessAccessControlListButton" runat="server" Text="$res:NewStatelessAccessControlListButton:Text" OnClick="NewStatelessAccessControlListButton_Click" CausesValidation="False" RequiresSynchronousPostBack="true" />
    </ContentTemplate> 
  </asp:UpdatePanel>
</asp:Content>
