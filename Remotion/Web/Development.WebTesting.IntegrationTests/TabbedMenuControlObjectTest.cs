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
      Assert.That (tabbedMenu.GetStatusText(), Is.EqualTo ("MyStatusText"));
    }

    [Test]
    public void TestSelectMenuItem ()
    {
      var home = Start();

      var tabbedMenu = home.GetTabbedMenu().Single();

      tabbedMenu.SelectItem ("EventCommandTab");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("EventCommandTab|Event"));

      tabbedMenu.SelectItem().WithIndex (1);
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("EventCommandTab|Event"));

      tabbedMenu.SelectItem().WithHtmlID ("body_MyTabbedMenu_MainMenuTabStrip_EventCommandTab");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("EventCommandTab|Event"));

      tabbedMenu.SelectItem().WithText ("EventCommandTabTitle");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("EventCommandTab|Event"));
    }

    [Test]
    public void TestSelectMenuItemCommand ()
    {
      var home = Start();

      var tabbedMenu = home.GetTabbedMenu().Single();

      tabbedMenu.SelectItem ("EventCommandTab");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("EventCommandTab|Event"));

      tabbedMenu.SelectItem ("HrefCommandTab");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.Empty);

      tabbedMenu.SelectItem ("EventCommandTab");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("EventCommandTab|Event"));

      tabbedMenu.SelectItem ("WxeFunctionCommandTab");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.Empty);
    }

    [Test]
    public void TestSelectSubMenuItem ()
    {
      var home = Start();

      var tabbedMenu = home.GetTabbedMenu().Single();
      tabbedMenu.SelectItem ("TabWithSubMenu");

      tabbedMenu.SelectSubItem ("SubMenuTab1");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("SubMenuTab1|Event"));

      tabbedMenu.SelectSubItem().WithIndex (3);
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.Empty);

      tabbedMenu.SelectSubItem().WithHtmlID ("body_MyTabbedMenu_SubMenuTabStrip_SubMenuTab1");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("SubMenuTab1|Event"));

      tabbedMenu.SelectSubItem().WithText ("SubMenuTab2Title");
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("SubMenuTab2|Event"));
    }

    private RemotionPageObject Start ()
    {
      return Start ("TabbedMenuTest.wxe");
    }
  }
}