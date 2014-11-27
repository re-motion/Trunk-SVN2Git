using System;
using ActaNova.WebTesting.ControlObjects;
using ActaNova.WebTesting.PageObjects;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace ActaNova.WebTesting.ActaNovaExtensions
{
  /// <summary>
  /// ActaNova-specific extension methods for interacting with the main menu.
  /// </summary>
  public static class ActaNovaMainMenuExtensions
  {
    public static ActaNovaMainPageObject Neu_Akt ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Neu", "Akt").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Neu_Buergeranliegen ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Neu", "Bürgeranliegen").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Neu_Eingangsstueck ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Neu", "Eingangsstück").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Neu_Entwurf ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Neu", "Entwurf").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Neu_Geschaeftsfall ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Neu", "Geschäftsfall").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Neu_Organisation ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Neu", "Organisation").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Neu_Organisationsverteiler ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Neu", "Organisationsverteiler").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Neu_Person ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Neu", "Person").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Neu_VerteilerDynamisch ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Neu", "Verteiler (dynamisch)").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Neu_VerteilerStatisch ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Neu", "Verteiler (statisch)").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Verfahrensbereich_None ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Verfahrensbereich", "Kein Verfahrensbereich").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Verfahrensbereich_BA ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Verfahrensbereich", "BA - Bürgeranliegen").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Verfahrensbereich_BW ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Verfahrensbereich", "BW - Bauen und Wohnen").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Verfahrensbereich_GS ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Verfahrensbereich", "GS - Gesellschaft und Soziales").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Verfahrensbereich_SU ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Verfahrensbereich", "SU - Subventionen").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Suchen_Aufgabe ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Suchen", "Aufgaben/Termine", "Aufgabe").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Suchen_Termin ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Suchen", "Aufgaben/Termine", "Termin").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Suchen_FehlerberichtWunsch ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Suchen", "Fehlerbericht/Wunsch").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Suchen_Akt ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Suchen", "Geschäftsobjekt", "Akt").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Suchen_Buergeranliegen ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Suchen", "Geschäftsobjekt", "Bürgeranliegen").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Suchen_Eingangsstueck ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Suchen", "Geschäftsobjekt", "Eingangsstück").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Suchen_Entwurf ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Suchen", "Geschäftsobjekt", "Entwurf").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Suchen_Geschaeftsfall ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Suchen", "Geschäftsobjekt", "Geschäftsfall").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Suchen_AusgehendeEmail ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Suchen", "Inhaltsobjekt", "Dokument", "Ausgehende E-Mail").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Suchen_EingehendeEmail ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Suchen", "Inhaltsobjekt", "Dokument", "Eingehende E-Mail").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Suchen_EDIAKTExportDokument ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Suchen", "Inhaltsobjekt", "EDIAKT Export Dokument").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Suchen_Erledigung ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Suchen", "Inhaltsobjekt", "Erledigung").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Suchen_InternerEingang ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Suchen", "Inhaltsobjekt", "Interner Eingang").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Suchen_Rueckschein ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Suchen", "Inhaltsobjekt", "Rückschein").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Suchen_Organisationsverteiler ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Suchen", "Organisationsverteiler").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Suchen_Adresse ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Suchen", "Stammdatenobjekt", "Adresse").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Suchen_Email ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Suchen", "Stammdatenobjekt", "E-Mail").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Suchen_Grundstueck ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Suchen", "Stammdatenobjekt", "Grundstück").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Suchen_Information ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Suchen", "Stammdatenobjekt", "Information").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Suchen_Organisation ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Suchen", "Stammdatenobjekt", "Rechtsträger", "Organisation").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Suchen_Person ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Suchen", "Stammdatenobjekt", "Rechtsträger", "Person").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Suchen_Telefon ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Suchen", "Stammdatenobjekt", "Telefon").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Suchen_WebSeite ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Suchen", "Stammdatenobjekt", "Web-Seite").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Suchen_VerteilerDynamisch ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Suchen", "Verteiler", "Verteiler (dynamisch)").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Suchen_VerteilerStatisch ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Suchen", "Verteiler", "Verteiler (statisch)").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Extras_PasswortAendern ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Extras", "Passwort ändern").ExpectMainPage();
    }

    public static ActaNovaWindowPageObject Extras_Administration ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return
          mainMenu.Select (new[] { "Extras", "Administration" }, Opt.ContinueWhen (Wxe.PostBackCompleted))
              .ExpectNewWindow<ActaNovaWindowPageObject> ("Administration");
    }

    public static ActaNovaMainPageObject Extras_Benutzerprofil ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Extras", "Benutzerprofil").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Extras_Aktivitaetsprotokoll ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Extras", "Aktivitätsprotokoll").ExpectMainPage();
    }

    public static ActaNovaWindowPageObject Extras_FehlerberichteWuensche ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return
          mainMenu.Select (new[] { "Extras", "Fehlerberichte/Wünsche" }, Opt.ContinueWhen (Wxe.PostBackCompleted))
              .ExpectNewWindow<ActaNovaWindowPageObject> ("Fehlerberichte/Wünsche");
    }

    public static ActaNovaMainPageObject Extras_TempExportDokumente ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Extras", "Temp. Export Dokumente").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Extras_Prozessleitstand ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Extras", "Prozessleitstand").ExpectMainPage();
    }

    public static ActaNovaWindowPageObject Extras_Skartierung ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return
          mainMenu.Select (new[] { "Extras", "Skartierung" }, Opt.ContinueWhen (Wxe.PostBackCompleted))
              .ExpectNewWindow<ActaNovaWindowPageObject> ("Skartierungspakete");
    }

    public static ActaNovaMainPageObject Extras_Rueckstandsausweis ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Extras", "Rückstandsausweis").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Extras_RueckscheinErfassen ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Extras", "Rückschein erfassen").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Extras_BefehleVormerken ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select (new[] { "Extras", "Befehle vormerken" }, Opt.ContinueWhen (Wxe.PostBackCompleted)).ExpectMainPage();
    }

    public static ActaNovaMainPageObject Extras_VorgemerkteBefehle ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Extras", "Vorgemerkte Befehle").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Extras_BefehleSofortAusfuehren ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select (new[] { "Extras", "Befehle sofort ausführen" }, Opt.ContinueWhen (Wxe.PostBackCompleted)).ExpectMainPage();
    }

    public static ActaNovaMainPageObject Extras_GruppeZuweisen ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Extras", "Gruppe zuweisen").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Extras_EDIAKTImport ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select ("Extras", "EDIAKT", "EDIAKT Import").ExpectMainPage();
    }

    public static HtmlPageObject Hilfe_Gesamthilfe ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      throw new NotSupportedException (
          "Hilfe_Gesamthilfe cannot be implemented at the moment due to technical reasons: the new window does not have a title.");

#pragma warning disable 162 // unreachable code, can be dropped as soon as NotSupportedException has been removed.
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select (new[] { "Hilfe", "Gesamthilfe" }, Opt.ContinueImmediately()).ExpectNewWindow<HtmlPageObject> ("");
#pragma warning restore 162
    }

    public static HtmlPageObject Hilfe_Index ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select (new[] { "Hilfe", "Index" }, Opt.ContinueImmediately()).ExpectNewWindow<HtmlPageObject> ("Index of Help");
    }

    public static ActaNovaMainPageObject Hilfe_HilfesymboleAnzeigen ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select (new[] { "Hilfe", "Hilfesymbole anzeigen" }, Opt.ContinueWhen (Wxe.PostBackCompleted)).ExpectMainPage();
    }

    public static ActaNovaMainPageObject Hilfe_HilfesymboleVerbergen ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select (new[] { "Hilfe", "Hilfesymbole verbergen" }, Opt.ContinueWhen (Wxe.PostBackCompleted)).ExpectMainPage();
    }

    public static ActaNovaWindowPageObject Hilfe_Leistungskatalog ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return
          mainMenu.Select (new[] { "Hilfe", "Leistungskatalog" }, Opt.ContinueWhen (Wxe.PostBackCompleted))
              .ExpectNewWindow<ActaNovaWindowPageObject> ("Leistungsbeschreibung");
    }

    public static ActaNovaWindowPageObject Hilfe_Info ([NotNull] this ActaNovaMainMenuControlObject mainMenu)
    {
      ArgumentUtility.CheckNotNull ("mainMenu", mainMenu);

      return mainMenu.Select (new[] { "Hilfe", "Info" }, Opt.ContinueImmediately()).ExpectNewWindow<ActaNovaWindowPageObject> ("Information");
    }
  }
}