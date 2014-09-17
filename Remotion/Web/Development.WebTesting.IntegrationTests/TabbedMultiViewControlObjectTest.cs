using System;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class TabbedMultiViewControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelector ()
    {
      var home = Start ("TabbedMultiViewTest.wxe");

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var tabbedMultiView = home.GetControl (new TabbedMultiViewSelector(), new ControlSelectionParameters { ID = "MyTabbedMultiView" });
      Assert.That (tabbedMultiView.Scope.Text, Is.StringContaining ("Content1"));
      Assert.That (tabbedMultiView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));

      var topControls = tabbedMultiView.GetTopControls();
      Assert.That (topControls.Scope.Text, Is.StringContaining ("TopControls"));
      Assert.That (topControls.Scope.Text, Is.Not.StringContaining ("Content1"));
      var view = tabbedMultiView.GetActiveView();
      Assert.That (view.Scope.Text, Is.StringContaining ("Content1"));
      Assert.That (view.Scope.Text, Is.Not.StringContaining ("TopControls"));
      Assert.That (view.Scope.Text, Is.Not.StringContaining ("BottomControls"));
      var bottomControls = tabbedMultiView.GetBottomControls();
      Assert.That (bottomControls.Scope.Text, Is.StringContaining ("BottomControls"));
      Assert.That (bottomControls.Scope.Text, Is.Not.StringContaining ("Content1"));

      tabbedMultiView = home.GetControl (new TabbedMultiViewSelector(), new ControlSelectionParameters { Index = 1 });
      Assert.That (tabbedMultiView.Scope.Text, Is.StringContaining ("Content1"));
      Assert.That (tabbedMultiView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));

      tabbedMultiView = home.GetControl (new TabbedMultiViewSelector(), new ControlSelectionParameters());
      Assert.That (tabbedMultiView.Scope.Text, Is.StringContaining ("Content1"));
      Assert.That (tabbedMultiView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));

      home = tabbedMultiView.GetTabStrip().SwitchTo ("Tab2").Expect<RemotionPageObject>();
      tabbedMultiView = home.GetControl (new TabbedMultiViewSelector(), new ControlSelectionParameters { ID = "MyTabbedMultiView" });
      Assert.That (tabbedMultiView.Scope.Text, Is.StringContaining ("Content2"));
      Assert.That (tabbedMultiView.Scope.Text, Is.Not.StringContaining ("Content1"));

      home = tabbedMultiView.GetTabStrip().SwitchTo ("Tab1").Expect<RemotionPageObject>();
      tabbedMultiView = home.GetControl (new TabbedMultiViewSelector(), new ControlSelectionParameters { ID = "MyTabbedMultiView" });
      Assert.That (tabbedMultiView.Scope.Text, Is.StringContaining ("Content1"));
      Assert.That (tabbedMultiView.Scope.Text, Is.Not.StringContaining ("Content2"));
    }
  }
}