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

namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
	[TestFixture]
  [Obsolete ("The tested methods are obsolete.")]
	public class CheckTypeObsolete
	{
    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
		public void Fail_Type ()
    {
      ArgumentUtility.CheckType ("arg", 13, typeof (string));
    }

    [Test]
		public void Succeed_ValueType ()
    {
      Assert.AreEqual (1, ArgumentUtility.CheckType ("arg", (object) 1, typeof (int)));
    }

    [Test]
    public void Succeed_NullableValueTypeNull ()
    {
      Assert.AreEqual (null, ArgumentUtility.CheckType ("arg", (object) null, typeof (int?)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void Fail_ValueTypeNull ()
    {
      ArgumentUtility.CheckType ("arg", (object) null, typeof (int));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void Fail_ValueType ()
    {
      ArgumentUtility.CheckType ("arg", (object) DateTime.MinValue, typeof (int));
    }

    [Test]
		public void Succeed_ReferenceTypeNull ()
    {
      Assert.AreEqual (null, ArgumentUtility.CheckType ("arg", (object) null, typeof (string)));
    }

    [Test]
		public void Succeed_NotNull ()
    {
      Assert.AreEqual ("test", ArgumentUtility.CheckType ("arg", "test", typeof (string)));
    }
	}
}
