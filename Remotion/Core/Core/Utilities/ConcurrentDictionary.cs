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
using System.Collections.Generic;

namespace Remotion.Utilities
{
  // TODO 5057: When updating to 4.0, replace usages with ConcurrentDictionary from .NET framework.
  public class ConcurrentDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
  {
    private readonly object _lock = new object();

    private readonly Dictionary<TKey, TValue> _dictionary;

    // For testing only.
    public ConcurrentDictionary ()
        : this (EqualityComparer<TKey>.Default)
    {
    }

    public ConcurrentDictionary (IEqualityComparer<TKey> comparer)
    {
      ArgumentUtility.CheckNotNull ("comparer", comparer);

      _dictionary = new Dictionary<TKey, TValue> (comparer);
    }

    public int Count
    {
      get
      {
        lock (_lock)
        {
          return _dictionary.Count;
        }
      }
    }

    public TValue this [TKey key]
    {
      get
      {
        lock (_lock)
        {
          return _dictionary[key];
        }
      }
    }

    public bool ContainsKey (TKey key)
    {
      lock (_lock)
      {
        return _dictionary.ContainsKey (key);
      }
    }

    public void Add (TKey key, TValue value)
    {
      lock (_lock)
      {
        _dictionary.Add (key, value);
      }
    }

    public bool TryGetValue (TKey key, out TValue value)
    {
      lock (_lock)
      {
        return _dictionary.TryGetValue (key, out value);
      }
    }

    public TValue GetOrAdd (TKey key, Func<TKey, TValue> valueProvider)
    {
      lock (_lock)
      {
        TValue result;
        if (!_dictionary.TryGetValue (key, out result))
        {
          result = valueProvider (key);
          _dictionary.Add (key, result);
        }
        return result;
      }
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator ()
    {
      throw new NotImplementedException ();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
    {
      throw new NotImplementedException ();
    }
  }
}