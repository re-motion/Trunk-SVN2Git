using System;
using Coypu;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class WebTreeViewControlObjectTest : IntegrationTest
  {
    // Note: functionality is integration tested via BocTreeViewControlObject in BocTreeViewControlObjectTest.

    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var webTreeView = home.GetWebTreeView().ByID ("body_MyWebTreeView");
      Assert.That (webTreeView.Scope.Id, Is.EqualTo ("body_MyWebTreeView"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var webTreeView = home.GetWebTreeView().ByIndex (2);
      Assert.That (webTreeView.Scope.Id, Is.EqualTo ("body_MyWebTreeView2"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var webTreeView = home.GetWebTreeView().ByLocalID ("MyWebTreeView");
      Assert.That (webTreeView.Scope.Id, Is.EqualTo ("body_MyWebTreeView"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var webTreeView = home.GetWebTreeView().First();
      Assert.That (webTreeView.Scope.Id, Is.EqualTo ("body_MyWebTreeView"));
    }

    [Test]
    public void TestSelection_Single ()
    {
      var home = Start();
      var scope = new ScopeControlObject ("scope", home.Context.CloneForScope (home.Scope.FindId ("scope")));

      var webTreeView = scope.GetWebTreeView().Single();
      Assert.That (webTreeView.Scope.Id, Is.EqualTo ("body_MyWebTreeView2"));

      try
      {
        home.GetWebTreeView().Single();
        Assert.Fail ("Should not be able to unambigously find a web tree view.");
      }
      catch (AmbiguousException)
      {
      }
    }

    private RemotionPageObject Start ()
    {
      return Start ("WebTreeViewTest.aspx");
    }
  }
}