// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

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
      _unsafeData = new UnsafeDomainObjectCollectionData ();
      _data = new ArgumentCheckingCollectionDataDecorator (_unsafeData);
    }

    public DomainObjectCollectionData (IEnumerable<DomainObject> domainObjects) : this()
    {
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

      foreach (var domainObject in domainObjects)
      {
        Insert (Count, domainObject);
      }
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
      return GetEnumerator ();
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
      return _data.ContainsObjectID(objectID);
    }

    public DomainObject GetObject (int index)
    {
      return _data.GetObject(index);
    }

    public DomainObject GetObject (ObjectID objectID)
    {
      return _data.GetObject(objectID);
    }

    public int IndexOf (ObjectID objectID)
    {
      return _data.IndexOf(objectID);
    }

    public void Clear ()
    {
      _data.Clear();
    }

    public void Insert (int index, DomainObject domainObject)
    {
      _data.Insert(index, domainObject);
    }

    public void Remove (ObjectID objectID)
    {
      _data.Remove(objectID);
    }

    public void Replace (ObjectID oldDomainObjectID, DomainObject newDomainObject)
    {
      _data.Replace(oldDomainObjectID, newDomainObject);
    }
  }
}