using System;
using Coypu;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class DropDownMenuControlObjectTest : IntegrationTest
  {
    // Note that the <see cref="T:DropDownMenu.Mode"/>=<see cref="T:MenuMode.ContextMenu"/> option is tested indirectly by the BocTreeViewControlObjectTest.

    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var dropDownMenu = home.GetDropDownMenu().ByID ("body_MyDropDownMenu");
      Assert.That (dropDownMenu.Scope.Id, Is.EqualTo ("body_MyDropDownMenu"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var dropDownMenu = home.GetDropDownMenu().ByIndex (2);
      Assert.That (dropDownMenu.Scope.Id, Is.EqualTo ("body_MyDropDownMenu2"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var dropDownMenu = home.GetDropDownMenu().ByLocalID ("MyDropDownMenu");
      Assert.That (dropDownMenu.Scope.Id, Is.EqualTo ("body_MyDropDownMenu"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var dropDownMenu = home.GetDropDownMenu().First();
      Assert.That (dropDownMenu.Scope.Id, Is.EqualTo ("body_MyDropDownMenu"));
    }

    [Test]
    public void TestSelection_Single ()
    {
      var home = Start();
      var scope = new ScopeControlObject ("scope", home.Context.CloneForScope (home.Scope.FindId ("scope")));

      var dropDownMenu = scope.GetDropDownMenu().Single();
      Assert.That (dropDownMenu.Scope.Id, Is.EqualTo ("body_MyDropDownMenu2"));

      try
      {
        home.GetDropDownMenu().Single();
        Assert.Fail ("Should not be able to unambigously find a list menu.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestClickItem ()
    {
      var home = Start();

      var dropDownMenu = home.GetDropDownMenu().ByLocalID ("MyDropDownMenu");

      dropDownMenu.ClickItem ("ItemID5");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("ItemID5|Event"));

      dropDownMenu.ClickItem (2);
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.Empty);

      dropDownMenu.ClickItemByHtmlID ("body_MyDropDownMenu_3");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("ItemID4|WxeFunction"));

      dropDownMenu.ClickItemByText ("EventItem");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("ItemID1|Event"));
    }

    private RemotionPageObject Start ()
    {
      return Start ("DropDownMenuTest.wxe");
    }
  }
}