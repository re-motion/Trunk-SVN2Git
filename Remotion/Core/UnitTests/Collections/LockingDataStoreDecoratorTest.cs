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
  public class LockingDataStoreDecoratorTest
  {
    private LockingDataStoreDecorator<string, int> _decorator;

    private LockingDecoratorTestHelper<IDataStore<string, int>> _helper;

    [SetUp]
    public void SetUp ()
    {
      var innerDataStoreMock = MockRepository.GenerateStrictMock<IDataStore<string, int>>();

      _decorator = new LockingDataStoreDecorator<string, int> (innerDataStoreMock);

      var lockObject = PrivateInvoke.GetNonPublicField (_decorator, "_lock");
      _helper = new LockingDecoratorTestHelper<IDataStore<string, int>> (_decorator, lockObject, innerDataStoreMock);
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (((INullObject) _decorator).IsNull, Is.False);
    }

    [Test]
    public void ContainsKey ()
    {
      _helper.ExpectSynchronizedDelegation (store => store.ContainsKey ("a"), true);
    }

    [Test]
    public void Add ()
    {
      _helper.ExpectSynchronizedDelegation (store => store.Add ("a", 1));
    }

    [Test]
    public void Remove ()
    {
      _helper.ExpectSynchronizedDelegation (store => store.Remove ("b"), true);
    }

    [Test]
    public void Clear ()
    {
      _helper.ExpectSynchronizedDelegation (store => store.Clear());
    }

    [Test]
    public void Get_Value ()
    {
      _helper.ExpectSynchronizedDelegation (store => store["c"], 47);
    }

    [Test]
    public void Set_Value ()
    {
      _helper.ExpectSynchronizedDelegation (store => store["c"] = 17);
    }

    [Test]
    public void GetValueOrDefault ()
    {
      _helper.ExpectSynchronizedDelegation (store => store.GetValueOrDefault ("hugo"), 7);
    }

    [Test]
    public void TryGetValue ()
    {
      int value;
      _helper.ExpectSynchronizedDelegation (store => store.TryGetValue ("hugo", out value), true);
    }

    [Test]
    public void GetOrCreateValue ()
    {
      _helper.ExpectSynchronizedDelegation (store => store.GetOrCreateValue ("hugo", delegate { return 3; }), 17);
    }

    [Test]
    public void Serializable ()
    {
      Serializer.SerializeAndDeserialize (new LockingDataStoreDecorator<string, int> (new SimpleDataStore<string, int>()));
    }

    [Test]
    public void GetOrCreateValue_WithNestedTryGetValue_HasNoNestedValue ()
    {
      int expected = 13;

      var store = new LockingDataStoreDecorator<string, int?> (new SimpleDataStore<string, int?>());

      var actualValue = store.GetOrCreateValue (
          "key1",
          delegate (string key)
          {
            Assert.That (
                () => store.TryGetValue (key, out _),
                Throws.InvalidOperationException.With.Message.StringStarting (
                    "An attempt was detected to access the value for key ('key1') during the factory operation of GetOrCreateValue(key, factory)."));

            return expected;
          });

      Assert.That (actualValue, Is.EqualTo (expected));
    }

    [Test]
    public void GetOrCreateValue_WithNestedGetOrCreatedValue_ThrowsInvalidOperationException ()
    {
      int expected = 13;

      var store = new LockingDataStoreDecorator<string, int?> (new SimpleDataStore<string, int?>());

      var actualValue = store.GetOrCreateValue (
              "key1",
              delegate (string key)
              {
                Assert.That (
                    () => store.GetOrCreateValue (key, nestedKey => 13),
                    Throws.InvalidOperationException.With.Message.StringStarting (
                        "An attempt was detected to access the value for key ('key1') during the factory operation of GetOrCreateValue(key, factory)."));

                return expected;
          });

      Assert.That (actualValue, Is.EqualTo (expected));

      int? actualValue2;
      Assert.That (store.TryGetValue ("key1", out actualValue2), Is.True);
      Assert.That (actualValue2, Is.EqualTo (expected));
    }
  }
}