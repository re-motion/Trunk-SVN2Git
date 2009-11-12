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

namespace Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement
{
  /// <summary>
  /// Provides an an encapsulation of the data stored inside a <see cref="DomainObjectCollection"/>, implementing the 
  /// <see cref="IDomainObjectCollectionData"/> interface. The data is stored by means of two collections, an ordered <see cref="List{T}"/> of 
  /// <see cref="ObjectID"/>s and a <see cref="Dictionary{TKey,TValue}"/> mapping the IDs to <see cref="DomainObject"/> instances.
  /// </summary>
  [Serializable]
  public class DomainObjectCollectionData : IDomainObjectCollectionData
  {
    private readonly UnsafeDomainObjectCollectionData _unsafeData;
    private readonly IDomainObjectCollectionData _data;

    public DomainObjectCollectionData ()
    {
      _unsafeData = new UnsafeDomainObjectCollectionData();
      _data = new ArgumentCheckingCollectionDataDecorator (_unsafeData);
    }

    public DomainObjectCollectionData (IEnumerable<DomainObject> domainObjects)
        : this()
    {
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

      foreach (var domainObject in domainObjects)
        Insert (Count, domainObject);
    }

    public long Version
    {
      get { return _unsafeData.Version; }
    }

    public IEnumerator<DomainObject> GetEnumerator ()
    {
      return _data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public int Count
    {
      get { return _data.Count; }
    }

    public bool IsReadOnly
    {
      get { return _data.IsReadOnly; }
    }

    public bool ContainsObjectID (ObjectID objectID)
    {
      return _data.ContainsObjectID (objectID);
    }

    public DomainObject GetObject (int index)
    {
      return _data.GetObject (index);
    }

    public DomainObject GetObject (ObjectID objectID)
    {
      return _data.GetObject (objectID);
    }

    public int IndexOf (ObjectID objectID)
    {
      return _data.IndexOf (objectID);
    }

    public void Clear ()
    {
      _data.Clear();
    }

    public void Insert (int index, DomainObject domainObject)
    {
      _data.Insert (index, domainObject);
    }

    public void Remove (DomainObject domainObject)
    {
      _data.Remove (domainObject);
    }

    public void Replace (int index, DomainObject newDomainObject)
    {
      _data.Replace (index, newDomainObject);
    }
  }
}
