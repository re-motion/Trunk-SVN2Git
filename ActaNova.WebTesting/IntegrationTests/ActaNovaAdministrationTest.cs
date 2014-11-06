using System;
using ActaNova.WebTesting.PageObjects;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace ActaNova.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ActaNovaAdministrationTest : ActaNovaWebTestBase
  {
    [Test]
    public void TestClassificationTypesTab ()
    {
      var home = Start();

      var administration =
          home.MainMenu.Select (new[] { "Extras", "Administration" }, Continue.When (Wxe.PostBackCompleted))
              .ExpectNewWindow<ActaNovaWindowPageObject> ("Administration");

      var tabbedMenu = administration.GetOnlyTabbedMenu();
      tabbedMenu.SelectSubItem ("StandardClassificationTypes");

      var classificationTypeField = administration.GetOnlyFormGrid().GetDropDownList().Single();
      classificationTypeField.SelectOption().WithText ("Adressart");

      var classificationTypeList = administration.GetList().Single();
      Assert.That (classificationTypeList.GetRowCount(), Is.EqualTo (4));

      var downloadNotification = administration.ClickImage ("ExcelExport").Expect<ActaNovaMessageBoxPageObject>();
      downloadNotification.Confirm();
      
      administration.Close();

      var tempExportDokumente = home.MainMenu.Select ("Extras", "Temp. Export Dokumente").ExpectActaNova();
      var itemsList = tempExportDokumente.FormPage.GetList ("Items");
      Assert.That (itemsList.GetRowCount(), Is.EqualTo (1));

      itemsList.GetRow().WithIndex (1).GetCell().WithIndex (2).ExecuteCommand().ExpectActaNova (ActaNovaMessageBox.Yes);
      Assert.That (itemsList.GetRowCount(), Is.EqualTo (0));
    }

    [Test]
    public void TestSecurityTab ()
    {
      var home = Start();

      var administration =
          home.MainMenu.Select (new[] { "Extras", "Administration" }, Continue.When (Wxe.PostBackCompleted))
              .ExpectNewWindow<ActaNovaWindowPageObject> ("Administration");

      var tabbedMenu = administration.GetOnlyTabbedMenu();
      tabbedMenu.SelectItem ("SecurityTab");
      tabbedMenu.SelectSubItem ("AccessControlSubMenuTab");

      var securableClassesTree = administration.GetTreeView ("DerivedClasses");

      var permissions = securableClassesTree.GetRootNode()
          .GetNode()
          .WithIndex (1)
          .Click()
          .ExpectNewWindow<ActaNovaWindowPageObject> ("Berechtigungen");

      var objectPermissions = permissions.GetScope().ByID ("MainContentPlaceHolder_UpdatePanel_1");
      objectPermissions.GetWebButton ("ToggleAccessControlEntryButton").Click();
      objectPermissions.GetAutoComplete ("SpecificAbstractRole").FillWith ("Entitled for read only of object");
      permissions.Perform ("Save", Continue.When (Wxe.PostBackCompleted).AndWindowHasClosed());

      permissions = securableClassesTree.GetRootNode().GetNode().WithIndex (1).Click().ExpectNewWindow<ActaNovaWindowPageObject> ("Berechtigungen");

      objectPermissions = permissions.GetScope().ByID ("MainContentPlaceHolder_UpdatePanel_1");
      objectPermissions.GetWebButton ("ToggleAccessControlEntryButton").Click();
      objectPermissions.GetAutoComplete ("SpecificAbstractRole").FillWith ("Standard");
      permissions.Perform ("Save", Continue.When (Wxe.PostBackCompleted).AndWindowHasClosed());

      administration.Close();
    }
  }
}