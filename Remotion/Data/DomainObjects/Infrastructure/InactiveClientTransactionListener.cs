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
using System.Linq;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// An implementation of <see cref="IClientTransactionListener"/> which throws an exception if the <see cref="ClientTransaction"/> is about
  /// to be modified while inactive.
  /// </summary>
  [Serializable]
  public class InactiveClientTransactionListener : IClientTransactionListener
  {
    public void TransactionInitialize (ClientTransaction clientTransaction)
    {
      // not handled by this listener
    }

    public void TransactionDiscard (ClientTransaction clientTransaction)
    {
      // allowed (but TODO RM-5001)
    }

    public void SubTransactionCreating (ClientTransaction clientTransaction)
    {
      EnsureWriteable (clientTransaction, "SubTransactionCreating");
    }

    public void SubTransactionInitialize (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      Assertion.IsFalse (clientTransaction.IsActive); // while a subtransaction is being created, the parent must already be inactive
    }

    public void SubTransactionCreated (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      Assertion.IsFalse (clientTransaction.IsActive); // after a subtransaction has been created, the parent must already be inactive
    }

    public void NewObjectCreating (ClientTransaction clientTransaction, Type type)
    {
      EnsureWriteable (clientTransaction, "NewObjectCreating");
    }

    public void ObjectsLoading (ClientTransaction clientTransaction, ReadOnlyCollection<ObjectID> objectIDs)
    {
      // Allowed - this should be safe since the subtransaction can't have data for this object
      Assertion.DebugAssert (
          clientTransaction.SubTransaction == null
          || objectIDs.All (id => clientTransaction.SubTransaction.DataManager.DataContainers[id] == null));
    }

    public void ObjectsLoaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      // Allowed
    }

    public void ObjectsNotFound (ClientTransaction clientTransaction, ReadOnlyCollection<ObjectID> objectIDs)
    {
      // Allowed
    }

    public void ObjectsUnloading (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      // Allowed for read-only transactions, as the end-user API always affects the whole hierarchy
      // (DataContainerUnregistering and RelationEndPointUnregistering assert on the actual modification, though)
    }

    public void ObjectsUnloaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      // Allowed for read-only transactions, as the end-user API always affects the whole hierarchy
      // (DataContainerUnregistering and RelationEndPointUnregistering assert on the actual modification, though)
    }

    public void ObjectDeleting (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      EnsureWriteable (clientTransaction, "ObjectDeleting");
    }

    public void ObjectDeleted (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      EnsureWriteable (clientTransaction, "ObjectDeleted");
    }

    public void PropertyValueReading (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, ValueAccess valueAccess)
    {
      // Allowed
    }

    public void PropertyValueRead (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, object value, ValueAccess valueAccess)
    {
      // Allowed
    }

    public void PropertyValueChanging (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, object oldValue, object newValue)
    {
      EnsureWriteable (clientTransaction, "PropertyValueChanging");
    }

    public void PropertyValueChanged (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, object oldValue, object newValue)
    {
      EnsureWriteable (clientTransaction, "PropertyValueChanged");
    }

    public void RelationReading (ClientTransaction clientTransaction, DomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, ValueAccess valueAccess)
    {
      // Allowed
    }

    public void RelationRead (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject relatedObject,
        ValueAccess valueAccess)
    {
      // Allowed
    }

    public void RelationRead (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        ReadOnlyDomainObjectCollectionAdapter<DomainObject> relatedObjects,
        ValueAccess valueAccess)
    {
      // Allowed
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
      // Allowed
      return queryResult;
    }

    public IEnumerable<T> FilterCustomQueryResult<T> (ClientTransaction clientTransaction, IQuery query, IEnumerable<T> results)
    {
      // Allowed
      return results;
    }

    public void TransactionCommitting (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects, ICommittingEventRegistrar eventRegistrar)
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
      // Safe assuming the subtransaction does not have a complete end-point for the same ID (subtransaction needs to be loaded later)
      // (or when it has been unlocked - during subtx.Commit)
      Assertion.IsTrue (
          clientTransaction.IsActive
          || clientTransaction.SubTransaction == null
          || IsNullOrIncomplete (clientTransaction.SubTransaction.DataManager.RelationEndPoints[endPoint.ID]));
    }

    public void RelationEndPointMapUnregistering (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      // Safe assuming the subtransaction does not have a complete end-point for the same ID (subtransaction needs to be unloaded first)
      // (or when it has been unlocked - during subtx.Commit)
      Assertion.IsTrue (
          clientTransaction.IsActive
          || clientTransaction.SubTransaction == null
          || IsNullOrIncomplete (clientTransaction.SubTransaction.DataManager.RelationEndPoints[endPointID]));
    }

    public void RelationEndPointUnloading (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      // Safe assuming the subtransaction does not have a complete end-point for the same ID (subtransaction needs to be unloaded first)
      Assertion.IsTrue (
          clientTransaction.SubTransaction == null 
          || IsNullOrIncomplete (clientTransaction.SubTransaction.DataManager.RelationEndPoints[endPointID]));
    }

    public void ObjectMarkedInvalid (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      EnsureWriteable (clientTransaction, "ObjectMarkedInvalid");
    }

    public void ObjectMarkedNotInvalid (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      EnsureWriteable (clientTransaction, "ObjectMarkedNotInvalid");
    }

    public void DataContainerMapRegistering (ClientTransaction clientTransaction, DataContainer container)
    {
      // Safe assuming the subtransaction cannot already have a DataContainer for the same object (subtransaction needs to be loaded later)
      // (or when it has been unlocked - during subtx.Commit)
      Assertion.IsTrue (
          clientTransaction.IsActive
          || clientTransaction.SubTransaction == null
          || clientTransaction.SubTransaction.DataManager.DataContainers[container.ID] == null);
    }

    public void DataContainerMapUnregistering (ClientTransaction clientTransaction, DataContainer container)
    {
      // Safe assuming the subtransaction does not have a DataContainer for the same object (subtransaction needs to be unloaded first)
      // (or when it has been unlocked - during subtx.Commit)
      Assertion.IsTrue (
          clientTransaction.IsActive
          || clientTransaction.SubTransaction == null 
          || clientTransaction.SubTransaction.DataManager.DataContainers[container.ID] == null);
    }

    public void DataContainerStateUpdated (ClientTransaction clientTransaction, DataContainer container, StateType newDataContainerState)
    {
      EnsureWriteable (clientTransaction, "DataContainerStateUpdated");
    }

    public void VirtualRelationEndPointStateUpdated (ClientTransaction clientTransaction, RelationEndPointID endPointID, bool? newEndPointChangeState)
    {
      EnsureWriteable (clientTransaction, "VirtualRelationEndPointStateUpdated");
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }

    private void EnsureWriteable (ClientTransaction clientTransaction, string operation)
    {
      if (!clientTransaction.IsActive)
      {
        string message = string.Format (
            "The operation cannot be executed because the ClientTransaction is inactive. "
            + "Offending transaction modification: {0}.",
            operation);
        throw new ClientTransactionReadOnlyException (message);
      }
    }

    private bool IsNullOrIncomplete (IRelationEndPoint relationEndPoint)
    {
      return relationEndPoint == null || !relationEndPoint.IsDataComplete;
    }
  }
}