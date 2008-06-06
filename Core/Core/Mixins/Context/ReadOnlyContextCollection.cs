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
using System.Collections;
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Mixins.Context
{
  public class ReadOnlyContextCollection<TKey, TValue> : ICollection<TValue>, ICollection
  {
    private readonly Func<TValue, TKey> _keyGenerator;
    private readonly IDictionary<TKey, TValue> _internalCollection;

    public ReadOnlyContextCollection (Func<TValue, TKey> keyGenerator, IEnumerable<TValue> values)
    {
      ArgumentUtility.CheckNotNull ("keyGenerator", keyGenerator);
      _internalCollection = new Dictionary<TKey, TValue>();
      _keyGenerator = keyGenerator;

      foreach (TValue value in values)
      {
        ArgumentUtility.CheckNotNull ("values[" + _internalCollection.Count + "]", value);
        
        TKey key = _keyGenerator (value);
        TValue existingValue;
        if (_internalCollection.TryGetValue (key, out existingValue))
        {
          if (!value.Equals (existingValue))
          {
            string message = string.Format (
                "The items {0} and {1} are identified by the same key {2} and cannot both be added to the collection.",
                existingValue,
                value,
                key);
            throw new ArgumentException (message, "values");
          }
        }
        else
          _internalCollection.Add (key, value);
      }
    }

    public virtual int Count
    {
      get { return _internalCollection.Count; }
    }

    public virtual bool ContainsKey (TKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      return _internalCollection.ContainsKey (key);
    }

    public virtual bool Contains (TValue value)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      TKey key = _keyGenerator (value);
      TValue foundValue;
      if (!_internalCollection.TryGetValue (key, out foundValue))
        return false;
      else
        return value.Equals (foundValue);
    }

    public virtual TValue this[TKey key]
    {
      get
      {
        TValue value;
        if (!_internalCollection.TryGetValue (key, out value))
          return default (TValue);
        else
          return value;
      }
    }

    public virtual IEnumerator<TValue> GetEnumerator ()
    {
      return _internalCollection.Values.GetEnumerator();
    }

    public virtual void CopyTo (TValue[] array, int arrayIndex)
    {
      _internalCollection.Values.CopyTo (array, arrayIndex);
    }

    void ICollection<TValue>.Add (TValue item)
    {
      throw new NotSupportedException ("This list cannot be changed.");
    }

    void ICollection<TValue>.Clear ()
    {
      throw new NotSupportedException ("This list cannot be changed.");
    }

    bool ICollection<TValue>.Remove (TValue item)
    {
      throw new NotSupportedException ("This list cannot be changed.");
    }

    bool ICollection<TValue>.IsReadOnly
    {
      get { return true; }
    }

    void ICollection.CopyTo (Array array, int index)
    {
      ((ICollection) _internalCollection.Values).CopyTo (array, index);
    }

    object ICollection.SyncRoot
    {
      get { return ((ICollection)_internalCollection).SyncRoot; }
    }

    bool ICollection.IsSynchronized
    {
      get { return false; }
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }
  }
}
