// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
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
      akt.Nachname = "Köpenik";

      Assert.AreEqual ("Karl-Heinz", akt.Vorname);
      Assert.AreEqual ("Köpenik", akt.Nachname);
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
      akt.Sache = "Rubik-Würfel";

      Assert.AreEqual ("Rubik-Würfel", akt.Sache);

      // Person
      akt.Vorname = "Karl-Heinz";
      akt.Nachname = "Köpenik";

      Assert.AreEqual ("Karl-Heinz", akt.Vorname);
      Assert.AreEqual ("Köpenik", akt.Nachname);
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
      akt.Nachname = "Köpenik";

      Assert.AreEqual ("Karl-Heinz", akt.Vorname);
      Assert.AreEqual ("Köpenik", akt.Nachname);

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
      akt.Sache = "Rubik-Würfel";

      Assert.AreEqual ("Rubik-Würfel", akt.Sache);

      // Person
      akt.Vorname = "Karl-Heinz";
      akt.Nachname = "Köpenik";

      Assert.AreEqual ("Karl-Heinz", akt.Vorname);
      Assert.AreEqual ("Köpenik", akt.Nachname);

      // Angestellter
      akt.Position = "Hauptmann";

      Assert.AreEqual ("Hauptmann", akt.Position);
    }
  }
}
