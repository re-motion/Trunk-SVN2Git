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
