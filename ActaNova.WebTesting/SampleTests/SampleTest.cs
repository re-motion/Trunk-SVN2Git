using System;
using ActaNova.WebTesting.PageObjects;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

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
      //    .FillWith ("BA - Bürgeranliegen", Behavior.WaitFor (WaitFor.WxePostBackIn (home)));

      newCitizenConcernPage.DetailsArea.GetControl (
          new PerDomainPropertyControlSelectionCommand<BocAutoCompleteReferenceValueControlObject> (
              new BocAutoCompleteReferenceValueSelector(),
              "ApplicationContext")).FillWith ("BA - Bürgeranliegen", Behavior.WaitFor (WaitFor.WxePostBackIn (home)));

      home =
          newCitizenConcernPage.DetailsArea.Perform ("Cancel", Behavior.AcceptModalDialog().WaitFor (WaitFor.WxePostBackIn (newCitizenConcernPage)))
              .Expect<ActaNovaMainPageObject>();

      Assert.That (home.Header.CurrentApplicationContext, Is.EqualTo ("Verfahrensbereich BA"));
      Assert.That (home.Header.BreadCrumbs.Count, Is.EqualTo (1));
      Assert.That (home.Header.BreadCrumbs[0].Text, Is.EqualTo ("Eigener AV"));

      home.MainMenu.Select ("Verfahrensbereich", "Kein Verfahrensbereich");

      Assert.That (home.Header.CurrentApplicationContext, Is.Null);

      // Todo RM-6297: MainMenu.Select() should allow overridden IActionBehavior
      newCitizenConcernPage = home.MainMenu.Select ("Neu", "Bürgeranliegen").Expect<ActaNovaMainPageObject>();

      Assert.That (home.Header.BreadCrumbs.Count, Is.EqualTo (2));

      var confirmPage = newCitizenConcernPage.Header.BreadCrumbs[0].Click (Behavior.WaitFor (WaitFor.WxePostBack))
          .Expect<ActaNovaMessageBoxPageObject>();
      home = confirmPage.Confirm().Expect<ActaNovaMainPageObject>();
      
      Assert.That (home.Header.BreadCrumbs.Count, Is.EqualTo (1));
    }
  }
}