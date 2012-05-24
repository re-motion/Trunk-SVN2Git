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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.Logging;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Sorting;
using Remotion.ObjectBinding.Web.UI.Design;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.DropDownMenuImplementation;
using Remotion.Web.UI.Controls.ListMenuImplementation;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;
using StringArrayConverter=Remotion.Web.UI.Design.StringArrayConverter;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> 
  ///   This control can be used to display and edit a list of <see cref="IBusinessObject"/> instances.
  ///   The properties of the business objects are displayed in individual columns. 
  /// </summary>
  /// <include file='..\..\doc\include\UI\Controls\BocList.xml' path='BocList/Class/*' />
  // TODO: see "Doc\Bugs and ToDos.txt"
  [Designer (typeof (BocListDesigner))]
  [DefaultEvent ("CommandClick")]
  [ToolboxItemFilter ("System.Web.UI")]
  public class BocList : 
      BusinessObjectBoundEditableWebControl, 
      IBocList,
      IPostBackEventHandler,
      IPostBackDataHandler,
      IResourceDispatchTarget
  {
    #region Obsoletes

    [Obsolete("Use EnsureSortedBocListRowsGot instead. (Version 1.13.52)")]
    protected BocListRow[] EnsureGotIndexedRowsSorted()
    {
      return EnsureSortedBocListRowsGot();
    }

    [Obsolete("Use EnsureSortedBocListRowsGot instead. (Version 1.13.52)", true)]
    protected BocListRow[] GetIndexedRows(bool sorted)
    {
      throw new NotSupportedException ("Use EnsureSortedBocListRowsGot instead. (Version 1.13.52)");
    }

    #endregion

    //  constants
    private const string c_dataRowSelectorControlIDSuffix = "_Boc_SelectorControl_";
    private const string c_titleRowSelectorControlIDSuffix = "_Boc_SelectorControl_SelectAll";
    private const string c_availableViewsListIDSuffix = "_Boc_AvailableViewsList";
    private const string c_optionsMenuIDSuffix = "_Boc_OptionsMenu";
    private const string c_listMenuIDSuffix = "_Boc_ListMenu";

    private const int c_titleRowIndex = -1;

    /// <summary> Prefix applied to the post back argument of the event type column commands. </summary>
    private const string c_eventListItemCommandPrefix = "ListCommand=";

    /// <summary> Prefix applied to the post back argument of the custom columns. </summary>
    private const string c_customCellEventPrefix = "CustomCell=";

    private const string c_eventRowEditModePrefix = "RowEditMode=";
    private const string c_rowEditModeRequiredFieldIcon = "RequiredField.gif";
    private const string c_rowEditModeValidationErrorIcon = "ValidationError.gif";

    /// <summary> Prefix applied to the post back argument of the sort buttons. </summary>
    public const string SortCommandPrefix = "Sort=";

    public const string GoToCommandPrefix = "GoTo=";


    /// <summary> The key identifying a fixed column resource entry. </summary>
    private const string c_resourceKeyFixedColumns = "FixedColumns";

    /// <summary> The key identifying a options menu item resource entry. </summary>
    private const string c_resourceKeyOptionsMenuItems = "OptionsMenuItems";

    /// <summary> The key identifying a list menu item resource entry. </summary>
    private const string c_resourceKeyListMenuItems = "ListMenuItems";

    // types

    /// <summary> A list of control wide resources. </summary>
    /// <remarks> 
    ///   Resources will be accessed using 
    ///   <see cref="IResourceManager.GetString (Enum)"/>. 
    /// </remarks>
    [ResourceIdentifiers]
    [MultiLingualResources ("Remotion.ObjectBinding.Web.Globalization.BocList")]
    public enum ResourceIdentifier
    {
      EmptyListMessage,
      PageInfo,
      OptionsTitle,
      AvailableViewsListTitle,
      /// <summary>The alternate text for the required icon.</summary>
      RequiredFieldAlternateText,
      /// <summary>The tool tip text for the required icon.</summary>
      RequiredFieldTitle,
      /// <summary>The alternate text for the validation error icon.</summary>
      ValidationErrorInfoAlternateText,
      /// <summary> The alternate text for the sort ascending button. </summary>
      SortAscendingAlternateText,
      /// <summary> The alternate text for the sort descending button. </summary>
      SortDescendingAlternateText,
      RowEditModeErrorMessage,
      ListEditModeErrorMessage,
      RowEditModeEditAlternateText,
      RowEditModeSaveAlternateText,
      RowEditModeCancelAlternateText,
      GoToFirstAlternateText,
      GoToLastAlternateText,
      GoToNextAlternateText,
      GoToPreviousAlternateText,
      SelectAllRowsAlternateText,
      SelectRowAlternateText,
      IndexColumnTitle,
      /// <summary> The menu title text used for an automatically generated row menu column. </summary>
      RowMenuTitle
    }

    /// <summary> The possible directions for paging through the list. </summary>
    private enum GoToOption
    {
      /// <summary> Don't page. </summary>
      Undefined,
      /// <summary> Move to first page. </summary>
      First,
      /// <summary> Move to last page. </summary>
      Last,
      /// <summary> Move to previous page. </summary>
      Previous,
      /// <summary> Move to next page. </summary>
      Next
    }

    public enum RowEditModeCommand
    {
      Edit,
      Save,
      Cancel
    }

    // static members
    private static readonly Type[] s_supportedPropertyInterfaces = new[] { typeof (IBusinessObjectReferenceProperty) };

    private static readonly ILog s_log = LogManager.GetLogger (MethodBase.GetCurrentMethod().DeclaringType);

    private object s_menuItemClickEvent = new object();
    private static readonly object s_listItemCommandClickEvent = new object();
    private static readonly object s_customCellClickEvent = new object();

    private static readonly object s_sortingOrderChangingEvent = new object();
    private static readonly object s_sortingOrderChangedEvent = new object();

    private static readonly object s_dataRowRenderEvent = new object();

    private static readonly object s_editableRowChangesSavingEvent = new object();
    private static readonly object s_editableRowChangesSavedEvent = new object();
    private static readonly object s_editableRowChangesCancelingEvent = new object();
    private static readonly object s_editableRowChangesCanceledEvent = new object();


    // member fields

    /// <summary> The <see cref="DropDownList"/> used to select the column configuration. </summary>
    private readonly DropDownList _availableViewsList;

    /// <summary> The <see cref="string"/> that is rendered in front of the <see cref="_availableViewsList"/>. </summary>
    private string _availableViewsListTitle;

    /// <summary> The predefined column definition sets that the user can choose from at run-time. </summary>
    private readonly BocListViewCollection _availableViews;

    /// <summary> Determines whether to show the drop down list for selecting a view. </summary>
    private bool _showAvailableViewsList = true;

    /// <summary> The current <see cref="BocListView"/>. May be set at run time. </summary>
    private BocListView _selectedView;

    /// <summary> 
    ///   The zero-based index of the <see cref="BocListView"/> selected from 
    ///   <see cref="AvailableViews"/>.
    /// </summary>
    private int? _selectedViewIndex;

    private string _availableViewsListSelectedValue = string.Empty;
    private bool _isSelectedViewIndexSet;

    /// <summary> The <see cref="IList"/> displayed by the <see cref="BocList"/>. </summary>
    private IList _value;

    /// <summary> The user independent column definitions. </summary>
    private readonly BocColumnDefinitionCollection _fixedColumns;

    /// <summary> 
    ///   Contains a <see cref="BocColumnDefinition"/> for each property of the bound 
    ///   <see cref="IBusinessObject"/>. 
    /// </summary>
    private BocColumnDefinition[] _allPropertyColumns;

    /// <summary> Contains the <see cref="BocColumnDefinition"/> objects during the handling of the post back events. </summary>
    private BocColumnDefinition[] _columnDefinitionsPostBackEventHandlingPhase;

    /// <summary> Contains the <see cref="BocColumnDefinition"/> objects during the rendering phase. </summary>
    private BocColumnDefinition[] _columnDefinitionsRenderPhase;

    private bool _hasAppendedAllPropertyColumnDefinitions;


    /// <summary> Determines whether the options menu is shown. </summary>
    private bool _showOptionsMenu = true;

    /// <summary> Determines whether the list menu is shown. </summary>
    private bool _showListMenu = true;

    private RowMenuDisplay _rowMenuDisplay = RowMenuDisplay.Undefined;
    private string _optionsTitle;
    private string[] _hiddenMenuItems;
    private Unit _menuBlockWidth = Unit.Empty;
    private Unit _menuBlockOffset = Unit.Empty;
    private Unit _menuBlockItemOffset = Unit.Empty;
    private readonly DropDownMenu _optionsMenu;

    private readonly ListMenu _listMenu;

    /// <summary> Triplet &lt; IBusinessObject, listIndex, DropDownMenu &gt;</summary>
    private BocListRowMenuTuple[] _rowMenus;

    private readonly PlaceHolder _rowMenusPlaceHolder;

    /// <summary> 
    ///   HashTable &lt; 
    ///       Key = CustomColumn, 
    ///       Value = Triplet[] &lt; IBusinessObject, listIndex, Control &gt; &gt;
    /// </summary>
    private Dictionary<BocColumnDefinition, BocListCustomColumnTuple[]> _customColumns;

    private readonly PlaceHolder _customColumnsPlaceHolder;

    /// <summary> Determines wheter an empty list will still render its headers and the additional column sets list. </summary>
    private bool _showEmptyListEditMode = true;

    private bool _showMenuForEmptyListEditMode = true;
    private bool _showEmptyListReadOnlyMode;
    private bool _showMenuForEmptyListReadOnlyMode;
    private string _emptyListMessage;
    private bool _showEmptyListMessage;

    /// <summary> Determines whether to generate columns for all properties. </summary>
    private bool _showAllProperties;

    /// <summary> Determines whether to show the icons for each entry in <see cref="Value"/>. </summary>
    private bool _enableIcon = true;

    /// <summary> Determines whether to show the sort buttons. </summary>
    private bool _enableSorting = true;

    /// <summary> Determines whether to show the sorting order after the sorting button. Undefined interpreted as True. </summary>
    private bool? _showSortingOrder;

    /// <summary> Undefined interpreted as True. </summary>
    private bool? _enableMultipleSorting;

    private List<BocListSortingOrderEntry> _sortingOrder = new List<BocListSortingOrderEntry>();

    private BocListRow[] _indexedRowsSorted;

    /// <summary> Determines whether to enable the selecting of the data rows. </summary>
    private RowSelection _selection = RowSelection.Undefined;

    /// <summary> 
    ///   Contains the checked state for each of the selector controls in the <see cref="BocList"/>.
    ///   Hashtable&lt;int rowIndex, bool isChecked&gt; 
    /// </summary>
    private IList<int> _selectorControlCheckedState = new List<int>();

    private RowIndex _index = RowIndex.Undefined;
    private string _indexColumnTitle;
    private int? _indexOffset;

    /// <summary> Null, 0: show all objects, > 0: show n objects per page. </summary>
    private int? _pageSize;

    /// <summary>
    ///   Show page info ("page 1 of n") and links always (true),
    ///   or only if there is more than 1 page (false)
    /// </summary>
    private bool _alwaysShowPageInfo;

    /// <summary> The text providing the current page information to the user. </summary>
    private string _pageInfo;

    /// <summary> 
    ///   The navigation bar command that caused the post back. 
    ///   <see cref="GoToOption.Undefined"/> unless the navigation bar caused a post back.
    /// </summary>
    private GoToOption _goTo = GoToOption.Undefined;

    /// <summary> 
    ///   The index of the current row in the <see cref="IBusinessObject"/> this control is bound to.
    /// </summary>
    private int _currentRow;

    /// <summary> The index of the current page. </summary>
    private int _currentPage;

    /// <summary> The total number of pages required for paging through the entire list. </summary>
    private int _pageCount;

    /// <summary> Determines whether the client script is enabled. </summary>
    private bool _enableClientScript = true;

    private readonly IEditModeController _editModeController;
    private EditableRowDataSourceFactory _editModeDataSourceFactory = new EditableRowDataSourceFactory();
    private EditableRowControlFactory _editModeControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

    private string _errorMessage;
    private readonly ArrayList _validators;
    private bool? _isBrowserCapableOfSCripting;

    // construction and disposing

    public BocList ()
    {
      _availableViewsList = new DropDownList();
      _editModeController = new EditModeController (this);
      _optionsMenu = new DropDownMenu (this);
      _listMenu = new ListMenu (this);
      _rowMenusPlaceHolder = new PlaceHolder();
      _customColumnsPlaceHolder = new PlaceHolder();
      _fixedColumns = new BocColumnDefinitionCollection (this);
      _availableViews = new BocListViewCollection (this);
      _validators = new ArrayList();
    }

    // methods and properties

    protected override void CreateChildControls ()
    {
      _optionsMenu.ID = ID + c_optionsMenuIDSuffix;
      _optionsMenu.EventCommandClick += MenuItemEventCommandClick;
      _optionsMenu.WxeFunctionCommandClick += MenuItemWxeFunctionCommandClick;
      Controls.Add (_optionsMenu);

      _listMenu.ID = ID + c_listMenuIDSuffix;
      _listMenu.EventCommandClick += MenuItemEventCommandClick;
      _listMenu.WxeFunctionCommandClick += MenuItemWxeFunctionCommandClick;
      Controls.Add (_listMenu);

      _availableViewsList.ID = ID + c_availableViewsListIDSuffix;
      _availableViewsList.EnableViewState = false;
      _availableViewsList.AutoPostBack = true;
      Controls.Add (_availableViewsList);

      _editModeController.ID = ID + "_EditModeController";
      Controls.Add ((Control) _editModeController);

      Controls.Add (_rowMenusPlaceHolder);
      Controls.Add (_customColumnsPlaceHolder);
    }

    /// <summary> Calls the parent's <c>OnInit</c> method and initializes this control's sub-controls. </summary>
    /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      _availableViews.CollectionChanged += AvailableViews_CollectionChanged;
      Binding.BindingChanged += Binding_BindingChanged;

      if (!IsDesignMode)
      {
        Page.RegisterRequiresPostBack (this);
        InitializeMenusItems();
      }
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      base.RegisterHtmlHeadContents (htmlHeadAppender);

      var renderer = CreateRenderer();
      renderer.RegisterHtmlHeadContents (htmlHeadAppender, EditModeControlFactory);
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (! Page.IsPostBack)
        EnsureColumnsGot();
      else
        EnsureColumnsForPreviousLifeCycleGot();

      EnsureEditModeRestored();
      EnsureRowMenusInitialized();
      RestoreCustomColumns();
    }

    /// <summary> Implements interface <see cref="IPostBackEventHandler"/>. </summary>
    /// <param name="eventArgument"> &lt;prefix&gt;=&lt;value&gt; </param>
    void IPostBackEventHandler.RaisePostBackEvent (string eventArgument)
    {
      RaisePostBackEvent (eventArgument);
    }

    /// <param name="eventArgument"> &lt;prefix&gt;=&lt;value&gt; </param>
    protected virtual void RaisePostBackEvent (string eventArgument)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("eventArgument", eventArgument);

      eventArgument = eventArgument.Trim();
      if (eventArgument.StartsWith (c_eventListItemCommandPrefix))
        HandleListItemCommandEvent (eventArgument.Substring (c_eventListItemCommandPrefix.Length));
      else if (eventArgument.StartsWith (SortCommandPrefix))
        HandleResorting (eventArgument.Substring (SortCommandPrefix.Length));
      else if (eventArgument.StartsWith (c_customCellEventPrefix))
        HandleCustomCellEvent (eventArgument.Substring (c_customCellEventPrefix.Length));
      else if (eventArgument.StartsWith (c_eventRowEditModePrefix))
        HandleRowEditModeEvent (eventArgument.Substring (c_eventRowEditModePrefix.Length));
      else if (eventArgument.StartsWith (GoToCommandPrefix))
        HandleGoToEvent (eventArgument.Substring (GoToCommandPrefix.Length));
      else
        throw new ArgumentException ("Argument 'eventArgument' has unknown prefix: '" + eventArgument + "'.");
    }

    /// <summary> Invokes the <see cref="LoadPostData"/> method. </summary>
    bool IPostBackDataHandler.LoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      if (RequiresLoadPostData)
        return LoadPostData (postDataKey, postCollection);
      else
        return false;
    }

    /// <summary> Invokes the <see cref="RaisePostDataChangedEvent"/> method. </summary>
    void IPostBackDataHandler.RaisePostDataChangedEvent ()
    {
      RaisePostDataChangedEvent();
    }

    /// <summary>
    ///   Returns always <see langword="true"/>. 
    ///   Used to raise the post data changed event for getting the selected column definition set.
    /// </summary>
    protected virtual bool LoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      if (_editModeController.IsRowEditModeActive)
        return false;

      string dataRowSelectorControlFilter = ClientID + c_dataRowSelectorControlIDSuffix;
      string titleRowSelectorControlFilter = ClientID + c_titleRowSelectorControlIDSuffix;

      _selectorControlCheckedState.Clear();
      for (int i = 0; i < postCollection.Count; i++)
      {
        string key = postCollection.Keys[i];
        if (string.IsNullOrEmpty (key))
          continue;

        bool isDataRowSelectorControl = key.StartsWith (dataRowSelectorControlFilter);
        bool isTitleRowSelectorControl = (key == titleRowSelectorControlFilter);
        if (isDataRowSelectorControl || isTitleRowSelectorControl)
        {
          if ((_selection == RowSelection.SingleCheckBox || _selection == RowSelection.SingleRadioButton)
              && (_selectorControlCheckedState.Count > 1 || isTitleRowSelectorControl))
            continue;
          // The title row can occur multiple times, resulting in the title row value to be concatenated and thus not parsable.
          int rowIndex = isTitleRowSelectorControl ? c_titleRowIndex : int.Parse (postCollection[i]);
          _selectorControlCheckedState.Add (rowIndex);
        }
      }

      string newAvailableViewsListSelectedValue = postCollection[_availableViewsList.UniqueID];
      if (! StringUtility.IsNullOrEmpty (newAvailableViewsListSelectedValue)
          && _availableViewsListSelectedValue != newAvailableViewsListSelectedValue)
        return true;
      else
        return false;
    }

    /// <summary> Called when the state of the control has changed between postbacks. </summary>
    protected virtual void RaisePostDataChangedEvent ()
    {
      if (_availableViews.Count > 0)
      {
        string newAvailableViewsListSelectedValue =
            PageUtility.GetPostBackCollectionItem (Page, _availableViewsList.UniqueID);
        SelectedViewIndex = int.Parse (newAvailableViewsListSelectedValue);
      }
    }

    /// <summary> Handles post back events raised by a list item event. </summary>
    /// <param name="eventArgument"> &lt;column-index&gt;,&lt;list-index&gt; </param>
    private void HandleListItemCommandEvent (string eventArgument)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("eventArgument", eventArgument);

      string[] eventArgumentParts = eventArgument.Split (new[] { ',' }, 2);

      //  First part: column index
      int columnIndex;
      eventArgumentParts[0] = eventArgumentParts[0].Trim();
      try
      {
        if (eventArgumentParts[0].Length == 0)
          throw new FormatException();
        columnIndex = int.Parse (eventArgumentParts[0]);
      }
      catch (FormatException)
      {
        throw new ArgumentException (
            "First part of argument 'eventArgument' must be an integer. Expected format: '<column-index>,<list-index>'.");
      }

      //  Second part: list index
      int listIndex;
      eventArgumentParts[1] = eventArgumentParts[1].Trim();
      try
      {
        if (eventArgumentParts[1].Length == 0)
          throw new FormatException();
        listIndex = int.Parse (eventArgumentParts[1]);
      }
      catch (FormatException)
      {
        throw new ArgumentException (
            "Second part of argument 'eventArgument' must be an integer. Expected format: <column-index>,<list-index>'.");
      }

      BocColumnDefinition[] columns = EnsureColumnsForPreviousLifeCycleGot();

      if (columnIndex >= columns.Length)
      {
        throw new ArgumentOutOfRangeException (
            "Column index of argument 'eventargument' was out of the range of valid values."
            + "Index must be less than the number of displayed columns.'",
            (Exception) null);
      }

      BocCommandEnabledColumnDefinition column = (BocCommandEnabledColumnDefinition) columns[columnIndex];
      if (column.Command == null)
      {
        throw new ArgumentOutOfRangeException (
            string.Format (
                "The BocList '{0}' does not have a command inside column {1}.", ID, columnIndex));
      }
      BocListItemCommand command = column.Command;

      if (Value == null)
      {
        throw new InvalidOperationException (
            string.Format (
                "The BocList '{0}' does not have a Value when attempting to handle the list item click event.", ID));
      }

      switch (command.Type)
      {
        case CommandType.Event:
        {
          IBusinessObject businessObject = null;
          if (listIndex < Value.Count)
            businessObject = (IBusinessObject) Value[listIndex];
          OnListItemCommandClick (column, listIndex, businessObject);
          break;
        }
        case CommandType.WxeFunction:
        {
          IBusinessObject businessObject = null;
          if (listIndex < Value.Count)
            businessObject = (IBusinessObject) Value[listIndex];
          if (Page is IWxePage)
            command.ExecuteWxeFunction ((IWxePage) Page, listIndex, businessObject);
          //else
          //  command.ExecuteWxeFunction (Page, listIndex, businessObject);
          break;
        }
        default:
        {
          break;
        }
      }
    }

    /// <summary> Handles post back events raised by a custom cell event. </summary>
    /// <param name="eventArgument"> &lt;column-index&gt;,&lt;list-index&gt;[,&lt;customArgument&gt;] </param>
    private void HandleCustomCellEvent (string eventArgument)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("eventArgument", eventArgument);

      string[] eventArgumentParts = eventArgument.Split (new[] { ',' }, 3);

      //  First part: column index
      int columnIndex;
      eventArgumentParts[0] = eventArgumentParts[0].Trim();
      try
      {
        if (eventArgumentParts[0].Length == 0)
          throw new FormatException();
        columnIndex = int.Parse (eventArgumentParts[0]);
      }
      catch (FormatException)
      {
        throw new ArgumentException (
            "First part of argument 'eventArgument' must be an integer. Expected format: '<column-index>,<list-index>[,<customArgument>]'.");
      }

      //  Second part: list index
      int listIndex;
      eventArgumentParts[1] = eventArgumentParts[1].Trim();
      try
      {
        if (eventArgumentParts[1].Length == 0)
          throw new FormatException();
        listIndex = int.Parse (eventArgumentParts[1]);
      }
      catch (FormatException)
      {
        throw new ArgumentException (
            "Second part of argument 'eventArgument' must be an integer. Expected format: <column-index>,<list-index>[,<customArgument>]'.");
      }

      //  Thrid part, optional: customCellArgument
      string customCellArgument = null;
      if (eventArgumentParts.Length == 3)
      {
        eventArgumentParts[2] = eventArgumentParts[2].Trim();
        customCellArgument = eventArgumentParts[2];
      }
      BocColumnDefinition[] columns = EnsureColumnsForPreviousLifeCycleGot();

      if (columnIndex >= columns.Length)
      {
        throw new ArgumentOutOfRangeException (
            "Column index of argument 'eventargument' was out of the range of valid values. Index must be less than the number of displayed columns.'",
            (Exception) null);
      }
      
      if (Value == null)
      {
        throw new InvalidOperationException (
            string.Format (
                "The BocList '{0}' does not have a Value when attempting to handle the custom cell event.", ID));
      }

      BocCustomColumnDefinition column = (BocCustomColumnDefinition) columns[columnIndex];
      OnCustomCellClick (column, (IBusinessObject) Value[listIndex], customCellArgument);
    }

    /// <summary> Handles post back events raised by an row edit mode event. </summary>
    /// <param name="eventArgument"> &lt;list-index&gt;,&lt;command&gt; </param>
    private void HandleRowEditModeEvent (string eventArgument)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("eventArgument", eventArgument);

      string[] eventArgumentParts = eventArgument.Split (new char[] { ',' }, 2);

      //  First part: list index
      int listIndex;
      eventArgumentParts[0] = eventArgumentParts[0].Trim();
      try
      {
        if (eventArgumentParts[0].Length == 0)
          throw new FormatException();
        listIndex = int.Parse (eventArgumentParts[0]);
      }
      catch (FormatException)
      {
        throw new ArgumentException (
            "First part of argument 'eventArgument' must be an integer. Expected format: '<list-index>,<command>'.");
      }

      //  Second part: command
      RowEditModeCommand command;
      eventArgumentParts[1] = eventArgumentParts[1].Trim();
      try
      {
        if (eventArgumentParts[1].Length == 0)
          throw new FormatException();
        command = (RowEditModeCommand) Enum.Parse (typeof (RowEditModeCommand), eventArgumentParts[1]);
      }
      catch (FormatException)
      {
        throw new ArgumentException (
            "Second part of argument 'eventArgument' must be an integer. Expected format: <list-index>,<command>'.");
      }

      if (Value == null)
      {
        throw new InvalidOperationException (
            string.Format (
                "The BocList '{0}' does not have a Value when attempting to handle the list item click event.", ID));
      }

      switch (command)
      {
        case RowEditModeCommand.Edit:
        {
          if (listIndex >= Value.Count)
          {
            throw new ArgumentOutOfRangeException (
                "list-index of argument 'eventargument' was out of the range of valid values. Index must be less than the number of business objects in the list.'",
                (Exception) null);
          }
          SwitchRowIntoEditMode (listIndex);
          break;
        }
        case RowEditModeCommand.Save:
        {
          EndRowEditMode (true);
          break;
        }
        case RowEditModeCommand.Cancel:
        {
          EndRowEditMode (false);
          break;
        }
        default:
        {
          break;
        }
      }
    }

    /// <summary> Handles post back events raised by a go-to button. </summary>
    /// <param name="eventArgument"> &lt;GoToOption&gt; </param>
    private void HandleGoToEvent (string eventArgument)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("eventArgument", eventArgument);

      try
      {
        _goTo = (GoToOption) Enum.Parse (typeof (GoToOption), eventArgument);
      }
      catch (ArgumentException)
      {
        throw new ArgumentException ("Argument 'eventArgument' must be a value of the GoToOption enum.");
      }
    }

    /// <summary> Fires the <see cref="ListItemCommandClick"/> event. </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocList.xml' path='BocList/OnListItemCommandClick/*' />
    protected virtual void OnListItemCommandClick (
        BocCommandEnabledColumnDefinition column,
        int listIndex,
        IBusinessObject businessObject)
    {
      if (column != null && column.Command != null)
      {
        column.Command.OnClick (column, listIndex, businessObject);
        BocListItemCommandClickEventHandler commandClickHandler =
            (BocListItemCommandClickEventHandler) Events[s_listItemCommandClickEvent];
        if (commandClickHandler != null)
        {
          BocListItemCommandClickEventArgs e =
              new BocListItemCommandClickEventArgs (column.Command, column, listIndex, businessObject);
          commandClickHandler (this, e);
        }
      }
    }

    protected virtual void OnCustomCellClick (
        BocCustomColumnDefinition column,
        IBusinessObject businessObject,
        string argument)
    {
      BocCustomCellClickArguments args = new BocCustomCellClickArguments (this, businessObject, column);
      column.CustomCell.Click (args, argument);
      BocCustomCellClickEventHandler clickHandler =
          (BocCustomCellClickEventHandler) Events[s_customCellClickEvent];
      if (clickHandler != null)
      {
        BocCustomCellClickEventArgs e = new BocCustomCellClickEventArgs (column, businessObject, argument);
        clickHandler (this, e);
      }
    }

    /// <summary> Handles post back events raised by a sorting button. </summary>
    /// <param name="eventArgument"> &lt;column-index&gt; </param>
    private void HandleResorting (string eventArgument)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("eventArgument", eventArgument);

      int columnIndex;
      try
      {
        if (eventArgument.Length == 0)
          throw new FormatException();
        columnIndex = int.Parse (eventArgument);
      }
      catch (FormatException)
      {
        throw new ArgumentException ("Argument 'eventArgument' must be an integer.");
      }

      BocColumnDefinition[] columns = EnsureColumnsForPreviousLifeCycleGot();

      if (columnIndex >= columns.Length)
      {
        throw new ArgumentOutOfRangeException (
            "eventArgument",
            eventArgument,
            "Column index was out of the range of valid values. Index must be less than the number of displayed columns.'");
      }
      var column = columns[columnIndex];
      if (!(column is IBocSortableColumnDefinition && ((IBocSortableColumnDefinition) column).IsSortable))
        throw new ArgumentOutOfRangeException ("The BocList '" + ID + "' does not sortable column at index" + columnIndex + ".");

      var workingSortingOrder = new List<BocListSortingOrderEntry> (_sortingOrder);

      var oldSortingOrderEntry = workingSortingOrder.FirstOrDefault (entry => entry.Column == column) ?? BocListSortingOrderEntry.Empty;

      BocListSortingOrderEntry newSortingOrderEntry;
      //  Cycle: Ascending -> Descending -> None -> Ascending
      if (! oldSortingOrderEntry.IsEmpty)
      {
        workingSortingOrder.Remove (oldSortingOrderEntry);
        switch (oldSortingOrderEntry.Direction)
        {
          case SortingDirection.Ascending:
          {
            newSortingOrderEntry = new BocListSortingOrderEntry (oldSortingOrderEntry.Column, SortingDirection.Descending);
            break;
          }
          case SortingDirection.Descending:
          {
            newSortingOrderEntry = BocListSortingOrderEntry.Empty;
            break;
          }
          case SortingDirection.None:
          {
            newSortingOrderEntry = new BocListSortingOrderEntry (oldSortingOrderEntry.Column, SortingDirection.Ascending);
            break;
          }
          default:
          {
            throw new InvalidOperationException (string.Format ("SortingDirection '{0}' is not valid.", oldSortingOrderEntry.Direction));
          }
        }
      }
      else
      {
        newSortingOrderEntry = new BocListSortingOrderEntry ((IBocSortableColumnDefinition) column, SortingDirection.Ascending);
      }

      if (newSortingOrderEntry.IsEmpty)
      {
        if (workingSortingOrder.Count > 1 && ! IsMultipleSortingEnabled)
        {
          var entry = workingSortingOrder[0];
          workingSortingOrder.Clear();
          workingSortingOrder.Add (entry);
        }
      }
      else
      {
        if (! IsMultipleSortingEnabled)
          workingSortingOrder.Clear();
        workingSortingOrder.Add (newSortingOrderEntry);
      }

      BocListSortingOrderEntry[] oldSortingOrder = _sortingOrder.ToArray ();
      BocListSortingOrderEntry[] newSortingOrder = workingSortingOrder.ToArray();

      OnSortingOrderChanging (oldSortingOrder, newSortingOrder);
      _sortingOrder.Clear();
      _sortingOrder.AddRange (workingSortingOrder);
      OnSortingOrderChanged (oldSortingOrder, newSortingOrder);
      ResetRows();
    }

    protected virtual void OnSortingOrderChanging (
        BocListSortingOrderEntry[] oldSortingOrder, BocListSortingOrderEntry[] newSortingOrder)
    {
      BocListSortingOrderChangeEventHandler handler =
          (BocListSortingOrderChangeEventHandler) Events[s_sortingOrderChangingEvent];
      if (handler != null)
      {
        BocListSortingOrderChangeEventArgs e =
            new BocListSortingOrderChangeEventArgs (oldSortingOrder, newSortingOrder);
        handler (this, e);
      }
    }

    protected virtual void OnSortingOrderChanged (
        BocListSortingOrderEntry[] oldSortingOrder, BocListSortingOrderEntry[] newSortingOrder)
    {
      BocListSortingOrderChangeEventHandler handler =
          (BocListSortingOrderChangeEventHandler) Events[s_sortingOrderChangedEvent];
      if (handler != null)
      {
        BocListSortingOrderChangeEventArgs e =
            new BocListSortingOrderChangeEventArgs (oldSortingOrder, newSortingOrder);
        handler (this, e);
      }
    }

    /// <summary> Is raised when the sorting order of the <see cref="BocList"/> is about to change. </summary>
    /// <remarks> Will only be raised, if the change was caused by an UI action. </remarks>
    [Category ("Action")]
    [Description ("Occurs when the sorting order of the BocList is about to change.")]
    public event BocListSortingOrderChangeEventHandler SortingOrderChanging
    {
      add { Events.AddHandler (s_sortingOrderChangingEvent, value); }
      remove { Events.RemoveHandler (s_sortingOrderChangingEvent, value); }
    }

    /// <summary> Is raised when the sorting order of the <see cref="BocList"/> has changed. </summary>
    /// <remarks> Will only be raised, if the change was caused by an UI action. </remarks>
    [Category ("Action")]
    [Description ("Occurs when the sorting order of the BocList has to changed.")]
    public event BocListSortingOrderChangeEventHandler SortingOrderChanged
    {
      add { Events.AddHandler (s_sortingOrderChangedEvent, value); }
      remove { Events.RemoveHandler (s_sortingOrderChangedEvent, value); }
    }

    /// <summary>
    ///   Generates a <see cref="EditModeValidator"/>.
    /// </summary>
    /// <returns> Returns a list of <see cref="BaseValidator"/> objects. </returns>
    public override BaseValidator[] CreateValidators ()
    {
      if (IsReadOnly)
        return new BaseValidator[0];

      BaseValidator[] validators = _editModeController.CreateValidators (GetResourceManager());
      _validators.AddRange (validators);

      return validators;
    }

    /// <summary> Checks whether the control conforms to the required WAI level. </summary>
    /// <exception cref="WcagException"> Thrown if the control does not conform to the required WAI level. </exception>
    protected virtual void EvaluateWaiConformity (BocColumnDefinition[] columns)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("columns", columns);

      if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
      {
        if (ShowOptionsMenu)
          WcagHelper.Instance.HandleError (1, this, "ShowOptionsMenu");
        if (ShowListMenu)
          WcagHelper.Instance.HandleError (1, this, "ShowListMenu");
        if (ShowAvailableViewsList)
          WcagHelper.Instance.HandleError (1, this, "ShowAvailableViewsList");
        bool isPagingEnabled = _pageSize != null && _pageSize.Value != 0;
        if (isPagingEnabled)
          WcagHelper.Instance.HandleError (1, this, "PageSize");
        if (EnableSorting)
          WcagHelper.Instance.HandleWarning (1, this, "EnableSorting");
        if (RowMenuDisplay == RowMenuDisplay.Automatic)
          WcagHelper.Instance.HandleError (1, this, "RowMenuDisplay");

        for (int i = 0; i < columns.Length; i++)
        {
          if (columns[i] is BocRowEditModeColumnDefinition)
            WcagHelper.Instance.HandleError (1, this, string.Format ("Columns[{0}]", i));

          BocCommandEnabledColumnDefinition commandColumn = columns[i] as BocCommandEnabledColumnDefinition;
          if (commandColumn != null)
          {
            bool hasPostBackColumnCommand = commandColumn.Command != null
                                            && (commandColumn.Command.Type == CommandType.Event
                                                || commandColumn.Command.Type == CommandType.WxeFunction);
            if (hasPostBackColumnCommand)
              WcagHelper.Instance.HandleError (1, this, string.Format ("Columns[{0}].Command", i));
          }

          if (columns[i] is BocDropDownMenuColumnDefinition)
            WcagHelper.Instance.HandleError (1, this, string.Format ("Columns[{0}]", i));
        }
      }
      if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelDoubleARequired())
      {
        if (IsSelectionEnabled && ! IsIndexEnabled)
          WcagHelper.Instance.HandleError (2, this, "Selection");
      }
    }

    private void ResetRows ()
    {
      _indexedRowsSorted = null;
      ResetRowMenus();
    }

    public override void PrepareValidation ()
    {
      base.PrepareValidation();

      _editModeController.PrepareValidation();
    }

    protected override void OnPreRender (EventArgs e)
    {
      _optionsMenu.Visible = HasOptionsMenu;
      _optionsMenu.Enabled = !_editModeController.IsRowEditModeActive;
      _listMenu.Visible = HasListMenu;
      _listMenu.Enabled = !_editModeController.IsRowEditModeActive;

      EnsureChildControls();
      base.OnPreRender (e);

      // Must be executed before CalculateCurrentPage
      if (_editModeController.IsRowEditModeActive)
      {
        BocListRow[] sortedRows = EnsureSortedBocListRowsGot();
        for (int idxRows = 0; idxRows < sortedRows.Length; idxRows++)
        {
          int originalRowIndex = sortedRows[idxRows].Index;
          if (_editModeController.EditableRowIndex.Value == originalRowIndex)
          {
            _currentRow = idxRows;
            break;
          }
        }
      }
      CalculateCurrentPage (true);

      BocColumnDefinition[] columns = EnsureColumnsGot (true);

      EnsureEditModeValidatorsRestored();

      LoadResources (GetResourceManager());

      if (!IsDesignMode)
      {
        PreRenderMenuItems();

        EnsureRowMenusInitialized();
        PreRenderRowMenusItems();

        CreateCustomColumnControls (columns);
        InitCustomColumns();
        LoadCustomColumns();
        PreRenderCustomColumns();
        PreRenderListItemCommands();

        _optionsMenu.GetSelectionCount = GetSelectionCountScript();
      }

      PopulateAvailableViewsList();
    }

    /// <summary> Gets a <see cref="HtmlTextWriterTag.Div"/> as the <see cref="WebControl.TagKey"/>. </summary>
    protected override HtmlTextWriterTag TagKey
    {
      get { return HtmlTextWriterTag.Div; }
    }

    protected void CalculateCurrentPage (bool evaluateGoTo)
    {
      if (!IsPagingEnabled || Value == null)
        _pageCount = 1;
      else
      {
        _currentPage = _currentRow / _pageSize.Value;
        _pageCount = (int) Math.Ceiling ((double) Value.Count / _pageSize.Value);

        if (evaluateGoTo)
        {
          switch (_goTo)
          {
            case GoToOption.First:
            {
              _currentPage = 0;
              _currentRow = 0;
              break;
            }
            case GoToOption.Last:
            {
              _currentPage = _pageCount - 1;
              _currentRow = _currentPage * _pageSize.Value;
              break;
            }
            case GoToOption.Previous:
            {
              _currentPage--;
              _currentRow = _currentPage * _pageSize.Value;
              break;
            }
            case GoToOption.Next:
            {
              _currentPage++;
              _currentRow = _currentPage * _pageSize.Value;
              break;
            }
            default:
            {
              break;
            }
          }
        }

        if (_currentPage >= _pageCount || _currentRow >= Value.Count)
        {
          _currentPage = _pageCount - 1;
          _currentRow = Value.Count - 1;
        }
        if (_currentPage < 0 || _currentRow < 0)
        {
          _currentPage = 0;
          _currentRow = 0;
        }

        if (_goTo != GoToOption.Undefined && evaluateGoTo)
        {
          _selectorControlCheckedState.Clear();
          ResetRowMenus();
        }
      }
    }

    protected override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      if (Page != null)
        Page.VerifyRenderingInServerForm (this);

      BocColumnDefinition[] renderColumns = EnsureColumnsGot (IsDesignMode);
      EvaluateWaiConformity (renderColumns);

      if (IsDesignMode)
      {
        //  Normally set in OnPreRender, which is omitted during design-time
        if (_pageCount == 0)
          _pageCount = 1;
      }

      var renderer = CreateRenderer();
      renderer.Render (CreateRenderingContext (writer, GetColumnRenderers (renderColumns)));
    }

    protected virtual IBocListRenderer CreateRenderer ()
    {
      return ServiceLocator.GetInstance<IBocListRenderer> ();
    }

    protected virtual BocListRenderingContext CreateRenderingContext (HtmlTextWriter writer, BocColumnRenderer[] columnRenderers)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      return new BocListRenderingContext (Context, writer, this, columnRenderers);
    }

    public bool HasNavigator
    {
      get
      {
        bool hasNavigator = _alwaysShowPageInfo || _pageCount > 1;
        bool isReadOnly = IsReadOnly;
        bool showForEmptyList = isReadOnly && _showEmptyListReadOnlyMode
                                || !isReadOnly && _showEmptyListEditMode;
        if (!IsDesignMode && !HasValue && !showForEmptyList)
          hasNavigator = false;
        return hasNavigator;
      }
    }

    protected bool HasMenuBlock
    {
      get { return HasAvailableViewsList || HasOptionsMenu || HasListMenu; }
    }

    bool IBocList.HasMenuBlock
    {
      get { return HasMenuBlock; }
    }

    protected bool HasAvailableViewsList
    {
      get
      {
        if (WcagHelper.Instance.IsWaiConformanceLevelARequired())
          return false;

        if (! IsBrowserCapableOfScripting)
          return false;

        bool showAvailableViewsList = _showAvailableViewsList
                                      && (_availableViews.Count > 1
                                          || IsDesignMode);
        bool isReadOnly = IsReadOnly;
        bool showForEmptyList = isReadOnly && _showEmptyListReadOnlyMode
                                || ! isReadOnly && _showEmptyListEditMode;
        return showAvailableViewsList
               && (HasValue || showForEmptyList);
      }
    }

    protected bool IsBrowserCapableOfScripting
    {
      get
      {
        if (!_isBrowserCapableOfSCripting.HasValue)
        {
          var preRenderer = ServiceLocator.GetInstance<IClientScriptBehavior> ();
          _isBrowserCapableOfSCripting = preRenderer.IsBrowserCapableOfScripting(Context, this);
        }
        return _isBrowserCapableOfSCripting.Value;
      }
    }

    bool IBocList.HasAvailableViewsList
    {
      get { return HasAvailableViewsList; }
    }

    protected bool HasOptionsMenu
    {
      get
      {
        if (WcagHelper.Instance.IsWaiConformanceLevelARequired())
          return false;

        if (! IsBrowserCapableOfScripting)
          return false;

        bool showOptionsMenu = ShowOptionsMenu
                               && (OptionsMenuItems.Count > 0
                                   || IsDesignMode);
        bool isReadOnly = IsReadOnly;
        bool showForEmptyList = isReadOnly && ShowMenuForEmptyListReadOnlyMode
                                || ! isReadOnly && ShowMenuForEmptyListEditMode;
        return showOptionsMenu
               && (HasValue || showForEmptyList);
      }
    }

    bool IBocList.HasOptionsMenu
    {
      get { return HasOptionsMenu; }
    }

    protected bool HasListMenu
    {
      get
      {
        if (WcagHelper.Instance.IsWaiConformanceLevelARequired())
          return false;

        if (! IsBrowserCapableOfScripting)
          return false;

        bool showListMenu = ShowListMenu
                            && (ListMenuItems.Count > 0
                                || IsDesignMode);
        bool isReadOnly = IsReadOnly;
        bool showForEmptyList = isReadOnly && ShowMenuForEmptyListReadOnlyMode
                                || ! isReadOnly && ShowMenuForEmptyListEditMode;
        return showListMenu
               && (HasValue || showForEmptyList);
      }
    }

    bool IBocList.HasListMenu
    {
      get { return HasListMenu; }
    }

    private void PopulateAvailableViewsList ()
    {
      _availableViewsList.Items.Clear();

      if (_availableViews != null)
      {
        for (int i = 0; i < _availableViews.Count; i++)
        {
          BocListView columnDefinitionCollection = _availableViews[i];

          ListItem item = new ListItem (columnDefinitionCollection.Title, i.ToString());
          _availableViewsList.Items.Add (item);
          if (_selectedViewIndex != null
              && _selectedViewIndex == i)
            item.Selected = true;
        }
      }
    }

    /// <summary> Builds the input required marker. </summary>
    protected internal virtual Image GetRequiredMarker ()
    {
      Image requiredIcon = new Image();
      var themedResourceUrlResolver = ServiceLocator.GetInstance<IThemedResourceUrlResolverFactory> ().CreateResourceUrlResolver ();
      requiredIcon.ImageUrl = themedResourceUrlResolver.GetResourceUrl (this, ResourceType.Image, c_rowEditModeRequiredFieldIcon);

      IResourceManager resourceManager = GetResourceManager();
      requiredIcon.AlternateText = resourceManager.GetString (ResourceIdentifier.RequiredFieldAlternateText);
      requiredIcon.ToolTip = resourceManager.GetString (ResourceIdentifier.RequiredFieldTitle);

      requiredIcon.Style["vertical-align"] = "middle";
      return requiredIcon;
    }

    /// <summary> Builds the validation error marker. </summary>
    public virtual Image GetValidationErrorMarker ()
    {
      Image validationErrorIcon = new Image();
      var themedResourceUrlResolver = ServiceLocator.GetInstance<IThemedResourceUrlResolverFactory> ().CreateResourceUrlResolver ();
      validationErrorIcon.ImageUrl = themedResourceUrlResolver.GetResourceUrl (this, ResourceType.Image, c_rowEditModeValidationErrorIcon);

      IResourceManager resourceManager = GetResourceManager();
      validationErrorIcon.AlternateText = resourceManager.GetString (ResourceIdentifier.ValidationErrorInfoAlternateText);

      validationErrorIcon.Style["vertical-align"] = "middle";
      return validationErrorIcon;
    }

    protected virtual void OnDataRowRendering (BocListDataRowRenderEventArgs e)
    {
      BocListDataRowRenderEventHandler handler = (BocListDataRowRenderEventHandler) Events[s_dataRowRenderEvent];
      if (handler != null)
        handler (this, e);
    }

    void IBocList.OnDataRowRendering (BocListDataRowRenderEventArgs e)
    {
      OnDataRowRendering (e);
    }

    private string GetListItemCommandArgument (int columnIndex, int originalRowIndex)
    {
      return c_eventListItemCommandPrefix + columnIndex + "," + originalRowIndex;
    }

    string IBocList.GetListItemCommandArgument (int columnIndex, int originalRowIndex)
    {
      return GetListItemCommandArgument (columnIndex, originalRowIndex);
    }

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
    public string GetCustomCellPostBackClientEvent (int columnIndex, int listIndex, string customCellArgument)
    {
      if (_editModeController.IsRowEditModeActive)
        return "return false;";
      string postBackArgument = FormatCustomCellPostBackArgument (columnIndex, listIndex, customCellArgument);
      return Page.ClientScript.GetPostBackEventReference (this, postBackArgument) + ";";
    }

    /// <summary> Formats the arguments into a post back argument to be used by the client side post back event. </summary>
    private string FormatCustomCellPostBackArgument (int columnIndex, int listIndex, string customCellArgument)
    {
      if (customCellArgument == null)
        return c_customCellEventPrefix + columnIndex + "," + listIndex;
      else
        return c_customCellEventPrefix + columnIndex + "," + listIndex + "," + customCellArgument;
    }


    protected override void LoadControlState (object savedState)
    {
      object[] values = (object[]) savedState;

      base.LoadControlState (values[0]);
      _selectedViewIndex = (int?) values[1];
      _availableViewsListSelectedValue = (string) values[2];
      _currentRow = (int) values[3];
      _sortingOrder = (List<BocListSortingOrderEntry>) values[4];
      _selectorControlCheckedState = (IList<int>) values[5];
    }

    protected override object SaveControlState ()
    {
      var columns = EnsureColumnsGot();
      foreach (var sortingOrderEntry in _sortingOrder)
        sortingOrderEntry.SetColumnIndex (Array.IndexOf (columns, sortingOrderEntry.Column));

      object[] values = new object[6];

      values[0] = base.SaveControlState();
      values[1] = _selectedViewIndex;
      values[2] = _availableViewsListSelectedValue;
      values[3] = _currentRow;
      values[4] = _sortingOrder;
      values[5] = _selectorControlCheckedState;

      return values;
    }

    /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocList.xml' path='BocList/LoadValue/*' />
    public override void LoadValue (bool interim)
    {
      if (Property == null)
        return;

      if (DataSource == null)
        return;

      IList value = null;

      if (DataSource.BusinessObject != null)
        value = (IList) DataSource.BusinessObject.GetProperty (Property);

      LoadValueInternal (value, interim);
    }

    /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
    /// <param name="value"> 
    ///   The <see cref="Array"/> of objects implementing <see cref="IBusinessObject"/> to load,
    ///   or <see langword="null"/>. 
    /// </param>
    /// <include file='..\..\doc\include\UI\Controls\BocList.xml' path='BocList/LoadUnboundValue/*' />
    public void LoadUnboundValue (IBusinessObject[] value, bool interim)
    {
      LoadValueInternal (value, interim);
    }

    /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
    /// <param name="value"> 
    ///   The <see cref="IList"/> of objects implementing <see cref="IBusinessObject"/> to load,
    ///   or <see langword="null"/>. 
    /// </param>
    /// <include file='..\..\doc\include\UI\Controls\BocList.xml' path='BocList/LoadUnboundValue/*' />
    public void LoadUnboundValue (IList value, bool interim)
    {
      LoadValueInternal (value, interim);
    }

    /// <summary> Performs the actual loading for <see cref="LoadValue"/> and <see cref="LoadUnboundValue"/>. </summary>
    protected virtual void LoadValueInternal (IList value, bool interim)
    {
      if (! interim)
      {
        if (_editModeController.IsRowEditModeActive)
          EndRowEditMode (false);
        else if (_editModeController.IsListEditModeActive)
          EndListEditMode (false);
      }

      if (interim)
      {
        _value = value;
      }
      else
      {
        SetValue (value);
        IsDirty = false;
      }
    }

    /// <summary> Saves the <see cref="Value"/> into the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocList.xml' path='BocList/LoadValue/*' />
    public override void SaveValue (bool interim)
    {
      if (Property == null)
        return;

      if (DataSource == null)
        return;

      if (!interim)
      {
        if (_editModeController.IsRowEditModeActive)
          EndRowEditMode (true);
        else if (_editModeController.IsListEditModeActive)
          EndListEditMode (true);
      }

      if (IsDirty && SaveValueToDomainModel())
      {
        if (!interim)
          IsDirty = false;
      }
    }

    /// <summary> Find the <see cref="IResourceManager"/> for this control. </summary>
    protected virtual IResourceManager GetResourceManager ()
    {
      return GetResourceManager (typeof (ResourceIdentifier));
    }

    IResourceManager IBocList.GetResourceManager ()
    {
      return GetResourceManager();
    }

    /// <summary> Handles refreshing the bound control. </summary>
    /// <param name="sender"> The source of the event. </param>
    /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
    private void Binding_BindingChanged (object sender, EventArgs e)
    {
      _allPropertyColumns = null;
    }

    private BocColumnDefinition[] GetAllPropertyColumns ()
    {
      if (_allPropertyColumns != null)
        return _allPropertyColumns;

      IBusinessObjectProperty[] properties;
      if (DataSource == null)
        properties = new IBusinessObjectProperty[0];
      else if (Property == null)
        properties = DataSource.BusinessObjectClass.GetPropertyDefinitions();
      else
        properties = Property.ReferenceClass.GetPropertyDefinitions();

      _allPropertyColumns = new BocColumnDefinition[properties.Length];
      for (int i = 0; i < properties.Length; i++)
      {
        IBusinessObjectProperty property = properties[i];
        BocSimpleColumnDefinition column = new BocSimpleColumnDefinition();
        column.ColumnTitle = property.DisplayName;
        column.SetPropertyPath (property.BusinessObjectProvider.CreatePropertyPath (new[] { property }));
        column.OwnerControl = this;
        _allPropertyColumns[i] = column;
      }
      return _allPropertyColumns;
    }

    protected virtual void InitializeMenusItems ()
    {
    }

    protected virtual void PreRenderMenuItems ()
    {
      if (_hiddenMenuItems == null)
        return;

      BocDropDownMenu.HideMenuItems (ListMenuItems, _hiddenMenuItems);
      BocDropDownMenu.HideMenuItems (OptionsMenuItems, _hiddenMenuItems);
    }

    /// <summary> 
    ///   Forces the recreation of the menus to be displayed in the <see cref="BocDropDownMenuColumnDefinition"/>.
    /// </summary>
    private void ResetRowMenus ()
    {
      _rowMenus = null;
    }

    /// <summary> 
    ///   Creates a <see cref="BocDropDownMenuColumnDefinition"/> if <see cref="RowMenuDisplay"/> is set to
    ///   <see cref="!:RowMenuDisplay.Automatic"/>.
    /// </summary>
    /// <returns> A <see cref="BocDropDownMenuColumnDefinition"/> instance or <see langword="null"/>. </returns>
    private BocDropDownMenuColumnDefinition GetRowMenuColumn ()
    {
      if (_rowMenuDisplay == RowMenuDisplay.Automatic)
      {
        BocDropDownMenuColumnDefinition dropDownMenuColumn = new BocDropDownMenuColumnDefinition();
        dropDownMenuColumn.Width = Unit.Percentage (0);
        dropDownMenuColumn.MenuTitleText = GetResourceManager().GetString (ResourceIdentifier.RowMenuTitle);
        return dropDownMenuColumn;
      }
      return null;
    }

    /// <summary> 
    ///   Tests that the <paramref name="columnDefinitions"/> array holds exactly one
    ///   <see cref="BocDropDownMenuColumnDefinition"/> if the <see cref="RowMenuDisplay"/> is set to 
    ///   <see cref="!:RowMenuDisplay.Automatic"/> or <see cref="!:RowMenuDisplay.Manual"/>.
    /// </summary>
    private void CheckRowMenuColumns (BocColumnDefinition[] columnDefinitions)
    {
      bool isFound = false;
      for (int i = 0; i < columnDefinitions.Length; i++)
      {
        if (columnDefinitions[i] is BocDropDownMenuColumnDefinition)
        {
          if (isFound)
            throw new InvalidOperationException ("Only a single BocDropDownMenuColumnDefinition is allowed in the BocList '" + ID + "'.");
          isFound = true;
        }
      }
      if (RowMenuDisplay == RowMenuDisplay.Manual && ! isFound)
      {
        throw new InvalidOperationException (
            "No BocDropDownMenuColumnDefinition was found in the BocList '" + ID + "' but the RowMenuDisplay was set to manual.");
      }
    }

    private void EnsureRowMenusInitialized ()
    {
      if (_rowMenus != null)
        return;
      if (! AreRowMenusEnabled)
        return;
      if (IsDesignMode)
        return;
      if (!HasValue)
        return;

      EnsureChildControls();
      CalculateCurrentPage (false);

      if (IsPagingEnabled)
        _rowMenus = new BocListRowMenuTuple[PageSize.Value];
      else
        _rowMenus = new BocListRowMenuTuple[Value.Count];
      _rowMenusPlaceHolder.Controls.Clear();


      int firstRow = 0;
      int totalRowCount = Value.Count;
      int rowCountWithOffset = totalRowCount;

      if (IsPagingEnabled)
      {
        firstRow = _currentPage * _pageSize.Value;
        rowCountWithOffset = firstRow + _pageSize.Value;
        //  Check row count on last page
        rowCountWithOffset = (rowCountWithOffset < Value.Count) ? rowCountWithOffset : Value.Count;
      }

      BocListRow[] rows = EnsureSortedBocListRowsGot();

      for (int idxAbsoluteRows = firstRow, idxRelativeRows = 0;
           idxAbsoluteRows < rowCountWithOffset;
           idxAbsoluteRows++, idxRelativeRows++)
      {
        BocListRow row = rows[idxAbsoluteRows];
        int originalRowIndex = row.Index;

        DropDownMenu dropDownMenu = new DropDownMenu (this);
        dropDownMenu.ID = ID + "_RowMenu_" + originalRowIndex;
        dropDownMenu.EventCommandClick += RowMenu_EventCommandClick;
        dropDownMenu.WxeFunctionCommandClick += RowMenu_WxeFunctionCommandClick;

        _rowMenusPlaceHolder.Controls.Add (dropDownMenu);
        WebMenuItem[] menuItems = InitializeRowMenuItems (row.BusinessObject, originalRowIndex);
        dropDownMenu.MenuItems.AddRange (menuItems);

        _rowMenus[idxRelativeRows] = new BocListRowMenuTuple (row.BusinessObject, originalRowIndex, dropDownMenu);
      }
    }

    /// <summary> Creates the menu items for a data row. </summary>
    /// <param name="businessObject"> 
    ///   The <see cref="IBusinessObject"/> of the row for which the menu items are being generated. 
    /// </param>
    /// <param name="listIndex"> The position of the <paramref name="businessObject"/> in the list of values. </param>
    /// <returns> A <see cref="WebMenuItem"/> array with the menu items generated by the implementation. </returns>
    protected virtual WebMenuItem[] InitializeRowMenuItems (IBusinessObject businessObject, int listIndex)
    {
      return new WebMenuItem[0];
    }

    /// <summary> PreRenders the menu items for all row menus. </summary>
    private void PreRenderRowMenusItems ()
    {
      if (_rowMenus == null)
        return;

      for (int i = 0; i < _rowMenus.Length; i++)
      {
        BocListRowMenuTuple rowMenuTuple = _rowMenus[i];
        if (rowMenuTuple != null)
        {
          IBusinessObject businessObject = rowMenuTuple.Item1;
          int listIndex = rowMenuTuple.Item2;
          DropDownMenu dropDownMenu = rowMenuTuple.Item3;
          PreRenderRowMenuItems (dropDownMenu.MenuItems, businessObject, listIndex);
          dropDownMenu.Visible = dropDownMenu.MenuItems.Cast<WebMenuItem>().Any (item => item.EvaluateVisible());
        }
      }
    }

    /// <summary> PreRenders the menu items for a data row. </summary>
    /// <param name="menuItems"> The menu items to be displayed for the row. </param>
    /// <param name="businessObject"> 
    ///   The <see cref="IBusinessObject"/> of the row for which the menu items are being generated. 
    /// </param>
    /// <param name="listIndex"> The position of the <paramref name="businessObject"/> in the list of values. </param>
    protected virtual void PreRenderRowMenuItems (
        WebMenuItemCollection menuItems,
        IBusinessObject businessObject,
        int listIndex)
    {
    }

    /// <summary> 
    ///   Event handler for the <see cref="MenuBase.EventCommandClick"/> of the <b>RowMenu</b>.
    /// </summary>
    private void RowMenu_EventCommandClick (object sender, WebMenuItemClickEventArgs e)
    {
      for (int i = 0; i < _rowMenus.Length; i++)
      {
        BocListRowMenuTuple rowMenuTuple = _rowMenus[i];
        if (rowMenuTuple != null)
        {
          DropDownMenu rowMenu = rowMenuTuple.Item3;
          if (rowMenu == sender)
          {
            IBusinessObject businessObject = rowMenuTuple.Item1;
            int listIndex = rowMenuTuple.Item2;
            OnRowMenuItemEventCommandClick (e.Item, businessObject, listIndex);
            return;
          }
        }
      }
    }

    /// <summary> Handles the click on an Event command of a row menu. </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocList.xml' path='BocList/OnRowMenuItemEventCommandClick/*' />
    protected virtual void OnRowMenuItemEventCommandClick (
        WebMenuItem menuItem,
        IBusinessObject businessObject,
        int listIndex)
    {
      if (menuItem != null && menuItem.Command != null)
      {
        if (menuItem is BocMenuItem)
          ((BocMenuItemCommand) menuItem.Command).OnClick ((BocMenuItem) menuItem);
        else
          menuItem.Command.OnClick();
      }
    }

    /// <summary> 
    ///   Event handler for the <see cref="MenuBase.WxeFunctionCommandClick"/> of the <b>RowMenu</b>.
    /// </summary>
    private void RowMenu_WxeFunctionCommandClick (object sender, WebMenuItemClickEventArgs e)
    {
      for (int i = 0; i < _rowMenus.Length; i++)
      {
        BocListRowMenuTuple rowMenuTuple = _rowMenus[i];
        if (rowMenuTuple != null)
        {
          DropDownMenu rowMenu = rowMenuTuple.Item3;
          if (rowMenu == sender)
          {
            IBusinessObject businessObject = rowMenuTuple.Item1;
            int listIndex = rowMenuTuple.Item2;
            OnRowMenuItemWxeFunctionCommandClick (e.Item, businessObject, listIndex);
            return;
          }
        }
      }
    }

    /// <summary> Handles the click to a WXE function command or a row menu. </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocList.xml' path='BocList/OnRowMenuItemWxeFunctionCommandClick/*' />
    protected virtual void OnRowMenuItemWxeFunctionCommandClick (
        WebMenuItem menuItem,
        IBusinessObject businessObject,
        int listIndex)
    {
      if (menuItem != null && menuItem.Command != null)
      {
        if (menuItem is BocMenuItem)
        {
          BocMenuItemCommand command = (BocMenuItemCommand) menuItem.Command;
          if (Page is IWxePage)
            command.ExecuteWxeFunction ((IWxePage) Page, new[] { listIndex }, new[] { businessObject });
          //else
          //  command.ExecuteWxeFunction (Page, new int[1] {listIndex}, new IBusinessObject[1] {businessObject});
        }
        else
        {
          Command command = menuItem.Command;
          if (Page is IWxePage)
            command.ExecuteWxeFunction ((IWxePage) Page, null);
          //else
          //  command.ExecuteWxeFunction (Page, null, new NameValueCollection (0));
        }
      }
    }

    /// <summary> 
    ///   Gets a flag describing whether a <see cref="DropDownMenu"/> is shown in the 
    ///   <see cref="BocDropDownMenuColumnDefinition"/>.
    /// </summary>
    protected virtual bool AreRowMenusEnabled
    {
      get
      {
        if (WcagHelper.Instance.IsWaiConformanceLevelARequired())
          return false;
        if (_rowMenuDisplay == RowMenuDisplay.Undefined
            || _rowMenuDisplay == RowMenuDisplay.Disabled)
          return false;
        return true;
      }
    }

    private void PreRenderListItemCommands ()
    {
      if (IsDesignMode)
        return;
      if (!HasValue)
        return;

      int firstRow = 0;
      int totalRowCount = Value.Count;
      int rowCountWithOffset = totalRowCount;

      if (IsPagingEnabled)
      {
        firstRow = _currentPage * _pageSize.Value;
        rowCountWithOffset = firstRow + _pageSize.Value;
        //  Check row count on last page
        rowCountWithOffset = (rowCountWithOffset < Value.Count) ? rowCountWithOffset : Value.Count;
      }

      BocColumnDefinition[] columns = EnsureColumnsGot (false);
      BocListRow[] rows = EnsureSortedBocListRowsGot();

      for (int idxAbsoluteRows = firstRow, idxRelativeRows = 0;
           idxAbsoluteRows < rowCountWithOffset;
           idxAbsoluteRows++, idxRelativeRows++)
      {
        BocListRow row = rows[idxAbsoluteRows];

        for (int idxColumns = 0; idxColumns < columns.Length; idxColumns++)
        {
          BocCommandEnabledColumnDefinition commandColumn = columns[idxColumns] as BocCommandEnabledColumnDefinition;
          if (commandColumn != null && commandColumn.Command != null)
          {
            commandColumn.Command.RegisterForSynchronousPostBack (
                this,
                GetListItemCommandArgument (idxColumns, row.Index),
                string.Format ("BocList '{0}', Column '{1}'", ID, commandColumn.ItemID));
          }
        }
      }
    }

    /// <summary> Restores the custom columns from the previous life cycle. </summary>
    private void RestoreCustomColumns ()
    {
      if (! Page.IsPostBack)
        return;
      CreateCustomColumnControls (EnsureColumnsForPreviousLifeCycleGot());
      InitCustomColumns();
      LoadCustomColumns();
    }

    /// <summary> Creates the controls for the custom columns in the <paramref name="columns"/> array. </summary>
    private void CreateCustomColumnControls (BocColumnDefinition[] columns)
    {
      _customColumns = null;
      _customColumnsPlaceHolder.Controls.Clear();

      if (IsDesignMode)
        return;
      if (!HasValue)
        return;

      EnsureChildControls();

      CalculateCurrentPage (false);

      _customColumns = new Dictionary<BocColumnDefinition, BocListCustomColumnTuple[]>();

      int firstRow = 0;
      int totalRowCount = Value.Count;
      int rowCountWithOffset = totalRowCount;

      if (IsPagingEnabled)
      {
        firstRow = _currentPage * _pageSize.Value;
        rowCountWithOffset = firstRow + _pageSize.Value;
        //  Check row count on last page
        rowCountWithOffset = (rowCountWithOffset < Value.Count) ? rowCountWithOffset : Value.Count;
      }

      BocListRow[] rows = EnsureSortedBocListRowsGot();

      for (int idxColumns = 0; idxColumns < columns.Length; idxColumns++)
      {
        BocCustomColumnDefinition customColumn = columns[idxColumns] as BocCustomColumnDefinition;
        if (customColumn != null
            && (customColumn.Mode == BocCustomColumnDefinitionMode.ControlsInAllRows
                || customColumn.Mode == BocCustomColumnDefinitionMode.ControlInEditedRow))
        {
          PlaceHolder placeHolder = new PlaceHolder();
          _customColumnsPlaceHolder.Controls.Add (placeHolder);

          BocListCustomColumnTuple[] customColumnTuples;
          if (IsPagingEnabled)
            customColumnTuples = new BocListCustomColumnTuple[PageSize.Value];
          else
            customColumnTuples = new BocListCustomColumnTuple[Value.Count];
          _customColumns[customColumn] = customColumnTuples;

          for (int idxAbsoluteRows = firstRow, idxRelativeRows = 0;
               idxAbsoluteRows < rowCountWithOffset;
               idxAbsoluteRows++, idxRelativeRows++)
          {
            BocListRow row = rows[idxAbsoluteRows];
            int originalRowIndex = row.Index;

            if (customColumn.Mode == BocCustomColumnDefinitionMode.ControlInEditedRow
                && (_editModeController.EditableRowIndex == null
                    || _editModeController.EditableRowIndex.Value != originalRowIndex))
              continue;

            BocCustomCellArguments args = new BocCustomCellArguments (this, customColumn);
            Control control = customColumn.CustomCell.CreateControlInternal (args);
            control.ID = ID + "_CustomColumnControl_" + idxColumns + "_" + originalRowIndex;
            placeHolder.Controls.Add (control);
            customColumnTuples[idxRelativeRows] = new BocListCustomColumnTuple (row.BusinessObject, originalRowIndex, control);
          }
        }
      }
    }

    /// <summary> Invokes the <see cref="BocCustomColumnDefinitionCell.Init"/> method for each custom column. </summary>
    private void InitCustomColumns ()
    {
      BocColumnDefinition[] columns = EnsureColumnsForPreviousLifeCycleGot();
      for (int idxColumns = 0; idxColumns < columns.Length; idxColumns++)
      {
        BocCustomColumnDefinition customColumn = columns[idxColumns] as BocCustomColumnDefinition;
        if (customColumn != null
            && (customColumn.Mode == BocCustomColumnDefinitionMode.ControlsInAllRows
                || (customColumn.Mode == BocCustomColumnDefinitionMode.ControlInEditedRow
                    && _editModeController.IsRowEditModeActive)))

        {
          BocCustomCellArguments args = new BocCustomCellArguments (this, customColumn);
          customColumn.CustomCell.Init (args);
        }
      }
    }

    /// <summary>
    ///   Invokes the <see cref="BocCustomColumnDefinitionCell.Load"/> method for each cell with a control in the 
    ///   custom columns. 
    /// </summary>
    private void LoadCustomColumns ()
    {
      if (_customColumns == null)
        return;

      BocColumnDefinition[] columns = EnsureColumnsForPreviousLifeCycleGot();
      for (int idxColumns = 0; idxColumns < columns.Length; idxColumns++)
      {
        BocCustomColumnDefinition customColumn = columns[idxColumns] as BocCustomColumnDefinition;
        if (customColumn != null
            && (customColumn.Mode == BocCustomColumnDefinitionMode.ControlsInAllRows
                || customColumn.Mode == BocCustomColumnDefinitionMode.ControlInEditedRow))
        {
          BocListCustomColumnTuple[] customColumnTuples = _customColumns[customColumn];
          for (int idxRows = 0; idxRows < customColumnTuples.Length; idxRows++)
          {
            BocListCustomColumnTuple customColumnTuple = customColumnTuples[idxRows];
            if (customColumnTuple != null)
            {
              int originalRowIndex = customColumnTuple.Item2;
              if (customColumn.Mode == BocCustomColumnDefinitionMode.ControlInEditedRow
                  && (_editModeController.EditableRowIndex == null
                      || _editModeController.EditableRowIndex.Value != originalRowIndex))
                continue;
              IBusinessObject businessObject = customColumnTuple.Item1;
              Control control = customColumnTuple.Item3;

              BocCustomCellLoadArguments args =
                  new BocCustomCellLoadArguments (this, businessObject, customColumn, originalRowIndex, control);
              customColumn.CustomCell.Load (args);
            }
          }
        }
      }
    }

    private bool ValidateCustomColumns ()
    {
      if (_customColumns == null)
        return true;

      if (!_editModeController.IsRowEditModeActive)
        return true;

      bool isValid = true;
      BocColumnDefinition[] columns = EnsureColumnsForPreviousLifeCycleGot();
      for (int idxColumns = 0; idxColumns < columns.Length; idxColumns++)
      {
        BocCustomColumnDefinition customColumn = columns[idxColumns] as BocCustomColumnDefinition;
        if (customColumn != null
            && customColumn.Mode == BocCustomColumnDefinitionMode.ControlInEditedRow)
        {
          BocListCustomColumnTuple[] customColumnTuples = _customColumns[customColumn];
          for (int idxRows = 0; idxRows < customColumnTuples.Length; idxRows++)
          {
            BocListCustomColumnTuple customColumnTuple = customColumnTuples[idxRows];
            if (customColumnTuple != null)
            {
              int originalRowIndex = customColumnTuple.Item2;
              if (_editModeController.EditableRowIndex.Value == originalRowIndex)
              {
                IBusinessObject businessObject = customColumnTuple.Item1;
                Control control = customColumnTuple.Item3;
                BocCustomCellValidationArguments args =
                    new BocCustomCellValidationArguments (this, businessObject, customColumn, control);
                customColumn.CustomCell.Validate (args);
                isValid &= args.IsValid;
              }
            }
          }
        }
      }
      return isValid;
    }

    /// <summary> Invokes the <see cref="BocCustomColumnDefinitionCell.PreRender"/> method for each custom column.  </summary>
    private void PreRenderCustomColumns ()
    {
      BocColumnDefinition[] columns = EnsureColumnsGot (true);
      for (int i = 0; i < columns.Length; i++)
      {
        BocCustomColumnDefinition customColumn = columns[i] as BocCustomColumnDefinition;
        if (customColumn != null)
        {
          BocCustomCellArguments args = new BocCustomCellArguments (this, customColumn);
          customColumn.CustomCell.PreRender (args);
        }
      }
    }

    private BocColumnDefinition[] EnsureColumnsForPreviousLifeCycleGot ()
    {
      if (_columnDefinitionsPostBackEventHandlingPhase == null)
        _columnDefinitionsPostBackEventHandlingPhase = ControlExistedInPreviousRequest ? GetColumnsInternal() : EnsureColumnsGot();
      return _columnDefinitionsPostBackEventHandlingPhase;
    }

    private BocColumnDefinition[] EnsureColumnsGot (bool forceRefresh)
    {
      if (_columnDefinitionsRenderPhase == null || forceRefresh)
        _columnDefinitionsRenderPhase = GetColumnsInternal();
      return _columnDefinitionsRenderPhase;
    }

    private BocColumnDefinition[] EnsureColumnsGot ()
    {
      return EnsureColumnsGot (false);
    }

    private BocColumnRenderer[] GetColumnRenderers (BocColumnDefinition[] columns)
    {
      var columnRendererBuilder = new BocColumnRendererArrayBuilder (columns, ServiceLocator, WcagHelper.Instance);
      columnRendererBuilder.IsListReadOnly = IsReadOnly;
      columnRendererBuilder.EnableIcon = EnableIcon;
      columnRendererBuilder.IsListEditModeActive = _editModeController.IsListEditModeActive;
      columnRendererBuilder.IsBrowserCapableOfScripting = IsBrowserCapableOfScripting;
      columnRendererBuilder.IsClientSideSortingEnabled = IsClientSideSortingEnabled;
      columnRendererBuilder.HasSortingKeys = HasSortingKeys;
      columnRendererBuilder.SortingOrder = _sortingOrder.AsReadOnly();

      return columnRendererBuilder.CreateColumnRenderers ();
    }

    /// <summary>
    ///   Compiles the <see cref="BocColumnDefinition"/> objects from the <see cref="FixedColumns"/>,
    ///   the <see cref="_allPropertyColumns"/> and the <see cref="SelectedView"/>
    ///   into a single array.
    /// </summary>
    /// <returns> An array of <see cref="BocColumnDefinition"/> objects. </returns>
    private BocColumnDefinition[] GetColumnsInternal ()
    {
      _hasAppendedAllPropertyColumnDefinitions = false;

      List<BocColumnDefinition> columnDefinitionList = new List<BocColumnDefinition>();

      AppendFixedColumns (columnDefinitionList);
      if (_showAllProperties)
        EnsureAllPropertyColumnsDefinitionsAppended (null, columnDefinitionList);
      AppendRowMenuColumn (columnDefinitionList);
      AppendSelectedViewColumns (columnDefinitionList);

      var columnDefinitions = GetColumns (columnDefinitionList.ToArray());

      _sortingOrder = RestoreSortingOrderColumns (_sortingOrder, columnDefinitions);

      CheckRowMenuColumns (columnDefinitions);

      return columnDefinitions;
    }

    private void AppendFixedColumns (List<BocColumnDefinition> columnDefinitionList)
    {
      foreach (BocColumnDefinition columnDefinition in _fixedColumns)
      {
        if (columnDefinition is BocAllPropertiesPlaceholderColumnDefinition)
        {
          EnsureAllPropertyColumnsDefinitionsAppended (
              (BocAllPropertiesPlaceholderColumnDefinition) columnDefinition, columnDefinitionList);
        }
        else
          columnDefinitionList.Add (columnDefinition);
      }
    }

    private void AppendRowMenuColumn (List<BocColumnDefinition> columnDefinitionList)
    {
      BocDropDownMenuColumnDefinition dropDownMenuColumn = GetRowMenuColumn();
      if (dropDownMenuColumn != null)
        columnDefinitionList.Add (dropDownMenuColumn);
    }

    private void AppendSelectedViewColumns (List<BocColumnDefinition> columnDefinitionList)
    {
      EnsureSelectedViewIndexSet();
      if (_selectedView == null)
        return;

      foreach (BocColumnDefinition columnDefinition in _selectedView.ColumnDefinitions)
      {
        if (columnDefinition is BocAllPropertiesPlaceholderColumnDefinition)
        {
          EnsureAllPropertyColumnsDefinitionsAppended (
              (BocAllPropertiesPlaceholderColumnDefinition) columnDefinition, columnDefinitionList);
        }
        else
          columnDefinitionList.Add (columnDefinition);
      }
    }

    private void EnsureAllPropertyColumnsDefinitionsAppended (
        BocAllPropertiesPlaceholderColumnDefinition placeholderColumnDefinition, List<BocColumnDefinition> columnDefinitionList)
    {
      if (_hasAppendedAllPropertyColumnDefinitions)
        return;

      BocColumnDefinition[] allPropertyColumnDefinitions = GetAllPropertyColumns();
      Unit width = Unit.Empty;
      string cssClass = string.Empty;

      if (placeholderColumnDefinition != null)
      {
        if (! placeholderColumnDefinition.Width.IsEmpty)
        {
          double value = placeholderColumnDefinition.Width.Value / allPropertyColumnDefinitions.Length;
          value = Math.Round (value, 1);
          width = new Unit (value, placeholderColumnDefinition.Width.Type);
        }
        cssClass = placeholderColumnDefinition.CssClass;
      }

      foreach (BocColumnDefinition columnDefinition in allPropertyColumnDefinitions)
      {
        columnDefinition.CssClass = cssClass;
        columnDefinition.Width = width;
      }

      columnDefinitionList.AddRange (allPropertyColumnDefinitions);
      _hasAppendedAllPropertyColumnDefinitions = true;
    }

    private List<BocListSortingOrderEntry> RestoreSortingOrderColumns (List<BocListSortingOrderEntry> sortingOrder, BocColumnDefinition[] columnDefinitions)
    {
      return sortingOrder
          .Where (entry => !entry.IsEmpty)
          .Select (
              entry =>
              entry.Column == null
                  ? new BocListSortingOrderEntry ((IBocSortableColumnDefinition) columnDefinitions[entry.ColumnIndex], entry.Direction)
                  : entry)
          .ToList();
    }

    /// <summary>
    ///   Override this method to modify the column definitions displayed in the <see cref="BocList"/> in the
    ///   current page life cycle.
    /// </summary>
    /// <remarks>
    ///   This call can happen more than once in the control's life cycle, passing different 
    ///   arrays in <paramref name="columnDefinitions" />. It is therefor important to not cache the return value
    ///   in the override of <see cref="GetColumns"/>.
    /// </remarks>
    /// <param name="columnDefinitions"> 
    ///   The <see cref="BocColumnDefinition"/> array containing the columns defined by the <see cref="BocList"/>. 
    /// </param>
    /// <returns> The <see cref="BocColumnDefinition"/> array. </returns>
    protected virtual BocColumnDefinition[] GetColumns (BocColumnDefinition[] columnDefinitions)
    {
      return columnDefinitions;
    }

    /// <summary>
    ///   Gets a flag set <see langword="true"/> if the <see cref="Value"/> is sorted before it is displayed.
    /// </summary>
    [Browsable (false)]
    public bool HasSortingKeys
    {
      get { return _sortingOrder.Any (entry => !entry.IsEmpty); }
    }

    /// <summary> Sets the sorting order for the <see cref="BocList"/>. </summary>
    /// <remarks>
    ///   <para>
    ///     It is recommended to only set the sorting order when the <see cref="BocList"/> is initialized for the first 
    ///     time. During subsequent postbacks, setting the sorting order before the post back events of the 
    ///     <see cref="BocList"/> have been handled, will undo the user's chosen sorting order.
    ///   </para><para>
    ///     Does not raise the <see cref="SortingOrderChanging"/> and <see cref="SortingOrderChanged"/>.
    ///   </para><para>
    ///     Use <see cref="ClearSortingOrder"/> if you need to clear the sorting order.
    ///   </para>
    /// </remarks>
    /// <param name="newSortingOrder"> 
    ///   The new sorting order of the <see cref="BocList"/>. Must not be <see langword="null"/> or contain 
    ///   <see langword="null"/>.
    /// </param>
    /// <exception cref="InvalidOperationException">EnableMultipleSorting == False &amp;&amp; sortingOrder.Length > 1</exception>
    public void SetSortingOrder (params BocListSortingOrderEntry[] newSortingOrder)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("newSortingOrder", newSortingOrder);

      _sortingOrder.Clear();
      if (! IsMultipleSortingEnabled && newSortingOrder.Length > 1)
        throw new InvalidOperationException ("Attempted to set multiple sorting keys but EnableMultipleSorting is False.");
      else
        _sortingOrder.AddRange (newSortingOrder);

      ResetRows();
    }

    /// <summary> Clears the sorting order for the <see cref="BocList"/>. </summary>
    /// <remarks>
    ///   Does not raise the <see cref="SortingOrderChanging"/> and <see cref="SortingOrderChanged"/>.
    /// </remarks>
    public void ClearSortingOrder ()
    {
      _sortingOrder.Clear();

      ResetRows();
    }

    /// <summary>
    ///   Gets the sorting order for the <see cref="BocList"/>.
    /// </summary>
    /// <remarks>
    ///   If the list also contains a collection of available views, then this method shoud only be called after the 
    ///   <see cref="AvailableViews"/> have been set. Otherwise the result can vary from a wrong sorting order to an 
    ///   <see cref="IndexOutOfRangeException"/>.
    /// </remarks>
    public BocListSortingOrderEntry[] GetSortingOrder ()
    {
      EnsureColumnsGot();
      return _sortingOrder.ToArray ();
    }

    /// <summary>
    ///   Sorts the <see cref="Value"/>'s <see cref="IBusinessObject"/> instances using the sorting keys
    ///   and returns the sorted <see cref="IBusinessObject"/> instances as a new array. The original values remain
    ///   untouched.
    /// </summary>
    /// <returns> 
    ///   An <see cref="IBusinessObject"/> array sorted by the sorting keys or <see langword="null"/> if the list is
    ///   not sorted.
    /// </returns>
    public IBusinessObject[] GetSortedRows ()
    {
      if (! HasSortingKeys)
        return null;

      BocListRow[] sortedRows = EnsureSortedBocListRowsGot();

      return sortedRows.Select (r => r.BusinessObject).ToArray();
    }

    protected BocListRow[] EnsureSortedBocListRowsGot ()
    {
      if (_indexedRowsSorted == null)
        _indexedRowsSorted = GetSortedBocListRows().ToArray();
      return _indexedRowsSorted;
    }

    protected IEnumerable<BocListRow> GetSortedBocListRows ()
    {
      if (!HasValue)
        return new BocListRow[0];

      var rows = Value.Cast<IBusinessObject>().Select ((row, rowIndex) => new BocListRow (rowIndex, row));

      return SortBocListRows (rows, GetSortingOrder());
    }

    protected virtual IEnumerable<BocListRow> SortBocListRows (IEnumerable<BocListRow> rows, BocListSortingOrderEntry[] sortingOrder)
    {
      ArgumentUtility.CheckNotNull ("rows", rows);
      ArgumentUtility.CheckNotNull ("sortingOrder", sortingOrder);

      return rows.OrderBy (sortingOrder);
    }

    BocListRow[] IBocList.GetRowsToDisplay (out int firstRow)
    {
      firstRow = 0;
      int totalRowCount = HasValue ? Value.Count : 0;
      int displayedRowCount = totalRowCount;

      if (IsPagingEnabled && HasValue)
      {
        firstRow = CurrentPage * PageSize.Value;
        displayedRowCount = PageSize.Value;
     

        //  Check row count on last page
        if (Value.Count < (firstRow + displayedRowCount))
          displayedRowCount = Value.Count - firstRow;
      }
      var allRows = EnsureSortedBocListRowsGot();

      BocListRow[] rowsToDisplay = new BocListRow[displayedRowCount];
      for (int i = 0; i < displayedRowCount; i++)
        rowsToDisplay[i] = allRows[firstRow + i];
        
      return rowsToDisplay;
    }

    /// <summary>
    ///   Removes the columns provided by <see cref="SelectedView"/> from the <see cref="_sortingOrder"/> list.
    /// </summary>
    private void RemoveDynamicColumnsFromSortingOrder ()
    {
      if (HasSortingKeys)
      {
        var staticColumns = new HashSet<BocColumnDefinition> (_fixedColumns.Cast<BocColumnDefinition>().Concat (GetAllPropertyColumns()));
        var oldCount = _sortingOrder.Count;
        _sortingOrder.RemoveAll (entry => !staticColumns.Contains ((BocColumnDefinition) entry.Column));
        if (oldCount != _sortingOrder.Count)
          ResetRows();
      }
    }

    /// <summary> Dispatches the resources passed in <paramref name="values"/> to the control's properties. </summary>
    /// <param name="values"> An <c>IDictonary</c>: &lt;string key, string value&gt;. </param>
    void IResourceDispatchTarget.Dispatch (IDictionary values)
    {
      ArgumentUtility.CheckNotNull ("values", values);
      Dispatch (values);
    }

    /// <summary> Dispatches the resources passed in <paramref name="values"/> to the control's properties. </summary>
    /// <param name="values"> An <c>IDictonary</c>: &lt;string key, string value&gt;. </param>
    protected virtual void Dispatch (IDictionary values)
    {
      HybridDictionary fixedColumnValues = new HybridDictionary();
      HybridDictionary optionsMenuItemValues = new HybridDictionary();
      HybridDictionary listMenuItemValues = new HybridDictionary();
      HybridDictionary propertyValues = new HybridDictionary();

      //  Parse the values

      foreach (DictionaryEntry entry in values)
      {
        string key = (string) entry.Key;
        string[] keyParts = key.Split (new[] { ':' }, 3);

        //  Is a property/value entry?
        if (keyParts.Length == 1)
        {
          string property = keyParts[0];
          propertyValues.Add (property, entry.Value);
        }
            //  Is collection entry?
        else if (keyParts.Length == 3)
        {
          //  Compound key: "collectionID:elementID:property"
          string collectionID = keyParts[0];
          string elementID = keyParts[1];
          string property = keyParts[2];

          IDictionary currentCollection = null;

          //  Switch to the right collection
          switch (collectionID)
          {
            case c_resourceKeyFixedColumns:
            {
              currentCollection = fixedColumnValues;
              break;
            }
            case c_resourceKeyOptionsMenuItems:
            {
              currentCollection = optionsMenuItemValues;
              break;
            }
            case c_resourceKeyListMenuItems:
            {
              currentCollection = listMenuItemValues;
              break;
            }
            default:
            {
              //  Invalid collection property
              s_log.Debug (
                  "BocList '" + ID + "' in naming container '" + NamingContainer.GetType().FullName + "' on page '" + Page
                  + "' does not contain a collection property named '" + collectionID + "'.");
              break;
            }
          }

          //  Add the property/value pair to the collection
          if (currentCollection != null)
          {
            //  Get the dictonary for the current element
            IDictionary elementValues = (IDictionary) currentCollection[elementID];

            //  If no dictonary exists, create it and insert it into the elements hashtable.
            if (elementValues == null)
            {
              elementValues = new HybridDictionary();
              currentCollection[elementID] = elementValues;
            }

            //  Insert the argument and resource's value into the dictonary for the specified element.
            elementValues.Add (property, entry.Value);
          }
        }
        else
        {
          //  Not supported format or invalid property
          s_log.Debug (
              "BocList '" + ID + "' in naming container '" + NamingContainer.GetType().FullName + "' on page '" + Page
              + "' received a resource with an invalid or unknown key '" + key
              + "'. Required format: 'property' or 'collectionID:elementID:property'.");
        }
      }

      //  Dispatch simple properties
      ResourceDispatcher.DispatchGeneric (this, propertyValues);

      //  Dispatch to collections
      _fixedColumns.Dispatch (fixedColumnValues, this, "FixedColumns");
      OptionsMenuItems.Dispatch (optionsMenuItemValues, this, "OptionsMenuItems");
      ListMenuItems.Dispatch (listMenuItemValues, this, "ListMenuItems");
    }

    /// <summary> Loads the resources into the control's properties. </summary>
    protected override void LoadResources (IResourceManager resourceManager)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);

      if (IsDesignMode)
        return;
      base.LoadResources (resourceManager);

      string key;
      key = ResourceManagerUtility.GetGlobalResourceKey (IndexColumnTitle);
      if (! StringUtility.IsNullOrEmpty (key))
        IndexColumnTitle = resourceManager.GetString (key);

      key = ResourceManagerUtility.GetGlobalResourceKey (PageInfo);
      if (! StringUtility.IsNullOrEmpty (key))
        PageInfo = resourceManager.GetString (key);

      key = ResourceManagerUtility.GetGlobalResourceKey (EmptyListMessage);
      if (! StringUtility.IsNullOrEmpty (key))
        EmptyListMessage = resourceManager.GetString (key);

      key = ResourceManagerUtility.GetGlobalResourceKey (OptionsTitle);
      if (! StringUtility.IsNullOrEmpty (key))
        OptionsTitle = resourceManager.GetString (key);

      key = ResourceManagerUtility.GetGlobalResourceKey (AvailableViewsListTitle);
      if (! StringUtility.IsNullOrEmpty (key))
        AvailableViewsListTitle = resourceManager.GetString (key);

      key = ResourceManagerUtility.GetGlobalResourceKey (ErrorMessage);
      if (! StringUtility.IsNullOrEmpty (key))
        ErrorMessage = resourceManager.GetString (key);

      _fixedColumns.LoadResources (resourceManager);
      OptionsMenuItems.LoadResources (resourceManager);
      ListMenuItems.LoadResources (resourceManager);
    }

    /// <summary> Is raised when a data row is rendered. </summary>
    [Category ("Action")]
    [Description ("Occurs when a data row is rendered.")]
    public event BocListDataRowRenderEventHandler DataRowRender
    {
      add { Events.AddHandler (s_dataRowRenderEvent, value); }
      remove { Events.RemoveHandler (s_dataRowRenderEvent, value); }
    }

    /// <summary> The <see cref="IBusinessObjectReferenceProperty"/> object this control is bound to. </summary>
    /// <value>An <see cref="IBusinessObjectReferenceProperty"/> object.</value>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public new IBusinessObjectReferenceProperty Property
    {
      get { return (IBusinessObjectReferenceProperty) base.Property; }
      set { base.Property = ArgumentUtility.CheckType<IBusinessObjectReferenceProperty> ("value", value); }
    }

    /// <summary> Gets or sets the current value. </summary>
    /// <value> An object implementing <see cref="IList"/>. </value>
    /// <remarks> The dirty state is reset when the value is set. </remarks>
    [Browsable (false)]
    public new IList Value
    {
      get { return GetValue(); }
      set
      {
        IsDirty = true;
        SetValue (value);
      }
    }

    /// <summary>
    /// Gets the value from the backing field.
    /// </summary>
    protected IList GetValue()
    {
      return _value;
    }

    /// <summary>
    /// Sets the value from the backing field.
    /// </summary>
    /// <remarks>
    /// <para>Setting the value via this method does not affect the control's dirty state.</para>
    /// </remarks>
    protected void SetValue (IList value)
    {
      _value = value;
      ClearSelectedRows();
      ResetRows();
    }

    /// <summary> Gets or sets the current value when <see cref="Value"/> through polymorphism. </summary>
    /// <value> The value must be of type <see cref="IList"/>. </value>
    protected override sealed object ValueImplementation
    {
      get { return Value; }
      set { Value = ArgumentUtility.CheckType<IList> ("value", value); }
    }

    /// <summary>Gets a flag indicating whether the <see cref="BocList"/> contains a value. </summary>
    public override bool HasValue
    {
      get { return _value != null && _value.Count > 0; }
    }

    /// <summary>
    ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; using its 
    ///   <see cref="Control.ClientID"/>.
    /// </summary>
    public override Control TargetControl
    {
      get { return this; }
    }

    /// <summary>
    ///   Gets a flag that determines whether it is valid to generate HTML &lt;label&gt; tags referencing the
    ///   <see cref="TargetControl"/>.
    /// </summary>
    /// <value> Always <see langword="false"/>. </value>
    public override bool UseLabel
    {
      get { return false; }
    }

    /// <summary> Gets or sets the dirty flag. </summary>
    /// <value> 
    ///   Evaluates <see langword="true"/> if either the <see cref="BocList"/> or one of the edit mode controls is 
    ///   dirty.
    /// </value>
    /// <seealso cref="BusinessObjectBoundEditableWebControl.IsDirty">BusinessObjectBoundEditableWebControl.IsDirty</seealso>
    public override bool IsDirty
    {
      get
      {
        if (base.IsDirty)
          return true;

        return _editModeController.IsDirty();
      }
      set { base.IsDirty = value; }
    }

    /// <summary> 
    ///   Returns the <see cref="Control.ClientID"/> values of all controls whose value can be modified in the user 
    ///   interface.
    /// </summary>
    /// <returns> 
    ///   Returns the <see cref="Control.ClientID"/> values of the edit mode controls for the row currently being edited.
    /// </returns>
    /// <seealso cref="BusinessObjectBoundEditableWebControl.GetTrackedClientIDs">BusinessObjectBoundEditableWebControl.GetTrackedClientIDs</seealso>
    public override string[] GetTrackedClientIDs ()
    {
      if (IsReadOnly)
        return new string[0];
      else
        return _editModeController.GetTrackedClientIDs();
    }

    /// <summary> The <see cref="BocList"/> supports properties of type <see cref="IBusinessObjectReferenceProperty"/>. </summary>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportedPropertyInterfaces"/>
    protected override Type[] SupportedPropertyInterfaces
    {
      get { return s_supportedPropertyInterfaces; }
    }

    /// <summary> The <see cref="BocList"/> supports only list properties. </summary>
    /// <returns> <see langword="true"/> if <paramref name="isList"/> is <see langword="true"/>. </returns>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportsPropertyMultiplicity"/>
    protected override bool SupportsPropertyMultiplicity (bool isList)
    {
      return isList;
    }

    /// <summary> Gets the user independent column definitions. </summary>
    /// <remarks> Behavior undefined if set after initialization phase or changed between postbacks. </remarks>
    [PersistenceMode (PersistenceMode.InnerProperty)]
    [ListBindable (false)]
    //  Default category
    [Description ("The user independent column definitions.")]
    [DefaultValue ((string) null)]
    public BocColumnDefinitionCollection FixedColumns
    {
      get { return _fixedColumns; }
    }

    //  No designer support intended
    /// <summary> Gets the predefined column definition sets that the user can choose from at run-time. </summary>
    //  [PersistenceMode(PersistenceMode.InnerProperty)]
    //  [ListBindable (false)]
    //  //  Default category
    //  [Description ("The predefined column definition sets that the user can choose from at run-time.")]
    //  [DefaultValue ((string) null)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public BocListViewCollection AvailableViews
    {
      get { return _availableViews; }
    }

    /// <summary>
    ///   Gets or sets the selected <see cref="BocListView"/> used to
    ///   supplement the <see cref="FixedColumns"/>.
    /// </summary>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public BocListView SelectedView
    {
      get
      {
        EnsureSelectedViewIndexSet();
        return _selectedView;
      }
      set
      {
        bool hasChanged = _selectedView != value;
        _selectedView = value;
        ArgumentUtility.CheckNotNullOrEmpty ("AvailableViews", _availableViews);
        _selectedViewIndex = null;

        if (_selectedView != null)
        {
          for (int i = 0; i < _availableViews.Count; i++)
          {
            if (_availableViews[i] == _selectedView)
            {
              _selectedViewIndex = i;
              break;
            }
          }

          if (_selectedViewIndex == null)
            throw new ArgumentOutOfRangeException ("value");
        }

        if (hasChanged)
          RemoveDynamicColumnsFromSortingOrder();
      }
    }

    private void EnsureSelectedViewIndexSet ()
    {
      if (_isSelectedViewIndexSet)
        return;
      if (_selectedViewIndex == null)
        SelectedViewIndex = _selectedViewIndex;
      else if (_availableViews.Count == 0)
        SelectedViewIndex = null;
      else if (_selectedViewIndex.Value >= _availableViews.Count)
        SelectedViewIndex = _availableViews.Count - 1;
      else
        SelectedViewIndex = _selectedViewIndex;
      _isSelectedViewIndexSet = true;
    }

    /// <summary>
    ///   Gets or sets the index of the selected <see cref="BocListView"/> used to
    ///   supplement the <see cref="FixedColumns"/>.
    /// </summary>
    private int? SelectedViewIndex
    {
      get { return _selectedViewIndex; }
      set
      {
        if (value != null
            && (value.Value < 0 || value.Value >= _availableViews.Count))
          throw new ArgumentOutOfRangeException ("value");

        if ((_editModeController.IsRowEditModeActive || _editModeController.IsListEditModeActive)
            && _isSelectedViewIndexSet
            && _selectedViewIndex != value)
          throw new InvalidOperationException ("The selected column definition set cannot be changed while the BocList is in row edit mode.");

        bool hasIndexChanged = _selectedViewIndex != value;
        _selectedViewIndex = value;

        _selectedView = null;
        if (_selectedViewIndex != null)
        {
          int selectedIndex = _selectedViewIndex.Value;
          if (selectedIndex < _availableViews.Count)
            _selectedView = _availableViews[selectedIndex];
        }

        if (hasIndexChanged)
          RemoveDynamicColumnsFromSortingOrder();
      }
    }

    private void AvailableViews_CollectionChanged (object sender, CollectionChangeEventArgs e)
    {
      if (_selectedViewIndex == null
          && _availableViews.Count > 0)
        _selectedViewIndex = 0;
      else if (_selectedViewIndex >= _availableViews.Count)
      {
        if (_availableViews.Count > 0)
          _selectedViewIndex = _availableViews.Count - 1;
        else
          _selectedViewIndex = null;
      }
    }

    public void ClearSelectedRows ()
    {
      _selectorControlCheckedState.Clear();
    }

    /// <summary> Gets the <see cref="IBusinessObject"/> objects selected in the <see cref="BocList"/>. </summary>
    /// <returns> An array of <see cref="IBusinessObject"/> objects. </returns>
    public IBusinessObject[] GetSelectedBusinessObjects ()
    {
      if (Value == null)
        return new IBusinessObject[0];

      int[] selectedRows = GetSelectedRows();
      IBusinessObject[] selectedBusinessObjects = new IBusinessObject[selectedRows.Length];

      for (int i = 0; i < selectedRows.Length; i++)
      {
        int rowIndex = selectedRows[i];
        IBusinessObject businessObject = Value[rowIndex] as IBusinessObject;
        if (businessObject != null)
          selectedBusinessObjects[i] = businessObject;
      }
      return selectedBusinessObjects;
    }

    /// <summary> Gets indices for the rows selected in the <see cref="BocList"/>. </summary>
    /// <returns> An array of <see cref="int"/> values. </returns>
    public int[] GetSelectedRows ()
    {
      ArrayList selectedRows = new ArrayList();
      foreach (int entry in _selectorControlCheckedState)
      {
        if (entry == c_titleRowIndex)
          continue;

        selectedRows.Add (entry);
      }
      selectedRows.Sort();
      return (int[]) selectedRows.ToArray (typeof (int));
    }

    /// <summary> Sets the <see cref="IBusinessObject"/> objects selected in the <see cref="BocList"/>. </summary>
    /// <param name="selectedObjects"> An <see cref="IList"/> of <see cref="IBusinessObject"/> objects. </param>>
    /// <exception cref="InvalidOperationException"> 
    ///   Thrown if the number of rows do not match the <see cref="Selection"/> mode 
    ///   or the <see cref="Value"/> is <see langword="null"/>.
    /// </exception>
    public void SetSelectedBusinessObjects (IList selectedObjects)
    {
      ArgumentUtility.CheckNotNull ("selectedObjects", selectedObjects);
      ArgumentUtility.CheckItemsNotNullAndType ("selectedObjects", selectedObjects, typeof (IBusinessObject));

      if (Value == null)
        throw new InvalidOperationException (string.Format ("The BocList '{0}' does not have a Value.", ID));

      SetSelectedRows (Utilities.ListUtility.IndicesOf (Value, selectedObjects, false));
    }


    /// <summary> Sets indices for the rows selected in the <see cref="BocList"/>. </summary>
    /// <param name="selectedRows"> An array of <see cref="int"/> values. </param>
    /// <exception cref="InvalidOperationException"> Thrown if the number of rows do not match the <see cref="Selection"/> mode.</exception>
    public void SetSelectedRows (int[] selectedRows)
    {
      if ((_selection == RowSelection.Undefined || _selection == RowSelection.Disabled)
          && selectedRows.Length > 0)
        throw new InvalidOperationException ("Cannot select rows if the BocList is set to RowSelection.Disabled.");

      if ((_selection == RowSelection.SingleCheckBox
           || _selection == RowSelection.SingleRadioButton)
          && selectedRows.Length > 1)
        throw new InvalidOperationException ("Cannot select more than one row if the BocList is set to RowSelection.Single.");

      _selectorControlCheckedState.Clear();
      for (int i = 0; i < selectedRows.Length; i++)
      {
        int rowIndex = selectedRows[i];
        _selectorControlCheckedState.Add (rowIndex);
      }
    }


    /// <summary> Adds the <paramref name="businessObjects"/> to the <see cref="Value"/> collection. </summary>
    /// <remarks> Sets the dirty state. </remarks>
    public void AddRows (IBusinessObject[] businessObjects)
    {
      _editModeController.AddRows (businessObjects, EnsureColumnsForPreviousLifeCycleGot(), EnsureColumnsGot());
    }

    internal void AddRowsInternal (IBusinessObject[] businessObjects)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("businessObjects", businessObjects);

      Value = ListUtility.AddRange (Value, businessObjects, Property, false, true);
    }

    /// <summary> Adds the <paramref name="businessObject"/> to the <see cref="Value"/> collection. </summary>
    /// <remarks> Sets the dirty state. </remarks>
    public int AddRow (IBusinessObject businessObject)
    {
      return _editModeController.AddRow (businessObject, EnsureColumnsForPreviousLifeCycleGot(), EnsureColumnsGot());
    }

    internal int AddRowInternal (IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);

      Value = ListUtility.AddRange (Value, businessObject, Property, false, true);

      if (Value == null)
        return -1;
      else
        return Value.Count - 1;
    }

    /// <summary> Removes the <paramref name="businessObjects"/> from the <see cref="Value"/> collection. </summary>
    /// <remarks> Sets the dirty state. </remarks>
    public void RemoveRows (IBusinessObject[] businessObjects)
    {
      _editModeController.RemoveRows (businessObjects);
    }

    internal void RemoveRowsInternal (IBusinessObject[] businessObjects)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("businessObjects", businessObjects);

      Value = ListUtility.Remove (Value, businessObjects, Property, false);
    }

    /// <summary> Removes the <paramref name="businessObject"/> from the <see cref="Value"/> collection. </summary>
    /// <remarks> Sets the dirty state. </remarks>
    public void RemoveRow (IBusinessObject businessObject)
    {
      _editModeController.RemoveRow (businessObject);
    }

    /// <summary> 
    ///   Removes the <see cref="IBusinessObject"/> at the specifed <paramref name="index"/> from the 
    ///   <see cref="Value"/> collection. 
    /// </summary>
    /// <remarks> Sets the dirty state. </remarks>
    public void RemoveRow (int index)
    {
      if (Value == null)
        return;
      if (index > Value.Count)
        throw new ArgumentOutOfRangeException ("index");

      RemoveRow ((IBusinessObject) Value[index]);
    }

    internal void RemoveRowInternal (IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);

      Value = ListUtility.Remove (Value, businessObject, Property, false);
    }

    /// <summary>
    ///   Saves changes to previous edited row and starts editing for the specified row.
    /// </summary>
    /// <remarks> 
    ///   <para>
    ///     Once the list is in edit mode, it is important not to change to index of the edited 
    ///     <see cref="IBusinessObject"/> in <see cref="Value"/>. Otherwise the wrong object would be edited.
    ///     Use <see cref="IsRowEditModeActive"/> to programatically check whether it is save to insert a row.
    ///     It is always save to add a row using the <see cref="AddRow"/> and <see cref="AddRows"/> methods.
    ///   </para><para>
    ///     While the list is in row edit mode, all commands and menus for this list are disabled with the exception
    ///     of those rendered in the <see cref="BocRowEditModeColumnDefinition"/> column.
    ///   </para>
    /// </remarks>
    /// <param name="index"> The index of the row to be edited. </param>
    public void SwitchRowIntoEditMode (int index)
    {
      _editModeController.SwitchRowIntoEditMode (index, EnsureColumnsForPreviousLifeCycleGot(), EnsureColumnsGot());
    }

    /// <summary>
    ///   Saves changes to the edited row rows and (re-)starts editing for the entire list.
    /// </summary>
    /// <remarks> 
    ///   <para>
    ///     Once the list is in edit mode, it is important not to change to order of the objects in <see cref="Value"/>. 
    ///     Otherwise the wrong objects would be edited. Use <see cref="IsListEditModeActive"/> to programatically check 
    ///     whether it is save to insert a row.
    ///   </para>
    /// </remarks>
    public void SwitchListIntoEditMode ()
    {
      if (IsPagingEnabled)
      {
        throw new InvalidOperationException (
            string.Format (
                "Cannot switch BocList '{0}' in to List Edit Mode: Paging Enabled.", ID));
      }

      _editModeController.SwitchListIntoEditMode (EnsureColumnsForPreviousLifeCycleGot(), EnsureColumnsGot());
    }

    /// <summary> 
    ///   Ends the current edit mode, appends the <paramref name="businessObject"/> to the list and switches the new
    ///   row into edit mode.
    /// </summary>
    /// <remarks>
    ///   If already in row edit mode and the previous row cannot be saved, the new row will not be added to the list.
    /// </remarks>
    /// <param name="businessObject"> The <see cref="IBusinessObject"/> to add. Must not be <see langword="null"/>. </param>
    public bool AddAndEditRow (IBusinessObject businessObject)
    {
      return _editModeController.AddAndEditRow (businessObject, EnsureColumnsForPreviousLifeCycleGot(), EnsureColumnsGot());
    }

    /// <summary>
    ///   Ends the current edit mode and optionally validates and saves the changes made during edit mode.
    /// </summary>
    /// <remarks> 
    ///   If <paramref name="saveChanges"/> is <see langword="true"/>, the edit mode will only be ended once the 
    ///   validation has been successful.
    /// </remarks>
    /// <param name="saveChanges"> 
    ///   <see langword="true"/> to validate and save the changes, <see langword="false"/> to discard them.
    /// </param>
    public void EndRowEditMode (bool saveChanges)
    {
      _editModeController.EndRowEditMode (saveChanges, EnsureColumnsForPreviousLifeCycleGot());
    }

    internal void EndRowEditModeCleanUp (int modifiedRowIndex)
    {
      if (! IsReadOnly)
      {
        ResetRows();
        BocListRow[] sortedRows = EnsureSortedBocListRowsGot();
        for (int idxRows = 0; idxRows < sortedRows.Length; idxRows++)
        {
          int originalRowIndex = sortedRows[idxRows].Index;
          if (modifiedRowIndex == originalRowIndex)
          {
            _currentRow = idxRows;
            break;
          }
        }
      }
    }

    /// <summary>
    ///   Ends the current edit mode and optionally validates and saves the changes made during edit mode.
    /// </summary>
    /// <remarks> 
    ///   If <paramref name="saveChanges"/> is <see langword="true"/>, the edit mode will only be ended once the 
    ///   validation has been successful.
    /// </remarks>
    /// <param name="saveChanges"> 
    ///   <see langword="true"/> to validate and save the changes, <see langword="false"/> to discard them.
    /// </param>
    public void EndListEditMode (bool saveChanges)
    {
      _editModeController.EndListEditMode (saveChanges, EnsureColumnsForPreviousLifeCycleGot());
    }

    internal void EndListEditModeCleanUp ()
    {
      if (! IsReadOnly)
        ResetRows();
    }

    private void EnsureEditModeRestored ()
    {
      _editModeController.EnsureEditModeRestored (EnsureColumnsForPreviousLifeCycleGot());
    }

    private void EnsureEditModeValidatorsRestored ()
    {
      _editModeController.EnsureValidatorsRestored();
    }

    /// <summary> Explicitly validates the changes made to the edit mode. </summary>
    /// <returns> <see langword="true"/> if the rows contain only valid values. </returns>
    public bool ValidateEditableRows ()
    {
      return _editModeController.Validate();
    }

    internal bool ValidateEditableRowsInternal ()
    {
      return ValidateCustomColumns();
    }

    /// <summary> Gets a flag that determines wheter the <see cref="BocList"/> is n row edit mode. </summary>
    /// <remarks>
    ///   Queried where the rendering depends on whether the list is in edit mode. 
    ///   Affected code: sorting buttons, additional columns list, paging buttons, selected column definition set index
    /// </remarks>
    [Browsable (false)]
    public bool IsRowEditModeActive
    {
      get { return _editModeController.IsRowEditModeActive; }
    }

    /// <summary> Gets a flag that determines wheter the <see cref="BocList"/> is n row edit mode. </summary>
    /// <remarks>
    ///   Queried where the rendering depends on whether the list is in edit mode. 
    ///   Affected code: sorting buttons, additional columns list, paging buttons, selected column definition set index
    /// </remarks>
    [Browsable (false)]
    public bool IsListEditModeActive
    {
      get { return _editModeController.IsListEditModeActive; }
    }

    /// <summary> Gets the index of the currently modified row. </summary>
    [Browsable (false)]
    public int? EditableRowIndex
    {
      get { return _editModeController.EditableRowIndex; }
    }

    /// <summary>
    ///   Gets or sets a flag that determines whether to show the asterisks in the title row for columns having 
    ///   edit mode controls.
    /// </summary>
    [Description ("Set false to hide the asterisks in the title row for columns having edit mode control.")]
    [Category ("Edit Mode")]
    [DefaultValue (true)]
    public bool ShowEditModeRequiredMarkers
    {
      get { return _editModeController.ShowEditModeRequiredMarkers; }
      set { _editModeController.ShowEditModeRequiredMarkers = value; }
    }

    /// <summary>
    ///   Gets or sets a flag that determines whether to show an exclamation mark in front of each control with 
    ///   an validation error.
    /// </summary>
    [Description ("Set true to show an exclamation mark in front of each control with an validation error.")]
    [Category ("Edit Mode")]
    [DefaultValue (false)]
    public bool ShowEditModeValidationMarkers
    {
      get { return _editModeController.ShowEditModeValidationMarkers; }
      set { _editModeController.ShowEditModeValidationMarkers = value; }
    }

    /// <summary>
    ///   Gets or sets a flag that determines whether to render validation messages and client side validators.
    /// </summary>
    [Description ("Set true to prevent the validation messages from being rendered. This also disables any client side validation in the edited row.")
    ]
    [Category ("Edit Mode")]
    [DefaultValue (false)]
    public bool DisableEditModeValidationMessages
    {
      get { return _editModeController.DisableEditModeValidationMessages; }
      set { _editModeController.DisableEditModeValidationMessages = value; }
    }

    /// <summary> Gets or sets a flag that enables the <see cref="EditModeValidator"/>. </summary>
    /// <remarks> 
    ///   <see langword="false"/> to prevent the <see cref="EditModeValidator"/> from being created by
    ///   <see cref="CreateValidators"/>.
    /// </remarks>
    [Description ("Enables the EditModeValidator.")]
    [Category ("Edit Mode")]
    [DefaultValue (true)]
    public bool EnableEditModeValidator
    {
      get { return _editModeController.EnableEditModeValidator; }
      set { _editModeController.EnableEditModeValidator = value; }
    }

    /// <summary> Is raised before the changes to the editable row are saved. </summary>
    [Category ("Action")]
    [Description ("Is raised before the changes to the editable row are saved.")]
    public event BocListEditableRowChangesEventHandler EditableRowChangesSaving
    {
      add { Events.AddHandler (s_editableRowChangesSavingEvent, value); }
      remove { Events.RemoveHandler (s_editableRowChangesSavingEvent, value); }
    }

    /// <summary> Is raised after the changes to the editable row have been saved. </summary>
    [Category ("Action")]
    [Description ("Is raised after the changes to the editable row have been saved.")]
    public event BocListItemEventHandler EditableRowChangesSaved
    {
      add { Events.AddHandler (s_editableRowChangesSavedEvent, value); }
      remove { Events.RemoveHandler (s_editableRowChangesSavedEvent, value); }
    }

    /// <summary> Is raised before the changes to the editable row are canceled. </summary>
    [Category ("Action")]
    [Description ("Is raised before the changes to the editable row are canceled.")]
    public event BocListEditableRowChangesEventHandler EditableRowChangesCanceling
    {
      add { Events.AddHandler (s_editableRowChangesCancelingEvent, value); }
      remove { Events.RemoveHandler (s_editableRowChangesCancelingEvent, value); }
    }

    /// <summary> Is raised after the changes to the editable row have been canceled. </summary>
    [Category ("Action")]
    [Description ("Is raised after the changes to the editable row have been canceled.")]
    public event BocListItemEventHandler EditableRowChangesCanceled
    {
      add { Events.AddHandler (s_editableRowChangesCanceledEvent, value); }
      remove { Events.RemoveHandler (s_editableRowChangesCanceledEvent, value); }
    }

    /// <summary> 
    ///   Gets or sets the <see cref="EditableRowDataSourceFactory"/> used to create the data souce for the edit mode
    ///   controls.
    /// </summary>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public EditableRowDataSourceFactory EditModeDataSourceFactory
    {
      get { return _editModeDataSourceFactory; }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        _editModeDataSourceFactory = value;
      }
    }

    /// <summary> 
    ///   Gets or sets the <see cref="EditableRowControlFactory"/> used to create the controls for the edit mode.
    /// </summary>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public EditableRowControlFactory EditModeControlFactory
    {
      get { return _editModeControlFactory; }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        _editModeControlFactory = value;
      }
    }

    protected internal virtual void OnEditableRowChangesSaving (
        int index,
        IBusinessObject businessObject,
        IBusinessObjectDataSource dataSource,
        IBusinessObjectBoundEditableWebControl[] controls)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);
      ArgumentUtility.CheckNotNull ("dataSource", dataSource);
      ArgumentUtility.CheckNotNull ("controls", controls);

      BocListEditableRowChangesEventHandler handler =
          (BocListEditableRowChangesEventHandler) Events[s_editableRowChangesSavingEvent];
      if (handler != null)
      {
        BocListEditableRowChangesEventArgs e =
            new BocListEditableRowChangesEventArgs (index, businessObject, dataSource, controls);
        handler (this, e);
      }
    }

    protected internal virtual void OnEditableRowChangesSaved (int index, IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);

      BocListItemEventHandler handler = (BocListItemEventHandler) Events[s_editableRowChangesSavedEvent];
      if (handler != null)
      {
        BocListItemEventArgs e = new BocListItemEventArgs (index, businessObject);
        handler (this, e);
      }
    }

    protected internal virtual void OnEditableRowChangesCanceling (
        int index,
        IBusinessObject businessObject,
        IBusinessObjectDataSource dataSource,
        IBusinessObjectBoundEditableWebControl[] controls)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);
      ArgumentUtility.CheckNotNull ("dataSource", dataSource);
      ArgumentUtility.CheckNotNull ("controls", controls);

      BocListEditableRowChangesEventHandler handler =
          (BocListEditableRowChangesEventHandler) Events[s_editableRowChangesCancelingEvent];
      if (handler != null)
      {
        BocListEditableRowChangesEventArgs e =
            new BocListEditableRowChangesEventArgs (index, businessObject, dataSource, controls);
        handler (this, e);
      }
    }

    protected internal virtual void OnEditableRowChangesCanceled (int index, IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);

      BocListItemEventHandler handler = (BocListItemEventHandler) Events[s_editableRowChangesCanceledEvent];
      if (handler != null)
      {
        BocListItemEventArgs e = new BocListItemEventArgs (index, businessObject);
        handler (this, e);
      }
    }


    /// <summary> Adds the <paramref name="businessObjects"/> to the <see cref="Value"/> collection. </summary>
    /// <remarks> Sets the dirty state. </remarks>
    protected virtual void InsertBusinessObjects (IBusinessObject[] businessObjects)
    {
      AddRows (businessObjects);
    }

    /// <summary> Removes the <paramref name="businessObjects"/> from the <see cref="Value"/> collection. </summary>
    /// <remarks> Sets the dirty state. </remarks>
    protected virtual void RemoveBusinessObjects (IBusinessObject[] businessObjects)
    {
      RemoveRows (businessObjects);
    }

    private void MenuItemEventCommandClick (object sender, WebMenuItemClickEventArgs e)
    {
      OnMenuItemEventCommandClick (e.Item);
    }

    /// <summary> Fires the <see cref="MenuItemClick"/> event. </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocList.xml' path='BocList/OnMenuItemEventCommandClick/*' />
    protected virtual void OnMenuItemEventCommandClick (WebMenuItem menuItem)
    {
      ArgumentUtility.CheckNotNull ("menuItem", menuItem);

      // Just pro forma. MenuBase already fired Command.Click before click-handler is invoked.
      // OnClick only fires once because of a guard-condition.
      if (menuItem.Command != null)
        menuItem.Command.OnClick ();

      if (menuItem is BocMenuItem && menuItem.Command is BocMenuItemCommand)
        ((BocMenuItemCommand) menuItem.Command).OnClick ((BocMenuItem) menuItem);

      WebMenuItemClickEventHandler menuItemClickHandler = (WebMenuItemClickEventHandler) Events[s_menuItemClickEvent];
      if (menuItemClickHandler != null)
      {
        WebMenuItemClickEventArgs e = new WebMenuItemClickEventArgs (menuItem);
        menuItemClickHandler (this, e);
      }
    }

    private void MenuItemWxeFunctionCommandClick (object sender, WebMenuItemClickEventArgs e)
    {
      OnMenuItemWxeFunctionCommandClick (e.Item);
    }

    /// <summary> Handles the click to a WXE function command. </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocList.xml' path='BocList/OnMenuItemWxeFunctionCommandClick/*' />
    protected virtual void OnMenuItemWxeFunctionCommandClick (WebMenuItem menuItem)
    {
      ArgumentUtility.CheckNotNull ("menuItem", menuItem);

      if (menuItem.Command == null)
        return;

      if (menuItem is BocMenuItem)
      {
        BocMenuItemCommand command = (BocMenuItemCommand) menuItem.Command;
        if (Page is IWxePage)
          command.ExecuteWxeFunction ((IWxePage) Page, GetSelectedRows(), GetSelectedBusinessObjects());
        //else
        //  command.ExecuteWxeFunction (Page, GetSelectedRows(), GetSelectedBusinessObjects());
      }
      else
      {
        Command command = menuItem.Command;
        if (Page is IWxePage)
          command.ExecuteWxeFunction ((IWxePage) Page, null);
        //else
        //  command.ExecuteWxeFunction (Page, null, new NameValueCollection (0));
      }
    }

    bool IBocMenuItemContainer.IsReadOnly
    {
      get { return IsReadOnly; }
    }

    bool IBocMenuItemContainer.IsSelectionEnabled
    {
      get { return IsSelectionEnabled; }
    }

    IBusinessObject[] IBocMenuItemContainer.GetSelectedBusinessObjects ()
    {
      return GetSelectedBusinessObjects();
    }

    void IBocMenuItemContainer.InsertBusinessObjects (IBusinessObject[] businessObjects)
    {
      InsertBusinessObjects (businessObjects);
    }

    void IBocMenuItemContainer.RemoveBusinessObjects (IBusinessObject[] businessObjects)
    {
      RemoveBusinessObjects (businessObjects);
    }

    /// <summary> 
    ///   Gets or sets a flag that determines wheter an empty list will still render its headers 
    ///   and the additonal column sets  (read-only mode only). 
    /// </summary>
    /// <value> <see langword="false"/> to hide the headers and the addtional column sets if the list is empty. </value>
    [Category ("Appearance")]
    [Description ("Determines whether the list headers and the additional column sets will be rendered if no data is provided (read-only mode only).")
    ]
    [DefaultValue (false)]
    public virtual bool ShowEmptyListReadOnlyMode
    {
      get { return _showEmptyListReadOnlyMode; }
      set { _showEmptyListReadOnlyMode = value; }
    }

    /// <summary> 
    ///   Gets or sets a flag that determines wheter an empty list will still render its headers 
    ///   and the additonal column sets (edit mode only). 
    /// </summary>
    /// <value> <see langword="false"/> to hide the headers and the addtional column sets if the list is empty. </value>
    [Category ("Appearance")]
    [Description ("Determines whether the list headers and the additional column sets will be rendered if no data is provided (edit mode only).")]
    [DefaultValue (true)]
    public virtual bool ShowEmptyListEditMode
    {
      get { return _showEmptyListEditMode; }
      set { _showEmptyListEditMode = value; }
    }

    /// <summary> 
    ///   Gets or sets a flag that determines wheter an empty list will still render its option and list menus. 
    ///   (read-only mode only).
    /// </summary>
    /// <value> <see langword="false"/> to hide the option and list menus if the list is empty. </value>
    [Category ("Menu")]
    [Description ("Determines whether the options and list menus will be rendered if no data is provided (read-only mode only).")]
    [DefaultValue (false)]
    public virtual bool ShowMenuForEmptyListReadOnlyMode
    {
      get { return _showMenuForEmptyListReadOnlyMode; }
      set { _showMenuForEmptyListReadOnlyMode = value; }
    }

    /// <summary> 
    ///   Gets or sets a flag that determines wheter an empty list will still render its option and list menus
    ///   (edit mode only).
    /// </summary>
    /// <value> <see langword="false"/> to hide the option and list menus if the list is empty. </value>
    [Category ("Menu")]
    [Description ("Determines whether the options and list menus will be rendered if no data is provided (edit mode only).")]
    [DefaultValue (true)]
    public virtual bool ShowMenuForEmptyListEditMode
    {
      get { return _showMenuForEmptyListEditMode; }
      set { _showMenuForEmptyListEditMode = value; }
    }

    /// <summary>
    ///   Gets or sets a flag that indicates whether the control automatically generates a column 
    ///   for each property of the bound object.
    /// </summary>
    /// <value> <see langword="true"/> show all properties of the bound business object. </value>
    [Category ("Appearance")]
    [Description ("Indicates whether the control automatically generates a column for each property of the bound object.")]
    [DefaultValue (false)]
    public virtual bool ShowAllProperties
    {
      get { return _showAllProperties; }
      set { _showAllProperties = value; }
    }

    /// <summary>
    ///   Gets or sets a flag that indicates whether to display an icon in front of the first value 
    ///   column.
    /// </summary>
    /// <value> <see langword="true"/> to enable the icon. </value>
    [Category ("Appearance")]
    [Description ("Enables the icon in front of the first value column.")]
    [DefaultValue (true)]
    public virtual bool EnableIcon
    {
      get { return _enableIcon; }
      set { _enableIcon = value; }
    }

    /// <summary>
    ///   Gets or sets a flag that determines whether to to enable cleint side sorting.
    /// </summary>
    /// <value> <see langword="true"/> to enable the sorting buttons. </value>
    [Category ("Behavior")]
    [Description ("Enables the sorting button in front of each value column's header.")]
    [DefaultValue (true)]
    public virtual bool EnableSorting
    {
      get { return _enableSorting; }
      set { _enableSorting = value; }
    }

    protected bool IsClientSideSortingEnabled
    {
      get { return EnableSorting; }
    }

    bool IBocList.IsClientSideSortingEnabled
    {
      get { return IsClientSideSortingEnabled; }
    }

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
    [Category ("Appearance")]
    [Description ("Enables the sorting order display after each sorting button. Undefined is interpreted as true.")]
    [DefaultValue (typeof (bool?), "")]
    public virtual bool? ShowSortingOrder
    {
      get { return _showSortingOrder; }
      set { _showSortingOrder = value; }
    }

    protected virtual bool IsShowSortingOrderEnabled
    {
      get { return ShowSortingOrder != false; }
    }

    bool IBocList.IsShowSortingOrderEnabled
    {
      get { return IsShowSortingOrderEnabled; }
    }

    [Category ("Behavior")]
    [Description ("Enables sorting by multiple columns. Undefined is interpreted as true.")]
    [DefaultValue (typeof (bool?), "")]
    public virtual bool? EnableMultipleSorting
    {
      get { return _enableMultipleSorting; }
      set
      {
        _enableMultipleSorting = value;
        if (_sortingOrder.Count > 1 && ! IsMultipleSortingEnabled)
        {
          BocListSortingOrderEntry entry = (BocListSortingOrderEntry) _sortingOrder[0];
          _sortingOrder.Clear();
          _sortingOrder.Add (entry);
        }
      }
    }

    protected virtual bool IsMultipleSortingEnabled
    {
      get { return EnableMultipleSorting != false; }
    }

    /// <summary>
    ///   Gets or sets a flag that determines whether to display the options menu.
    /// </summary>
    /// <value> <see langword="true"/> to show the options menu. </value>
    [Category ("Menu")]
    [Description ("Enables the options menu.")]
    [DefaultValue (true)]
    public virtual bool ShowOptionsMenu
    {
      get { return _showOptionsMenu; }
      set { _showOptionsMenu = value; }
    }

    /// <summary>
    ///   Gets or sets a flag that determines whether to display the list menu.
    /// </summary>
    /// <value> <see langword="true"/> to show the list menu. </value>
    [Category ("Menu")]
    [Description ("Enables the list menu.")]
    [DefaultValue (true)]
    public virtual bool ShowListMenu
    {
      get { return _showListMenu; }
      set { _showListMenu = value; }
    }

    /// <summary> Gets or sets a value that determines if the row menu is being displayed. </summary>
    /// <value> <see cref="!:RowMenuDisplay.Undefined"/> is interpreted as <see cref="!:RowMenuDisplay.Disabled"/>. </value>
    [Category ("Menu")]
    [Description ("Enables the row menu. Undefined is interpreted as Disabled.")]
    [DefaultValue (RowMenuDisplay.Undefined)]
    public RowMenuDisplay RowMenuDisplay
    {
      get { return _rowMenuDisplay; }
      set { _rowMenuDisplay = value; }
    }

    /// <summary>
    ///   Gets or sets a value that indicating the row selection mode.
    /// </summary>
    /// <remarks> 
    ///   If row selection is enabled, the control displays a checkbox in front of each row
    ///   and highlights selected data rows.
    /// </remarks>
    [Category ("Behavior")]
    [Description ("Indicates whether row selection is enabled.")]
    [DefaultValue (RowSelection.Undefined)]
    public virtual RowSelection Selection
    {
      get { return _selection; }
      set { _selection = value; }
    }

    protected internal bool IsSelectionEnabled
    {
      get { return _selection != RowSelection.Undefined && _selection != RowSelection.Disabled; }
    }

    /// <summary> Gets or sets a value that indicating the row index is enabled. </summary>
    /// <value> 
    ///   <see langword="RowIndex.InitialOrder"/> to show the of the initial (unsorted) list and
    ///   <see langword="RowIndex.SortedOrder"/> to show the index based on the current sorting order. 
    ///   Defaults to <see cref="RowIndex.Undefined"/>, which is interpreted as <see langword="RowIndex.Disabled"/>.
    /// </value>
    /// <remarks> If row selection is enabled, the control displays an index in front of each row. </remarks>
    [Category ("Appearance")]
    [Description ("Indicates whether the row index is enabled. Undefined is interpreted as Disabled.")]
    [DefaultValue (RowIndex.Undefined)]
    public virtual RowIndex Index
    {
      get { return _index; }
      set { _index = value; }
    }

    protected bool IsIndexEnabled
    {
      get { return _index != RowIndex.Undefined && _index != RowIndex.Disabled; }
    }

    bool IBocList.IsIndexEnabled
    {
      get { return IsIndexEnabled; }
    }

    /// <summary> Gets or sets the offset for the rendered index. </summary>
    /// <value> Defaults to <see langword="null"/>. </value>
    [Category ("Appearance")]
    [Description ("The offset for the rendered index.")]
    [DefaultValue (typeof (int?), "")]
    public int? IndexOffset
    {
      get { return _indexOffset; }
      set { _indexOffset = value; }
    }


    /// <summary> Gets or sets the text that is displayed in the index column's title row. </summary>
    /// <remarks> The value will not be HTML encoded. </remarks>
    [Category ("Appearance")]
    [Description ("The text that is displayed in the index column's title row. The value will not be HTML encoded.")]
    [DefaultValue (null)]
    public string IndexColumnTitle
    {
      get { return _indexColumnTitle; }
      set { _indexColumnTitle = value; }
    }

    protected bool AreDataRowsClickSensitive ()
    {
      return HasClientScript
             && ! WcagHelper.Instance.IsWaiConformanceLevelARequired()
             && IsBrowserCapableOfScripting;
    }

    bool IBocList.AreDataRowsClickSensitive ()
    {
      return AreDataRowsClickSensitive();
    }

    /// <summary> The number of rows displayed per page. </summary>
    /// <value> 
    ///   An integer greater than zero to limit the number of rows per page to the specified value,
    ///   or zero, less than zero or <see langword="null"/> to show all rows.
    /// </value>
    [Category ("Appearance")]
    [Description ("The number of rows displayed per page. Set PageSize to 0 to show all rows.")]
    [DefaultValue (typeof (int?), "")]
    public virtual int? PageSize
    {
      get { return _pageSize; }
      set
      {
        if (value == null || value.Value < 0)
          _pageSize = null;
        else
          _pageSize = value;
      }
    }

    protected bool IsPagingEnabled
    {
      get { return ! WcagHelper.Instance.IsWaiConformanceLevelARequired() && _pageSize != null && _pageSize.Value != 0; }
    }

    bool IBocList.IsPagingEnabled
    {
      get { return IsPagingEnabled; }
    }

    /// <summary>
    ///   Gets or sets a flag that indicates whether to the show the page count even when there 
    ///   is just one page.
    /// </summary>
    /// <value> 
    ///   <see langword="true"/> to force showing the page info, even if the rows fit onto a single 
    ///   page.
    /// </value>
    [Category ("Behavior")]
    [Description ("Indicates whether to the show the page count even when there is just one page.")]
    [DefaultValue (false)]
    public virtual bool AlwaysShowPageInfo
    {
      get { return _alwaysShowPageInfo; }
      set { _alwaysShowPageInfo = value; }
    }

    /// <summary> Gets or sets the text providing the current page information to the user. </summary>
    /// <remarks> Use {0} for the current page and {1} for the total page count. The value will not be HTML encoded. </remarks>
    [Category ("Appearance")]
    [Description (
        "The text providing the current page information to the user. Use {0} for the current page and {1} for the total page count. The value will not be HTML encoded."
        )]
    [DefaultValue (null)]
    public string PageInfo
    {
      get { return _pageInfo; }
      set { _pageInfo = value; }
    }

    /// <summary> Gets or sets the text rendered if the list is empty. </summary>
    /// <remarks> The value will not be HTML encoded. </remarks>
    [Category ("Appearance")]
    [Description ("The text if the list is empty. The value will not be HTML encoded.")]
    [DefaultValue (null)]
    public string EmptyListMessage
    {
      get { return _emptyListMessage; }
      set { _emptyListMessage = value; }
    }

    /// <summary> Gets or sets a flag whether to render the <see cref="EmptyListMessage"/>. </summary>
    [Category ("Appearance")]
    [Description ("A flag that determines whether the EmpryListMessage is rendered.")]
    [DefaultValue (false)]
    public bool ShowEmptyListMessage
    {
      get { return _showEmptyListMessage; }
      set { _showEmptyListMessage = value; }
    }

    /// <summary> Gets or sets a flag that determines whether the client script is enabled. </summary>
    /// <remarks> Effects only advanced scripts used for selcting data rows. </remarks>
    /// <value> <see langref="true"/> to enable the client script. </value>
    [Category ("Behavior")]
    [Description (" True to enable the client script for the pop-up calendar. ")]
    [DefaultValue (true)]
    public bool EnableClientScript
    {
      get { return _enableClientScript; }
      set { _enableClientScript = value; }
    }

    /// <summary> Is raised when a column type <see cref="BocCustomColumnDefinition"/> is clicked on. </summary>
    [Category ("Action")]
    [Description ("Occurs when a custom column is clicked on.")]
    public event BocCustomCellClickEventHandler CustomCellClick
    {
      add { Events.AddHandler (s_customCellClickEvent, value); }
      remove { Events.RemoveHandler (s_customCellClickEvent, value); }
    }

    /// <summary> Is raised when a column with a command of type <see cref="CommandType.Event"/> is clicked. </summary>
    [Category ("Action")]
    [Description ("Occurs when a column with a command of type Event is clicked inside an column.")]
    public event BocListItemCommandClickEventHandler ListItemCommandClick
    {
      add { Events.AddHandler (s_listItemCommandClickEvent, value); }
      remove { Events.RemoveHandler (s_listItemCommandClickEvent, value); }
    }

    /// <summary> Is raised when a menu item with a command of type <see cref="CommandType.Event"/> is clicked. </summary>
    [Category ("Action")]
    [Description ("Is raised when a menu item with a command of type Event is clicked.")]
    public event WebMenuItemClickEventHandler MenuItemClick
    {
      add { Events.AddHandler (s_menuItemClickEvent, value); }
      remove { Events.RemoveHandler (s_menuItemClickEvent, value); }
    }

    /// <summary> Gets or sets the offset between the items in the <c>menu block</c>. </summary>
    /// <remarks> The <see cref="MenuBlockOffset"/> is applied as a <c>margin</c> attribute. </remarks>
    [Category ("Menu")]
    [Description ("The offset between the items in the menu section.")]
    [DefaultValue (typeof (Unit), "")]
    public Unit MenuBlockItemOffset
    {
      get { return _menuBlockItemOffset; }
      set { _menuBlockItemOffset = value; }
    }

    /// <summary> Gets the <see cref="BocMenuItem"/> objects displayed in the <see cref="BocList"/>'s options menu. </summary>
    [PersistenceMode (PersistenceMode.InnerProperty)]
    [ListBindable (false)]
    [Category ("Menu")]
    [Description ("The menu items displayed by options menu.")]
    [DefaultValue ((string) null)]
    [Editor (typeof (BocMenuItemCollectionEditor), typeof (UITypeEditor))]
    public WebMenuItemCollection OptionsMenuItems
    {
      get { return _optionsMenu.MenuItems; }
    }

    /// <summary> Gets the <see cref="BocMenuItem"/> objects displayed in the <see cref="BocList"/>'s menu area. </summary>
    [PersistenceMode (PersistenceMode.InnerProperty)]
    [ListBindable (false)]
    [Category ("Menu")]
    [Description ("The menu items displayed in the list's menu area.")]
    [DefaultValue ((string) null)]
    [Editor (typeof (BocMenuItemCollectionEditor), typeof (UITypeEditor))]
    public WebMenuItemCollection ListMenuItems
    {
      get { return _listMenu.MenuItems; }
    }

    /// <summary> Gets or sets the width reserved for the menu block. </summary>
    [Category ("Menu")]
    [Description ("The width reserved for the menu block.")]
    [DefaultValue (typeof (Unit), "")]
    public Unit MenuBlockWidth
    {
      get { return _menuBlockWidth; }
      set { _menuBlockWidth = value; }
    }

    /// <summary> Gets or sets the offset between the table and the menu block. </summary>
    [Category ("Menu")]
    [Description ("The offset between the table and the menu block.")]
    [DefaultValue (typeof (Unit), "")]
    public Unit MenuBlockOffset
    {
      get { return _menuBlockOffset; }
      set { _menuBlockOffset = value; }
    }

    /// <summary> Gets or sets the list of menu items to be hidden. </summary>
    /// <value> The <see cref="WebMenuItem.ItemID"/> values of the menu items to hide. </value>
    [Category ("Menu")]
    [Description ("The list of menu items to be hidden, identified by their ItemIDs.")]
    [DefaultValue ((string) null)]
    [PersistenceMode (PersistenceMode.Attribute)]
    [TypeConverter (typeof (StringArrayConverter))]
    public string[] HiddenMenuItems
    {
      get
      {
        if (_hiddenMenuItems == null)
          return new string[0];
        return _hiddenMenuItems;
      }
      set { _hiddenMenuItems = value; }
    }

    /// <summary>
    ///   Gets or sets a value that indicates whether the control displays a drop down list 
    ///   containing the available column definition sets.
    /// </summary>
    [Category ("Menu")]
    [Description ("Indicates whether the control displays a drop down list containing the available views.")]
    [DefaultValue (true)]
    public bool ShowAvailableViewsList
    {
      get { return _showAvailableViewsList; }
      set { _showAvailableViewsList = value; }
    }

    /// <summary> Gets or sets the text that is rendered as a title for the drop list of additional columns. </summary>
    /// <remarks> The value will not be HTML encoded. </remarks>
    [Category ("Menu")]
    [Description ("The text that is rendered as a title for the list of available views. The value will not be HTML encoded.")]
    [DefaultValue ("")]
    public string AvailableViewsListTitle
    {
      get { return _availableViewsListTitle; }
      set { _availableViewsListTitle = value; }
    }

    /// <summary> Gets or sets the text that is rendered as a label for the <c>options menu</c>. </summary>
    [Category ("Menu")]
    [Description ("The text that is rendered as a label for the options menu.")]
    [DefaultValue ("")]
    public string OptionsTitle
    {
      get { return _optionsTitle; }
      set { _optionsTitle = value; }
    }

    /// <summary> Gets or sets the rendering option for the <c>list menu</c>. </summary>
    [Category ("Menu")]
    [Description ("Defines how the items will be rendered.")]
    [DefaultValue (ListMenuLineBreaks.All)]
    public ListMenuLineBreaks ListMenuLineBreaks
    {
      get { return _listMenu.LineBreaks; }
      set { _listMenu.LineBreaks = value; }
    }

    /// <summary> Gets or sets the validation error message. </summary>
    /// <value> 
    ///   The error message displayed when validation fails. The default value is an empty <see cref="String"/>.
    ///   In case of the default value, the text is read from the resources for this control.
    /// </value>
    [Description ("Validation message displayed if there is an error.")]
    [Category ("Validator")]
    [DefaultValue ("")]
    public string ErrorMessage
    {
      get { return _errorMessage; }
      set
      {
        _errorMessage = value;
        for (int i = 0; i < _validators.Count; i++)
        {
          BaseValidator validator = (BaseValidator) _validators[i];
          validator.ErrorMessage = _errorMessage;
        }
      }
    }

    public int CurrentPage
    {
      get { return _currentPage; }
    }

    public int PageCount
    {
      get { return _pageCount; }
    }

    public bool HasClientScript
    {
      get { return (!IsDesignMode && EnableClientScript); }
    }

    DropDownList IBocList.AvailableViewsList
    {
      get { return _availableViewsList; }
    }

    IDropDownMenu IBocList.OptionsMenu
    {
      get
      {
        if (string.IsNullOrEmpty (OptionsTitle))
          _optionsMenu.TitleText = GetResourceManager().GetString (ResourceIdentifier.OptionsTitle);
        else
          _optionsMenu.TitleText = OptionsTitle;

        _optionsMenu.Enabled = !_editModeController.IsRowEditModeActive;
        _optionsMenu.IsReadOnly = IsReadOnly;

        return _optionsMenu;
      }
    }

    IListMenu IBocList.ListMenu
    {
      get { return _listMenu; }
    }

    IList<int> IBocList.SelectorControlCheckedState
    {
      get { return _selectorControlCheckedState; }
    }

    IEditModeController IBocList.EditModeController
    {
      get { return _editModeController; }
    }

    ArrayList IBocList.Validators
    {
      get { return _validators; }
    }

    BocListRowMenuTuple[] IBocList.RowMenus
    {
      get { return _rowMenus; }
    }

    IDictionary<BocColumnDefinition, BocListCustomColumnTuple[]> IBocList.CustomColumns
    {
      get { return _customColumns; }
    }

    bool IBocRenderableControl.IsDesignMode
    {
      get { return IsDesignMode; }
    }

    string IBocList.GetSelectorControlClientId (int? rowIndex)
    {
      return ClientID + c_dataRowSelectorControlIDSuffix + (rowIndex.HasValue ? rowIndex.Value.ToString() : string.Empty);
    }

    string IBocList.GetSelectAllControlClientID ()
    {
      return ClientID + c_titleRowSelectorControlIDSuffix;
    }

    string IBocList.GetSelectionChangedHandlerScript()
    {
      return HasListMenu
                 ? string.Format ("function(bocList) {{ {0} }}", _listMenu.GetUpdateScriptReference (GetSelectionCountScript()))
                 : "function(){{}}";
    }

    protected string GetSelectionCountScript ()
    {
      return "function() { return BocList_GetSelectionCount ('" + ClientID + "'); }";
    }
  }

  public enum RowSelection
  {
    Undefined = -1,
    Disabled = 0,
    SingleCheckBox = 1,
    SingleRadioButton = 2,
    Multiple = 3
  }

  public enum RowIndex
  {
    Undefined,
    Disabled,
    InitialOrder,
    SortedOrder
  }

  public enum RowMenuDisplay
  {
    Undefined,
    /// <summary> No menus will be shown, even if a <see cref="BocDropDownMenuColumnDefinition"/> has been created. </summary>
    Disabled,
    /// <summary> The developer must manually provide a <see cref="BocDropDownMenuColumnDefinition"/>. </summary>
    Manual,
    /// <summary> The <see cref="BocList"/> will automatically create a <see cref="BocDropDownMenuColumnDefinition"/>. </summary>
    Automatic
  }
}
