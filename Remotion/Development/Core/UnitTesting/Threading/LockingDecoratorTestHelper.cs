//// This file is part of the re-motion Core Framework (www.re-motion.org)
//// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//// 
//// The re-motion Core Framework is free software; you can redistribute it 
//// and/or modify it under the terms of the GNU Lesser General Public License 
//// as published by the Free Software Foundation; either version 2.1 of the 
//// License, or (at your option) any later version.
//// 
//// re-motion is distributed in the hope that it will be useful, 
//// but WITHOUT ANY WARRANTY; without even the implied warranty of 
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
//// GNU Lesser General Public License for more details.
//// 
//// You should have received a copy of the GNU Lesser General Public License
//// along with re-motion; if not, see http://www.gnu.org/licenses.
//// 

//using System;
//using Remotion.Collections;
//using Remotion.Utilities;

//namespace Remotion.Development.UnitTesting.Threading
//{
//  public class LockingDecoratorTestHelper<TInnerMock, TLock>
//  {
//    private readonly TInnerMock _innerMock;
//    private readonly TLock _lockObject;

//    public LockingDecoratorTestHelper (TInnerMock innerMock, TLock lockObject)
//    {
//      ArgumentUtility.CheckNotNull ("innerMock", innerMock);
//      ArgumentUtility.CheckNotNull ("lockObject", lockObject);

//      _innerMock = innerMock;
//      _lockObject = lockObject;
//    }

//    public void ExpectSynchronizedDelegation<TResult> (Func<TInnerMock, TResult> action, TResult fakeResult)
//    {
//      _innerDataStoreMock
//          .Expect (mock => action (mock))
//          .Return (fakeResult)
//          .WhenCalled (mi => LockingDataStoreDecoratorTestHelper.CheckLockIsHeld (_store));
//      _innerDataStoreMock.Replay ();

//      TResult actualResult = action (_store);

//      _innerDataStoreMock.VerifyAllExpectations ();
//      Assert.That (actualResult, Is.EqualTo (fakeResult));
//    }

//    public void ExpectSynchronizedDelegation (Action<IDataStore<string, int>> action)
//    {
//      _innerDataStoreMock
//          .Expect (action)
//          .WhenCalled (mi => LockingDataStoreDecoratorTestHelper.CheckLockIsHeld (_store));
//      _innerDataStoreMock.Replay ();

//      action (_store);

//      _innerDataStoreMock.VerifyAllExpectations ();
//    }
//  }
//}