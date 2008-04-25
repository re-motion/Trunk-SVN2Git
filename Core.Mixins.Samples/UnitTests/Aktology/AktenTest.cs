using System;
using NUnit.Framework;
using Remotion.Mixins.Samples.Aktology;

namespace Remotion.Mixins.Samples.UnitTests.Aktology
{
  [TestFixture]
  public class AktenTest
  {
    [Test]
    public void PersonenAkt ()
    {
      IPersonenAkt akt = AktFactory.CreatePersonenAkt ();
      Assert.IsNotNull (akt);

      // Akt
      akt.ID = "AKTE X";
      akt.Inhalt = "Eine spannende Mystery-Serie mit David Duchovny und Gillian Anderson.";

      Assert.AreEqual ("AKTE X", akt.ID);
      Assert.AreEqual ("Eine spannende Mystery-Serie mit David Duchovny und Gillian Anderson.", akt.Inhalt);
      
      // Person
      akt.Vorname = "Karl-Heinz";
      akt.Nachname = "K�penik";

      Assert.AreEqual ("Karl-Heinz", akt.Vorname);
      Assert.AreEqual ("K�penik", akt.Nachname);
    }

    [Test]
    public void PersonenSachAkt ()
    {
      IPersonenSachAkt akt = AktFactory.CreatePersonenSachAkt ();
      Assert.IsNotNull (akt);

      // Akt
      akt.ID = "AKTE X";
      akt.Inhalt = "Eine spannende Mystery-Serie mit David Duchovny und Gillian Anderson.";

      Assert.AreEqual ("AKTE X", akt.ID);
      Assert.AreEqual ("Eine spannende Mystery-Serie mit David Duchovny und Gillian Anderson.", akt.Inhalt);

      // SachAkt
      akt.Sache = "Rubik-W�rfel";

      Assert.AreEqual ("Rubik-W�rfel", akt.Sache);

      // Person
      akt.Vorname = "Karl-Heinz";
      akt.Nachname = "K�penik";

      Assert.AreEqual ("Karl-Heinz", akt.Vorname);
      Assert.AreEqual ("K�penik", akt.Nachname);
    }

    [Test]
    public void AngestelltenAkt ()
    {
      IAngestelltenAkt akt = AktFactory.CreateAngestelltenAkt ();
      Assert.IsNotNull (akt);

      // Akt
      akt.ID = "AKTE X";
      akt.Inhalt = "Eine spannende Mystery-Serie mit David Duchovny und Gillian Anderson.";

      Assert.AreEqual ("AKTE X", akt.ID);
      Assert.AreEqual ("Eine spannende Mystery-Serie mit David Duchovny und Gillian Anderson.", akt.Inhalt);

      // Person
      akt.Vorname = "Karl-Heinz";
      akt.Nachname = "K�penik";

      Assert.AreEqual ("Karl-Heinz", akt.Vorname);
      Assert.AreEqual ("K�penik", akt.Nachname);

      // Angestellter
      akt.Position = "Hauptmann";

      Assert.AreEqual ("Hauptmann", akt.Position);
    }

    [Test]
    public void AngestelltenSachAkt ()
    {
      IAngestelltenSachAkt akt = AktFactory.CreateAngestelltenSachAkt ();
      Assert.IsNotNull (akt);

      // Akt
      akt.ID = "AKTE X";
      akt.Inhalt = "Eine spannende Mystery-Serie mit David Duchovny und Gillian Anderson.";

      Assert.AreEqual ("AKTE X", akt.ID);
      Assert.AreEqual ("Eine spannende Mystery-Serie mit David Duchovny und Gillian Anderson.", akt.Inhalt);

      // SachAkt
      akt.Sache = "Rubik-W�rfel";

      Assert.AreEqual ("Rubik-W�rfel", akt.Sache);

      // Person
      akt.Vorname = "Karl-Heinz";
      akt.Nachname = "K�penik";

      Assert.AreEqual ("Karl-Heinz", akt.Vorname);
      Assert.AreEqual ("K�penik", akt.Nachname);

      // Angestellter
      akt.Position = "Hauptmann";

      Assert.AreEqual ("Hauptmann", akt.Position);
    }
  }
}
