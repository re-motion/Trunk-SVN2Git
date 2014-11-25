using System;
using ActaNova.WebTesting.ActaNovaExtensions;
using ActaNova.WebTesting.ControlObjects;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Development.WebTesting.FluentControlSelection;

namespace ActaNova.WebTesting.SampleTests
{
  [TestFixture]
  public class SampleTests : ActaNovaWebTestBase
  {
    [Test]
    public void SampleTest_Eingangsstueck_erzeugen ()
    {
      var date = DateTime.Now;

      var home = Start();

      var incomingPage = home.MainMenu.Neu_Eingangsstueck();
      
      incomingPage.FormPage.SetApplicationContextTo ("BW - Bauen und Wohnen");
      incomingPage.FormPage.GetTreeViewAutoComplete ("SubjectArea")
          .Select ("BW - Bauen und Wohnen", "WF - Wohnbau - Förderungen", "KS - Kleine Sanierung", "A - Kleine Sanierung - Zuschuss, Antrag");
      incomingPage.FormPage.GetDateTimeValue ("PostmarkDate").SetDate (date);
      incomingPage.FormPage.GetAutoComplete ("ActualSubmitter").FillWith ("Baier Anton (EG/3)");

      incomingPage.FormPage.SwitchTo ("SpecialdataFormPage_view");
      incomingPage.FormPage.GetDateTimeValue().ByDisplayName ("Fremddatum").SetDate (date);

      incomingPage.FormPage.SwitchTo ("SignaturesFormPage_view");
      var signaturesList = incomingPage.FormPage.GetList ("Signatures");
      Assert.That (signaturesList.GetNumberOfRows(), Is.EqualTo (0));

      incomingPage.FormPage.Perform ("Save");

      var signaturePopup = incomingPage.FormPage.Unterschreiben();
      signaturePopup.Sign ("Test1", "Meine Anmerkung");

      Assert.That (signaturesList.GetNumberOfRows(), Is.EqualTo (1));
      Assert.That (signaturesList.GetRow().WithIndex (1).GetCell ("SignatureAnnotation").GetText(), Is.EqualTo ("Meine Anmerkung"));

      home = incomingPage.FormPage.Perform ("SaveAndReturn").ExpectMainPage();

      var eigenerAv = home.WorkListPage.GetWorkList();
      Assert.That (eigenerAv.GetNumberOfRows(), Is.EqualTo (5));

      incomingPage = eigenerAv.GetRow().WithIndex (1).OpenWorkListItem();
      Assert.That (incomingPage.FormPage.GetTitle(), Is.StringStarting (string.Format ("Eingangsstück \"{0}/", date.ToShortDateString())));
      Assert.That (incomingPage.FormPage.GetTextValue("DisplayName").GetText(), Is.StringStarting (string.Format ("{0}", date.ToShortDateString())));
      incomingPage.FormPage.Perform ("Cancel").ExpectMainPage();

      var weiterleitenPopup = eigenerAv.GetRow()
          .WithIndex (1)
          .GetCell ("WorkSteps")
          .HoverAndGetListPopup()
          .ClickItem ("Weiterleiten")
          .ExpectNewActaNovaPopupWindow ("Aktivität weiterleiten");

      weiterleitenPopup.GetAutoComplete ("User").FillWith ("Pan Peter (EG/2)");
      weiterleitenPopup.Perform ("Save");

      Assert.That (eigenerAv.GetNumberOfRows(), Is.EqualTo (4));
    }
  }
}
