using System;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocTreeViewAutoCompleteReferenceValueControlObjectTest : ActaNovaWebTestBase
  {
    [Test]
    public void Test ()
    {
      var home = Start();

      var createIncoming = home.MainMenu.Select ("Neu", "Eingangsstück").ExpectMainPage();
      createIncoming.FormPage.GetAutoComplete ("ApplicationContext").FillWith ("BW - Bauen und Wohnen");
      createIncoming.FormPage.GetTreeViewAutoComplete ("SubjectArea")
          .Select ("WF - Wohnbau - Förderungen", "KS - Kleine Sanierung", "A - Kleine Sanierung - Zuschuss, Antrag");

      createIncoming.FormPage.GetOnlyTabbedMultiView().SwitchTo ("SpecialdataFormPage_view");

      // If the tree view auto complete selection didn't work, this is going to fail!
      createIncoming.FormPage.GetDateTimeValue().ByDisplayName ("Fremddatum").SetDate (DateTime.Now);

      createIncoming.FormPage.GetOnlyTabbedMultiView().SwitchTo ("BaseIncomingFormPage_view");
      Assert.That (
          createIncoming.FormPage.GetTreeViewAutoComplete ("SubjectArea").GetText(),
          Is.EqualTo ("BW-WF-KS-A - Kleine Sanierung - Zuschuss, Antrag"));

      createIncoming.FormPage.Perform ("Cancel", Continue.When (Wxe.PostBackCompletedIn (createIncoming)).AndModalDialogHasBeenAccepted())
          .ExpectMainPage();
    }

    [Test]
    public void TestFillWith ()
    {
      var home = Start();

      var createIncoming = home.MainMenu.Select ("Neu", "Eingangsstück").ExpectMainPage();
      createIncoming.FormPage.GetAutoComplete ("ApplicationContext").FillWith ("BW - Bauen und Wohnen");
      createIncoming.FormPage.GetTreeViewAutoComplete ("SubjectArea").FillWith ("A - Kleine Sanierung - Zuschuss, Antrag");

      createIncoming.FormPage.GetOnlyTabbedMultiView().SwitchTo ("SpecialdataFormPage_view");

      // If the tree view auto complete selection didn't work, this is going to fail!
      createIncoming.FormPage.GetDateTimeValue().ByDisplayName ("Fremddatum").SetDate (DateTime.Now);

      createIncoming.FormPage.GetOnlyTabbedMultiView().SwitchTo ("BaseIncomingFormPage_view");
      Assert.That (
          createIncoming.FormPage.GetTreeViewAutoComplete ("SubjectArea").GetText(),
          Is.EqualTo ("BW-WF-KS-A - Kleine Sanierung - Zuschuss, Antrag"));

      createIncoming.FormPage.Perform ("Cancel", Continue.When (Wxe.PostBackCompletedIn (createIncoming)).AndModalDialogHasBeenAccepted())
          .ExpectMainPage();
    }
  }
}