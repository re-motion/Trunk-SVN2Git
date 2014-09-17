using System;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class SingleViewControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelector ()
    {
      var home = Start("SingleViewTest.aspx");

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var singleView = home.GetControl (new SingleViewSelector(), new ControlSelectionParameters { ID = "MySingleView" });
      Assert.That (singleView.Scope.Text, Is.StringContaining ("Content"));
      Assert.That (singleView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));

      var topControls = singleView.GetTopControls();
      Assert.That (topControls.Scope.Text, Is.StringContaining ("TopControls"));
      Assert.That (topControls.Scope.Text, Is.Not.StringContaining ("Content"));
      var view = singleView.GetView();
      Assert.That (view.Scope.Text, Is.StringContaining ("Content"));
      Assert.That (view.Scope.Text, Is.Not.StringContaining ("TopControls"));
      Assert.That (view.Scope.Text, Is.Not.StringContaining ("BottomControls"));
      var bottomControls = singleView.GetBottomControls();
      Assert.That (bottomControls.Scope.Text, Is.StringContaining ("BottomControls"));
      Assert.That (bottomControls.Scope.Text, Is.Not.StringContaining ("Content"));

      singleView = home.GetControl (new SingleViewSelector(), new ControlSelectionParameters { Index = 1 });
      Assert.That (singleView.Scope.Text, Is.StringContaining ("Content"));
      Assert.That (singleView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));

      singleView = home.GetControl (new SingleViewSelector(), new ControlSelectionParameters());
      Assert.That (singleView.Scope.Text, Is.StringContaining ("Content"));
      Assert.That (singleView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }
  }
}