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
