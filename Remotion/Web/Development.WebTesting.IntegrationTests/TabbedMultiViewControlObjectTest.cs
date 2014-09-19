using System;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class TabbedMultiViewControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start ("TabbedMultiViewTest.wxe");

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var tabbedMultiView = home.GetTabbedMultiView().ByID ("body_MyTabbedMultiView");
      Assert.That (tabbedMultiView.Scope.Text, Is.StringContaining ("Content1"));
      Assert.That (tabbedMultiView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start ("TabbedMultiViewTest.wxe");

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var tabbedMultiView = home.GetTabbedMultiView().ByIndex (1);
      Assert.That (tabbedMultiView.Scope.Text, Is.StringContaining ("Content"));
      Assert.That (tabbedMultiView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start ("TabbedMultiViewTest.wxe");

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var tabbedMultiView = home.GetTabbedMultiView().ByLocalID ("MyTabbedMultiView");
      Assert.That (tabbedMultiView.Scope.Text, Is.StringContaining ("Content1"));
      Assert.That (tabbedMultiView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start ("TabbedMultiViewTest.wxe");

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var tabbedMultiView = home.GetTabbedMultiView().First();
      Assert.That (tabbedMultiView.Scope.Text, Is.StringContaining ("Content1"));
      Assert.That (tabbedMultiView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }

    [Test]
    public void TestSelection_Single ()
    {
      var home = Start ("TabbedMultiViewTest.wxe");

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var tabbedMultiView = home.GetTabbedMultiView().Single();
      Assert.That (tabbedMultiView.Scope.Text, Is.StringContaining ("Content1"));
      Assert.That (tabbedMultiView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }

    [Test]
    public void TestSubScope_TopControls ()
    {
      var home = Start ("TabbedMultiViewTest.wxe");

      var tabbedMultiView = home.GetTabbedMultiView().Single();

      var topControls = tabbedMultiView.GetTopControls();
      Assert.That (topControls.Scope.Text, Is.StringContaining ("TopControls"));
      Assert.That (topControls.Scope.Text, Is.Not.StringContaining ("Content1"));
    }

    [Test]
    public void TestSubScope_View ()
    {
      var home = Start ("TabbedMultiViewTest.wxe");

      var tabbedMultiView = home.GetTabbedMultiView().Single();

      var view = tabbedMultiView.GetActiveView();
      Assert.That (view.Scope.Text, Is.StringContaining ("Content1"));
      Assert.That (view.Scope.Text, Is.Not.StringContaining ("TopControls"));
      Assert.That (view.Scope.Text, Is.Not.StringContaining ("BottomControls"));
    }

    [Test]
    public void TestSubScope_BottomControls ()
    {
      var home = Start ("TabbedMultiViewTest.wxe");

      var tabbedMultiView = home.GetTabbedMultiView().Single();

      var bottomControls = tabbedMultiView.GetBottomControls();
      Assert.That (bottomControls.Scope.Text, Is.StringContaining ("BottomControls"));
      Assert.That (bottomControls.Scope.Text, Is.Not.StringContaining ("Content1"));
    }

    [Test]
    public void TestTabStrip ()
    {
      var home = Start ("TabbedMultiViewTest.wxe");

      var tabbedMultiView = home.GetTabbedMultiView().Single();

      home = tabbedMultiView.SwitchTo ("Tab2").Expect<RemotionPageObject>();
      tabbedMultiView = home.GetTabbedMultiView().Single();
      Assert.That (tabbedMultiView.Scope.Text, Is.StringContaining ("Content2"));
      Assert.That (tabbedMultiView.Scope.Text, Is.Not.StringContaining ("Content1"));

      home = tabbedMultiView.SwitchToByLabel ("Tab1Title").Expect<RemotionPageObject>();
      tabbedMultiView = home.GetTabbedMultiView().Single();
      Assert.That (tabbedMultiView.Scope.Text, Is.StringContaining ("Content1"));
      Assert.That (tabbedMultiView.Scope.Text, Is.Not.StringContaining ("Content2"));
    }
  }
}