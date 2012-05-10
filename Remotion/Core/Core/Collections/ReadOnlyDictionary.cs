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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remotion.Collections
{
  /// <summary>
  /// Read-only wrapper around an <see cref="IDictionary{TKey,TValue}"/> which itself explicitely implements <see cref="IDictionary{TKey,TValue}"/>.
  /// </summary>
  /// <remarks>
  /// Behaves analogue to <see cref="ReadOnlyCollection{T}"/>, i.e. not supported methods required by <see cref="IDictionary{TKey,TValue}"/> 
  /// throw <see cref="NotSupportedException"/>|s.
  /// <para/>
  /// </remarks>
  [Serializable]
  public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
  {
    private readonly IDictionary<TKey, TValue> _dictionary;

    public ReadOnlyDictionary (IDictionary<TKey, TValue> dictionary)
    {
      _dictionary = dictionary;
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator ()
    {
      return _dictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return _dictionary.GetEnumerator();
    }


    public bool ContainsKey (TKey key)
    {
      return _dictionary.ContainsKey (key);
    }


    public bool TryGetValue (TKey key, out TValue value)
    {
      return _dictionary.TryGetValue (key, out value);
    }


    void ICollection<KeyValuePair<TKey, TValue>>.Add (KeyValuePair<TKey, TValue> item)
    {
      throw new NotSupportedException ("Dictionary is read-only.");
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Clear ()
    {
      throw new NotSupportedException ("Dictionary is read-only.");
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains (KeyValuePair<TKey, TValue> item)
    {
      return _dictionary.Contains (item);
    }

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo (KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      _dictionary.CopyTo (array, arrayIndex);
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove (KeyValuePair<TKey, TValue> item)
    {
      throw new NotSupportedException ("Dictionary is read-only.");
    }

    public int Count
    {
      get { return _dictionary.Count; }
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
    {
      get { return true; }
    }


    public TValue this [TKey key]
    {
      get { return _dictionary[key]; }
    }

    public ICollection<TKey> Keys
    {
      get { return new ReadOnlyCollectionDecorator<TKey> (_dictionary.Keys); }
    }

    public ICollection<TValue> Values
    {
      get { return new ReadOnlyCollectionDecorator<TValue> (_dictionary.Values); }
    }
    
    public override string ToString ()
    {
      return _dictionary.ToString();
    }


    void IDictionary<TKey, TValue>.Add (TKey key, TValue value)
    {
      throw new NotSupportedException ("Dictionary is read-only.");
    }

    bool IDictionary<TKey, TValue>.Remove (TKey key)
    {
      throw new NotSupportedException ("Dictionary is read-only.");
    }


    TValue IDictionary<TKey, TValue>.this [TKey key]
    {
      get { return _dictionary[key]; }
      set { throw new NotSupportedException ("Dictionary is read-only."); }
    }
  }
}
