﻿using System;
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
      Assert.That (formGrid1.Scope.Text, Is.StringContaining ("Content1"));
      Assert.That (formGrid1.Scope.Text, Is.Not.StringContaining ("DoNotFindMe1"));

      var formGrid2 = home.GetControl (new FormGridSelector(), new ControlSelectionParameters { Index = 2 });
      Assert.That (formGrid2.Scope.Text, Is.StringContaining ("Content2"));
      Assert.That (formGrid2.Scope.Text, Is.Not.StringContaining ("DoNotFindMe2"));

      formGrid2 = home.GetControl (new FormGridSelector(), new ControlSelectionParameters { Title = "MyFormGrid2" });
      Assert.That (formGrid2.Scope.Text, Is.StringContaining ("Content2"));
      Assert.That (formGrid2.Scope.Text, Is.Not.StringContaining ("DoNotFindMe2"));
    }

    [Test]
    public void TestSelector_WithoutParameters ()
    {
      var home = Start("FormGridSingleTest.aspx");

      Assert.That (home.Scope.Text, Is.StringContaining ("DoNotFindMe"));

      var formGrid = home.GetControl (new FormGridSelector(), new ControlSelectionParameters());
      Assert.That (formGrid.Scope.Text, Is.StringContaining ("Content"));
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