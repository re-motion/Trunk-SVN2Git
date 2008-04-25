<%@ Control Language="c#" AutoEventWireup="True" Codebehind="BocLiteralUserControl.ascx.cs" Inherits="OBWTest.IndividualControlTests.BocLiteralUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>




<div style="BORDER-RIGHT: black thin solid; BORDER-TOP: black thin solid; BORDER-LEFT: black thin solid; BORDER-BOTTOM: black thin solid; BACKGROUND-COLOR: #ffff99" runat="server" visible="false" ID="NonVisualControls">
<remotion:formgridmanager id=FormGridManager runat="server"/><remotion:BindableObjectDataSourceControl id=CurrentObject runat="server" Type="Remotion.ObjectBinding.Sample::Person"/></div>
      <table id=FormGrid runat="server">
        <tr>
          <td colSpan=4><remotion:boctextvalue id=FirstNameField runat="server" ReadOnly="True" datasourcecontrol="CurrentObject" PropertyIdentifier="FirstName"></remotion:boctextvalue>&nbsp;<remotion:boctextvalue id=LastNameField runat="server" ReadOnly="True" datasourcecontrol="CurrentObject" PropertyIdentifier="LastName"></remotion:boctextvalue></td></tr>
        <tr>
          <td></td>
          <td><remotion:BocLiteral id=CVField runat="server" datasourcecontrol="CurrentObject" PropertyIdentifier="CVString" /></td>
          <td>
            <p>bound</p></td>
          <td style="WIDTH: 20%"><asp:label id=CVFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><remotion:BocLiteral id=UnboundCVField runat="server" /></td>
          <td>
            <p>unbound, value not set</p></td>
          <td style="WIDTH: 20%"><asp:label id=UnboundCVFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
      </table>
      <p><remotion:webbutton id=CVTestSetNullButton runat="server" Text="VC Set Null" width="220px"/><remotion:webbutton id=CVTestSetNewValueButton runat="server" Text="CVSet New Value" width="220px"/></p>
