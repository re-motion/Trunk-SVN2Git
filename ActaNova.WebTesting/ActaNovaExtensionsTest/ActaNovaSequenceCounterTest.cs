using System;
using ActaNova.WebTesting.ActaNovaExtensions;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.ActaNovaExtensionsTest
{
  [TestFixture]
  public class ActaNovaSequenceCounterTest : ActaNovaWebTestBase
  {
    [Test]
    public void TestOuterFrameUsageAfterTabbedMultiViewTabSwitch ()
    {
      var home = Start();

      var editCitizenConcern = home.WorkListPage.GetWorkList().GetCellWhere ("WorkItem", "04.06.2009/1").OpenWorkListItem();
      Assert.That (home.Header.GetNumberOfBreadCrumbs(), Is.EqualTo (2));

      editCitizenConcern.FormPage.SwitchTo ("IncomingEnclosuresFormPage_view");

      editCitizenConcern.Tree.GetNode().WithDisplayText ("Gruppen AV").Expand();

      editCitizenConcern.Tree.GetNode()
          .WithDisplayText ("04.06.2009/1")
          .GetNode ("ActualSubmitters")
          .Expand();

      editCitizenConcern.Tree.GetNode().WithDisplayText ("Gruppen AV").Collapse();
    }

    [Test]
    public void TestOuterFrameUsageAfterAdditionalCommandsMenuSelect ()
    {
      var home = Start();

      var editCitizenConcern = home.WorkListPage.GetWorkList().GetCellWhere ("WorkItem", "04.06.2009/1").OpenWorkListItem();
      Assert.That (home.Header.GetNumberOfBreadCrumbs(), Is.EqualTo (2));

      var createMail = editCitizenConcern.FormPage.GetAdditionalCommandsMenu()
          .SelectItem()
          .WithDisplayText ("Mail versenden", Continue.When (Wxe.PostBackCompletedInParent(editCitizenConcern)))
          .ExpectMainPage();

      Assert.That (home.Header.GetNumberOfBreadCrumbs(), Is.EqualTo (3));
      
      createMail.Header.GetBreadCrumb (3).Click();
      createMail.Header.GetBreadCrumb (1).ClickAndPreventDataLoss();

      Assert.That (home.Header.GetNumberOfBreadCrumbs(), Is.EqualTo (3));
    }

    [Test]
    public void TestOuterFrameUsageAfterPerform ()
    {
      var home = Start();

      var editCitizenConcern = home.WorkListPage.GetWorkList().GetCellWhere ("WorkItem", "04.06.2009/1").OpenWorkListItem();
      home = editCitizenConcern.FormPage.Perform ("Cancel").ExpectMainPage();

      Assert.That (home.Header.GetNumberOfBreadCrumbs(), Is.EqualTo (1));
    }

    [Test]
    public void TestOuterFrameUsageAfterNewApplicationContext ()
    {
      var home = Start();

      var createIncoming = home.MainMenu.Neu_Eingangsstueck();
      createIncoming.FormPage.SetApplicationContextTo ("BW - Bauen und Wohnen");

      Assert.That (createIncoming.Header.GetCurrentApplicationContext(), Is.EqualTo ("Verfahrensbereich BW"));
    }
  }
}