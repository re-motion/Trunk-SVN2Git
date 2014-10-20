using System;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class TabbedMenuControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var tabbedMenu = home.GetTabbedMenu().ByID ("body_MyTabbedMenu");
      Assert.That (tabbedMenu.Scope.Id, Is.EqualTo ("body_MyTabbedMenu"));
      Assert.That (tabbedMenu.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var tabbedMenu = home.GetTabbedMenu().ByIndex (1);
      Assert.That (tabbedMenu.Scope.Id, Is.EqualTo ("body_MyTabbedMenu"));
      Assert.That (tabbedMenu.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var tabbedMenu = home.GetTabbedMenu().ByLocalID ("MyTabbedMenu");
      Assert.That (tabbedMenu.Scope.Id, Is.EqualTo ("body_MyTabbedMenu"));
      Assert.That (tabbedMenu.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var tabbedMenu = home.GetTabbedMenu().First();
      Assert.That (tabbedMenu.Scope.Id, Is.EqualTo ("body_MyTabbedMenu"));
      Assert.That (tabbedMenu.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }

    [Test]
    public void TestSelection_Single ()
    {
      var home = Start();

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var tabbedMenu = home.GetTabbedMenu().Single();
      Assert.That (tabbedMenu.Scope.Id, Is.EqualTo ("body_MyTabbedMenu"));
      Assert.That (tabbedMenu.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }

    [Test]
    public void TestStatusText ()
    {
      var home = Start();

      var tabbedMenu = home.GetTabbedMenu().Single();
      Assert.That (tabbedMenu.StatusText, Is.EqualTo ("MyStatusText"));
    }

    [Test]
    public void TestSelectMenuItem ()
    {
      var home = Start();

      var tabbedMenu = home.GetTabbedMenu().Single();

      tabbedMenu.SelectMenuItem ("EventCommandTab");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("EventCommandTab|Event"));

      tabbedMenu.SelectMenuItem (1);
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("EventCommandTab|Event"));

      tabbedMenu.SelectMenuItemByHtmlID ("body_MyTabbedMenu_MainMenuTabStrip_EventCommandTab");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("EventCommandTab|Event"));

      tabbedMenu.SelectMenuItemByText ("EventCommandTabTitle");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("EventCommandTab|Event"));
    }

    [Test]
    public void TestSelectMenuItemCommand ()
    {
      var home = Start();

      var tabbedMenu = home.GetTabbedMenu().Single();

      tabbedMenu.SelectMenuItem ("EventCommandTab");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("EventCommandTab|Event"));

      tabbedMenu.SelectMenuItem ("HrefCommandTab");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.Empty);

      tabbedMenu.SelectMenuItem ("EventCommandTab");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("EventCommandTab|Event"));

      tabbedMenu.SelectMenuItem ("WxeFunctionCommandTab");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.Empty);
    }

    [Test]
    public void TestSelectSubMenuItem ()
    {
      var home = Start();

      var tabbedMenu = home.GetTabbedMenu().Single();
      tabbedMenu.SelectMenuItem ("TabWithSubMenu");

      tabbedMenu.SelectSubMenuItem ("SubMenuTab1");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("SubMenuTab1|Event"));

      tabbedMenu.SelectSubMenuItem (3);
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.Empty);

      tabbedMenu.SelectSubMenuItemByHtmlID ("body_MyTabbedMenu_SubMenuTabStrip_SubMenuTab1");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("SubMenuTab1|Event"));

      tabbedMenu.SelectSubMenuItemByText ("SubMenuTab2Title");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("SubMenuTab2|Event"));
    }

    private RemotionPageObject Start ()
    {
      return Start ("TabbedMenuTest.wxe");
    }
  }
}