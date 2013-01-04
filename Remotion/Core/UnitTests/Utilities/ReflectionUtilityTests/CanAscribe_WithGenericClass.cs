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
      Assert.That (ReflectionUtility.CanAscribe (typeof (GenericType<ParameterType>), typeof (GenericType<>)), Is.True);
      Assert.That (ReflectionUtility.CanAscribe (typeof (GenericType<ParameterType>), typeof (GenericType<ParameterType>)), Is.True);
      Assert.That (ReflectionUtility.CanAscribe (typeof (GenericType<ParameterType>), typeof (GenericType<object>)), Is.False);
    }

    [Test]
    public void ClosedGenericType_WithTwoTypeParameters ()
    {
      Assert.That (ReflectionUtility.CanAscribe (typeof (GenericType<ParameterType, int>), typeof (GenericType<,>)), Is.True);
      Assert.That (ReflectionUtility.CanAscribe (typeof (GenericType<ParameterType, int>), typeof (GenericType<ParameterType, int>)), Is.True);
      Assert.That (ReflectionUtility.CanAscribe (typeof (GenericType<ParameterType, int>), typeof (GenericType<object, int>)), Is.False);
    }

    [Test]
    public void OpenGenericType ()
    {
      Assert.That (ReflectionUtility.CanAscribe (typeof (GenericType<>), typeof (GenericType<>)), Is.True);
      Assert.That (ReflectionUtility.CanAscribe (typeof (GenericType<>), typeof (GenericType<ParameterType>)), Is.False);
    }

    [Test]
    public void OpenGenericType_WithTwoTypeParameters ()
    {
      Assert.That (ReflectionUtility.CanAscribe (typeof (GenericType<>), typeof (GenericType<>)), Is.True);
      Assert.That (ReflectionUtility.CanAscribe (typeof (GenericType<>), typeof (GenericType<ParameterType>)), Is.False);
    }

    [Test]
    public void OpenGenericType_WithOneOpenTypeParameter ()
    {
      Assert.That (ReflectionUtility.CanAscribe (typeof (DerivedOpenGenericType<>), typeof (GenericType<,>)), Is.True);
    }

    [Test]
    public void ClosedDerivedGenericType ()
    {
      Assert.That (ReflectionUtility.CanAscribe (typeof (DerivedGenericType<ParameterType>), typeof (GenericType<>)), Is.True);
      Assert.That (ReflectionUtility.CanAscribe (typeof (DerivedGenericType<ParameterType>), typeof (GenericType<ParameterType>)), Is.True);
      Assert.That (ReflectionUtility.CanAscribe (typeof (DerivedGenericType<ParameterType>), typeof (GenericType<object>)), Is.False);
    }

    [Test]
    public void OpenDerivedGenericType ()
    {
      Assert.That (ReflectionUtility.CanAscribe (typeof (DerivedGenericType<>), typeof (GenericType<>)), Is.True);
      Assert.That (ReflectionUtility.CanAscribe (typeof (DerivedGenericType<>), typeof (GenericType<ParameterType>)), Is.False);
    }

    [Test]
    public void NonGenericDerivedGenericType ()
    {
      Assert.That (ReflectionUtility.CanAscribe (typeof (DerivedGenericType), typeof (GenericType<>)), Is.True);
      Assert.That (ReflectionUtility.CanAscribe (typeof (DerivedGenericType), typeof (GenericType<ParameterType>)), Is.True);
      Assert.That (ReflectionUtility.CanAscribe (typeof (DerivedGenericType), typeof (GenericType<object>)), Is.False);
    }

    [Test]
    public void ClosedGenericDerivedGenericType ()
    {
      Assert.That (ReflectionUtility.CanAscribe (typeof (GenericDerivedGenericType<int>), typeof (GenericType<>)), Is.True);
      Assert.That (ReflectionUtility.CanAscribe (typeof (GenericDerivedGenericType<int>), typeof (GenericType<ParameterType>)), Is.True);
      Assert.That (ReflectionUtility.CanAscribe (typeof (GenericDerivedGenericType<int>), typeof (GenericType<object>)), Is.False);
    }

    [Test]
    public void OpenGenericDerivedGenericType ()
    {
      Assert.That (ReflectionUtility.CanAscribe (typeof (GenericDerivedGenericType<>), typeof (GenericType<>)), Is.True);
      Assert.That (ReflectionUtility.CanAscribe (typeof (GenericDerivedGenericType<>), typeof (GenericType<ParameterType>)), Is.True);
      Assert.That (ReflectionUtility.CanAscribe (typeof (GenericDerivedGenericType<>), typeof (GenericType<object>)), Is.False);
    }

    [Test]
    public void BaseType ()
    {
      Assert.That (ReflectionUtility.CanAscribe (typeof (BaseType), typeof (GenericType<>)), Is.False);
    }
  }
}
