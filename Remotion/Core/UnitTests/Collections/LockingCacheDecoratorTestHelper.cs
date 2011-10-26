// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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

//
using System;
using System.Threading;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.UnitTests.Collections
{
  public static class LockingCacheDecoratorTestHelper
  {
    public static void CheckLockIsHeld<TKey, TValue> (LockingCacheDecorator<TKey, TValue> lockingCacheDecorator)
    {
      ArgumentUtility.CheckNotNull ("lockingCacheDecorator", lockingCacheDecorator);

      bool lockAcquired = TryAcquireLockFromOtherThread (lockingCacheDecorator);
      Assert.That (lockAcquired, Is.False, "Parallel thread should have been blocked.");
    }

    public static void CheckLockIsNotHeld<TKey, TValue> (LockingCacheDecorator<TKey, TValue> lockingCacheDecorator)
    {
      ArgumentUtility.CheckNotNull ("lockingCacheDecorator", lockingCacheDecorator);

      bool lockAcquired = TryAcquireLockFromOtherThread (lockingCacheDecorator);
      Assert.That (lockAcquired, Is.True, "Parallel thread should not have been blocked.");
    }

    private static bool TryAcquireLockFromOtherThread<TKey, TValue> (LockingCacheDecorator<TKey, TValue> lockingCacheDecorator)
    {
      var lockObject = PrivateInvoke.GetNonPublicField (lockingCacheDecorator, "_lock");
      Assert.That (lockObject, Is.Not.Null);

      bool lockAcquired = true;
      ThreadRunner.Run (() => lockAcquired = Monitor.TryEnter (lockObject, TimeSpan.Zero));
      return lockAcquired;
    }
  }
}