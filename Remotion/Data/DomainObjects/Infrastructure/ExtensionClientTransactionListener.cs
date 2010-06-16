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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// A <see cref="IClientTransactionListener"/> implementation that notifies <see cref="IClientTransactionExtension">IClientTransactionExtensions</see>
  /// about transaction events.
  /// </summary>
  /// <remarks>
  /// The <see cref="ClientTransaction"/> class uses this listener to implement its extension mechanism.
  /// </remarks>
  [Serializable]
  public class ExtensionClientTransactionListener : IClientTransactionListener
  {
    private readonly ClientTransactionExtensionCollection _extensions;

    public ExtensionClientTransactionListener (ClientTransactionExtensionCollection extensions)
    {
      _extensions = extensions;
    }

    public ClientTransactionExtensionCollection Extensions
    {
      get { return _extensions; }
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
      // not handled by this listener
    }

    public void SubTransactionCreated (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      // not handled by this listener
    }

    public void NewObjectCreating (ClientTransaction clientTransaction, Type type, DomainObject instance)
    {
      Extensions.NewObjectCreating (clientTransaction, type);
    }

    public void ObjectsLoading (ClientTransaction clientTransaction, ReadOnlyCollection<ObjectID> objectIDs)
    {
      Extensions.ObjectsLoading (clientTransaction, objectIDs);
    }

    public void ObjectsUnloaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      Extensions.ObjectsUnloaded (clientTransaction, unloadedDomainObjects);
    }

    public void ObjectsLoaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      Extensions.ObjectsLoaded (clientTransaction, domainObjects);
    }

    public void ObjectsUnloading (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      Extensions.ObjectsUnloading (clientTransaction, unloadedDomainObjects);
    }

    public void ObjectDeleting (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      Extensions.ObjectDeleting (clientTransaction, domainObject);
    }

    public void ObjectDeleted (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      Extensions.ObjectDeleted (clientTransaction, domainObject);
    }

    public void PropertyValueReading (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
      Extensions.PropertyValueReading (clientTransaction, dataContainer, propertyValue, valueAccess);
    }

    public void PropertyValueRead (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
      Extensions.PropertyValueRead (clientTransaction, dataContainer, propertyValue, value, valueAccess);
    }

    public void PropertyValueChanging (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      if (propertyValue.Definition.PropertyType != typeof (ObjectID))
        Extensions.PropertyValueChanging (clientTransaction, dataContainer, propertyValue, oldValue, newValue);
    }

    public void PropertyValueChanged (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      if (propertyValue.Definition.PropertyType != typeof (ObjectID))
        Extensions.PropertyValueChanged (clientTransaction, dataContainer, propertyValue, oldValue, newValue);
    }

    public void RelationReading (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
      Extensions.RelationReading (clientTransaction, domainObject, propertyName, valueAccess);
    }

    public void RelationReading (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, ValueAccess valueAccess)
    {
      Extensions.RelationReading (clientTransaction, domainObject, relationEndPointDefinition.PropertyName, valueAccess);
    }

    public void RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess)
    {
      Extensions.RelationRead (clientTransaction, domainObject, propertyName, relatedObject, valueAccess);
    }

    public void RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, DomainObject relatedObject, ValueAccess valueAccess)
    {
      Extensions.RelationRead (clientTransaction, domainObject, relationEndPointDefinition.PropertyName, relatedObject, valueAccess);
    }

    public void RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, ReadOnlyDomainObjectCollectionAdapter<DomainObject> relatedObjects, ValueAccess valueAccess)
    {
      Extensions.RelationRead (clientTransaction, domainObject, propertyName, relatedObjects, valueAccess);
    }

    public void RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, ReadOnlyDomainObjectCollectionAdapter<DomainObject> relatedObjects, ValueAccess valueAccess)
    {
      Extensions.RelationRead (clientTransaction, domainObject, relationEndPointDefinition.PropertyName, relatedObjects, valueAccess);
    }

    public void RelationChanging (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      Extensions.RelationChanging (clientTransaction, domainObject, relationEndPointDefinition.PropertyName, oldRelatedObject, newRelatedObject);
    }

    public void RelationChanged (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName)
    {
      Extensions.RelationChanged (clientTransaction, domainObject, propertyName);
    }

    public void RelationChanged (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition)
    {
      Extensions.RelationChanged (clientTransaction, domainObject, relationEndPointDefinition.PropertyName);
    }

    public QueryResult<T> FilterQueryResult<T> (ClientTransaction clientTransaction, QueryResult<T> queryResult) where T: DomainObject
    {
      return Extensions.FilterQueryResult (clientTransaction, queryResult);
    }

    public void TransactionCommitting (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      Extensions.Committing (clientTransaction, domainObjects);
    }

    public void TransactionCommitted (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      Extensions.Committed (clientTransaction, domainObjects);
    }

    public void TransactionRollingBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      Extensions.RollingBack (clientTransaction, domainObjects);
    }

    public void TransactionRolledBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      Extensions.RolledBack (clientTransaction, domainObjects);
    }

    public void RelationEndPointMapRegistering (ClientTransaction clientTransaction, RelationEndPoint endPoint)
    {
      // not handled by this listener
    }

    public void RelationEndPointMapUnregistering (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      // not handled by this listener
    }

    public void RelationEndPointUnloading (ClientTransaction clientTransaction, RelationEndPoint endPoint)
    {
      // not handled by this listener
    }

    public void DataManagerMarkingObjectInvalid (ClientTransaction clientTransaction, ObjectID id)
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
