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
    private readonly List<ObjectID> _orderedObjectIDs = new List<ObjectID> ();
    private readonly Dictionary<ObjectID, DomainObject> _objectsByID = new Dictionary<ObjectID, DomainObject> ();

    public DomainObjectCollectionData ()
    {
    }

    public DomainObjectCollectionData (IEnumerable<DomainObject> domainObjects)
    {
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

      foreach (var domainObject in domainObjects)
      {
        Insert (Count, domainObject);
      }
    }

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
      PerformClear();
      IncrementVersion ();
    }

    public void Insert (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      if (ContainsObjectID (domainObject.ID))
        throw new InvalidOperationException (string.Format ("The collection already contains an object with ID '{0}'.", domainObject.ID));

      PerformInsert(index, domainObject);
      IncrementVersion ();
    }

    public void Remove (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      var index = IndexOf (objectID);
      if (index != -1)
      {
        PerformRemove (index, objectID);
        IncrementVersion();
      }
    }

    public void Replace (ObjectID oldDomainObjectID, DomainObject newDomainObject)
    {
      ArgumentUtility.CheckNotNull ("oldDomainObjectID", oldDomainObjectID);
      ArgumentUtility.CheckNotNull ("newDomainObject", newDomainObject);

      int index = IndexOf (oldDomainObjectID);
      if (index == -1)
        throw new KeyNotFoundException (string.Format ("The collection does not contain a DomainObject with ID '{0}'.", oldDomainObjectID));

      if (ContainsObjectID (newDomainObject.ID) && oldDomainObjectID != newDomainObject.ID)
        throw new InvalidOperationException (string.Format ("The collection already contains an object with ID '{0}'.", newDomainObject.ID));

      if (GetObject (index) != newDomainObject)
      {
        PerformReplace (index, oldDomainObjectID, newDomainObject);
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

    protected virtual void PerformClear ()
    {
      _orderedObjectIDs.Clear ();
      _objectsByID.Clear ();
    }

    protected virtual void PerformInsert (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      _orderedObjectIDs.Insert (index, domainObject.ID);
      _objectsByID.Add (domainObject.ID, domainObject);
    }

    protected virtual void PerformRemove (int index, ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      Assertion.IsTrue (_orderedObjectIDs[index] == objectID, "index and objectID must match");

      _orderedObjectIDs.RemoveAt (index);
      _objectsByID.Remove (objectID);
    }

    protected virtual void PerformReplace (int index, ObjectID oldDomainObjectID, DomainObject newDomainObject)
    {
      ArgumentUtility.CheckNotNull ("oldDomainObjectID", oldDomainObjectID);
      ArgumentUtility.CheckNotNull ("newDomainObject", newDomainObject);

      Assertion.IsTrue (_orderedObjectIDs[index] == oldDomainObjectID, "index and oldDomainObjectID must match");
      Assertion.IsTrue (!_objectsByID.ContainsKey (newDomainObject.ID), "newDomainObject.ID must not be part of the collection");

      _orderedObjectIDs.RemoveAt (index);
      _objectsByID.Remove (oldDomainObjectID);

      _orderedObjectIDs.Insert (index, newDomainObject.ID);
      _objectsByID.Add (newDomainObject.ID, newDomainObject);
    }

    private void IncrementVersion ()
    {
      ++Version;
    }
  }
}