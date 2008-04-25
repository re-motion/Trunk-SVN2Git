<%@ Control Language="c#" AutoEventWireup="false" Codebehind="BocMultilineTextValueUserControl.ascx.cs" Inherits="OBWTest.IndividualControlTests.BocMultilineTextValueUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>




<div style="BORDER-RIGHT: black thin solid; BORDER-TOP: black thin solid; BORDER-LEFT: black thin solid; BORDER-BOTTOM: black thin solid; BACKGROUND-COLOR: #ffff99" runat="server" visible="false" ID="NonVisualControls">
<remotion:formgridmanager id=FormGridManager runat="server"/><remotion:BindableObjectDataSourceControl id=CurrentObject runat="server" Type="Remotion.ObjectBinding.Sample::Person"/></div>
      <table id=FormGrid runat="server">
        <tr>
          <td colSpan=4><remotion:boctextvalue id=FirstNameField runat="server" ReadOnly="True" datasourcecontrol="CurrentObject" PropertyIdentifier="FirstName"></remotion:boctextvalue>&nbsp;<remotion:boctextvalue id=LastNameField runat="server" ReadOnly="True" datasourcecontrol="CurrentObject" PropertyIdentifier="LastName"></remotion:boctextvalue></td></tr>
        <tr>
          <td></td>
          <td><remotion:bocmultilinetextvalue id=CVField runat="server" datasourcecontrol="CurrentObject" PropertyIdentifier="CV" requirederrormessage="Eingabe erforderlich" Width="150px" required="True">
<textboxstyle rows="3" textmode="MultiLine" autopostback="True" maxlength="50">
</TextBoxStyle>
            </remotion:bocmultilinetextvalue></td>
          <td>
            <p>bound, required=true</p></td>
          <td style="WIDTH: 20%"><asp:label id=CVFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><remotion:bocmultilinetextvalue id=ReadOnlyCVField runat="server" ReadOnly="True" datasourcecontrol="CurrentObject" PropertyIdentifier="CV" Width="150px">
              <textboxstyle rows="5" textmode="MultiLine">
              </textboxstyle>
            </remotion:bocmultilinetextvalue></td>
          <td>
            <p>bound, read-only</p></td>
          <td style="WIDTH: 20%"><asp:label id=ReadOnlyCVFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><remotion:bocmultilinetextvalue id=UnboundCVField runat="server" Width="150px">
<textboxstyle rows="3" textmode="MultiLine">
</TextBoxStyle></remotion:bocmultilinetextvalue></td>
          <td>
            <p>unbound, value not set, list-box,
              required=false</p></td>
          <td style="WIDTH: 20%"><asp:label id=UnboundCVFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><remotion:bocmultilinetextvalue id=UnboundReadOnlyCVField runat="server" ReadOnly="True" Width="150px">
              <textboxstyle rows="5" textmode="MultiLine">
              </textboxstyle></remotion:bocmultilinetextvalue></td>
          <td>
            <p>unbound, value set, read only</p></td>
          <td style="WIDTH: 20%"><asp:label id=UnboundReadOnlyCVFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><remotion:bocmultilinetextvalue id=DisabledCVField runat="server" datasourcecontrol="CurrentObject" PropertyIdentifier="CV" requirederrormessage="Eingabe erforderlich" Width="150px" required="True" enabled=false>
<textboxstyle rows="3" textmode="MultiLine">
</TextBoxStyle>
            </remotion:bocmultilinetextvalue></td>
          <td>
            <p>disabled, bound, required=true</p></td>
          <td style="WIDTH: 20%"><asp:label id=DisabledCVFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><remotion:bocmultilinetextvalue id=DisabledReadOnlyCVField runat="server" ReadOnly="True" datasourcecontrol="CurrentObject" PropertyIdentifier="CV" Width="150px" enabled=false>
              <textboxstyle rows="5" textmode="MultiLine">
              </textboxstyle>
            </remotion:bocmultilinetextvalue></td>
          <td>
            <p>disabled, bound, read-only</p></td>
          <td style="WIDTH: 20%"><asp:label id=DisabledReadOnlyCVFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><remotion:bocmultilinetextvalue id=DisabledUnboundCVField runat="server" Width="150px" enabled=false>
<textboxstyle rows="3" textmode="MultiLine">
</TextBoxStyle></remotion:bocmultilinetextvalue></td>
          <td>
            <p> disabled, unbound, value set, list-box,
              required=false</p></td>
          <td style="WIDTH: 20%"><asp:label id=DisabledUnboundCVFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr>
        <tr>
          <td></td>
          <td><remotion:bocmultilinetextvalue id=DisabledUnboundReadOnlyCVField runat="server" ReadOnly="True" Width="150px" enabled=false>
              <textboxstyle rows="5" textmode="MultiLine">
              </textboxstyle></remotion:bocmultilinetextvalue></td>
          <td>
            <p>disabled, unbound, value set, read only</p></td>
          <td style="WIDTH: 20%"><asp:label id=DisabledUnboundReadOnlyCVFieldValueLabel runat="server" enableviewstate="False">#</asp:label></td></tr></table>
      <p>CV Field Text Changed: <asp:label id=CVFieldTextChangedLabel runat="server" EnableViewState="False">#</asp:label></p>
      <p><remotion:webbutton id=CVTestSetNullButton runat="server" Text="VC Set Null" width="220px"/><remotion:webbutton id=CVTestSetNewValueButton runat="server" Text="CVSet New Value" width="220px"/></p>
      <p><br><remotion:webbutton id=ReadOnlyCVTestSetNullButton runat="server" Text="Read Only CV Set Null" width="220px"/><remotion:webbutton id=ReadOnlyCVTestSetNewValueButton runat="server" Text="Read Only CV Set New Value" width="220px"/></p>
