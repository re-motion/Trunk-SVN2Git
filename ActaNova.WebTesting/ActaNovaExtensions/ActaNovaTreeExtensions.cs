using System;
using ActaNova.WebTesting.ControlObjects;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace ActaNova.WebTesting.ActaNovaExtensions
{
  public static class ActaNovaTreeExtensions
  {
    public static ActaNovaTreeNodeControlObject EigenerAV ([NotNull] this ActaNovaTreeControlObject tree)
    {
      ArgumentUtility.CheckNotNull ("tree", tree);

      return tree.GetNode().WithText ("Eigener AV");
    }

    public static ActaNovaTreeNodeControlObject GruppenAV ([NotNull] this ActaNovaTreeControlObject tree)
    {
      ArgumentUtility.CheckNotNull ("tree", tree);

      return tree.GetNode().WithText ("Gruppen AV");
    }

    public static ActaNovaTreeNodeControlObject StellvertretungsAV ([NotNull] this ActaNovaTreeControlObject tree)
    {
      ArgumentUtility.CheckNotNull ("tree", tree);

      return tree.GetNode().WithText ("Stellvertretungs AV");
    }

    public static ActaNovaTreeNodeControlObject WiedervorlageAV ([NotNull] this ActaNovaTreeControlObject tree)
    {
      ArgumentUtility.CheckNotNull ("tree", tree);

      return tree.GetNode().WithText ("Wiedervorlage AV");
    }

    public static ActaNovaTreeNodeControlObject ZurueckziehenAV ([NotNull] this ActaNovaTreeControlObject tree)
    {
      ArgumentUtility.CheckNotNull ("tree", tree);

      return tree.GetNode().WithText ("Zurückziehen AV");
    }

    public static ActaNovaTreeNodeControlObject MeineAufgabenTermine ([NotNull] this ActaNovaTreeControlObject tree)
    {
      ArgumentUtility.CheckNotNull ("tree", tree);

      return tree.GetNode().WithText ("Meine Aufgaben/Termine");
    }

    public static ActaNovaTreeNodeControlObject GruppenAufgabenTermine ([NotNull] this ActaNovaTreeControlObject tree)
    {
      ArgumentUtility.CheckNotNull ("tree", tree);

      return tree.GetNode().WithText ("Gruppen Aufgaben/Termine");
    }

    public static ActaNovaTreeNodeControlObject Favoriten ([NotNull] this ActaNovaTreeControlObject tree)
    {
      ArgumentUtility.CheckNotNull ("tree", tree);

      return tree.GetNode().WithText ("Favoriten");
    }

    public static ActaNovaTreeNodeControlObject ZuletztGespeicherteObjekte ([NotNull] this ActaNovaTreeControlObject tree)
    {
      ArgumentUtility.CheckNotNull ("tree", tree);

      return tree.GetNode().WithText ("Zuletzt gespeicherte Objekte");
    }
  }
}