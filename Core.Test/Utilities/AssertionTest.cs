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
using Assertion=Remotion.Utilities.Assertion;
using AssertionException=Remotion.Utilities.AssertionException;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class AssertionTest
  {
    [Test]
    public void TestIsTrueHolds ()
    {
      Assertion.IsTrue (true);
    }

    [Test]
    [ExpectedException (typeof (AssertionException))]
    public void TestIsTrueFails ()
    {
      Assertion.IsTrue (false);
    }
  }
}
