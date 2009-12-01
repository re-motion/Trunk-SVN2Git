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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Transport
{
  [Serializable]
  public class TransportTransactionListener : IClientTransactionListener
  {
    [NonSerialized]
    private readonly DomainObjectTransporter _transporter;

    public TransportTransactionListener (DomainObjectTransporter transporter)
    {
      ArgumentUtility.CheckNotNull ("transporter", transporter);
      _transporter = transporter;
    }

    public void SubTransactionCreating ()
    {
      
    }

    public void SubTransactionCreated (ClientTransaction subTransaction)
    {
      
    }

    public void NewObjectCreating (Type type, DomainObject instance)
    {
      
    }

    public void ObjectLoading (ObjectID id)
    {
      
    }

    public void ObjectsLoaded (ReadOnlyCollection<DomainObject> domainObjects)
    {
      
    }

    public void ObjectGotID (DomainObject instance, ObjectID id)
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
      if (_transporter == null)
        throw new InvalidOperationException ("Cannot use the transported transaction for changing properties after it has been deserialized.");

      if (!_transporter.IsLoaded (dataContainer.ID))
      {
        string message = string.Format ("Object '{0}' cannot be modified for transportation because it hasn't been loaded yet. Load it before "
            + "manipulating it.", dataContainer.ID);
        throw new InvalidOperationException(message);
      }
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

    public void RelationRead (DomainObject domainObject, string propertyName, ReadOnlyCollection<DomainObject> relatedObjects, ValueAccess valueAccess)
    {
      
    }

    public void RelationChanging (DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      
    }

    public void RelationChanged (DomainObject domainObject, string propertyName)
    {
      
    }

    public QueryResult<T> FilterQueryResult<T> (QueryResult<T> queryResult) where T : DomainObject
    {
      return queryResult;
    }

    public void TransactionCommitting (ReadOnlyCollection<DomainObject> domainObjects)
    {
      throw new InvalidOperationException ("The transport transaction cannot be committed.");
    }

    public void TransactionCommitted (ReadOnlyCollection<DomainObject> domainObjects)
    {
      
    }

    public void TransactionRollingBack (ReadOnlyCollection<DomainObject> domainObjects)
    {
      throw new InvalidOperationException ("The transport transaction cannot be rolled back.");
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

    public void RelationEndPointMapPerformingDelete (RelationEndPointID[] endPointIDs)
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
