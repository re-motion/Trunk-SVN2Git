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
  public class InvalidatedTransactionListener : IClientTransactionListener
  {
    private Exception CreateException ()
    {
      return new InvalidOperationException ("The transaction can no longer be used because it has been discarded.");
    }

    public void SubTransactionCreating ()
    {
      throw CreateException();
    }

    public void SubTransactionCreated (ClientTransaction subTransaction)
    {
      throw CreateException();
    }

    public void NewObjectCreating (Type type, DomainObject instance)
    {
      throw CreateException();
    }

    public void ObjectsLoading (ReadOnlyCollection<ObjectID> objectIDs)
    {
      throw CreateException();
    }

    public void ObjectsUnloaded (ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      throw CreateException ();
    }

    public void ObjectsLoaded (ReadOnlyCollection<DomainObject> domainObjects)
    {
      throw CreateException();
    }

    public void ObjectsUnloading (ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      throw CreateException ();
    }

    public void ObjectDeleting (DomainObject domainObject)
    {
      throw CreateException();
    }

    public void ObjectDeleted (DomainObject domainObject)
    {
      throw CreateException();
    }

    public void PropertyValueReading (DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
      throw CreateException();
    }

    public void PropertyValueRead (DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
      throw CreateException();
    }

    public void PropertyValueChanging (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      throw CreateException();
    }

    public void PropertyValueChanged (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      throw CreateException();
    }

    public void RelationReading (DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
      throw CreateException();
    }

    public void RelationRead (DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess)
    {
      throw CreateException();
    }

    public void RelationRead (DomainObject domainObject, string propertyName, ReadOnlyDomainObjectCollectionAdapter<DomainObject> relatedObjects, ValueAccess valueAccess)
    {
      throw CreateException();
    }

    public void RelationChanging (DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      throw CreateException();
    }

    public void RelationChanged (DomainObject domainObject, string propertyName)
    {
      throw CreateException();
    }

    public QueryResult<T> FilterQueryResult<T> (QueryResult<T> queryResult) where T : DomainObject
    {
      throw CreateException();
    }

    public void TransactionCommitting (ReadOnlyCollection<DomainObject> domainObjects)
    {
      throw CreateException();
    }

    public void TransactionCommitted (ReadOnlyCollection<DomainObject> domainObjects)
    {
      throw CreateException();
    }

    public void TransactionRollingBack (ReadOnlyCollection<DomainObject> domainObjects)
    {
      throw CreateException();
    }

    public void TransactionRolledBack (ReadOnlyCollection<DomainObject> domainObjects)
    {
      throw CreateException();
    }

    public void RelationEndPointMapRegistering (RelationEndPoint endPoint)
    {
      throw CreateException();
    }

    public void RelationEndPointMapUnregistering (RelationEndPointID endPointID)
    {
      throw CreateException();
    }

    public void RelationEndPointUnloading (RelationEndPoint endPoint)
    {
      throw CreateException();
    }

    public void DataManagerMarkingObjectDiscarded (ObjectID id)
    {
      throw CreateException();
    }

    public void DataContainerMapRegistering (DataContainer container)
    {
      throw CreateException();
    }

    public void DataContainerMapUnregistering (DataContainer container)
    {
      throw CreateException();
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
