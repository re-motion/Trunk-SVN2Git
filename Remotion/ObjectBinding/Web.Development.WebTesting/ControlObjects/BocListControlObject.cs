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
using Coypu;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.ObjectBinding.Web.Contract.DiagnosticMetadata;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocList"/>.
  /// </summary>
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
      currentPageTextInputScope.FillInWithAndWait (Keys.Backspace + page, FinishInput.WithTab, Context, Continue.When (Wxe.PostBackCompleted).Build());
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

      var column = GetColumnByItemID (itemID);
      return GetCellWhereColumnContains (column, containsCellText);
    }

    BocListCellControlObject IControlObjectWithCellsInRowsWhereColumnContains<BocListCellControlObject>.ColumnWithIndexContains (
        int index,
        string containsCellText)
    {
      ArgumentUtility.CheckNotNull ("containsCellText", containsCellText);

      var column = GetColumnByIndex (index);
      return GetCellWhereColumnContains (column, containsCellText);
    }

    BocListCellControlObject IControlObjectWithCellsInRowsWhereColumnContains<BocListCellControlObject>.ColumnWithTitleContains (
        string title,
        string containsCellText)
    {
      ArgumentUtility.CheckNotNull ("title", title);
      ArgumentUtility.CheckNotNull ("containsCellText", containsCellText);

      var column = GetColumnByTitle (title);
      return GetCellWhereColumnContains (column, containsCellText);
    }

    private BocListCellControlObject GetCellWhereColumnContains (ColumnDefinition column, string containsCellText)
    {
      if (column.HasDiagnosticMetadata)
      {
        var cssSelector = string.Format (
            ".bocListTable .bocListTableBody .bocListDataRow .bocListDataCell[{0}='{1}'] span[{2}*='{3}']",
            DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex,
            column.Index,
            DiagnosticMetadataAttributesForObjectBinding.BocListCellContents,
            containsCellText);
        var cellScope = Scope.FindCss (cssSelector).FindXPath ("../..");
        return CreateCellControlObject (GetHtmlID(), cellScope);
      }
      else
      {
        var xPathSelector = string.Format (
            ".//tbody{0}/tr/td[contains(.,'{1}')]",
            XPathUtils.CreateHasClassCheck ("bocListTableBody"),
            containsCellText);
        var cellScope = Scope.FindXPath (xPathSelector);
        return CreateCellControlObject (GetHtmlID(), cellScope);
      }
    }

    public void ClickOnSortColumn ([NotNull] string columnItemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("columnItemID", columnItemID);

      var column = GetColumnByItemID (columnItemID);
      ClickOnSortColumn (column.Index);
    }

    public void ClickOnSortColumn (int columnIndex)
    {
      var sortColumnScope = Scope.FindTagWithAttribute (
          ".bocListFakeTableHead th",
          DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex,
          columnIndex.ToString());

      var sortColumnLinkScope = sortColumnScope.FindLink();
      sortColumnLinkScope.ClickAndWait (Context, Continue.When (Wxe.PostBackCompleted).Build());
    }

    public void ClickOnSortColumnByTitle ([NotNull] string columnTitle)
    {
      ArgumentUtility.CheckNotNull ("columnTitle", columnTitle);

      var column = GetColumnByTitle (columnTitle);
      ClickOnSortColumn (column.Index);
    }

    public void ChangeViewTo ([NotNull] string itemID, [CanBeNull] ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      var actualCompletionDetector = GetActualCompletionDetector (completionDetection);
      ChangeViewTo (scope => scope.SelectOptionByDMA (DiagnosticMetadataAttributes.ItemID, itemID), actualCompletionDetector);
    }

    public void ChangeViewTo (int index, [CanBeNull] ICompletionDetection completionDetection = null)
    {
      var actualCompletionDetector = GetActualCompletionDetector (completionDetection);
      ChangeViewTo (scope => scope.SelectOptionByIndex (index), actualCompletionDetector);
    }

    public void ChangeViewToByLabel ([NotNull] string label, [CanBeNull] ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNull ("label", label);

      var actualCompletionDetector = GetActualCompletionDetector (completionDetection);
      ChangeViewTo (scope => scope.SelectOption (label), actualCompletionDetector);
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

    private void ChangeViewTo ([NotNull] Action<ElementScope> selectAction, [NotNull] ICompletionDetector completionDetector)
    {
      ArgumentUtility.CheckNotNull ("selectAction", selectAction);
      ArgumentUtility.CheckNotNull ("completionDetector", completionDetector);

      var availableViewsScope = GetAvailableViewsScope();
      availableViewsScope.PerformAction (selectAction, Context, completionDetector);
    }

    protected virtual ElementScope GetAvailableViewsScope ()
    {
      return Scope.FindChild ("Boc_AvailableViewsList");
    }

    private ElementScope GetCurrentPageTextInputScope ()
    {
      return Scope.FindIdEndingWith ("Boc_CurrentPage_TextBox");
    }
  }
}