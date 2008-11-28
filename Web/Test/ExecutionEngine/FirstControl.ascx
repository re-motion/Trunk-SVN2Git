<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FirstControl.ascx.cs" Inherits="Remotion.Web.Test.ExecutionEngine.FirstControl" %>
<div style="background-color:#CCFFCC">
  First Control
  <p>
    Last postback on control:
    <asp:Label ID="ControlLabel" runat="server" /><br />
    <remotion:WebButton ID="ExecuteNextStepButton" runat="server" Text="Execute Next Step (Return)" OnClick="ExecuteNextStep_Click" />
    <remotion:WebButton ID="ExecuteSecondUserControlButton" runat="server" Text="Execute Second User Control" OnClick="ExecuteSecondUserControlButton_Click" />
    <asp:Button ID="ExecuteSecondUserControlAspNetButton" runat="server" Text="Execute Second User Control by ASP.NET Button" OnClick="ExecuteSecondUserControlButton_Click" />
    <remotion:WebButton ID="PostbackButton" runat="server" Text="Postback to Control" /><br />
    ViewState: <asp:Label ID="ViewStateLabel" runat="server" /><br />
    ControlState: <asp:Label ID="ControlStateLabel" runat="server" /><br />
    <remotion:ControlMock ID="SubControlWithState" runat="server" /><br />
    <asp:TextBox ID="SubControlWithFormElement" runat="server" EnableViewState="false" />
  </p>
</div>
