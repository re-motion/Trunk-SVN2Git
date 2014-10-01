<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BocAutoCompleteReferenceValueUserControlTestOutput.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls.BocAutoCompleteReferenceValueUserControlTestOutput" %>
<table border="1">
  <tr><td>BOUI (normal):</td><td colspan="3"><asp:Label ID="BOUINormalLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>BOUI (no auto postback):</td><td colspan="3"><asp:Label ID="BOUINoAutoPostBackLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr>
    <td>Action performed:</td>
    <td><asp:Label ID="ActionPerformedSenderLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td>
    <td><asp:Label ID="ActionPerformedLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td>
    <td><asp:Label ID="ActionPerformedParameterLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td>
  </tr>
</table>
