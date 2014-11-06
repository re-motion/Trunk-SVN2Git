using System;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ActaNovaAutoCompleteReferenceValueControlObjectTest : ActaNovaWebTestBase
  {
    [Test]
    [Ignore("Fails until fixed WebButton DMA rendering is integrated into ActaNova.")]
    public void Test ()
    {
      var home = Start();

      home = home.MainMenu.Select ("Verfahrensbereich", "BA - Bürgeranliegen").ExpectMainPage();
      Assert.That (home.Header.GetCurrentApplicationContext(), Is.EqualTo ("Verfahrensbereich BA"));

      var createIncomingPage = home.MainMenu.Select ("Neu", "Eingangsstück").ExpectMainPage();

      var actualSubmitter = createIncomingPage.FormPage.GetAutoComplete ("ActualSubmitter");
      actualSubmitter.FillWith ("Baier Anton (EG/3)");
      Assert.That (actualSubmitter.GetText(), Is.EqualTo ("Baier Anton (EG/3)"));

      var showUserPage = actualSubmitter.Edit().ExpectMainPage();
      Assert.That (showUserPage.FormPage.GetTextValue ("LastName").GetText(), Is.EqualTo ("Baier"));
      createIncomingPage = showUserPage.FormPage.Perform ("Cancel").ExpectMainPage();

      actualSubmitter = createIncomingPage.FormPage.GetAutoComplete ("ActualSubmitter");
      actualSubmitter.Clear();
      Assert.That (actualSubmitter.GetText(), Is.EqualTo (""));

      actualSubmitter.GetSearchClassDropDown().SelectOption().WithText ("Person");
      var searchPage = actualSubmitter.Search().ExpectMainPage();
      Assert.That (searchPage.GetTitle(), Is.EqualTo ("Person suchen"));
      createIncomingPage = searchPage.FormPage.Perform ("Cancel").ExpectMainPage();

      actualSubmitter = createIncomingPage.FormPage.GetAutoComplete ("ActualSubmitter");
      var createPersonPage = actualSubmitter.New().ExpectMainPage();
      Assert.That (searchPage.GetTitle(), Is.EqualTo ("Person erzeugen"));
      createIncomingPage =
          createPersonPage.FormPage.Perform ("Cancel", Continue.When (Wxe.PostBackCompleted).AndModalDialogHasBeenAccepted()).ExpectMainPage();

      var applicationContext = createIncomingPage.FormPage.GetAutoComplete ("ApplicationContext");
      applicationContext.Clear();
      Assert.That (createIncomingPage.Header.GetCurrentApplicationContext(), Is.Null);
    }
  }
}