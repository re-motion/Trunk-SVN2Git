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
    public void TransactionInitializing (ClientTransaction clientTransaction)
    {
      // not handled by this listener
    }

    public void TransactionDiscarding (ClientTransaction clientTransaction)
    {
      // not handled by this listener
    }

    public virtual void SubTransactionCreating (ClientTransaction clientTransaction)
    {
      EnsureWriteable (clientTransaction, "SubTransactionCreating");
    }

    public virtual void SubTransactionCreated (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      Assertion.IsTrue (clientTransaction.IsReadOnly); // after a subtransaction has been created, the parent must be read-only
    }

    public virtual void NewObjectCreating (ClientTransaction clientTransaction, Type type, DomainObject instance)
    {
      EnsureWriteable (clientTransaction, "NewObjectCreating");
    }

    public virtual void ObjectsLoading (ClientTransaction clientTransaction, ReadOnlyCollection<ObjectID> objectIDs)
    {
      EnsureWriteable (clientTransaction, "ObjectsLoading");
    }

    public void ObjectsUnloaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      Assertion.IsFalse (clientTransaction.IsReadOnly);
    }

    public virtual void ObjectsLoaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      Assertion.IsFalse (clientTransaction.IsReadOnly);
    }

    public void ObjectsUnloading (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      EnsureWriteable (clientTransaction, "ObjectsUnloading");
    }

    public virtual void ObjectDeleting (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      EnsureWriteable (clientTransaction, "ObjectDeleting");
    }

    public virtual void ObjectDeleted (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      Assertion.IsFalse (clientTransaction.IsReadOnly);
    }

    public virtual void PropertyValueReading (
        ClientTransaction clientTransaction,
        DataContainer dataContainer,
        PropertyValue propertyValue,
        ValueAccess valueAccess)
    {
    }

    public virtual void PropertyValueRead (
        ClientTransaction clientTransaction,
        DataContainer dataContainer,
        PropertyValue propertyValue,
        object value,
        ValueAccess valueAccess)
    {
    }

    public virtual void PropertyValueChanging (
        ClientTransaction clientTransaction,
        DataContainer dataContainer,
        PropertyValue propertyValue,
        object oldValue,
        object newValue)
    {
      EnsureWriteable (clientTransaction, "PropertyValueChanging");
    }

    public virtual void PropertyValueChanged (
        ClientTransaction clientTransaction,
        DataContainer dataContainer,
        PropertyValue propertyValue,
        object oldValue,
        object newValue)
    {
      Assertion.IsFalse (clientTransaction.IsReadOnly);
    }

    public virtual void RelationReading (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
    }

    public virtual void RelationRead (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        string propertyName,
        DomainObject relatedObject,
        ValueAccess valueAccess)
    {
    }

    public virtual void RelationRead (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        string propertyName,
        ReadOnlyDomainObjectCollectionAdapter<DomainObject> relatedObjects,
        ValueAccess valueAccess)
    {
    }

    public virtual void RelationChanging (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        string propertyName,
        DomainObject oldRelatedObject,
        DomainObject newRelatedObject)
    {
      EnsureWriteable (clientTransaction, "RelationChanging");
    }

    public virtual void RelationChanged (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName)
    {
      Assertion.IsFalse (clientTransaction.IsReadOnly);
    }

    public QueryResult<T> FilterQueryResult<T> (ClientTransaction clientTransaction, QueryResult<T> queryResult) where T: DomainObject
    {
      return queryResult;
    }

    public virtual void TransactionCommitting (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      EnsureWriteable (clientTransaction, "TransactionCommitting");
    }

    public virtual void TransactionCommitted (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      Assertion.IsFalse (clientTransaction.IsReadOnly);
    }

    public virtual void TransactionRollingBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      EnsureWriteable (clientTransaction, "TransactionRollingBack");
    }

    public virtual void TransactionRolledBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> domainObjects)
    {
      Assertion.IsFalse (clientTransaction.IsReadOnly);
    }

    public virtual void RelationEndPointMapRegistering (ClientTransaction clientTransaction, RelationEndPoint endPoint)
    {
      EnsureWriteable (clientTransaction, "RelationEndPointMapRegistering");
    }

    public virtual void RelationEndPointMapUnregistering (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      EnsureWriteable (clientTransaction, "RelationEndPointMapUnregistering");
    }

    public void RelationEndPointUnloading (ClientTransaction clientTransaction, RelationEndPoint endPoint)
    {
      EnsureWriteable (clientTransaction, "RelationEndPointUnloading");
    }

    public virtual void DataManagerMarkingObjectInvalid (ClientTransaction clientTransaction, ObjectID id)
    {
      // also allowed for read-only transactions
    }

    public virtual void DataContainerMapRegistering (ClientTransaction clientTransaction, DataContainer container)
    {
      Assertion.IsFalse (clientTransaction.IsReadOnly);
    }

    public virtual void DataContainerMapUnregistering (ClientTransaction clientTransaction, DataContainer container)
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