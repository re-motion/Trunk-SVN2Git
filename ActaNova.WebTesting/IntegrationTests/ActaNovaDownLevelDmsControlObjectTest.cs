using System;
using NUnit.Framework;

namespace ActaNova.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ActaNovaDownLevelDmsControlObjectTest : ActaNovaWebTestBase
  {
    [Test]
    public void Test ()
    {
      const string fileName = "SampleFile.txt";

      var home = Start();

      var editPage = home.FormPage.GetList ("Activities").GetRow (1).GetCell ("WorkItem").ExecuteCommand().ExpectMainPage();
      editPage.FormPage.GetOnlyTabbedMultiView().SwitchTo ("IncomingEnclosuresFormPage_view");
      var documentsHierarchyList = editPage.FormPage.GetList ("DocumentsHierarchy");
      var rowCount = documentsHierarchyList.GetNumberOfRows();
      documentsHierarchyList.GetListMenu().SelectItem ("NewCommand");

      var dialog = editPage.FormPage.GetDialog();
      dialog.GetTextValue ("Name").FillWith ("MyName");
      dialog.GetOnlyDownLevelDms().UploadFile (fileName);
      dialog.Perform ("TakeOverDetails");

      Assert.That (documentsHierarchyList.GetNumberOfRows(), Is.EqualTo (rowCount + 1));
    }
  }
}