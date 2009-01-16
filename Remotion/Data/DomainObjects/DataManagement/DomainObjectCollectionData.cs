// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Provides an an encapsulation of the data stored inside a <see cref="DomainObjectCollection"/>, implementing the 
  /// <see cref="IDomainObjectCollectionData"/> interface. The data is stored by means of two collections, an ordered <see cref="List{T}"/> of 
  /// <see cref="ObjectID"/>s and a <see cref="Dictionary{TKey,TValue}"/> mapping the IDs to <see cref="DomainObject"/> instances.
  /// </summary>
  public class DomainObjectCollectionData : IDomainObjectCollectionData
  {
    private readonly List<ObjectID> _orderedObjectIDs = new List<ObjectID> ();
    private readonly Dictionary<ObjectID, DomainObject> _objectsByID = new Dictionary<ObjectID, DomainObject> ();

    public int Version { get; private set; }

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
      bool found = _objectsByID.TryGetValue (objectID, out result);
      if (!found)
        throw new KeyNotFoundException (string.Format ("The collection does not contain a DomainObject with ID '{0}'.", objectID));

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

      if (ContainsObjectID (domainObject.ID))
        throw new InvalidOperationException (string.Format ("The collection already contains an object with ID '{0}'.", domainObject.ID));

      _orderedObjectIDs.Insert (index, domainObject.ID);
      _objectsByID.Add (domainObject.ID, domainObject);

      IncrementVersion ();
    }

    public void Remove (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      _orderedObjectIDs.Remove (objectID);
      _objectsByID.Remove (objectID);

      IncrementVersion ();
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

      Remove (oldDomainObjectID);
      Insert (index, newDomainObject);
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