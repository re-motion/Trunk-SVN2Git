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
using Remotion.Utilities;

namespace Remotion.Collections
{
  //TODO: Doc
  [Serializable]
  public class Cache<TKey, TValue> : ICache<TKey, TValue> 
  {
    private readonly SimpleDataStore<TKey, TValue> _dataStore;

    // construction and disposing

    public Cache ()
      : this (null)
    {
    }

    public Cache (IEqualityComparer<TKey> comparer)
    {
      _dataStore = new SimpleDataStore<TKey, TValue> (comparer);
    }

    // methods and properties

    public void Add (TKey key, TValue value)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      _dataStore[key] = value;
    }

    public bool TryGetValue (TKey key, out TValue value)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      
      return _dataStore.TryGetValue (key, out value);
    }

    public void Clear ()
    {
      _dataStore.Clear ();
    }

    public TValue GetOrCreateValue (TKey key, Func<TKey,TValue> valueFactory)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("valueFactory", valueFactory);

      return _dataStore.GetOrCreateValue (key, valueFactory);
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
