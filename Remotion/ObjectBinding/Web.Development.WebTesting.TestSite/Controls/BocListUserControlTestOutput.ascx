<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BocListUserControlTestOutput.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls.BocListUserControlTestOutput" %>
<table border="1">
  <tr><td>Selected indices (normal):</td><td colspan="3"><asp:Label ID="SelectedIndicesLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Selected view (normal):</td><td colspan="3"><asp:Label ID="SelectedViewLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Edit mode (normal):</td><td colspan="3"><asp:Label ID="EditModeLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr>
    <td>Action performed:</td>
    <td><asp:Label ID="ActionPerformedSenderLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td>
    <td><asp:Label ID="ActionPerformedSenderRowLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td>
    <td><asp:Label ID="ActionPerformedLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td>
    <td><asp:Label ID="ActionPerformedParameterLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td>
  </tr>
</table>
