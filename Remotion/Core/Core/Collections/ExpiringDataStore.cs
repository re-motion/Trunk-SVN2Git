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
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Collections
{
  // TODO 3919: Add ExpiringDataStoreFactory class that has 6 factory methods:
  //   - Create<TKey, TValue, TExpirationInfo> ([IEqualityComparer], IExpirationPolicy)
  //   - CreateWithLocking<TKey, TValue, TExpirationInfo> ([IEqualityComparer], IExpirationPolicy)
  //   - CreateWithLazyLocking<TKey, TValue, TExpirationInfo> where TValue : class ([IEqualityComparer], IExpirationPolicy)

  /// <summary>
  /// The <see cref="ExpiringDataStore{TKey,TValue,TExpirationInfo}"/> stores values that can be expire.
  /// </summary>
  public class ExpiringDataStore<TKey, TValue, TExpirationInfo> : IDataStore<TKey, TValue>
  {
    private readonly SimpleDataStore<TKey, Tuple<TValue, TExpirationInfo>> _innerDataStore;
    private readonly IExpirationPolicy<TValue, TExpirationInfo> _expirationPolicy;

    public ExpiringDataStore (IExpirationPolicy<TValue, TExpirationInfo> expirationPolicy, IEqualityComparer<TKey> equalityComparer)
    {
      ArgumentUtility.CheckNotNull ("expirationPolicy", expirationPolicy);
      ArgumentUtility.CheckNotNull ("equalityComparer", equalityComparer);

      _innerDataStore = new SimpleDataStore<TKey, Tuple<TValue, TExpirationInfo>> (equalityComparer);
      _expirationPolicy = expirationPolicy;
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }

    public bool ContainsKey (TKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      // TODO 3919: Test single expired item is removed
      // TODO 3919: return TryGetValue(key, out dummy);

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
      _innerDataStore.Clear();
    }

    public TValue this [TKey key]
    {
      // TODO 3919: Test that RemoveExpiredItems() is called
      // TODO 3919: Test single expired item is removed
      // TODO 3919: if (!TryGetValue(key, out value)) throw KeyNotFoundException, else return value
      // TODO 3919: (Should be 5 tests, the same as for ContainsKey.)
      get { return _innerDataStore[key].Item1; }
      // TODO 3919: RemoveExpiredItems(); 
      set
      {
        RemoveExpiredItems();
        _innerDataStore[key] = Tuple.Create(value, _expirationPolicy.GetExpirationInfo(value)); 
      }
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

      RemoveExpiredItems();

      Tuple<TValue, TExpirationInfo> valueResult;
      if (_innerDataStore.TryGetValue (key, out valueResult))
      {
        if (!_expirationPolicy.IsExpired (valueResult.Item1, valueResult.Item2))
        {
          value = valueResult.Item1;
          return true;
        }

        RemoveWithoutScanning (key);
      }

      value = default (TValue);
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
      _innerDataStore.Add (key, Tuple.Create(value, _expirationPolicy.GetExpirationInfo(value)));
    }

    private bool RemoveWithoutScanning (TKey key)
    {
      return _innerDataStore.Remove (key);
    }

    private void RemoveExpiredItems ()
    {
      if (_expirationPolicy.ShouldScanForExpiredItems())
      {
        var expiredKeys = (
            from kvp in _innerDataStore 
            where _expirationPolicy.IsExpired (kvp.Value.Item1, kvp.Value.Item2) 
            select kvp.Key).ToList();
        
        foreach (var key in expiredKeys)
          RemoveWithoutScanning (key);

        _expirationPolicy.ItemsScanned();
      }
    }
  }
}