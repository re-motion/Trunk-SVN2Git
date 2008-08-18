<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FirstControl.ascx.cs" Inherits="Remotion.Web.Test.ExecutionEngine.FirstControl" %>
<div>
  First Control
  <p>
    Last postback on control:
    <asp:Label ID="ControlLabel" runat="server" /><br />
    <remotion:WebButton ID="ExecuteSubFunctionButton" runat="server" Text="Execute Second User Control" OnClick="ExecuteSubFunctionButton_Click" />
    <remotion:WebButton ID="PostbackButton" runat="server" Text="Postback to Control" /><br />
    ViewState: <asp:Label ID="ViewStateLabel" runat="server" /><br />
    ControlState: <asp:Label ID="ControlStateLabel" runat="server" />
  </p>
</div>
