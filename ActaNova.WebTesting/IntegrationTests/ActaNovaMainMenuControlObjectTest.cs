using System;
using ActaNova.WebTesting.PageObjects;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ActaNovaMainMenuControlObjectTest : ActaNovaWebTestBase
  {
    [Test]
    public void Test ()
    {
      var home = Start();

      var aktErzeugenPage = home.MainMenu.Select ("Neu", "Akt").ExpectMainPage();
      Assert.That (aktErzeugenPage.GetTitle(), Is.EqualTo ("Akt erzeugen"));

      var forbiddenNote = home.MainMenu.Select (new[] { "Verfahrensbereich", "BW - Bauen und Wohnen" }, Continue.When (Wxe.PostBackCompleted))
          .Expect<ActaNovaMessageBoxPageObject>();
      forbiddenNote.Confirm (Continue.Immediately());

      home = aktErzeugenPage.FormPage.Perform (
          "Cancel",
          Continue.When (Wxe.PostBackCompletedIn (aktErzeugenPage)),
          HandleModalDialog.Accept())
          .ExpectMainPage();

      home = home.MainMenu.Select ("Verfahrensbereich", "BW - Bauen und Wohnen").ExpectMainPage();

      home = home.MainMenu.Select ("Suchen", "Stammdatenobjekt", "Rechtsträger", "Organisation").ExpectMainPage();

      var administration =
          home.MainMenu.Select (new[] { "Extras", "Administration" }, Continue.When (Wxe.PostBackCompleted))
              .ExpectNewWindow<ActaNovaWindowPageObject> ("Administration");
      administration.Close();

      var wuensche = home.MainMenu.Select (new[] { "Extras", "Fehlerberichte/Wünsche" }, Continue.When (Wxe.PostBackCompleted))
          .ExpectNewWindow<ActaNovaWindowPageObject> ("Fehlerberichte/Wünsche");
      wuensche.Close();

      home = home.MainMenu.Select (new[] { "Extras", "Befehle vormerken" }, Continue.When (Wxe.PostBackCompleted)).ExpectMainPage();
      home = home.MainMenu.Select (new[] { "Extras", "Befehle sofort ausführen" }, Continue.When (Wxe.PostBackCompleted)).ExpectMainPage();

      home.Refresh();
    }
  }
}