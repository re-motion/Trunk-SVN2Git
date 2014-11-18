using System;
using ActaNova.WebTesting.PageObjects;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ActaNovaFormPageObjectTest : ActaNovaWebTestBase
  {
    [Test]
    public void Test ()
    {
      var home = Start();

      var editIncomingPage = home.WorkListPage.GetWorkList().GetCellWhere ("WorkItem", "04.04.2001/1").ExecuteCommand().ExpectMainPage();
      Assert.That (editIncomingPage.FormPage.GetTitle(), Is.EqualTo ("Eingangsstück \"04.04.2001/1\" bearbeiten"));
      Assert.That (editIncomingPage.FormPage.GetCurrentActivityName(), Is.EqualTo ("AV-Element prüfen und verteilen"));

      editIncomingPage = editIncomingPage.FormPage.PreviousActivity().ExpectMainPage();
      Assert.That (editIncomingPage.FormPage.GetTitle(), Is.EqualTo ("Bürgeranliegen \"04.06.2009/1\" bearbeiten"));
      Assert.That (editIncomingPage.FormPage.GetCurrentActivityName(), Is.EqualTo ("Bürgeranliegen prüfen und bearbeiten"));

      editIncomingPage = editIncomingPage.FormPage.NextActivity().ExpectMainPage();
      Assert.That (editIncomingPage.FormPage.GetTitle(), Is.EqualTo ("Eingangsstück \"04.04.2001/1\" bearbeiten"));
      Assert.That (editIncomingPage.FormPage.GetCurrentActivityName(), Is.EqualTo ("AV-Element prüfen und verteilen"));

      var weiterleitenWindow = editIncomingPage.FormPage.HoverWorkStepsControl()
          .ClickItem ("Weiterleiten")
          .ExpectNewPopupWindow<ActaNovaPopupWindowPageObject> ("Aktivität weiterleiten");
      weiterleitenWindow.Perform ("Cancel", Continue.When (Wxe.PostBackCompletedInContext(weiterleitenWindow.Context.ParentContext)));

      editIncomingPage.FormPage.PressPinButton();
      Assert.That (editIncomingPage.Tree.GetNode().WithDisplayText ("Favoriten").Expand().GetNumberOfChildren(), Is.EqualTo (3));

      editIncomingPage.FormPage.PressPinButton();
      Assert.That (editIncomingPage.Tree.GetNode().WithDisplayText ("Favoriten").Expand().GetNumberOfChildren(), Is.EqualTo (4));

      var permalink = editIncomingPage.FormPage.GetPermalink();

      var printWindow = editIncomingPage.FormPage.Print();
      printWindow.Perform ("CancelDetails", Continue.When (Wxe.PostBackCompletedInContext(printWindow.Context.ParentContext)));

      // Todo EVB-8268: Enable as soon as the bug report window does not show a yellow page anymore.
      //var bugReportWindow = editIncomingPage.FormPage.CreateBugReport();
      //bugReportWindow.Close();

      // Todo EVB-8268: Enable as soon as the help window does have a title.
      //var helpPage = editIncomingPage.FormPage.OpenHelp();
      //helpPage.Close();

      var createMailPage = editIncomingPage.FormPage.GetAdditionalCommandsMenu().SelectItem().WithDisplayText ("Mail versenden").ExpectMainPage();
      editIncomingPage =
          createMailPage.FormPage.Perform ("Cancel", Continue.When (Wxe.PostBackCompletedIn (createMailPage)), HandleModalDialog.Accept())
              .ExpectMainPage();

      editIncomingPage.FormPage.Perform ("Cancel");

      using (var browser2 = CreateNewBrowser())
      {
        var home2 = StartAgain<ActaNovaMainPageObject> (browser2, permalink);
        Assert.That (home2.GetTitle(), Is.EqualTo ("Eingangsstück \"04.04.2001/1\" bearbeiten"));
      }
    }
  }
}