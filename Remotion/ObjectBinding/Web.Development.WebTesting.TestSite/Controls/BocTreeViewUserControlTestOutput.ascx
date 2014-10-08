<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BocTreeViewUserControlTestOutput.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls.BocTreeViewUserControlTestOutput" %>
<table border="1">
  <tr><td>Selected node (normal):</td><td colspan="3"><asp:Label ID="NormalSelectedNodeLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Selected node (no top level expander):</td><td colspan="3"><asp:Label ID="NoTopLevelExpanderSelectedNodeLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Selected node (no look ahead evaluation):</td><td colspan="3"><asp:Label ID="NoLookAheadEvaluationSelectedNodeLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Selected node (no property identifier):</td><td colspan="3"><asp:Label ID="NoPropertyIdentifierSelectedNodeLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr>
    <td>Action performed:</td>
    <td><asp:Label ID="ActionPerformedSenderLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td>
    <td><asp:Label ID="ActionPerformedLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td>
    <td><asp:Label ID="ActionPerformedParameterLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td>
  </tr>
</table>
