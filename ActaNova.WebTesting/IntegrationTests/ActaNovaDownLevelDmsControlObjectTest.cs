using System;
using System.IO;
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
      var filePath = Path.GetFullPath (fileName);

      var home = Start();

      var editPage = home.FormPage.GetList ("Activities").GetRow().WithIndex (1).GetCell ("WorkItem").ExecuteCommand().ExpectMainPage();
      editPage.FormPage.GetOnlyTabbedMultiView().SwitchTo ("IncomingEnclosuresFormPage_view");
      var documentsHierarchyList = editPage.FormPage.GetList ("DocumentsHierarchy");
      var rowCount = documentsHierarchyList.GetRowCount();
      documentsHierarchyList.GetListMenu().SelectItem ("NewCommand");

      var dialog = editPage.FormPage.GetDialog();
      dialog.GetTextValue ("Name").FillWith ("MyName");
      dialog.GetOnlyDownLevelDms().UploadFile (filePath);
      dialog.Perform ("TakeOverDetails");

      Assert.That (documentsHierarchyList.GetRowCount(), Is.EqualTo(rowCount + 1));
    }
  }
}