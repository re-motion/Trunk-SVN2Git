using System;
using ActaNova.WebTesting.PageObjects;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ActaNovaMessageBoxControlObjectTest : ActaNovaWebTestBase
  {
    [Test]
    public void TestOkay ()
    {
      var home = Start();

      var createCitizenConcernPage = home.MainMenu.Select ("Neu", "Bürgeranliegen").ExpectMainPage();

      Assert.That (home.GetTitle(), Is.EqualTo ("Bürgeranliegen erzeugen"));

      var cancelConfirmation =
          createCitizenConcernPage.Header.GetBreadCrumb(1).Click (Opt.ContinueWhen (Wxe.PostBackCompleted)).Expect<ActaNovaMessageBoxPageObject>();
      cancelConfirmation.Confirm();

      Assert.That (home.GetTitle(), Is.EqualTo ("Eigener AV"));
    }

    [Test]
    public void TestCancel ()
    {
      var home = Start();

      var createCitizenConcernPage = home.MainMenu.Select ("Neu", "Bürgeranliegen").ExpectMainPage();

      Assert.That (home.GetTitle(), Is.EqualTo ("Bürgeranliegen erzeugen"));

      var cancelConfirmation =
          createCitizenConcernPage.Header.GetBreadCrumb(1).Click (Opt.ContinueWhen (Wxe.PostBackCompleted)).Expect<ActaNovaMessageBoxPageObject>();
      cancelConfirmation.Cancel();

      Assert.That (home.GetTitle(), Is.EqualTo ("Bürgeranliegen erzeugen"));
    }
  }
}