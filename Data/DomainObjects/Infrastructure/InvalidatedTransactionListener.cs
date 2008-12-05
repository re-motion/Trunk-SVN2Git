// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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

    public void ObjectLoading (ObjectID id)
    {
      throw CreateException();
    }

    public void ObjectInitializedFromDataContainer (ObjectID id, DomainObject instance)
    {
      throw CreateException();
    }

    public void ObjectsLoaded (DomainObjectCollection domainObjects)
    {
      throw CreateException();
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

    public void RelationRead (DomainObject domainObject, string propertyName, DomainObjectCollection relatedObjects, ValueAccess valueAccess)
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

    public void FilterQueryResult (DomainObjectCollection queryResult, IQuery query)
    {
      throw CreateException();
    }

    public void TransactionCommitting (DomainObjectCollection domainObjects)
    {
      throw CreateException();
    }

    public void TransactionCommitted (DomainObjectCollection domainObjects)
    {
      throw CreateException();
    }

    public void TransactionRollingBack (DomainObjectCollection domainObjects)
    {
      throw CreateException();
    }

    public void TransactionRolledBack (DomainObjectCollection domainObjects)
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

    public void RelationEndPointMapPerformingDelete (RelationEndPointID[] endPointIDs)
    {
      throw CreateException();
    }

    public void RelationEndPointMapCopyingFrom (RelationEndPointMap source)
    {
      throw CreateException();
    }

    public void RelationEndPointMapCopyingTo (RelationEndPointMap destination)
    {
      throw CreateException();
    }

    public void DataManagerMarkingObjectDiscarded (ObjectID id)
    {
      throw CreateException();
    }

    public void DataManagerCopyingFrom (DataManager source)
    {
      throw CreateException();
    }

    public void DataManagerCopyingTo (DataManager destination)
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

    public void DataContainerMapCopyingFrom (DataContainerMap source)
    {
      throw CreateException();
    }

    public void DataContainerMapCopyingTo (DataContainerMap destination)
    {
      throw CreateException();
    }
  }
}
