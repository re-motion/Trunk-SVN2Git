﻿using System;
using ActaNova.WebTesting.ControlObjects;
using Coypu;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace ActaNova.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ActaNovaListTest : ActaNovaWebTestBase
  {
    [Test]
    public void TestGetTopBlock ()
    {
      var home = Start();

      var editIncoming = home.WorkListPage.GetWorkList().GetCellWhere ("WorkItem", "04.04.2001/1").ExecuteCommand().ExpectMainPage();
      editIncoming.FormPage.GetOnlyTabbedMultiView().SwitchTo ("SpecialdataFormPage_view");

      var referencesList = editIncoming.FormPage.GetList().ByDisplayName ("Referenzliste");
      Assert.That (referencesList.GetTopBlock().GetAutoComplete ("Value").Scope.Exists(), Is.True);

      Assert.That (referencesList.Scope.Text, Is.StringContaining ("Unterstützung"));
      Assert.That (referencesList.GetTopBlock().Scope.Text, Is.Not.StringContaining ("Unterstützung"));
    }

    [Test]
    public void TestGetTopRightAlignedBlock ()
    {
      var home = Start();

      var editIncoming = home.WorkListPage.GetWorkList().GetCellWhere ("WorkItem", "04.04.2001/1").ExecuteCommand().ExpectMainPage();
      editIncoming.FormPage.GetOnlyTabbedMultiView().SwitchTo ("IncomingEnclosuresFormPage_view");

      var documentsList = editIncoming.FormPage.GetList ("DocumentsHierarchy");
      Assert.That (documentsList.GetTopRightAlignedBlock().GetReferenceValue().ByLocalID ("DocumentTemplates").Scope.Exists(), Is.True);

      Assert.That (documentsList.Scope.Text, Is.StringContaining ("Sample Email.txt"));
      Assert.That (documentsList.GetTopRightAlignedBlock().Scope.Text, Is.Not.StringContaining ("Sample Email.txt"));
    }

    [Test]
    public void TestFilter ()
    {
      var home = Start();

      var activityList = home.WorkListPage.GetWorkList();

      activityList.Filter ("Geschäftsfall");
      Assert.That (activityList.GetRowCount(), Is.EqualTo (2));

      activityList.ClearFilter();
      Assert.That (activityList.GetRowCount(), Is.EqualTo (4));
    }

    [Test]
    public void TestOpenColumnConfiguration ()
    {
      var home = Start();

      var editIncoming = home.WorkListPage.GetWorkList().GetCellWhere ("WorkItem", "04.04.2001/1").ExecuteCommand().ExpectMainPage();
      editIncoming.FormPage.GetOnlyTabbedMultiView().SwitchTo ("SpecialdataFormPage_view");

      var columnConfiguration = editIncoming.FormPage.GetList().ByDisplayName ("Referenzliste").OpenColumnConfiguration().ExpectMainPage();
      columnConfiguration.FormPage.Perform ("Close");

      editIncoming.FormPage.GetList().ByDisplayName ("Referenzliste").SetPreferredView();
    }

    [Test]
    public void TestChangeViewTo ()
    {
      var home = Start();

      var editIncoming = home.WorkListPage.GetWorkList().GetCellWhere ("WorkItem", "04.04.2001/1").ExecuteCommand().ExpectMainPage();
      editIncoming.FormPage.GetOnlyTabbedMultiView().SwitchTo ("SpecialdataFormPage_view");

      editIncoming.FormPage.GetList().ByDisplayName ("Referenzliste").ChangeViewToByLabel ("Standard", Continue.Immediately());
    }

    [Test]
    public void TestHierarchyList ()
    {
      var home = Start();

      var editIncoming = home.WorkListPage.GetWorkList().GetCellWhere ("WorkItem", "04.04.2001/1").ExecuteCommand().ExpectMainPage();
      editIncoming.FormPage.GetOnlyTabbedMultiView().SwitchTo ("IncomingEnclosuresFormPage_view");

      var documentsList = editIncoming.FormPage.GetList ("DocumentsHierarchy");
      documentsList.GetCellWhere().ColumnWithIndexContains (3, "Sample Email.txt").ExpandHierarchyRow();

      Assert.That (documentsList.GetCellWhere().ColumnWithIndexContains (3, "Sample Email Attachment.doc").Scope.Exists(), Is.True);
    }

    [Test]
    public void TestHoverAndGetTreePopup ()
    {
      var home = Start();

      var popup = home.WorkListPage.GetWorkList().GetRowWhere ("WorkItem", "OE-W/1/BW-WH-WD/1").GetCell("DocumentsHierarchy").HoverAndGetTreePopup();

      Assert.That (popup.GetNode ("Reinschrift").GetText(), Is.EqualTo ("Reinschrift.pdf")); // yep, contains match.
      Assert.That (popup.GetNode ("Reinschrift.pdf").GetText(), Is.EqualTo ("Reinschrift.pdf"));

      popup = home.WorkListPage.GetWorkList().GetRowWhere ("WorkItem", "OE/2/BW-BV-BA-M/1").GetCell("DocumentsHierarchy").HoverAndGetTreePopup();
      Assert.That (popup.GetNode("Keine Einträge.").GetText(), Is.EqualTo("Keine Einträge."));
    }
  }
}