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
using System.Collections.ObjectModel;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
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

    public void TransactionInitializing (ClientTransaction clientTransaction)
    {
      // not handled by this listener
    }

    public void TransactionDiscarding (ClientTransaction clientTransaction)
    {
      // not handled by this listener
    }

    public void SubTransactionCreating (ClientTransaction clientTransaction)
    {
      Extension.SubTransactionCreating (clientTransaction);
    }

    public void SubTransactionCreated (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      Extension.SubTransactionCreated (clientTransaction, subTransaction);
    }

    public void NewObjectCreating (ClientTransaction clientTransaction, Type type, DomainObject instance)
    {
      Extension.NewObjectCreating (clientTransaction, type);
    }

    public void ObjectsLoading (ClientTransaction clientTransaction, ReadOnlyCollection<ObjectID> objectIDs)
    {
      Extension.ObjectsLoading (clientTransaction, objectIDs);
    }

    public void ObjectsUnloaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      Extension.ObjectsUnloaded (clientTransaction, unloadedDomainObjects);
    }

    public void ObjectsLoaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      Extension.ObjectsLoaded (clientTransaction, domainObjects);
    }

    public void ObjectsUnloading (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      Extension.ObjectsUnloading (clientTransaction, unloadedDomainObjects);
    }

    public void ObjectDeleting (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      Extension.ObjectDeleting (clientTransaction, domainObject);
    }

    public void ObjectDeleted (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      Extension.ObjectDeleted (clientTransaction, domainObject);
    }

    public void PropertyValueReading (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
      Extension.PropertyValueReading (clientTransaction, dataContainer, propertyValue, valueAccess);
    }

    public void PropertyValueRead (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
      Extension.PropertyValueRead (clientTransaction, dataContainer, propertyValue, value, valueAccess);
    }

    public void PropertyValueChanging (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      if (!propertyValue.Definition.IsObjectID)
        Extension.PropertyValueChanging (clientTransaction, dataContainer, propertyValue, oldValue, newValue);
    }

    public void PropertyValueChanged (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      if (!propertyValue.Definition.IsObjectID)
        Extension.PropertyValueChanged (clientTransaction, dataContainer, propertyValue, oldValue, newValue);
    }

    public void RelationReading (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, ValueAccess valueAccess)
    {
      Extension.RelationReading (clientTransaction, domainObject, relationEndPointDefinition, valueAccess);
    }

    public void RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, DomainObject relatedObject, ValueAccess valueAccess)
    {
      Extension.RelationRead (clientTransaction, domainObject, relationEndPointDefinition, relatedObject, valueAccess);
    }

    public void RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, ReadOnlyDomainObjectCollectionAdapter<DomainObject> relatedObjects, ValueAccess valueAccess)
    {
      Extension.RelationRead (clientTransaction, domainObject, relationEndPointDefinition, relatedObjects, valueAccess);
    }

    public void RelationChanging (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      Extension.RelationChanging (clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject);
    }

    public void RelationChanged (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition)
    {
      Extension.RelationChanged (clientTransaction, domainObject, relationEndPointDefinition);
    }

    public QueryResult<T> FilterQueryResult<T> (ClientTransaction clientTransaction, QueryResult<T> queryResult) where T: DomainObject
    {
      return Extension.FilterQueryResult (clientTransaction, queryResult);
    }

    public void TransactionCommitting (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      Extension.Committing (clientTransaction, domainObjects);
    }

    public void TransactionCommitted (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      Extension.Committed (clientTransaction, domainObjects);
    }

    public void TransactionRollingBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      Extension.RollingBack (clientTransaction, domainObjects);
    }

    public void TransactionRolledBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      Extension.RolledBack (clientTransaction, domainObjects);
    }

    public void RelationEndPointMapRegistering (ClientTransaction clientTransaction, IRelationEndPoint endPoint)
    {
      // not handled by this listener
    }

    public void RelationEndPointMapUnregistering (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      // not handled by this listener
    }

    public void RelationEndPointUnloading (ClientTransaction clientTransaction, IRelationEndPoint endPoint)
    {
      // not handled by this listener
    }

    public void DataManagerDiscardingObject (ClientTransaction clientTransaction, ObjectID id)
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
