using System;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class SingleViewControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start ("SingleViewTest.aspx");

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var singleView = home.GetSingleView().ByID ("body_MySingleView");
      Assert.That (singleView.Scope.Text, Is.StringContaining ("Content"));
      Assert.That (singleView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start ("SingleViewTest.aspx");

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var singleView = home.GetSingleView().ByIndex (1);
      Assert.That (singleView.Scope.Text, Is.StringContaining ("Content"));
      Assert.That (singleView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start ("SingleViewTest.aspx");

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var singleView = home.GetSingleView().ByLocalID ("MySingleView");
      Assert.That (singleView.Scope.Text, Is.StringContaining ("Content"));
      Assert.That (singleView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start ("SingleViewTest.aspx");

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var singleView = home.GetSingleView().First();
      Assert.That (singleView.Scope.Text, Is.StringContaining ("Content"));
      Assert.That (singleView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }

    [Test]
    public void TestSelection_Single ()
    {
      var home = Start ("SingleViewTest.aspx");

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var singleView = home.GetSingleView().Single();
      Assert.That (singleView.Scope.Text, Is.StringContaining ("Content"));
      Assert.That (singleView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }

    [Test]
    public void TestSubScope_TopControls ()
    {
      var home = Start ("SingleViewTest.aspx");

      var singleView = home.GetSingleView().Single();

      var topControls = singleView.GetTopControls();
      Assert.That (topControls.Scope.Text, Is.StringContaining ("TopControls"));
      Assert.That (topControls.Scope.Text, Is.Not.StringContaining ("Content"));
    }

    [Test]
    public void TestSubScope_View ()
    {
      var home = Start ("SingleViewTest.aspx");

      var singleView = home.GetSingleView().Single();

      var view = singleView.GetView();
      Assert.That (view.Scope.Text, Is.StringContaining ("Content"));
      Assert.That (view.Scope.Text, Is.Not.StringContaining ("TopControls"));
      Assert.That (view.Scope.Text, Is.Not.StringContaining ("BottomControls"));
    }

    [Test]
    public void TestSubScope_BottomControls ()
    {
      var home = Start ("SingleViewTest.aspx");

      var singleView = home.GetSingleView().Single();

      var bottomControls = singleView.GetBottomControls();
      Assert.That (bottomControls.Scope.Text, Is.StringContaining ("BottomControls"));
      Assert.That (bottomControls.Scope.Text, Is.Not.StringContaining ("Content"));
    }
  }
}