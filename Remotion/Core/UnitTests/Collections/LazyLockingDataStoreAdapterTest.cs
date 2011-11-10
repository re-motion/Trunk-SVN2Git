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

//
using System;
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
      ExpectSynchronizedDelegation (
          store => store.ContainsKey ("test"), 
          store => store.ContainsKey ("test"), 
          true,
          true);

      ExpectSynchronizedDelegation (
          store => store.ContainsKey ("test"),
          store => store.ContainsKey ("test"),
          false,
          false);
    }

    [Test]
    public void Add ()
    {
      var value = new object ();

      ExpectSynchronizedDelegation (
          store => store.Add ("key", value),
          store => store.Add (Arg.Is ("key"), Arg<DoubleCheckedLockingContainer<object>>.Matches (c => c.Value == value)));
    }

    [Test]
    public void Remove ()
    {
      ExpectSynchronizedDelegation (
          store => store.Remove ("key"),
          store => store.Remove ("key"),
          true,
          true);

      ExpectSynchronizedDelegation (
          store => store.Remove ("key"),
          store => store.Remove ("key"),
          false,
          false);
    }

    [Test]
    public void Clear ()
    {
      ExpectSynchronizedDelegation (
          store => store.Clear(),
          store => store.Clear());
    }

    [Test]
    public void GetValue ()
    {
      var value = new object();
      var doubleCheckedLockingContainer = CreateContainerThatChecksForNotProtected (value);

      ExpectSynchronizedDelegation (
          store => store["test"],
          store => store["test"],
          doubleCheckedLockingContainer,
          value);
    }

    [Test]
    public void SetValue ()
    {
      var value = new object ();

      ExpectSynchronizedDelegation (
          store => store["key"] = value,
          store => store[Arg.Is ("key")] = Arg<DoubleCheckedLockingContainer<object>>.Matches (c => c.Value == value));
    }

    [Test]
    public void GetValueOrDefault_ValueFound ()
    {
      var value = new object ();
      var doubleCheckedLockingContainer = CreateContainerThatChecksForNotProtected (value);

      ExpectSynchronizedDelegation (
          store => store.GetValueOrDefault ("test"),
          store => store.GetValueOrDefault ("test"),
          doubleCheckedLockingContainer,
          value);
    }

    [Test]
    public void GetValueOrDefault_NoValueFound ()
    {
      ExpectSynchronizedDelegation (
          store => store.GetValueOrDefault ("test"),
          store => store.GetValueOrDefault ("test"),
          null,
          null);
    }

    [Test]
    public void TryGetValue_ValueFound ()
    {
      var value = new object ();
      var doubleCheckedLockingContainer = CreateContainerThatChecksForNotProtected (value);

      object result = null;
      ExpectSynchronizedDelegation (
          store => store.TryGetValue ("key", out result),
          store => store.TryGetValue (
              Arg.Is ("key"),
              out Arg<DoubleCheckedLockingContainer<object>>.Out (doubleCheckedLockingContainer).Dummy),
          true,
          true);

      Assert.That (result, Is.SameAs (value));
    }

    [Test]
    public void TryGetValue_NoValueFound ()
    {
      object result = null;
      ExpectSynchronizedDelegation (
          store => store.TryGetValue ("key", out result),
          store => store.TryGetValue (
              Arg.Is ("key"),
              out Arg<DoubleCheckedLockingContainer<object>>.Out (null).Dummy),
          false,
          false);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetOrCreateValue ()
    {
      var value = new object ();
      var doubleCheckedLockingContainer = CreateContainerThatChecksForNotProtected (value);

      ExpectSynchronizedDelegation (
          store => store.GetOrCreateValue ("key", key => key + "123"),
          store => store.GetOrCreateValue (
              Arg.Is ("key"),
              Arg<Func<string, DoubleCheckedLockingContainer<object>>>.Matches (f => f ("Test").Value.Equals ("Test123"))),
          doubleCheckedLockingContainer,
          value);
    }

    private void CheckInnerDataStoreIsProtected ()
    {
      var lockingDataStoreDecorator = GetLockingDataStoreDecorator (_store);
      LockingDataStoreDecoratorTestHelper.CheckLockIsHeld (lockingDataStoreDecorator);
    }

    private void CheckInnerDataStoreIsNotProtected ()
    {
      var lockingDataStoreDecorator = GetLockingDataStoreDecorator (_store);
      LockingDataStoreDecoratorTestHelper.CheckLockIsNotHeld (lockingDataStoreDecorator);
    }

    private DoubleCheckedLockingContainer<object> CreateContainerThatChecksForNotProtected (object value)
    {
      return new DoubleCheckedLockingContainer<object> (() =>
      {
        CheckInnerDataStoreIsNotProtected();
        return value;
      });
    }

    private LockingDataStoreDecorator<string, DoubleCheckedLockingContainer<object>> GetLockingDataStoreDecorator (
        LazyLockingDataStoreAdapter<string, object> lazyLockingDataStoreAdapter)
    {
      return (LockingDataStoreDecorator<string, DoubleCheckedLockingContainer<object>>) 
          PrivateInvoke.GetNonPublicField (lazyLockingDataStoreAdapter, "_innerDataStore");
    }

    private void ExpectSynchronizedDelegation<TResultOfExpectation, TResultOfMethod> (
        Func<IDataStore<string, object>, TResultOfMethod> action,
        Func<IDataStore<string, DoubleCheckedLockingContainer<object>>, TResultOfExpectation> expectedInnerAction,
        TResultOfExpectation innerResult,
        TResultOfMethod expectedResult)
    {
      _innerDataStoreMock.BackToRecord();
      _innerDataStoreMock
          .Expect (mock => expectedInnerAction (mock))
          .Return (innerResult)
          .WhenCalled (mi => CheckInnerDataStoreIsProtected());
      _innerDataStoreMock.Replay ();

      var actualResult = action (_store);

      _innerDataStoreMock.VerifyAllExpectations ();
      Assert.That (actualResult, Is.EqualTo (expectedResult));
    }

    private void ExpectSynchronizedDelegation (
        Action<IDataStore<string, object>> action, 
        Action<IDataStore<string, DoubleCheckedLockingContainer<object>>> expectedInnerAction)
    {
      _innerDataStoreMock
          .Expect (expectedInnerAction)
          .WhenCalled (mi => CheckInnerDataStoreIsProtected ());
      _innerDataStoreMock.Replay ();

      action (_store);

      _innerDataStoreMock.VerifyAllExpectations ();
    }
  }
}