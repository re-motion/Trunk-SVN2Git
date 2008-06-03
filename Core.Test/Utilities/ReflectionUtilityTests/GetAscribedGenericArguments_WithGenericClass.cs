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
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ReflectionUtilityTests
{
  [TestFixture]
  public class GetAscribedGenericArguments_WithGenericClass
  {
    [Test]
    public void ClosedGenericType ()
    {
      Assert.AreEqual (
          new Type[] {typeof (ParameterType)},
          ReflectionUtility.GetAscribedGenericArguments (typeof (GenericType<ParameterType>), typeof (GenericType<>)));

      Assert.AreEqual (
          new Type[] {typeof (ParameterType)},
          ReflectionUtility.GetAscribedGenericArguments (typeof (GenericType<ParameterType>), typeof (GenericType<ParameterType>)));
    }

    [Test]
    public void ClosedGenericType_WithTwoTypeParameters ()
    {
      Assert.AreEqual (
          new Type[] {typeof (ParameterType), typeof (int)},
          ReflectionUtility.GetAscribedGenericArguments (typeof (GenericType<ParameterType, int>), typeof (GenericType<,>)));

      Assert.AreEqual (
          new Type[] {typeof (ParameterType), typeof (int)},
          ReflectionUtility.GetAscribedGenericArguments (typeof (GenericType<ParameterType, int>), typeof (GenericType<ParameterType, int>)));
    }

    [Test]
    public void OpenGenericType ()
    {
      Type[] types = ReflectionUtility.GetAscribedGenericArguments (typeof (GenericType<>), typeof (GenericType<>));
      Assert.AreEqual (1, types.Length);
      Assert.AreEqual ("T", types[0].Name);
    }

    [Test]
    public void OpenGenericType_WithTwoTypeParameters ()
    {
      Type[] types = ReflectionUtility.GetAscribedGenericArguments (typeof (GenericType<,>), typeof (GenericType<,>));
      Assert.AreEqual (2, types.Length);
      Assert.AreEqual ("T1", types[0].Name);
      Assert.AreEqual ("T2", types[1].Name);
    }

    [Test]
    public void OpenGenericType_WithOneOpenTypeParameter ()
    {
      Type[] types = ReflectionUtility.GetAscribedGenericArguments (typeof (DerivedOpenGenericType<>), typeof (GenericType<,>));
      Assert.AreEqual (2, types.Length);
      Assert.AreEqual (typeof (ParameterType), types[0]);
      Assert.AreEqual ("T", types[1].Name);
    }

    [Test]
    public void ClosedDerivedGenericType ()
    {
      Assert.AreEqual (
          new Type[] {typeof (ParameterType)},
          ReflectionUtility.GetAscribedGenericArguments (typeof (DerivedGenericType<ParameterType>), typeof (GenericType<>)));

      Assert.AreEqual (
          new Type[] {typeof (ParameterType)},
          ReflectionUtility.GetAscribedGenericArguments (typeof (DerivedGenericType<ParameterType>), typeof (GenericType<ParameterType>)));
    }

    [Test]
    public void OpenDerivedGenericType ()
    {
      Type[] types = ReflectionUtility.GetAscribedGenericArguments (typeof (DerivedGenericType<>), typeof (GenericType<>));
      Assert.AreEqual (1, types.Length);
      Assert.AreEqual ("T", types[0].Name);
    }

    [Test]
    public void NonGenericDerivedGenericType ()
    {
      Assert.AreEqual (
          new Type[] {typeof (ParameterType)},
          ReflectionUtility.GetAscribedGenericArguments (typeof (DerivedGenericType), typeof (GenericType<>)));

      Assert.AreEqual (
          new Type[] {typeof (ParameterType)},
          ReflectionUtility.GetAscribedGenericArguments (typeof (DerivedGenericType), typeof (GenericType<ParameterType>)));
    }

    [Test]
    public void ClosedGenericDerivedGenericType ()
    {
      Assert.AreEqual (
          new Type[] {typeof (ParameterType)},
          ReflectionUtility.GetAscribedGenericArguments (typeof (GenericDerivedGenericType<int>), typeof (GenericType<>)));

      Assert.AreEqual (
          new Type[] {typeof (ParameterType)},
          ReflectionUtility.GetAscribedGenericArguments (typeof (GenericDerivedGenericType<int>), typeof (GenericType<ParameterType>)));
    }

    [Test]
    public void OpenGenericDerivedGenericType ()
    {
      Assert.AreEqual (
          new Type[] {typeof (ParameterType)},
          ReflectionUtility.GetAscribedGenericArguments (typeof (GenericDerivedGenericType<>), typeof (GenericType<>)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException),
        ExpectedMessage =
        "Argument type has type Remotion.UnitTests.Utilities.ReflectionUtilityTests.BaseType when type "
        + "Remotion.UnitTests.Utilities.ReflectionUtilityTests.GenericType`1[T] was expected.\r\n"
        + "Parameter name: type")]
    public void BaseType ()
    {
      ReflectionUtility.GetAscribedGenericArguments (typeof (BaseType), typeof (GenericType<>));
    }
  }
}
