using System;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocListAsGridControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByID ("body_DataEditControl_JobList_Normal");
      Assert.That (bocList.Scope.Id, Is.EqualTo ("body_DataEditControl_JobList_Normal"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByIndex (1);
      Assert.That (bocList.Scope.Id, Is.EqualTo ("body_DataEditControl_JobList_Normal"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Normal");
      Assert.That (bocList.Scope.Id, Is.EqualTo ("body_DataEditControl_JobList_Normal"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().First();
      Assert.That (bocList.Scope.Id, Is.EqualTo ("body_DataEditControl_JobList_Normal"));
    }

    [Test]
    public void TestSelection_Single ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().Single();
      Assert.That (bocList.Scope.Id, Is.EqualTo ("body_DataEditControl_JobList_Normal"));
    }

    [Test]
    public void TestSelection_DisplayName ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByDisplayName ("Jobs");
      Assert.That (bocList.Scope.Id, Is.EqualTo ("body_DataEditControl_JobList_Normal"));
    }

    [Test]
    public void TestSelection_DomainProperty ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByDomainProperty ("Jobs");
      Assert.That (bocList.Scope.Id, Is.EqualTo ("body_DataEditControl_JobList_Normal"));
    }

    [Test]
    public void TestSelection_DomainPropertyAndClass ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByDomainProperty ("Jobs", "Remotion.ObjectBinding.Sample.Person, Remotion.ObjectBinding.Sample");
      Assert.That (bocList.Scope.Id, Is.EqualTo ("body_DataEditControl_JobList_Normal"));
    }

    [Test]
    public void TestGetColumnTitles ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Normal");
      Assert.That (
          bocList.GetColumnTitles(),
          Is.EquivalentTo (new[] { "I_ndex", null, "Command", "Menu", "Title", "StartDate", "EndDate", "DisplayName", "TitleWithCmd" }));
    }

    [Test]
    public void TestGetRowCount ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Normal");
      Assert.That (bocList.GetRowCount(), Is.EqualTo (5));
    }

    [Test]
    public void TestGetRow ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Normal");

      var row = bocList.GetRow ("0ba19f5c-f2a2-4c9f-83c9-e6d25b461d98");
      Assert.That (row.GetCell (8).GetText(), Is.EqualTo ("CEO"));

      row = bocList.GetRow (1);
      Assert.That (row.GetCell (8).GetText(), Is.EqualTo ("Programmer"));
    }

    [Test]
    public void TestGetDropDownMenu ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Normal");
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

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Normal");
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

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Normal");
      var row = bocList.GetRow (2);

      var cell = row.GetCell ("DisplayName");
      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));

      cell = row.GetCell (8);
      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));
    }

    [Test]
    public void TestRowClickSelectCheckbox ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Normal");
      var row = bocList.GetRow (2);

      row.ClickSelectCheckbox();
      row.GetCell (3).PerformCommand();

      Assert.That (home.Scope.FindIdEndingWith ("SelectedIndicesLabel").Text, Is.EqualTo ("1"));
    }

    [Test]
    public void TestRowGetRowDropDownMenu ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Normal");
      var row = bocList.GetRow (2);
      var dropDownMenu = row.GetRowDropDownMenu();
      dropDownMenu.ClickItem ("RowMenuItemCmd2");

      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("JobList_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderRowLabel").Text, Is.EqualTo ("1"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("RowContextMenuClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("RowMenuItemCmd2|Row menu 2"));
    }

    [Test]
    public void TestCellGetText ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Normal");
      var cell = bocList.GetRow (2).GetCell (8);

      Assert.That (cell.GetText(), Is.EqualTo ("CEO"));
    }

    [Test]
    public void TestCellPerformCommand ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Normal");
      var cell = bocList.GetRow (2).GetCell (3);

      cell.PerformCommand();

      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderLabel").Text, Is.EqualTo ("JobList_Normal"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedSenderRowLabel").Text, Is.EqualTo ("1"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedLabel").Text, Is.EqualTo ("CellCommandClick"));
      Assert.That (home.Scope.FindIdEndingWith ("ActionPerformedParameterLabel").Text, Is.EqualTo ("RowCmd"));
    }
    
    [Test]
    public void TestEditableCellGetControl ()
    {
      var home = Start();

      var bocList = home.GetListAsGrid().ByLocalID ("JobList_Normal");
      var editableRow = bocList.GetRow (2);
      var editableCell = editableRow.GetCell (5);

      var bocText = editableCell.GetTextValue().First();
      bocText.FillWith ("NewTitle");

      Assert.That (bocText.GetText(), Is.EqualTo ("NewTitle"));
    }

    private RemotionPageObject Start ()
    {
      return Start ("BocListAsGrid");
    }
  }
}