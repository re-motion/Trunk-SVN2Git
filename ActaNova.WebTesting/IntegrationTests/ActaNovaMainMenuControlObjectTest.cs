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

      var forbiddenNote = home.MainMenu.Select (new[] { "Verfahrensbereich", "BW - Bauen und Wohnen" }, Opt.ContinueWhen (Wxe.PostBackCompleted))
          .Expect<ActaNovaMessageBoxPageObject>();
      forbiddenNote.Confirm (Opt.ContinueImmediately());

      home =
          aktErzeugenPage.FormPage.Perform ("Cancel", Opt.ContinueWhen (Wxe.PostBackCompletedIn (aktErzeugenPage)).AcceptModalDialog())
              .ExpectMainPage();

      home = home.MainMenu.Select ("Verfahrensbereich", "BW - Bauen und Wohnen").ExpectMainPage();

      home = home.MainMenu.Select ("Suchen", "Stammdatenobjekt", "Rechtsträger", "Organisation").ExpectMainPage();

      var administration =
          home.MainMenu.Select (new[] { "Extras", "Administration" }, Opt.ContinueWhen (Wxe.PostBackCompleted))
              .ExpectNewWindow<ActaNovaWindowPageObject> ("Administration");
      administration.Close();

      var wuensche = home.MainMenu.Select (new[] { "Extras", "Fehlerberichte/Wünsche" }, Opt.ContinueWhen (Wxe.PostBackCompleted))
          .ExpectNewWindow<ActaNovaWindowPageObject> ("Fehlerberichte/Wünsche");
      wuensche.Close();

      home = home.MainMenu.Select (new[] { "Extras", "Befehle vormerken" }, Opt.ContinueWhen (Wxe.PostBackCompleted)).ExpectMainPage();
      home = home.MainMenu.Select (new[] { "Extras", "Befehle sofort ausführen" }, Opt.ContinueWhen (Wxe.PostBackCompleted)).ExpectMainPage();

      home.Refresh();
    }
  }
}