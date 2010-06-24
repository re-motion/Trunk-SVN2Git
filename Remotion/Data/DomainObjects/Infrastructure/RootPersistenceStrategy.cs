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
    private readonly IDataManager _dataManager;

    public RootPersistenceStrategy (Guid transactionID, IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);

      _transactionID = transactionID;
      _dataManager = dataManager;
    }

    public ClientTransaction ParentTransaction
    {
      get { return null; }
    }

    public void PersistData (IEnumerable<DataContainer> changedDataContainers)
    {
      ArgumentUtility.CheckNotNull ("changedDataContainers", changedDataContainers);

      var collection = new DataContainerCollection (changedDataContainers, false);
      if (collection.Count > 0)
      {
        using (var persistenceManager = CreatePersistenceManager())
        {
          persistenceManager.Save (collection);
        }
      }
    }

    public ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      ObjectID newObjectID;
      using (var persistenceManager = CreatePersistenceManager())
      {
        newObjectID = persistenceManager.CreateNewObjectID (classDefinition);
      }
      return newObjectID;
    }

    public DataContainer LoadDataContainer (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      using (var persistenceManager = CreatePersistenceManager())
      {
        return persistenceManager.LoadDataContainer (id);
      }
    }

    public DataContainerCollection LoadDataContainers (ICollection<ObjectID> objectIDs, bool throwOnNotFound)
    {
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

      if (objectIDs.Count == 0)
        return new DataContainerCollection();

      using (var persistenceManager = CreatePersistenceManager())
      {
        return persistenceManager.LoadDataContainers (objectIDs, throwOnNotFound);
      }
    }

    public DataContainer LoadRelatedDataContainer (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

      var originatingDataContainer = _dataManager.GetDataContainerWithLazyLoad (relationEndPointID.ObjectID);

      DataContainer relatedDataContainer;
      using (var persistenceManager = CreatePersistenceManager())
      {
        relatedDataContainer = persistenceManager.LoadRelatedDataContainer (originatingDataContainer, relationEndPointID);

        // This assertion is only true if single related objects are never loaded lazily; otherwise, a "merge" would be necessary.
        // (Like in MergeLoadedDomainObjects.)
        Assertion.IsTrue (
            relatedDataContainer == null || _dataManager.DataContainerMap[relatedDataContainer.ID] == null,
            "ObjectEndPoints are created eagerly, so this related object can't have been loaded so far. "
            + "(Otherwise LoadRelatedDataContainer wouldn't have been called.)");
        return relatedDataContainer;
      }
    }

    public DataContainerCollection LoadRelatedDataContainers (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

      using (var persistenceManager = CreatePersistenceManager())
      {
        return persistenceManager.LoadRelatedDataContainers (relationEndPointID);
      }
    }

    public DataContainer[] LoadDataContainersForQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      if (query.QueryType != QueryType.Collection)
        throw new ArgumentException ("Only collection queries can be used to load data containers.", "query");

      using (var storageProviderManager = CreateStorageProviderManager())
      {
        StorageProvider provider = storageProviderManager.GetMandatory (query.StorageProviderID);
        return provider.ExecuteCollectionQuery (query);
      }
    }

    public object LoadScalarForQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      if (query.QueryType != QueryType.Scalar)
        throw new ArgumentException ("Only scalar queries can be used to load scalar results.", "query");

      using (var storageProviderManager = CreateStorageProviderManager())
      {
        StorageProvider provider = storageProviderManager.GetMandatory (query.StorageProviderID);
        return provider.ExecuteScalarQuery (query);
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