// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// This class acts as a read-only adapter for another <see cref="IDomainObjectCollectionData"/> object. Every modifying method of the
  /// <see cref="IDomainObjectTransactionContext"/> will throw an <see cref="InvalidOperationException"/> when invoked on this class.
  /// </summary>
  public class ReadOnlyDomainObjectCollectionData : IDomainObjectCollectionData
  {
    private readonly IDomainObjectCollectionData _wrappedData;

    public ReadOnlyDomainObjectCollectionData (IDomainObjectCollectionData wrappedData)
    {
      _wrappedData = wrappedData;
    }

    public IEnumerator<DomainObject> GetEnumerator ()
    {
      return _wrappedData.GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public int Count
    {
      get { return _wrappedData.Count; }
    }

    public bool IsReadOnly
    {
      get { return true; }
    }

    public bool ContainsObjectID (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      return _wrappedData.ContainsObjectID (objectID);
    }

    public DomainObject GetObject (int index)
    {
      return _wrappedData.GetObject (index);
    }

    public DomainObject GetObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      return _wrappedData.GetObject (objectID);
    }

    public int IndexOf (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      return _wrappedData.IndexOf (objectID);
    }

    public void Clear ()
    {
      throw new InvalidOperationException ("Cannot clear a read-only collection.");
    }

    public void Insert (int index, DomainObject domainObject)
    {
      throw new InvalidOperationException ("Cannot insert an item into a read-only collection.");
    }

    public void Remove (ObjectID objectID)
    {
      throw new InvalidOperationException ("Cannot remove an item from a read-only collection.");
    }

    public void Replace (ObjectID oldDomainObjectID, DomainObject newDomainObject)
    {
      throw new InvalidOperationException ("Cannot replace an item in a read-only collection.");
    }
  }
}