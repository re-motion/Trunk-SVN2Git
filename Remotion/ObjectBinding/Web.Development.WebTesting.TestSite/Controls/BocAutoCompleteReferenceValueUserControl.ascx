<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BocAutoCompleteReferenceValueUserControl.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls.BocAutoCompleteReferenceValueUserControl" %>
<remotion:FormGridManager ID="FormGridManager" runat="server" />
<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
<table id="FormGrid" runat="server">
  <tr>
    <td></td>
    <td>
      <remotion:BocAutoCompleteReferenceValue ID="PartnerField" SearchServicePath="AutoCompleteService.asmx"
        TextBoxStyle-AutoPostBack="true" ReadOnly="False" DataSourceControl="CurrentObject" PropertyIdentifier="Partner" CompletionSetCount="5" runat="server">
        
        <PersistedCommand>
          <remotion:BocCommand Type="Event"></remotion:BocCommand>
        </PersistedCommand>

        <OptionsMenuItems>
          <remotion:BocMenuItem Text="My menu command">
            <PersistedCommand>
            <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
          </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem Text="My menu command 2">
            <PersistedCommand>
            <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
          </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem Text="My menu command 3">
            <PersistedCommand>
            <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
          </PersistedCommand>
          </remotion:BocMenuItem>
        </OptionsMenuItems>
      </remotion:BocAutoCompleteReferenceValue>
    </td>
  </tr>
</table>
