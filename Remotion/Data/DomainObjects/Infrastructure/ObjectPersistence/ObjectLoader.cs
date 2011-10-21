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
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Utilities;
using Remotion.Collections;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Implements the mechanisms for loading a set of <see cref="DomainObject"/> objects into a <see cref="ClientTransaction"/>.
  /// This class should only be used by <see cref="ClientTransaction"/> and its subclasses.
  /// </summary>
  /// <remarks>
  /// This class signals all load-related events, but it does not signal the <see cref="IClientTransactionListener.FilterQueryResult{T}"/> event.
  /// </remarks>
  [Serializable]
  public class ObjectLoader : IObjectLoader
  {
    private readonly IPersistenceStrategy _persistenceStrategy;
    private readonly ClientTransaction _clientTransaction;
    private readonly IClientTransactionListener _transactionEventSink;
    private readonly IEagerFetcher _eagerFetcher;

    public ObjectLoader (
        ClientTransaction clientTransaction,
        IPersistenceStrategy persistenceStrategy,
        IClientTransactionListener transactionEventSink,
        IEagerFetcher eagerFetcher)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("persistenceStrategy", persistenceStrategy);
      ArgumentUtility.CheckNotNull ("transactionEventSink", transactionEventSink);

      _persistenceStrategy = persistenceStrategy;
      _clientTransaction = clientTransaction;
      _transactionEventSink = transactionEventSink;
      _eagerFetcher = eagerFetcher;
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public IPersistenceStrategy PersistenceStrategy
    {
      get { return _persistenceStrategy; }
    }

    public IClientTransactionListener TransactionEventSink
    {
      get { return _transactionEventSink; }
    }

    public IEagerFetcher EagerFetcher
    {
      get { return _eagerFetcher; }
    }

    public DomainObject LoadObject (ObjectID id, IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("id", id);
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);

      var dataContainer = _persistenceStrategy.LoadDataContainer (id);
      return UnwrapLoadedObject (dataContainer, dataManager);
    }

    public DomainObject[] LoadObjects (IList<ObjectID> idsToBeLoaded, bool throwOnNotFound, IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("idsToBeLoaded", idsToBeLoaded);
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);

      var dataContainers = _persistenceStrategy.LoadDataContainers (idsToBeLoaded, throwOnNotFound);

      // TODO 4428
      //Assertion.IsTrue (dataContainers.Count == idsToBeLoaded.Count, "Persistence strategy must return exactly as many items as requested.");
      //Assertion.DebugAssert (
      //    dataContainers.Select ((dc, i) => dc != null ? dc.ID : idsToBeLoaded[i]).SequenceEqual (idsToBeLoaded), 
      //    "Persistence strategy result must be in the same order as the input IDs (with not found objects replaced with null).");

      var objectDictionary = UnwrapLoadedObjects<DomainObject> (dataContainers, dataManager)
          .Where (domainObject => domainObject != null)
          .ToDictionary (domainObject => domainObject.ID);
      return idsToBeLoaded.Select (id => objectDictionary.GetValueOrDefault (id)).ToArray();
    }

    public DomainObject LoadRelatedObject (RelationEndPointID relationEndPointID, IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);

      if (!relationEndPointID.Definition.IsVirtual)
        throw new ArgumentException ("LoadRelatedObject can only be used with virtual end points.", "relationEndPointID");

      if (relationEndPointID.Definition.Cardinality != CardinalityType.One)
        throw new ArgumentException ("LoadRelatedObject can only be used with one-valued end points.", "relationEndPointID");

      var originatingDataContainer = dataManager.GetDataContainerWithLazyLoad (relationEndPointID.ObjectID);
      var relatedDataContainer = _persistenceStrategy.LoadRelatedDataContainer (originatingDataContainer, relationEndPointID);
      return UnwrapLoadedObject (relatedDataContainer, dataManager);
    }

    public DomainObject[] LoadRelatedObjects (RelationEndPointID relationEndPointID, IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);

      if (relationEndPointID.Definition.Cardinality != CardinalityType.Many)
        throw new ArgumentException ("LoadRelatedObjects can only be used with many-valued end points.", "relationEndPointID");

      var relatedDataContainers = _persistenceStrategy.LoadRelatedDataContainers (relationEndPointID);
      return UnwrapLoadedObjects<DomainObject> (relatedDataContainers, dataManager).ToArray();
    }

    public T[] LoadCollectionQueryResult<T> (IQuery query, IDataManager dataManager) where T : DomainObject
    {
      ArgumentUtility.CheckNotNull ("query", query);
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);

      var dataContainers = _persistenceStrategy.LoadDataContainersForQuery (query);
      var resultArray = UnwrapLoadedObjects<T> (dataContainers, dataManager).ToArray();
      
      if (resultArray.Length > 0)
      {
        foreach (var fetchQuery in query.EagerFetchQueries)
          _eagerFetcher.PerformEagerFetching (resultArray, fetchQuery.Key, fetchQuery.Value, this, dataManager);
      }

      return resultArray;
    }

    private IEnumerable<T> UnwrapLoadedObjects<T> (IEnumerable<DataContainer> queryResult, IDataManager dataManager) 
        where T : DomainObject
    {
      var registrar = new LoadedObjectRegistrationAgent (_clientTransaction, _transactionEventSink, dataManager);
      var loadedObjects = queryResult.Select (dc => GetLoadedObject (dc, dataManager));

      return registrar.GetDomainObjects (loadedObjects).Select (ConvertLoadedDomainObject<T>);
    }

    private T ConvertLoadedDomainObject<T> (DomainObject domainObject) where T : DomainObject
    {
      if (domainObject == null || domainObject is T)
        return (T) domainObject;
      else
      {
        var message = string.Format (
            "The query returned an object of type '{0}', but a query result of type '{1}' was expected.",
            domainObject.GetPublicDomainObjectType(),
            typeof (T));
        throw new UnexpectedQueryResultException (message);
      }
    }

    private DomainObject UnwrapLoadedObject (DataContainer dataContainer, IDataManager dataManager)
    {
      var registrar = new LoadedObjectRegistrationAgent (_clientTransaction, _transactionEventSink, dataManager);
      var loadedObject = GetLoadedObject (dataContainer, dataManager);

      return registrar.GetDomainObject (loadedObject);
    }

    private ILoadedObject GetLoadedObject (DataContainer dataContainer, IDataManager dataManager)
    {
      if (dataContainer == null)
        return new NullLoadedObject ();
      else
      {
        var existingDataContainer = dataManager.GetDataContainerWithoutLoading (dataContainer.ID);
        if (existingDataContainer != null)
          return new AlreadyExistingLoadedObject (existingDataContainer);
        else
          return new FreshlyLoadedObject (dataContainer);
      }
    }
  }
}