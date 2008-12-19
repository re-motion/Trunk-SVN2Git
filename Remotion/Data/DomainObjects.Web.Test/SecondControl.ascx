<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SecondControl.ascx.cs" Inherits="Remotion.Data.DomainObjects.Web.Test.SecondControl" %>
<%@ Register TagPrefix="webTest" TagName="ControlWithAllDataTypes" Src="ControlWithAllDataTypes.ascx" %>

<div style="background-color:#ccccff">
  <label>This is the second control.</label><BR />
  <label>Input parameter: </label><asp:Label id="GuidInLabel" runat="server" /><BR />
  <label>Output parameter: </label><asp:Label id="GuidOutLabel" runat="server" /><BR />
  <label>ClientTransaction: </label><asp:Label id="ClientTransactionLabel" runat="server" /><BR />
  <remotion:WebButton id="RefreshButton" runat="server" 
    Text="Refresh"
    onclick="RefreshButton_Click"
    />
  <br />
  <br />
  <webTest:ControlWithAllDataTypes id="ControlWithAllDataTypes" runat="server" PerformNextStepOnSave="false" OnSaved="ControlWithAllDataTypes_Saved" />
  <remotion:WebButton id="NewObjectButton" runat="server" 
    Text="New Object" 
    onclick="NewObjectButton_Click" />
  <br />
  <remotion:WebButton id="ReturnButton" runat="server" 
    Text="Return" 
    onclick="ReturnButton_Click" />
</div>