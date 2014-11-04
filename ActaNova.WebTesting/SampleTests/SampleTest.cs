using System;
using ActaNova.WebTesting.Infrastructure;
using ActaNova.WebTesting.IntegrationTests;
using ActaNova.WebTesting.PageObjects;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.SampleTests
{
  [TestFixture]
  public class SampleTest : ActaNovaWebTestBase
  {
    [Test]
    public void MySampleTest ()
    {
      var home = Start();

      // Todo RM-6297: MainMenu.Select() should allow overridden IActionBehavior
      var newCitizenConcernPage = home.MainMenu.Select ("Neu", "Bürgeranliegen").Expect<ActaNovaMainPageObject>();

      //newCitizenConcernPage.DetailsArea.GetControl (
      //    new PerHtmlIDControlSelectionCommand<BocAutoCompleteReferenceValueControlObject> (
      //        new BocAutoCompleteReferenceValueSelector(),
      //        "CitizenConcernFormPage_view_LazyContainer_ctl01_ObjectFormPageDataSource_ApplicationContext"))
      //    .FillWith ("BA - Bürgeranliegen", Continue.When (WaitFor.WxePostBackIn (home)));

      newCitizenConcernPage.DetailsArea.GetControl (
          new PerDomainPropertyControlSelectionCommand<BocAutoCompleteReferenceValueControlObject> (
              new BocAutoCompleteReferenceValueSelector(),
              "ApplicationContext")).FillWith ("BA - Bürgeranliegen", Continue.When (Wxe.PostBackCompletedIn (home)));

      home = newCitizenConcernPage.DetailsArea.Perform (
          "Cancel",
          Continue.When (Wxe.PostBackCompletedIn (newCitizenConcernPage)).AndModalDialogHasBeenAccepted()).Expect<ActaNovaMainPageObject>();

      Assert.That (home.Header.GetCurrentApplicationContext(), Is.EqualTo ("Verfahrensbereich BA"));
      Assert.That (home.Header.GetBreadCrumbs().Count, Is.EqualTo (1));
      Assert.That (home.Header.GetBreadCrumbs()[0].GetText(), Is.EqualTo ("Eigener AV"));

      home.MainMenu.Select ("Verfahrensbereich", "Kein Verfahrensbereich");

      Assert.That (home.Header.GetCurrentApplicationContext(), Is.Null);

      // Todo RM-6297: MainMenu.Select() should allow overridden IActionBehavior
      newCitizenConcernPage = home.MainMenu.Select ("Neu", "Bürgeranliegen").Expect<ActaNovaMainPageObject>();

      Assert.That (home.Header.GetBreadCrumbs().Count, Is.EqualTo (2));

      var confirmPage = newCitizenConcernPage.Header.GetBreadCrumbs()[0].Click (Continue.When (Wxe.PostBackCompleted))
          .Expect<MessageBoxPageObject>();
      home = confirmPage.Confirm().Expect<ActaNovaMainPageObject>();

      Assert.That (home.Header.GetBreadCrumbs().Count, Is.EqualTo (1));
    }
  }
}