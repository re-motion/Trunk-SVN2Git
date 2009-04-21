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
  public class GetAscribedGenericArguments_WithNonGenericInterface
  {
    [Test]
    public void DerivedType ()
    {
      Assert.AreEqual (new Type[0], ReflectionUtility.GetAscribedGenericArguments (typeof (DerivedType), typeof (IDerivedInterface)));
    }

    [Test]
    public void DerivedInterface ()
    {
      Assert.AreEqual (new Type[0], ReflectionUtility.GetAscribedGenericArguments (typeof (IDerivedInterface), typeof (IDerivedInterface)));
    }

    [Test]
    public void DerivedInterfaceFromBaseInterface ()
    {
      Assert.AreEqual (new Type[0], ReflectionUtility.GetAscribedGenericArguments (typeof (IDerivedInterface), typeof (IBaseInterface)));
    }

    [Test]
    public void DerivedTypeFromBaseInterface ()
    {
      Assert.AreEqual (new Type[0], ReflectionUtility.GetAscribedGenericArguments (typeof (DerivedType), typeof (IBaseInterface)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException),
        ExpectedMessage =
        "Argument type has type Remotion.UnitTests.Utilities.ReflectionUtilityTests.BaseType when type "
        + "Remotion.UnitTests.Utilities.ReflectionUtilityTests.IDerivedInterface was expected.\r\n"
        + "Parameter name: type")]
    public void BaseType ()
    {
      ReflectionUtility.GetAscribedGenericArguments (typeof (BaseType), typeof (IDerivedInterface));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException),
        ExpectedMessage =
        "Argument type has type Remotion.UnitTests.Utilities.ReflectionUtilityTests.IBaseInterface when type "
        + "Remotion.UnitTests.Utilities.ReflectionUtilityTests.IDerivedInterface was expected.\r\n"
        + "Parameter name: type")]
    public void BaseInterface ()
    {
      ReflectionUtility.GetAscribedGenericArguments (typeof (IBaseInterface), typeof (IDerivedInterface));
    }
  }
}
