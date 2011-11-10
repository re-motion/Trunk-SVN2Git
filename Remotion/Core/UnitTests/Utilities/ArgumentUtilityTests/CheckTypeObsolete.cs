// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
