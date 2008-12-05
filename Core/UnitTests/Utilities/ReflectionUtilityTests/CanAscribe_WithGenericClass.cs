// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ReflectionUtilityTests
{
  [TestFixture]
  public class CanAscribe_WithGenericClass
  {
    [Test]
    public void ClosedGenericType ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (GenericType<ParameterType>), typeof (GenericType<>)));
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (GenericType<ParameterType>), typeof (GenericType<ParameterType>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (GenericType<ParameterType>), typeof (GenericType<object>)));
    }

    [Test]
    public void ClosedGenericType_WithTwoTypeParameters ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (GenericType<ParameterType, int>), typeof (GenericType<,>)));
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (GenericType<ParameterType, int>), typeof (GenericType<ParameterType, int>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (GenericType<ParameterType, int>), typeof (GenericType<object, int>)));
    }

    [Test]
    public void OpenGenericType ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (GenericType<>), typeof (GenericType<>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (GenericType<>), typeof (GenericType<ParameterType>)));
    }

    [Test]
    public void OpenGenericType_WithTwoTypeParameters ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (GenericType<>), typeof (GenericType<>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (GenericType<>), typeof (GenericType<ParameterType>)));
    }

    [Test]
    public void OpenGenericType_WithOneOpenTypeParameter ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (DerivedOpenGenericType<>), typeof (GenericType<,>)));
    }

    [Test]
    public void ClosedDerivedGenericType ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (DerivedGenericType<ParameterType>), typeof (GenericType<>)));
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (DerivedGenericType<ParameterType>), typeof (GenericType<ParameterType>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (DerivedGenericType<ParameterType>), typeof (GenericType<object>)));
    }

    [Test]
    public void OpenDerivedGenericType ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (DerivedGenericType<>), typeof (GenericType<>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (DerivedGenericType<>), typeof (GenericType<ParameterType>)));
    }

    [Test]
    public void NonGenericDerivedGenericType ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (DerivedGenericType), typeof (GenericType<>)));
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (DerivedGenericType), typeof (GenericType<ParameterType>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (DerivedGenericType), typeof (GenericType<object>)));
    }

    [Test]
    public void ClosedGenericDerivedGenericType ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (GenericDerivedGenericType<int>), typeof (GenericType<>)));
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (GenericDerivedGenericType<int>), typeof (GenericType<ParameterType>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (GenericDerivedGenericType<int>), typeof (GenericType<object>)));
    }

    [Test]
    public void OpenGenericDerivedGenericType ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (GenericDerivedGenericType<>), typeof (GenericType<>)));
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (GenericDerivedGenericType<>), typeof (GenericType<ParameterType>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (GenericDerivedGenericType<>), typeof (GenericType<object>)));
    }

    [Test]
    public void BaseType ()
    {
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (BaseType), typeof (GenericType<>)));
    }
  }
}
