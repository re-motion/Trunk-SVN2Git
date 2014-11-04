using System;
using ActaNova.WebTesting.Infrastructure;
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
    public void TestActaNovaTree ()
    {
      var home = Start();

      var eigenerAv = home.Tree.GetNode().WithIndex (1);
      Assert.That (eigenerAv.GetText(), Is.EqualTo ("Eigener AV"));

      home = home.Tree.GetNode().WithIndex (2).Expand().GetNode().WithText ("egora Gemeinde").Select().Expect<ActaNovaMainPageObject>();
      Assert.That (home.DetailsArea.FormPageTitle, Is.EqualTo ("egora Gemeinde AV"));

      var geschaeftsfall = home.Tree.GetNode().WithText ("Favoriten").Expand().Collapse().Expand().GetNode().WithIndex (2);
      home = geschaeftsfall.Select().Expect<ActaNovaMainPageObject>();
      Assert.That (home.DetailsArea.FormPageTitle, Is.EqualTo ("Geschäftsfall \"OE/1/BW-BV-BA-M/1\" bearbeiten"));

      home =
          geschaeftsfall.GetNode ("Files")
              .Expand()
              .GetNode().WithIndex (1)
              .Expand()
              .GetNode ("WrappedDocumentsHierarchy")
              .Expand()
              .Select()
              .Expect<ActaNovaMainPageObject>();
      Assert.That (home.DetailsArea.FormPageTitle, Is.EqualTo ("Akt \"OE/1\" bearbeiten"));
    }


    [Test]
    public void TestRefresh ()
    {
      var home = Start();

      if (home.DetailsArea.FormPageTitle != "Eigener AV")
        home.Tree.GetNode().WithText ("Eigener AV").Select();

      // Todo RM-6297: MainMenu.Select() should allow overridden IActionBehavior
      home = home.MainMenu.Select ("Extras", "Benutzerprofil").Expect<ActaNovaMainPageObject>();
      home = home.Refresh().Expect<ActaNovaMainPageObject>();

      Assert.That (home.GetTitle(), Is.EqualTo ("Eigener AV"));
    }

    [Test]
    public void TestActaNovaHeader ()
    {
      var home = Start();

      Assert.That (home.Header.GetCurrentUser(), Is.EqualTo ("Muster Max, Ing."));
      Assert.That (home.Header.GetCurrentGroup(), Is.EqualTo ("EG/1"));
      Assert.That (home.Header.GetCurrentApplicationContext(), Is.Null);
    }

    [Test]
    public void TestActaNovaHeaderOpenDefaultGroupControl ()
    {
      var home = Start();

      var defaultGroupControl = home.Header.OpenDefaultGroupControlWhenStandardIsDisplayed();
      Assert.That (defaultGroupControl.GetText(), Is.Empty);

      defaultGroupControl.SelectOption().WithText ("Kanzlei (Kanzlei)");

      defaultGroupControl = home.Header.OpenDefaultGroupControl();
      Assert.That (defaultGroupControl.GetText(), Is.EqualTo ("Kanzlei (Kanzlei)"));

      defaultGroupControl.SelectOption().WithIndex (1);

      defaultGroupControl = home.Header.OpenDefaultGroupControlWhenStandardIsDisplayed();
      Assert.That (defaultGroupControl.GetText(), Is.Empty);
    }

    [Test]
    public void TestActaNovaHeaderOpenCurrentTenantControl ()
    {
      var home = Start();

      var currentTenantControl = home.Header.OpenCurrentTenantControl();
      Assert.That (currentTenantControl.GetText(), Is.EqualTo ("Acta Nova Gemeinde"));

      currentTenantControl.SelectOption().WithText ("Acta Nova Ortsteil 1", Continue.When(ActaNovaCompletion.OuterInnerOuterUpdated));

      currentTenantControl = home.Header.OpenCurrentTenantControl();
      Assert.That (currentTenantControl.GetText(), Is.EqualTo ("Acta Nova Ortsteil 1"));

      currentTenantControl.SelectOption().WithIndex (1, Continue.When (ActaNovaCompletion.OuterInnerOuterUpdated));

      currentTenantControl = home.Header.OpenCurrentTenantControl();
      Assert.That (currentTenantControl.GetText(), Is.EqualTo ("Acta Nova Gemeinde"));
    }

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
      Assert.That (home.Header.GetBreadCrumbs()[0].Text, Is.EqualTo ("Eigener AV"));

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