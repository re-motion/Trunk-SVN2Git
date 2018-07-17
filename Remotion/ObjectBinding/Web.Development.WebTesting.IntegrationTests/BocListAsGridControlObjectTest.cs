﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Linq;
using Coypu;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.TestCaseFactories;
using Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.ObjectBinding.Web.Development.WebTesting.ScreenshotCreation.BocList;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Fluent;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocListAsGridControlObjectTest : IntegrationTest
  {
    [Test]
    [RemotionTestCaseSource (typeof (DisabledTestCaseFactory<BocListAsGridSelector, BocListAsGridControlObject>))]
    [RemotionTestCaseSource (typeof (ReadOnlyTestCaseFactory<BocListAsGridSelector, BocListAsGridControlObject>))]
    [RemotionTestCaseSource (typeof (LabelTestCaseFactory<BocListAsGridSelector, BocListAsGridControlObject>))]
    [RemotionTestCaseSource (typeof (ValidationErrorTestCaseFactory<BocListAsGridSelector, BocListAsGridControlObject>))]
    public void GenericTests (GenericSelectorTestAction<BocListAsGridSelector, BocListAsGridControlObject> testAction)
    {
      testAction (Helper, e => e.ListAsGrids(), "listAsGrid");
    }

    [Test]
    [RemotionTestCaseSource (typeof (HtmlIDControlSelectorTestCaseFactory<BocListAsGridSelector, BocListAsGridControlObject>))]
    [RemotionTestCaseSource (typeof (IndexControlSelectorTestCaseFactory<BocListAsGridSelector, BocListAsGridControlObject>))]
    [RemotionTestCaseSource (typeof (LocalIDControlSelectorTestCaseFactory<BocListAsGridSelector, BocListAsGridControlObject>))]
    [RemotionTestCaseSource (typeof (FirstControlSelectorTestCaseFactory<BocListAsGridSelector, BocListAsGridControlObject>))]
    [RemotionTestCaseSource (typeof (SingleControlSelectorTestCaseFactory<BocListAsGridSelector, BocListAsGridControlObject>))]
    [RemotionTestCaseSource (typeof (DomainPropertyControlSelectorTestCaseFactory<BocListAsGridSelector, BocListAsGridControlObject>))]
    [RemotionTestCaseSource (typeof (DisplayNameControlSelectorTestCaseFactory<BocListAsGridSelector, BocListAsGridControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<BocListAsGridSelector, BocListAsGridControlObject> testAction)
    {
      testAction (Helper, e => e.ListAsGrids(), "listAsGrid");
    }

    [Category ("Screenshot")]
    [Test]
    public void ScreenshotTest_DerivedType ()
    {
      var home = Start();
      var controlObjectContext = home.ListAsGrids().GetByLocalID ("JobList_Normal").Context;
      var controlObject = new DerivedBocListAsGridControlObject (controlObjectContext);
      var fluentControlObject = controlObject.ForScreenshot();
      var derivedControlObject = SelfResolvableFluentScreenshot.Create (
          new DerivedScreenshotBocList<BocListAsGridControlObject, BocListAsGridRowControlObject, BocListAsGridCellControlObject> (
              fluentControlObject.GetTarget().FluentList, fluentControlObject.GetTarget().FluentElement));

      var fluentTableContainer = derivedControlObject.GetTableContainer();
      Assert.That (fluentTableContainer, Is.Not.Null);
      var derivedTableContainer = SelfResolvableFluentScreenshot.Create (
          new DerivedScreenshotBocListTableContainer<BocListAsGridControlObject, BocListAsGridRowControlObject, BocListAsGridCellControlObject> (
              fluentTableContainer.GetTarget().FluentList, fluentTableContainer.GetTarget().FluentElement));
      Assert.That (derivedTableContainer.GetHeaderRow(), Is.Not.Null);
      Assert.That (derivedTableContainer.GetRowCount(), Is.Not.Null);
      Assert.That (derivedTableContainer.GetColumn(), Is.Not.Null);
      Assert.That (derivedTableContainer.GetColumn (1), Is.Not.Null);
      Assert.That (derivedTableContainer.GetColumn ("RowCmd"), Is.Not.Null);
      Assert.That (derivedTableContainer.GetRow(), Is.Not.Null);
      Assert.That (derivedTableContainer.GetRow (1), Is.Not.Null);
      Assert.That (derivedTableContainer.GetRow ("0ba19f5c-f2a2-4c9f-83c9-e6d25b461d98"), Is.Not.Null);
      //Assert.That (fluentTableContainer.GetCell(), Is.Not.Null);
      //Assert.That (fluentTableContainer.GetCell(0), Is.Not.Null);
      //Assert.That (fluentTableContainer.GetCell(""), Is.Not.Null);

      var fluentNavigator = derivedControlObject.GetNavigator();
      Assert.That (fluentNavigator, Is.Not.Null);
      var derivedNavigator = SelfResolvableFluentScreenshot.Create (
          new DerivedScreenshotBocListNavigator<BocListAsGridControlObject, BocListAsGridRowControlObject, BocListAsGridCellControlObject> (
              fluentNavigator.GetTarget().FluentList, fluentNavigator.GetTarget().FluentElement));
      Assert.That (derivedNavigator.GetFirstPageButton(), Is.Not.Null);
      Assert.That (derivedNavigator.GetLastPageButton(), Is.Not.Null);
      Assert.That (derivedNavigator.GetNextPageButton(), Is.Not.Null);
      Assert.That (derivedNavigator.GetPreviousPageButton(), Is.Not.Null);
      Assert.That (derivedNavigator.GetPageInformationText(), Is.Not.Null);

      var fluentMenuBlock = derivedControlObject.GetMenuBlock();
      Assert.That (fluentMenuBlock, Is.Not.Null);
      var derivedMenuBlock = SelfResolvableFluentScreenshot.Create (
          new DerivedScreenshotBocListMenuBlock<BocListAsGridControlObject, BocListAsGridRowControlObject, BocListAsGridCellControlObject> (
              fluentMenuBlock.GetTarget().FluentList, fluentMenuBlock.GetTarget().FluentElement));
      Assert.That (derivedMenuBlock.GetDropDownMenu(), Is.Not.Null);
      Assert.That (derivedMenuBlock.GetListMenu(), Is.Not.Null);

      var fluentDropDown = derivedMenuBlock.GetViewsMenu();
      Assert.That (fluentDropDown, Is.Not.Null);
      var derivedDropDown = SelfResolvableFluentScreenshot.Create (
          new DerivedScreenshotBocListDropDown<BocListAsGridControlObject, BocListAsGridRowControlObject, BocListAsGridCellControlObject> (
              fluentDropDown.GetTarget().FluentList, fluentDropDown.GetTarget().FluentElement));
      Assert.That (() => derivedDropDown.Open(), Throws.Nothing);
    }

    [Category ("Screenshot")]
    [Test]
    public void ScreenshotTest_DerivedTypeGeneric ()
    {
      var home = Start();
      var controlObjectContext = home.ListAsGrids().GetByLocalID ("JobList_Normal").Context;
      var controlObject = new DerivedBocListAsGridControlObject<DerivedBocListAsGridRowControlObject> (controlObjectContext);
      var fluentControlObject = controlObject.ForBocListAsGridScreenshot<DerivedBocListAsGridControlObject<DerivedBocListAsGridRowControlObject>, DerivedBocListAsGridRowControlObject>();
      var derivedControlObject = SelfResolvableFluentScreenshot.Create (
          new DerivedScreenshotBocList<DerivedBocListAsGridControlObject<DerivedBocListAsGridRowControlObject>, DerivedBocListAsGridRowControlObject, BocListAsGridCellControlObject> (
              fluentControlObject.GetTarget().FluentList, fluentControlObject.GetTarget().FluentElement));

      var fluentTableContainer = derivedControlObject.GetTableContainer();
      Assert.That (fluentTableContainer, Is.Not.Null);
      var derivedTableContainer = SelfResolvableFluentScreenshot.Create (
          new DerivedScreenshotBocListTableContainer<DerivedBocListAsGridControlObject<DerivedBocListAsGridRowControlObject>, DerivedBocListAsGridRowControlObject, BocListAsGridCellControlObject> (
              fluentTableContainer.GetTarget().FluentList, fluentTableContainer.GetTarget().FluentElement));
      Assert.That (derivedTableContainer.GetHeaderRow(), Is.Not.Null);
      Assert.That (derivedTableContainer.GetRowCount(), Is.Not.Null);
      Assert.That (derivedTableContainer.GetColumn(), Is.Not.Null);
      Assert.That (derivedTableContainer.GetColumn (1), Is.Not.Null);
      Assert.That (derivedTableContainer.GetColumn ("RowCmd"), Is.Not.Null);
      Assert.That (derivedTableContainer.GetRow(), Is.Not.Null);
      Assert.That (derivedTableContainer.GetRow (1), Is.Not.Null);
      Assert.That (derivedTableContainer.GetRow ("0ba19f5c-f2a2-4c9f-83c9-e6d25b461d98"), Is.Not.Null);
      //Assert.That (fluentTableContainer.GetCell(), Is.Not.Null);
      //Assert.That (fluentTableContainer.GetCell(0), Is.Not.Null);
      //Assert.That (fluentTableContainer.GetCell(""), Is.Not.Null);

      var fluentNavigator = derivedControlObject.GetNavigator();
      Assert.That (fluentNavigator, Is.Not.Null);
      var derivedNavigator = SelfResolvableFluentScreenshot.Create (
          new DerivedScreenshotBocListNavigator<DerivedBocListAsGridControlObject<DerivedBocListAsGridRowControlObject>, DerivedBocListAsGridRowControlObject, BocListAsGridCellControlObject> (
              fluentNavigator.GetTarget().FluentList, fluentNavigator.GetTarget().FluentElement));
      Assert.That (derivedNavigator.GetFirstPageButton(), Is.Not.Null);
      Assert.That (derivedNavigator.GetLastPageButton(), Is.Not.Null);
      Assert.That (derivedNavigator.GetNextPageButton(), Is.Not.Null);
      Assert.That (derivedNavigator.GetPreviousPageButton(), Is.Not.Null);
      Assert.That (derivedNavigator.GetPageInformationText(), Is.Not.Null);

      var fluentMenuBlock = derivedControlObject.GetMenuBlock();
      Assert.That (fluentMenuBlock, Is.Not.Null);
      var derivedMenuBlock = SelfResolvableFluentScreenshot.Create (
          new DerivedScreenshotBocListMenuBlock<DerivedBocListAsGridControlObject<DerivedBocListAsGridRowControlObject>, DerivedBocListAsGridRowControlObject, BocListAsGridCellControlObject> (
              fluentMenuBlock.GetTarget().FluentList, fluentMenuBlock.GetTarget().FluentElement));
      Assert.That (derivedMenuBlock.GetDropDownMenu(), Is.Not.Null);
      Assert.That (derivedMenuBlock.GetListMenu(), Is.Not.Null);

      var fluentDropDown = derivedMenuBlock.GetViewsMenu();
      Assert.That (fluentDropDown, Is.Not.Null);
      var derivedDropDown = SelfResolvableFluentScreenshot.Create (
          new DerivedScreenshotBocListDropDown<DerivedBocListAsGridControlObject<DerivedBocListAsGridRowControlObject>, DerivedBocListAsGridRowControlObject, BocListAsGridCellControlObject> (
              fluentDropDown.GetTarget().FluentList, fluentDropDown.GetTarget().FluentElement));
      Assert.That (() => derivedDropDown.Open(), Throws.Nothing);
    }

    [Test]
    public void TestGetColumnDefinitions ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID ("JobList_Normal");
      Assert.That (
          bocList.GetColumnDefinitions().Select (cd => cd.Title),
          Is.EquivalentTo (new[] { "I_ndex", null, "Command", "Menu", "Title", "StartDate", "EndDate", "DisplayName", "TitleWithCmd" }));
    }

    [Test]
    public void TestGetDisplayedRows ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID ("JobList_Normal");
      var rows = bocList.GetDisplayedRows();
      Assert.That (rows.Count, Is.EqualTo (8));
      Assert.That (rows[1].GetCell ("DisplayName").GetText(), Is.EqualTo ("CEO"));
    }

    [Test]
    public void TestGetNumberOfRows ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID ("JobList_Normal");
      Assert.That (bocList.GetNumberOfRows(), Is.EqualTo (8));
    }

    [Test]
    public void TestEmptyList ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID ("JobList_Empty");
      Assert.That (bocList.GetNumberOfRows(), Is.EqualTo (0));
      Assert.That (bocList.IsEmpty(), Is.True);
      Assert.That (bocList.GetEmptyMessage(), Is.EqualTo ("A wonderful empty list."));
    }

    [Test]
    public void TestSelectAllAndDeselectAll ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID ("JobList_Normal");

      var firstRow = bocList.GetRow (1);
      var lastRow = bocList.GetRow (bocList.GetNumberOfRows());
      Assert.That (firstRow.IsSelected, Is.False);
      Assert.That (lastRow.IsSelected, Is.False);

      bocList.SelectAll();

      Assert.That (firstRow.IsSelected, Is.True);
      Assert.That (lastRow.IsSelected, Is.True);

      bocList.SelectAll();

      Assert.That (firstRow.IsSelected, Is.True);
      Assert.That (lastRow.IsSelected, Is.True);

      bocList.DeselectAll();

      Assert.That (firstRow.IsSelected, Is.False);
      Assert.That (lastRow.IsSelected, Is.False);
    }

    [Test]
    public void TestGetRow ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID ("JobList_Normal");

      var row = bocList.GetRow ("0ba19f5c-f2a2-4c9f-83c9-e6d25b461d98");
      Assert.That (row.GetCell (8).GetText(), Is.EqualTo ("CEO"));

      row = bocList.GetRow (1);
      Assert.That (row.GetCell (8).GetText(), Is.EqualTo ("Programmer"));
    }

    [Test]
    public void TestGetDropDownMenu ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID ("JobList_Normal");
      var dropDownMenu = bocList.GetDropDownMenu();
      dropDownMenu.SelectItem ("OptCmd2");

      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("JobList_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderRowLabel").Text, Is.EqualTo ("-1"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("ListMenuOrOptionsClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("OptCmd2|Option command 2"));
    }

    [Test]
    public void TestGetListMenu ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID ("JobList_Normal");
      var listMenu = bocList.GetListMenu();
      listMenu.SelectItem ("ListMenuCmd3");

      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("JobList_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderRowLabel").Text, Is.EqualTo ("-1"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("ListMenuOrOptionsClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("ListMenuCmd3|LM cmd 3"));
    }

    [Test]
    public void TestRowGetCell ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID ("JobList_Normal");
      var row = bocList.GetRow (2);

      var cell = row.GetCell ("DisplayName");
      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));

      cell = row.GetCell (8);
      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));

      cell = row.GetCell().WithColumnTitle ("DisplayName");
      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));

      cell = row.GetCell().WithColumnTitleContains ("layNam");
      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));
    }

    [Test]
    public void TestRowSelectAndDeselect ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID ("JobList_Normal");
      var row = bocList.GetRow (2);

      row.Select();
      row.GetCell (3).ExecuteCommand();
      Assert.That (home.Scope.FindIdEndingWith ("SelectedIndicesLabel").Text, Is.EqualTo ("1"));

      row.Select();
      row.GetCell (3).ExecuteCommand();
      Assert.That (home.Scope.FindIdEndingWith ("SelectedIndicesLabel").Text, Is.EqualTo ("1"));

      row.Deselect();
      row.GetCell (3).ExecuteCommand();
      Assert.That (home.Scope.FindIdEndingWith ("SelectedIndicesLabel").Text, Is.EqualTo ("NoneSelected"));
    }

    [Test]
    public void TestRowGetRowDropDownMenu ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID ("JobList_Normal");
      var row = bocList.GetRow (2);
      var dropDownMenu = row.GetDropDownMenu();
      dropDownMenu.SelectItem ("RowMenuItemCmd2");

      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("JobList_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderRowLabel").Text, Is.EqualTo ("1"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("RowContextMenuClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("RowMenuItemCmd2|Row menu 2"));
    }

    [Test]
    public void TestCellGetText ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID ("JobList_Normal");
      var cell = bocList.GetRow (2).GetCell (8);

      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));
    }

    [Test]
    public void TestCellPerformCommand ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID ("JobList_Normal");
      var cell = bocList.GetRow (2).GetCell (3);

      cell.ExecuteCommand();

      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("JobList_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderRowLabel").Text, Is.EqualTo ("1"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("CellCommandClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("RowCmd"));
    }

    [Test]
    public void TestEditableCellGetControl ()
    {
      var home = Start();

      var bocList = home.ListAsGrids().GetByLocalID ("JobList_Normal");
      var editableRow = bocList.GetRow (2);
      var editableCell = editableRow.GetCell (5);

      var bocText = editableCell.TextValues().First();
      bocText.FillWith ("NewTitle");

      Assert.That (bocText.GetText(), Is.EqualTo ("NewTitle"));
    }

    [Test]
    public void TestGetCurrentPage ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");

      Assert.That (bocList.GetCurrentPage(), Is.EqualTo (1));
    }

    [Test]
    public void TestGetCurrentPage_WithoutNavigator ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Empty");
      Assert.That (bocList.GetCurrentPage(), Is.EqualTo (1));
    }

    [Test]
    public void TestGetNumberOfPages ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");

      Assert.That (bocList.GetNumberOfPages(), Is.EqualTo (1));
    }

    [Test]
    public void TestGetNumberOfPages_WithoutNavigator ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Empty");
      Assert.That (bocList.GetNumberOfPages(), Is.EqualTo (1));
    }
    
    [Test]
    public void TestGoToSpecificPage ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");

      Assert.That (
          () => bocList.GoToSpecificPage (1),
          Throws.Exception.TypeOf<MissingHtmlException>().With.Message.EqualTo ("Unable to change current page of the list. List is currently in edit mode."));
    }

    [Test]
    public void TestGoToFirstPage ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");

      Assert.That (
          () => bocList.GoToFirstPage(),
          Throws.Exception.TypeOf<MissingHtmlException>().With.Message.EqualTo ("Unable to change current page of the list. List is currently in edit mode."));
    }

    [Test]
    public void TestGoToPreviousPage ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");

      Assert.That (
          () => bocList.GoToPreviousPage(),
          Throws.Exception.TypeOf<MissingHtmlException>().With.Message.EqualTo ("Unable to change current page of the list. List is currently in edit mode."));
    }

    [Test]
    public void TestGoToNextPage ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");

      Assert.That (
          () => bocList.GoToNextPage(),
          Throws.Exception.TypeOf<MissingHtmlException>().With.Message.EqualTo ("Unable to change current page of the list. List is currently in edit mode."));
    }

    [Test]
    public void TestGoToLastPage ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID ("JobList_Normal");

      Assert.That (
          () => bocList.GoToLastPage(),
          Throws.Exception.TypeOf<MissingHtmlException>().With.Message.EqualTo ("Unable to change current page of the list. List is currently in edit mode."));
    }

    private WxePageObject Start ()
    {
      return Start ("BocListAsGrid");
    }

    private class DerivedBocListAsGridControlObject : BocListAsGridControlObject
    {
      public DerivedBocListAsGridControlObject (ControlObjectContext context)
          : base (context)
      {
      }
    }

    private class DerivedBocListAsGridControlObject<TBocListRowControlObject> : BocListAsGridControlObject<TBocListRowControlObject>
        where TBocListRowControlObject : BocListAsGridRowControlObject
    {
      public DerivedBocListAsGridControlObject (ControlObjectContext context)
          : base (context)
      {
      }
    }

    private class DerivedBocListAsGridRowControlObject : BocListAsGridRowControlObject
    {
      public DerivedBocListAsGridRowControlObject (IBocListRowControlObjectHostAccessor accessor, ControlObjectContext context)
          : base (accessor, context)
      {
      }
    }

    private class DerivedScreenshotBocList<TList, TRow, TCell> : ScreenshotBocList<TList, TRow, TCell>
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      public DerivedScreenshotBocList (
          IFluentScreenshotElementWithCovariance<TList> fluentList,
          IFluentScreenshotElement<ElementScope> fluentElement)
          : base (fluentList, fluentElement)
      {
      }
    }

    private class DerivedScreenshotBocListTableContainer<TList, TRow, TCell> : ScreenshotBocListTableContainer<TList, TRow, TCell>
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      public DerivedScreenshotBocListTableContainer (
          IFluentScreenshotElementWithCovariance<ScreenshotBocList<TList, TRow, TCell>> fluentList,
          IFluentScreenshotElement<ElementScope> fluentElement)
          : base (fluentList, fluentElement)
      {
      }
    }

    private class DerivedScreenshotBocListNavigator<TList, TRow, TCell> : ScreenshotBocListNavigator<TList, TRow, TCell>
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      public DerivedScreenshotBocListNavigator (
          IFluentScreenshotElementWithCovariance<ScreenshotBocList<TList, TRow, TCell>> fluentList,
          IFluentScreenshotElement<ElementScope> fluentElement)
          : base (fluentList, fluentElement)
      {
      }
    }

    private class DerivedScreenshotBocListMenuBlock<TList, TRow, TCell> : ScreenshotBocListMenuBlock<TList, TRow, TCell>
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      public DerivedScreenshotBocListMenuBlock (
          IFluentScreenshotElementWithCovariance<ScreenshotBocList<TList, TRow, TCell>> fluentList,
          IFluentScreenshotElement<ElementScope> fluentElement)
          : base (fluentList, fluentElement)
      {
      }
    }

    private class DerivedScreenshotBocListDropDown<TList, TRow, TCell> : ScreenshotBocListDropDown<TList, TRow, TCell>
        where TList : BocListControlObjectBase<TRow, TCell>, IControlObjectWithRows<TRow>
        where TRow : ControlObject, IControlObjectWithCells<TCell>
        where TCell : ControlObject
    {
      public DerivedScreenshotBocListDropDown (
          IFluentScreenshotElementWithCovariance<ScreenshotBocList<TList, TRow, TCell>> fluentList,
          IFluentScreenshotElement<ElementScope> fluentElement)
          : base (fluentList, fluentElement)
      {
      }
    }
  }
}