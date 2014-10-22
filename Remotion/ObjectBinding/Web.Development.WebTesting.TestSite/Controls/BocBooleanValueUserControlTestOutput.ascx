<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BocBooleanValueUserControlTestOutput.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls.BocBooleanValueUserControlTestOutput" %>
<table border="1">
  <tr><td>Current value (normal):</td><td><asp:Label ID="NormalCurrentValueLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Current value (no auto postback):</td><td><asp:Label ID="NoAutoPostBackCurrentValueLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
  <tr><td>Current value (not required, tri-state):</td><td><asp:Label ID="TriStateCurrentValueLabel" ViewStateMode="Disabled" runat="server"></asp:Label></td></tr>
</table>
