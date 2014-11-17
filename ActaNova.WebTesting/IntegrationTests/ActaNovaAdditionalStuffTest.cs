using System;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ActaNovaAdditionalStuffTest : ActaNovaWebTestBase
  {
    [Test]
    public void TestMultipleGridColumns ()
    {
      var home = Start();

      var editIncoming = home.WorkListPage.GetWorkList().GetCellWhere ("WorkItem", "04.04.2001/1").ExecuteCommand().ExpectMainPage();
      editIncoming.FormPage.GetOnlyTabbedMultiView().SwitchTo ("EditOwnedObjectSpecificSecurityFormPage_view");

      var typeEnum = editIncoming.FormPage.GetEnumValue ("VisibleObjectSpecificSecurityType");
      Assert.That (typeEnum.GetSelectedOption(), Is.EqualTo ("Standard"));

      var inheritBool = editIncoming.FormPage.GetBooleanValue ("InheritObjectSpecificSecurity");
      inheritBool.SetTo (false);
      typeEnum.SelectOption ("Extended");

      var visibleObjectSpecificSecurtiyEntries = editIncoming.FormPage.GetList ("VisibleObjectSpecificSecurityEntries");
      Assert.That (
          visibleObjectSpecificSecurtiyEntries.GetTopBlock().Scope.Text,
          Is.StringContaining ("Besitzer Muster Max, Ing. (EG/1) ist immer am Objekt berechtigt"));
      Assert.That (visibleObjectSpecificSecurtiyEntries.GetRowCount(), Is.EqualTo (1));

      editIncoming.FormPage.Perform ("Cancel", Continue.When (Wxe.PostBackCompletedIn (editIncoming)), HandleModalDialog.Accept());
    }
  }
}