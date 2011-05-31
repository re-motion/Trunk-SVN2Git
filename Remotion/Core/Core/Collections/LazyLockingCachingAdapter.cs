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
using Remotion.Utilities;

namespace Remotion.Collections
{
  /// <summary>
  /// Adapts an implementation of <see cref="ICache{TKey,TValue}"/> that stores <see cref="DoubleCheckedLockingContainer{T}"/> holding
  /// lazily constructed values so that users can access those values without indirection, and in a thread-safe way. Use 
  /// <see cref="CacheFactory.CreateWithLazyLocking{TKey,TValue}()"/> to create an instance of this type.
  /// </summary>
  /// <typeparam name="TKey">The type of the keys.</typeparam>
  /// <typeparam name="TValue">The type of the values.</typeparam>
  /// <threadsafety static="true" instance="true" />
  /// <remarks>
  /// This class internally combines a <see cref="LockingCacheDecorator{TKey,TValue}"/> with <see cref="DoubleCheckedLockingContainer{T}"/>
  /// instances. This leads to the effect that the lock used for the synchronization of the data store is always held for a very short time only,
  /// even if the factory delegate for a specific value takes a long time to execute.
  /// </remarks>
  // Not serializable because DoubleCheckedLockingContainer is not serializable.
  public class LazyLockingCachingAdapter<TKey, TValue> : ICache<TKey, TValue> where TValue : class 
  {
    private readonly LockingCacheDecorator<TKey, DoubleCheckedLockingContainer<TValue>> _innerCache;

    public LazyLockingCachingAdapter (ICache<TKey, DoubleCheckedLockingContainer<TValue>> innerCache)
    {
      ArgumentUtility.CheckNotNull ("innerCache", innerCache);

      _innerCache = new LockingCacheDecorator<TKey, DoubleCheckedLockingContainer<TValue>> (innerCache);
    }

    public bool IsNull
    {
      get { return false; }
    }

    public TValue GetOrCreateValue (TKey key, Func<TKey, TValue> valueFactory)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("valueFactory", valueFactory);

      var doubleCheckedLockingContainer = _innerCache.GetOrCreateValue (key, k => new DoubleCheckedLockingContainer<TValue> (() => valueFactory (k)));
      return doubleCheckedLockingContainer.Value;
    }

    public bool TryGetValue (TKey key, out TValue value)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      DoubleCheckedLockingContainer<TValue> result;

      if (_innerCache.TryGetValue (key, out result))
      {
        value = result.Value;
        return true;
      }

      value = null;
      return false;
    }

    public void Clear ()
    {
      _innerCache.Clear ();
    }
  }
}