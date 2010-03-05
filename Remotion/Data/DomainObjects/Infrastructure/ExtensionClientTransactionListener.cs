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
  /// A <see cref="IClientTransactionListener"/> implementation that notifies <see cref="IClientTransactionExtension">IClientTransactionExtensions</see>
  /// about transaction events.
  /// </summary>
  /// <remarks>
  /// The <see cref="ClientTransaction"/> class uses this listener to implement its extension mechanism.
  /// </remarks>
  [Serializable]
  public class ExtensionClientTransactionListener : IClientTransactionListener
  {
    private readonly ClientTransactionExtensionCollection _extensions;
    private readonly ClientTransaction _clientTransaction;

    public ExtensionClientTransactionListener (ClientTransaction clientTransaction, ClientTransactionExtensionCollection extensions)
    {
      _clientTransaction = clientTransaction;
      _extensions = extensions;
    }

    public ClientTransactionExtensionCollection Extensions
    {
      get { return _extensions; }
    }

    public void TransactionInitializing ()
    {
      // not handled by this listener
    }

    public void TransactionDiscarding ()
    {
      // not handled by this listener
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
      Extensions.NewObjectCreating (_clientTransaction, type);
    }

    public void ObjectsLoading (ReadOnlyCollection<ObjectID> objectIDs)
    {
      Extensions.ObjectsLoading (_clientTransaction, objectIDs);
    }

    public void ObjectsUnloaded (ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      Extensions.ObjectsUnloaded (_clientTransaction, unloadedDomainObjects);
    }

    public void ObjectsLoaded (ReadOnlyCollection<DomainObject> domainObjects)
    {
      Extensions.ObjectsLoaded (_clientTransaction, domainObjects);
    }

    public void ObjectsUnloading (ReadOnlyCollection<DomainObject> unloadedDomainObjects)
    {
      Extensions.ObjectsUnloading (_clientTransaction, unloadedDomainObjects);
    }

    public void ObjectDeleting (DomainObject domainObject)
    {
      Extensions.ObjectDeleting (_clientTransaction, domainObject);
    }

    public void ObjectDeleted (DomainObject domainObject)
    {
      Extensions.ObjectDeleted (_clientTransaction, domainObject);
    }

    public void PropertyValueReading (DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
      Extensions.PropertyValueReading (_clientTransaction, dataContainer, propertyValue, valueAccess);
    }

    public void PropertyValueRead (DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
      Extensions.PropertyValueRead (_clientTransaction, dataContainer, propertyValue, value, valueAccess);
    }

    public void PropertyValueChanging (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      if (propertyValue.Definition.PropertyType != typeof (ObjectID))
        Extensions.PropertyValueChanging (_clientTransaction, dataContainer, propertyValue, oldValue, newValue);
    }

    public void PropertyValueChanged (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      if (propertyValue.Definition.PropertyType != typeof (ObjectID))
        Extensions.PropertyValueChanged (_clientTransaction, dataContainer, propertyValue, oldValue, newValue);
    }

    public void RelationReading (DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
      Extensions.RelationReading (_clientTransaction, domainObject, propertyName, valueAccess);
    }

    public void RelationRead (DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess)
    {
      Extensions.RelationRead (_clientTransaction, domainObject, propertyName, relatedObject, valueAccess);
    }

    public void RelationRead (DomainObject domainObject, string propertyName, ReadOnlyDomainObjectCollectionAdapter<DomainObject> relatedObjects, ValueAccess valueAccess)
    {
      Extensions.RelationRead (_clientTransaction, domainObject, propertyName, relatedObjects, valueAccess);
    }

    public void RelationChanging (DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      Extensions.RelationChanging (_clientTransaction, domainObject, propertyName, oldRelatedObject, newRelatedObject);
    }

    public void RelationChanged (DomainObject domainObject, string propertyName)
    {
      Extensions.RelationChanged (_clientTransaction, domainObject, propertyName);
    }

    public QueryResult<T> FilterQueryResult<T> (QueryResult<T> queryResult) where T: DomainObject
    {
      return Extensions.FilterQueryResult (_clientTransaction, queryResult);
    }

    public void TransactionCommitting (ReadOnlyCollection<DomainObject> domainObjects)
    {
      Extensions.Committing (_clientTransaction, domainObjects);
    }

    public void TransactionCommitted (ReadOnlyCollection<DomainObject> domainObjects)
    {
      Extensions.Committed (_clientTransaction, domainObjects);
    }

    public void TransactionRollingBack (ReadOnlyCollection<DomainObject> domainObjects)
    {
      Extensions.RollingBack (_clientTransaction, domainObjects);
    }

    public void TransactionRolledBack (ReadOnlyCollection<DomainObject> domainObjects)
    {
      Extensions.RolledBack (_clientTransaction, domainObjects);
    }

    public void RelationEndPointMapRegistering (RelationEndPoint endPoint)
    {
      // not handled by this listener
    }

    public void RelationEndPointMapUnregistering (RelationEndPointID endPointID)
    {
      // not handled by this listener
    }

    public void RelationEndPointUnloading (RelationEndPoint endPoint)
    {
      // not handled by this listener
    }

    public void DataManagerMarkingObjectDiscarded (ObjectID id)
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

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
