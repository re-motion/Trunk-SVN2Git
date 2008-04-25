<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TenantListForm.aspx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure.TenantListForm" MasterPageFile="../SecurityManagerMasterPage.Master"  %>
<%@ Register TagPrefix="securityManager" Src="TenantListControl.ascx" TagName="TenantListControl" %>
<%@ Register TagPrefix="securityManager" Src="../ErrorMessageControl.ascx" TagName="ErrorMessageControl" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <securityManager:ErrorMessageControl id="ErrorMessageControl" runat="server" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <securityManager:TenantListControl ID="TenantListControl" runat="server" />
</asp:Content>
