// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
  public class ValidatedDefinitionDescriptionTest
  {
    [Test]
    public void FromDefinition ()
    {
      var definition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (object));
      
      var description = ValidatedDefinitionDescription.FromDefinition (definition);

      Assert.That (description.Kind, Is.EqualTo ("TargetClassDefinition"));
      Assert.That (description.FullName, Is.EqualTo (definition.FullName));
      Assert.That (description.Signature, Is.Null);
      Assert.That (description.ParentDescription, Is.Null);
    }

    [Test]
    public void FromDefinition_WithParent ()
    {
      var parentDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (object));
      var nestedDefinition = DefinitionObjectMother.CreateMixinDefinition (parentDefinition, typeof (string));
      
      var nestedDescription = ValidatedDefinitionDescription.FromDefinition (nestedDefinition);

      Assert.That (nestedDescription.ParentDescription, Is.EqualTo (ValidatedDefinitionDescription.FromDefinition (parentDefinition)));
    }

    [Test]
    public void FromDefinition_WithSignature ()
    {
      var declaringDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (object));
      var methodDefinition = DefinitionObjectMother.CreateMethodDefinition (declaringDefinition, typeof (object).GetMethod ("ToString"));
      
      var description = ValidatedDefinitionDescription.FromDefinition (methodDefinition);

      Assert.That (description.Signature, Is.EqualTo ("System.String()"));
    }

    [Test]
    public void Equals_True_WithNulls ()
    {
      var one = new ValidatedDefinitionDescription ("a", "b", null, null);
      var two = new ValidatedDefinitionDescription ("a", "b", null, null);

      Assert.That (one.Equals ((object) two), Is.True);
      Assert.That (one.Equals (two), Is.True);
      Assert.That (one == two, Is.True);
      Assert.That (one != two, Is.False);
    }

    [Test]
    public void Equals_True_WithoutNulls ()
    {
      var one = new ValidatedDefinitionDescription ("a", "b", "s", new ValidatedDefinitionDescription ("c", "d", null, null));
      var two = new ValidatedDefinitionDescription ("a", "b", "s", new ValidatedDefinitionDescription ("c", "d", null, null));

      Assert.That (one.Equals ((object) two), Is.True);
      Assert.That (one.Equals (two), Is.True);
      Assert.That (one == two, Is.True);
      Assert.That (one != two, Is.False);
    }

     [Test]
    public void Equals_False_Null ()
    {
      var one = new ValidatedDefinitionDescription ("a", "b", null, null);

      Assert.That (one.Equals ((object) null), Is.False);
      Assert.That (one.Equals (null), Is.False);
      Assert.That (one == null, Is.False);
      Assert.That (null == one, Is.False);
      Assert.That (one != null, Is.True);
      Assert.That (null != one, Is.True);
    }

    [Test]
    public void Equals_False_DifferentKind ()
    {
      var one = new ValidatedDefinitionDescription ("a", "b", null, null);
      var two = new ValidatedDefinitionDescription ("a2", "b", null, null);

      Assert.That (one.Equals ((object) two), Is.False);
      Assert.That (one.Equals (two), Is.False);
      Assert.That (one == two, Is.False);
      Assert.That (one != two, Is.True);
    }

    [Test]
    public void Equals_False_DifferentFullName ()
    {
      var one = new ValidatedDefinitionDescription ("a", "b", null, null);
      var two = new ValidatedDefinitionDescription ("a", "b2", null, null);

      Assert.That (one.Equals ((object) two), Is.False);
      Assert.That (one.Equals (two), Is.False);
      Assert.That (one == two, Is.False);
      Assert.That (one != two, Is.True);
    }

    [Test]
    public void Equals_False_DifferentSignature_OneNull ()
    {
      var one = new ValidatedDefinitionDescription ("a", "b", null, null);
      var two = new ValidatedDefinitionDescription ("a", "b", "s", null);

      Assert.That (one.Equals ((object) two), Is.False);
      Assert.That (two.Equals ((object) one), Is.False);
      Assert.That (one.Equals (two), Is.False);
      Assert.That (two.Equals (one), Is.False);
      Assert.That (one == two, Is.False);
      Assert.That (two == one, Is.False);
      Assert.That (one != two, Is.True);
      Assert.That (two != one, Is.True);
    }

    [Test]
    public void Equals_False_DifferentSignature_BothNonNull ()
    {
      var one = new ValidatedDefinitionDescription ("a", "b", "s", null);
      var two = new ValidatedDefinitionDescription ("a", "b", "s2", null);

      Assert.That (one.Equals ((object) two), Is.False);
      Assert.That (one.Equals (two), Is.False);
      Assert.That (one == two, Is.False);
      Assert.That (one != two, Is.True);
    }

    [Test]
    public void Equals_False_DifferentParent_OneNull ()
    {
      var one = new ValidatedDefinitionDescription ("a", "b", null, null);
      var two = new ValidatedDefinitionDescription ("a", "b", null, new ValidatedDefinitionDescription ("c", "d", null, null));

      Assert.That (one.Equals ((object) two), Is.False);
      Assert.That (two.Equals ((object) one), Is.False);
      Assert.That (one.Equals (two), Is.False);
      Assert.That (two.Equals (one), Is.False);
      Assert.That (one == two, Is.False);
      Assert.That (two == one, Is.False);
      Assert.That (one != two, Is.True);
      Assert.That (two != one, Is.True);
    }

    [Test]
    public void Equals_False_DifferentParent_BothNonNull ()
    {
      var one = new ValidatedDefinitionDescription ("a", "b", null, new ValidatedDefinitionDescription ("c", "d", null, null));
      var two = new ValidatedDefinitionDescription ("a", "b", null, new ValidatedDefinitionDescription ("c", "d2", null, null));

      Assert.That (one.Equals ((object) two), Is.False);
      Assert.That (one.Equals (two), Is.False);
      Assert.That (one == two, Is.False);
      Assert.That (one != two, Is.True);
    }
    
    [Test]
    public void GetHashcode_EqualObjects ()
    {
      var one = new ValidatedDefinitionDescription ("a", "b", null, null);
      var two = new ValidatedDefinitionDescription ("a", "b", null, null);

      Assert.That (one.GetHashCode(), Is.EqualTo (two.GetHashCode()));
    }

    [Test]
    public void GetHashcode_EqualObjects_WithoutNulls ()
    {
      var one = new ValidatedDefinitionDescription ("a", "b", "s", new ValidatedDefinitionDescription ("c", "d", null, null));
      var two = new ValidatedDefinitionDescription ("a", "b", "s", new ValidatedDefinitionDescription ("c", "d", null, null));

      Assert.That (one.GetHashCode(), Is.EqualTo (two.GetHashCode()));
    }

    [Test]
    public void Serialization ()
    {
      var id = new ValidatedDefinitionDescription ("a", "b", "s", new ValidatedDefinitionDescription ("c", "d", null, null));

      var deserializedID = Serializer.SerializeAndDeserialize (id);

      Assert.That (deserializedID, Is.Not.SameAs (id));
      Assert.That (deserializedID, Is.EqualTo (id));
    }

    [Test]
    public void ToString_WithNulls ()
    {
      var one = new ValidatedDefinitionDescription ("SpecialDefinition", "x.y", null, null);

      Assert.That (one.ToString(), Is.EqualTo ("x.y [SpecialDefinition]"));
    }

    [Test]
    public void ToString_WithoutNulls ()
    {
      var one = new ValidatedDefinitionDescription (
          "SpecialDefinition", "x.y", "System.Int32(System.String)", new ValidatedDefinitionDescription ("a", "b", null, null));

      Assert.That (one.ToString(), Is.EqualTo ("b [a] -> x.y [SpecialDefinition, System.Int32(System.String)]"));
    }
  }
}