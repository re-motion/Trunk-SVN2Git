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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.Collections
{
  public class MultiDictionary<TKey, TValue> : IDictionary<TKey, IList<TValue>>
  {
    private readonly Dictionary<TKey, IList<TValue>> _innerDictionary = new Dictionary<TKey, IList<TValue>>();


    public IEnumerator<KeyValuePair<TKey, IList<TValue>>> GetEnumerator ()
    {
      return _innerDictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public void Add (KeyValuePair<TKey, IList<TValue>> item)
    {
      ArgumentUtility.CheckNotNull ("item", item);
      _innerDictionary.Add (item.Key, item.Value);
    }

    public void Clear ()
    {
      _innerDictionary.Clear();
    }

    public bool Contains (KeyValuePair<TKey, IList<TValue>> item)
    {
      ArgumentUtility.CheckNotNull ("item", item);
      return _innerDictionary.Contains (item);
    }

    public void CopyTo (KeyValuePair<TKey, IList<TValue>>[] array, int arrayIndex)
    {
      throw new NotImplementedException();
    }

    public bool Remove (KeyValuePair<TKey, IList<TValue>> item)
    {
      ArgumentUtility.CheckNotNull ("item", item);
      return _innerDictionary.Remove (item.Key);
    }
    
    int ICollection<KeyValuePair<TKey, IList<TValue>>>.Count
    {
      get { return KeyCount; }
    }

    public bool IsReadOnly
    {
      get { return true; }
    }

    public bool ContainsKey (TKey key)
    {
      return _innerDictionary.ContainsKey (key);
    }

    public void Add (TKey key, IList<TValue> value)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("value", value);
      _innerDictionary.Add (key, value);
    }

    public bool Remove (TKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      return _innerDictionary.Remove (key);
    }

    public bool TryGetValue (TKey key, out IList<TValue> value)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      return _innerDictionary.TryGetValue (key, out value);
    }

    public IList<TValue> this [TKey key]
    {
      get
      {
        IList<TValue> valueList;
        if (!_innerDictionary.TryGetValue (key, out valueList))
        {
          valueList = new List<TValue>();
          _innerDictionary.Add (key, valueList);
        }
        return valueList;
      }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        _innerDictionary[key] = value;
      }
    }

    public ICollection<TKey> Keys
    {
      get { throw new NotImplementedException(); }
    }

    public ICollection<IList<TValue>> Values
    {
      get { throw new NotImplementedException(); }
    }

    public int KeyCount
    {
      get { return _innerDictionary.Count; }
    }

    public int CountValues ()
    {
      return this.Sum (kvp => kvp.Value.Count);
    }

  }
}