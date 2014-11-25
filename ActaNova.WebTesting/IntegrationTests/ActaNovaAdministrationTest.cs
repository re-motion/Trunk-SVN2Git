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
      tabbedMenu.SubMenu.SelectItem ("StandardClassificationTypes");

      var classificationTypeField = administration.GetOnlyFormGrid().GetDropDownList().Single();
      classificationTypeField.SelectOption().WithDisplayText ("Adressart");

      var classificationTypeList = administration.GetList().Single();
      Assert.That (classificationTypeList.GetNumberOfRows(), Is.EqualTo (4));

      var downloadNotification = administration.GetImageButton ("ExcelExportButton").Click().Expect<ActaNovaMessageBoxPageObject>();
      downloadNotification.Confirm();

      administration.Close();

      var tempExportDokumente = home.MainMenu.Select ("Extras", "Temp. Export Dokumente").ExpectMainPage();
      var itemsList = tempExportDokumente.FormPage.GetList ("Items");
      Assert.That (itemsList.GetNumberOfRows(), Is.EqualTo (1));

      var deletionConfirmation = itemsList.GetRow (1).GetCell (2).ExecuteCommand().Expect<ActaNovaMessageBoxPageObject>();
      deletionConfirmation.Yes();

      Assert.That (itemsList.GetNumberOfRows(), Is.EqualTo (0));
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
      tabbedMenu.SubMenu.SelectItem ("AccessControlSubMenuTab");

      var securableClassesTree = administration.GetTreeView ("DerivedClasses");

      var permissions = securableClassesTree.GetRootNode()
          .GetNode()
          .WithIndex (1)
          .Click()
          .ExpectNewWindow<ActaNovaWindowPageObject> ("Berechtigungen");

      var objectPermissions = permissions.GetScope().ByID ("MainContentPlaceHolder_UpdatePanel_1");
      objectPermissions.GetWebButton ("ToggleAccessControlEntryButton").Click();
      objectPermissions.GetAutoComplete ("SpecificAbstractRole").FillWith ("Beim Objekt nur lesend berechtigt");
      permissions.Perform ("Save", Continue.When (Wxe.PostBackCompletedInContext (permissions.Context.ParentContext)));

      permissions = securableClassesTree.GetRootNode().GetNode().WithIndex (1).Click().ExpectNewWindow<ActaNovaWindowPageObject> ("Berechtigungen");

      objectPermissions = permissions.GetScope().ByID ("MainContentPlaceHolder_UpdatePanel_1");
      objectPermissions.GetWebButton ("ToggleAccessControlEntryButton").Click();
      objectPermissions.GetAutoComplete ("SpecificAbstractRole").FillWith ("Standard");
      permissions.Perform ("Save", Continue.When (Wxe.PostBackCompletedInContext (permissions.Context.ParentContext)));

      administration.Close();
    }
  }
}