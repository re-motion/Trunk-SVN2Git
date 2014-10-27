using System;
using Coypu;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocListControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var bocList = home.GetList().ByID ("body_DataEditControl_JobList_Normal");
      Assert.That (bocList.Scope.Id, Is.EqualTo ("body_DataEditControl_JobList_Normal"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var bocList = home.GetList().ByIndex (2);
      Assert.That (bocList.Scope.Id, Is.EqualTo ("body_DataEditControl_JobList_ReadOnly"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var bocList = home.GetList().ByLocalID ("JobList_Normal");
      Assert.That (bocList.Scope.Id, Is.EqualTo ("body_DataEditControl_JobList_Normal"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var bocList = home.GetList().First();
      Assert.That (bocList.Scope.Id, Is.EqualTo ("body_DataEditControl_JobList_Normal"));
    }

    [Test]
    public void TestSelection_Single ()
    {
      var home = Start();

      try
      {
        home.GetList().Single();
        Assert.Fail ("Should not be able to unambigously find a BOC list.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestSelection_DisplayName ()
    {
      var home = Start();

      var bocList = home.GetList().ByDisplayName ("Jobs");
      Assert.That (bocList.Scope.Id, Is.EqualTo ("body_DataEditControl_JobList_Normal"));
    }

    [Test]
    public void TestSelection_DomainProperty ()
    {
      var home = Start();

      var bocList = home.GetList().ByDomainProperty ("Jobs");
      Assert.That (bocList.Scope.Id, Is.EqualTo ("body_DataEditControl_JobList_Normal"));
    }

    [Test]
    public void TestSelection_DomainPropertyAndClass ()
    {
      var home = Start();

      var bocList = home.GetList().ByDomainProperty ("Jobs", "Remotion.ObjectBinding.Sample.Person, Remotion.ObjectBinding.Sample");
      Assert.That (bocList.Scope.Id, Is.EqualTo ("body_DataEditControl_JobList_Normal"));
    }

    [Test]
    public void TestGetColumnTitles ()
    {
      var home = Start();

      var bocList = home.GetList().ByLocalID ("JobList_Normal");
      Assert.That (
          bocList.GetColumnTitles(),
          Is.EquivalentTo (new[] { "I_ndex", null, null, "Command", "Menu", "Title", "StartDate", "EndDate", "DisplayName", "TitleWithCmd" }));
    }

    [Test]
    public void TestGetRowCount ()
    {
      var home = Start();

      var bocList = home.GetList().ByLocalID ("JobList_Normal");
      Assert.That (bocList.GetRowCount(), Is.EqualTo (2));
    }

    [Test]
    public void TestPaging ()
    {
      var home = Start();

      var bocList = home.GetList().ByLocalID ("JobList_Normal");
      Assert.That (bocList.GetNumberOfPages(), Is.EqualTo (3));

      bocList.GoToNextPage();
      Assert.That (bocList.GetCurrentPage(), Is.EqualTo (2));

      bocList.GoToPreviousPage();
      Assert.That (bocList.GetCurrentPage(), Is.EqualTo (1));

      bocList.GoToLastPage();
      Assert.That (bocList.GetCurrentPage(), Is.EqualTo (3));
      Assert.That (bocList.GetRowCount(), Is.EqualTo (1));

      bocList.GoToFirstPage();
      Assert.That (bocList.GetCurrentPage(), Is.EqualTo (1));

      bocList.GoToSpecificPage (3);
      Assert.That (bocList.GetCurrentPage(), Is.EqualTo (3));
    }

    [Test]
    public void TestGetRow ()
    {
      var home = Start();

      var bocList = home.GetList().ByLocalID ("JobList_Normal");

      var row = bocList.GetRow ("0ba19f5c-f2a2-4c9f-83c9-e6d25b461d98");
      Assert.That (row.GetCell (6).GetText(), Is.EqualTo ("CEO"));

      row = bocList.GetRow (1);
      Assert.That (row.GetCell (6).GetText(), Is.EqualTo ("Programmer"));
    }

    [Test]
    public void TestGetRowWhere ()
    {
      var home = Start();

      var bocList = home.GetList().ByLocalID ("JobList_Normal");

      var row = bocList.GetRowWhere ("Title", "CEO");
      Assert.That (row.GetCell (6).GetText(), Is.EqualTo ("CEO"));

      row = bocList.GetRowWhere (6, "CEO");
      Assert.That (row.GetCell (6).GetText(), Is.EqualTo ("CEO"));

      row = bocList.GetRowWhereByColumnTitle ("Title", "CEO");
      Assert.That (row.GetCell (6).GetText(), Is.EqualTo ("CEO"));
    }

    [Test]
    public void TestGetCellWhere ()
    {
      var home = Start();

      var bocList = home.GetList().ByLocalID ("JobList_Normal");

      var cell = bocList.GetCellWhere ("Title", "CEO");
      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));

      cell = bocList.GetCellWhere (6, "CEO");
      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));

      cell = bocList.GetCellWhereByColumnTitle ("Title", "CEO");
      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));
    }

    [Test]
    public void TestClickOnSortColumn ()
    {
      var home = Start();

      var bocList = home.GetList().ByLocalID ("JobList_Normal");
      
      bocList.ClickOnSortColumn ("StartDate");
      bocList.ClickOnSortColumn ("Title");
      Assert.That (bocList.GetRow (2).GetCell (6).GetText(), Is.EqualTo ("Programmer"));

      bocList.ClickOnSortColumn (6);
      Assert.That (bocList.GetRow (2).GetCell (6).GetText(), Is.EqualTo ("Clerk"));

      bocList.ClickOnSortColumnByTitle ("Title");
      bocList.ClickOnSortColumnByTitle ("StartDate");
      Assert.That (bocList.GetRow (2).GetCell (6).GetText(), Is.EqualTo ("Developer"));
    }

    [Test]
    public void TestChangeViewTo ()
    {
      var home = Start();

      var bocList = home.GetList().ByLocalID ("JobList_Normal");

      bocList.ChangeViewToByLabel ("View 1");
      Assert.That (home.Scope.FindIdEndingWith ("SelectedViewLabel").Text, Is.EqualTo ("ViewCmd1"));

      bocList.ChangeViewTo (2);
      Assert.That (home.Scope.FindIdEndingWith ("SelectedViewLabel").Text, Is.EqualTo ("ViewCmd2"));

      bocList.ChangeViewTo ("ViewCmd1");
      Assert.That (home.Scope.FindIdEndingWith ("SelectedViewLabel").Text, Is.EqualTo ("ViewCmd1"));
    }

    [Test]
    public void TestGetDropDownMenu ()
    {
      var home = Start();

      var bocList = home.GetList().ByLocalID ("JobList_Normal");
      var dropDownMenu = bocList.GetDropDownMenu();
      dropDownMenu.ClickItem ("OptCmd2");

      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("JobList_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderRowLabel").Text, Is.EqualTo ("-1"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("ListMenuOrOptionsClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("OptCmd2|Option command 2"));
    }

    [Test]
    public void TestGetListMenu ()
    {
      var home = Start();

      var bocList = home.GetList().ByLocalID ("JobList_Normal");
      var listMenu = bocList.GetListMenu();
      listMenu.ClickItem ("ListMenuCmd3");

      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("JobList_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderRowLabel").Text, Is.EqualTo ("-1"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("ListMenuOrOptionsClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("ListMenuCmd3|LM cmd 3"));
    }

    [Test]
    public void TestRowGetCell ()
    {
      var home = Start();

      var bocList = home.GetList().ByLocalID ("JobList_Normal");
      var row = bocList.GetRow (2);

      var cell = row.GetCell ("Title");
      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));

      cell = row.GetCell (6);
      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));
    }

    [Test]
    public void TestRowClickSelectCheckbox ()
    {
      var home = Start();

      var bocList = home.GetList().ByLocalID ("JobList_Normal");
      var row = bocList.GetRow (2);

      row.ClickSelectCheckbox();
      row.GetCell (4).PerformCommand();

      Assert.That (home.Scope.FindIdEndingWith ("SelectedIndicesLabel").Text, Is.EqualTo ("1"));
    }

    [Test]
    public void TestRowGetRowDropDownMenu ()
    {
      var home = Start();

      var bocList = home.GetList().ByLocalID ("JobList_Normal");
      var row = bocList.GetRow (2);
      var dropDownMenu = row.GetRowDropDownMenu();
      dropDownMenu.ClickItem ("RowMenuItemCmd2");

      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("JobList_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderRowLabel").Text, Is.EqualTo ("1"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("RowContextMenuClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("RowMenuItemCmd2|Row menu 2"));
    }

    [Test]
    public void TestRowEdit ()
    {
      var home = Start();

      var bocList = home.GetList().ByLocalID ("JobList_Normal");
      var row = bocList.GetRow (2);

      Assert.That (home.Scope.FindIdEndingWith ("EditModeLabel").Text, Is.EqualTo ("False"));

      row.Edit();
      Assert.That (home.Scope.FindIdEndingWith ("EditModeLabel").Text, Is.EqualTo ("True"));
    }

    [Test]
    public void TestEditableRowSave ()
    {
      var home = Start();

      var bocList = home.GetList().ByLocalID ("JobList_Normal");
      var editableRow = bocList.GetRow (2).Edit();

      editableRow.Save();
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("JobList_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderRowLabel").Text, Is.EqualTo ("1"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("InLineEdit"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("Saved"));
    }

    [Test]
    public void TestEditableRowCancel ()
    {
      var home = Start();

      var bocList = home.GetList().ByLocalID ("JobList_Normal");
      var editableRow = bocList.GetRow (2).Edit();

      editableRow.Cancel();
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("JobList_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderRowLabel").Text, Is.EqualTo ("1"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("InLineEdit"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("Canceled"));
    }

    [Test]
    public void TestEditableRowGetCell ()
    {
      var home = Start();

      var bocList = home.GetList().ByLocalID ("JobList_Normal");
      var editableRow = bocList.GetRow (2).Edit();

      var cell = editableRow.GetCell ("Title");
      Assert.That (cell.GetTextValue().First().GetText(), Is.EqualTo ("CEO"));

      cell = editableRow.GetCell (6);
      Assert.That (cell.GetTextValue().First().GetText(), Is.EqualTo ("CEO"));
    }

    [Test]
    public void TestCellGetText ()
    {
      var home = Start();

      var bocList = home.GetList().ByLocalID ("JobList_Normal");
      var cell = bocList.GetRow (2).GetCell (9);

      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));
    }

    [Test]
    public void TestCellPerformCommand ()
    {
      var home = Start();

      var bocList = home.GetList().ByLocalID ("JobList_Normal");
      var cell = bocList.GetRow (2).GetCell (4);

      cell.PerformCommand();

      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("JobList_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderRowLabel").Text, Is.EqualTo ("1"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("CellCommandClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("|Command")); // Todo RM-6297: ItemID?
    }
    
    [Test]
    public void TestEditableCellGetControl ()
    {
      var home = Start();

      var bocList = home.GetList().ByLocalID ("JobList_Normal");
      var editableRow = bocList.GetRow (2).Edit();
      var editableCell = editableRow.GetCell (6);

      var bocText = editableCell.GetTextValue().First();
      bocText.FillWith ("NewTitle");

      editableRow.Save();
      Assert.That (bocList.GetCellWhere ("Title", "NewTitle").GetText(), Is.EqualTo ("NewTitle"));
    }

    private RemotionPageObject Start ()
    {
      return Start ("BocList");
    }
  }
}