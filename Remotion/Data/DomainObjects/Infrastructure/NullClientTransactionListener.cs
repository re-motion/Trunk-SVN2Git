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
  /// <summary>
  /// <see cref="INullObject"/> implementation of <see cref="IClientTransactionListener"/>.
  /// </summary>
  public sealed class NullClientTransactionListener : IClientTransactionListener
  {
    public static readonly IClientTransactionListener Instance = new NullClientTransactionListener();

    private NullClientTransactionListener ()
    {
    }

    public bool IsNull
    {
      get { return true; }
    }

    public void TransactionInitializing ()
    {
    }

    public void TransactionDiscarding ()
    {
    }

    public void SubTransactionCreated (ClientTransaction subTransaction)
    {
    }

    public void SubTransactionCreating ()
    {
    }

    public void NewObjectCreating (Type type, DomainObject instance)
    {
    }

    public void ObjectsLoading (ReadOnlyCollection<ObjectID> objectIDs)
    {
    }

    public void ObjectsLoaded (ReadOnlyCollection<DomainObject> domainObjects)
    {
    }

    public void ObjectsUnloading (ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
    }

    public void ObjectsUnloaded (ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
    }

    public void ObjectDeleting (DomainObject domainObject)
    {
    }

    public void ObjectDeleted (DomainObject domainObject)
    {
    }

    public void PropertyValueReading (DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
    }

    public void PropertyValueRead (DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
    }

    public void PropertyValueChanging (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
    }

    public void PropertyValueChanged (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
    }

    public void RelationReading (DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
    }

    public void RelationRead (DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess)
    {
    }

    public void RelationRead (
        DomainObject domainObject, string propertyName, ReadOnlyDomainObjectCollectionAdapter<DomainObject> relatedObjects, ValueAccess valueAccess)
    {
    }

    public void RelationChanging (DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
    }

    public void RelationChanged (DomainObject domainObject, string propertyName)
    {
    }

    public QueryResult<T> FilterQueryResult<T> (QueryResult<T> queryResult) where T: DomainObject
    {
      return queryResult;
    }

    public void TransactionCommitting (ReadOnlyCollection<DomainObject> domainObjects)
    {
    }

    public void TransactionCommitted (ReadOnlyCollection<DomainObject> domainObjects)
    {
    }

    public void TransactionRollingBack (ReadOnlyCollection<DomainObject> domainObjects)
    {
    }

    public void TransactionRolledBack (ReadOnlyCollection<DomainObject> domainObjects)
    {
    }

    public void RelationEndPointMapRegistering (RelationEndPoint endPoint)
    {
    }

    public void RelationEndPointMapUnregistering (RelationEndPointID endPointID)
    {
    }

    public void RelationEndPointUnloading (RelationEndPoint endPoint)
    {
    }

    public void DataManagerMarkingObjectDiscarded (ObjectID id)
    {
    }

    public void DataContainerMapRegistering (DataContainer container)
    {
    }

    public void DataContainerMapUnregistering (DataContainer container)
    {
    }
  }
}