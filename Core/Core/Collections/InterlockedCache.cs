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
using Remotion.Utilities;

namespace Remotion.Collections
{
  /// <summary>
  /// A simple thread-safe cache.
  /// </summary>
  [Serializable]
  public class InterlockedCache<TKey, TValue> : ICache<TKey, TValue>
  {
    private Dictionary<TKey, TValue> _cache;

    public InterlockedCache ()
      : this (null)
    {
    }

    public InterlockedCache (IEqualityComparer<TKey> comparer)
    {
      _cache = new Dictionary<TKey,TValue> (comparer);
    }

    // Add is not safe for interlocked caches

    public TValue GetOrCreateValue (TKey key, Func<TKey, TValue> valueFactory)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("valueFactory", valueFactory);

      lock (_cache)
      {
        TValue value;
        if (! _cache.TryGetValue (key, out value))
        {
          value = valueFactory (key);
          _cache.Add (key, value);
        }
        return value;
      }
    }

    public bool TryGetValue (TKey key, out TValue value)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      lock (_cache)
        return _cache.TryGetValue (key, out value);
    }

    public void Clear ()
    {
      lock (_cache)
        _cache.Clear ();
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
