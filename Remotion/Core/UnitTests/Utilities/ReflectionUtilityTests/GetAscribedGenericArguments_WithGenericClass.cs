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
  public class GetAscribedGenericArguments_WithGenericClass
  {
    [Test]
    public void ClosedGenericType ()
    {
      Assert.That (ReflectionUtility.GetAscribedGenericArguments (typeof (GenericType<ParameterType>), typeof (GenericType<>)), Is.EqualTo (new Type[] {typeof (ParameterType)}));

      Assert.That (ReflectionUtility.GetAscribedGenericArguments (typeof (GenericType<ParameterType>), typeof (GenericType<ParameterType>)), Is.EqualTo (new Type[] {typeof (ParameterType)}));
    }

    [Test]
    public void ClosedGenericType_WithTwoTypeParameters ()
    {
      Assert.That (ReflectionUtility.GetAscribedGenericArguments (typeof (GenericType<ParameterType, int>), typeof (GenericType<,>)), Is.EqualTo (new Type[] {typeof (ParameterType), typeof (int)}));

      Assert.That (ReflectionUtility.GetAscribedGenericArguments (typeof (GenericType<ParameterType, int>), typeof (GenericType<ParameterType, int>)), Is.EqualTo (new Type[] {typeof (ParameterType), typeof (int)}));
    }

    [Test]
    public void OpenGenericType ()
    {
      Type[] types = ReflectionUtility.GetAscribedGenericArguments (typeof (GenericType<>), typeof (GenericType<>));
      Assert.That (types.Length, Is.EqualTo (1));
      Assert.That (types[0].Name, Is.EqualTo ("T"));
    }

    [Test]
    public void OpenGenericType_WithTwoTypeParameters ()
    {
      Type[] types = ReflectionUtility.GetAscribedGenericArguments (typeof (GenericType<,>), typeof (GenericType<,>));
      Assert.That (types.Length, Is.EqualTo (2));
      Assert.That (types[0].Name, Is.EqualTo ("T1"));
      Assert.That (types[1].Name, Is.EqualTo ("T2"));
    }

    [Test]
    public void OpenGenericType_WithOneOpenTypeParameter ()
    {
      Type[] types = ReflectionUtility.GetAscribedGenericArguments (typeof (DerivedOpenGenericType<>), typeof (GenericType<,>));
      Assert.That (types.Length, Is.EqualTo (2));
      Assert.That (types[0], Is.EqualTo (typeof (ParameterType)));
      Assert.That (types[1].Name, Is.EqualTo ("T"));
    }

    [Test]
    public void ClosedDerivedGenericType ()
    {
      Assert.That (ReflectionUtility.GetAscribedGenericArguments (typeof (DerivedGenericType<ParameterType>), typeof (GenericType<>)), Is.EqualTo (new Type[] {typeof (ParameterType)}));

      Assert.That (ReflectionUtility.GetAscribedGenericArguments (typeof (DerivedGenericType<ParameterType>), typeof (GenericType<ParameterType>)), Is.EqualTo (new Type[] {typeof (ParameterType)}));
    }

    [Test]
    public void OpenDerivedGenericType ()
    {
      Type[] types = ReflectionUtility.GetAscribedGenericArguments (typeof (DerivedGenericType<>), typeof (GenericType<>));
      Assert.That (types.Length, Is.EqualTo (1));
      Assert.That (types[0].Name, Is.EqualTo ("T"));
    }

    [Test]
    public void NonGenericDerivedGenericType ()
    {
      Assert.That (ReflectionUtility.GetAscribedGenericArguments (typeof (DerivedGenericType), typeof (GenericType<>)), Is.EqualTo (new Type[] {typeof (ParameterType)}));

      Assert.That (ReflectionUtility.GetAscribedGenericArguments (typeof (DerivedGenericType), typeof (GenericType<ParameterType>)), Is.EqualTo (new Type[] {typeof (ParameterType)}));
    }

    [Test]
    public void ClosedGenericDerivedGenericType ()
    {
      Assert.That (ReflectionUtility.GetAscribedGenericArguments (typeof (GenericDerivedGenericType<int>), typeof (GenericType<>)), Is.EqualTo (new Type[] {typeof (ParameterType)}));

      Assert.That (ReflectionUtility.GetAscribedGenericArguments (typeof (GenericDerivedGenericType<int>), typeof (GenericType<ParameterType>)), Is.EqualTo (new Type[] {typeof (ParameterType)}));
    }

    [Test]
    public void OpenGenericDerivedGenericType ()
    {
      Assert.That (ReflectionUtility.GetAscribedGenericArguments (typeof (GenericDerivedGenericType<>), typeof (GenericType<>)), Is.EqualTo (new Type[] {typeof (ParameterType)}));
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
