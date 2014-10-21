<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BocAutoCompleteReferenceValueUserControl.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls.BocAutoCompleteReferenceValueUserControl" %>
<remotion:FormGridManager ID="FormGridManager" runat="server" />
<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
<table id="FormGrid" runat="server">
  <tr>
    <td></td>
    <td>
      <remotion:BocAutoCompleteReferenceValue ID="PartnerField_Normal"
        SearchServicePath="AutoCompleteService.asmx"
        TextBoxStyle-AutoPostBack="true"
        ReadOnly="False"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="Partner"
        CompletionSetCount="5"
        IconServicePath="IconService.asmx"
        runat="server">
        
        <PersistedCommand>
          <remotion:BocCommand Type="Event"></remotion:BocCommand>
        </PersistedCommand>

        <OptionsMenuItems>
          <remotion:BocMenuItem ItemID="OptCmd1" Text="My menu command">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd2" Text="My menu command 2">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd3" Text="My menu command 3">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
        </OptionsMenuItems>
      </remotion:BocAutoCompleteReferenceValue>
    </td>
    <td>(normal)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocAutoCompleteReferenceValue ID="PartnerField_Normal_AlternativeRendering"
        SearchServicePath="AutoCompleteService.asmx"
        TextBoxStyle-AutoPostBack="true"
        ReadOnly="False"
        HasValueEmbeddedInsideOptionsMenu="True"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="Partner"
        CompletionSetCount="5"
        IconServicePath="IconService.asmx"
        runat="server">
        
        <PersistedCommand>
          <remotion:BocCommand Type="Event"></remotion:BocCommand>
        </PersistedCommand>

        <OptionsMenuItems>
          <remotion:BocMenuItem ItemID="OptCmd1" Text="My menu command">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd2" Text="My menu command 2">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd3" Text="My menu command 3">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
        </OptionsMenuItems>
      </remotion:BocAutoCompleteReferenceValue>
    </td>
    <td>(normal, alternative rendering)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocAutoCompleteReferenceValue ID="PartnerField_ReadOnly"
        SearchServicePath="AutoCompleteService.asmx"
        TextBoxStyle-AutoPostBack="true"
        ReadOnly="True"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="Partner"
        CompletionSetCount="5"
        IconServicePath="IconService.asmx"
        runat="server">
        
        <PersistedCommand>
          <remotion:BocCommand Type="Event"></remotion:BocCommand>
        </PersistedCommand>

        <OptionsMenuItems>
          <remotion:BocMenuItem ItemID="OptCmd1" Text="My menu command">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd2" Text="My menu command 2">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd3" Text="My menu command 3">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
        </OptionsMenuItems>
      </remotion:BocAutoCompleteReferenceValue>
    </td>
    <td>(read-only)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocAutoCompleteReferenceValue ID="PartnerField_ReadOnly_AlternativeRendering"
        SearchServicePath="AutoCompleteService.asmx"
        TextBoxStyle-AutoPostBack="true"
        ReadOnly="True"
        HasValueEmbeddedInsideOptionsMenu="False"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="Partner"
        CompletionSetCount="5"
        IconServicePath="IconService.asmx"
        runat="server">
        
        <PersistedCommand>
          <remotion:BocCommand Type="Event"></remotion:BocCommand>
        </PersistedCommand>

        <OptionsMenuItems>
          <remotion:BocMenuItem ItemID="OptCmd1" Text="My menu command">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd2" Text="My menu command 2">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd3" Text="My menu command 3">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
        </OptionsMenuItems>
      </remotion:BocAutoCompleteReferenceValue>
    </td>
    <td>(read-only, alternative rendering)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocAutoCompleteReferenceValue ID="PartnerField_Disabled"
        SearchServicePath="AutoCompleteService.asmx"
        TextBoxStyle-AutoPostBack="false"
        Enabled="False"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="Partner"
        CompletionSetCount="5"
        IconServicePath="IconService.asmx"
        runat="server">
        
        <PersistedCommand>
          <remotion:BocCommand Type="Event"></remotion:BocCommand>
        </PersistedCommand>

        <OptionsMenuItems>
          <remotion:BocMenuItem ItemID="OptCmd1" Text="My menu command">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd2" Text="My menu command 2">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd3" Text="My menu command 3">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
        </OptionsMenuItems>
      </remotion:BocAutoCompleteReferenceValue>
    </td>
    <td>(disabled)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocAutoCompleteReferenceValue ID="PartnerField_NoAutoPostBack"
        SearchServicePath="AutoCompleteService.asmx"
        TextBoxStyle-AutoPostBack="false"
        ReadOnly="False"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="Partner"
        CompletionSetCount="5"
        IconServicePath="IconService.asmx"
        runat="server">
        
        <PersistedCommand>
          <remotion:BocCommand Type="Event"></remotion:BocCommand>
        </PersistedCommand>

        <OptionsMenuItems>
          <remotion:BocMenuItem ItemID="OptCmd1" Text="My menu command">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd2" Text="My menu command 2">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd3" Text="My menu command 3">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
        </OptionsMenuItems>
      </remotion:BocAutoCompleteReferenceValue>
    </td>
    <td>(no auto postback)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocAutoCompleteReferenceValue ID="PartnerField_NoCommandNoMenu"
        SearchServicePath="AutoCompleteService.asmx"
        TextBoxStyle-AutoPostBack="false"
        ReadOnly="False"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="Partner"
        CompletionSetCount="5"
        IconServicePath="IconService.asmx"
        runat="server">
      </remotion:BocAutoCompleteReferenceValue>
    </td>
    <td>(no command & no menu)</td>
  </tr>
</table>
