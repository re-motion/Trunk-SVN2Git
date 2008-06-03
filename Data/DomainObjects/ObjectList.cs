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
using Remotion.Data.DomainObjects.Infrastructure;

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
  }
}
