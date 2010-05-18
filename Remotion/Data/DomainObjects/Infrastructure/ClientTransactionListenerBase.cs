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

namespace Remotion.Data.DomainObjects.Infrastructure
{
  [Serializable]
  public abstract class ClientTransactionListenerBase : IClientTransactionListener
  {
    public virtual void TransactionInitializing ()
    {
    }

    public virtual void TransactionDiscarding ()
    {
    }

    public virtual void SubTransactionCreating ()
    {
    }

    public virtual void SubTransactionCreated (ClientTransaction subTransaction)
    {
    }

    public virtual void NewObjectCreating (Type type, DomainObject instance)
    {
    }

    public virtual void ObjectsLoading (ReadOnlyCollection<ObjectID> objectIDs)
    {
    }

    public virtual void ObjectsLoaded (ReadOnlyCollection<DomainObject> domainObjects)
    {
    }

    public virtual void ObjectsUnloading (ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
    }

    public virtual void ObjectsUnloaded (ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
    }

    public virtual void ObjectDeleting (DomainObject domainObject)
    {
    }

    public virtual void ObjectDeleted (DomainObject domainObject)
    {
    }

    public virtual void PropertyValueReading (DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
    }

    public virtual void PropertyValueRead (DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
    }

    public virtual void PropertyValueChanging (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
    }

    public virtual void PropertyValueChanged (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
    }

    public virtual void RelationReading (DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
    }

    public virtual void RelationRead (DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess)
    {
    }

    public virtual void RelationRead (
        DomainObject domainObject, string propertyName, ReadOnlyDomainObjectCollectionAdapter<DomainObject> relatedObjects, ValueAccess valueAccess)
    {
    }

    public virtual void RelationChanging (
        DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
    }

    public virtual void RelationChanged (DomainObject domainObject, string propertyName)
    {
    }

    public virtual QueryResult<T> FilterQueryResult<T> (QueryResult<T> queryResult) where T: DomainObject
    {
      return queryResult;
    }

    public virtual void TransactionCommitting (ReadOnlyCollection<DomainObject> domainObjects)
    {
    }

    public virtual void TransactionCommitted (ReadOnlyCollection<DomainObject> domainObjects)
    {
    }

    public virtual void TransactionRollingBack (ReadOnlyCollection<DomainObject> domainObjects)
    {
    }

    public virtual void TransactionRolledBack (ReadOnlyCollection<DomainObject> domainObjects)
    {
    }

    public virtual void RelationEndPointMapRegistering (RelationEndPoint endPoint)
    {
    }

    public virtual void RelationEndPointMapUnregistering (RelationEndPointID endPointID)
    {
    }

    public virtual void RelationEndPointUnloading (RelationEndPoint endPoint)
    {
    }

    public virtual void DataManagerMarkingObjectInvalid (ObjectID id)
    {
    }

    public virtual void DataContainerMapRegistering (DataContainer container)
    {
    }

    public virtual void DataContainerMapUnregistering (DataContainer container)
    {
    }

    public virtual bool IsNull
    {
      get { return false; }
    }
  }
}