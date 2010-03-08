// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering.DropDownMenu;
using Remotion.Web.UI.Controls.Rendering.ListMenu;
using Remotion.Web.UI.Globalization;
using Image=System.Web.UI.WebControls.Image;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public interface IBocList
      : IBusinessObjectBoundEditableWebControl,
        IBocRenderableControl,
        IPostBackEventHandler,
        IPostBackDataHandler,
        IBocMenuItemContainer,
        IBocListSortingOrderProvider,
        IResourceDispatchTarget
  {
    /// <summary> Is raised when the sorting order of the <see cref="BocList"/> is about to change. </summary>
    /// <remarks> Will only be raised, if the change was caused by an UI action. </remarks>
    event BocListSortingOrderChangeEventHandler SortingOrderChanging;

    /// <summary> Is raised when the sorting order of the <see cref="BocList"/> has changed. </summary>
    /// <remarks> Will only be raised, if the change was caused by an UI action. </remarks>
    event BocListSortingOrderChangeEventHandler SortingOrderChanged;

    new bool IsReadOnly { get; }

    new IList Value { get; }

    bool HasNavigator { get; }

    /// <summary>
    ///   Gets a flag set <see langword="true"/> if the <see cref="IBusinessObjectBoundControl.Value"/> is sorted before it is displayed.
    /// </summary>
    bool HasSortingKeys { get; }

    /// <summary> Gets the user independent column definitions. </summary>
    /// <remarks> Behavior undefined if set after initialization phase or changed between postbacks. </remarks>
    BocColumnDefinitionCollection FixedColumns { get; }

    /// <summary> Gets the predefined column definition sets that the user can choose from at run-time. </summary>
    BocListViewCollection AvailableViews { get; }

    /// <summary>
    ///   Gets or sets the selected <see cref="BocListView"/> used to
    ///   supplement the <see cref="FixedColumns"/>.
    /// </summary>
    BocListView SelectedView { get; }

    /// <summary>
    ///   Gets or sets a flag that determines whether to show the asterisks in the title row for columns having 
    ///   edit mode controls.
    /// </summary>
    bool ShowEditModeRequiredMarkers { get; }

    /// <summary>
    ///   Gets or sets a flag that determines whether to show an exclamation mark in front of each control with 
    ///   an validation error.
    /// </summary>
    bool ShowEditModeValidationMarkers { get; }

    /// <summary>
    ///   Gets or sets a flag that determines whether to render validation messages and client side validators.
    /// </summary>
    bool DisableEditModeValidationMessages { get; }

    /// <summary> Gets or sets a flag that enables the <see cref="EditModeValidator"/>. </summary>
    bool EnableEditModeValidator { get; }

    /// <summary> 
    ///   Gets or sets the <see cref="EditableRowDataSourceFactory"/> used to create the data souce for the edit mode
    ///   controls.
    /// </summary>
    EditableRowDataSourceFactory EditModeDataSourceFactory { get; }

    /// <summary> 
    ///   Gets or sets the <see cref="EditableRowControlFactory"/> used to create the controls for the edit mode.
    /// </summary>
    EditableRowControlFactory EditModeControlFactory { get; }

    /// <summary> 
    ///   Gets or sets a flag that determines wheter an empty list will still render its headers 
    ///   and the additonal column sets  (read-only mode only). 
    /// </summary>
    /// <value> <see langword="false"/> to hide the headers and the addtional column sets if the list is empty. </value>
    bool ShowEmptyListReadOnlyMode { get; }

    /// <summary> 
    ///   Gets or sets a flag that determines wheter an empty list will still render its headers 
    ///   and the additonal column sets (edit mode only). 
    /// </summary>
    /// <value> <see langword="false"/> to hide the headers and the addtional column sets if the list is empty. </value>
    bool ShowEmptyListEditMode { get; }

    /// <summary> 
    ///   Gets or sets a flag that determines wheter an empty list will still render its option and list menus. 
    ///   (read-only mode only).
    /// </summary>
    /// <value> <see langword="false"/> to hide the option and list menus if the list is empty. </value>
    bool ShowMenuForEmptyListReadOnlyMode { get; }

    /// <summary> 
    ///   Gets or sets a flag that determines wheter an empty list will still render its option and list menus
    ///   (edit mode only).
    /// </summary>
    /// <value> <see langword="false"/> to hide the option and list menus if the list is empty. </value>
    bool ShowMenuForEmptyListEditMode { get; }

    /// <summary>
    ///   Gets or sets a flag that indicates whether the control automatically generates a column 
    ///   for each property of the bound object.
    /// </summary>
    /// <value> <see langword="true"/> show all properties of the bound business object. </value>
    bool ShowAllProperties { get; }

    /// <summary>
    ///   Gets or sets a flag that indicates whether to display an icon in front of the first value 
    ///   column.
    /// </summary>
    /// <value> <see langword="true"/> to enable the icon. </value>
    bool EnableIcon { get; }

    /// <summary>
    ///   Gets or sets a flag that determines whether to to enable cleint side sorting.
    /// </summary>
    /// <value> <see langword="true"/> to enable the sorting buttons. </value>
    bool EnableSorting { get; }

    /// <summary>
    ///   Gets or sets a flag that determines whether to display the sorting order index 
    ///   after each sorting button.
    /// </summary>
    /// <remarks> 
    ///   Only displays the index if more than one column is included in the sorting.
    /// </remarks>
    /// <value> 
    ///   <see langword="NaBooleanEnum.True"/> to show the sorting order index after the button. 
    ///   Defaults to <see langword="null"/>, which is interpreted as <see langword="true"/>.
    /// </value>
    bool? ShowSortingOrder { get; }

    bool? EnableMultipleSorting { get; }

    /// <summary>
    ///   Gets or sets a flag that determines whether to display the options menu.
    /// </summary>
    /// <value> <see langword="true"/> to show the options menu. </value>
    bool ShowOptionsMenu { get; }

    /// <summary>
    ///   Gets or sets a flag that determines whether to display the list menu.
    /// </summary>
    /// <value> <see langword="true"/> to show the list menu. </value>
    bool ShowListMenu { get; }

    /// <summary> Gets or sets a value that determines if the row menu is being displayed. </summary>
    /// <value> <see cref="Controls.RowMenuDisplay.Undefined"/> is interpreted as <see cref="Controls.RowMenuDisplay.Disabled"/>. </value>
    RowMenuDisplay RowMenuDisplay { get; }

    /// <summary>
    ///   Gets or sets a value that indicating the row selection mode.
    /// </summary>
    /// <remarks> 
    ///   If row selection is enabled, the control displays a checkbox in front of each row
    ///   and highlights selected data rows.
    /// </remarks>
    RowSelection Selection { get; }

    bool IsIndexEnabled { get; }

    /// <summary> Gets or sets a value that indicating the row index is enabled. </summary>
    /// <value> 
    ///   <see langword="RowIndex.InitialOrder"/> to show the of the initial (unsorted) list and
    ///   <see langword="RowIndex.SortedOrder"/> to show the index based on the current sorting order. 
    ///   Defaults to <see cref="RowIndex.Undefined"/>, which is interpreted as <see langword="RowIndex.Disabled"/>.
    /// </value>
    /// <remarks> If row selection is enabled, the control displays an index in front of each row. </remarks>
    RowIndex Index { get; }

    /// <summary> Gets or sets the offset for the rendered index. </summary>
    /// <value> Defaults to <see langword="null"/>. </value>
    int? IndexOffset { get; }

    /// <summary> Gets or sets the text that is displayed in the index column's title row. </summary>
    /// <remarks> The value will not be HTML encoded. </remarks>
    string IndexColumnTitle { get; }

    /// <summary> The number of rows displayed per page. </summary>
    /// <value> 
    ///   An integer greater than zero to limit the number of rows per page to the specified value,
    ///   or zero, less than zero or <see langword="null"/> to show all rows.
    /// </value>
    int? PageSize { get; }

    /// <summary>
    ///   Gets or sets a flag that indicates whether to the show the page count even when there 
    ///   is just one page.
    /// </summary>
    /// <value> 
    ///   <see langword="true"/> to force showing the page info, even if the rows fit onto a single 
    ///   page.
    /// </value>
    bool AlwaysShowPageInfo { get; }

    /// <summary> Gets or sets the text providing the current page information to the user. </summary>
    /// <remarks> Use {0} for the current page and {1} for the total page count. The value will not be HTML encoded. </remarks>
    string PageInfo { get; }

    /// <summary> Gets or sets the text rendered if the list is empty. </summary>
    /// <remarks> The value will not be HTML encoded. </remarks>
    string EmptyListMessage { get; }

    /// <summary> Gets or sets a flag whether to render the <see cref="BocList.EmptyListMessage"/>. </summary>
    bool ShowEmptyListMessage { get; }

    /// <summary> Gets or sets a flag that determines whether the client script is enabled. </summary>
    /// <remarks> Effects only advanced scripts used for selcting data rows. </remarks>
    /// <value> <see langref="true"/> to enable the client script. </value>
    bool EnableClientScript { get; }

    /// <summary> Gets or sets the offset between the items in the <c>menu block</c>. </summary>
    /// <remarks> The <see cref="MenuBlockOffset"/> is applied as a <c>margin</c> attribute. </remarks>
    Unit MenuBlockItemOffset { get; }

    /// <summary> Gets the <see cref="BocMenuItem"/> objects displayed in the <see cref="BocList"/>'s options menu. </summary>
    WebMenuItemCollection OptionsMenuItems { get; }

    /// <summary> Gets the <see cref="BocMenuItem"/> objects displayed in the <see cref="BocList"/>'s menu area. </summary>
    WebMenuItemCollection ListMenuItems { get; }

    /// <summary> Gets or sets the width reserved for the menu block. </summary>
    Unit MenuBlockWidth { get; }

    /// <summary> Gets or sets the offset between the <c>list block</c> and the <c>menu block</c>. </summary>
    /// <remarks> The <see cref="MenuBlockOffset"/> is applied as a <c>padding</c> attribute. </remarks>
    Unit MenuBlockOffset { get; }

    /// <summary> Gets or sets the list of menu items to be hidden. </summary>
    /// <value> The <see cref="WebMenuItem.ItemID"/> values of the menu items to hide. </value>
    string[] HiddenMenuItems { get; }

    /// <summary>
    ///   Gets or sets a value that indicates whether the control displays a drop down list 
    ///   containing the available column definition sets.
    /// </summary>
    bool ShowAvailableViewsList { get; }

    /// <summary> Gets or sets the text that is rendered as a title for the drop list of additional columns. </summary>
    /// <remarks> The value will not be HTML encoded. </remarks>
    string AvailableViewsListTitle { get; }

    /// <summary> Gets or sets the text that is rendered as a label for the <c>options menu</c>. </summary>
    string OptionsTitle { get; }

    /// <summary> Gets or sets the rendering option for the <c>list menu</c>. </summary>
    ListMenuLineBreaks ListMenuLineBreaks { get; }

    /// <summary> Gets or sets the validation error message. </summary>
    /// <value> 
    ///   The error message displayed when validation fails. The default value is an empty <see cref="String"/>.
    ///   In case of the default value, the text is read from the resources for this control.
    /// </value>
    string ErrorMessage { get; }

    int CurrentPage { get; }
    int PageCount { get; }
    bool HasClientScript { get; }
    DropDownList AvailableViewsList { get; }
    IDropDownMenu OptionsMenu { get; }

    /// <summary>Gets the <see cref="BusinessObjectBinding"/> object used to manage the binding for this <see cref="BusinessObjectBoundWebControl"/>.</summary>
    /// <value> The <see cref="BusinessObjectBinding"/> instance used to manage this control's binding. </value>
    BusinessObjectBinding Binding { get; }

    string AccessKey { get; }
    Color BackColor { get; }
    Color BorderColor { get; }
    Unit BorderWidth { get; }
    BorderStyle BorderStyle { get; }
    bool ControlStyleCreated { get; }
    FontInfo Font { get; }
    Color ForeColor { get; }
    bool HasAttributes { get; }
    short TabIndex { get; }
    string ToolTip { get; }
    System.Collections.Generic.IList<int> SelectorControlCheckedState { get; }
    IEditModeController EditModeController { get; }
    ArrayList SortingOrder { get; }
    ArrayList Validators { get; }
    BocListRowMenuTuple[] RowMenus { get; }
    System.Collections.Generic.IDictionary<BocColumnDefinition, BocListCustomColumnTuple[]> CustomColumns { get; }
    bool IsEmptyList { get; }
    bool HasListMenu { get; }
    bool IsClientSideSortingEnabled { get; }
    bool HasOptionsMenu { get; }
    bool HasAvailableViewsList { get; }
    bool HasMenuBlock { get; }
    bool IsPagingEnabled { get; }
    bool IsShowSortingOrderEnabled { get; }
    IListMenu ListMenu { get; }

    /// <summary> Builds the validation error marker. </summary>
    Image GetValidationErrorMarker ();

    /// <summary>
    ///   Obtains a reference to a client-side script function that causes, when invoked, a server postback to the form.
    /// </summary>
    /// <remarks> 
    ///   <para>
    ///     If the <see cref="BocList"/> is in row edit mode, <c>return false;</c> will be returned to prevent actions on 
    ///     this list.
    ///   </para><para>
    ///     Insert the return value in the rendered onClick event.
    ///   </para>
    /// </remarks>
    /// <param name="columnIndex"> The index of the column for which the post back function should be created. </param>
    /// <param name="listIndex"> The index of the business object for which the post back function should be created. </param>
    /// <param name="customCellArgument"> 
    ///   The argument to be passed to the <see cref="BocCustomColumnDefinitionCell"/>'s <c>OnClick</c> method.
    ///   Can be <see langword="null"/>.
    /// </param>
    /// <returns></returns>
    string GetCustomCellPostBackClientEvent (int columnIndex, int listIndex, string customCellArgument);

    /// <summary>
    ///   Sorts the <see cref="BocList.Value"/>'s <see cref="IBusinessObject"/> instances using the sorting keys
    ///   and returns the sorted <see cref="IBusinessObject"/> instances as a new array. The original values remain
    ///   untouched.
    /// </summary>
    /// <returns> 
    ///   An <see cref="IBusinessObject"/> array sorted by the sorting keys or <see langword="null"/> if the list is
    ///   not sorted.
    /// </returns>
    IBusinessObject[] GetSortedRows ();

    /// <summary> Is raised when a data row is rendered. </summary>
    event BocListDataRowRenderEventHandler DataRowRender;

    void ClearSelectedRows ();

    /// <summary> Gets indices for the rows selected in the <see cref="BocList"/>. </summary>
    /// <returns> An array of <see cref="int"/> values. </returns>
    int[] GetSelectedRows ();

    /// <summary> Is raised before the changes to the editable row are saved. </summary>
    event BocListEditableRowChangesEventHandler EditableRowChangesSaving;

    /// <summary> Is raised after the changes to the editable row have been saved. </summary>
    event BocListItemEventHandler EditableRowChangesSaved;

    /// <summary> Is raised before the changes to the editable row are canceled. </summary>
    event BocListEditableRowChangesEventHandler EditableRowChangesCanceling;

    /// <summary> Is raised after the changes to the editable row have been canceled. </summary>
    event BocListItemEventHandler EditableRowChangesCanceled;

    /// <summary> Is raised when a column type <see cref="BocCustomColumnDefinition"/> is clicked on. </summary>
    event BocCustomCellClickEventHandler CustomCellClick;

    /// <summary> Is raised when a column with a command of type <see cref="CommandType.Event"/> is clicked. </summary>
    event BocListItemCommandClickEventHandler ListItemCommandClick;

    /// <summary> Is raised when a menu item with a command of type <see cref="CommandType.Event"/> is clicked. </summary>
    event WebMenuItemClickEventHandler MenuItemClick;

    void ApplyStyle (Style s);
    void CopyBaseAttributes (WebControl controlSrc);
    void MergeStyle (Style s);
    void RenderBeginTag (HtmlTextWriter writer);
    void RenderEndTag (HtmlTextWriter writer);
    IResourceManager GetResourceManager ();
    BocColumnDefinition[] GetColumns ();
    bool IsColumnVisible (BocColumnDefinition columnDefinition);
    string GetListItemCommandArgument (int columnIndex, int originalRowIndex);
    BocListRow[] GetRowsToDisplay (out int startAbsoluteIndex);
    void OnDataRowRendering (BocListDataRowRenderEventArgs args);
    bool AreDataRowsClickSensitive ();
    void OnPreRender ();
    void OnLoad ();
    void SwitchRowIntoEditMode (int rowIndex);
    string GetSelectorControlClientId (int? absoluteRowIndex);
    string GetSelectAllControlClientID ();
  }
}
