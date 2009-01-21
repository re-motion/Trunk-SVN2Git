// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement
{
  /// <summary>
  /// Implements the <see cref="IDomainObjectCollectionData"/>, storing <see cref="DomainObject"/> data. This class does not do any semantic checks
  /// when its operations are executed. This means that it is possible to get it into inconsistent state. Always combine it with a 
  /// <see cref="ArgumentCheckingCollectionDataDecorator"/> (or use <see cref="DomainObjectCollectionData"/> instead).
  /// </summary>
  [Serializable]
  internal class UnsafeDomainObjectCollectionData : IDomainObjectCollectionData
  {
    private readonly List<ObjectID> _orderedObjectIDs = new List<ObjectID> ();
    private readonly Dictionary<ObjectID, DomainObject> _objectsByID = new Dictionary<ObjectID, DomainObject> ();

    public long Version { get; private set; }

    public int Count
    {
      get { return _orderedObjectIDs.Count; }
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    public bool ContainsObjectID (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      return _objectsByID.ContainsKey (objectID);
    }

    public DomainObject GetObject (int index)
    {
      return _objectsByID[_orderedObjectIDs[index]];
    }

    public DomainObject GetObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      
      DomainObject result;
      _objectsByID.TryGetValue (objectID, out result);
      return result;
    }

    public int IndexOf (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      return _orderedObjectIDs.IndexOf (objectID);
    }

    public void Clear ()
    {
      _orderedObjectIDs.Clear ();
      _objectsByID.Clear ();

      IncrementVersion ();
    }

    public void Insert (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      _orderedObjectIDs.Insert (index, domainObject.ID);
      _objectsByID.Add (domainObject.ID, domainObject);

      IncrementVersion ();
    }

    public void Remove (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      var index = IndexOf (objectID);
      if (index != -1)
      {
        _orderedObjectIDs.RemoveAt (index);
        _objectsByID.Remove (objectID);

        IncrementVersion();
      }
    }

    public void Replace (ObjectID oldDomainObjectID, DomainObject newDomainObject)
    {
      ArgumentUtility.CheckNotNull ("oldDomainObjectID", oldDomainObjectID);
      ArgumentUtility.CheckNotNull ("newDomainObject", newDomainObject);

      int index = IndexOf (oldDomainObjectID);
      if (GetObject (index) != newDomainObject)
      {
        _orderedObjectIDs.RemoveAt (index);
        _objectsByID.Remove (oldDomainObjectID);

        _orderedObjectIDs.Insert (index, newDomainObject.ID);
        _objectsByID.Add (newDomainObject.ID, newDomainObject);

        IncrementVersion ();
      }
    }

    public IEnumerator<DomainObject> GetEnumerator ()
    {
      var enumeratedVersion = Version;
      for (int i = 0; i < Count; i++)
      {
        if (Version != enumeratedVersion)
          throw new InvalidOperationException ("Collection was modified during enumeration.");

        yield return GetObject (i);
      }

      // Need to check again, in case Count was decreased while enumerating
      if (Version != enumeratedVersion)
        throw new InvalidOperationException ("Collection was modified during enumeration.");
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    private void IncrementVersion ()
    {
      ++Version;
    }
  }
}