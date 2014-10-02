﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BocListUserControl.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls.BocListUserControl" %>
<remotion:FormGridManager ID="FormGridManager" runat="server" />
<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
<table id="FormGrid" runat="server">
  <tr>
    <td></td>
    <td>
      <testsite:TestBocListWithRowMenuItems
        ID="JobList_Normal"
        ReadOnly="False"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="Jobs"
        
        AlwaysShowPageInfo="True"
        AvailableViewsListTitle="V_iews:"
        EnableEditModeValidator="true"
        EnableMultipleSorting="true"
        EnableSorting="true"
        Index="SortedOrder"
        IndexColumnTitle="I_ndex"
        IndexOffset="10"
        ListMenuLineBreaks="BetweenGroups"
        OptionsTitle="O_ptions:"
        PageSize="2"
        RowMenuDisplay="Manual"
        Selection="Multiple"
        ShowAllProperties="True"
        ShowAvailableViewsList="True"
        ShowEditModeRequiredMarkers="true"
        ShowEditModeValidationMarkers="true"
        ShowListMenu="true"
        ShowOptionsMenu="true"
        ShowSortingOrder="True"
        
        Width="100%"
        Height="10em"
        runat="server">
        <OptionsMenuItems>
          <remotion:BocMenuItem ItemID="OptCmd1" Text="Option command">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd2" Text="Option command 2">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd3" Text="Option command 3">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
        </OptionsMenuItems>
        <ListMenuItems>
          <remotion:BocMenuItem ItemID="ListMenuCmd1" Text="LM cmd">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="ListMenuCmd2" Icon-Url="~/Images/SampleIcon.gif" Icon-AlternateText="SampleIcon" Icon-ToolTip="SampleIcon">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="ListMenuCmd3" Text="LM cmd 3">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
        </ListMenuItems>
        <FixedColumns>
          <remotion:BocRowEditModeColumnDefinition ItemID="EditRow" EditText="Edit" SaveText="Save" CancelText="Cancel" Width="2em" />
          <remotion:BocCommandColumnDefinition ItemID="RowCmd" Text="Row command" ColumnTitle="Command">
            <PersistedCommand>
              <remotion:BocListItemCommand Type="Event" CommandStateType="Remotion.ObjectBinding.Sample::PersonListItemCommandState"></remotion:BocListItemCommand>
            </PersistedCommand>
          </remotion:BocCommandColumnDefinition>
          <remotion:BocDropDownMenuColumnDefinition ItemID="RowMenu" MenuTitleText="Context" Width="16px" ColumnTitle="Menu"/>
          <remotion:BocAllPropertiesPlaceholderColumnDefinition/>
          <remotion:BocSimpleColumnDefinition ColumnTitle="TitleWithCmd" PropertyPathIdentifier="Title">
            <PersistedCommand>
              <remotion:BocListItemCommand Type="Event" CommandStateType="Remotion.ObjectBinding.Sample::PersonListItemCommandState"></remotion:BocListItemCommand>
            </PersistedCommand>
          </remotion:BocSimpleColumnDefinition>
        </FixedColumns>
      </testsite:TestBocListWithRowMenuItems>
    </td>
    <td>&nbsp; (normal)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <testsite:TestBocListWithRowMenuItems
        ID="JobList_ReadOnly"
        ReadOnly="True"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="Jobs"
        
        AlwaysShowPageInfo="True"
        AvailableViewsListTitle="V_iews:"
        EnableEditModeValidator="true"
        EnableMultipleSorting="true"
        EnableSorting="true"
        Index="SortedOrder"
        IndexColumnTitle="I_ndex"
        IndexOffset="10"
        ListMenuLineBreaks="BetweenGroups"
        OptionsTitle="O_ptions:"
        PageSize="2"
        RowMenuDisplay="Manual"
        Selection="Multiple"
        ShowAllProperties="True"
        ShowAvailableViewsList="True"
        ShowEditModeRequiredMarkers="true"
        ShowEditModeValidationMarkers="true"
        ShowListMenu="true"
        ShowOptionsMenu="true"
        ShowSortingOrder="True"
        
        Width="100%"
        Height="10em"
        runat="server">
        <OptionsMenuItems>
          <remotion:BocMenuItem ItemID="OptCmd1" Text="Option command">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd2" Text="Option command 2">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="OptCmd3" Text="Option command 3">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
        </OptionsMenuItems>
        <ListMenuItems>
          <remotion:BocMenuItem ItemID="ListMenuCmd1" Text="LM cmd">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="ListMenuCmd2" Icon-Url="~/Images/SampleIcon.gif" Icon-AlternateText="SampleIcon" Icon-ToolTip="SampleIcon">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem ItemID="ListMenuCmd3" Text="LM cmd 3">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
        </ListMenuItems>
        <FixedColumns>
          <remotion:BocRowEditModeColumnDefinition ItemID="EditRow" EditText="Edit" SaveText="Save" CancelText="Cancel" Width="2em" />
          <remotion:BocCommandColumnDefinition ItemID="RowCmd" Text="Row command" ColumnTitle="Command">
            <PersistedCommand>
              <remotion:BocListItemCommand Type="Event" CommandStateType="Remotion.ObjectBinding.Sample::PersonListItemCommandState"></remotion:BocListItemCommand>
            </PersistedCommand>
          </remotion:BocCommandColumnDefinition>
          <remotion:BocDropDownMenuColumnDefinition ItemID="RowMenu" MenuTitleText="Context" Width="16px" ColumnTitle="Menu"/>
          <remotion:BocAllPropertiesPlaceholderColumnDefinition/>
          <remotion:BocSimpleColumnDefinition ColumnTitle="TitleWithCmd" PropertyPathIdentifier="Title">
            <PersistedCommand>
              <remotion:BocListItemCommand Type="Event" CommandStateType="Remotion.ObjectBinding.Sample::PersonListItemCommandState"></remotion:BocListItemCommand>
            </PersistedCommand>
          </remotion:BocSimpleColumnDefinition>
        </FixedColumns>
      </testsite:TestBocListWithRowMenuItems>  
    </td>
    <td>&nbsp; (read-only)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <testsite:TestBocListWithRowMenuItems
        ID="JobList_NoItemIDs"
        ReadOnly="False"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="Jobs"
        
        AlwaysShowPageInfo="True"
        AvailableViewsListTitle="V_iews:"
        EnableEditModeValidator="true"
        EnableMultipleSorting="true"
        EnableSorting="true"
        Index="SortedOrder"
        IndexColumnTitle="I_ndex"
        IndexOffset="10"
        ListMenuLineBreaks="BetweenGroups"
        OptionsTitle="O_ptions:"
        PageSize="2"
        RowMenuDisplay="Manual"
        Selection="Multiple"
        ShowAllProperties="True"
        ShowAvailableViewsList="True"
        ShowEditModeRequiredMarkers="true"
        ShowEditModeValidationMarkers="true"
        ShowListMenu="true"
        ShowOptionsMenu="true"
        ShowSortingOrder="True"
        
        Width="100%"
        Height="10em"
        runat="server">
        <OptionsMenuItems>
          <remotion:BocMenuItem Text="Option command">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem Text="Option command 2">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem Text="Option command 3">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
        </OptionsMenuItems>
        <ListMenuItems>
          <remotion:BocMenuItem Text="LM cmd">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem Icon-Url="~/Images/SampleIcon.gif">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
          <remotion:BocMenuItem Text="LM cmd 3">
            <PersistedCommand>
              <remotion:BocMenuItemCommand Type="Event"></remotion:BocMenuItemCommand>
            </PersistedCommand>
          </remotion:BocMenuItem>
        </ListMenuItems>
        <FixedColumns>
          <remotion:BocRowEditModeColumnDefinition EditText="Edit" SaveText="Save" CancelText="Cancel" Width="2em" />
          <remotion:BocCommandColumnDefinition Text="Row command" ColumnTitle="Command">
            <PersistedCommand>
              <remotion:BocListItemCommand Type="Event" CommandStateType="Remotion.ObjectBinding.Sample::PersonListItemCommandState"></remotion:BocListItemCommand>
            </PersistedCommand>
          </remotion:BocCommandColumnDefinition>
          <remotion:BocDropDownMenuColumnDefinition MenuTitleText="" Width="16px" ColumnTitle=""/>
          <remotion:BocAllPropertiesPlaceholderColumnDefinition/>
          <remotion:BocSimpleColumnDefinition ColumnTitle="" PropertyPathIdentifier="Title">
            <PersistedCommand>
              <remotion:BocListItemCommand Type="Event" CommandStateType="Remotion.ObjectBinding.Sample::PersonListItemCommandState"></remotion:BocListItemCommand>
            </PersistedCommand>
          </remotion:BocSimpleColumnDefinition>
        </FixedColumns>
      </testsite:TestBocListWithRowMenuItems>
    </td>
    <td>&nbsp; (no ItemIDs)</td>
  </tr>
</table>