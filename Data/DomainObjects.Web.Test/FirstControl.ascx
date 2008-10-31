<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FirstControl.ascx.cs" Inherits="Remotion.Data.DomainObjects.Web.Test.FirstControl" %>
<%@ Register TagPrefix="webTest" TagName="ControlWithAllDataTypes" Src="ControlWithAllDataTypes.ascx" %>

<div style="background-color:#ccffcc">
  <label>This is the first control.</label><BR />
  <label>Input parameter: </label><asp:Label id="GuidInLabel" runat="server" /><BR />
  <label>Output parameter: </label><asp:Label id="GuidOutLabel" runat="server" /><BR />
  <label>ClientTransaction: </label><asp:Label id="ClientTransactionLabel" runat="server" /><BR />
  <remotion:WebButton id="NonTransactionUserControlStepButton" runat="server" 
    Text="Call non-transacted control" 
    onclick="NonTransactionUserControlStepButton_Click" />
  <remotion:WebButton id="RootTransactionUserControlStepButton" runat="server" 
    Text="Call root-transacted control" 
    onclick="RootTransactionUserControlStepButton_Click" />
  <remotion:WebButton id="SubTransactionUserControlStepButton" runat="server" 
    Text="Call sub-transacted control" 
    onclick="SubTransactionUserControlStepButton_Click" />
  <br />
  <remotion:WebButton id="SaveButton" runat="server" 
    Text="Save" 
    onclick="SaveButton_Click" />
  <remotion:WebButton id="ReturnButton" runat="server" 
    Text="Return" 
    onclick="ReturnButton_Click" />
</div>