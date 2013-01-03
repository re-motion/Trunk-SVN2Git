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
using System.Collections.ObjectModel;
using Remotion.Data.DomainObjects.DataManagement;
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
  public class ExtensionClientTransactionListener : ClientTransactionListenerBase
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

    public override void TransactionInitialize (ClientTransaction clientTransaction)
    {
      _extension.TransactionInitialize (clientTransaction);
    }

    public override void TransactionDiscard (ClientTransaction clientTransaction)
    {
      _extension.TransactionDiscard (clientTransaction);
    }

    public override void SubTransactionCreating (ClientTransaction clientTransaction)
    {
      _extension.SubTransactionCreating (clientTransaction);
    }

    public override void SubTransactionInitialize (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      _extension.SubTransactionInitialize (clientTransaction, subTransaction);
    }

    public override void SubTransactionCreated (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      _extension.SubTransactionCreated (clientTransaction, subTransaction);
    }

    public override void NewObjectCreating (ClientTransaction clientTransaction, Type type)
    {
      _extension.NewObjectCreating (clientTransaction, type);
    }

    public override void ObjectsLoading (ClientTransaction clientTransaction, ReadOnlyCollection<ObjectID> objectIDs)
    {
      _extension.ObjectsLoading (clientTransaction, objectIDs);
    }

    public override void ObjectsUnloaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      _extension.ObjectsUnloaded (clientTransaction, unloadedDomainObjects);
    }

    public override void ObjectsLoaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      _extension.ObjectsLoaded (clientTransaction, domainObjects);
    }

    public override void ObjectsUnloading (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      _extension.ObjectsUnloading (clientTransaction, unloadedDomainObjects);
    }

    public override void ObjectDeleting (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      _extension.ObjectDeleting (clientTransaction, domainObject);
    }

    public override void ObjectDeleted (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      _extension.ObjectDeleted (clientTransaction, domainObject);
    }

    public override void PropertyValueReading (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, ValueAccess valueAccess)
    {
      _extension.PropertyValueReading (clientTransaction, domainObject, propertyDefinition, valueAccess);
    }

    public override void PropertyValueRead (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, object value, ValueAccess valueAccess)
    {
      _extension.PropertyValueRead (clientTransaction, domainObject, propertyDefinition, value, valueAccess);
    }

    public override void PropertyValueChanging (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, object oldValue, object newValue)
    {
      _extension.PropertyValueChanging (clientTransaction, domainObject, propertyDefinition, oldValue, newValue);
    }

    public override void PropertyValueChanged (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, object oldValue, object newValue)
    {
      _extension.PropertyValueChanged (clientTransaction, domainObject, propertyDefinition, oldValue, newValue);
    }

    public override void RelationReading (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, ValueAccess valueAccess)
    {
      _extension.RelationReading (clientTransaction, domainObject, relationEndPointDefinition, valueAccess);
    }

    public override void RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, DomainObject relatedObject, ValueAccess valueAccess)
    {
      _extension.RelationRead (clientTransaction, domainObject, relationEndPointDefinition, relatedObject, valueAccess);
    }

    public override void RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, ReadOnlyDomainObjectCollectionAdapter<DomainObject> relatedObjects, ValueAccess valueAccess)
    {
      _extension.RelationRead (clientTransaction, domainObject, relationEndPointDefinition, relatedObjects, valueAccess);
    }

    public override void RelationChanging (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      _extension.RelationChanging (clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject);
    }

    public override void RelationChanged (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      _extension.RelationChanged (clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject);
    }

    public override QueryResult<T> FilterQueryResult<T> (ClientTransaction clientTransaction, QueryResult<T> queryResult)
    {
      return _extension.FilterQueryResult (clientTransaction, queryResult);
    }

    public override void TransactionCommitting (
        ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects, ICommittingEventRegistrar eventRegistrar)
    {
      _extension.Committing (clientTransaction, domainObjects, eventRegistrar);
    }

    public override void TransactionCommitValidate (ClientTransaction clientTransaction, ReadOnlyCollection<PersistableData> committedData)
    {
      _extension.CommitValidate (clientTransaction, committedData);
    }

    public override void TransactionCommitted (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      _extension.Committed (clientTransaction, domainObjects);
    }

    public override void TransactionRollingBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      _extension.RollingBack (clientTransaction, domainObjects);
    }

    public override void TransactionRolledBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      _extension.RolledBack (clientTransaction, domainObjects);
    }
  }
}
