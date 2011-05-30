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
using System.Collections.Generic;

namespace Remotion.Collections
{
  /// <summary>
  /// The <see cref="DataStoreFactory"/> provides factory methods to create new data stores.
  /// </summary>
  public static class DataStoreFactory
  {
    public static SimpleDataStore<TKey, TValue> Create<TKey, TValue> ()
    {
      return new SimpleDataStore<TKey, TValue>();
    }

    public static SimpleDataStore<TKey, TValue> Create<TKey, TValue> (IEqualityComparer<TKey> comparer)
    {
      return new SimpleDataStore<TKey, TValue> (comparer);
    }

    public static LockingDataStoreDecorator<TKey, TValue> CreateWithLocking<TKey, TValue> ()
    {
      return new LockingDataStoreDecorator<TKey, TValue> (new SimpleDataStore<TKey, TValue>());
    }

    public static LockingDataStoreDecorator<TKey, TValue> CreateWithLocking<TKey, TValue> (IEqualityComparer<TKey> comparer)
    {
      return new LockingDataStoreDecorator<TKey, TValue> (new SimpleDataStore<TKey, TValue> (comparer));
    }

    public static LazyLockingDataStoreAdapter<TKey, TValue> CreateWithLazyLocking<TKey, TValue> () where TValue: class
    {
      return new LazyLockingDataStoreAdapter<TKey, TValue> (new SimpleDataStore<TKey, DoubleCheckedLockingContainer<TValue>>());
    }

    public static LazyLockingDataStoreAdapter<TKey, TValue> CreateWithLazyLocking<TKey, TValue> (IEqualityComparer<TKey> comparer) where TValue: class
    {
      return new LazyLockingDataStoreAdapter<TKey, TValue> (new SimpleDataStore<TKey, DoubleCheckedLockingContainer<TValue>> (comparer));
    }
  }
}