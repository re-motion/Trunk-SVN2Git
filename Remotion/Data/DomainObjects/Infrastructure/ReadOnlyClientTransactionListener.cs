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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// An implementation of <see cref="IClientTransactionListener"/> which throws an exception if the <see cref="ClientTransaction"/> is about
  /// to be modified while in a read-only state.
  /// </summary>
  [Serializable]
  public class ReadOnlyClientTransactionListener : IClientTransactionListener
  {
    public void TransactionInitialize (ClientTransaction clientTransaction)
    {
      // not handled by this listener
    }

    public void TransactionDiscard (ClientTransaction clientTransaction)
    {
      // not handled by this listener
    }

    public void SubTransactionCreating (ClientTransaction clientTransaction)
    {
      EnsureWriteable (clientTransaction, "SubTransactionCreating");
    }

    public void SubTransactionInitialize (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      Assertion.IsTrue (clientTransaction.IsReadOnly); // while a subtransaction is being created, the parent must be read-only
    }

    public void SubTransactionCreated (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      Assertion.IsTrue (clientTransaction.IsReadOnly); // after a subtransaction has been created, the parent must be read-only
    }

    public void NewObjectCreating (ClientTransaction clientTransaction, Type type)
    {
      EnsureWriteable (clientTransaction, "NewObjectCreating");
    }

    public void ObjectsLoading (ClientTransaction clientTransaction, ReadOnlyCollection<ObjectID> objectIDs)
    {
      EnsureWriteable (clientTransaction, "ObjectsLoading");
    }

    public void ObjectsUnloaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      EnsureWriteable (clientTransaction, "ObjectsUnloaded");
    }

    public void ObjectsLoaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      EnsureWriteable (clientTransaction, "ObjectsLoaded");
    }

    public void ObjectsUnloading (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      EnsureWriteable (clientTransaction, "ObjectsUnloading");
    }

    public void ObjectDeleting (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      EnsureWriteable (clientTransaction, "ObjectDeleting");
    }

    public void ObjectDeleted (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      EnsureWriteable (clientTransaction, "ObjectDeleted");
    }

    public void PropertyValueReading (
        ClientTransaction clientTransaction,
        DataContainer dataContainer,
        PropertyValue propertyValue,
        ValueAccess valueAccess)
    {
    }

    public void PropertyValueRead (
        ClientTransaction clientTransaction,
        DataContainer dataContainer,
        PropertyValue propertyValue,
        object value,
        ValueAccess valueAccess)
    {
    }

    public void PropertyValueChanging (
        ClientTransaction clientTransaction,
        DataContainer dataContainer,
        PropertyValue propertyValue,
        object oldValue,
        object newValue)
    {
      EnsureWriteable (clientTransaction, "PropertyValueChanging");
    }

    public void PropertyValueChanged (
        ClientTransaction clientTransaction,
        DataContainer dataContainer,
        PropertyValue propertyValue,
        object oldValue,
        object newValue)
    {
      EnsureWriteable (clientTransaction, "PropertyValueChanged");
    }

    public void RelationReading (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, ValueAccess valueAccess)
    {
    }

    public void RelationRead (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject relatedObject,
        ValueAccess valueAccess)
    {
    }

    public void RelationRead (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        ReadOnlyDomainObjectCollectionAdapter<DomainObject> relatedObjects,
        ValueAccess valueAccess)
    {
    }

    public void RelationChanging (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject oldRelatedObject,
        DomainObject newRelatedObject)
    {
      EnsureWriteable (clientTransaction, "RelationChanging");
    }

    public void RelationChanged (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      EnsureWriteable (clientTransaction, "RelationChanged");
    }

    public QueryResult<T> FilterQueryResult<T> (ClientTransaction clientTransaction, QueryResult<T> queryResult) where T: DomainObject
    {
      return queryResult;
    }

    public IEnumerable<T> FilterCustomQueryResult<T> (ClientTransaction clientTransaction, IQuery query, IEnumerable<T> results)
    {
      return results;
    }

    public void TransactionCommitting (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      EnsureWriteable (clientTransaction, "TransactionCommitting");
    }

    public void TransactionCommitValidate (ClientTransaction clientTransaction, ReadOnlyCollection<PersistableData> committedData)
    {
      EnsureWriteable (clientTransaction, "TransactionCommitValidate");
    }

    public void TransactionCommitted (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      EnsureWriteable (clientTransaction, "TransactionCommitted");
    }

    public void TransactionRollingBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      EnsureWriteable (clientTransaction, "TransactionRollingBack");
    }

    public void TransactionRolledBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      EnsureWriteable (clientTransaction, "TransactionRolledBack");
    }

    public void RelationEndPointMapRegistering (ClientTransaction clientTransaction, IRelationEndPoint endPoint)
    {
      EnsureWriteable (clientTransaction, "RelationEndPointMapRegistering");
    }

    public void RelationEndPointMapUnregistering (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      EnsureWriteable (clientTransaction, "RelationEndPointMapUnregistering");
    }

    public void RelationEndPointUnloading (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      EnsureWriteable (clientTransaction, "RelationEndPointUnloading");
    }

    public void ObjectMarkedInvalid (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      // also allowed for read-only transactions
    }

    public void ObjectMarkedNotInvalid (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      // also allowed for read-only transactions
    }

    public void DataContainerMapRegistering (ClientTransaction clientTransaction, DataContainer container)
    {
      EnsureWriteable (clientTransaction, "DataContainerMapRegistering");
    }

    public void DataContainerMapUnregistering (ClientTransaction clientTransaction, DataContainer container)
    {
      EnsureWriteable (clientTransaction, "DataContainerMapUnregistering");
    }

    public void DataContainerStateUpdated (ClientTransaction clientTransaction, DataContainer container, StateType newDataContainerState)
    {
      // low-level event also allowed for read-only transactions
    }

    public void VirtualRelationEndPointStateUpdated (ClientTransaction clientTransaction, RelationEndPointID endPointID, bool? newEndPointChangeState)
    {
      // low-level event also allowed for read-only transactions
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }

    private void EnsureWriteable (ClientTransaction clientTransaction, string operation)
    {
      if (clientTransaction.IsReadOnly)
      {
        string message = string.Format (
            "The operation cannot be executed because the ClientTransaction is read-only. "
            + "Offending transaction modification: {0}.",
            operation);
        throw new ClientTransactionReadOnlyException (message);
      }
    }
  }
}