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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  [Serializable]
  public class BindingClientTransactionListener : IClientTransactionListener
  {
    private readonly ClientTransaction _bindingTransaction;

    public BindingClientTransactionListener (ClientTransaction bindingTransaction)
    {
      ArgumentUtility.CheckNotNull ("bindingTransaction", bindingTransaction);
      _bindingTransaction = bindingTransaction;
    }

    public void SubTransactionCreating ()
    {
      // not handled by this listener
    }

    public void SubTransactionCreated (ClientTransaction subTransaction)
    {
      // not handled by this listener
    }

    public void NewObjectCreating (Type type, DomainObject instance)
    {
      // not handled by this listener
    }

    public void ObjectLoading (ObjectID id)
    {
      // not handled by this listener
    }

    public void ObjectInitializedFromDataContainer (ObjectID id, DomainObject instance)
    {
      instance.BindToTransaction (_bindingTransaction);
    }

    public void ObjectsLoaded (DomainObjectCollection domainObjects)
    {
      // not handled by this listener
    }

    public void ObjectDeleting (DomainObject domainObject)
    {
      // not handled by this listener
    }

    public void ObjectDeleted (DomainObject domainObject)
    {
      // not handled by this listener
    }

    public void PropertyValueReading (DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
      // not handled by this listener
    }

    public void PropertyValueRead (DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
      // not handled by this listener
    }

    public void PropertyValueChanging (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      // not handled by this listener
    }

    public void PropertyValueChanged (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      // not handled by this listener
    }

    public void RelationReading (DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
      // not handled by this listener
    }

    public void RelationRead (DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess)
    {
      // not handled by this listener
    }

    public void RelationRead (DomainObject domainObject, string propertyName, DomainObjectCollection relatedObjects, ValueAccess valueAccess)
    {
      // not handled by this listener
    }

    public void RelationChanging (DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      // not handled by this listener
    }

    public void RelationChanged (DomainObject domainObject, string propertyName)
    {
      // not handled by this listener
    }

    public void FilterQueryResult (DomainObjectCollection queryResult, IQuery query)
    {
      // not handled by this listener
    }

    public void TransactionCommitting (DomainObjectCollection domainObjects)
    {
      // not handled by this listener
    }

    public void TransactionCommitted (DomainObjectCollection domainObjects)
    {
      // not handled by this listener
    }

    public void TransactionRollingBack (DomainObjectCollection domainObjects)
    {
      // not handled by this listener
    }

    public void TransactionRolledBack (DomainObjectCollection domainObjects)
    {
      // not handled by this listener
    }

    public void RelationEndPointMapRegistering (RelationEndPoint endPoint)
    {
      // not handled by this listener
    }

    public void RelationEndPointMapUnregistering (RelationEndPointID endPointID)
    {
      // not handled by this listener
    }

    public void RelationEndPointMapPerformingDelete (RelationEndPointID[] endPointIDs)
    {
      // not handled by this listener
    }

    public void RelationEndPointMapCopyingFrom (RelationEndPointMap source)
    {
      // not handled by this listener
    }

    public void RelationEndPointMapCopyingTo (RelationEndPointMap destination)
    {
      // not handled by this listener
    }

    public void DataManagerMarkingObjectDiscarded (ObjectID id)
    {
      // not handled by this listener
    }

    public void DataManagerCopyingFrom (DataManager source)
    {
      // not handled by this listener
    }

    public void DataManagerCopyingTo (DataManager destination)
    {
      // not handled by this listener
    }

    public void DataContainerMapRegistering (DataContainer container)
    {
      // not handled by this listener
    }

    public void DataContainerMapUnregistering (DataContainer container)
    {
      // not handled by this listener
    }

    public void DataContainerMapCopyingFrom (DataContainerMap source)
    {
      // not handled by this listener
    }

    public void DataContainerMapCopyingTo (DataContainerMap destination)
    {
      // not handled by this listener
    }
  }
}
