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
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Represents a top-level <see cref="ClientTransaction"/>, which does not have a parent transaction.
  /// </summary>
  [Serializable]
  public class RootPersistenceStrategy : IPersistenceStrategy
  {
    private readonly Guid _transactionID;

    public RootPersistenceStrategy (Guid transactionID)
    {
      _transactionID = transactionID;
    }

    public Guid TransactionID
    {
      get { return _transactionID; }
    }

    public virtual ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      using (var persistenceManager = CreatePersistenceManager())
      {
        return persistenceManager.CreateNewObjectID (classDefinition);
      }
    }

    public virtual DataContainer LoadDataContainer (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      using (var persistenceManager = CreatePersistenceManager())
      {
        return persistenceManager.LoadDataContainer (id);
      }
    }

    public virtual DataContainerCollection LoadDataContainers (ICollection<ObjectID> objectIDs, bool throwOnNotFound)
    {
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

      if (objectIDs.Count == 0)
        return new DataContainerCollection();

      using (var persistenceManager = CreatePersistenceManager())
      {
        return persistenceManager.LoadDataContainers (objectIDs, throwOnNotFound);
      }
    }

    public virtual DataContainer LoadRelatedDataContainer (DataContainer originatingDataContainer, RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

      using (var persistenceManager = CreatePersistenceManager())
      {
        return persistenceManager.LoadRelatedDataContainer (originatingDataContainer, relationEndPointID);
      }
    }

    public virtual DataContainerCollection LoadRelatedDataContainers (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

      using (var persistenceManager = CreatePersistenceManager())
      {
        return persistenceManager.LoadRelatedDataContainers (relationEndPointID);
      }
    }

    public virtual DataContainer[] LoadDataContainersForQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      if (query.QueryType != QueryType.Collection)
        throw new ArgumentException ("Only collection queries can be used to load data containers.", "query");

      using (var storageProviderManager = CreateStorageProviderManager())
      {
        StorageProvider provider = storageProviderManager.GetMandatory (query.StorageProviderDefinition.Name);
        return provider.ExecuteCollectionQuery (query).ToArray();
      }
    }

    public virtual object LoadScalarForQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      if (query.QueryType != QueryType.Scalar)
        throw new ArgumentException ("Only scalar queries can be used to load scalar results.", "query");

      using (var storageProviderManager = CreateStorageProviderManager())
      {
        StorageProvider provider = storageProviderManager.GetMandatory (query.StorageProviderDefinition.Name);
        return provider.ExecuteScalarQuery (query);
      }
    }

    public virtual void PersistData (IEnumerable<PersistableData> data)
    {
      ArgumentUtility.CheckNotNull ("data", data);

      // Filter out those items whose state is only Changed due to relation changes - we don't persist those
      var dataContainers = data.Select (item => item.DataContainer).Where (dc => dc.State != StateType.Unchanged);
      
      var collection = new DataContainerCollection (dataContainers, false);
      if (collection.Count > 0)
      {
        using (var persistenceManager = CreatePersistenceManager ())
        {
          persistenceManager.Save (collection);
        }
      }
    }

    private PersistenceManager CreatePersistenceManager ()
    {
      return new PersistenceManager (CreatePersistenceListener());
    }

    private StorageProviderManager CreateStorageProviderManager ()
    {
      return new StorageProviderManager (CreatePersistenceListener ());
    }

    private IPersistenceListener CreatePersistenceListener ()
    {
      var listenerFactories = SafeServiceLocator.Current.GetAllInstances<IPersistenceListenerFactory>();
      return new CompoundPersistenceListener (listenerFactories.Select (f => f.CreatePersistenceListener (_transactionID)));
    }
  }
}