// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.DropDownMenuImplementation;
using Remotion.Web.UI.Controls.ListMenuImplementation;
using Image=System.Web.UI.WebControls.Image;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation
{
  public interface IBocList
      : IBusinessObjectBoundEditableWebControl,
        IBocRenderableControl,
        IBocMenuItemContainer
  {
    new bool IsReadOnly { get; }

    new IList Value { get; }

    bool HasNavigator { get; }

    /// <summary>
    ///   Gets a flag set <see langword="true"/> if the <see cref="IBusinessObjectBoundControl.Value"/> is sorted before it is displayed.
    /// </summary>
    bool HasSortingKeys { get; }

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

    int CurrentPage { get; }
    int PageCount { get; }
    bool HasClientScript { get; }
    DropDownList AvailableViewsList { get; }
    IDropDownMenu OptionsMenu { get; }

    System.Collections.Generic.IList<int> SelectorControlCheckedState { get; }
    IEditModeController EditModeController { get; }
    ArrayList Validators { get; }
    BocListRowMenuTuple[] RowMenus { get; }
    System.Collections.Generic.IDictionary<BocColumnDefinition, BocListCustomColumnTuple[]> CustomColumns { get; }
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
    IResourceManager GetResourceManager ();
    string GetListItemCommandArgument (int columnIndex, int originalRowIndex);
    BocListRow[] GetRowsToDisplay (out int startAbsoluteIndex);
    void OnDataRowRendering (BocListDataRowRenderEventArgs args);
    bool AreDataRowsClickSensitive ();
    string GetSelectorControlClientId (int? absoluteRowIndex);
    string GetSelectAllControlClientID ();
    string GetSelectionChangedHandlerScript ();
  }
}