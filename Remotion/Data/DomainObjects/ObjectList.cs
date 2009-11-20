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
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  [Serializable]
  public class ObjectList<T> : DomainObjectCollection, IList<T> where T : DomainObject
  {
    private static IDomainObjectCollectionData CheckStrategy (IDomainObjectCollectionData dataStrategy)
    {
      ArgumentUtility.CheckNotNull ("dataStrategy", dataStrategy);

      if (!typeof (T).IsAssignableFrom (dataStrategy.RequiredItemType))
      {
        var message = string.Format (
            "The given data strategy must have a required item type of '{0}' in order to be used with this collection type.",
            typeof (T));
        throw new ArgumentException (message, "dataStrategy");
      }
      return dataStrategy;
    }

    public ObjectList()
        : base (typeof (T))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectList{T}"/> class with a given <see cref="IDomainObjectCollectionData"/>
    /// data storage strategy. The <see cref="IDomainObjectCollectionData"/>'s <see cref="IDomainObjectCollectionData.RequiredItemType"/>
    /// must be set to <typeparamref name="T"/>.
    /// </summary>
    /// <param name="dataStrategy">The <see cref="IDomainObjectCollectionData"/> instance to use as the data storage strategy.</param>
    /// <remarks>
    /// <para>
    /// Derived classes must support this constructor.
    /// </para>
    /// <para>
    /// The given <paramref name="dataStrategy"/> is directly used, so it should perform any argument checks and event raising on its own.
    /// </para>
    /// </remarks>
    public ObjectList (IDomainObjectCollectionData dataStrategy)
        : base (CheckStrategy (dataStrategy))
    {
    }

    public ObjectList (ObjectList<T> collection, bool isCollectionReadOnly)
      : base (collection, isCollectionReadOnly)
    {
    }

    public ObjectList (IEnumerable<T> collection, bool isCollectionReadOnly)
      : base (collection.Cast<DomainObject>(), typeof (T), isCollectionReadOnly)
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
      return ContainsObject (item);
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
      return ArrayUtility.Convert (this);
    }
  }
}
