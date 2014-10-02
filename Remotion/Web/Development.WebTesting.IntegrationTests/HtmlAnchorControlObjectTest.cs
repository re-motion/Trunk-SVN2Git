using System;
using Coypu;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class HtmlAnchorControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start ("HtmlAnchorTest.wxe");

      var webButton = home.GetHtmlAnchor().ByID ("body_MyHtmlAnchor");
      Assert.That (webButton.Scope.Id, Is.EqualTo ("body_MyHtmlAnchor"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start ("HtmlAnchorTest.wxe");

      var webButton = home.GetHtmlAnchor().ByIndex (2);
      Assert.That (webButton.Scope.Id, Is.EqualTo ("body_MyHtmlAnchor2"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start ("HtmlAnchorTest.wxe");

      var webButton = home.GetHtmlAnchor().ByLocalID ("MyHtmlAnchor2");
      Assert.That (webButton.Scope.Id, Is.EqualTo ("body_MyHtmlAnchor2"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start ("HtmlAnchorTest.wxe");

      var webButton = home.GetHtmlAnchor().First();
      Assert.That (webButton.Scope.Id, Is.EqualTo ("body_MyHtmlAnchor"));
    }

    [Test]
    public void TestSelection_Single ()
    {
      var home = Start ("HtmlAnchorTest.wxe");
      var scope = new ScopeControlObject ("scope", home.Context.CloneForScope (home.Scope.FindId ("scope")));

      var webButton = scope.GetHtmlAnchor().Single();
      Assert.That (webButton.Scope.Id, Is.EqualTo ("body_MyHtmlAnchor2"));

      try
      {
        home.GetHtmlAnchor().Single();
        Assert.Fail ("Should not be able to unambigously find an HTML anchor.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestClick ()
    {
      var home = Start ("HtmlAnchorTest.wxe");

      var hyperLink = home.GetHtmlAnchor().ByLocalID ("MyHtmlAnchor");
      home = hyperLink.Click().Expect<RemotionPageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("MyCommand"));

      var hyperLink2 = home.GetHtmlAnchor().ByLocalID ("MyHtmlAnchor2");
      home = hyperLink2.Click().Expect<RemotionPageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.Empty);
    }
  }
}