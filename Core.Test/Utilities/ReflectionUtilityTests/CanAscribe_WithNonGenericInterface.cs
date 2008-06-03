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
  public class CanAscribe_WithNonGenericInterface
  {
    [Test]
    public void DerivedType ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (IDerivedInterface), typeof (IDerivedInterface)));
    }

    [Test]
    public void BaseType ()
    {
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (IBaseInterface), typeof (IDerivedInterface)));
    }
  }
}
