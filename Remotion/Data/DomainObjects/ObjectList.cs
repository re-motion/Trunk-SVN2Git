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
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  [Serializable]
  public class ObjectList<T> : DomainObjectCollection, IList<T> where T : DomainObject
  {
    public ObjectList()
        : base (typeof (T))
    {
    }

    public ObjectList (ObjectList<T> collection, bool isCollectionReadOnly)
      : base (collection, isCollectionReadOnly)
    {
    }

    public ObjectList (IEnumerable<T> collection, bool isCollectionReadOnly)
      : base (collection.Cast<DomainObject>(), isCollectionReadOnly)
    {
    }

    public int IndexOf (T item)
    {
      return base.IndexOf (item);
    }

    public void Insert (int index, T item)
    {
      base.Insert (index, item);
    }

    public new T this [int index]
    {
      get { return (T) base[index]; }
      set { base[index] = value; }
    }

    public new T this [ObjectID id]
    {
      get { return (T) base[id]; }
    }

    public new ObjectList<T> Clone ()
    {
      return (ObjectList<T>) base.Clone();
    }

    public new ObjectList<T> Clone (bool makeCloneReadOnly)
    {
      return (ObjectList<T>) base.Clone (makeCloneReadOnly);
    }

    public void Add (T item)
    {
      base.Add (item);
    }

    public void AddRange (IEnumerable<T> items)
    {
      base.AddRange (items);
    }

    bool ICollection<T>.Contains (T item)
    {
      return base.ContainsObject (item);
    }

    public void CopyTo (T[] array, int arrayIndex)
    {
      base.CopyTo (array, arrayIndex);
    }

    public bool Remove (T item)
    {
      bool result = ContainsObject (item);
      base.Remove (item);
      return result;
    }

    public new IEnumerator<T> GetEnumerator ()
    {
      foreach (T t in (IEnumerable) this)
        yield return t;
    }

    public T[] ToArray()
    {
      return ArrayUtility.Convert<T> (this);
    }
  }
}
