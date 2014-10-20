using System;
using System.Collections.Generic;
using System.Linq;
using Coypu;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.ObjectBinding.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/>.
  /// </summary>
  [UsedImplicitly]
  public class BocListControlObject : BocControlObject
  {
    // Todo RM-6297: Add Header helper class.
    private readonly List<string> _headerItemIDs;
    private readonly List<string> _headerLabels;

    public BocListControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
      _headerItemIDs = RetryUntilTimeout.Run (
          () => Scope.FindAllCss (".bocListFakeTableHead th").Select (s => s[DiagnosticMetadataAttributes.ItemID]).ToList(),
          Context.Configuration.SearchTimeout,
          Context.Configuration.RetryInterval);
      _headerLabels = RetryUntilTimeout.Run (
          () => Scope.FindAllCss (".bocListFakeTableHead th").Select (s => s.Text).ToList(),
          Context.Configuration.SearchTimeout,
          Context.Configuration.RetryInterval);
    }

    public IReadOnlyCollection<string> GetHeaderLabels ()
    {
      return _headerLabels;
    }

    public int GetRowCount ()
    {
      return RetryUntilTimeout.Run (
          () => Scope.FindAllCss (".bocListTable .bocListTableBody tr").Count(),
          Context.Configuration.SearchTimeout,
          Context.Configuration.RetryInterval);
    }

    public int GetCurrentPage ()
    {
      var currentPageTextInputScope = GetCurrentPageTextInputScope();
      return int.Parse (currentPageTextInputScope.Value);
    }

    public int GetNumberOfPages ()
    {
      var navigatorDivScope = Scope.FindCss (".bocListNavigator"); // Todo RM-6297: render directly on bocList root element?
      return int.Parse (navigatorDivScope[DiagnosticMetadataAttributesForObjectBinding.BocListNumberOfPages]);
    }

    public void GoToSpecificPage (int page)
    {
      var currentPageTextInputScope = GetCurrentPageTextInputScope();
      Action<ElementScope> fillAction = s =>
      {
        s.SendKeys (Keys.Backspace + page); // JS prevents clearing, so FillWith is not possible
        Then.PressEnter (s);
      };
      currentPageTextInputScope.PerformAction (fillAction, Context, Behavior.WaitFor (WaitFor.WxePostBack));
    }

    public void GoToFirstPage ()
    {
      var firstPageLinkScope = FindChild ("Navigation_First");
      firstPageLinkScope.ClickAndWait (Context, Behavior.WaitFor (WaitFor.WxePostBack));
    }

    public void GoToPreviousPage ()
    {
      var previousPageLinkScope = FindChild ("Navigation_Previous");
      previousPageLinkScope.ClickAndWait (Context, Behavior.WaitFor (WaitFor.WxePostBack));
    }

    public void GoToNextPage ()
    {
      var nextPageLinkScope = FindChild ("Navigation_Next");
      nextPageLinkScope.ClickAndWait (Context, Behavior.WaitFor (WaitFor.WxePostBack));
    }

    public void GoToLastPage ()
    {
      var lastPageLinkScope = FindChild ("Navigation_Last");
      lastPageLinkScope.ClickAndWait (Context, Behavior.WaitFor (WaitFor.WxePostBack));
    }

    public BocListRowControlObject GetRow (string itemID)
    {
      var cssSelector = string.Format (
          ".bocListTable .bocListTableBody .bocListDataRow[{0}='{1}']",
          DiagnosticMetadataAttributes.ItemID,
          itemID);
      return GetRowByCssSelector (cssSelector);
    }

    public BocListRowControlObject GetRow (int index)
    {
      var cssSelector = string.Format (
          ".bocListTable .bocListTableBody .bocListDataRow[{0}='{1}']",
          DiagnosticMetadataAttributesForObjectBinding.BocListRowIndex,
          index);
      return GetRowByCssSelector (cssSelector);
    }

    public BocListRowControlObject GetRowByHtmlID (string htmlID)
    {
      throw new NotSupportedException ("BocList rows cannot be selected using a full HTML ID.");
    }

    private BocListRowControlObject GetRowByCssSelector (string cssSelector)
    {
      var rowScope = Scope.FindCss (cssSelector);
      return new BocListRowControlObject (this, ID, Context.CloneForScope (rowScope));
    }

    // Todo RM-6297: In general: rename headerItemID to columnItemID?
    public BocListRowControlObject GetRowWhere (string headerItemID, string containsCellText)
    {
      var cell = GetCellWhere (headerItemID, containsCellText);
      return GetRowFromCell (cell);
    }

    public BocListRowControlObject GetRowWhere (int headerIndex, string containsCellText)
    {
      var cell = GetCellWhere (headerIndex, containsCellText);
      return GetRowFromCell (cell);
    }

    // Todo RM-6297: In general: rename headerText to columnHeaderText?
    public BocListRowControlObject GetRowWhereByText (string headerText, string containsCellText)
    {
      var cell = GetCellWhereByText (headerText, containsCellText);
      return GetRowFromCell (cell);
    }

    private BocListRowControlObject GetRowFromCell (BocListCellControlObject cell)
    {
      var rowScope = cell.Scope.FindXPath ("..");
      return new BocListRowControlObject (this, ID, Context.CloneForScope (rowScope));
    }

    public BocListCellControlObject GetCellWhere (string headerItemID, string containsCellText)
    {
      var index = GetHeaderLabelIndex (headerItemID);
      return GetCellWhere (index, containsCellText);
    }

    public BocListCellControlObject GetCellWhere (int index, string containsCellText)
    {
      var cssSelector = string.Format (
          ".bocListTable .bocListTableBody .bocListDataRow .bocListDataCell[{0}='{1}'] span[{2}*='{3}']",
          DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex,
          index,
          DiagnosticMetadataAttributesForObjectBinding.BocListCellContents,
          containsCellText);
      var cellScope = Scope.FindCss (cssSelector).FindXPath ("../..");
      return new BocListCellControlObject (ID, Context.CloneForScope (cellScope));
    }

    public BocListCellControlObject GetCellWhereByText (string headerText, string containsCellText)
    {
      var index = GetHeaderLabelIndexByText (headerText);
      return GetCellWhere (index, containsCellText);
    }

    public void ClickOnSortColumn (string headerLabel)
    {
      var index = GetHeaderLabelIndexByText (headerLabel);

      var sortColumnScope = Scope.FindDMA (
          ".bocListFakeTableHead th",
          DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex,
          index.ToString());

      var sortColumnLinkScope = sortColumnScope.FindLink();

      sortColumnLinkScope.ClickAndWait (Context, Behavior.WaitFor (WaitFor.WxePostBack));
    }

    public void ChangeViewTo (string itemID)
    {
      // Todo RM-6297: think about implementation...
    }

    public void ChangeViewToByLabel (string label)
    {
      var availableViewsScope = FindChild ("Boc_AvailableViewsList");
      availableViewsScope.PerformAction (scope => scope.SelectOption (label), Context, Behavior.WaitFor (WaitFor.WxePostBack));
    }

    public DropDownMenuControlObject GetDropDownMenu ()
    {
      var dropDownMenuScope = FindChild ("Boc_OptionsMenu");
      return new DropDownMenuControlObject (dropDownMenuScope.Id, Context.CloneForScope (dropDownMenuScope));
    }

    public ListMenuControlObject GetListMenu ()
    {
      var listMenuScope = FindChild ("Boc_ListMenu");
      return new ListMenuControlObject (listMenuScope.Id, Context.CloneForScope (listMenuScope));
    }

    internal int GetHeaderLabelIndex (string headerItemID)
    {
      var indexOf = _headerItemIDs.IndexOf (headerItemID);
      if (indexOf == -1)
      {
        throw new ArgumentOutOfRangeException ("headerItemID", headerItemID, "Header item ID does not exist.");
        // Todo RM-6297: Better exception type.
      }

      return indexOf + 1;
    }

    private int GetHeaderLabelIndexByText (string headerText)
    {
      var indexOf = _headerLabels.IndexOf (headerText);
      if (indexOf == -1)
        throw new ArgumentOutOfRangeException ("headerText", headerText, "Header text does not exist."); // Todo RM-6297: Better exception type.

      return indexOf + 1;
    }

    private ElementScope GetCurrentPageTextInputScope ()
    {
      return Scope.FindIdEndingWith ("Boc_CurrentPage_TextBox");
    }
  }


  /// <summary>
  /// Control object representing a row within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/>.
  /// </summary>
  public class BocListRowControlObject : BocControlObject
  {
    private readonly BocListControlObject _bocList;
    private readonly int _rowIndex;

    public BocListRowControlObject (BocListControlObject bocList, [NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
      _bocList = bocList;
      _rowIndex = int.Parse (Scope[DiagnosticMetadataAttributesForObjectBinding.BocListRowIndex]);
    }

    public void ClickSelectCheckbox ()
    {
      var zeroBasedRowIndex = _rowIndex - 1;
      var rowSelectorCheckboxScope = FindChild (string.Format ("RowSelector_{0}", zeroBasedRowIndex));
      rowSelectorCheckboxScope.Click();
    }

    public BocListEditableRowControlObject Edit ()
    {
      var editCommandScope = Scope.FindDMA ("td", DiagnosticMetadataAttributesForObjectBinding.BocListWellKnownEditCell, "true");
      var editCommandLinkScope = editCommandScope.FindLink();
      editCommandLinkScope.ClickAndWait (Context, Behavior.WaitFor (WaitFor.WxePostBack));

      return new BocListEditableRowControlObject (_bocList, ID, Context);
    }

    public BocListCellControlObject GetCell (string headerItemID)
    {
      var index = _bocList.GetHeaderLabelIndex (headerItemID);
      return GetCell (index);
    }

    public BocListCellControlObject GetCell (int index)
    {
      var cellScope = Scope.FindDMA ("td", DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex, index.ToString());
      return new BocListCellControlObject (ID, Context.CloneForScope (cellScope));
    }

    public BocListCellControlObject GetCellByHtmlID (string htmlID)
    {
      throw new NotSupportedException ("BocList cells cannot be selected using a full HTML ID.");
    }

    public DropDownMenuControlObject GetRowDropDownMenu ()
    {
      var cellScope = Scope.FindDMA ("td", DiagnosticMetadataAttributesForObjectBinding.BocListWellKnownRowDropDownMenuCell, "true");
      var rowDropDownMenuScope = cellScope.FindCss ("span.DropDownMenuContainer");
      return new DropDownMenuControlObject (rowDropDownMenuScope.Id, Context.CloneForScope (rowDropDownMenuScope));
    }
  }

  /// <summary>
  /// Control object representing a row in edit-mode within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/>.
  /// </summary>
  public class BocListEditableRowControlObject : BocControlObject
  {
    private readonly BocListControlObject _bocList;

    public BocListEditableRowControlObject (BocListControlObject bocList, [NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
      _bocList = bocList;
    }

    public BocListRowControlObject Save ()
    {
      var editCell = GetWellKnownEditCell();

      var save = editCell.GetControl (new PerIndexControlSelectionCommand<CommandControlObject> (new CommandSelector(), 1));
      save.Click();

      return new BocListRowControlObject (_bocList, ID, Context);
    }

    public BocListRowControlObject Cancel ()
    {
      var editCell = GetWellKnownEditCell();

      var cancel = editCell.GetControl (new PerIndexControlSelectionCommand<CommandControlObject> (new CommandSelector(), 2));
      cancel.Click();

      return new BocListRowControlObject (_bocList, ID, Context);
    }

    public BocListEditableCellControlObject GetCell (string headerItemID)
    {
      var index = _bocList.GetHeaderLabelIndex (headerItemID);
      return GetCell (index);
    }

    public BocListEditableCellControlObject GetCell (int index)
    {
      var cellScope = Scope.FindDMA ("td", DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex, index.ToString());
      return new BocListEditableCellControlObject (ID, Context.CloneForScope (cellScope));
    }

    public BocListEditableCellControlObject GetCellByHtmlID (string htmlID)
    {
      throw new NotSupportedException ("BocList cells cannot be selected using a full HTML ID.");
    }

    private BocListEditableCellControlObject GetWellKnownEditCell ()
    {
      var editCellScope = Scope.FindDMA ("td", DiagnosticMetadataAttributesForObjectBinding.BocListWellKnownEditCell, "true");
      return new BocListEditableCellControlObject (ID, Context.CloneForScope (editCellScope));
    }
  }

  /// <summary>
  /// Control object representing a cell within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/>.
  /// </summary>
  public class BocListCellControlObject : BocControlObject
  {
    public BocListCellControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    /// <summary>
    /// Returns the text content of the cell.
    /// </summary>
    public string GetText ()
    {
      return Scope.Text.Trim();
    }

    /// <summary>
    /// Performs the cell's command.
    /// </summary>
    public UnspecifiedPageObject PerformCommand (IActionBehavior actionBehavior = null)
    {
      var actualWaitingStrategy = GetActualActionBehavior (actionBehavior);

      var commandScope = Scope.FindLink();
      commandScope.ClickAndWait (Context, actualWaitingStrategy);
      return UnspecifiedPage();
    }
  }

  /// <summary>
  /// Control object representing a cell in edit-mode within a <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/>.
  /// </summary>
  public class BocListEditableCellControlObject : BocControlObject, IControlHost
  {
    public BocListEditableCellControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    public TControlObject GetControl<TControlObject> (IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      return controlSelectionCommand.Select (Context);
    }
  }
}