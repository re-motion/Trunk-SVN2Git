<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroupTypeListControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure.GroupTypeListControl" %>

<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.OrganizationalStructure.GroupType, Remotion.SecurityManager" />
<remotion:FormGridManager ID="FormGridManager" runat="server" ValidatorVisibility="HideValidators" />
<table cellpadding="0" cellspacing="0" style="width: 100%; height: 100%;">
  <tr>
    <td style="height: 100%;">
      <table id="FormGrid" runat="server" cellpadding="0" cellspacing="0" style="width: 100%; height: 100%;">
        <tr class="underlinedMarkerCellRow">
          <td class="formGridTitleCell" style="white-space: nowrap;">
            <remotion:SmartLabel runat="server" id="GroupTypeListLabel" Text="###"/>
          </td>
          <td style="DISPLAY: none;WIDTH: 100%"></td>
        </tr>
        <tr>
          <td style="height: 100%; vertical-align: top;">
            <remotion:BocList ID="GroupTypeList" runat="server" DataSourceControl="CurrentObject" OnListItemCommandClick="GroupTypeList_ListItemCommandClick" ShowEmptyListMessage="True" ShowEmptyListReadOnlyMode="True">
              <FixedColumns>
                <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="DisplayName">
                  <PersistedCommand>
                    <remotion:BocListItemCommand Type="Event" />
                  </PersistedCommand>
                </remotion:BocSimpleColumnDefinition>
              </FixedColumns>
            </remotion:BocList>
          </td>
        </tr>
      </table>
    </td>
  </tr>
  <tr>
    <td style="padding-left: 5px; padding-top: 5px;">
      <remotion:WebButton ID="NewGroupTypeButton" runat="server" OnClick="NewGroupTypeButton_Click" />
    </td>
  </tr>
</table>
