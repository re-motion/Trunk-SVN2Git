using System;
using System.Collections.Generic;
using System.Linq;
using Coypu;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WaitingStrategies;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="BocList"/>.
  /// </summary>
  public class BocListControlObject : BocControlObject
  {
    private readonly List<string> _headerLabels;

    public BocListControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
      _headerLabels =
          new RetryUntilTimeout<List<string>> (
              () => Scope.FindAllCss (".bocListFakeTableHead th").Select (s => s.Text).ToList(),
              Context.Configuration.SearchTimeout,
              Context.Configuration.RetryInterval).Run();
    }

    public IReadOnlyCollection<string> GetHeaderLabels ()
    {
      return _headerLabels;
    }

    public int GetRowCount ()
    {
      return new RetryUntilTimeout<int> (
          () => Scope.FindAllCss (".bocListTable .bocListTableBody tr").Count(),
          Context.Configuration.SearchTimeout,
          Context.Configuration.RetryInterval).Run();
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
      currentPageTextInputScope.PerformActionUsingWaitStrategy (fillAction, Context, WaitFor.WxePostBack);
    }

    public void GoToFirstPage ()
    {
      var firstPageLinkScope = FindChild ("Navigation_First");
      firstPageLinkScope.ClickAndWait (Context, WaitFor.WxePostBack);
    }

    public void GoToPreviousPage ()
    {
      var previousPageLinkScope = FindChild ("Navigation_Previous");
      previousPageLinkScope.ClickAndWait (Context, WaitFor.WxePostBack);
    }

    public void GoToNextPage ()
    {
      var nextPageLinkScope = FindChild ("Navigation_Next");
      nextPageLinkScope.ClickAndWait (Context, WaitFor.WxePostBack);
    }

    public void GoToLastPage ()
    {
      var lastPageLinkScope = FindChild ("Navigation_Last");
      lastPageLinkScope.ClickAndWait (Context, WaitFor.WxePostBack);
    }

    public BocListRowControlObject GetRow (string itemID)
    {
      var cssSelector = string.Format (
          ".bocListTable .bocListTableBody .bocListDataRow[{0}='{1}']",
          DiagnosticMetadataAttributes.ItemID,
          itemID);
      var rowScope = Scope.FindCss (cssSelector);
      rowScope.Now(); // Todo RM-6297: Change CloneForScope to ensure .Now()?
      return new BocListRowControlObject (ID, Context.CloneForScope (rowScope));
    }

    public BocListRowControlObject GetRow (int index)
    {
      var cssSelector = string.Format (
          ".bocListTable .bocListTableBody .bocListDataRow[{0}='{1}']",
          DiagnosticMetadataAttributesForObjectBinding.BocListRowIndex,
          index);
      var rowScope = Scope.FindCss (cssSelector);
      rowScope.Now(); // Todo RM-6297: Change CloneForScope to ensure .Now()?
      return new BocListRowControlObject (ID, Context.CloneForScope (rowScope));
    }

    public BocListRowControlObject GetRowByHtmlID (string htmlID)
    {
      throw new NotSupportedException ("BocList rows cannot be selected using the full HTML ID.");
    }

    public BocListRowControlObject GetRowWhere (string headerLabel, string containsText)
    {
      var cell = GetCellWhere (headerLabel, containsText);
      var rowScope = cell.Scope.FindXPath ("..");
      rowScope.Now(); // Todo RM-6297: Change CloneForScope to ensure .Now()?
      return new BocListRowControlObject (ID, Context.CloneForScope (rowScope));
    }

    public BocListCellControlObject GetCellWhere (string headerLabel, string containsText)
    {
      var index = GetHeaderLabelIndex (headerLabel);
      var cssSelector = string.Format (
          ".bocListTable .bocListTableBody .bocListDataRow .bocListDataCell[{0}='{1}'] span[{2}*='{3}']",
          DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex,
          index,
          DiagnosticMetadataAttributesForObjectBinding.BocListCellContents,
          containsText);
      var cellScope = Scope.FindCss (cssSelector).FindXPath ("../..");
      cellScope.Now(); // Todo RM-6297: Change CloneForScope to ensure .Now()?
      return new BocListCellControlObject (ID, Context.CloneForScope (cellScope));
    }

    public void ClickOnSortColumn (string headerLabel)
    {
      var index = GetHeaderLabelIndex (headerLabel);

      var cssSelector = string.Format (".bocListFakeTableHead th[{0}='{1}'] a", DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex, index);
      var sortColumnLinkScope = Scope.FindCss (cssSelector);

      sortColumnLinkScope.ClickAndWait (Context, WaitFor.WxePostBack);
    }

    public void ChangeViewTo (string itemID)
    {
      // Todo RM-6297: think about implementation...
    }

    public void ChangeViewToByLabel (string label)
    {
      var availableViewsScope = FindChild ("Boc_AvailableViewsList");
      availableViewsScope.PerformActionUsingWaitStrategy (scope => scope.SelectOption (label), Context, WaitFor.WxePostBack);
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

    private int GetHeaderLabelIndex (string headerLabel)
    {
      return _headerLabels.IndexOf (headerLabel) + 1;
    }

    private ElementScope GetCurrentPageTextInputScope ()
    {
      return Scope.FindIdEndingWith ("Boc_CurrentPage_TextBox");
    }
  }


  /// <summary>
  /// Control object representing a row within a <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocList"/>.
  /// </summary>
  public class BocListRowControlObject : BocControlObject
  {
    private readonly int _rowIndex;

    public BocListRowControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
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
      var editCommandScope =
          Scope.FindCss (string.Format ("td[{0}='{1}'] a", DiagnosticMetadataAttributesForObjectBinding.BocListWellKnownEditCell, "true"));
      editCommandScope.ClickAndWait (Context, WaitFor.WxePostBack);

      return new BocListEditableRowControlObject (ID, Context);
    }

    public BocListCellControlObject GetCell (int index)
    {
      var cssSelector = string.Format ("td[{0}='{1}']", DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex, index);
      var cellScope = Scope.FindCss (cssSelector);
      cellScope.Now(); // Todo RM-6297: Change CloneForScope to ensure .Now()?

      return new BocListCellControlObject (ID, Context.CloneForScope (cellScope));
    }

    public DropDownMenuControlObject GetRowDropDownMenu ()
    {
      var cssSelector = string.Format (
          "td[{0}='{1}'] span.DropDownMenuContainer",
          DiagnosticMetadataAttributesForObjectBinding.BocListWellKnownRowDropDownMenuCell,
          "true");
      var rowDropDownMenuScope = Scope.FindCss (cssSelector);
      return new DropDownMenuControlObject (rowDropDownMenuScope.Id, Context.CloneForScope (rowDropDownMenuScope));
    }
  }

  /// <summary>
  /// Control object representing a row in edit-mode within a <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocList"/>.
  /// </summary>
  public class BocListEditableRowControlObject : BocControlObject
  {
    public BocListEditableRowControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    public BocListRowControlObject Save ()
    {
      var editCell = GetWellKnownEditCell();

      var save = editCell.GetControl (new PerIndexControlSelectionCommand<CommandControlObject> (new CommandSelector(), 1));
      save.Click();

      return new BocListRowControlObject (ID, Context);
    }

    public BocListRowControlObject Cancel ()
    {
      var editCell = GetWellKnownEditCell();

      var cancel = editCell.GetControl (new PerIndexControlSelectionCommand<CommandControlObject> (new CommandSelector(), 2));
      cancel.Click();

      return new BocListRowControlObject (ID, Context);
    }

    public BocListEditableCellControlObject GetCell (int index)
    {
      var cssSelector = string.Format ("td[{0}='{1}']", DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex, index);
      var cellScope = Scope.FindCss (cssSelector);
      cellScope.Now(); // Todo RM-6297: Change CloneForScope to ensure .Now()?

      return new BocListEditableCellControlObject (ID, Context.CloneForScope (cellScope));
    }

    private BocListEditableCellControlObject GetWellKnownEditCell ()
    {
      var editCellScope =
          Scope.FindCss (string.Format ("td[{0}='{1}']", DiagnosticMetadataAttributesForObjectBinding.BocListWellKnownEditCell, "true"));
      editCellScope.Now(); // Todo RM-6297: Change CloneForScope to ensure .Now()?

      return new BocListEditableCellControlObject (ID, Context.CloneForScope (editCellScope));
    }
  }

  /// <summary>
  /// Control object representing a cell within a <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocList"/>.
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
    public UnspecifiedPageObject PerformCommand (IWaitingStrategy waitingStrategy = null)
    {
      var actualWaitingStrategy = GetActualWaitingStrategy (waitingStrategy);

      var commandScope = Scope.FindCss ("a");
      commandScope.ClickAndWait (Context, actualWaitingStrategy);
      return UnspecifiedPage();
    }
  }

  /// <summary>
  /// Control object representing a cell in edit-mode within a <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocList"/>.
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