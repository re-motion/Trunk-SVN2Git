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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement
{
  /// <summary>
  /// Implements the <see cref="IDomainObjectCollectionData"/>, forwarding all requests to an implementation of 
  /// <see cref="ICollectionChangeDelegate"/> (<see cref="CollectionEndPoint"/>).
  /// </summary>
  public class ChangeDelegateCollectionData : IDomainObjectCollectionData
  {
    private readonly DomainObjectCollectionData _data = new DomainObjectCollectionData ();

    private readonly ICollectionChangeDelegate _changeDelegate;
    private readonly IDomainObjectCollectionEventRaiser _eventRaiser;
    private readonly DomainObjectCollection _parentCollection;

    public ChangeDelegateCollectionData (
        ICollectionChangeDelegate changeDelegate,
        IDomainObjectCollectionEventRaiser eventRaiser,
        DomainObjectCollection parentCollection)
    {
      ArgumentUtility.CheckNotNull ("changeDelegate", changeDelegate);
      ArgumentUtility.CheckNotNull ("eventRaiser", eventRaiser);
      ArgumentUtility.CheckNotNull ("parentCollection", parentCollection);

      _changeDelegate = changeDelegate;
      _eventRaiser = eventRaiser;
      _parentCollection = parentCollection;
    }

    public ICollectionChangeDelegate ChangeDelegate
    {
      get { return _changeDelegate; }
    }

    public int Count
    {
      get { return _data.Count; }
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    public bool ContainsObjectID (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      return _data.ContainsObjectID (objectID);
    }

    public DomainObject GetObject (int index)
    {
      return _data.GetObject (index);
    }

    public DomainObject GetObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      return _data.GetObject (objectID);
    }

    public int IndexOf (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      return _data.IndexOf (objectID);
    }

    public IEnumerator<DomainObject> GetEnumerator ()
    {
      return _data.GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public void Clear ()
    {
      for (int i = Count - 1; i >= 0; --i)
        _changeDelegate.PerformRemove (_parentCollection, GetObject (i));
    }

    public void Insert (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      _changeDelegate.PerformInsert (_parentCollection, domainObject, index);
    }

    public void Remove (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      if (ContainsObjectID (objectID))
        _changeDelegate.PerformRemove (_parentCollection, GetObject (objectID));
    }

    public void Replace (ObjectID oldDomainObjectID, DomainObject newDomainObject)
    {
      ArgumentUtility.CheckNotNull ("oldDomainObjectID", oldDomainObjectID);
      ArgumentUtility.CheckNotNull ("newDomainObject", newDomainObject);

      _changeDelegate.PerformReplace (_parentCollection, newDomainObject, IndexOf (oldDomainObjectID));
    }

    public void RaiseBeginAddEvent (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      _eventRaiser.BeginAdd (index, domainObject);
    }

    public void RaiseEndAddEvent (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      _eventRaiser.EndAdd (index, domainObject);
    }

    public void RaiseBeginRemoveEvent (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      _eventRaiser.BeginRemove (index, domainObject);
    }

    public void RaiseEndRemoveEvent (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      _eventRaiser.EndRemove(index, domainObject);
    }

    public void InsertData (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      _data.Insert (index, domainObject);
    }

    public void RemoveData (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      _data.Remove (objectID);
    }

    public void ReplaceData (ObjectID oldDomainObjectID, DomainObject newDomainObject)
    {
      ArgumentUtility.CheckNotNull ("oldDomainObjectID", oldDomainObjectID);
      ArgumentUtility.CheckNotNull ("newDomainObject", newDomainObject);

      _data.Replace (oldDomainObjectID, newDomainObject);
    }
  }
}