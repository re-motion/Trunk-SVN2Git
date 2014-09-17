using System;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class TabStripControlObjectTest : IntegrationTest
  {
    [Test]
    public void Test ()
    {
      var home = Start("TabStripTest.wxe");

      var tabStrip1 = home.GetControl (new TabStripSelector(), new ControlSelectionParameters { ID = "MyTabStrip1" });
      Assert.That (tabStrip1.Scope.Id, Is.EqualTo("MyTabStrip1"));

      var tabStrip2 = home.GetControl (new TabStripSelector(), new ControlSelectionParameters { Index = 2 });
      Assert.That (tabStrip2.Scope.Id, Is.EqualTo("MyTabStrip2"));

      var scope = new ScopeControlObject ("scope", home.Context.CloneForScope(home.Scope.FindId ("scope")));
      tabStrip2 = scope.GetControl (new TabStripSelector(), new ControlSelectionParameters());
      Assert.That (tabStrip2.Scope.Id, Is.EqualTo("MyTabStrip2"));

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