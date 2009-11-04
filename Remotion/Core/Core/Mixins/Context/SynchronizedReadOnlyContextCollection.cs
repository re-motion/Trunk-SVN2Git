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
using Remotion.Utilities;

namespace Remotion.Mixins.Context
{
  public class SynchronizedReadOnlyContextCollection<TKey, TValue> : ICollection<TValue>, ICollection
  {
    private readonly object _syncObject = new object();
    private readonly ReadOnlyContextCollection<TKey, TValue> _internalCollection;

    public SynchronizedReadOnlyContextCollection (Func<TValue, TKey> keyGenerator, IEnumerable<TValue> values)
    {
      ArgumentUtility.CheckNotNull ("keyGenerator", keyGenerator);
      _internalCollection = new ReadOnlyContextCollection<TKey, TValue> (keyGenerator, values);
    }

    public int Count
    {
      get
      {
        lock (_syncObject)
        {
          return _internalCollection.Count;
        }
      }
    }

    public bool ContainsKey (TKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      lock (_syncObject)
      {
        return _internalCollection.ContainsKey (key);
      }
    }

    public bool Contains (TValue value)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      lock (_syncObject)
      {
        return _internalCollection.Contains (value);
      }
    }

    public TValue this[TKey key]
    {
      get
      {
        return _internalCollection[key];
      }
    }

    public IEnumerator<TValue> GetEnumerator ()
    {
      lock (_syncObject)
      {
        return _internalCollection.GetEnumerator();
      }
    }

    public void CopyTo (TValue[] array, int arrayIndex)
    {
      lock (_syncObject)
      {
        _internalCollection.CopyTo (array, arrayIndex);
      }
    }

    void ICollection<TValue>.Add (TValue item)
    {
      lock (_syncObject)
      {
        ((ICollection<TValue>)_internalCollection).Add (item);
      }
    }

    void ICollection<TValue>.Clear ()
    {
      lock (_syncObject)
      {
        ((ICollection<TValue>) _internalCollection).Clear ();
      }
    }

    bool ICollection<TValue>.Remove (TValue item)
    {
      lock (_syncObject)
      {
        return ((ICollection<TValue>) _internalCollection).Remove (item);
      }
    }

    bool ICollection<TValue>.IsReadOnly
    {
      get
      {
        lock (_syncObject)
        {
          return ((ICollection<TValue>) _internalCollection).IsReadOnly;
        }
      }
    }

    void ICollection.CopyTo (Array array, int index)
    {
      ((ICollection) _internalCollection).CopyTo (array, index);
    }

    object ICollection.SyncRoot
    {
      get { return _syncObject; }
    }

    bool ICollection.IsSynchronized
    {
      get { return true; }
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      lock (_syncObject)
      {
        return ((IEnumerable) _internalCollection).GetEnumerator();
      }
    }
  }
}
