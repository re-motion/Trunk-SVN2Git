using System;
using ActaNova.WebTesting.PageObjects;
using NUnit.Framework;

namespace ActaNova.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ActaNovaSignaturePopupWindowPageObjectTest : ActaNovaWebTestBase
  {
    [Test]
    public void Test ()
    {
      var home = Start();

      var editIncomingPage = home.WorkListPage.GetWorkList().GetCellWhere ("WorkItem", "04.04.2001/1").ExecuteCommand().ExpectMainPage();

      var signaturePopup = editIncomingPage.FormPage.GetAdditionalCommandsMenu()
          .SelectItem()
          .WithDisplayText ("Heranholen")
          .ExpectNewPopupWindow<ActaNovaSignaturePopupWindowPageObject> ("Unterschreiben");

      signaturePopup.Cancel();

      signaturePopup = editIncomingPage.FormPage.GetAdditionalCommandsMenu()
          .SelectItem()
          .WithDisplayText ("Heranholen")
          .ExpectNewPopupWindow<ActaNovaSignaturePopupWindowPageObject> ("Unterschreiben");

      var annotation = Guid.NewGuid().ToString();
      var signPage = signaturePopup.Sign ("Test1", annotation);
      editIncomingPage = signPage.FormPage.Perform ("SaveAndReturn").ExpectMainPage();

      editIncomingPage.FormPage.GetOnlyTabbedMultiView().SwitchTo ("SignaturesFormPage_view");
      var signaturesList = editIncomingPage.FormPage.GetList ("Signatures");
      var rowCount = signaturesList.GetNumberOfRows();
      var signatureAnnotationCell = signaturesList.GetRow (rowCount).GetCell ("SignatureAnnotation");
      Assert.That (signatureAnnotationCell.GetText(), Is.EqualTo (annotation));
    }

    [Test]
    public void TestBatchSign ()
    {
      var home = Start();

      var editIncomingPage = home.WorkListPage.GetWorkList().GetCellWhere ("WorkItem", "04.04.2001/1").ExecuteCommand().ExpectMainPage();

      var signaturePopup = editIncomingPage.FormPage.GetAdditionalCommandsMenu()
          .SelectItem()
          .WithDisplayText ("Unterschreiben")
          .ExpectNewPopupWindow<ActaNovaSignaturePopupWindowPageObject> ("Unterschreiben");

      var annotation = Guid.NewGuid().ToString();
      editIncomingPage = signaturePopup.BatchSign ("Test1", annotation);

      var batchedCommands = editIncomingPage.MainMenu.Select ("Extras", "Vorgemerkte Befehle").ExpectMainPage();
      var batchedCommandsList = batchedCommands.FormPage.GetList ("BatchedCommands");
      batchedCommandsList.GetListMenu().SelectItem ("DeleteAllExecuted");
      Assert.That (batchedCommandsList.GetNumberOfRows(), Is.EqualTo (1));
      Assert.That (batchedCommandsList.GetRow (1).GetCell ("BatchedCommandState").GetText(), Is.EqualTo ("Vorgemerkt"));

      batchedCommandsList.GetRow (1).ClickSelectCheckbox();
      var confirmationPage = batchedCommandsList.GetListMenu().SelectItem ("Execute").Expect<ActaNovaMessageBoxPageObject>();
      signaturePopup = confirmationPage.Yes().ExpectNewPopupWindow<ActaNovaSignaturePopupWindowPageObject> ("Unterschreiben");
      signaturePopup.Sign ("Test1");

      Assert.That (batchedCommandsList.GetRow (1).GetCell ("BatchedCommandState").GetText(), Is.EqualTo ("Ausgeführt"));
    }
  }
}