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
    public void TestActaNovaTree ()
    {
      var home = Start();

      var eigenerAv = home.Tree.GetNode (1);
      Assert.That (eigenerAv.GetText(), Is.EqualTo ("Eigener AV"));

      home = home.Tree.GetNode (2).Expand().GetNodeByText ("egora Gemeinde").Select().Expect<ActaNovaMainPageObject>();
      Assert.That (home.DetailsArea.FormPageTitle, Is.EqualTo ("egora Gemeinde AV"));

      var geschaeftsfall = home.Tree.GetNodeByText ("Favoriten").Expand().Collapse().Expand().GetNode (2);
      home = geschaeftsfall.Select().Expect<ActaNovaMainPageObject>();
      Assert.That (home.DetailsArea.FormPageTitle, Is.EqualTo ("Geschäftsfall \"OE/1/BW-BV-BA-M/1\" bearbeiten"));

      home =
          geschaeftsfall.GetNode ("Files")
              .Expand()
              .GetNode (1)
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

      // Todo RM-6297: MainMenu.Select() should allow overridden IActionBehavior
      home = home.MainMenu.Select ("Extras", "Benutzerprofil").Expect<ActaNovaMainPageObject>();
      home = home.Refresh().Expect<ActaNovaMainPageObject>();

      Assert.That (home.GetTitle(), Is.EqualTo ("Eigener AV"));
    }

    [Test]
    public void TestActaNovaHeader ()
    {
      var home = Start();

      Assert.That (home.Header.CurrentUser, Is.EqualTo ("Muster Max, Ing."));
      Assert.That (home.Header.CurrentGroup, Is.EqualTo ("EG/1"));
      Assert.That (home.Header.CurrentApplicationContext, Is.Null);
    }

    [Test]
    public void TestActaNovaHeaderOpenDefaultGroupControl ()
    {
      var home = Start();

      var defaultGroupControl = home.Header.OpenDefaultGroupControlWhenStandardIsDisplayed();
      Assert.That (defaultGroupControl.GetText(), Is.Empty);

      defaultGroupControl.SelectOptionByText ("Kanzlei (Kanzlei)");

      defaultGroupControl = home.Header.OpenDefaultGroupControl();
      Assert.That (defaultGroupControl.GetText(), Is.EqualTo ("Kanzlei (Kanzlei)"));

      defaultGroupControl.SelectOption (1);

      defaultGroupControl = home.Header.OpenDefaultGroupControlWhenStandardIsDisplayed();
      Assert.That (defaultGroupControl.GetText(), Is.Empty);
    }

    [Test]
    public void TestActaNovaHeaderOpenCurrentTenantControl ()
    {
      var home = Start();

      var currentTenantControl = home.Header.OpenCurrentTenantControl();
      Assert.That (currentTenantControl.GetText(), Is.EqualTo ("Acta Nova Gemeinde"));

      currentTenantControl.SelectOptionByText ("Acta Nova Ortsteil 1");

      currentTenantControl = home.Header.OpenCurrentTenantControl();
      Assert.That (currentTenantControl.GetText(), Is.EqualTo ("Acta Nova Ortsteil 1"));

      currentTenantControl.SelectOption (1);

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