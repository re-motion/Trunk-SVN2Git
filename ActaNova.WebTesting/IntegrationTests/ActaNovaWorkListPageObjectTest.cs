using ActaNova.WebTesting.PageObjects;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ActaNovaWorkListPageObjectTest : ActaNovaWebTestBase
  {
    [Test]
    public void Test ()
    {
      var home = Start();

      // Todo EVB-8268: Enable as soon as the RSS feed window does have a title.
      //var rssFeedWindow = home.WorkListPage.OpenRssFeed();
      //rssFeedWindow.Close();

      // Todo EVB-8268: Enable as soon as the help window does have a title.
      //var helpPage = home.WorkListPage.OpenHelp();
      //helpPage.Close();

      // Todo EVB-8268: Enable as soon as the bug report window does not show a yellow page anymore.
      //var bugReportWindow = home.WorkListPage.CreateBugReport();
      //bugReportWindow.Close();

      var workList = home.WorkListPage.GetWorkList();
      workList.GetRowWhere ("WorkItem", "04.06.2009/1").ClickSelectCheckbox();

      var weiterleitenPopup = home.WorkListPage.GetWorkStepMenu()
          .SelectItem()
          .WithDisplayText ("Weiterleiten")
          .ExpectNewPopupWindow<ActaNovaPopupWindowPageObject> ("Aktivität weiterleiten");
      weiterleitenPopup.Perform ("Cancel");
    }
  }
}