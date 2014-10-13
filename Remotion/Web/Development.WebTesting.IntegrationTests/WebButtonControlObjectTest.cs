using System;
using Coypu;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class WebButtonControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var webButton = home.GetWebButton().ByID ("body_MyWebButton1Sync");
      Assert.That (webButton.Scope.Id, Is.EqualTo ("body_MyWebButton1Sync"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var webButton = home.GetWebButton().ByIndex (2);
      Assert.That (webButton.Scope.Id, Is.EqualTo ("body_MyWebButton2Async"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var webButton = home.GetWebButton().ByLocalID ("MyWebButton3Href");
      Assert.That (webButton.Scope.Id, Is.EqualTo ("body_MyWebButton3Href"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var webButton = home.GetWebButton().First();
      Assert.That (webButton.Scope.Id, Is.EqualTo ("body_MyWebButton1Sync"));
    }

    [Test]
    public void TestSelection_Single ()
    {
      var home = Start();
      var scope = new ScopeControlObject ("scope", home.Context.CloneForScope (home.Scope.FindId ("scope")));

      var webButton = scope.GetWebButton().Single();
      Assert.That (webButton.Scope.Id, Is.EqualTo ("body_MyWebButton3Href"));

      try
      {
        home.GetWebButton().Single();
        Assert.Fail ("Should not be able to unambigously find a button.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestSelection_Text ()
    {
      var home = Start();

      var webButton = home.GetWebButton().ByText ("AsyncButton");
      Assert.That (webButton.Scope.Id, Is.EqualTo ("body_MyWebButton2Async"));
    }

    [Test]
    public void TestSelection_CommandName ()
    {
      var home = Start();

      var webButton = home.GetWebButton().ByCommandName ("Sync");
      Assert.That (webButton.Scope.Id, Is.EqualTo ("body_MyWebButton1Sync"));
    }

    [Test]
    public void TestClick ()
    {
      var home = Start();

      var syncWebButton = home.GetWebButton().ByCommandName ("Sync");
      home = syncWebButton.Click().Expect<RemotionPageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("Sync"));

      var asyncWebButton = home.GetWebButton().ByCommandName ("Async");
      home = asyncWebButton.Click().Expect<RemotionPageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("Async"));

      var hrefWebButton = home.GetWebButton().ByText ("HrefButton");
      home = hrefWebButton.Click().Expect<RemotionPageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.Empty);
    }

    private RemotionPageObject Start ()
    {
      return Start("WebButtonTest.wxe");
    }
  }
}