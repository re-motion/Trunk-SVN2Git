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

//
using System;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class LazyLockingCachingAdapterTest
  {
    private ICache<string, DoubleCheckedLockingContainer<object>> _innerCacheMock;
    private LazyLockingCachingAdapter<string, object> _cachingAdapter;

    [SetUp]
    public void SetUp ()
    {
      _innerCacheMock = MockRepository.GenerateStrictMock<ICache<string, DoubleCheckedLockingContainer<object>>> ();
      _cachingAdapter = new LazyLockingCachingAdapter<string, object> (_innerCacheMock);
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (((INullObject) _cachingAdapter).IsNull, Is.False);
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
    public void Clear ()
    {
      ExpectSynchronizedDelegation (
          store => store.Clear (),
          store => store.Clear ());
    }

    private void CheckInnerCacheIsProtected ()
    {
      var lockingCacheDecorator = GetLockingCacheDecorator (_cachingAdapter);
      LockingCacheDecoratorTestHelper.CheckLockIsHeld (lockingCacheDecorator);
    }

    private void CheckInnerCacheIsNotProtected ()
    {
      var lockingCacheDecorator = GetLockingCacheDecorator (_cachingAdapter);
      LockingCacheDecoratorTestHelper.CheckLockIsNotHeld (lockingCacheDecorator);
    }

    private DoubleCheckedLockingContainer<object> CreateContainerThatChecksForNotProtected (object value)
    {
      return new DoubleCheckedLockingContainer<object> (() =>
      {
        CheckInnerCacheIsNotProtected ();
        return value;
      });
    }

    private LockingCacheDecorator<string, DoubleCheckedLockingContainer<object>> GetLockingCacheDecorator (
        LazyLockingCachingAdapter<string, object> lazyLockingCacheAdapter)
    {
      return (LockingCacheDecorator<string, DoubleCheckedLockingContainer<object>>)
          PrivateInvoke.GetNonPublicField (lazyLockingCacheAdapter, "_innerCache");
    }

    private void ExpectSynchronizedDelegation<TResultOfExpectation, TResultOfMethod> (
        Func<ICache<string, object>, TResultOfMethod> action,
        Func<ICache<string, DoubleCheckedLockingContainer<object>>, TResultOfExpectation> expectedInnerAction,
        TResultOfExpectation innerResult,
        TResultOfMethod expectedResult)
    {
      _innerCacheMock.BackToRecord ();
      _innerCacheMock
          .Expect (mock => expectedInnerAction (mock))
          .Return (innerResult)
          .WhenCalled (mi => CheckInnerCacheIsProtected ());
      _innerCacheMock.Replay ();

      var actualResult = action (_cachingAdapter);

      _innerCacheMock.VerifyAllExpectations ();
      Assert.That (actualResult, Is.EqualTo (expectedResult));
    }

    private void ExpectSynchronizedDelegation (
        Action<ICache<string, object>> action,
        Action<ICache<string, DoubleCheckedLockingContainer<object>>> expectedInnerAction)
    {
      _innerCacheMock
          .Expect (expectedInnerAction)
          .WhenCalled (mi => CheckInnerCacheIsProtected ());
      _innerCacheMock.Replay ();

      action (_cachingAdapter);

      _innerCacheMock.VerifyAllExpectations ();
    }

  }
}