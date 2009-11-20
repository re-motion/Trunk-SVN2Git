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
  /// Decorates <see cref="IDomainObjectCollectionData"/> by raising events whenever the inner collection is modified. The events are raised via
  /// an <see cref="IDomainObjectCollectionEventRaiser"/> instance before and after the modification.
  /// </summary>
  [Serializable]
  public class EventRaisingCollectionDataDecorator : IDomainObjectCollectionData
  {
    private readonly IDomainObjectCollectionEventRaiser _eventRaiser;
    private readonly IDomainObjectCollectionData _wrappedData;

    public EventRaisingCollectionDataDecorator (IDomainObjectCollectionEventRaiser eventRaiser, IDomainObjectCollectionData wrappedData)
    {
      ArgumentUtility.CheckNotNull ("eventRaiser", eventRaiser);
      ArgumentUtility.CheckNotNull ("wrappedData", wrappedData);

      _eventRaiser = eventRaiser;
      _wrappedData = wrappedData;
    }

    public IDomainObjectCollectionEventRaiser EventRaiser
    {
      get { return _eventRaiser; }
    }

    public int Count
    {
      get { return _wrappedData.Count; }
    }

    public Type RequiredItemType
    {
      get { return _wrappedData.RequiredItemType; }
    }

    public ICollectionEndPoint AssociatedEndPoint
    {
      get { return _wrappedData.AssociatedEndPoint; }
    }

    public IDomainObjectCollectionData GetUndecoratedDataStore ()
    {
      return _wrappedData.GetUndecoratedDataStore ();
    }

    public bool ContainsObjectID (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      return _wrappedData.ContainsObjectID(objectID);
    }

    public DomainObject GetObject (int index)
    {
      return _wrappedData.GetObject(index);
    }

    public DomainObject GetObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      return _wrappedData.GetObject(objectID);
    }

    public int IndexOf (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      return _wrappedData.IndexOf(objectID);
    }

    public void Clear ()
    {
      var removedObjects = new Stack<DomainObject> (); // holds the removed objects in order to raise 

      int index = 0;
      foreach (var domainObject in this)
      {
        _eventRaiser.BeginRemove (index, domainObject);
        removedObjects.Push (domainObject);
        ++index;
      }

      Assertion.IsTrue (index == Count);

      _wrappedData.Clear ();

      foreach (var domainObject in removedObjects)
      {
        --index;
        _eventRaiser.EndRemove (index, domainObject);
      }

      Assertion.IsTrue (index == 0);
    }

    public void Insert (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      _eventRaiser.BeginAdd (index, domainObject);
      _wrappedData.Insert (index, domainObject);
      _eventRaiser.EndAdd (index, domainObject);
    }

    public void Remove (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      
      int index = IndexOf (domainObject.ID);
      if (index != -1)
      {
        _eventRaiser.BeginRemove (index, domainObject);
        _wrappedData.Remove (domainObject);
        _eventRaiser.EndRemove (index, domainObject);
      }
    }

    public void Remove (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      int index = IndexOf (objectID);
      if (index != -1)
      {
        var domainObject = GetObject (objectID);

        _eventRaiser.BeginRemove (index, domainObject);
        _wrappedData.Remove (objectID);
        _eventRaiser.EndRemove (index, domainObject);
      }
    }

    public void Replace (int index, DomainObject newDomainObject)
    {
      ArgumentUtility.CheckNotNull ("newDomainObject", newDomainObject);

      var oldDomainObject = GetObject (index);
      if (oldDomainObject != newDomainObject)
      {
        _eventRaiser.BeginRemove (index, oldDomainObject);
        _eventRaiser.BeginAdd (index, newDomainObject);
        _wrappedData.Replace (index, newDomainObject);
        _eventRaiser.EndRemove (index, oldDomainObject);
        _eventRaiser.EndAdd (index, newDomainObject);
      }
    }

    public IEnumerator<DomainObject> GetEnumerator ()
    {
      return _wrappedData.GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator ();
    }
  }
}
