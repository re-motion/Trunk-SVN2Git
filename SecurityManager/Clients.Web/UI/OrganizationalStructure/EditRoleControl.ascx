<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditRoleControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure.EditRoleControl" %>

<remotion:FormGridManager ID="FormGridManager" runat="server" ValidatorVisibility="HideValidators" />
<remotion:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.OrganizationalStructure.Role, Remotion.SecurityManager" />
<table id="FormGrid" runat="server" cellpadding="0" cellspacing="0">
  <tr class="underlinedMarkerCellRow">
    <td class="formGridTitleCell" style="white-space: nowrap;" colspan="2">
      <remotion:SmartLabel runat="server" id="RoleLabel" Text="###" />
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocReferenceValue runat="server" ID="UserField" DataSourceControl="CurrentObject" PropertyIdentifier="User">
        <PersistedCommand>
          <remotion:BocCommand />
        </PersistedCommand>
      </remotion:BocReferenceValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocReferenceValue runat="server" ID="GroupField" DataSourceControl="CurrentObject" PropertyIdentifier="Group" OnSelectionChanged="GroupField_SelectionChanged">
        <PersistedCommand>
          <remotion:BocCommand />
        </PersistedCommand>
        <DropDownListStyle AutoPostBack="True" />
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
