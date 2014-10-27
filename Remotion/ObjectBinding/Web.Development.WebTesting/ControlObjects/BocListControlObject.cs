using System;
using System.Collections.Generic;
using System.Linq;
using Coypu;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.ObjectBinding.Web.Contract.DiagnosticMetadata;
using Remotion.Utilities;
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

    private class BocListRowControlObjectHostAccessor : IBocListRowControlObjectHostAccessor
    {
      private readonly BocListControlObject _bocList;

      public BocListRowControlObjectHostAccessor (BocListControlObject bocList)
      {
        _bocList = bocList;
      }

      public int GetColumnIndex (string columnItemID)
      {
        return _bocList.GetColumnIndex (columnItemID);
      }
    }

    private readonly IBocListRowControlObjectHostAccessor _accessor;
    private readonly List<ColumnDefinition> _columns;

    public BocListControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
      _accessor = new BocListRowControlObjectHostAccessor (this);
      _columns = RetryUntilTimeout.Run (
          () => Scope.FindAllCss (".bocListFakeTableHead th")
              .Select (s => new ColumnDefinition (s[DiagnosticMetadataAttributes.ItemID], s[DiagnosticMetadataAttributes.Text]))
              .ToList());
    }

    public IReadOnlyCollection<string> GetColumnTitles ()
    {
      return _columns.Select (cd => cd.Title).ToList();
    }

    public int GetRowCount ()
    {
      return RetryUntilTimeout.Run (() => Scope.FindAllCss (".bocListTable .bocListTableBody tr").Count());
    }

    public int GetCurrentPage ()
    {
      var currentPageTextInputScope = GetCurrentPageTextInputScope();
      return int.Parse (currentPageTextInputScope.Value);
    }

    public int GetNumberOfPages ()
    {
      var navigatorDivScope = Scope.FindCss (".bocListNavigator");
      return int.Parse (navigatorDivScope[DiagnosticMetadataAttributesForObjectBinding.BocListNumberOfPages]);
    }

    public void GoToSpecificPage (int page)
    {
      var currentPageTextInputScope = GetCurrentPageTextInputScope();
      currentPageTextInputScope.FillWithAndWait (Keys.Backspace + page, Then.TabAway, Context, Behavior.WaitFor (WaitFor.WxePostBack));
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

    public BocListRowControlObject GetRow ([NotNull] string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

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

    [Obsolete ("BocList rows cannot be selected using a full HTML ID.", true)]
    public BocListRowControlObject GetRowByHtmlID ([NotNull] string htmlID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      // Method declaration exists for symmetry reasons only.

      throw new NotSupportedException ("BocList rows cannot be selected using a full HTML ID.");
    }

    private BocListRowControlObject GetRowByCssSelector (string cssSelector)
    {
      var rowScope = Scope.FindCss (cssSelector);
      return new BocListRowControlObject (_accessor, ID, Context.CloneForScope (rowScope));
    }

    public BocListRowControlObject GetRowWhere ([NotNull] string columnItemID, [NotNull] string containsCellText)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnItemID", columnItemID);
      ArgumentUtility.CheckNotNull ("containsCellText", containsCellText);

      var cell = GetCellWhere (columnItemID, containsCellText);
      return GetRowFromCell (cell);
    }

    public BocListRowControlObject GetRowWhere (int columnIndex, [NotNull] string containsCellText)
    {
      ArgumentUtility.CheckNotNull ("containsCellText", containsCellText);

      var cell = GetCellWhere (columnIndex, containsCellText);
      return GetRowFromCell (cell);
    }

    public BocListRowControlObject GetRowWhereByColumnTitle ([NotNull] string columnTitle, [NotNull] string containsCellText)
    {
      ArgumentUtility.CheckNotNull ("columnTitle", columnTitle);
      ArgumentUtility.CheckNotNull ("containsCellText", containsCellText);

      var cell = GetCellWhereByColumnTitle (columnTitle, containsCellText);
      return GetRowFromCell (cell);
    }

    private BocListRowControlObject GetRowFromCell (BocListCellControlObject cell)
    {
      var rowScope = cell.Scope.FindXPath ("..");
      return new BocListRowControlObject (_accessor, ID, Context.CloneForScope (rowScope));
    }

    public BocListCellControlObject GetCellWhere ([NotNull] string columnItemID, [NotNull] string containsCellText)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnItemID", columnItemID);
      ArgumentUtility.CheckNotNull ("containsCellText", containsCellText);

      var index = GetColumnIndex (columnItemID);
      return GetCellWhere (index, containsCellText);
    }

    public BocListCellControlObject GetCellWhere (int columnIndex, [NotNull] string containsCellText)
    {
      ArgumentUtility.CheckNotNull ("containsCellText", containsCellText);

      var cssSelector = string.Format (
          ".bocListTable .bocListTableBody .bocListDataRow .bocListDataCell[{0}='{1}'] span[{2}*='{3}']",
          DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex,
          columnIndex,
          DiagnosticMetadataAttributesForObjectBinding.BocListCellContents,
          containsCellText);
      var cellScope = Scope.FindCss (cssSelector).FindXPath ("../..");
      return new BocListCellControlObject (ID, Context.CloneForScope (cellScope));
    }

    public BocListCellControlObject GetCellWhereByColumnTitle ([NotNull] string columnTitle, [NotNull] string containsCellText)
    {
      ArgumentUtility.CheckNotNull ("columnTitle", columnTitle);
      ArgumentUtility.CheckNotNull ("containsCellText", containsCellText);

      var index = GetColumnIndexByTitle (columnTitle);
      return GetCellWhere (index, containsCellText);
    }

    public void ClickOnSortColumn ([NotNull] string columnItemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnItemID", columnItemID);

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

    public void ClickOnSortColumnByTitle ([NotNull] string columnTitle)
    {
      ArgumentUtility.CheckNotNull ("columnTitle", columnTitle);

      var index = GetColumnIndexByTitle (columnTitle);
      ClickOnSortColumn (index);
    }

    public void ChangeViewTo ([NotNull] string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      ChangeViewTo (scope => scope.SelectOptionByDMA (DiagnosticMetadataAttributes.ItemID, itemID));
    }

    public void ChangeViewTo (int index)
    {
      ChangeViewTo (scope => scope.SelectOptionByIndex (index));
    }

    public void ChangeViewToByLabel ([NotNull] string label)
    {
      ArgumentUtility.CheckNotNull ("label", label);

      ChangeViewTo (scope => scope.SelectOption (label));
    }

    private void ChangeViewTo ([NotNull] Action<ElementScope> selectAction)
    {
      ArgumentUtility.CheckNotNull ("selectAction", selectAction);

      var availableViewsScope = GetAvailableViewsScope();
      availableViewsScope.PerformAction (selectAction, Context, Behavior.WaitFor (WaitFor.WxePostBack));
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

    private int GetColumnIndex (string columnItemID)
    {
      var indexOf = _columns.IndexOf (cd => cd.ItemID == columnItemID);
      if (indexOf == -1)
        throw new KeyNotFoundException (string.Format ("Column item ID '{0}' does not exist.", columnItemID));

      return indexOf + 1;
    }

    private int GetColumnIndexByTitle (string columnTitle)
    {
      var indexOf = _columns.IndexOf (cd => cd.Title == columnTitle);
      if (indexOf == -1)
        throw new KeyNotFoundException (string.Format ("Column title '{0}' does not exist.", columnTitle));

      return indexOf + 1;
    }

    private ElementScope GetCurrentPageTextInputScope ()
    {
      return Scope.FindIdEndingWith ("Boc_CurrentPage_TextBox");
    }
  }
}