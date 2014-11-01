using System;
using Coypu;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class FormGridControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var formGrid = home.GetFormGrid().ByID ("body_My1FormGrid");
      Assert.That (formGrid.Scope.Text, Is.StringContaining ("Content1"));
      Assert.That (formGrid.Scope.Text, Is.Not.StringContaining ("DoNotFindMe1"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var formGrid = home.GetFormGrid().ByIndex (2);
      Assert.That (formGrid.Scope.Text, Is.StringContaining ("Content2"));
      Assert.That (formGrid.Scope.Text, Is.Not.StringContaining ("DoNotFindMe2"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var formGrid = home.GetFormGrid().ByLocalID ("My1FormGrid");
      Assert.That (formGrid.Scope.Text, Is.StringContaining ("Content1"));
      Assert.That (formGrid.Scope.Text, Is.Not.StringContaining ("DoNotFindMe1"));
    }

    [Test]
    public void TestSelection_ByTitle ()
    {
      var home = Start();

      var formGrid = home.GetFormGrid().ByTitle ("MyFormGrid2");
      Assert.That (formGrid.Scope.Text, Is.StringContaining ("Content2"));
      Assert.That (formGrid.Scope.Text, Is.Not.StringContaining ("DoNotFindMe2"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var formGrid = home.GetFormGrid().First();
      Assert.That (formGrid.Scope.Text, Is.StringContaining ("Content1"));
      Assert.That (formGrid.Scope.Text, Is.Not.StringContaining ("DoNotFindMe1"));
    }

    [Test]
    public void TestSelection_Single ()
    {
      var home = Start();
      var scope = new ScopeControlObject (home.Context.CloneForControl (home.Scope.FindId ("scope")));

      var formGrid = scope.GetFormGrid().Single();
      Assert.That (formGrid.Scope.Text, Is.StringContaining ("Content2"));
      Assert.That (formGrid.Scope.Text, Is.Not.StringContaining ("DoNotFindMe2"));

      try
      {
        home.GetFormGrid().Single();
        Assert.Fail ("Should not be able to unambigously find a form grid.");
      }
      catch (AmbiguousException)
      {
      }
    }

    private RemotionPageObject Start ()
    {
      return Start ("FormGridTest.aspx");
    }
  }
}