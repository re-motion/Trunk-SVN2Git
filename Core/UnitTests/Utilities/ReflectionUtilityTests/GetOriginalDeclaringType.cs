/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ReflectionUtilityTests
{
  [TestFixture]
  public class GetOriginalDeclaringType
  {
    public abstract class ClassWithMixedProperties
    {
      public abstract int Int32 { get; set; }

      public virtual string String
      {
        get { return ""; }
        set { Dev.Null = value; }
      }
    }

    public abstract class DerivedClassWithMixedProperties : ClassWithMixedProperties
    {
      public override int Int32
      {
        get { return 0; }
        set { }
      }

      public abstract string OtherString { get; set; }
      public new abstract string String { get; set; }
    }

    [Test]
    public void GetOriginalDeclaringType_ForPropertyOnBaseClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<ClassWithMixedProperties> ("String");

      Assert.AreSame (
          typeof (ClassWithMixedProperties),
          ReflectionUtility.GetOriginalDeclaringType (propertyInfo));
    }

    [Test]
    public void GetOriginalDeclaringType_ForPropertyOnDerivedClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<DerivedClassWithMixedProperties> ("OtherString");

      Assert.AreSame (
          typeof (DerivedClassWithMixedProperties),
          ReflectionUtility.GetOriginalDeclaringType (propertyInfo));
    }

    [Test]
    public void GetOriginalDeclaringType_ForNewPropertyOnDerivedClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<DerivedClassWithMixedProperties> ("String");

      Assert.AreSame (
          typeof (DerivedClassWithMixedProperties),
          ReflectionUtility.GetOriginalDeclaringType (propertyInfo));
    }

    [Test]
    public void GetOriginalDeclaringType_ForOverriddenPropertyOnBaseClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<ClassWithMixedProperties> ("Int32");

      Assert.AreSame (
          typeof (ClassWithMixedProperties),
          ReflectionUtility.GetOriginalDeclaringType (propertyInfo));
    }

    [Test]
    public void GetOriginalDeclaringType_ForOverriddenPropertyOnDerivedClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<DerivedClassWithMixedProperties> ("Int32");

      Assert.AreSame (
          typeof (ClassWithMixedProperties),
          ReflectionUtility.GetOriginalDeclaringType (propertyInfo));
    }

    protected PropertyInfo GetPropertyInfo<T> (string property)
    {
      return typeof (T).GetProperty (property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
    }
  }
}
