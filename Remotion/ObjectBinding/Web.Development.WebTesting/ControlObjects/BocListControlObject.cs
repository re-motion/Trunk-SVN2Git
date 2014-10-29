using System;
using Coypu;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.ObjectBinding.Web.Contract.DiagnosticMetadata;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/>.
  /// </summary>
  [UsedImplicitly]
  public class BocListControlObject : BocListControlObjectBase<BocListRowControlObject, BocListCellControlObject>
  {
    public BocListControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
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
      return CreateRowControlObject (ID, rowScope, Accessor);
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
      return CreateCellControlObject (ID, cellScope);
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

    protected override BocListRowControlObject CreateRowControlObject (
        string id,
        ElementScope rowScope,
        IBocListRowControlObjectHostAccessor accessor)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNull ("rowScope", rowScope);
      ArgumentUtility.CheckNotNull ("accessor", accessor);

      return new BocListRowControlObject (accessor, ID, Context.CloneForScope (rowScope));
    }

    protected override BocListCellControlObject CreateCellControlObject (string id, ElementScope cellScope)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNull ("cellScope", cellScope);

      return new BocListCellControlObject (ID, Context.CloneForScope (cellScope));
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

    private ElementScope GetCurrentPageTextInputScope ()
    {
      return Scope.FindIdEndingWith ("Boc_CurrentPage_TextBox");
    }
  }
}