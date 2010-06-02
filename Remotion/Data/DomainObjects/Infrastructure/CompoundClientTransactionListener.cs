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
using System.Linq;
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
    private readonly List<IClientTransactionListener> _listeners = new List<IClientTransactionListener>();

    public IEnumerable<IClientTransactionListener> Listeners
    {
      get { return _listeners; }
    }

    public void AddListener (IClientTransactionListener listener)
    {
      ArgumentUtility.CheckNotNull ("listener", listener);

      _listeners.Add (listener);
    }

    public void TransactionInitializing (ClientTransaction clientTransaction)
    {
      foreach (var listener in _listeners)
        listener.TransactionInitializing (clientTransaction);
    }

    public void TransactionDiscarding (ClientTransaction clientTransaction)
    {
      foreach (var listener in _listeners)
        listener.TransactionDiscarding (clientTransaction);
    }

    public void SubTransactionCreating (ClientTransaction clientTransaction)
    {
      foreach (var listener in _listeners)
        listener.SubTransactionCreating(clientTransaction);
    }

    public void SubTransactionCreated (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      foreach (var listener in _listeners)
        listener.SubTransactionCreated (clientTransaction, subTransaction);
    }

    public void NewObjectCreating (ClientTransaction clientTransaction, Type type, DomainObject instance)
    {
      foreach (var listener in _listeners)
        listener.NewObjectCreating (clientTransaction, type, instance);
    }

    public void ObjectsLoading (ClientTransaction clientTransaction, ReadOnlyCollection<ObjectID> objectIDs)
    {
      foreach (var listener in _listeners)
        listener.ObjectsLoading (clientTransaction, objectIDs);
    }

    public void ObjectsUnloaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      foreach (var listener in _listeners)
        listener.ObjectsUnloaded (clientTransaction, unloadedDomainObjects);
    }

    public void ObjectsLoaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      foreach (var listener in _listeners)
        listener.ObjectsLoaded (clientTransaction, domainObjects);
    }

    public void ObjectsUnloading (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      foreach (var listener in _listeners)
        listener.ObjectsUnloading (clientTransaction, unloadedDomainObjects);
    }

    public void ObjectDeleting (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      foreach (var listener in _listeners)
        listener.ObjectDeleting (clientTransaction, domainObject);
    }

    public void ObjectDeleted (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      foreach (var listener in _listeners)
        listener.ObjectDeleted (clientTransaction, domainObject);
    }

    public void PropertyValueReading (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
      foreach (var listener in _listeners)
        listener.PropertyValueReading (clientTransaction, dataContainer, propertyValue, valueAccess);
    }

    public void PropertyValueRead (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
      foreach (var listener in _listeners)
        listener.PropertyValueRead (clientTransaction, dataContainer, propertyValue, value, valueAccess);
    }

    public void PropertyValueChanging (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      foreach (var listener in _listeners)
        listener.PropertyValueChanging (clientTransaction, dataContainer, propertyValue, oldValue, newValue);
    }

    public void PropertyValueChanged (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      foreach (var listener in _listeners)
        listener.PropertyValueChanged (clientTransaction, dataContainer, propertyValue, oldValue, newValue);
    }

    public void RelationReading (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
      foreach (var listener in _listeners)
        listener.RelationReading (clientTransaction, domainObject, propertyName, valueAccess);
    }

    public void RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess)
    {
      foreach (var listener in _listeners)
        listener.RelationRead (clientTransaction, domainObject, propertyName, relatedObject, valueAccess);
    }

    public void RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, ReadOnlyDomainObjectCollectionAdapter<DomainObject> relatedObjects, ValueAccess valueAccess)
    {
      foreach (var listener in _listeners)
        listener.RelationRead (clientTransaction, domainObject, propertyName, relatedObjects, valueAccess);
    }

    public void RelationChanging (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      foreach (var listener in _listeners)
        listener.RelationChanging (clientTransaction, domainObject, propertyName, oldRelatedObject, newRelatedObject);
    }

    public void RelationChanged (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName)
    {
      foreach (var listener in _listeners)
        listener.RelationChanged (clientTransaction, domainObject, propertyName);
    }

    public QueryResult<T> FilterQueryResult<T> (ClientTransaction clientTransaction, QueryResult<T> queryResult) where T: DomainObject
    {
      return _listeners.Aggregate (queryResult, (current, listener) => listener.FilterQueryResult (clientTransaction, current));
    }

    public void TransactionCommitting (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      foreach (var listener in _listeners)
        listener.TransactionCommitting (clientTransaction, domainObjects);
    }

    public void TransactionCommitted (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      foreach (var listener in _listeners)
        listener.TransactionCommitted (clientTransaction, domainObjects);
    }

    public void TransactionRollingBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      foreach (var listener in _listeners)
        listener.TransactionRollingBack (clientTransaction, domainObjects);
    }

    public void TransactionRolledBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      foreach (var listener in _listeners)
        listener.TransactionRolledBack (clientTransaction, domainObjects);
    }

    public void RelationEndPointMapRegistering (ClientTransaction clientTransaction, RelationEndPoint endPoint)
    {
      foreach (var listener in _listeners)
        listener.RelationEndPointMapRegistering (clientTransaction, endPoint);
    }

    public void RelationEndPointMapUnregistering (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      foreach (var listener in _listeners)
        listener.RelationEndPointMapUnregistering (clientTransaction, endPointID);
    }

    public void RelationEndPointUnloading (ClientTransaction clientTransaction, RelationEndPoint endPoint)
    {
      foreach (var listener in _listeners)
        listener.RelationEndPointUnloading (clientTransaction, endPoint);
    }

    public void DataManagerMarkingObjectInvalid (ClientTransaction clientTransaction, ObjectID id)
    {
      foreach (var listener in _listeners)
        listener.DataManagerMarkingObjectInvalid (clientTransaction, id);
    }

    public void DataContainerMapRegistering (ClientTransaction clientTransaction, DataContainer container)
    {
      foreach (var listener in _listeners)
        listener.DataContainerMapRegistering (clientTransaction, container);
    }

    public void DataContainerMapUnregistering (ClientTransaction clientTransaction, DataContainer container)
    {
      foreach (var listener in _listeners)
        listener.DataContainerMapUnregistering (clientTransaction, container);
    }

    public void DataContainerStateUpdated (ClientTransaction clientTransaction, DataContainer container, StateType newDataContainerState)
    {
      foreach (var listener in _listeners)
        listener.DataContainerStateUpdated (clientTransaction, container, newDataContainerState);
    }

    public void VirtualRelationEndPointStateUpdated (ClientTransaction clientTransaction, RelationEndPointID endPointID, bool? newEndPointChangeState)
    {
      foreach (var listener in _listeners)
        listener.VirtualRelationEndPointStateUpdated (clientTransaction, endPointID, newEndPointChangeState);
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
