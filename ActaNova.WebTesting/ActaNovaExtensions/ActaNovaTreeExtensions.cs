using System;
using ActaNova.WebTesting.ControlObjects;
using ActaNova.WebTesting.PageObjects;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace ActaNova.WebTesting.ActaNovaExtensions
{
  /// <summary>
  /// ActaNova-specific extension methods for interacting with the main tree.
  /// </summary>
  public static class ActaNovaTreeExtensions
  {
    public static ActaNovaTreeNodeControlObject GetEigenerAvNode ([NotNull] this ActaNovaTreeControlObject tree)
    {
      ArgumentUtility.CheckNotNull ("tree", tree);

      return tree.GetNode().WithDisplayText ("Eigener AV");
    }

    public static ActaNovaMainPageObject SelectEigenerAvNode ([NotNull] this ActaNovaTreeControlObject tree)
    {
      ArgumentUtility.CheckNotNull ("tree", tree);

      return tree.GetEigenerAvNode().Select().ExpectMainPage();
    }

    public static ActaNovaTreeNodeControlObject GetGruppenAvNode ([NotNull] this ActaNovaTreeControlObject tree)
    {
      ArgumentUtility.CheckNotNull ("tree", tree);

      return tree.GetNode().WithDisplayText ("Gruppen AV");
    }

    public static ActaNovaMainPageObject SelectGruppenAvNode ([NotNull] this ActaNovaTreeControlObject tree)
    {
      ArgumentUtility.CheckNotNull ("tree", tree);

      return tree.GetGruppenAvNode().Select().ExpectMainPage();
    }

    public static ActaNovaTreeNodeControlObject GetStellvertretungsAvNode ([NotNull] this ActaNovaTreeControlObject tree)
    {
      ArgumentUtility.CheckNotNull ("tree", tree);

      return tree.GetNode().WithDisplayText ("Stellvertretungs AV");
    }

    public static ActaNovaMainPageObject SelectStellvertretungsAvNode ([NotNull] this ActaNovaTreeControlObject tree)
    {
      ArgumentUtility.CheckNotNull ("tree", tree);

      return tree.GetStellvertretungsAvNode().Select().ExpectMainPage();
    }

    public static ActaNovaTreeNodeControlObject GetWiedervorlageAvNode ([NotNull] this ActaNovaTreeControlObject tree)
    {
      ArgumentUtility.CheckNotNull ("tree", tree);

      return tree.GetNode().WithDisplayText ("Wiedervorlage AV");
    }

    public static ActaNovaMainPageObject SelectWiedervorlageAvNode ([NotNull] this ActaNovaTreeControlObject tree)
    {
      ArgumentUtility.CheckNotNull ("tree", tree);

      return tree.GetWiedervorlageAvNode().Select().ExpectMainPage();
    }

    public static ActaNovaTreeNodeControlObject GetZurueckziehenAvNode ([NotNull] this ActaNovaTreeControlObject tree)
    {
      ArgumentUtility.CheckNotNull ("tree", tree);

      return tree.GetNode().WithDisplayText ("Zurückziehen AV");
    }

    public static ActaNovaMainPageObject SelectZurueckziehenAvNode ([NotNull] this ActaNovaTreeControlObject tree)
    {
      ArgumentUtility.CheckNotNull ("tree", tree);

      return tree.GetZurueckziehenAvNode().Select().ExpectMainPage();
    }

    public static ActaNovaTreeNodeControlObject GetMeineAufgabenTermineNode ([NotNull] this ActaNovaTreeControlObject tree)
    {
      ArgumentUtility.CheckNotNull ("tree", tree);

      return tree.GetNode().WithDisplayText ("Meine Aufgaben/Termine");
    }

    public static ActaNovaMainPageObject SelectMeineAufgabenTermineNode ([NotNull] this ActaNovaTreeControlObject tree)
    {
      ArgumentUtility.CheckNotNull ("tree", tree);

      return tree.GetMeineAufgabenTermineNode().Select().ExpectMainPage();
    }

    public static ActaNovaTreeNodeControlObject GetGruppenAufgabenTermineNode ([NotNull] this ActaNovaTreeControlObject tree)
    {
      ArgumentUtility.CheckNotNull ("tree", tree);

      return tree.GetNode().WithDisplayText ("Gruppen Aufgaben/Termine");
    }

    public static ActaNovaMainPageObject SelectGruppenAufgabenTermineNode ([NotNull] this ActaNovaTreeControlObject tree)
    {
      ArgumentUtility.CheckNotNull ("tree", tree);

      return tree.GetGruppenAufgabenTermineNode().Select().ExpectMainPage();
    }

    public static ActaNovaTreeNodeControlObject GetFavoritenNode ([NotNull] this ActaNovaTreeControlObject tree)
    {
      ArgumentUtility.CheckNotNull ("tree", tree);

      return tree.GetNode().WithDisplayText ("Favoriten");
    }

    public static ActaNovaMainPageObject SelectFavoritenNode ([NotNull] this ActaNovaTreeControlObject tree)
    {
      ArgumentUtility.CheckNotNull ("tree", tree);

      return tree.GetFavoritenNode().Select().ExpectMainPage();
    }

    public static ActaNovaTreeNodeControlObject GetZuletztGespeicherteObjekteNode ([NotNull] this ActaNovaTreeControlObject tree)
    {
      ArgumentUtility.CheckNotNull ("tree", tree);

      return tree.GetNode().WithDisplayText ("Zuletzt gespeicherte Objekte");
    }

    public static ActaNovaMainPageObject SelectZuletztGespeicherteObjekteNode ([NotNull] this ActaNovaTreeControlObject tree)
    {
      ArgumentUtility.CheckNotNull ("tree", tree);

      return tree.GetZuletztGespeicherteObjekteNode().Select().ExpectMainPage();
    }
  }
}