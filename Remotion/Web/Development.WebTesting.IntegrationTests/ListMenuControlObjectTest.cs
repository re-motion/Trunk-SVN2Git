using System;
using Coypu;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ListMenuControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var listMenu = home.GetListMenu().ByID ("body_MyListMenu");
      Assert.That (listMenu.Scope.Id, Is.EqualTo ("body_MyListMenu"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var listMenu = home.GetListMenu().ByIndex (2);
      Assert.That (listMenu.Scope.Id, Is.EqualTo ("body_MyListMenu2"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var listMenu = home.GetListMenu().ByLocalID ("MyListMenu");
      Assert.That (listMenu.Scope.Id, Is.EqualTo ("body_MyListMenu"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var listMenu = home.GetListMenu().First();
      Assert.That (listMenu.Scope.Id, Is.EqualTo ("body_MyListMenu"));
    }

    [Test]
    public void TestSelection_Single ()
    {
      var home = Start();
      var scope = new ScopeControlObject (home.Context.CloneForControl (home, home.Scope.FindId ("scope")));

      var listMenu = scope.GetListMenu().Single();
      Assert.That (listMenu.Scope.Id, Is.EqualTo ("body_MyListMenu2"));

      try
      {
        home.GetListMenu().Single();
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

      var listMenu = home.GetListMenu().ByLocalID ("MyListMenu");

      listMenu.ClickItem ("ItemID5");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("ItemID5|Event"));

      listMenu.ClickItem (2);
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.Empty);

      listMenu.ClickItemByHtmlID ("body_MyListMenu_3");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("ItemID4|WxeFunction"));

      listMenu.ClickItemByText ("EventItem");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("ItemID1|Event"));
    }

    private RemotionPageObject Start ()
    {
      return Start ("ListMenuTest.wxe");
    }
  }
}