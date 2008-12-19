<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FourthControl.ascx.cs" Inherits="Remotion.Web.Test.ExecutionEngine.FourthControl" %>
<div style="background-color: #FFCCCC">
  Fourth Control
  <p>
    Last postback on control:
    <asp:Label ID="ControlLabel" runat="server" /><br />
    <remotion:WebButton ID="ExecuteNextStepButton" runat="server" Text="Execute Next Step (Return)" OnClick="ExecuteNextStep_Click" />
    <remotion:WebButton ID="PostbackButton" runat="server" Text="Postback to Control" /><br />
    ViewState: <asp:Label ID="ViewStateLabel" runat="server" /><br />
    ControlState: <asp:Label ID="ControlStateLabel" runat="server" /><br />
    <remotion:WebButton ID="ExecuteThridUserControlButton" runat="server" 
      Text="Throw invalid operation exception / Act as sink for previous button's event target" OnClick="ExecuteThirdUserControlButton_Click" />    
 </p>
</div>
