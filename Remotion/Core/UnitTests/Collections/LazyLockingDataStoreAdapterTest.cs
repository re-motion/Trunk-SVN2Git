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
using Rhino.Mocks;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class LazyLockingDataStoreAdapterTest
  {
    private IDataStore<string, DoubleCheckedLockingContainer<object>> _innerDataStoreMock;
    private LazyLockingDataStoreAdapter<string, object> _store;

    [SetUp]
    public void SetUp ()
    {
      _innerDataStoreMock = MockRepository.GenerateStrictMock<IDataStore<string, DoubleCheckedLockingContainer<object>>>();
      _store = new LazyLockingDataStoreAdapter<string, object> (_innerDataStoreMock);
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (((INullObject) _store).IsNull, Is.False);
    }

    [Test]
    public void ContainsKey_True ()
    {
      _innerDataStoreMock
          .Expect (mock => mock.ContainsKey ("test"))
          .Return (true)
          .WhenCalled (mi => CheckInnerDataStoreProtected ());
      _innerDataStoreMock.Replay();

      var result = _store.ContainsKey ("test");

      _innerDataStoreMock.VerifyAllExpectations();
      Assert.That (result, Is.True);
    }

    [Test]
    public void ContainsKey_False ()
    {
      _innerDataStoreMock
          .Expect (mock => mock.ContainsKey ("test"))
          .Return (false)
          .WhenCalled (mi => CheckInnerDataStoreProtected ());
      _innerDataStoreMock.Replay();

      var result = _store.ContainsKey ("test");

      _innerDataStoreMock.VerifyAllExpectations();
      Assert.That (result, Is.False);
    }

    [Test]
    public void Add ()
    {
      var value = new object();

      _innerDataStoreMock
          .Expect (mock => mock.Add (Arg.Is ("key"), Arg<DoubleCheckedLockingContainer<object>>.Matches (c => c.Value == value)))
          .WhenCalled (mi => CheckInnerDataStoreProtected());
      _innerDataStoreMock.Replay();

      _store.Add ("key", value);

      _innerDataStoreMock.VerifyAllExpectations();
    }

    [Test]
    public void Remove ()
    {
      _innerDataStoreMock
          .Expect (mock => mock.Remove ("key"))
          .Return (true)
          .WhenCalled (mi => CheckInnerDataStoreProtected ());
      _innerDataStoreMock.Replay();

      var result = _store.Remove ("key");

      _innerDataStoreMock.VerifyAllExpectations();
      Assert.That (result, Is.True);
    }

    [Test]
    public void Clear ()
    {
      _innerDataStoreMock
          .Expect (mock => mock.Clear())
          .WhenCalled (mi => CheckInnerDataStoreProtected ());
      _innerDataStoreMock.Replay();

      _store.Clear();

      _innerDataStoreMock.VerifyAllExpectations();
    }

    [Test]
    public void GetValue ()
    {
      var value = new object();
      var doubleCheckedLockingContainer = CreateContainerThatChecksForNotProtected (value);

      _innerDataStoreMock
          .Expect (mock => mock["test"])
          .Return (doubleCheckedLockingContainer)
          .WhenCalled (mi => CheckInnerDataStoreProtected ());
      _innerDataStoreMock.Replay();

      var result = _store["test"];

      _innerDataStoreMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (value));
    }

    [Test]
    public void SetValue ()
    {
      var value = new object();

      _innerDataStoreMock
          .Expect (mock => (mock[Arg.Is ("test")] = Arg<DoubleCheckedLockingContainer<object>>.Matches (c => c.Value == value)))
          .WhenCalled (mi => CheckInnerDataStoreProtected ());
      _innerDataStoreMock.Replay();

      _store["test"] = value;

      _innerDataStoreMock.VerifyAllExpectations();
    }

    [Test]
    public void GetValueOrDefault_ValueFound ()
    {
      var value = new object();
      var doubleCheckedLockingContainer = CreateContainerThatChecksForNotProtected (value);

      _innerDataStoreMock
          .Expect (mock => mock.GetValueOrDefault ("test"))
          .Return (doubleCheckedLockingContainer)
          .WhenCalled (mi => CheckInnerDataStoreProtected ());
      _innerDataStoreMock.Replay();

      var result = _store.GetValueOrDefault ("test");

      _innerDataStoreMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (value));
    }

    [Test]
    public void GetValueOrDefault_NoValueFound ()
    {
      _innerDataStoreMock
          .Expect (mock => mock.GetValueOrDefault ("test"))
          .Return (null)
          .WhenCalled (mi => CheckInnerDataStoreProtected ());
      _innerDataStoreMock.Replay ();

      var result = _store.GetValueOrDefault ("test");

      _innerDataStoreMock.VerifyAllExpectations ();
      Assert.That (result, Is.Null);
    }

    [Test]
    public void TryGetValue_ValueFound ()
    {
      var value = new object();
      var doubleCheckedLockingContainer = CreateContainerThatChecksForNotProtected (value);

      _innerDataStoreMock
          .Expect (
              mock => mock.TryGetValue (
                  Arg.Is ("key"),
                  out Arg<DoubleCheckedLockingContainer<object>>.Out (doubleCheckedLockingContainer).Dummy))
          .Return (true)
          .WhenCalled (mi => CheckInnerDataStoreProtected ());
      _innerDataStoreMock.Replay();

      object result;
      var found = _store.TryGetValue ("key", out result);

      Assert.That (found, Is.True);
      Assert.That (result, Is.SameAs (value));
    }

    [Test]
    public void TryGetValue_NoValueFound ()
    {
      _innerDataStoreMock
          .Expect (mock => mock.TryGetValue (Arg.Is ("key"), out Arg<DoubleCheckedLockingContainer<object>>.Out (null).Dummy))
          .Return (false)
          .WhenCalled (mi => CheckInnerDataStoreProtected ());
      _innerDataStoreMock.Replay();

      object result;
      var found = _store.TryGetValue ("key", out result);

      Assert.That (found, Is.False);
      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetOrCreateValue ()
    {
      var value = new object();
      var doubleCheckedLockingContainer = CreateContainerThatChecksForNotProtected (value);

      _innerDataStoreMock
          .Expect (mock => mock.GetOrCreateValue (
              Arg.Is("Test"), 
              Arg<Func<string, DoubleCheckedLockingContainer<object>>>.Matches (f => f("Test").Value.Equals ("Test123"))))
          .Return (doubleCheckedLockingContainer)
          .WhenCalled (mi => CheckInnerDataStoreProtected ());
      _innerDataStoreMock.Replay();

      var result = _store.GetOrCreateValue ("Test", key => key + "123");

      _innerDataStoreMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (value));
    }

    private DoubleCheckedLockingContainer<object> CreateContainerThatChecksForNotProtected (object value)
    {
      return new DoubleCheckedLockingContainer<object> (() =>
      {
        CheckInnerDataStoreNotProtected();
        return value;
      });
    }

    private void CheckInnerDataStoreProtected ()
    {
      bool lockAcquired = TryAcquireLockFromOtherThread();
      Assert.That (lockAcquired, Is.False, "Parallel thread should have been blocked.");
    }

    private void CheckInnerDataStoreNotProtected ()
    {
      bool lockAcquired = TryAcquireLockFromOtherThread ();
      Assert.That (lockAcquired, Is.True, "Parallel thread should not have been blocked.");
    }

    private bool TryAcquireLockFromOtherThread ()
    {
      var innerDataStore = 
          (LockingDataStoreDecorator<string, DoubleCheckedLockingContainer<object>>) PrivateInvoke.GetNonPublicField (_store, "_innerDataStore");
      var lockObject = PrivateInvoke.GetNonPublicField (innerDataStore, "_lock");
      Assert.That (lockObject, Is.Not.Null);

      bool lockAcquired = true;
      ThreadRunner.Run (() => lockAcquired = Monitor.TryEnter (lockObject, TimeSpan.Zero));
      return lockAcquired;
    }
  }
}