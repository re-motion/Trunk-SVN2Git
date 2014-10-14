using System;
using Coypu;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class AnchorControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var webLinkButton = home.GetAnchor().ByID ("body_MyWebLinkButton");
      Assert.That (webLinkButton.Scope.Id, Is.EqualTo ("body_MyWebLinkButton"));

      var smartHyperLink = home.GetAnchor().ByID ("body_MySmartHyperLink");
      Assert.That (smartHyperLink.Scope.Id, Is.EqualTo ("body_MySmartHyperLink"));

      var aspLinkButton = home.GetAnchor().ByID ("body_MyAspLinkButton");
      Assert.That (aspLinkButton.Scope.Id, Is.EqualTo ("body_MyAspLinkButton"));

      var aspHyperLink = home.GetAnchor().ByID ("body_MyAspHyperLink");
      Assert.That (aspHyperLink.Scope.Id, Is.EqualTo ("body_MyAspHyperLink"));

      var htmlAnchor = home.GetAnchor().ByID ("body_MyHtmlAnchor");
      Assert.That (htmlAnchor.Scope.Id, Is.EqualTo ("body_MyHtmlAnchor"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var htmlAnchor = home.GetAnchor().ByIndex (2);
      Assert.That (htmlAnchor.Scope.Id, Is.EqualTo ("body_MySmartHyperLink"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var webLinkButton = home.GetAnchor().ByLocalID ("MyWebLinkButton");
      Assert.That (webLinkButton.Scope.Id, Is.EqualTo ("body_MyWebLinkButton"));

      var smartHyperLink = home.GetAnchor().ByLocalID ("MySmartHyperLink");
      Assert.That (smartHyperLink.Scope.Id, Is.EqualTo ("body_MySmartHyperLink"));

      var aspLinkButton = home.GetAnchor().ByLocalID ("MyAspLinkButton");
      Assert.That (aspLinkButton.Scope.Id, Is.EqualTo ("body_MyAspLinkButton"));

      var aspHyperLink = home.GetAnchor().ByLocalID ("MyAspHyperLink");
      Assert.That (aspHyperLink.Scope.Id, Is.EqualTo ("body_MyAspHyperLink"));

      var htmlAnchor = home.GetAnchor().ByLocalID ("MyHtmlAnchor");
      Assert.That (htmlAnchor.Scope.Id, Is.EqualTo ("body_MyHtmlAnchor"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var htmlAnchor = home.GetAnchor().First();
      Assert.That (htmlAnchor.Scope.Id, Is.EqualTo ("body_MyWebLinkButton"));
    }

    [Test]
    public void TestSelection_Single ()
    {
      var home = Start();
      var scope = new ScopeControlObject ("scope", home.Context.CloneForScope (home.Scope.FindId ("scope")));

      var htmlAnchor = scope.GetAnchor().Single();
      Assert.That (htmlAnchor.Scope.Id, Is.EqualTo ("body_MyHtmlAnchor"));

      try
      {
        home.GetAnchor().Single();
        Assert.Fail ("Should not be able to unambigously find an HTML anchor.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestClick ()
    {
      var home = Start();

      var webLinkButton = home.GetAnchor().ByLocalID ("MyWebLinkButton");
      home = webLinkButton.Click().Expect<RemotionPageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("MyWebLinkButton|MyWebLinkButtonCommand"));

      var smartHyperLink = home.GetAnchor().ByLocalID ("MySmartHyperLink");
      home = smartHyperLink.Click().Expect<RemotionPageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.Empty);

      var aspLinkButton = home.GetAnchor().ByLocalID ("MyAspLinkButton");
      home = aspLinkButton.Click().Expect<RemotionPageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("MyAspLinkButton|MyAspLinkButtonCommand"));

      var aspHyperLink = home.GetAnchor().ByLocalID ("MyAspHyperLink");
      home = aspHyperLink.Click().Expect<RemotionPageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.Empty);

      var htmlAnchor = home.GetAnchor().ByLocalID ("MyHtmlAnchor");
      home = htmlAnchor.Click().Expect<RemotionPageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("MyHtmlAnchor"));
    }

    private RemotionPageObject Start ()
    {
      return Start ("AnchorTest.wxe");
    }
  }
}