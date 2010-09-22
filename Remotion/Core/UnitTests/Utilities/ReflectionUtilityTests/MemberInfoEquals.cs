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
using Remotion.UnitTests.Utilities.ReflectionUtilityTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ReflectionUtilityTests
{
  [TestFixture]
  public class MemberInfoEquals
  {
    [Test]
    public void MemberInfoEquals_DifferentMethods ()
    {
      var methodInfo1 = typeof (MemberInfoEqualsTestClass<string>).GetMethod ("NonGenericMethod");
      var methodInfo2 = typeof (MemberInfoEqualsTestClass<string>).GetMethod ("GenericMethod");

      Assert.That (ReflectionUtility.MemberInfoEquals (methodInfo1, methodInfo2), Is.False);
    }

    [Test]
    public void MemberInfoEquals_DifferentDeclaringType ()
    {
      var methodInfo1 = typeof (MemberInfoEqualsTestClass<string>).GetMethod ("Trim");
      var methodInfo2 = typeof (string).GetMethod ("Trim", new Type[0]);

      Assert.That (ReflectionUtility.MemberInfoEquals (methodInfo1, methodInfo2), Is.False);
    }

    [Test]
    public void MemberInfoEquals_NonGenericMethod ()
    {
      var methodInfo1 = typeof (MemberInfoEqualsTestClass<string>).GetMethod ("NonGenericMethod");
      var methodInfo2 = typeof (MemberInfoEqualsTestClass<string>).GetMethod ("NonGenericMethod");

      Assert.That (ReflectionUtility.MemberInfoEquals (methodInfo1, methodInfo2), Is.True);
    }

    [Test]
    public void MemberInfoEquals_NonGenericMethod_DerivedMethod ()
    {
      var methodInfo1 = typeof (MemberInfoEqualsTestClass<string>).GetMethod ("NonGenericMethod");
      var methodInfo2 = typeof (DerivedMemberInfoEqualsTestClass).GetMethod ("NonGenericMethod");

      Assert.That (ReflectionUtility.MemberInfoEquals (methodInfo1, methodInfo2), Is.True);
    }

    [Test]
    public void MemberInfoEquals_GenericOpenMethod ()
    {
      var methodInfo1 = typeof (MemberInfoEqualsTestClass<string>).GetMethod ("GenericMethod");
      var methodInfo2 = typeof (MemberInfoEqualsTestClass<string>).GetMethod ("GenericMethod");

      Assert.That (ReflectionUtility.MemberInfoEquals (methodInfo1, methodInfo2), Is.True);
    }

    [Test]
    public void MemberInfoEquals_GenericClosedMethod ()
    {
      var methodInfo1 = typeof (MemberInfoEqualsTestClass<string>).GetMethod ("GenericMethod").MakeGenericMethod(typeof(int));
      var methodInfo2 = typeof (MemberInfoEqualsTestClass<string>).GetMethod ("GenericMethod").MakeGenericMethod(typeof(int));

      Assert.That (ReflectionUtility.MemberInfoEquals (methodInfo1, methodInfo2), Is.True);
    }

    [Test]
    public void MemberInfoEquals_GenericClosedMethod_DifferentTypes ()
    {
      var methodInfo1 = typeof (MemberInfoEqualsTestClass<string>).GetMethod ("GenericMethod").MakeGenericMethod (typeof (int));
      var methodInfo2 = typeof (MemberInfoEqualsTestClass<string>).GetMethod ("GenericMethod").MakeGenericMethod (typeof (bool));

      Assert.That (ReflectionUtility.MemberInfoEquals (methodInfo1, methodInfo2), Is.False);
    }

    [Test]
    public void MemberInfoEquals_DifferentProperties ()
    {
      var propertyInfo1 = typeof (MemberInfoEqualsTestClass<string>).GetProperty ("Property");
      var propertyInfo2 = typeof (MemberInfoEqualsTestClass<string>).GetProperty ("GenericProperty");

      Assert.That (ReflectionUtility.MemberInfoEquals (propertyInfo1, propertyInfo2), Is.False);
    }

    [Test]
    public void MemberInfoEquals_NonGenericProperties ()
    {
      var propertyInfo1 = typeof (MemberInfoEqualsTestClass<string>).GetProperty ("Property");
      var propertyInfo2 = typeof (MemberInfoEqualsTestClass<string>).GetProperty ("Property");

      Assert.That (ReflectionUtility.MemberInfoEquals (propertyInfo1, propertyInfo2), Is.True);
    }

    [Test]
    public void MemberInfoEquals_NonGenericProperties_DerivedType ()
    {
      var propertyInfo1 = typeof (MemberInfoEqualsTestClass<string>).GetProperty ("Property");
      var propertyInfo2 = typeof (DerivedMemberInfoEqualsTestClass).GetProperty ("Property");

      Assert.That (ReflectionUtility.MemberInfoEquals (propertyInfo1, propertyInfo2), Is.True);
    }

    [Test]
    public void MemberInfoEquals_GenericProperties ()
    {
      var propertyInfo1 = typeof (MemberInfoEqualsTestClass<string>).GetProperty ("GenericProperty");
      var propertyInfo2 = typeof (MemberInfoEqualsTestClass<string>).GetProperty ("GenericProperty");

      Assert.That (ReflectionUtility.MemberInfoEquals (propertyInfo1, propertyInfo2), Is.True);
    }

    [Test]
    public void MemberInfoEquals_GenericProperties_DifferentTypes ()
    {
      var propertyInfo1 = typeof (MemberInfoEqualsTestClass<string>).GetProperty ("GenericProperty");
      var propertyInfo2 = typeof (MemberInfoEqualsTestClass<bool>).GetProperty ("GenericProperty");

      Assert.That (ReflectionUtility.MemberInfoEquals (propertyInfo1, propertyInfo2), Is.False);
    }

    [Test]
    public void MemberInfoEquals_Field ()
    {
      var fieldInfo1 = typeof (MemberInfoEqualsTestClass<string>).GetProperty ("PublicField");
      var fieldInfo2 = typeof (MemberInfoEqualsTestClass<string>).GetProperty ("PublicField");

      Assert.That (ReflectionUtility.MemberInfoEquals (fieldInfo1, fieldInfo2), Is.True);
    }
  }
}