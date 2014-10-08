using System;
using ActaNova.WebTesting.PageObjects;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlSelection;

namespace ActaNova.WebTesting.SampleTests
{
  [TestFixture]
  public class SampleTest : ActaNovaWebTestBase
  {
    [Test]
    public void MySampleTest ()
    {
      var home = Start();

      var newCitizenConcernPage = home.MainMenu.Select ("New", "Citizen concern").Expect<ActaNovaMainPageObject>();

      newCitizenConcernPage.DetailsArea.GetControl (
          new PerDomainPropertyControlSelectionCommand<BocAutoCompleteReferenceValueControlObject> (
              new BocAutoCompleteReferenceValueSelector(),
              "ApplicationContext")).FillWith ("BA - Bürgeranliegen");


    }
  }
}