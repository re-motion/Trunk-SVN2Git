<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditGroupForm.aspx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure.EditGroupForm" MasterPageFile="../SecurityManagerMasterPage.Master"  %>
<%@ Register Assembly="Remotion.Web" Namespace="Remotion.Web.UI.Controls" TagPrefix="remotion" %>
<%@ Register TagPrefix="securityManager" Src="EditGroupControl.ascx" TagName="EditGroupControl" %>
<%@ Register TagPrefix="securityManager" Src="../ErrorMessageControl.ascx" TagName="ErrorMessageControl" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <securityManager:ErrorMessageControl id="ErrorMessageControl" runat="server" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <securityManager:EditGroupControl id="EditGroupControl" runat="server"></securityManager:EditGroupControl>
</asp:Content>
<asp:Content ID="ActualBottomControlsPlaceHolder" runat="server" ContentPlaceHolderID="BottomControlsPlaceHolder">
  <table cellpadding="0" cellspacing="0">
    <tr>
      <td>
        <remotion:WebButton ID="SaveButton" runat="server" Text="$res:Save" OnClick="SaveButton_Click" CausesValidation="false"/>
      </td>
      <td>
        <remotion:WebButton ID="CancelButton" runat="server" Text="$res:Cancel" style="margin-left: 5px;" 
          OnClick="CancelButton_Click" CausesValidation="false"/>
      </td>
    </tr>
  </table>
</asp:Content>
