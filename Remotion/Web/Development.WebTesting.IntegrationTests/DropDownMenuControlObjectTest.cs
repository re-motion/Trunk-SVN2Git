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
    // Note: the <see cref="T:DropDownMenu.Mode"/>=<see cref="T:MenuMode.ContextMenu"/> option is tested indirectly by the BocTreeViewControlObjectTest.

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
      var scope = new ScopeControlObject (home.Context.CloneForControl (home, home.Scope.FindId ("scope")));

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
    public void TestSelection_Text ()
    {
      var home = Start();

      var dropDownMenu = home.GetDropDownMenu().ByText ("MyTitleText");
      Assert.That (dropDownMenu.Scope.Id, Is.EqualTo ("body_MyDropDownMenu2"));
    }

    [Test]
    public void TestClickItem ()
    {
      var home = Start();

      var dropDownMenu = home.GetDropDownMenu().ByLocalID ("MyDropDownMenu");

      dropDownMenu.SelectItem ("ItemID5");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("ItemID5|Event"));

      dropDownMenu.SelectItem().WithIndex (2);
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.Empty);

      dropDownMenu.SelectItem().WithHtmlID ("body_MyDropDownMenu_3");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("ItemID4|WxeFunction"));

      dropDownMenu.SelectItem().WithText ("EventItem");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("ItemID1|Event"));
    }

    private RemotionPageObject Start ()
    {
      return Start ("DropDownMenuTest.wxe");
    }
  }
}