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

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class CacheFactoryTest
  {
    private StringComparer _comparer;

    [SetUp]
    public void SetUp ()
    {
      _comparer = StringComparer.InvariantCultureIgnoreCase;
    }

    [Test]
    public void Create ()
    {
      var result = CacheFactory.Create<string, int> ();

      Assert.That (result, Is.TypeOf (typeof (Cache<string, int>)));
    }

    [Test]
    public void Create_IEqualityComparerOverload ()
    {
      var result = CacheFactory.Create<string, int> (_comparer);

      Assert.That (result, Is.TypeOf (typeof (Cache<string, int>)));
      var innerCache = PrivateInvoke.GetNonPublicField (result, "_dataStore");
      Assert.That (innerCache, Is.TypeOf (typeof (SimpleDataStore<string, int>)));
      Assert.That (((SimpleDataStore<string, int>) innerCache).Comparer, Is.SameAs (_comparer));
    }

    [Test]
    public void CreateWithLocking ()
    {
      var result = CacheFactory.CreateWithLocking<string, int> ();

      Assert.That (result, Is.TypeOf (typeof (LockingCacheDecorator<string, int>)));
      var innerCache = PrivateInvoke.GetNonPublicField (result, "_innerCache");
      Assert.That (innerCache, Is.TypeOf (typeof (Cache<string, int>)));
    }

    [Test]
    public void CreateWithLocking_IEqualityComparerOverload ()
    {
      var result = CacheFactory.CreateWithLocking<string, int> (_comparer);

      Assert.That (result, Is.TypeOf (typeof (LockingCacheDecorator<string, int>)));
      var innerCache = PrivateInvoke.GetNonPublicField (result, "_innerCache");
      Assert.That (innerCache, Is.TypeOf (typeof (Cache<string, int>)));
      var innerDataStore = PrivateInvoke.GetNonPublicField (innerCache, "_dataStore");
      Assert.That (((SimpleDataStore<string, int>) innerDataStore).Comparer, Is.SameAs (_comparer));
    }

    [Test]
    public void CreateWithLazyLocking ()
    {
      var result = CacheFactory.CreateWithLazyLocking<string, object> ();

      Assert.That (result, Is.TypeOf (typeof (LazyLockingCachingAdapter<string, object>)));
      var innerCache = PrivateInvoke.GetNonPublicField (result, "_innerCache");
      Assert.That (
          innerCache, 
          Is.TypeOf (typeof (LockingCacheDecorator<string, DoubleCheckedLockingContainer<LazyLockingCachingAdapter<string, object>.Wrapper>>)));
      var innerInnerCache = PrivateInvoke.GetNonPublicField (innerCache, "_innerCache");
      Assert.That (innerInnerCache, Is.TypeOf (typeof (Cache<string, DoubleCheckedLockingContainer<LazyLockingCachingAdapter<string, object>.Wrapper>>)));
    }

    [Test]
    public void CreateWithLazyLocking_IEqualityComparerOverload ()
    {
      var result = CacheFactory.CreateWithLazyLocking<string, object> (_comparer);

      Assert.That (result, Is.TypeOf (typeof (LazyLockingCachingAdapter<string, object>)));
      var innerCache = PrivateInvoke.GetNonPublicField (result, "_innerCache");
      Assert.That (innerCache, Is.TypeOf (typeof (LockingCacheDecorator<string, DoubleCheckedLockingContainer<LazyLockingCachingAdapter<string, object>.Wrapper>>)));
      var innerInnerCache = PrivateInvoke.GetNonPublicField (innerCache, "_innerCache");
      Assert.That (innerInnerCache, Is.TypeOf (typeof (Cache<string, DoubleCheckedLockingContainer<LazyLockingCachingAdapter<string, object>.Wrapper>>)));
      var underlyingDataStore = PrivateInvoke.GetNonPublicField (innerInnerCache, "_dataStore");
      Assert.That (underlyingDataStore, Is.TypeOf (typeof (SimpleDataStore<string, DoubleCheckedLockingContainer<LazyLockingCachingAdapter<string, object>.Wrapper>>)));
      Assert.That (((SimpleDataStore<string, DoubleCheckedLockingContainer<LazyLockingCachingAdapter<string, object>.Wrapper>>) underlyingDataStore).Comparer, Is.SameAs (_comparer));
    }
  }
}