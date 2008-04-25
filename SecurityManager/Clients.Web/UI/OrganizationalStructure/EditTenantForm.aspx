<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditTenantForm.aspx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure.EditTenantForm" MasterPageFile="../SecurityManagerMasterPage.Master"  %>
<%@ Register Assembly="Remotion.Web" Namespace="Remotion.Web.UI.Controls" TagPrefix="remotion" %>
<%@ Register TagPrefix="securityManager" Src="EditTenantControl.ascx" TagName="EditTenantControl" %>
<%@ Register TagPrefix="securityManager" Src="../ErrorMessageControl.ascx" TagName="ErrorMessageControl" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <securityManager:ErrorMessageControl id="ErrorMessageControl" runat="server" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <securityManager:EditTenantControl id="EditTenantControl" runat="server" />
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
