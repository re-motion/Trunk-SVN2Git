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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class ColumnDefinitionTest
  {
    public string DummyProperty { get; set; }
    public string OtherProperty { get; set; }

    [Test]
    public void Equals_True ()
    {
      var definition1 = new ColumnDefinition ("Name", GetType ().GetProperty ("DummyProperty"));
      var definition2 = new ColumnDefinition ("Name", GetType ().GetProperty ("DummyProperty"));

      Assert.IsTrue (definition1.Equals (definition2));
    }

    [Test]
    public void Equals_DifferentName_False ()
    {
      var definition1 = new ColumnDefinition ("Name", GetType ().GetProperty ("DummyProperty"));
      var definition2 = new ColumnDefinition ("Some Other Name", GetType ().GetProperty ("DummyProperty"));

      Assert.IsFalse (definition1.Equals (definition2));
    }

    [Test]
    public void Equals_DifferentProperty_False ()
    {
      var definition1 = new ColumnDefinition ("Name", GetType ().GetProperty ("DummyProperty"));
      var definition2 = new ColumnDefinition ("Name", GetType ().GetProperty ("OtherProperty"));

      Assert.IsFalse (definition1.Equals (definition2));
    }

    [Test]
    public void Equals_Null_False ()
    {
      var definition1 = new ColumnDefinition ("Name", GetType ().GetProperty ("DummyProperty"));

      Assert.IsFalse (definition1.Equals (null));
    }

    [Test]
    public void GetHashCode_EqualsTrue ()
    {
      var definition1 = new ColumnDefinition ("Name", GetType ().GetProperty ("DummyProperty"));
      var definition2 = new ColumnDefinition ("Name", GetType ().GetProperty ("DummyProperty"));

      Assert.AreEqual (definition1.GetHashCode(), definition2.GetHashCode());
    }

    [Test]
    public void GetHashCode_DifferentName_EqualsFalse ()
    {
      var definition1 = new ColumnDefinition ("Name", GetType ().GetProperty ("DummyProperty"));
      var definition2 = new ColumnDefinition ("Some Other Name", GetType ().GetProperty ("DummyProperty"));

      Assert.AreNotEqual (definition1.GetHashCode (), definition2.GetHashCode ());
    }

    [Test]
    public void GetHashCode_DifferentProperty_EqualsFalse ()
    {
      var definition1 = new ColumnDefinition ("Name", GetType ().GetProperty ("DummyProperty"));
      var definition2 = new ColumnDefinition ("Name", GetType ().GetProperty ("OtherProperty"));

      Assert.AreNotEqual (definition1.GetHashCode (), definition2.GetHashCode ());
    }
  }
}