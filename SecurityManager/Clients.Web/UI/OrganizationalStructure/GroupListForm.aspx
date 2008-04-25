<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GroupListForm.aspx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure.GroupListForm" MasterPageFile="../SecurityManagerMasterPage.Master"  %>
<%@ Register TagPrefix="securityManager" Src="GroupListControl.ascx" TagName="GroupListControl" %>
<%@ Register TagPrefix="securityManager" Src="../ErrorMessageControl.ascx" TagName="ErrorMessageControl" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <securityManager:ErrorMessageControl id="ErrorMessageControl" runat="server" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <securityManager:GroupListControl ID="GroupListControl" runat="server"></securityManager:GroupListControl>
</asp:Content>
