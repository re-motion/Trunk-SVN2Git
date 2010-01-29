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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transaction
{
  [Serializable]
  public class ClientTransactionExtensionWithQueryFiltering : IClientTransactionExtension
  {
    public void SubTransactionCreating (ClientTransaction parentClientTransaction)
    {
    }

    public void SubTransactionCreated (ClientTransaction parentClientTransaction, ClientTransaction subTransaction)
    {
    }

    public virtual void NewObjectCreating (ClientTransaction clientTransaction, Type type)
    {
    }

    public virtual void ObjectsLoading (ClientTransaction clientTransaction, ReadOnlyCollection<ObjectID> objectIDs)
    {
    }

    public virtual void NewObjectCreated (DomainObject newDomainObject)
    {
    }

    public virtual void ObjectsLoaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> loadedDomainObjects)
    {
    }

    public void ObjectsUnloading (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
    }

    public void ObjectsUnloaded (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
    }

    public virtual void ObjectDeleting (ClientTransaction clientTransaction, DomainObject domainObject)
    {
    }

    public virtual void ObjectDeleted (ClientTransaction clientTransaction, DomainObject domainObject)
    {
    }

    public virtual void PropertyValueReading (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
    }

    public virtual void PropertyValueRead (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
    }

    public virtual void PropertyValueChanging (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
    }

    public virtual void PropertyValueChanged (ClientTransaction clientTransaction, DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
    }

    public virtual void RelationReading (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
    }

    public virtual void RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess)
    {
    }

    public virtual void RelationRead (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, ReadOnlyDomainObjectCollectionAdapter<DomainObject> relatedObjects, ValueAccess valueAccess)
    {
    }

    public virtual void RelationChanging (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
    }

    public virtual void RelationChanged (ClientTransaction clientTransaction, DomainObject domainObject, string propertyName)
    {
    }

    public virtual QueryResult<T> FilterQueryResult<T> (ClientTransaction clientTransaction, QueryResult<T> queryResult) where T : DomainObject
    {
      if (queryResult.Count > 0)
      {
        var queryResultList = queryResult.ToObjectList ();
        queryResultList.RemoveAt (0);
        return new QueryResult<T> (queryResult.Query, queryResultList.ToArray());
      }
      return queryResult;
    }

    public virtual void Committing (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> changedDomainObjects)
    {
    }

    public virtual void Committed (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> changedDomainObjects)
    {
    }

    public virtual void RollingBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> changedDomainObjects)
    {
    }

    public virtual void RolledBack (ClientTransaction clientTransaction, ReadOnlyCollection<DomainObject> changedDomainObjects)
    {
    }
  }
}
