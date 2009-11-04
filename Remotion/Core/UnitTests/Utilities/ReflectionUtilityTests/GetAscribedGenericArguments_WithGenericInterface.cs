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
using System.Reflection;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ReflectionUtilityTests
{
  [TestFixture]
  public class GetAscribedGenericArguments_WithGenericInterface
  {
    [Test]
    public void ClosedGenericType ()
    {
      Assert.AreEqual (
          new Type[] {typeof (ParameterType)},
          ReflectionUtility.GetAscribedGenericArguments (typeof (TypeWithGenericInterface<ParameterType>), typeof (IGenericInterface<>)));

      Assert.AreEqual (
          new Type[] {typeof (ParameterType)},
          ReflectionUtility.GetAscribedGenericArguments (typeof (IGenericInterface<ParameterType>), typeof (IGenericInterface<>)));
    }

    [Test]
    public void ClosedGenericType_WithDerivedType ()
    {
      Assert.AreEqual (
          new Type[] {typeof (ParameterType)},
          ReflectionUtility.GetAscribedGenericArguments (typeof (DerivedTypeWithGenericInterface<ParameterType>), typeof (IGenericInterface<>)));

      Assert.AreEqual (
          new Type[] {typeof (ParameterType)},
          ReflectionUtility.GetAscribedGenericArguments (typeof (IDerivedGenericInterface<ParameterType>), typeof (IGenericInterface<>)));
    }

    [Test]
    public void ClosedGenericType_WithTwoTypeParameters ()
    {
      Assert.AreEqual (
          new Type[] {typeof (ParameterType), typeof (int)},
          ReflectionUtility.GetAscribedGenericArguments (typeof (TypeWithGenericInterface<ParameterType, int>), typeof (IGenericInterface<,>)));
    }

    [Test]
    public void OpenGenericType_WithClass ()
    {
      Type[] types = ReflectionUtility.GetAscribedGenericArguments (typeof (TypeWithGenericInterface<>), typeof (IGenericInterface<>));
      Assert.AreEqual (1, types.Length);
      Assert.AreEqual ("T", types[0].Name);
    }

    [Test]
    public void OpenGenericType_WihtInterface ()
    {
      Type[] types = ReflectionUtility.GetAscribedGenericArguments (typeof (IGenericInterface<>), typeof (IGenericInterface<>));
      Assert.AreEqual (1, types.Length);
      Assert.AreEqual ("T", types[0].Name);
    }

    [Test]
    public void OpenGenericType_WithTwoTypeParameters ()
    {
      Type[] types = ReflectionUtility.GetAscribedGenericArguments (typeof (TypeWithGenericInterface<,>), typeof (IGenericInterface<,>));
      Assert.AreEqual (2, types.Length);
      Assert.AreEqual ("T1", types[0].Name);
      Assert.AreEqual ("T2", types[1].Name);
    }

    [Test]
    public void OpenGenericType_WithOneOpenTypeParameter ()
    {
      Type[] types = ReflectionUtility.GetAscribedGenericArguments (typeof (TypeWithDerivedOpenGenericInterface<>), typeof (IGenericInterface<,>));
      Assert.AreEqual (2, types.Length);
      Assert.AreEqual (typeof (ParameterType), types[0]);
      Assert.AreEqual ("T", types[1].Name);
    }

    [Test]
    public void ClosedDerivedGenericType ()
    {
      Assert.AreEqual (
          new Type[] {typeof (ParameterType)},
          ReflectionUtility.GetAscribedGenericArguments (typeof (TypeWithDerivedGenericInterface<ParameterType>), typeof (IGenericInterface<>)));
    }

    [Test]
    public void OpenDerivedGenericType_WithClass ()
    {
      Type[] types = ReflectionUtility.GetAscribedGenericArguments (typeof (TypeWithDerivedGenericInterface<>), typeof (IGenericInterface<>));
      Assert.AreEqual (1, types.Length);
      Assert.AreEqual ("T", types[0].Name);


      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (IDerivedGenericInterface<>), typeof (IGenericInterface<>)));
    }

    [Test]
    public void OpenDerivedGenericType_WithInterface ()
    {
      Type[] types = ReflectionUtility.GetAscribedGenericArguments (typeof (IDerivedGenericInterface<>), typeof (IGenericInterface<>));
      Assert.AreEqual (1, types.Length);
      Assert.AreEqual ("T", types[0].Name);
    }

    [Test]
    public void NonGenericDerivedGenericType ()
    {
      Assert.AreEqual (
          new Type[] {typeof (ParameterType)},
          ReflectionUtility.GetAscribedGenericArguments (typeof (TypeWithDerivedGenericInterface), typeof (IGenericInterface<>)));

      Assert.AreEqual (
          new Type[] { typeof (ParameterType) },
          ReflectionUtility.GetAscribedGenericArguments (typeof (IDerivedGenericInterface), typeof (IGenericInterface<>)));

      Assert.AreEqual (
          new Type[] { typeof (ParameterType) },
          ReflectionUtility.GetAscribedGenericArguments (typeof (IDoubleDerivedGenericInterface), typeof (IGenericInterface<>)));
    }

    [Test]
    [ExpectedException (typeof (AmbiguousMatchException), ExpectedMessage = "The type Remotion.UnitTests.Utilities.ReflectionUtilityTests."
        + "IDoubleInheritingGenericInterface implements the given interface type Remotion.UnitTests.Utilities.ReflectionUtilityTests."
        + "IGenericInterface`1 more than once.")]
    public void TwoSetsOfArguments ()
    {
      ReflectionUtility.GetAscribedGenericArguments (typeof (IDoubleInheritingGenericInterface), typeof (IGenericInterface<>));
    }

    [Test]
    public void TwoSetsOfArgumentsOfWhichOneFitsTheRequest ()
    {
      Assert.AreEqual (new Type[] {typeof (int) },
          ReflectionUtility.GetAscribedGenericArguments (typeof (IDoubleInheritingGenericInterface), typeof (IGenericInterface<int>)));
    }

    [Test]
    public void ClosedGenericDerivedGenericType ()
    {
      Assert.AreEqual (
          new Type[] {typeof (ParameterType)},
          ReflectionUtility.GetAscribedGenericArguments (typeof (TypeWithGenericDerivedGenericInterface<int>), typeof (IGenericInterface<>)));
    }

    [Test]
    public void OpenGenericDerivedGenericType ()
    {
      Assert.AreEqual (
          new Type[] {typeof (ParameterType)},
          ReflectionUtility.GetAscribedGenericArguments (typeof (TypeWithGenericDerivedGenericInterface<>), typeof (IGenericInterface<>)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException),
        ExpectedMessage =
        "Argument type has type Remotion.UnitTests.Utilities.ReflectionUtilityTests.TypeWithBaseInterface when type "
        + "Remotion.UnitTests.Utilities.ReflectionUtilityTests.IGenericInterface`1[T] was expected.\r\n"
        + "Parameter name: type")]
    public void BaseType ()
    {
      ReflectionUtility.GetAscribedGenericArguments (typeof (TypeWithBaseInterface), typeof (IGenericInterface<>));
    }
  }
}
