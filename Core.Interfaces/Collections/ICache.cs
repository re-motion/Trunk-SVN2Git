/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;

namespace Remotion.Collections
{
  /// <summary>
  /// Provides a comnmon interface for caches, which provide efficient storage and retrieval for values that are costly to calculate.
  /// </summary>
  /// <typeparam name="TKey">The key type via which values should be indexed.</typeparam>
  /// <typeparam name="TValue">The type of the values to be stored in the cache.</typeparam>
  /// <remarks>
  /// Caches are only meant for performance improvement, they are not reliable data containers. Do not rely on values being present in the cache;
  /// caches might choose to remove individual items (or all their items) at any time. If a reliable store is needed, use 
  /// <see cref="IDictionary{TKey,TValue}"/> or <see cref="IDataStore{TKey,TValue}"/>.
  /// </remarks>
  public interface ICache<TKey, TValue> : INullObject
  {
    TValue GetOrCreateValue (TKey key, Func<TKey,TValue> valueFactory);
    bool TryGetValue (TKey key, out TValue value);
    void Clear();
  }
}

