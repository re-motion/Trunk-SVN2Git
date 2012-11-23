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
using Remotion.Collections;
using Remotion.Development.RhinoMocks.UnitTesting.Threading;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class LockingCacheDecoratorTest
  {
    private LockingCacheDecorator<string, int> _decorator;

    private LockingDecoratorTestHelper<ICache<string, int>> _helper;

    [SetUp]
    public void SetUp ()
    {
      var innerCacheMock = MockRepository.GenerateStrictMock<ICache<string, int>>();

      _decorator = new LockingCacheDecorator<string, int> (innerCacheMock);

      var lockObject = PrivateInvoke.GetNonPublicField (_decorator, "_lock");
      _helper = new LockingDecoratorTestHelper<ICache<string, int>> (_decorator, lockObject, innerCacheMock);
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (((INullObject) _decorator).IsNull, Is.False);
    }

    [Test]
    public void GetOrCreateValue ()
    {
      _helper.ExpectSynchronizedDelegation (cache => cache.GetOrCreateValue ("hugo", delegate { return 3; }), 17);
    }

    [Test]
    public void TryGetValue ()
    {
      int value;
      _helper.ExpectSynchronizedDelegation (store => store.TryGetValue ("hugo", out value), true);
    }

    [Test]
    public void Clear ()
    {
      _helper.ExpectSynchronizedDelegation (store => store.Clear());
    }

    [Test]
    public void Serializable ()
    {
      Serializer.SerializeAndDeserialize (new LockingCacheDecorator<string, int> (new Cache<string, int>()));
    }
  }
}