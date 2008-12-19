// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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

