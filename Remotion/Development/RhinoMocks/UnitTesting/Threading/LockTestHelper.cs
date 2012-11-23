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
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Development.RhinoMocks.UnitTesting.Threading
{
  /// <summary>
  /// A helper class for testing locking code.
  /// </summary>
  public static class LockTestHelper
  {
    public static void CheckLockIsHeld (object lockObject)
    {
      ArgumentUtility.CheckNotNull ("lockObject", lockObject);

      var lockAcquired = CouldAcquireLockFromOtherThread (lockObject);
      Assert.That (lockAcquired, Is.False, "Parallel thread should have been blocked.");
    }

    public static void CheckLockIsNotHeld (object lockObject)
    {
      ArgumentUtility.CheckNotNull ("lockObject", lockObject);

      var lockAcquired = CouldAcquireLockFromOtherThread (lockObject);
      Assert.That (lockAcquired, Is.True, "Parallel thread should NOT have been blocked.");
    }

    public static bool CouldAcquireLockFromOtherThread (object lockObject)
    {
      ArgumentUtility.CheckNotNull ("lockObject", lockObject);

      var lockAcquired = false;
      ThreadRunner.Run (
          () =>
          {
            lockAcquired = Monitor.TryEnter (lockObject);
            if (lockAcquired)
              Monitor.Exit (lockObject);
          });

      return lockAcquired;
    }
  }
}