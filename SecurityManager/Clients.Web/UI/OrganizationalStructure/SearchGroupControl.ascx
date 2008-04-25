<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchGroupControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure.SearchGroupControl" %>

<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.OrganizationalStructure.Group, Remotion.SecurityManager" />
<remotion:FormGridManager ID="FormGridManager" runat="server" ValidatorVisibility="HideValidators" />
<table cellpadding="0" cellspacing="0" style="width: 100%; height: 100%;">
  <tr>
    <td style="height: 100%;">
      <table id="FormGrid" runat="server" cellpadding="0" cellspacing="0" style="width: 100%; height: 100%;">
        <tr class="underlinedMarkerCellRow">
          <td class="formGridTitleCell" style="white-space: nowrap;">
            <remotion:SmartLabel runat="server" id="GroupListLabel" Text="###"/>
          </td>
          <td style="DISPLAY: none;WIDTH: 100%"></td>
        </tr>
        <tr>
          <td style="height: 100%; vertical-align: top;">
            <remotion:BocList ID="GroupList" runat="server" DataSourceControl="CurrentObject" OnListItemCommandClick="GroupList_ListItemCommandClick" ShowEmptyListMessage="true" ShowEmptyListReadOnlyMode="true">
              <FixedColumns>
                <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="DisplayName">
                  <PersistedCommand>
                    <remotion:BocListItemCommand />
                  </PersistedCommand>
                </remotion:BocSimpleColumnDefinition>
                <remotion:BocCommandColumnDefinition ItemID="ApplyItem" Text="$res:Apply" Width="0%">
                  <PersistedCommand>
                    <remotion:BocListItemCommand Type="Event" />
                  </PersistedCommand>
                </remotion:BocCommandColumnDefinition>
              </FixedColumns>
            </remotion:BocList>
          </td>
        </tr>
      </table>
    </td>
  </tr>
</table>
