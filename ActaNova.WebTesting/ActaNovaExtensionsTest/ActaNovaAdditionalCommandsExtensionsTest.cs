using System;
using ActaNova.WebTesting.ActaNovaExtensions;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.ActaNovaExtensionsTest
{
  [TestFixture]
  public class ActaNovaAdditionalCommandsExtensionsTest : ActaNovaWebTestBase
  {
    [Test]
    public void Test ()
    {
      var home = Start();

      var editCitizenConcern = home.WorkListPage.GetWorkList().GetCellWhere ("WorkItem", "04.06.2009/1").OpenWorkListItem();

      var signaturePopup = editCitizenConcern.FormPage.Abschliessen();
      signaturePopup.Cancel();

      var searchFile = editCitizenConcern.FormPage.Protokollieren();
      editCitizenConcern = searchFile.FormPage.Perform ("Cancel").ExpectMainPage();

      signaturePopup = editCitizenConcern.FormPage.Stornieren();
      signaturePopup.Cancel();

      signaturePopup = editCitizenConcern.FormPage.Unterschreiben();
      signaturePopup.Cancel();

      signaturePopup = editCitizenConcern.FormPage.Heranholen();
      signaturePopup.Cancel();

      editCitizenConcern = editCitizenConcern.FormPage.BarcodeDrucken();

      var createOutgoingMail = editCitizenConcern.FormPage.MailVersenden();
      editCitizenConcern = createOutgoingMail.FormPage.PerformAndConfirmDataLoss ("Cancel").ExpectMainPage();

      signaturePopup = editCitizenConcern.FormPage.Sperren();
      signaturePopup.Cancel();

      var saveAndForward = editCitizenConcern.FormPage.SpeichernUndWeiterleiten();
      editCitizenConcern = saveAndForward.FormPage.Perform ("Cancel", Continue.When(Wxe.PostBackCompleted)).ExpectMainPage();

      var print = editCitizenConcern.FormPage.Gesamtdruck();
      editCitizenConcern = print.FormPage.Perform ("Cancel").ExpectMainPage();

      var ediaktExportWindow = editCitizenConcern.FormPage.ExportierenNachEDIAKT();
      ediaktExportWindow.Perform ("Cancel");

      var dialog = editCitizenConcern.FormPage.BesitzUebernehmen();
      dialog.Perform ("CancelDetails");

      dialog = editCitizenConcern.FormPage.BesitzUebergeben();
      dialog.Perform ("CancelDetails");

      var editSecurtiyInheritance = editCitizenConcern.FormPage.SicherheitsvererbungBearbeiten();
      editCitizenConcern = editSecurtiyInheritance.FormPage.Perform ("Cancel", Continue.When(Wxe.PostBackCompleted)).ExpectMainPage();

      editCitizenConcern.FormPage.NextActivity();
      editCitizenConcern.FormPage.NextActivity();

      signaturePopup = editCitizenConcern.FormPage.Umprotokollieren();
      signaturePopup.Cancel();
    }
  }
}