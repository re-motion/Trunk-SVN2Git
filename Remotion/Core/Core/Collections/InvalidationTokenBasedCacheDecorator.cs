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
using Remotion.Utilities;

namespace Remotion.Collections
{
  [Serializable]
  public class InvalidationTokenBasedCacheDecorator<TKey, TValue> : ICache<TKey, TValue>
  {
    private readonly ICache<TKey, TValue> _innerCache;
    private readonly CacheInvalidationToken _cacheInvalidationToken;
    private CacheInvalidationToken.Revision _revision;

    public InvalidationTokenBasedCacheDecorator (ICache<TKey, TValue> innerCache, CacheInvalidationToken cacheInvalidationToken)
    {
      ArgumentUtility.CheckNotNull ("innerCache", innerCache);
      ArgumentUtility.CheckNotNull ("cacheInvalidationToken", cacheInvalidationToken);

      _innerCache = innerCache;
      _cacheInvalidationToken = cacheInvalidationToken;
      _revision = _cacheInvalidationToken.GetCurrent();
    }

    public CacheInvalidationToken CacheInvalidationToken
    {
      get { return _cacheInvalidationToken; }
    }

    public TValue GetOrCreateValue (TKey key, Func<TKey, TValue> valueFactory)
    {
      ArgumentUtility.DebugCheckNotNull ("key", key);
      ArgumentUtility.DebugCheckNotNull ("valueFactory", valueFactory);

      CheckRevision();
      return _innerCache.GetOrCreateValue (key, valueFactory);
    }

    public bool TryGetValue (TKey key, out TValue value)
    {
      ArgumentUtility.DebugCheckNotNull ("key", key);

      CheckRevision();
      return _innerCache.TryGetValue (key, out value);
    }

    void ICache<TKey, TValue>.Clear ()
    {
      _innerCache.Clear();
      _revision = _cacheInvalidationToken.GetCurrent();
    }

    bool INullObject.IsNull
    {
      get { return _innerCache.IsNull; }
    }

    private void CheckRevision ()
    {
      if (!_cacheInvalidationToken.IsCurrent (_revision))
      {
        _innerCache.Clear();
        _revision = _cacheInvalidationToken.GetCurrent();
      }
    }
  }
}