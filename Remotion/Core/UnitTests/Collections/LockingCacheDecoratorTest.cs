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
using System;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class LockingCacheDecoratorTest
  {
    private ICache<string, int> _innerCacheMock;
    private LockingCacheDecorator<string, int> _cache;

    [SetUp]
    public void SetUp ()
    {
      _innerCacheMock = MockRepository.GenerateStrictMock<ICache<string, int>> ();
      _cache = new LockingCacheDecorator<string, int> (_innerCacheMock);
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (((INullObject) _cache).IsNull, Is.False);
    }

    [Test]
    public void GetOrCreateValue ()
    {
      ExpectSynchronizedDelegation (cache => cache.GetOrCreateValue ("hugo", delegate { return 3; }), 17);
    }

    [Test]
    public void TryGetValue ()
    {
      int value;
      ExpectSynchronizedDelegation (store => store.TryGetValue ("hugo", out value), true);
    }

    [Test]
    public void Clear ()
    {
      ExpectSynchronizedDelegation (store => store.Clear ());
    }

    [Test]
    public void Serializable ()
    {
      Serializer.SerializeAndDeserialize (new LockingCacheDecorator<string, int> (new Cache<string, int> ()));
    }

    private void ExpectSynchronizedDelegation<TResult> (Func<ICache<string, int>, TResult> action, TResult fakeResult)
    {
      _innerCacheMock
          .Expect (mock => action (mock))
          .Return (fakeResult)
          .WhenCalled (mi => LockingCacheDecoratorTestHelper.CheckLockIsHeld (_cache));
      _innerCacheMock.Replay ();

      TResult actualResult = action (_cache);

      _innerCacheMock.VerifyAllExpectations ();
      Assert.That (actualResult, Is.EqualTo (fakeResult));
    }

    private void ExpectSynchronizedDelegation (Action<ICache<string, int>> action)
    {
      _innerCacheMock
          .Expect (action)
          .WhenCalled (mi => LockingCacheDecoratorTestHelper.CheckLockIsHeld (_cache));
      _innerCacheMock.Replay ();

      action (_cache);

      _innerCacheMock.VerifyAllExpectations ();
    }
  }
}
