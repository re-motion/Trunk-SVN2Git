<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditGroupControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure.EditGroupControl" %>

<remotion:FormGridManager ID="FormGridManager" runat="server" ValidatorVisibility="HideValidators" />
<remotion:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.OrganizationalStructure.Group, Remotion.SecurityManager" />
<table id="FormGrid" runat="server" cellpadding="0" cellspacing="0">
  <tr class="underlinedMarkerCellRow">
    <td class="formGridTitleCell" style="white-space: nowrap;" colspan="2">
      <remotion:SmartLabel runat="server" id="GroupLabel" Text="###" />
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue ID="ShortName" runat="server" DataSourceControl="CurrentObject"
        PropertyIdentifier="ShortName">
        <TextBoxStyle MaxLength="100" />
      </remotion:BocTextValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTextValue ID="NameField" runat="server" DataSourceControl="CurrentObject"
        PropertyIdentifier="Name">
        <TextBoxStyle MaxLength="100" />
      </remotion:BocTextValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocReferenceValue ID="GroupTypeField" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="GroupType">
      <PersistedCommand>
        <remotion:BocCommand />
      </PersistedCommand>
    </remotion:BocReferenceValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocReferenceValue ID="ParentField" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Parent">
        <PersistedCommand>
          <remotion:BocCommand />
        </PersistedCommand>
      </remotion:BocReferenceValue>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocList ID="ChildrenList" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Children" OnMenuItemClick="ChildrenList_MenuItemClick" Selection="Multiple" ShowEmptyListMessage="true" ShowEmptyListReadOnlyMode="true">
        <FixedColumns>
          <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="DisplayName">
            <PersistedCommand>
              <remotion:BocListItemCommand />
            </PersistedCommand>
          </remotion:BocSimpleColumnDefinition>
        </FixedColumns>
        <ListMenuItems>
          <remotion:BocMenuItem ItemID="AddItem" Text="$res:Add">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Show="EditMode" />
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="RemoveItem" RequiredSelection="OneOrMore" Text="$res:Remove">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Show="EditMode" />
            </PersistedCommand>
          </remotion:BocMenuItem>
        </ListMenuItems>
      </remotion:BocList>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocList ID="RolesList" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Roles" OnMenuItemClick="RolesList_MenuItemClick" Selection="Multiple" ShowEmptyListMessage="true" ShowEmptyListReadOnlyMode="true">
        <FixedColumns>
          <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="User">
            <PersistedCommand>
              <remotion:BocListItemCommand />
            </PersistedCommand>
          </remotion:BocSimpleColumnDefinition>
          <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="Position">
            <PersistedCommand>
              <remotion:BocListItemCommand />
            </PersistedCommand>
          </remotion:BocSimpleColumnDefinition>
        </FixedColumns>
        <ListMenuItems>
          <remotion:BocMenuItem ItemID="NewItem" Text="$res:New">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Show="EditMode" />
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="EditItem" RequiredSelection="ExactlyOne" Text="$res:Edit">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Show="EditMode" />
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="DeleteItem" RequiredSelection="OneOrMore" Text="$res:Delete">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Show="EditMode" />
            </PersistedCommand>
          </remotion:BocMenuItem>
        </ListMenuItems>
      </remotion:BocList>
    </td>
  </tr>
</table>
