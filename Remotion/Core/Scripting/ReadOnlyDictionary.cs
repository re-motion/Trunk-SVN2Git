// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Remotion.Scripting
{
  public class ReadOnlyDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
  {
    private readonly Dictionary<TKey, TValue> _dictionary;

    public ReadOnlyDictionary (Dictionary<TKey, TValue> dictionary)
    {
      _dictionary = dictionary;
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator ()
    {
      return ((IEnumerable<KeyValuePair<TKey, TValue>>) _dictionary).GetEnumerator ();
    }

    public IEnumerator GetEnumerator ()
    {
      return ((IEnumerable) _dictionary).GetEnumerator();
    }


    public bool ContainsKey (TKey key)
    {
      return _dictionary.ContainsKey (key);
    }

    public bool ContainsValue (TValue value)
    {
      return _dictionary.ContainsValue (value);
    }


    public void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      _dictionary.GetObjectData (info, context);
    }

    public void OnDeserialization (object sender)
    {
      _dictionary.OnDeserialization (sender);
    }

    public bool TryGetValue (TKey key, out TValue value)
    {
      return _dictionary.TryGetValue (key, out value);
    }

    public IEqualityComparer<TKey> Comparer
    {
      get { return _dictionary.Comparer; }
    }

    public int Count
    {
      get { return _dictionary.Count; }
    }


    public TValue this[TKey key]
    {
      get { return _dictionary[key]; }
    }


    public override string ToString ()
    {
      return _dictionary.ToString();
    }
  }
}