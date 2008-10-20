<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SecondControl.ascx.cs" Inherits="Remotion.Web.Test.ExecutionEngine.SecondControl" %>
<div>
  Second Control
  <p>
    Last postback on control:
    <asp:Label ID="ControlLabel" runat="server" /><br />
    <remotion:WebButton ID="ExecuteNextStepButton" runat="server" Text="Execute Next Step (Return)" OnClick="ExecuteNextStep_Click" />
    <remotion:WebButton ID="PostbackButton" runat="server" Text="Postback to Control" /><br />
    ViewState: <asp:Label ID="ViewStateLabel" runat="server" /><br />
    ControlState: <asp:Label ID="ControlStateLabel" runat="server" /><br />
    <remotion:WebButton ID="ExecuteSubFunctionButton" runat="server" Text="Throw invalid operation exception / Act as sink for previous postback's event target" OnClick="ExecuteSubFunctionButton_Click" />    
 </p>
</div>
