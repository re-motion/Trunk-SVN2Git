<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditGroupTypePositionControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure.EditGroupTypePositionControl" %>

<remotion:FormGridManager ID="FormGridManager" runat="server" ValidatorVisibility="HideValidators" />
<remotion:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.OrganizationalStructure.GroupTypePosition, Remotion.SecurityManager" />
<table id="FormGrid" runat="server" cellpadding="0" cellspacing="0">
  <tr class="underlinedMarkerCellRow">
    <td class="formGridTitleCell" style="white-space: nowrap;" colspan="2">
      <remotion:SmartLabel runat="server" id="GroupTypePositionLabel" Text="###" />
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocReferenceValue runat="server" ID="GroupTypeField" DataSourceControl="CurrentObject" PropertyIdentifier="GroupType">
        <PersistedCommand>
          <remotion:BocCommand />
        </PersistedCommand>
      </remotion:BocReferenceValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocReferenceValue runat="server" ID="PositionField" DataSourceControl="CurrentObject" PropertyIdentifier="Position">
        <PersistedCommand>
          <remotion:BocCommand />
        </PersistedCommand>
      </remotion:BocReferenceValue>
    </td>
  </tr>
</table>
