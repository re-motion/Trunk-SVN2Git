// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework.SyntaxHelpers;
using Remotion.ExtensibleEnums;
using Remotion.UnitTests.ExtensibleEnums.TestDomain;

namespace Remotion.UnitTests.ExtensibleEnums
{
  [TestFixture]
  public class ExtensibleEnumTest
  {
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
    public new void ToString ()
    {
      var value = new Color ("Red");

      Assert.That (value.ToString(), Is.EqualTo ("Color: Red"));
    }

    [Test]
    public void ToString_DerivedEnum ()
    {
      var value = new MetallicColor ("RedMetallic");

      Assert.That (value.ToString (), Is.EqualTo ("Color: RedMetallic"));
    }

    [Test]
    public void Values ()
    {
      Assert.That (Color.Values, Is.Not.Null);
    }

    [Test]
    public void Values_FromCache ()
    {
      Assert.That (Color.Values, Is.SameAs (ExtensibleEnumValuesCache.Instance.GetValues (typeof (Color))));
    }
  }
}