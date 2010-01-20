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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Implements a collection of <see cref="IClientTransactionListener"/> objects.
  /// </summary>
  [Serializable]
  public class CompoundClientTransactionListener : IClientTransactionListener
  {
    private readonly List<IClientTransactionListener> _listeners = new List<IClientTransactionListener> ();

    public void AddListener (IClientTransactionListener listener)
    {
      ArgumentUtility.CheckNotNull ("listener", listener);

      _listeners.Add (listener);
    }

    public void SubTransactionCreating ()
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.SubTransactionCreating ();
    }

    public void SubTransactionCreated (ClientTransaction subTransaction)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.SubTransactionCreated (subTransaction);
    }

    public void NewObjectCreating (Type type, DomainObject instance)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.NewObjectCreating (type, instance);
    }

    public void ObjectLoading (ObjectID id)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.ObjectLoading (id);
    }

    public void ObjectGotID (DomainObject instance, ObjectID id)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.ObjectGotID (instance, id);
    }

    public void ObjectsLoaded (ReadOnlyCollection<DomainObject> domainObjects)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.ObjectsLoaded (domainObjects);
    }

    public void ObjectDeleting (DomainObject domainObject)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.ObjectDeleting (domainObject);
    }

    public void ObjectDeleted (DomainObject domainObject)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.ObjectDeleted (domainObject);
    }

    public void PropertyValueReading (DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.PropertyValueReading (dataContainer, propertyValue, valueAccess);
    }

    public void PropertyValueRead (DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.PropertyValueRead (dataContainer, propertyValue, value, valueAccess);
    }

    public void PropertyValueChanging (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.PropertyValueChanging (dataContainer, propertyValue, oldValue, newValue);
    }

    public void PropertyValueChanged (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.PropertyValueChanged (dataContainer, propertyValue, oldValue, newValue);
    }

    public void RelationReading (DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.RelationReading (domainObject, propertyName, valueAccess);
    }

    public void RelationRead (DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.RelationRead (domainObject, propertyName, relatedObject, valueAccess);
    }

    public void RelationRead (DomainObject domainObject, string propertyName, ReadOnlyCollection<DomainObject> relatedObjects, ValueAccess valueAccess)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.RelationRead (domainObject, propertyName, relatedObjects, valueAccess);
    }

    public void RelationChanging (DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.RelationChanging (domainObject, propertyName, oldRelatedObject, newRelatedObject);
    }

    public void RelationChanged (DomainObject domainObject, string propertyName)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.RelationChanged (domainObject, propertyName);
    }

    public QueryResult<T> FilterQueryResult<T> (QueryResult<T> queryResult) where T: DomainObject
    {
      foreach (IClientTransactionListener listener in _listeners)
        queryResult = listener.FilterQueryResult (queryResult);
      return queryResult;
    }

    public void TransactionCommitting (ReadOnlyCollection<DomainObject> domainObjects)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.TransactionCommitting (domainObjects);
    }

    public void TransactionCommitted (ReadOnlyCollection<DomainObject> domainObjects)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.TransactionCommitted (domainObjects);
    }

    public void TransactionRollingBack (ReadOnlyCollection<DomainObject> domainObjects)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.TransactionRollingBack (domainObjects);
    }

    public void TransactionRolledBack (ReadOnlyCollection<DomainObject> domainObjects)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.TransactionRolledBack (domainObjects);
    }

    public void RelationEndPointMapRegistering (RelationEndPoint endPoint)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.RelationEndPointMapRegistering (endPoint);
    }

    public void RelationEndPointMapUnregistering (RelationEndPointID endPointID)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.RelationEndPointMapUnregistering (endPointID);
    }

    public void RelationEndPointMapPerformingDelete (RelationEndPointID[] endPointIDs)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.RelationEndPointMapPerformingDelete (endPointIDs);
    }

    public void RelationEndPointUnloading (RelationEndPoint endPoint)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.RelationEndPointUnloading (endPoint);
    }

    public void DataManagerMarkingObjectDiscarded (ObjectID id)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.DataManagerMarkingObjectDiscarded (id);
    }

    public void DataContainerMapRegistering (DataContainer container)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.DataContainerMapRegistering (container);
    }

    public void DataContainerMapUnregistering (DataContainer container)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.DataContainerMapUnregistering (container);
    }
  }
}
