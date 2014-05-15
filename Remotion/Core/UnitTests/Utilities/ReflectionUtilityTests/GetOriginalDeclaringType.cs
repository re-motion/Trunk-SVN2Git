// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Reflection;
using NUnit.Framework;
using Remotion.UnitTests.Utilities.ReflectionUtilityTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ReflectionUtilityTests
{
  [TestFixture]
  public class GetOriginalDeclaringType
  {
    [Test]
    public void GetOriginalDeclaringType_ForPropertyOnBaseClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<ClassWithDifferentProperties> ("String");

      Assert.That (ReflectionUtility.GetOriginalDeclaringType (propertyInfo), Is.SameAs (typeof (ClassWithDifferentProperties)));
    }

    [Test]
    public void GetOriginalDeclaringType_ForPropertyOnDerivedClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<DerivedClassWithDifferentProperties> ("OtherString");

      Assert.That (ReflectionUtility.GetOriginalDeclaringType (propertyInfo), Is.SameAs (typeof (DerivedClassWithDifferentProperties)));
    }

    [Test]
    public void GetOriginalDeclaringType_ForNewPropertyOnDerivedClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<DerivedClassWithDifferentProperties> ("String");

      Assert.That (ReflectionUtility.GetOriginalDeclaringType (propertyInfo), Is.SameAs (typeof (DerivedClassWithDifferentProperties)));
    }

    [Test]
    public void GetOriginalDeclaringType_ForOverriddenPropertyOnBaseClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<ClassWithDifferentProperties> ("Int32");

      Assert.That (ReflectionUtility.GetOriginalDeclaringType (propertyInfo), Is.SameAs (typeof (ClassWithDifferentProperties)));
    }

    [Test]
    public void GetOriginalDeclaringType_ForOverriddenPropertyOnDerivedClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<DerivedClassWithDifferentProperties> ("Int32");

      Assert.That (ReflectionUtility.GetOriginalDeclaringType (propertyInfo), Is.SameAs (typeof (ClassWithDifferentProperties)));
    }

    [Test]
    public void GetOriginalDeclaringType_ForOverriddenPropertyOnDerivedOfDerivedClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<DerivedOfDerivedClassWithDifferentProperties> ("Int32");

      Assert.That (ReflectionUtility.GetOriginalDeclaringType (propertyInfo), Is.SameAs (typeof (ClassWithDifferentProperties)));
    }


    [Test]
    public void GetOriginalDeclaringType_ForMethodOnBaseClass ()
    {
      MethodInfo MethodInfo = GetMethodInfo<ClassWithDifferentProperties> ("get_String");

      Assert.That (ReflectionUtility.GetOriginalDeclaringType (MethodInfo), Is.SameAs (typeof (ClassWithDifferentProperties)));
    }

    [Test]
    public void GetOriginalDeclaringType_ForMethodOnDerivedClass ()
    {
      MethodInfo MethodInfo = GetMethodInfo<DerivedClassWithDifferentProperties> ("get_OtherString");

      Assert.That (ReflectionUtility.GetOriginalDeclaringType (MethodInfo), Is.SameAs (typeof (DerivedClassWithDifferentProperties)));
    }

    [Test]
    public void GetOriginalDeclaringType_ForNewMethodOnDerivedClass ()
    {
      MethodInfo MethodInfo = GetMethodInfo<DerivedClassWithDifferentProperties> ("get_String");

      Assert.That (ReflectionUtility.GetOriginalDeclaringType (MethodInfo), Is.SameAs (typeof (DerivedClassWithDifferentProperties)));
    }

    [Test]
    public void GetOriginalDeclaringType_ForOverriddenMethodOnBaseClass ()
    {
      MethodInfo MethodInfo = GetMethodInfo<ClassWithDifferentProperties> ("get_Int32");

      Assert.That (ReflectionUtility.GetOriginalDeclaringType (MethodInfo), Is.SameAs (typeof (ClassWithDifferentProperties)));
    }

    [Test]
    public void GetOriginalDeclaringType_ForOverriddenMethodOnDerivedClass ()
    {
      MethodInfo MethodInfo = GetMethodInfo<DerivedClassWithDifferentProperties> ("get_Int32");

      Assert.That (ReflectionUtility.GetOriginalDeclaringType (MethodInfo), Is.SameAs (typeof (ClassWithDifferentProperties)));
    }

    [Test]
    public void GetOriginalDeclaringType_ForOverriddenMethodOnDerivedOfDerivedClass ()
    {
      MethodInfo MethodInfo = GetMethodInfo<DerivedOfDerivedClassWithDifferentProperties> ("get_Int32");

      Assert.That (ReflectionUtility.GetOriginalDeclaringType (MethodInfo), Is.SameAs (typeof (ClassWithDifferentProperties)));
    }


    protected PropertyInfo GetPropertyInfo<T> (string property)
    {
      return typeof (T).GetProperty (property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
    }

    protected MethodInfo GetMethodInfo<T> (string method)
    {
      return typeof (T).GetMethod (method, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
    }
  }
}
