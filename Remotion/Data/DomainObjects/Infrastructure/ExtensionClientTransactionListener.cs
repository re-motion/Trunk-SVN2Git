// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// A <see cref="IClientTransactionListener"/> implementation that notifies <see cref="IClientTransactionExtension"/> instances
  /// about transaction events.
  /// </summary>
  /// <remarks>
  /// The <see cref="ClientTransaction"/> class uses this listener to implement its extension mechanism.
  /// </remarks>
  [Serializable]
  public class ExtensionClientTransactionListener : IClientTransactionListener
  {
    private readonly IClientTransactionExtension _extension;

    public ExtensionClientTransactionListener (IClientTransactionExtension extension)
    {
      _extension = extension;
    }

    public IClientTransactionExtension Extension
    {
      get { return _extension; }
    }

    public void TransactionInitialize (ClientTransaction clientTransaction)
    {
      _extension.TransactionInitialize (clientTransaction);
    }

    public void TransactionDiscard (ClientTransaction clientTransaction)
    {
      _extension.TransactionDiscard (clientTransaction);
    }

    public void SubTransactionCreating (ClientTransaction clientTransaction)
    {
      _extension.SubTransactionCreating (clientTransaction);
    }

    public void SubTransactionInitialize (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      _extension.SubTransactionInitialize (clientTransaction, subTransaction);
    }

    public void SubTransactionCreated (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      _extension.SubTransactionCreated (clientTransaction, subTransaction);
    }

    public void NewObjectCreating (ClientTransaction clientTransaction, Type type)
    {
      _extension.NewObjectCreating (clientTransaction, type);
    }

    public void ObjectsLoading (ClientTransaction clientTransaction, ReadOnlyCollection<ObjectID> objectIDs)
    {
      _extension.ObjectsLoading (clientTransaction, objectIDs);
    }

    public void ObjectsUnloaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      _extension.ObjectsUnloaded (clientTransaction, unloadedDomainObjects);
    }

    public void ObjectsLoaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      _extension.ObjectsLoaded (clientTransaction, domainObjects);
    }

    public void ObjectsUnloading (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      _extension.ObjectsUnloading (clientTransaction, unloadedDomainObjects);
    }

    public void ObjectDeleting (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      _extension.ObjectDeleting (clientTransaction, domainObject);
    }

    public void ObjectDeleted (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      _extension.ObjectDeleted (clientTransaction, domainObject);
    }

    public void PropertyValueReading (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyDefinition propertyDefinition, ValueAccess valueAccess)
    {
      _extension.PropertyValueReading (clientTransaction, dataContainer, propertyDefinition, valueAccess);
    }

    public void PropertyValueRead (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyDefinition propertyDefinition, object value, ValueAccess valueAccess)
    {
      _extension.PropertyValueRead (clientTransaction, dataContainer, propertyDefinition, value, valueAccess);
    }

    public void PropertyValueChanging (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyDefinition propertyDefinition, object oldValue, object newValue)
    {
      if (!propertyDefinition.IsObjectID)
        _extension.PropertyValueChanging (clientTransaction, dataContainer, propertyDefinition, oldValue, newValue);
    }

    public void PropertyValueChanged (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyDefinition propertyDefinition, object oldValue, object newValue)
    {
      if (!propertyDefinition.IsObjectID)
        _extension.PropertyValueChanged (clientTransaction, dataContainer, propertyDefinition, oldValue, newValue);
    }

    public void RelationReading (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, ValueAccess valueAccess)
    {
      _extension.RelationReading (clientTransaction, domainObject, relationEndPointDefinition, valueAccess);
    }

    public void RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, DomainObject relatedObject, ValueAccess valueAccess)
    {
      _extension.RelationRead (clientTransaction, domainObject, relationEndPointDefinition, relatedObject, valueAccess);
    }

    public void RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, ReadOnlyDomainObjectCollectionAdapter<DomainObject> relatedObjects, ValueAccess valueAccess)
    {
      _extension.RelationRead (clientTransaction, domainObject, relationEndPointDefinition, relatedObjects, valueAccess);
    }

    public void RelationChanging (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      _extension.RelationChanging (clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject);
    }

    public void RelationChanged (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      _extension.RelationChanged (clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject);
    }

    public QueryResult<T> FilterQueryResult<T> (ClientTransaction clientTransaction, QueryResult<T> queryResult) where T: DomainObject
    {
      return _extension.FilterQueryResult (clientTransaction, queryResult);
    }

    public IEnumerable<T> FilterCustomQueryResult<T> (ClientTransaction clientTransaction, IQuery query, IEnumerable<T> results)
    {
      return _extension.FilterCustomQueryResult (clientTransaction, query, results);
    }

    public void TransactionCommitting (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      _extension.Committing (clientTransaction, domainObjects);
    }

    public void TransactionCommitValidate (ClientTransaction clientTransaction, ReadOnlyCollection<PersistableData> committedData)
    {
      _extension.CommitValidate (clientTransaction, committedData);
    }

    public void TransactionCommitted (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      _extension.Committed (clientTransaction, domainObjects);
    }

    public void TransactionRollingBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      _extension.RollingBack (clientTransaction, domainObjects);
    }

    public void TransactionRolledBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      _extension.RolledBack (clientTransaction, domainObjects);
    }

    public void RelationEndPointMapRegistering (ClientTransaction clientTransaction, IRelationEndPoint endPoint)
    {
      // not handled by this listener
    }

    public void RelationEndPointMapUnregistering (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      // not handled by this listener
    }

    public void RelationEndPointUnloading (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      // not handled by this listener
    }

    public void ObjectMarkedInvalid (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      // not handled by this listener
    }

    public void ObjectMarkedNotInvalid (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      // not handled by this listener
    }

    public void DataContainerMapRegistering (ClientTransaction clientTransaction, DataContainer container)
    {
      // not handled by this listener
    }

    public void DataContainerMapUnregistering (ClientTransaction clientTransaction, DataContainer container)
    {
      // not handled by this listener
    }

    public void DataContainerStateUpdated (ClientTransaction clientTransaction, DataContainer container, StateType newDataContainerState)
    {
      // not handled by this listener
    }

    public void VirtualRelationEndPointStateUpdated (ClientTransaction clientTransaction, RelationEndPointID endPointID, bool? newEndPointChangeState)
    {
      // not handled by this listener
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
