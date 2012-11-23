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
using NUnit.Framework;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Development.RhinoMocks.UnitTesting.Threading
{
  /// <summary>
  /// A helper class for testing locking decorators.
  /// </summary>
  /// <typeparam name="T">The interface type of the decorator.</typeparam>
  public class LockingDecoratorTestHelper<T>
      where T : class // Needed for RhinoMocks (Expect method).
  {
    private readonly T _lockingDecorator;
    private readonly object _lockObject;
    private readonly T _innerMock;

    public LockingDecoratorTestHelper (T lockingDecorator, object lockObject, T innerMock)
    {
      ArgumentUtility.CheckNotNull ("lockingDecorator", lockingDecorator);
      ArgumentUtility.CheckNotNull ("lockObject", lockObject);
      ArgumentUtility.CheckNotNull ("innerMock", innerMock);

      _lockingDecorator = lockingDecorator;
      _lockObject = lockObject;
      _innerMock = innerMock;
    }

    public void ExpectSynchronizedDelegation<TResult> (Func<T, TResult> action, TResult fakeResult)
    {
      ArgumentUtility.CheckNotNull ("action", action);
      ArgumentUtility.CheckNotNull ("fakeResult", fakeResult);

      _innerMock
          .Expect (mock => action (mock))
          .Return (fakeResult)
          .WhenCalled (mi => LockTestHelper.CheckLockIsHeld (_lockObject));

      var actualResult = action (_lockingDecorator);

      _innerMock.VerifyAllExpectations();
      Assert.That (actualResult, Is.EqualTo (fakeResult));
    }

    public void ExpectSynchronizedDelegation (Action<T> action)
    {
      ArgumentUtility.CheckNotNull ("action", action);

      _innerMock
          .Expect (action)
          .WhenCalled (mi => LockTestHelper.CheckLockIsHeld (_lockObject));

      action (_lockingDecorator);

      _innerMock.VerifyAllExpectations();
    }
  }
}