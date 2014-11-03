using System;
using Coypu;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.ObjectBinding.Web.Contract.DiagnosticMetadata;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/>.
  /// </summary>
  [UsedImplicitly]
  public class BocListControlObject
      : BocListControlObjectBase<BocListRowControlObject, BocListCellControlObject>,
          IControlObjectWithRowsWhereColumnContains<BocListRowControlObject>,
          IControlObjectWithCellsInRowsWhereColumnContains<BocListCellControlObject>
  {
    public BocListControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
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
      currentPageTextInputScope.FillWithAndWait (Keys.Backspace + page, FinishInput.WithTab, Context, Continue.When (Wxe.PostBackCompleted).Build());
    }

    public void GoToFirstPage ()
    {
      var firstPageLinkScope = Scope.FindChild ("Navigation_First");
      firstPageLinkScope.ClickAndWait (Context, Continue.When (Wxe.PostBackCompleted).Build());
    }

    public void GoToPreviousPage ()
    {
      var previousPageLinkScope = Scope.FindChild ("Navigation_Previous");
      previousPageLinkScope.ClickAndWait (Context, Continue.When (Wxe.PostBackCompleted).Build());
    }

    public void GoToNextPage ()
    {
      var nextPageLinkScope = Scope.FindChild ("Navigation_Next");
      nextPageLinkScope.ClickAndWait (Context, Continue.When (Wxe.PostBackCompleted).Build());
    }

    public void GoToLastPage ()
    {
      var lastPageLinkScope = Scope.FindChild ("Navigation_Last");
      lastPageLinkScope.ClickAndWait (Context, Continue.When (Wxe.PostBackCompleted).Build());
    }

    public IControlObjectWithRowsWhereColumnContains<BocListRowControlObject> GetRowWhere ()
    {
      return this;
    }

    public BocListRowControlObject GetRowWhere (string columnItemID, string containsCellText)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnItemID", columnItemID);
      ArgumentUtility.CheckNotNull ("containsCellText", containsCellText);

      return GetRowWhere().ColumnWithItemIDContains (columnItemID, containsCellText);
    }

    BocListRowControlObject IControlObjectWithRowsWhereColumnContains<BocListRowControlObject>.ColumnWithItemIDContains (
        string itemID,
        string containsCellText)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);
      ArgumentUtility.CheckNotNull ("containsCellText", containsCellText);

      var cell = GetCellWhere (itemID, containsCellText);
      return GetRowFromCell (cell);
    }

    BocListRowControlObject IControlObjectWithRowsWhereColumnContains<BocListRowControlObject>.ColumnWithIndexContains (
        int index,
        string containsCellText)
    {
      ArgumentUtility.CheckNotNull ("containsCellText", containsCellText);

      var cell = GetCellWhere().ColumnWithIndexContains (index, containsCellText);
      return GetRowFromCell (cell);
    }

    BocListRowControlObject IControlObjectWithRowsWhereColumnContains<BocListRowControlObject>.ColumnWithTitleContains (
        string title,
        string containsCellText)
    {
      ArgumentUtility.CheckNotNull ("title", title);
      ArgumentUtility.CheckNotNull ("containsCellText", containsCellText);

      var cell = GetCellWhere().ColumnWithTitleContains (title, containsCellText);
      return GetRowFromCell (cell);
    }

    private BocListRowControlObject GetRowFromCell (BocListCellControlObject cell)
    {
      var rowScope = cell.Scope.FindXPath ("..");
      return CreateRowControlObject (GetHtmlID(), rowScope, Accessor);
    }

    public IControlObjectWithCellsInRowsWhereColumnContains<BocListCellControlObject> GetCellWhere ()
    {
      return this;
    }

    public BocListCellControlObject GetCellWhere (string columnItemID, string containsCellText)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnItemID", columnItemID);
      ArgumentUtility.CheckNotNull ("containsCellText", containsCellText);

      return GetCellWhere().ColumnWithItemIDContains (columnItemID, containsCellText);
    }

    BocListCellControlObject IControlObjectWithCellsInRowsWhereColumnContains<BocListCellControlObject>.ColumnWithItemIDContains (
        string itemID,
        string containsCellText)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);
      ArgumentUtility.CheckNotNull ("containsCellText", containsCellText);

      var index = GetColumnIndex (itemID);
      return GetCellWhere().ColumnWithIndexContains (index, containsCellText);
    }

    BocListCellControlObject IControlObjectWithCellsInRowsWhereColumnContains<BocListCellControlObject>.ColumnWithIndexContains (
        int index,
        string containsCellText)
    {
      ArgumentUtility.CheckNotNull ("containsCellText", containsCellText);

      var cssSelector = string.Format (
          ".bocListTable .bocListTableBody .bocListDataRow .bocListDataCell[{0}='{1}'] span[{2}*='{3}']",
          DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex,
          index,
          DiagnosticMetadataAttributesForObjectBinding.BocListCellContents,
          containsCellText);
      var cellScope = Scope.FindCss (cssSelector).FindXPath ("../..");
      return CreateCellControlObject (GetHtmlID(), cellScope);
    }

    BocListCellControlObject IControlObjectWithCellsInRowsWhereColumnContains<BocListCellControlObject>.ColumnWithTitleContains (
        string title,
        string containsCellText)
    {
      ArgumentUtility.CheckNotNull ("title", title);
      ArgumentUtility.CheckNotNull ("containsCellText", containsCellText);

      var index = GetColumnIndexByTitle (title);
      return GetCellWhere().ColumnWithIndexContains (index, containsCellText);
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
      sortColumnLinkScope.ClickAndWait (Context, Continue.When (Wxe.PostBackCompleted).Build());
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

    protected override BocListRowControlObject CreateRowControlObject (
        string id,
        ElementScope rowScope,
        IBocListRowControlObjectHostAccessor accessor)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNull ("rowScope", rowScope);
      ArgumentUtility.CheckNotNull ("accessor", accessor);

      return new BocListRowControlObject (accessor, Context.CloneForControl (rowScope));
    }

    protected override BocListCellControlObject CreateCellControlObject (string id, ElementScope cellScope)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNull ("cellScope", cellScope);

      return new BocListCellControlObject (Context.CloneForControl (cellScope));
    }

    private void ChangeViewTo ([NotNull] Action<ElementScope> selectAction)
    {
      ArgumentUtility.CheckNotNull ("selectAction", selectAction);

      var availableViewsScope = GetAvailableViewsScope();
      availableViewsScope.PerformAction (selectAction, Context, Continue.When (Wxe.PostBackCompleted).Build());
    }

    private ElementScope GetAvailableViewsScope ()
    {
      return Scope.FindChild ("Boc_AvailableViewsList");
    }

    private ElementScope GetCurrentPageTextInputScope ()
    {
      return Scope.FindIdEndingWith ("Boc_CurrentPage_TextBox");
    }
  }
}