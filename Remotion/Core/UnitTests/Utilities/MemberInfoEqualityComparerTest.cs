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
using Remotion.UnitTests.Utilities.MemberInfoEqualityComparerTestDomain;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class MemberInfoEqualityComparerTest
  {
    [Test]
    public void Equals_True_SameInstance ()
    {
      var one = typeof (ClassWithMethods).GetMethod ("SimpleMethod1");
      var two = one;

      var result = MemberInfoEqualityComparer.Instance.Equals (one, two);

      Assert.That (result, Is.True);
    }

    [Test]
    public void Equals_True_DifferentReflectedTypes ()
    {
      var one = typeof (ClassWithMethods).GetMethod ("SimpleMethod1");
      var two = typeof (DerivedClassWithMethods).GetMethod ("SimpleMethod1");

      Assert.That (one.DeclaringType, Is.SameAs (two.DeclaringType));

      var result = MemberInfoEqualityComparer.Instance.Equals (one, two);

      Assert.That (result, Is.True);
    }

    [Test]
    public void Equals_True_NullMembers ()
    {
      var result = MemberInfoEqualityComparer.Instance.Equals (null, null);

      Assert.That (result, Is.True);
    }

    [Test]
    public void Equals_False_NullMemberOnLeftSide ()
    {
      var one = typeof (ClassWithMethods).GetMethod ("SimpleMethod1");

      var result = MemberInfoEqualityComparer.Instance.Equals (one, null);

      Assert.That (result, Is.False);
    }

    [Test]
    public void Equals_False_NullMemberOnRightSide ()
    {
      var one = typeof (ClassWithMethods).GetMethod ("SimpleMethod1");

      var result = MemberInfoEqualityComparer.Instance.Equals (null, one);

      Assert.That (result, Is.False);
    }

    [Test]
    public void Equals_True_NullDeclaringType ()
    {
      var one = new FakeMemberInfo (null, 1, typeof (object).Module);
      var two = new FakeMemberInfo (null, 1, typeof (object).Module);

      Assert.That (one.DeclaringType, Is.SameAs (two.DeclaringType));

      var result = MemberInfoEqualityComparer.Instance.Equals (one, two);

      Assert.That (result, Is.True);
    }

    [Test]
    public void Equals_True_ArrayMembers ()
    {
      var one = typeof (int[]).GetMethod ("Set");
      var two = typeof (int[]).GetMethod ("Set");

      var result = MemberInfoEqualityComparer.Instance.Equals (one, two);

      Assert.That (result, Is.True);
    }

    [Test]
    public void Equals_True_AbstractArrayMembers ()
    {
      var one = typeof (Array).GetMethod ("CopyTo", new[] { typeof (Array), typeof (int) });
      var two = typeof (int[]).GetMethod ("CopyTo", new[] { typeof (Array), typeof (int) });

      var result = MemberInfoEqualityComparer.Instance.Equals (one, two);

      Assert.That (result, Is.True);
    }

    [Test]
    public void Equals_True_SameMember_OnSameDeclaringTypeInstantiations ()
    {
      var one = typeof (GenericClassWithMethods<object>).GetMethod ("SimpleMethod");
      var two = typeof (GenericClassWithMethods<object>).GetMethod ("SimpleMethod");

      var result = MemberInfoEqualityComparer.Instance.Equals (one, two);

      Assert.That (result, Is.True);
    }

    [Test]
    public void Equals_True_SameMember_OnGenericTypeDefinition ()
    {
      var one = typeof (GenericClassWithMethods<>).GetMethod ("SimpleMethod");
      var two = typeof (GenericClassWithMethods<>).GetMethod ("SimpleMethod");

      var result = MemberInfoEqualityComparer.Instance.Equals (one, two);

      Assert.That (result, Is.True);
    }

    [Test]
    public void Equals_True_SameMethod_WithSameMethodInstantiations ()
    {
      var one = typeof (ClassWithMethods).GetMethod ("GenericMethod").MakeGenericMethod (typeof (object));
      var two = typeof (ClassWithMethods).GetMethod ("GenericMethod").MakeGenericMethod (typeof (object));

      var result = MemberInfoEqualityComparer.Instance.Equals (one, two);

      Assert.That (result, Is.True);
    }

    [Test]
    public void Equals_True_SameMethod_GenericMethodDefinition ()
    {
      var one = typeof (ClassWithMethods).GetMethod ("GenericMethod");
      var two = typeof (ClassWithMethods).GetMethod ("GenericMethod");

      Assert.That (one.IsGenericMethodDefinition, Is.True);

      var result = MemberInfoEqualityComparer.Instance.Equals (one, two);

      Assert.That (result, Is.True);
    }

    [Test]
    public void Equals_False_DifferentMetadataTokens ()
    {
      var one = typeof (ClassWithMethods).GetMethod ("SimpleMethod1");
      var two = typeof (ClassWithMethods).GetMethod ("SimpleMethod2");

      var result = MemberInfoEqualityComparer.Instance.Equals (one, two);

      Assert.That (result, Is.False);
    }

    [Test]
    public void Equals_False_DifferentTypes ()
    {
      var one = typeof (ClassWithMethods).GetMethod ("SimpleMethod1");
      var two = new FakeMemberInfo (typeof (string), one.MetadataToken, one.Module);

      var result = MemberInfoEqualityComparer.Instance.Equals (one, two);

      Assert.That (result, Is.False);
    }

    [Test]
    public void Equals_False_DifferentModules ()
    {
      var one = new FakeMemberInfo (typeof (object), 1, typeof (object).Module);
      var two = new FakeMemberInfo (one.DeclaringType, one.MetadataToken, typeof (MemberInfoEqualityComparerTest).Module);

      var result = MemberInfoEqualityComparer.Instance.Equals (one, two);

      Assert.That (result, Is.False);
    }

    [Test]
    public void Equals_False_SameMember_OnDifferentDeclaringTypeInstantiations ()
    {
      var one = typeof (GenericClassWithMethods<string>).GetMethod ("SimpleMethod");
      var two = typeof (GenericClassWithMethods<object>).GetMethod ("SimpleMethod");

      var result = MemberInfoEqualityComparer.Instance.Equals (one, two);

      Assert.That (result, Is.False);
    }

    [Test]
    public void Equals_False_SameMethod_WithDifferentMethodInstantiations ()
    {
      var one = typeof (ClassWithMethods).GetMethod ("GenericMethod").MakeGenericMethod (typeof (string));
      var two = typeof (ClassWithMethods).GetMethod ("GenericMethod").MakeGenericMethod (typeof (object));

      var result = MemberInfoEqualityComparer.Instance.Equals (one, two);

      Assert.That (result, Is.False);
    }

    [Test]
    public void Equals_False_WithGenericMethodDefinition_AndInstantiation ()
    {
      var one = typeof (ClassWithMethods).GetMethod ("GenericMethod");
      var two = typeof (ClassWithMethods).GetMethod ("GenericMethod").MakeGenericMethod (typeof (object));

      var result = MemberInfoEqualityComparer.Instance.Equals (one, two);

      Assert.That (result, Is.False);
    }

    [Test]
    public void Equals_False_ArrayMembers ()
    {
      var one = typeof (int[]).GetMethods ()[0];
      var two = typeof (int[]).GetMethods ()[1];

      Assert.That (one, Is.Not.SameAs (two));

      var result = MemberInfoEqualityComparer.Instance.Equals (one, two);

      Assert.That (result, Is.True);
    }

    [Test]
    public void Equals_Methods ()
    {
      var methodInfo1a = typeof (ClassWithMethods).GetMethod ("SimpleMethod1");
      var methodInfo1b = typeof (DerivedClassWithMethods).GetMethod ("SimpleMethod1");
      var methodInfo2 = typeof (ClassWithMethods).GetMethod ("SimpleMethod2");
      var methodInfo3 = typeof (ClassWithMethods).GetMethod ("OverriddenMethod");
      var methodInfo4 = typeof (DerivedClassWithMethods).GetMethod ("OverriddenMethod");

      Assert.That (MemberInfoEqualityComparer.Instance.Equals (methodInfo1a, methodInfo1b), Is.True);
      Assert.That (MemberInfoEqualityComparer.Instance.Equals (methodInfo1a, methodInfo2), Is.False);
      Assert.That (MemberInfoEqualityComparer.Instance.Equals (methodInfo3, methodInfo4), Is.False);
    }

    [Test]
    public void Equals_Properties ()
    {
      var propertyInfo1a = typeof (ClassWithProperties).GetProperty ("Property1");
      var propertyInfo1b = typeof (DerivedClassWithProperties).GetProperty ("Property1");
      var propertyInfo2 = typeof (ClassWithProperties).GetProperty ("Property2");

      Assert.That (MemberInfoEqualityComparer.Instance.Equals (propertyInfo1a, propertyInfo1b), Is.True);
      Assert.That (MemberInfoEqualityComparer.Instance.Equals (propertyInfo1a, propertyInfo2), Is.False);
    }

    [Test]
    public void Equals_Field ()
    {
      var fieldInfo1a = typeof (ClassWithFields).GetField ("Field1");
      var fieldInfo1b = typeof (DerivedClassWithFields).GetField ("Field1");
      var fieldInfo2 = typeof (ClassWithFields).GetField ("Field2");

      Assert.That (MemberInfoEqualityComparer.Instance.Equals (fieldInfo1a, fieldInfo1b), Is.True);
      Assert.That (MemberInfoEqualityComparer.Instance.Equals (fieldInfo1a, fieldInfo2), Is.False);
    }

    [Test]
    public void GetHashcode_SameInstance ()
    {
      var one = typeof (ClassWithMethods).GetMethod ("SimpleMethod1");
      var two = one;

      Assert.That (MemberInfoEqualityComparer.Instance.GetHashCode (one), Is.EqualTo (MemberInfoEqualityComparer.Instance.GetHashCode (two)));
    }

    [Test]
    public void GetHashcode_DifferentReflectedType ()
    {
      var one = typeof (ClassWithMethods).GetMethod ("SimpleMethod1");
      var two = typeof (DerivedClassWithMethods).GetMethod ("SimpleMethod1");

      Assert.That (one.DeclaringType, Is.SameAs (two.DeclaringType));
      Assert.That (MemberInfoEqualityComparer.Instance.GetHashCode (one), Is.EqualTo (MemberInfoEqualityComparer.Instance.GetHashCode (two)));
    }

    [Test]
    public void GetHashcode_NullDeclaringType ()
    {
      var one = new FakeMemberInfo (null, 1, typeof (object).Module);
      var two = new FakeMemberInfo (null, 1, typeof (object).Module);

      Assert.That (one.DeclaringType, Is.SameAs (two.DeclaringType));
      Assert.That (MemberInfoEqualityComparer.Instance.GetHashCode (one), Is.EqualTo (MemberInfoEqualityComparer.Instance.GetHashCode (two)));
    }

    [Test]
    public void GetHashcode_ArrayMembers ()
    {
      var one = typeof (int[]).GetMethod ("Set");
      var two = typeof (int[]).GetMethod ("Set");

      Assert.That (MemberInfoEqualityComparer.Instance.GetHashCode (one), Is.EqualTo (MemberInfoEqualityComparer.Instance.GetHashCode (two)));
    }

    [Test]
    public void GetHashcode_AbstractArrayMembers ()
    {
      var one = typeof (Array).GetMethod ("CopyTo", new[] { typeof (Array), typeof (int) });
      var two = typeof (bool[]).GetMethod ("CopyTo", new[] { typeof (Array), typeof (int) });

      Assert.That (MemberInfoEqualityComparer.Instance.GetHashCode (one), Is.EqualTo (MemberInfoEqualityComparer.Instance.GetHashCode (two)));
    }

    [Test]
    public void GetHashcode_SameMember_OnSameDeclaringTypeInstantiations ()
    {
      var one = typeof (GenericClassWithMethods<object>).GetMethod ("SimpleMethod");
      var two = typeof (GenericClassWithMethods<object>).GetMethod ("SimpleMethod");

      Assert.That (MemberInfoEqualityComparer.Instance.GetHashCode (one), Is.EqualTo (MemberInfoEqualityComparer.Instance.GetHashCode (two)));
    }

    [Test]
    public void GetHashcode_SameMember_OnGenericTypeDefinition ()
    {
      var one = typeof (GenericClassWithMethods<>).GetMethod ("SimpleMethod");
      var two = typeof (GenericClassWithMethods<>).GetMethod ("SimpleMethod");

      Assert.That (MemberInfoEqualityComparer.Instance.GetHashCode (one), Is.EqualTo (MemberInfoEqualityComparer.Instance.GetHashCode (two)));
    }

    [Test]
    public void GetHashCode_NullMember ()
    {
      Assert.That (MemberInfoEqualityComparer.Instance.GetHashCode (null), Is.EqualTo (0));
    }

  }
}