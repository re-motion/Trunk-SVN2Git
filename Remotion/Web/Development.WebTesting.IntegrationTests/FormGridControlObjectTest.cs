using System;
using Coypu;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class FormGridControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelector_WithParameters ()
    {
      var home = Start("FormGridTest.aspx");

      var formGrid1 = home.GetControl (new FormGridSelector(), new ControlSelectionParameters { ID = "My1FormGrid" });
      Assert.That (formGrid1.Scope.Text, Is.StringContaining ("FindMe1"));
      Assert.That (formGrid1.Scope.Text, Is.Not.StringContaining ("DoNotFindMe1"));

      var formGrid2 = home.GetControl (new FormGridSelector(), new ControlSelectionParameters { Index = 2 });
      Assert.That (formGrid2.Scope.Text, Is.StringContaining ("FindMe2"));
      Assert.That (formGrid2.Scope.Text, Is.Not.StringContaining ("DoNotFindMe2"));

      // Todo RM-6297: Change title to "MyFormGrid2" as soon as the FormGrid data-title rendering is fixed.
      formGrid2 = home.GetControl (new FormGridSelector(), new ControlSelectionParameters { Title = "My2FormGrid" });
      Assert.That (formGrid2.Scope.Text, Is.StringContaining ("FindMe2"));
      Assert.That (formGrid2.Scope.Text, Is.Not.StringContaining ("DoNotFindMe2"));
    }

    [Test]
    public void TestSelector_WithoutParameters ()
    {
      var home = Start("SingleFormGridTest.aspx");

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var formGrid = home.GetControl (new FormGridSelector(), new ControlSelectionParameters());
      Assert.That (formGrid.Scope.Text, Is.StringContaining ("FindMe"));
      Assert.That (formGrid.Scope.Text, Is.Not.StringContaining ("DoNotFindMe"));

      home = Start ("FormGridTest.aspx");
      try
      {
        home.GetControl (new FormGridSelector(), new ControlSelectionParameters());
      }
      catch(AmbiguousException)
      {
        return;
      }

      Assert.Fail ("Should not be able to unambigously find a form grid.");
    }
  }
}