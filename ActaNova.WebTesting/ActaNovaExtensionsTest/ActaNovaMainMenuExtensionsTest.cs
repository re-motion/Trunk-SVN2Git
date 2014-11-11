using System;
using ActaNova.WebTesting.ActaNovaExtensions;
using NUnit.Framework;

namespace ActaNova.WebTesting.ActaNovaExtensionsTest
{
  [TestFixture]
  public class ActaNovaMainMenuExtensionsTest : ActaNovaWebTestBase
  {
    [Test]
    public void TestActaNovaMainMenuExtensions_Neu ()
    {
      var home = Start();

      home = home.MainMenu.Neu_Akt();
      Assert.That (home.GetTitle(), Is.EqualTo ("Akt erzeugen"));

      home = home.MainMenu.Neu_Buergeranliegen();
      Assert.That (home.GetTitle(), Is.EqualTo ("Bürgeranliegen erzeugen"));

      home = home.MainMenu.Neu_Eingangsstueck();
      Assert.That (home.GetTitle(), Is.EqualTo ("Eingangsstück erzeugen"));

      home = home.MainMenu.Neu_Entwurf();
      Assert.That (home.GetTitle(), Is.EqualTo ("Entwurf erzeugen"));

      home = home.MainMenu.Neu_Geschaeftsfall();
      Assert.That (home.GetTitle(), Is.EqualTo ("Geschäftsfall erzeugen"));

      home = home.MainMenu.Neu_Organisation();
      Assert.That (home.GetTitle(), Is.EqualTo ("Organisation erzeugen"));

      home = home.MainMenu.Neu_Organisationsverteiler();
      Assert.That (home.GetTitle(), Is.EqualTo ("Organisationsverteiler erzeugen"));

      home = home.MainMenu.Neu_Person();
      Assert.That (home.GetTitle(), Is.EqualTo ("Person erzeugen"));

      home = home.MainMenu.Neu_VerteilerStatisch();
      Assert.That (home.GetTitle(), Is.EqualTo ("Verteiler (statisch) erzeugen"));

      home.Header.GetBreadCrumbs()[0].ClickAndConfirmDataLoss();
      Assert.That (home.GetTitle(), Is.EqualTo ("Eigener AV"));
    }

    [Test]
    public void TestActaNovaMainMenuExtensions_Verfahrensbereich ()
    {
      var home = Start();

      home = home.MainMenu.Verfahrensbereich_BA();
      Assert.That (home.Header.GetCurrentApplicationContext(), Is.EqualTo ("Verfahrensbereich BA"));

      home = home.MainMenu.Neu_VerteilerDynamisch();
      Assert.That (home.GetTitle(), Is.EqualTo ("Verteiler (dynamisch) erzeugen"));

      home.Header.GetBreadCrumbs()[0].ClickAndConfirmDataLoss();
      Assert.That (home.GetTitle(), Is.EqualTo ("Eigener AV"));

      home = home.MainMenu.Verfahrensbereich_BW();
      Assert.That (home.Header.GetCurrentApplicationContext(), Is.EqualTo ("Verfahrensbereich BW"));

      home = home.MainMenu.Verfahrensbereich_GS();
      Assert.That (home.Header.GetCurrentApplicationContext(), Is.EqualTo ("Verfahrensbereich GS"));

      home = home.MainMenu.Verfahrensbereich_SU();
      Assert.That (home.Header.GetCurrentApplicationContext(), Is.EqualTo ("Verfahrensbereich SU"));

      home = home.MainMenu.Verfahrensbereich_None();
      Assert.That (home.Header.GetCurrentApplicationContext(), Is.Null);
    }

    [Test]
    public void TestActaNovaMainMenuExtensions_Suche ()
    {
      var home = Start();

      home = home.MainMenu.Verfahrensbereich_BA();
      Assert.That (home.Header.GetCurrentApplicationContext(), Is.EqualTo ("Verfahrensbereich BA"));

      home = home.MainMenu.Suchen_Aufgabe();
      Assert.That (home.GetTitle(), Is.EqualTo ("Aufgabe suchen"));

      home = home.MainMenu.Suchen_Termin();
      Assert.That (home.GetTitle(), Is.EqualTo ("Termin suchen"));

      home = home.MainMenu.Suchen_FehlerberichtWunsch();
      Assert.That (home.GetTitle(), Is.EqualTo ("Fehlerbericht/Wunsch suchen"));

      home = home.MainMenu.Suchen_Akt();
      Assert.That (home.GetTitle(), Is.EqualTo ("Akt suchen"));

      home = home.MainMenu.Suchen_Buergeranliegen();
      Assert.That (home.GetTitle(), Is.EqualTo ("Bürgeranliegen suchen"));

      home = home.MainMenu.Suchen_Eingangsstueck();
      Assert.That (home.GetTitle(), Is.EqualTo ("Eingangsstück suchen"));

      home = home.MainMenu.Suchen_Entwurf();
      Assert.That (home.GetTitle(), Is.EqualTo ("Entwurf suchen"));

      home = home.MainMenu.Suchen_Geschaeftsfall();
      Assert.That (home.GetTitle(), Is.EqualTo ("Geschäftsfall suchen"));

      home = home.MainMenu.Suchen_AusgehendeEmail();
      Assert.That (home.GetTitle(), Is.EqualTo ("Ausgehende E-Mail suchen"));

      home = home.MainMenu.Suchen_EingehendeEmail();
      Assert.That (home.GetTitle(), Is.EqualTo ("Eingehende E-Mail suchen"));

      home = home.MainMenu.Suchen_EDIAKTExportDokument();
      Assert.That (home.GetTitle(), Is.EqualTo ("EDIAKT Export Dokument suchen"));

      home = home.MainMenu.Suchen_Erledigung();
      Assert.That (home.GetTitle(), Is.EqualTo ("Erledigung suchen"));

      home = home.MainMenu.Suchen_InternerEingang();
      Assert.That (home.GetTitle(), Is.EqualTo ("Interner Eingang suchen"));

      home = home.MainMenu.Suchen_Rueckschein();
      Assert.That (home.GetTitle(), Is.EqualTo ("Rückschein suchen"));

      home = home.MainMenu.Suchen_Organisationsverteiler();
      Assert.That (home.GetTitle(), Is.EqualTo ("Organisationsverteiler suchen"));

      home = home.MainMenu.Suchen_Adresse();
      Assert.That (home.GetTitle(), Is.EqualTo ("Adresse suchen"));

      home = home.MainMenu.Suchen_Email();
      Assert.That (home.GetTitle(), Is.EqualTo ("E-Mail suchen"));

      home = home.MainMenu.Suchen_Grundstueck();
      Assert.That (home.GetTitle(), Is.EqualTo ("Grundstück suchen"));

      home = home.MainMenu.Suchen_Information();
      Assert.That (home.GetTitle(), Is.EqualTo ("Information suchen"));

      home = home.MainMenu.Suchen_Organisation();
      Assert.That (home.GetTitle(), Is.EqualTo ("Organisation suchen"));

      home = home.MainMenu.Suchen_Person();
      Assert.That (home.GetTitle(), Is.EqualTo ("Person suchen"));

      home = home.MainMenu.Suchen_Telefon();
      Assert.That (home.GetTitle(), Is.EqualTo ("Telefon suchen"));

      home.Header.GetBreadCrumbs()[0].Click();
      Assert.That (home.GetTitle(), Is.EqualTo ("Eigener AV"));

      home = home.MainMenu.Suchen_WebSeite();
      Assert.That (home.GetTitle(), Is.EqualTo ("Web-Seite suchen"));

      home = home.MainMenu.Suchen_VerteilerDynamisch();
      Assert.That (home.GetTitle(), Is.EqualTo ("Verteiler (dynamisch) suchen"));

      home = home.MainMenu.Suchen_VerteilerStatisch();
      Assert.That (home.GetTitle(), Is.EqualTo ("Verteiler (statisch) suchen"));
    }

    [Test]
    public void TestActaNovaMainMenuExtensions_Extras ()
    {
      var home = Start();

      home = home.MainMenu.Extras_PasswortAendern();
      Assert.That (home.GetTitle(), Is.EqualTo ("Passwort ändern"));

      var administrationWindow = home.MainMenu.Extras_Administration();
      Assert.That (administrationWindow.GetTitle(), Is.StringStarting ("ACTA NOVA Kommunal"));
      Assert.That (administrationWindow.GetTitle(), Is.StringEnding ("Administration"));
      administrationWindow.Close();

      home = home.MainMenu.Extras_Benutzerprofil();
      Assert.That (home.GetTitle(), Is.EqualTo ("Benutzerprofil"));

      home = home.MainMenu.Extras_Aktivitaetsprotokoll();
      Assert.That (home.GetTitle(), Is.EqualTo ("Aktivitätsprotokoll"));

      var fehlerberichteWuenscheWindow = home.MainMenu.Extras_FehlerberichteWuensche();
      Assert.That (fehlerberichteWuenscheWindow.GetTitle(), Is.StringStarting ("ACTA NOVA Kommunal"));
      Assert.That (fehlerberichteWuenscheWindow.GetTitle(), Is.StringEnding ("Fehlerberichte/Wünsche"));
      fehlerberichteWuenscheWindow.Close();

      home = home.MainMenu.Extras_TempExportDokumente();
      Assert.That (home.GetTitle(), Is.EqualTo ("Temporäre Export Dokumente"));

      home = home.MainMenu.Extras_Prozessleitstand();
      Assert.That (home.GetTitle(), Is.EqualTo ("Prozessleitstand"));

      var skartierungWindow = home.MainMenu.Extras_Skartierung();
      Assert.That (skartierungWindow.GetTitle(), Is.StringStarting ("ACTA NOVA Kommunal"));
      Assert.That (skartierungWindow.GetTitle(), Is.StringEnding ("Skartierungspakete"));
      skartierungWindow.Close();

      home = home.MainMenu.Extras_Rueckstandsausweis();
      Assert.That (home.GetTitle(), Is.EqualTo ("Rückstandsausweis"));

      home = home.MainMenu.Extras_RueckscheinErfassen();
      Assert.That (home.GetTitle(), Is.EqualTo ("Rückschein erfassen"));

      home.Header.GetBreadCrumbs()[0].Click();
      Assert.That (home.GetTitle(), Is.EqualTo ("Eigener AV"));

      home = home.MainMenu.Extras_BefehleVormerken();
      Assert.That (home.GetTitle(), Is.EqualTo ("Eigener AV"));

      home = home.MainMenu.Extras_BefehleSofortAusfuehren();
      Assert.That (home.GetTitle(), Is.EqualTo ("Eigener AV"));

      home = home.MainMenu.Extras_VorgemerkteBefehle();
      Assert.That (home.GetTitle(), Is.EqualTo ("Vorgemerkte Befehle"));

      home = home.MainMenu.Extras_EDIAKTImport();
      Assert.That (home.GetTitle(), Is.EqualTo ("EDIAKT importieren"));

      home.Header.GetBreadCrumbs()[0].ClickAndConfirmDataLoss();
      Assert.That (home.GetTitle(), Is.EqualTo ("Eigener AV"));
    }

    [Test]
    public void TestActaNovaMainMenuExtensions_Hilfe ()
    {
      var home = Start();

      // Todo EVB-8268: Uncomment as soon as Hilfe_Gesamthilfe extension method has been implemented.
      //var gesamthilfeWindow = home.MainMenu.Hilfe_Gesamthilfe();
      //Assert.That (gesamthilfeWindow.Scope.FindCss ("p.DokInfo").Text, Is.EqualTo ("Inhaltsverzeichnis"));
      //gesamthilfeWindow.Close();

      var indexWindow = home.MainMenu.Hilfe_Index();
      Assert.That (indexWindow.GetTitle(), Is.EqualTo ("Index of Help"));
      indexWindow.Close();

      home = home.MainMenu.Hilfe_HilfesymboleAnzeigen();
      Assert.That (home.GetTitle(), Is.EqualTo ("Eigener AV"));

      home = home.MainMenu.Hilfe_HilfesymboleVerbergen();
      Assert.That (home.GetTitle(), Is.EqualTo ("Eigener AV"));

      var leistungskatalogWindow = home.MainMenu.Hilfe_Leistungskatalog();
      Assert.That (leistungskatalogWindow.GetTitle(), Is.StringStarting ("ACTA NOVA Kommunal"));
      Assert.That (leistungskatalogWindow.GetTitle(), Is.StringEnding ("Leistungsbeschreibung"));
      leistungskatalogWindow.Close();

      var infoWindow = home.MainMenu.Hilfe_Info();
      Assert.That (infoWindow.GetTitle(), Is.StringStarting ("ACTA NOVA Kommunal"));
      Assert.That (infoWindow.GetTitle(), Is.StringEnding ("Information"));
      infoWindow.Close();
    }
  }
}