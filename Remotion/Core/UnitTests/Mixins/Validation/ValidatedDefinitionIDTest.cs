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
using Remotion.Development.UnitTesting;
using Remotion.Mixins.Validation;

namespace Remotion.UnitTests.Mixins.Validation
{
  [TestFixture]
  public class ValidatedDefinitionIDTest
  {
    [Test]
    public void Equals_True ()
    {
      var one = new ValidatedDefinitionID ("a", "b", null);
      var two = new ValidatedDefinitionID ("a", "b", null);

      Assert.That (one.Equals ((object) two), Is.True);
    }

    [Test]
    public void Equals_True_NonNullParentID ()
    {
      var one = new ValidatedDefinitionID ("a", "b", new ValidatedDefinitionID ("c", "d", null));
      var two = new ValidatedDefinitionID ("a", "b", new ValidatedDefinitionID ("c", "d", null));

      Assert.That (one.Equals ((object) two), Is.True);
    }

     [Test]
    public void Equals_False_Null ()
    {
      var one = new ValidatedDefinitionID ("a", "b", null);

      Assert.That (one.Equals ((object) null), Is.False);
    }

    [Test]
    public void Equals_False_DifferentKind ()
    {
      var one = new ValidatedDefinitionID ("a", "b", null);
      var two = new ValidatedDefinitionID ("a2", "b", null);

      Assert.That (one.Equals ((object) two), Is.False);
    }

    [Test]
    public void Equals_False_DifferentFullName ()
    {
      var one = new ValidatedDefinitionID ("a", "b", null);
      var two = new ValidatedDefinitionID ("a", "b2", null);

      Assert.That (one.Equals ((object) two), Is.False);
    }

    [Test]
    public void Equals_False_DifferentParent_OneNull ()
    {
      var one = new ValidatedDefinitionID ("a", "b", null);
      var two = new ValidatedDefinitionID ("a", "b", new ValidatedDefinitionID ("c", "d", null));

      Assert.That (one.Equals ((object) two), Is.False);
      Assert.That (two.Equals ((object) one), Is.False);
    }

    [Test]
    public void Equals_False_DifferentParent_BothNonNull ()
    {
      var one = new ValidatedDefinitionID ("a", "b", new ValidatedDefinitionID ("c", "d", null));
      var two = new ValidatedDefinitionID ("a", "b", new ValidatedDefinitionID ("c", "d2", null));

      Assert.That (one.Equals ((object) two), Is.False);
    }

    [Test]
    public void EquatableEquals_True ()
    {
      var one = new ValidatedDefinitionID ("a", "b", null);
      var two = new ValidatedDefinitionID ("a", "b", null);

      Assert.That (one.Equals (two), Is.True);
    }

    [Test]
    public void EquatableEquals_True_NonNullParentID ()
    {
      var one = new ValidatedDefinitionID ("a", "b", new ValidatedDefinitionID ("c", "d", null));
      var two = new ValidatedDefinitionID ("a", "b", new ValidatedDefinitionID ("c", "d", null));

      Assert.That (one.Equals (two), Is.True);
    }

    [Test]
    public void EquatableEquals_False_Null ()
    {
      var one = new ValidatedDefinitionID ("a", "b", null);

      Assert.That (one.Equals (null), Is.False);
    }

    [Test]
    public void EquatableEquals_False_DifferentKind ()
    {
      var one = new ValidatedDefinitionID ("a", "b", null);
      var two = new ValidatedDefinitionID ("a2", "b", null);

      Assert.That (one.Equals (two), Is.False);
    }

    [Test]
    public void EquatableEquals_False_DifferentFullName ()
    {
      var one = new ValidatedDefinitionID ("a", "b", null);
      var two = new ValidatedDefinitionID ("a", "b2", null);

      Assert.That (one.Equals (two), Is.False);
    }

    [Test]
    public void EquatableEquals_False_DifferentParent_OneNull ()
    {
      var one = new ValidatedDefinitionID ("a", "b", null);
      var two = new ValidatedDefinitionID ("a", "b", new ValidatedDefinitionID ("c", "d", null));

      Assert.That (one.Equals (two), Is.False);
      Assert.That (two.Equals (one), Is.False);
    }

    [Test]
    public void EquatableEquals_False_DifferentParent_BothNonNull ()
    {
      var one = new ValidatedDefinitionID ("a", "b", new ValidatedDefinitionID ("c", "d", null));
      var two = new ValidatedDefinitionID ("a", "b", new ValidatedDefinitionID ("c", "d2", null));

      Assert.That (one.Equals (two), Is.False);
    }

    [Test]
    public void EqualsOperator_True ()
    {
      var one = new ValidatedDefinitionID ("a", "b", null);
      var two = new ValidatedDefinitionID ("a", "b", null);

      Assert.That (one == two, Is.True);
      Assert.That (one != two, Is.False);
    }

    [Test]
    public void EqualsOperator_True_NonNullParentID ()
    {
      var one = new ValidatedDefinitionID ("a", "b", new ValidatedDefinitionID ("c", "d", null));
      var two = new ValidatedDefinitionID ("a", "b", new ValidatedDefinitionID ("c", "d", null));

      Assert.That (one == two, Is.True);
      Assert.That (one != two, Is.False);
    }

    [Test]
    public void EqualsOperator_False_Null ()
    {
      var one = new ValidatedDefinitionID ("a", "b", null);

      Assert.That (one == null, Is.False);
      Assert.That (one != null, Is.True);

      Assert.That (null == one, Is.False);
      Assert.That (null != one, Is.True);
    }

    [Test]
    public void EqualsOperator_False_DifferentKind ()
    {
      var one = new ValidatedDefinitionID ("a", "b", null);
      var two = new ValidatedDefinitionID ("a2", "b", null);

      Assert.That (one == two, Is.False);
      Assert.That (one != two, Is.True);
    }

    [Test]
    public void EqualsOperator_False_DifferentFullName ()
    {
      var one = new ValidatedDefinitionID ("a", "b", null);
      var two = new ValidatedDefinitionID ("a", "b2", null);

      Assert.That (one == two, Is.False);
      Assert.That (one != two, Is.True);
    }

    [Test]
    public void EqualsOperator_False_DifferentParent_OneNull ()
    {
      var one = new ValidatedDefinitionID ("a", "b", null);
      var two = new ValidatedDefinitionID ("a", "b", new ValidatedDefinitionID ("c", "d", null));

      Assert.That (one == two, Is.False);
      Assert.That (two == one, Is.False);
    }

    [Test]
    public void EqualsOperator_False_DifferentParent_BothNonNull ()
    {
      var one = new ValidatedDefinitionID ("a", "b", new ValidatedDefinitionID ("c", "d", null));
      var two = new ValidatedDefinitionID ("a", "b", new ValidatedDefinitionID ("c", "d2", null));

      Assert.That (one == two, Is.False);
    }

    [Test]
    public void GetHashcode_EqualObjects ()
    {
      var one = new ValidatedDefinitionID ("a", "b", null);
      var two = new ValidatedDefinitionID ("a", "b", null);

      Assert.That (one.GetHashCode(), Is.EqualTo (two.GetHashCode()));
    }

    [Test]
    public void GetHashcode_EqualObjects_NonNullParentID ()
    {
      var one = new ValidatedDefinitionID ("a", "b", new ValidatedDefinitionID ("c", "d", null));
      var two = new ValidatedDefinitionID ("a", "b", new ValidatedDefinitionID ("c", "d", null));

      Assert.That (one.GetHashCode(), Is.EqualTo (two.GetHashCode()));
    }

    [Test]
    public void Serialization ()
    {
      var id = new ValidatedDefinitionID ("a", "b", new ValidatedDefinitionID ("c", "d", null));

      var deserializedID = Serializer.SerializeAndDeserialize (id);

      Assert.That (deserializedID, Is.Not.SameAs (id));
      Assert.That (deserializedID, Is.EqualTo (id));
    }
  }
}