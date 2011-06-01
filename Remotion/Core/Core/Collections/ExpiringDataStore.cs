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
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Collections
{
  /// <summary>
  /// The <see cref="ExpiringDataStore{TKey,TValue}"/> stores values that can be expire.
  /// </summary>
  public class ExpiringDataStore<TKey, TValue> : IDataStore<TKey, TValue>
  {
    private readonly SimpleDataStore<TKey, TValue> _innerDataStore;
    private readonly IExpirationPolicy<TValue> _expirationPolicy;

    public ExpiringDataStore (IExpirationPolicy<TValue> expirationPolicy, IEqualityComparer<TKey> equalityComparer)
    {
      ArgumentUtility.CheckNotNull ("expirationPolicy", expirationPolicy);
      ArgumentUtility.CheckNotNull ("equalityComparer", equalityComparer);

      _innerDataStore = new SimpleDataStore<TKey, TValue> (equalityComparer);
      _expirationPolicy = expirationPolicy;
    }

    public bool IsNull
    {
      get { return false; }
    }

    public bool ContainsKey (TKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      return _innerDataStore.ContainsKey (key);
    }

    public void Add (TKey key, TValue value)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("value", value);

      RemoveExpiredItems();
      AddWithoutScanning (key, value);
    }

    public bool Remove (TKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      RemoveExpiredItems();
      return RemoveWithoutScanning (key);
    }

    public void Clear ()
    {
      foreach (var keyValuePair in _innerDataStore)
        _expirationPolicy.ItemRemoved (keyValuePair.Value);
      _innerDataStore.Clear();
    }

    public TValue this [TKey key]
    {
      get { return _innerDataStore[key]; }
      set { _innerDataStore[key] = value; }
    }

    public TValue GetValueOrDefault (TKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      TValue value;
      TryGetValue (key, out value);
      return value;
    }

    public bool TryGetValue (TKey key, out TValue value)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      if (_innerDataStore.TryGetValue (key, out value))
      {
        if (_expirationPolicy.IsExpired (value))
        {
          RemoveWithoutScanning (key);
          value = default(TValue);
          return false;
        }
        return true;
      }
      return false;
    }

    public TValue GetOrCreateValue (TKey key, Func<TKey, TValue> creator)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("creator", creator);

      TValue value;
      if (!TryGetValue (key, out value))
      {
        value = creator (key);
        AddWithoutScanning (key, value);
      }

      return value;
    }

    private void AddWithoutScanning (TKey key, TValue value)
    {
      _innerDataStore.Add (key, value);
      _expirationPolicy.ItemAdded (value);
    }

    private bool RemoveWithoutScanning (TKey key)
    {
      if (_innerDataStore.ContainsKey (key))
      {
        var value = this[key];
        _innerDataStore.Remove (key);
        _expirationPolicy.ItemRemoved (value);
        return true;
      }
      return false;
    }

    private void RemoveExpiredItems ()
    {
      if (_expirationPolicy.ShouldScanForExpiredItems())
      {
        var expiredKeys = new List<TKey>();

        foreach (var kvp in _innerDataStore)
        {
          if (_expirationPolicy.IsExpired (kvp.Value))
            expiredKeys.Add (kvp.Key);
        }

        foreach (var key in expiredKeys)
          RemoveWithoutScanning (key);

        _expirationPolicy.ItemsScanned();
      }
    }
  }
}