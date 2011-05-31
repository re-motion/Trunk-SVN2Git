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
using Remotion.Utilities;

namespace Remotion.Collections
{
  /// <summary>
  /// A simple thread-safe cache.
  /// </summary>
  [Serializable]
  public class LockingCacheDecorator<TKey, TValue> : ICache<TKey, TValue>
  {
    private readonly ICache<TKey, TValue> _innerCache;
    private readonly object _lock = new object ();

    public LockingCacheDecorator (ICache<TKey, TValue> innerCache)
    {
      ArgumentUtility.CheckNotNull ("innerCache", innerCache);

      _innerCache = innerCache;
    }

    // Add is not safe for interlocked caches

    public TValue GetOrCreateValue (TKey key, Func<TKey, TValue> valueFactory)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("valueFactory", valueFactory);

      lock (_lock)
        return _innerCache.GetOrCreateValue (key, valueFactory);
    }

    public bool TryGetValue (TKey key, out TValue value)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      lock (_lock)
        return _innerCache.TryGetValue (key, out value);
    }

    public void Clear ()
    {
      lock (_lock)
        _innerCache.Clear ();
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
