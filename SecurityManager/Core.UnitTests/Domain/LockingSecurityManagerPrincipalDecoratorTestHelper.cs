// This file is part of re-strict (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Threading;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.SecurityManager.Domain;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain
{
  public class LockingSecurityManagerPrincipalDecoratorTestHelper
  {
    private readonly LockingSecurityManagerPrincipalDecorator _lockingDecorator;
    private readonly ISecurityManagerPrincipal _innerMock;

    public LockingSecurityManagerPrincipalDecoratorTestHelper (
        LockingSecurityManagerPrincipalDecorator lockingDecorator, ISecurityManagerPrincipal innerMock)
    {
      ArgumentUtility.CheckNotNull ("lockingDecorator", lockingDecorator);
      ArgumentUtility.CheckNotNull ("innerMock", innerMock);

      _lockingDecorator = lockingDecorator;
      _innerMock = innerMock;
    }

    public void ExpectSynchronizedDelegation<TResult> (Func<ISecurityManagerPrincipal, TResult> action, TResult fakeResult)
    {
      _innerMock
          .Expect (mock => action (mock))
          .Return (fakeResult)
          .WhenCalled (mi => CheckLockIsHeld (_lockingDecorator));
      _innerMock.Replay();

      TResult actualResult = action (_lockingDecorator);

      _innerMock.VerifyAllExpectations();
      Assert.That (actualResult, Is.SameAs (fakeResult));
    }

    public void ExpectSynchronizedDelegation (Action<ISecurityManagerPrincipal> action)
    {
      _innerMock
          .Expect (action)
          .WhenCalled (mi => CheckLockIsHeld (_lockingDecorator));
      _innerMock.Replay();

      action (_lockingDecorator);

      _innerMock.VerifyAllExpectations();
    }

    public void CheckLockIsHeld (LockingSecurityManagerPrincipalDecorator lockingDecorator)
    {
      ArgumentUtility.CheckNotNull ("lockingDecorator", lockingDecorator);

      bool lockAcquired = TryAcquireLockFromOtherThread (lockingDecorator);
      Assert.That (lockAcquired, Is.False, "Parallel thread should have been blocked.");
    }

    public void CheckLockIsNotHeld (LockingSecurityManagerPrincipalDecorator lockingDecorator)
    {
      ArgumentUtility.CheckNotNull ("lockingDecorator", lockingDecorator);

      bool lockAcquired = TryAcquireLockFromOtherThread (lockingDecorator);
      Assert.That (lockAcquired, Is.True, "Parallel thread should not have been blocked.");
    }

    private bool TryAcquireLockFromOtherThread (LockingSecurityManagerPrincipalDecorator lockingDecorator)
    {
      var lockObject = PrivateInvoke.GetNonPublicField (lockingDecorator, "_lock");
      Assert.That (lockObject, Is.Not.Null);

      bool lockAcquired = true;
      ThreadRunner.Run (() => lockAcquired = Monitor.TryEnter (lockObject, TimeSpan.Zero));
      return lockAcquired;
    }
  }
}