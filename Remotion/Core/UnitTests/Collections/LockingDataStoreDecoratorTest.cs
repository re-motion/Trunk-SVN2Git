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
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class LockingDataStoreDecoratorTest
  {
    private IDataStore<string, int> _innerDataStoreMock;
    private LockingDataStoreDecorator<string, int> _store;

    [SetUp]
    public void SetUp ()
    {
      _innerDataStoreMock = MockRepository.GenerateStrictMock<IDataStore<string, int>> ();
      _store = new LockingDataStoreDecorator<string, int> (_innerDataStoreMock);
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (((INullObject) _store).IsNull, Is.False);
    }

    [Test]
    public void ContainsKey ()
    {
      ExpectSynchronizedDelegation (store => store.ContainsKey ("a"), true);
    }

    [Test]
    public void Add ()
    {
      ExpectSynchronizedDelegation (store => store.Add ("a", 1));
    }

    [Test]
    public void Remove ()
    {
      ExpectSynchronizedDelegation (store => store.Remove ("b"), true);
    }

    [Test]
    public void Clear ()
    {
      ExpectSynchronizedDelegation (store => store.Clear ());
    }

    [Test]
    public void Get_Value ()
    {
      ExpectSynchronizedDelegation (store => store["c"], 47);
    }

    [Test]
    public void Set_Value ()
    {
      ExpectSynchronizedDelegation (store => store["c"] = 17);
    }

    [Test]
    public void GetValueOrDefault ()
    {
      ExpectSynchronizedDelegation (store => store.GetValueOrDefault ("hugo"), 7);
    }

    [Test]
    public void TryGetValue ()
    {
      int value;
      ExpectSynchronizedDelegation (store => store.TryGetValue ("hugo", out value), true);
    }

    [Test]
    public void GetOrCreateValue ()
    {
      ExpectSynchronizedDelegation (store => store.GetOrCreateValue ("hugo", delegate { return 3; }), 17);
    }

    [Test]
    public void Serializable ()
    {
      Serializer.SerializeAndDeserialize (new LockingDataStoreDecorator<string, int>(new SimpleDataStore<string, int>()));
    }

    private void ExpectSynchronizedDelegation<TResult> (Func<IDataStore<string, int>, TResult> action, TResult fakeResult)
    {
      _innerDataStoreMock
          .Expect (mock => action(mock))
          .Return (fakeResult)
          .WhenCalled (mi => LockingDataStoreDecoratorTestHelper.CheckLockIsHeld (_store));
      _innerDataStoreMock.Replay();

      TResult actualResult = action (_store);

      _innerDataStoreMock.VerifyAllExpectations();
      Assert.That (actualResult, Is.EqualTo (fakeResult));
    }

    private void ExpectSynchronizedDelegation (Action<IDataStore<string, int>> action)
    {
      _innerDataStoreMock
          .Expect (action)
          .WhenCalled (mi => LockingDataStoreDecoratorTestHelper.CheckLockIsHeld (_store));
      _innerDataStoreMock.Replay ();

      action (_store);

      _innerDataStoreMock.VerifyAllExpectations ();
    }
  }
}