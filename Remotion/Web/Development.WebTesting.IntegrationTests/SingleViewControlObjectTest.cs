using System;
using Coypu;
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
      Assert.That (singleView.Scope.Text, Is.StringContaining ("FindMe"));
      Assert.That (singleView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));

      singleView = home.GetControl (new SingleViewSelector(), new ControlSelectionParameters { Index = 1 });
      Assert.That (singleView.Scope.Text, Is.StringContaining ("FindMe"));
      Assert.That (singleView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));

      singleView = home.GetControl (new SingleViewSelector(), new ControlSelectionParameters());
      Assert.That (singleView.Scope.Text, Is.StringContaining ("FindMe"));
      Assert.That (singleView.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));
    }
  }
}