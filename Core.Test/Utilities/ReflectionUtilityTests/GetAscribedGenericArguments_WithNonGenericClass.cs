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
  public class GetAscribedGenericArguments_WithNonGenericClass
  {
    [Test]
    public void DerivedType ()
    {
      Assert.AreEqual (new Type[0], ReflectionUtility.GetAscribedGenericArguments (typeof (DerivedType), typeof (DerivedType)));
    }

    [Test]
    public void DerivedTypeFromBaseType ()
    {
      Assert.AreEqual (new Type[0], ReflectionUtility.GetAscribedGenericArguments (typeof (DerivedType), typeof (BaseType)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException),
        ExpectedMessage =
        "Argument type has type Remotion.UnitTests.Utilities.ReflectionUtilityTests.BaseType when type "
        + "Remotion.UnitTests.Utilities.ReflectionUtilityTests.DerivedType was expected.\r\n"
        + "Parameter name: type")]
    public void BaseType ()
    {
      ReflectionUtility.GetAscribedGenericArguments (typeof (BaseType), typeof (DerivedType));
    }
  }
}
