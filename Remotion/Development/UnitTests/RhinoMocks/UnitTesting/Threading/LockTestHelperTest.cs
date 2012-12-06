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
using System.Threading;
using NUnit.Framework;
using Remotion.Development.RhinoMocks.UnitTesting.Threading;

namespace Remotion.Development.UnitTests.RhinoMocks.UnitTesting.Threading
{
  [TestFixture]
  public class LockTestHelperTest
  {
    private object _lockObject;

    [SetUp]
    public void SetUp ()
    {
      _lockObject = new object();
    }

    [TearDown]
    public void TearDown ()
    {
      Assert.That (Monitor.TryEnter (_lockObject), Is.True, "Lock should have been released.");
    }

    [Test]
    public void CheckLockIsHeld ()
    {
      lock (_lockObject)
        LockTestHelper.CheckLockIsHeld (_lockObject);
    }

    [Test]
    [ExpectedException (typeof (AssertionException), MatchType = MessageMatch.Contains,
        ExpectedMessage = "Parallel thread should have been blocked.")]
    public void CheckLockIsHeld_Throws ()
    {
      LockTestHelper.CheckLockIsHeld (_lockObject);
    }

    [Test]
    public void CheckLockIsNotHeld ()
    {
      LockTestHelper.CheckLockIsNotHeld (_lockObject);
    }

    [Test]
    [ExpectedException (typeof (AssertionException), MatchType = MessageMatch.Contains,
        ExpectedMessage = "Parallel thread should NOT have been blocked.")]
    public void CheckLockIsNotHeld_Throws ()
    {
      lock (_lockObject)
        LockTestHelper.CheckLockIsNotHeld (_lockObject);
    }

    [Test]
    public void CouldAcquireLockFromOtherThread_True ()
    {
      Assert.That (LockTestHelper.CouldAcquireLockFromOtherThread (_lockObject), Is.True);
    }

    [Test]
    public void CouldAcquireLockFromOtherThread_False ()
    {
      lock (_lockObject)
        Assert.That (LockTestHelper.CouldAcquireLockFromOtherThread (_lockObject), Is.False);
    }
  }
}