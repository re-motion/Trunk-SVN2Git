using System;
using Coypu;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class TabStripControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start ("TabStripTest.wxe");

      var tabStrip = home.GetTabStrip().ByID ("body_MyTabStrip1");
      Assert.That (tabStrip.Scope.Id, Is.EqualTo ("body_MyTabStrip1"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start ("TabStripTest.wxe");

      var tabStrip = home.GetTabStrip().ByIndex (2);
      Assert.That (tabStrip.Scope.Id, Is.EqualTo ("body_MyTabStrip2"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start ("TabStripTest.wxe");

      var tabStrip = home.GetTabStrip().ByLocalID ("MyTabStrip1");
      Assert.That (tabStrip.Scope.Id, Is.EqualTo ("body_MyTabStrip1"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start ("TabStripTest.wxe");

      var tabStrip = home.GetTabStrip().First();
      Assert.That (tabStrip.Scope.Id, Is.EqualTo ("body_MyTabStrip1"));
    }

    [Test]
    public void TestSelection_Single ()
    {
      var home = Start ("TabStripTest.wxe");
      var scope = new ScopeControlObject ("scope", home.Context.CloneForScope (home.Scope.FindId ("scope")));

      var tabStrip = scope.GetTabStrip().Single();
      Assert.That (tabStrip.Scope.Id, Is.EqualTo ("body_MyTabStrip2"));

      try
      {
        home.GetTabStrip().Single();
        Assert.Fail ("Should not be able to unambigously find a tab strip.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestSwitchTo ()
    {
      var home = Start ("TabStripTest.wxe");

      var tabStrip1 = home.GetTabStrip().First();
      var tabStrip2 = home.GetTabStrip().ByIndex (2);

      home = tabStrip1.SwitchTo ("Tab2").Expect<RemotionPageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("MyTabStrip1/Tab2"));

      home = tabStrip1.SwitchToByLabel ("Tab1Label").Expect<RemotionPageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("MyTabStrip1/Tab1"));

      home = tabStrip2.SwitchTo ("Tab2").Expect<RemotionPageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("MyTabStrip2/Tab2"));

      home = tabStrip2.SwitchToByLabel ("Tab1Label").Expect<RemotionPageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("MyTabStrip2/Tab1"));
    }
  }
}