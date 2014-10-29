<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BocDateTimeValueUserControlTestOutput.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls.BocDateTimeValueUserControlTestOutput" %>
<table border="1">
  <tr><td>Current value (normal):</td><td><asp:Label ID="NormalCurrentValueLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Current value (no auto postback):</td><td><asp:Label ID="NoAutoPostBackCurrentValueLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Current value (date-only):</td><td><asp:Label ID="DateOnlyCurrentValueLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Current value (with seconds):</td><td><asp:Label ID="WithSecondsCurrentValueLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
</table>
