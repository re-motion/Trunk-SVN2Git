<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchGroupForm.aspx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure.SearchGroupForm" MasterPageFile="../SecurityManagerMasterPage.Master"  %>
<%@ Register Assembly="Remotion.Web" Namespace="Remotion.Web.UI.Controls" TagPrefix="remotion" %>
<%@ Register TagPrefix="securityManager" Src="SearchGroupControl.ascx" TagName="SearchGroupControl" %>
<%@ Register TagPrefix="securityManager" Src="../ErrorMessageControl.ascx" TagName="ErrorMessageControl" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <securityManager:ErrorMessageControl id="ErrorMessageControl" runat="server" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <securityManager:SearchGroupControl ID="SearchGroupControl" runat="server"></securityManager:SearchGroupControl>
</asp:Content>
<asp:Content ID="ActualBottomControlsPlaceHolder" runat="server" ContentPlaceHolderID="BottomControlsPlaceHolder">
  <table cellpadding="0" cellspacing="0">
    <tr>
      <td>
        <remotion:WebButton ID="CancelButton" runat="server" Text="$res:Cancel" OnClick="CancelButton_Click" CausesValidation="false"/>
      </td>
    </tr>
  </table>
</asp:Content>
