using System;
using ActaNova.WebTesting.ActaNovaExtensions;
using ActaNova.WebTesting.ControlObjects;
using ActaNova.WebTesting.PageObjects;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

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
      Assert.That (signaturesList.GetRow (1).GetCell ("SignatureAnnotation").GetText(), Is.EqualTo ("Meine Anmerkung"));

      home = incomingPage.FormPage.Perform ("SaveAndReturn").ExpectMainPage();

      var eigenerAv = home.WorkListPage.GetWorkList();
      Assert.That (eigenerAv.GetNumberOfRows(), Is.EqualTo (5));

      incomingPage = eigenerAv.GetRow (1).OpenWorkListItem();
      Assert.That (incomingPage.FormPage.GetTitle(), Is.StringStarting (string.Format ("Eingangsstück \"{0}/", date.ToShortDateString())));
      Assert.That (incomingPage.FormPage.GetTextValue ("DisplayName").GetText(), Is.StringStarting (string.Format ("{0}", date.ToShortDateString())));
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

    [Test]
    public void SampleTest_MainFrame ()
    {
      var home = Start();

      var incomingPage = home.MainMenu.Neu_Eingangsstueck();
      Assert.That (incomingPage.Header.GetNumberOfBreadCrumbs(), Is.EqualTo (2));
      Assert.That (incomingPage.Header.GetCurrentApplicationContext(), Is.Null);

      incomingPage.FormPage.SetApplicationContextTo ("BW - Bauen und Wohnen");
      Assert.That (incomingPage.Header.GetCurrentApplicationContext(), Is.EqualTo ("Verfahrensbereich BW"));

      home = incomingPage.Tree.GetGruppenAvNode().SelectAndConfirmDataLoss().ExpectMainPage();
      home.MainMenu.Verfahrensbereich_None();

      Assert.That (incomingPage.Header.GetNumberOfBreadCrumbs(), Is.EqualTo (1));
      Assert.That (incomingPage.Header.GetCurrentApplicationContext(), Is.Null);

      home = home.Tree.SelectEigenerAvNode();
      Assert.That (home.Header.GetBreadCrumb (1).GetText(), Is.EqualTo ("Eigener AV"));
    }

    [Test]
    public void SampleTest_Administration ()
    {
      var home = Start();

      var administration = home.MainMenu.Extras_Administration();
      administration.GetOnlyTabbedMenu().SelectItem ("ApplicationContextTab");

      var verfahrensbereicheList = administration.GetList().Single();
      Assert.That (verfahrensbereicheList.GetNumberOfRows(), Is.EqualTo (4));
    }

    [Test]
    public void SampleTest_DownLevelDms ()
    {
      const string fileName = "SampleFile.txt";

      var home = Start();

      var citizenConcernPage = home.WorkListPage.GetWorkList().GetRow (1).OpenWorkListItem();
      var incomingPage = citizenConcernPage.FormPage.NextActivity().ExpectMainPage();

      incomingPage.FormPage.SwitchTo ("IncomingEnclosuresFormPage_view");
      var documentsList = incomingPage.FormPage.GetList ("DocumentsHierarchy");
      var oldNumberOfDocuments = documentsList.GetNumberOfRows();

      documentsList.GetListMenu().SelectItem ("NewCommand");

      var dialog = incomingPage.FormPage.GetDialog();
      dialog.GetOnlyDownLevelDms().UploadFile ("SampleFile.txt");
      dialog.Perform ("TakeOverDetails");

      var newNumberOfDocuments = documentsList.GetNumberOfRows();
      Assert.That (newNumberOfDocuments, Is.EqualTo (oldNumberOfDocuments + 1));

      var newDocument = documentsList.GetRow (newNumberOfDocuments);

      var signaturePopup = newDocument.GetDropDownMenu()
          .SelectItem()
          .WithDisplayText ("Stornieren")
          .ExpectNewPopupWindow<ActaNovaSignaturePopupWindowPageObject> ("Unterschreiben");
      signaturePopup.Sign ("Test1");

      signaturePopup = newDocument.GetDropDownMenu()
          .SelectItem()
          .WithDisplayText ("Stornierung aufheben")
          .ExpectNewPopupWindow<ActaNovaSignaturePopupWindowPageObject> ("Unterschreiben");
      signaturePopup.Sign ("Test1");

      var downloadHelper = NewDownloadHelper (fileName);
      downloadHelper.AssertFileDoesNotExistYet();

      newDocument.GetCell ("IDocument").ExecuteCommand();

      downloadHelper.PerformDownload();
      downloadHelper.DeleteFile();
    }
  }
}