using System;
using Coypu;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class WebTabStripControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var tabStrip = home.GetWebTabStrip().ByID ("body_MyTabStrip1");
      Assert.That (tabStrip.Scope.Id, Is.EqualTo ("body_MyTabStrip1"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var tabStrip = home.GetWebTabStrip().ByIndex (2);
      Assert.That (tabStrip.Scope.Id, Is.EqualTo ("body_MyTabStrip2"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var tabStrip = home.GetWebTabStrip().ByLocalID ("MyTabStrip1");
      Assert.That (tabStrip.Scope.Id, Is.EqualTo ("body_MyTabStrip1"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var tabStrip = home.GetWebTabStrip().First();
      Assert.That (tabStrip.Scope.Id, Is.EqualTo ("body_MyTabStrip1"));
    }

    [Test]
    public void TestSelection_Single ()
    {
      var home = Start();
      var scope = new ScopeControlObject (home.Context.CloneForControl (home, home.Scope.FindId ("scope")));

      var tabStrip = scope.GetWebTabStrip().Single();
      Assert.That (tabStrip.Scope.Id, Is.EqualTo ("body_MyTabStrip2"));

      try
      {
        home.GetWebTabStrip().Single();
        Assert.Fail ("Should not be able to unambigously find a tab strip.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestSwitchTo ()
    {
      var home = Start();

      var tabStrip1 = home.GetWebTabStrip().First();
      var tabStrip2 = home.GetWebTabStrip().ByIndex (2);

      home = tabStrip1.SwitchTo ("Tab2").Expect<RemotionPageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("MyTabStrip1/Tab2"));

      home = tabStrip1.SwitchTo().WithText ("Tab1Label").Expect<RemotionPageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("MyTabStrip1/Tab1"));

      home = tabStrip2.SwitchTo().WithIndex (2).Expect<RemotionPageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("MyTabStrip2/Tab2"));

      home = tabStrip2.SwitchTo().WithHtmlID ("body_MyTabStrip2_Tab1").Expect<RemotionPageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("MyTabStrip2/Tab1"));
    }

    private RemotionPageObject Start ()
    {
      return Start("WebTabStripTest.wxe");
    }
  }
}