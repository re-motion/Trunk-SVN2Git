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
    private class ColumnDefinition
    {
      private readonly string _itemID;
      private readonly string _title;

      public ColumnDefinition (string itemID, string title)
      {
        _itemID = itemID;
        _title = title;
      }

      public string ItemID
      {
        get { return _itemID; }
      }

      public string Title
      {
        get { return _title; }
      }
    }

    private readonly List<ColumnDefinition> _columns;

    public BocListControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
      _columns = RetryUntilTimeout.Run (
          () => Scope.FindAllCss (".bocListFakeTableHead th")
              .Select (s => new ColumnDefinition (s[DiagnosticMetadataAttributes.ItemID], s[DiagnosticMetadataAttributes.Text]))
              .ToList(),
          Context.Configuration.SearchTimeout,
          Context.Configuration.RetryInterval);
    }

    public IReadOnlyCollection<string> GetColumnTitles ()
    {
      return _columns.Select (cd => cd.Title).ToList();
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

    public BocListRowControlObject GetRowWhere (string columnItemID, string containsCellText)
    {
      var cell = GetCellWhere (columnItemID, containsCellText);
      return GetRowFromCell (cell);
    }

    public BocListRowControlObject GetRowWhere (int columnIndex, string containsCellText)
    {
      var cell = GetCellWhere (columnIndex, containsCellText);
      return GetRowFromCell (cell);
    }

    public BocListRowControlObject GetRowWhereByColumnTitle (string columnTitle, string containsCellText)
    {
      var cell = GetCellWhereByColumnTitle (columnTitle, containsCellText);
      return GetRowFromCell (cell);
    }

    private BocListRowControlObject GetRowFromCell (BocListCellControlObject cell)
    {
      var rowScope = cell.Scope.FindXPath ("..");
      return new BocListRowControlObject (this, ID, Context.CloneForScope (rowScope));
    }

    public BocListCellControlObject GetCellWhere (string columnItemID, string containsCellText)
    {
      var index = GetColumnIndex (columnItemID);
      return GetCellWhere (index, containsCellText);
    }

    public BocListCellControlObject GetCellWhere (int columnIndex, string containsCellText)
    {
      var cssSelector = string.Format (
          ".bocListTable .bocListTableBody .bocListDataRow .bocListDataCell[{0}='{1}'] span[{2}*='{3}']",
          DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex,
          columnIndex,
          DiagnosticMetadataAttributesForObjectBinding.BocListCellContents,
          containsCellText);
      var cellScope = Scope.FindCss (cssSelector).FindXPath ("../..");
      return new BocListCellControlObject (ID, Context.CloneForScope (cellScope));
    }

    public BocListCellControlObject GetCellWhereByColumnTitle (string columnTitle, string containsCellText)
    {
      var index = GetColumnIndexByTitle (columnTitle);
      return GetCellWhere (index, containsCellText);
    }

    public void ClickOnSortColumn (string columnItemID)
    {
      var index = GetColumnIndex (columnItemID);
      ClickOnSortColumn (index);
    }

    public void ClickOnSortColumn (int columnIndex)
    {
      var sortColumnScope = Scope.FindDMA (
          ".bocListFakeTableHead th",
          DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex,
          columnIndex.ToString());

      var sortColumnLinkScope = sortColumnScope.FindLink();
      sortColumnLinkScope.ClickAndWait (Context, Behavior.WaitFor (WaitFor.WxePostBack));
    }

    public void ClickOnSortColumnByTitle (string columnTitle)
    {
      var index = GetColumnIndexByTitle (columnTitle);
      ClickOnSortColumn (index);
    }

    public void ChangeViewTo (string itemID)
    {
      // Todo RM-6297: think about implementation...extend Coypu? ItemID rendering?
    }

    public void ChangeViewTo (int index)
    {
      // Todo RM-6297: think about implementation...extend Coypu?
      //var availableViewsScope = GetAvailableViewsScope();
      //availableViewsScope.PerformAction (scope => scope.SelectOption (index), Context, Behavior.WaitFor (WaitFor.WxePostBack));
    }

    public void ChangeViewToByLabel (string label)
    {
      var availableViewsScope = GetAvailableViewsScope();
      availableViewsScope.PerformAction (scope => scope.SelectOption (label), Context, Behavior.WaitFor (WaitFor.WxePostBack));
    }

    private ElementScope GetAvailableViewsScope ()
    {
      return FindChild ("Boc_AvailableViewsList");
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

    internal int GetColumnIndex (string columnItemID)
    {
      var indexOf = _columns.IndexOf (cd => cd.ItemID == columnItemID);
      if (indexOf == -1)
        throw new ArgumentOutOfRangeException ("columnItemID", columnItemID, "Column item ID does not exist."); // Todo RM-6297: Exception type.

      return indexOf + 1;
    }

    private int GetColumnIndexByTitle (string columnTitle)
    {
      var indexOf = _columns.IndexOf (cd => cd.Title == columnTitle);
      if (indexOf == -1)
        throw new ArgumentOutOfRangeException ("columnTitle", columnTitle, "Colum title does not exist."); // Todo RM-6297: Better exception type.

      return indexOf + 1;
    }

    private ElementScope GetCurrentPageTextInputScope ()
    {
      return Scope.FindIdEndingWith ("Boc_CurrentPage_TextBox");
    }
  }
}