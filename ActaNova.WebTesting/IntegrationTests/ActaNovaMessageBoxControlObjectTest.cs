using System;
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

      var newCitizenConcernPage = home.MainMenu.Select ("Neu", "Bürgeranliegen").ExpectActaNova();

      Assert.That (home.GetTitle(), Is.EqualTo ("Bürgeranliegen erzeugen"));

      home = newCitizenConcernPage.Header.GetBreadCrumbs()[0].Click(Continue.When(Wxe.PostBackCompleted)).ExpectActaNova (ActaNovaMessageBox.Okay);

      Assert.That (home.GetTitle(), Is.EqualTo ("Eigener AV"));
    }

    [Test]
    public void TestCancel ()
    {
      var home = Start();

      var newCitizenConcernPage = home.MainMenu.Select ("Neu", "Bürgeranliegen").ExpectActaNova();

      Assert.That (home.GetTitle(), Is.EqualTo ("Bürgeranliegen erzeugen"));

      home = newCitizenConcernPage.Header.GetBreadCrumbs()[0].Click(Continue.When(Wxe.PostBackCompleted)).ExpectActaNova (ActaNovaMessageBox.Cancel);

      Assert.That (home.GetTitle(), Is.EqualTo ("Bürgeranliegen erzeugen"));
    }
  }
}