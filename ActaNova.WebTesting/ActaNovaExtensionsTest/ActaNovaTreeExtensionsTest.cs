using System;
using ActaNova.WebTesting.ActaNovaExtensions;
using NUnit.Framework;

namespace ActaNova.WebTesting.ActaNovaExtensionsTest
{
  [TestFixture]
  public class ActaNovaTreeExtensionsTest : ActaNovaWebTestBase
  {
    [Test]
    public void TestGetNode ()
    {
      var home = Start();

      home = home.Tree.GetEigenerAvNode().Select().ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Eigener AV"));

      home = home.Tree.GetGruppenAvNode().Select().ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Gruppen AV"));

      home = home.Tree.GetStellvertretungsAvNode().Select().ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Stellvertretungs AV"));

      home = home.Tree.GetWiedervorlageAvNode().Select().ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Wiedervorlage AV"));

      home = home.Tree.GetZurueckziehenAvNode().Select().ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Zurückziehen AV"));

      home = home.Tree.GetMeineAufgabenTermineNode().Select().ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Meine Aufgaben/Termine"));

      home = home.Tree.GetGruppenAufgabenTermineNode().Select().ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Gruppen Aufgaben/Termine"));

      home = home.Tree.GetFavoritenNode().Select().ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Favoriten"));

      home = home.Tree.GetZuletztGespeicherteObjekteNode().Select().ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Zuletzt gespeicherte Objekte"));

      home = home.Tree.GetEigenerAvNode().Select().ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Eigener AV"));
    }

    [Test]
    public void TestSelectNode ()
    {
      var home = Start();

      home = home.Tree.SelectEigenerAvNode();
      Assert.That (home.GetTitle(), Is.EqualTo ("Eigener AV"));

      home = home.Tree.SelectGruppenAvNode();
      Assert.That (home.GetTitle(), Is.EqualTo ("Gruppen AV"));

      home = home.Tree.SelectStellvertretungsAvNode();
      Assert.That (home.GetTitle(), Is.EqualTo ("Stellvertretungs AV"));

      home = home.Tree.SelectWiedervorlageAvNode();
      Assert.That (home.GetTitle(), Is.EqualTo ("Wiedervorlage AV"));

      home = home.Tree.SelectZurueckziehenAvNode();
      Assert.That (home.GetTitle(), Is.EqualTo ("Zurückziehen AV"));

      home = home.Tree.SelectMeineAufgabenTermineNode();
      Assert.That (home.GetTitle(), Is.EqualTo ("Meine Aufgaben/Termine"));

      home = home.Tree.SelectGruppenAufgabenTermineNode();
      Assert.That (home.GetTitle(), Is.EqualTo ("Gruppen Aufgaben/Termine"));

      home = home.Tree.SelectFavoritenNode();
      Assert.That (home.GetTitle(), Is.EqualTo ("Favoriten"));

      home = home.Tree.SelectZuletztGespeicherteObjekteNode();
      Assert.That (home.GetTitle(), Is.EqualTo ("Zuletzt gespeicherte Objekte"));

      home = home.Tree.SelectEigenerAvNode();
      Assert.That (home.GetTitle(), Is.EqualTo ("Eigener AV"));
    }
  }
}