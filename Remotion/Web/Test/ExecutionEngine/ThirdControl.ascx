<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ThirdControl.ascx.cs" Inherits="Remotion.Web.Test.ExecutionEngine.ThirdControl" %>
<div style="background-color: #CCFFFF">
  Third Control
  <p>
    Last postback on control:
    <asp:Label ID="ControlLabel" runat="server" /><br />
    <remotion:WebButton ID="ExecuteNextStepButton" runat="server" Text="Execute Next Step (Return)" OnClick="ExecuteNextStep_Click" />
    <remotion:WebButton ID="ExecuteFourthUserControlButton" runat="server" Text="Execute Fourth User Control" OnClick="ExecuteFourthUserControlButton_Click" /><br />
    <remotion:WebButton ID="PostbackButton" runat="server" Text="Postback to Control" /><br />
    ViewState: <asp:Label ID="ViewStateLabel" runat="server" /><br />
    ControlState: <asp:Label ID="ControlStateLabel" runat="server" /><br />
    <remotion:WebButton ID="ExecuteThirdUserControlButton" runat="server" 
      Text="Throw invalid operation exception / Act as sink for previous button's event target" OnClick="ExecuteThirdUserControlButton_Click" />    
 </p>
</div>
