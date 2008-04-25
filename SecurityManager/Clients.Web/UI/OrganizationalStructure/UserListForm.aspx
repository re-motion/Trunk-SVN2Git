<%@ Page Language="C#" AutoEventWireup="true" Codebehind="UserListForm.aspx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure.UserListForm" MasterPageFile="../SecurityManagerMasterPage.Master" %>
<%@ Register TagPrefix="securityManager" Src="UserListControl.ascx" TagName="UserListControl" %>
<%@ Register TagPrefix="securityManager" Src="../ErrorMessageControl.ascx" TagName="ErrorMessageControl" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <securityManager:ErrorMessageControl id="ErrorMessageControl" runat="server" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <securityManager:UserListControl ID="UserListControl" runat="server"></securityManager:UserListControl>
</asp:Content>
