using System;
using ActaNova.WebTesting.ActaNovaExtensions;
using NUnit.Framework;

namespace ActaNova.WebTesting.ActaNovaExtensionsTest
{
  [TestFixture]
  public class ActaNovaTreeExtensionsTest : ActaNovaWebTestBase
  {
    [Test]
    public void Test ()
    {
      var home = Start();

      home = home.Tree.GruppenAV().Select().ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Gruppen AV"));

      home = home.Tree.StellvertretungsAV().Select().ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Stellvertretungs AV"));

      home = home.Tree.WiedervorlageAV().Select().ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Wiedervorlage AV"));

      home = home.Tree.ZurueckziehenAV().Select().ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Zurückziehen AV"));

      home = home.Tree.MeineAufgabenTermine().Select().ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Meine Aufgaben/Termine"));

      home = home.Tree.GruppenAufgabenTermine().Select().ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Gruppen Aufgaben/Termine"));

      home = home.Tree.Favoriten().Select().ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Favoriten"));

      home = home.Tree.ZuletztGespeicherteObjekte().Select().ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Zuletzt gespeicherte Objekte"));

      home = home.Tree.EigenerAV().Select().ExpectMainPage();
      Assert.That (home.GetTitle(), Is.EqualTo ("Eigener AV"));
    }
  }
}