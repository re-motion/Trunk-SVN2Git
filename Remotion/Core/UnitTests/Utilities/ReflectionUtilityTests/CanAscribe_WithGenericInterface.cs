// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
  public class CanAscribe_WithGenericInterface
  {
    [Test]
    public void ClosedGenericInterface ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (TypeWithGenericInterface<ParameterType>), typeof (IGenericInterface<>)));
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (TypeWithGenericInterface<ParameterType>), typeof (IGenericInterface<ParameterType>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (TypeWithGenericInterface<ParameterType>), typeof (IGenericInterface<object>)));

      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (IGenericInterface<ParameterType>), typeof (IGenericInterface<>)));
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (IGenericInterface<ParameterType>), typeof (IGenericInterface<ParameterType>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (IGenericInterface<ParameterType>), typeof (IGenericInterface<object>)));
    }

    [Test]
    public void ClosedGenericInterface_WithDerivedType ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (DerivedTypeWithGenericInterface<ParameterType>), typeof (IGenericInterface<>)));
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (DerivedTypeWithGenericInterface<ParameterType>), typeof (IGenericInterface<ParameterType>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (DerivedTypeWithGenericInterface<ParameterType>), typeof (IGenericInterface<object>)));

      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (IDerivedGenericInterface<ParameterType>), typeof (IGenericInterface<>)));
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (IDerivedGenericInterface<ParameterType>), typeof (IGenericInterface<ParameterType>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (IDerivedGenericInterface<ParameterType>), typeof (IGenericInterface<object>)));
    }

    [Test]
    public void ClosedGenericInterface_WithTwoTypeParameters ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (TypeWithGenericInterface<ParameterType, int>), typeof (IGenericInterface<,>)));
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (TypeWithGenericInterface<ParameterType, int>), typeof (IGenericInterface<ParameterType, int>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (TypeWithGenericInterface<ParameterType, int>), typeof (IGenericInterface<object, int>)));
    }

    [Test]
    public void OpenGenericInterface ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (TypeWithGenericInterface<>), typeof (IGenericInterface<>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (TypeWithGenericInterface<>), typeof (IGenericInterface<ParameterType>)));

      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (IGenericInterface<>), typeof (IGenericInterface<>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (IGenericInterface<>), typeof (IGenericInterface<ParameterType>)));
    }

    [Test]
    public void OpenGenericInterface_WithTwoTypeParameters ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (TypeWithGenericInterface<,>), typeof (IGenericInterface<,>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (TypeWithGenericInterface<,>), typeof (IGenericInterface<ParameterType,int>)));
    }

    [Test]
    public void OpenGenericInterface_WithOneOpenTypeParameter ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (TypeWithDerivedOpenGenericInterface<>), typeof (IGenericInterface<,>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (TypeWithDerivedOpenGenericInterface<>), typeof (IGenericInterface<ParameterType,int>)));
    }

    [Test]
    public void ClosedDerivedGenericInterface ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (TypeWithDerivedGenericInterface<ParameterType>), typeof (IGenericInterface<>)));
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (TypeWithDerivedGenericInterface<ParameterType>), typeof (IGenericInterface<ParameterType>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (TypeWithDerivedGenericInterface<ParameterType>), typeof (IGenericInterface<object>)));
    }

    [Test]
    public void OpenDerivedGenericInterface ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (TypeWithDerivedGenericInterface<>), typeof (IGenericInterface<>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (TypeWithDerivedGenericInterface<>), typeof (IGenericInterface<ParameterType>)));
      
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (IDerivedGenericInterface<>), typeof (IGenericInterface<>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (IDerivedGenericInterface<>), typeof (IGenericInterface<ParameterType>)));
    }

    [Test]
    public void NonGenericDerivedGenericInterface ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (TypeWithDerivedGenericInterface), typeof (IGenericInterface<>)));
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (TypeWithDerivedGenericInterface), typeof (IGenericInterface<ParameterType>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (TypeWithDerivedGenericInterface), typeof (IGenericInterface<object>)));

      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (IDerivedGenericInterface), typeof (IGenericInterface<>)));
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (IDerivedGenericInterface), typeof (IGenericInterface<ParameterType>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (IDerivedGenericInterface), typeof (IGenericInterface<object>)));
    }

    [Test]
    public void ClosedGenericDerivedGenericInterface ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (TypeWithGenericDerivedGenericInterface<int>), typeof (IGenericInterface<>)));
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (TypeWithGenericDerivedGenericInterface<int>), typeof (IGenericInterface<ParameterType>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (TypeWithGenericDerivedGenericInterface<int>), typeof (IGenericInterface<object>)));
    }

    [Test]
    public void OpenGenericDerivedGenericInterface ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (TypeWithGenericDerivedGenericInterface<>), typeof (IGenericInterface<>)));
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (TypeWithGenericDerivedGenericInterface<>), typeof (IGenericInterface<ParameterType>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (TypeWithGenericDerivedGenericInterface<>), typeof (IGenericInterface<object>)));
    }

    [Test]
    public void BaseInterface ()
    {
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (TypeWithBaseInterface), typeof (IGenericInterface<>)));
    }
  }
}
