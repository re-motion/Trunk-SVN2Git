// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.ExtensibleEnums.Infrastructure;
using Remotion.UnitTests.ExtensibleEnums.TestDomain;
using System.Linq;

namespace Remotion.UnitTests.ExtensibleEnums
{
  [TestFixture]
  public class ExtensibleEnumTest
  {
    [Test]
    public void Initialization_ShortIDOnly ()
    {
      Assert.That (EnumWithDifferentCtors.Values.ShortIDOnly().ShortID, Is.EqualTo ("ShortID"));
      Assert.That (EnumWithDifferentCtors.Values.ShortIDOnly().IDPrefix, Is.Null);
      Assert.That (EnumWithDifferentCtors.Values.ShortIDOnly().ID, Is.EqualTo ("ShortID"));
    }

    [Test]
    public void Initialization_ShortIDAndPrefix ()
    {
      Assert.That (EnumWithDifferentCtors.Values.ShortIDAndPrefix().ShortID, Is.EqualTo ("ShortID"));
      Assert.That (EnumWithDifferentCtors.Values.ShortIDAndPrefix().IDPrefix, Is.EqualTo ("Prefix"));
      Assert.That (EnumWithDifferentCtors.Values.ShortIDAndPrefix().ID, Is.EqualTo ("Prefix.ShortID"));
    }

    [Test]
    public void Initialization_ShortIDAndNullPrefix ()
    {
      Assert.That (EnumWithDifferentCtors.Values.ShortIDAndNullPrefix().ShortID, Is.EqualTo ("ShortID"));
      Assert.That (EnumWithDifferentCtors.Values.ShortIDAndNullPrefix().IDPrefix, Is.Null);
      Assert.That (EnumWithDifferentCtors.Values.ShortIDAndNullPrefix().ID, Is.EqualTo ("ShortID"));
    }

    [Test]
    public void Initialization_ShortIDAndEmptyPrefix ()
    {
      Assert.That (EnumWithDifferentCtors.Values.ShortIDAndEmptyPrefix().ShortID, Is.EqualTo ("ShortID"));
      Assert.That (EnumWithDifferentCtors.Values.ShortIDAndEmptyPrefix().IDPrefix, Is.Empty);
      Assert.That (EnumWithDifferentCtors.Values.ShortIDAndEmptyPrefix().ID, Is.EqualTo ("ShortID"));
    }

    [Test]
    public void Initialization_ShortIDAndType ()
    {
      Assert.That (EnumWithDifferentCtors.Values.ShortIDAndType().ShortID, Is.EqualTo ("ShortID"));
      Assert.That (EnumWithDifferentCtors.Values.ShortIDAndType().IDPrefix, Is.EqualTo (typeof (EnumWithDifferentCtorsExtensions).FullName));
      Assert.That (
          EnumWithDifferentCtors.Values.ShortIDAndType().ID,
          Is.EqualTo ("Remotion.UnitTests.ExtensibleEnums.TestDomain.EnumWithDifferentCtorsExtensions.ShortID"));
    }

    [Test]
    public void Initialization_MethodAsID ()
    {
      Assert.That (EnumWithDifferentCtors.Values.MethodAsID().ShortID, Is.EqualTo ("MethodAsID"));
      Assert.That (EnumWithDifferentCtors.Values.MethodAsID().IDPrefix, Is.EqualTo (typeof (EnumWithDifferentCtorsExtensions).FullName));
      Assert.That (
          EnumWithDifferentCtors.Values.MethodAsID().ID,
          Is.EqualTo ("Remotion.UnitTests.ExtensibleEnums.TestDomain.EnumWithDifferentCtorsExtensions.MethodAsID"));
    }

    [Test]
    public void Equals_True ()
    {
      var value1 = new Color ("ID");
      var value2 = new Color ("ID");

      Assert.That (value1.Equals (value2), Is.True);
    }

    [Test]
    public void Equals_False_DifferentIDs ()
    {
      var value1 = new Color ("ID1");
      var value2 = new Color ("ID2");

      Assert.That (value1.Equals (value2), Is.False);
    }

    [Test]
    public void Equals_False_DifferentTypes ()
    {
      var value1 = new Color ("ID");
      var value2 = new MetallicColor ("ID");

      Assert.That (value1.Equals (value2), Is.False);
    }

    [Test]
    public void Equals_False_Null ()
    {
      var value = new Color ("ID");

      Assert.That (value.Equals (null), Is.False);
    }

    [Test]
    public void GetHashCode_EqualObjects ()
    {
      var value1 = new Color ("ID");
      var value2 = new Color ("ID");

      Assert.That (value1.GetHashCode(), Is.EqualTo (value2.GetHashCode()));
    }

    [Test]
    public void ToString_ReturnsFullID ()
    {
      var value = new EnumWithDifferentCtors ("Prefix", "ShortID");

      Assert.That (value.ToString(), Is.EqualTo ("Prefix.ShortID"));
    }

    [Test]
    public void Values ()
    {
      Assert.That (Color.Values, Is.Not.Null);
    }

    [Test]
    public void Values_FromCache ()
    {
      Assert.That (Color.Values, Is.SameAs (ExtensibleEnumDefinitionCache.Instance.GetDefinition (typeof (Color))));
    }

    [Test]
    public void Values_IntegrationTest ()
    {
      var valueInfos = Color.Values.GetValueInfos();
      Assert.That (valueInfos.Select (info => info.Value).ToArray(),
          Is.EquivalentTo (new[] { 
              Color.Values.Red (), 
              Color.Values.Green (), 
              Color.Values.RedMetallic (), 
              Color.Values.LightRed(), 
              Color.Values.LightBlue() }));
    }

    [Test]
    public void GetEnumType ()
    {
      var value = new Color ("Red");
      Assert.That (value.GetEnumType(), Is.SameAs (typeof (Color)));
    }

    [Test]
    public void GetEnumType_DerivedType ()
    {
      var value = new MetallicColor ("RedMetallic");
      Assert.That (value.GetEnumType(), Is.SameAs (typeof (Color)));
    }

    [Test]
    public void GetLocalizedName ()
    {
      var value = new Color ("Red");
      Assert.That (value.GetLocalizedName(), Is.EqualTo ("Rot"));
    }

    [Test]
    public void GetLocalizedName_IntegrationTest ()
    {
      Assert.That (Color.Values.Red ().GetLocalizedName (), Is.EqualTo ("Rot"));
      Assert.That (Color.Values.Green ().GetLocalizedName (), Is.EqualTo ("Grün"));
      Assert.That (Color.Values.RedMetallic ().GetLocalizedName (), Is.EqualTo ("RedMetallic"));
      Assert.That (Color.Values.LightRed ().GetLocalizedName (), Is.EqualTo ("Hellrot"));
      Assert.That (Color.Values.LightBlue ().GetLocalizedName (), Is.EqualTo ("LightBlue"));
    }

    [Test]
    public void GetValueInfo ()
    {
      var value = new Color ("Red");
      Assert.That (value.GetValueInfo (), Is.SameAs (Color.Values.GetValueInfoByID ("Red")));
    }

    [Test]
    public void Serialization ()
    {
      var value = new EnumWithDifferentCtors ("Prefix", "Name");
      var deserializedValue = Serializer.SerializeAndDeserialize (value);

      Assert.That (deserializedValue, Is.EqualTo (value));
    }
  }
}
