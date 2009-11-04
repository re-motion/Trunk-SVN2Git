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
using System.Collections.ObjectModel;

namespace Remotion.Collections
{
  /// <summary>
  /// Read-only wrapper around a <see cref="Dictionary{TKey,TValue}"/> which explicitely implements <see cref="IDictionary{TKey,TValue}"/>.
  /// </summary>
  /// <remarks>
  /// Behaves analogue to <see cref="ReadOnlyCollection{T}"/>, i.e. not supported methods required by <see cref="IDictionary{TKey,TValue}"/> 
  /// throw <see cref="NotSupportedException"/>|s.
  /// <para/>
  /// See also <see cref="ReadOnlyDictionary{TKey,TValue}"/>.
  /// </remarks>
  [Serializable]
  public class ReadOnlyDictionarySpecific<TKey, TValue> : IDictionary<TKey, TValue>
  {
    private readonly Dictionary<TKey, TValue> _dictionary;

    public ReadOnlyDictionarySpecific (Dictionary<TKey, TValue> dictionary)
    {
      _dictionary = dictionary;
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator ()
    {
      return ((IEnumerable<KeyValuePair<TKey, TValue>>) _dictionary).GetEnumerator();
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

    public bool TryGetValue (TKey key, out TValue value)
    {
      return _dictionary.TryGetValue (key, out value);
    }


    public IEqualityComparer<TKey> Comparer
    {
      get { return _dictionary.Comparer; }
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
      return ((ICollection<KeyValuePair<TKey, TValue>>) _dictionary).Contains (item);
    }

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo (KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      ((ICollection<KeyValuePair<TKey, TValue>>) _dictionary).CopyTo (array, arrayIndex);
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

    // Note that System.Collections.Generic.Dictionary Keys is guranteed to be read-only, whereas IDictionary Keys makes no such gurantee.
    ICollection<TKey> IDictionary<TKey, TValue>.Keys
    {
      get { return _dictionary.Keys; }
    }

    // Note that System.Collections.Generic.Dictionary Values is guranteed to be read-only, whereas IDictionary Values makes no such gurantee.
    ICollection<TValue> IDictionary<TKey, TValue>.Values
    {
      get { return _dictionary.Values; }
    }
  }
}
