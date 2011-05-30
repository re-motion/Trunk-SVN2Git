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
using Remotion.Utilities;

namespace Remotion.Collections
{
  public class LazyLockingDataStoreAdapter<TKey, TValue> : IDataStore<TKey, TValue>
      where TValue: class
  {
    private readonly LockingDataStoreDecorator<TKey, DoubleCheckedLockingContainer<TValue>> _innerDataStore;

    public LazyLockingDataStoreAdapter (IDataStore<TKey, DoubleCheckedLockingContainer<TValue>> innerDataStore)
    {
      ArgumentUtility.CheckNotNull ("innerDataStore", innerDataStore);
      _innerDataStore = new LockingDataStoreDecorator<TKey, DoubleCheckedLockingContainer<TValue>> (innerDataStore);
    }

    public bool IsNull
    {
      get { return false; }
    }

    public bool ContainsKey (TKey key)
    {
      return _innerDataStore.ContainsKey (key);
    }

    public void Add (TKey key, TValue value)
    {
      _innerDataStore.Add (key, new DoubleCheckedLockingContainer<TValue>(() => value));
    }

    public bool Remove (TKey key)
    {
      return _innerDataStore.Remove (key);
    }

    public void Clear ()
    {
      _innerDataStore.Clear();
    }

    public TValue this [TKey key]
    {
      get { return _innerDataStore[key].Value; }
      set { _innerDataStore[key] = new DoubleCheckedLockingContainer<TValue>(()=>value); }
    }

    public TValue GetValueOrDefault (TKey key)
    {
      return _innerDataStore.GetValueOrDefault (key).Value;
    }

    public bool TryGetValue (TKey key, out TValue value)
    {
      DoubleCheckedLockingContainer<TValue> result;

      if (_innerDataStore.TryGetValue (key, out result))
      {
        value = result.Value;
        return true;
      }

      value = null;
      return false;
    }

    public TValue GetOrCreateValue (TKey key, Func<TKey, TValue> creator)
    {
      var doubleCheckedLockingContainer = _innerDataStore.GetOrCreateValue (key, k => new DoubleCheckedLockingContainer<TValue> (() => creator (k)));
      return doubleCheckedLockingContainer.Value;
    }
  }
}