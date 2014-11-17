using System;
using ActaNova.WebTesting.PageObjects;
using NUnit.Framework;

namespace ActaNova.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ActaNovaTincyMceControlObjectTest : ActaNovaWebTestBase
  {
    [Test]
    public void TestGetTextAndGetMarkup ()
    {
      var home = Start();
      var page = GotoCreateMailPageWithTinyMce (home);

      var tinyMce = page.FormPage.GetTinyMce ("EmailBody");
      Assert.That(tinyMce.GetText(), Is.Empty);
      Assert.That(tinyMce.GetMarkup(), Is.EqualTo("<p><br data-mce-bogus=\"1\"></p>"));

      tinyMce.FillWithMarkup ("My<br>text<br>is<br>great.");

      Assert.That(tinyMce.GetText(), Is.EqualTo(string.Format ("My{0}text{0}is{0}great.", Environment.NewLine)));
      Assert.That(tinyMce.GetMarkup(), Is.EqualTo("<p>My<br>text<br>is<br>great.</p>"));
    }

    [Test]
    public void TestFillWithAndFillWithMarkup ()
    {
      var home = Start();
      var page = GotoCreateMailPageWithTinyMce (home);

      var tinyMce = page.FormPage.GetTinyMce ("EmailBody");

      tinyMce.FillWith (string.Format ("My{0}text{0}is{0}great.", Environment.NewLine));
      Assert.That(tinyMce.GetMarkup(), Is.EqualTo("<p>My<br>text<br>is<br>great.</p>"));

      tinyMce.FillWithMarkup ("My<br><strong>text</strong><br>is<br>great.");
      Assert.That(tinyMce.GetText(), Is.EqualTo(string.Format ("My{0}text{0}is{0}great.", Environment.NewLine)));
      Assert.That(tinyMce.GetMarkup(), Is.EqualTo("<p>My<br><strong>text</strong><br>is<br>great.</p>"));
    }

    private ActaNovaMainPageObject GotoCreateMailPageWithTinyMce (ActaNovaMainPageObject home)
    {
      var editIncomingPage = home.WorkListPage.GetWorkList().GetCellWhere ("WorkItem", "04.04.2001/1").ExecuteCommand().ExpectMainPage();
      editIncomingPage.FormPage.GetOnlyTabbedMultiView().SwitchTo ("IncomingEnclosuresFormPage_view");

      var documentsHierarchyList = editIncomingPage.FormPage.GetList ("DocumentsHierarchy");
      documentsHierarchyList.GetRow().WithIndex (1).ClickSelectCheckbox();
      var createMailPage = documentsHierarchyList.GetListMenu().SelectItem ("DocumentSendFinalContentCommand").ExpectMainPage();
      return createMailPage;
    }
  }
}