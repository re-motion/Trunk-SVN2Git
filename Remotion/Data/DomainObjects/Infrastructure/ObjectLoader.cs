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
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.FunctionalProgramming;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
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
    private static readonly ILog s_log = LogManager.GetLogger (typeof (ObjectLoader));

    private readonly IPersistenceStrategy _persistenceStrategy;
    private readonly ClientTransaction _clientTransaction;
    private readonly IClientTransactionListener _eventSink;
    private readonly IEagerFetcher _fetcher;

    public ObjectLoader (
        ClientTransaction clientTransaction,
        IPersistenceStrategy persistenceStrategy,
        IClientTransactionListener eventSink,
        IEagerFetcher fetcher)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("persistenceStrategy", persistenceStrategy);
      ArgumentUtility.CheckNotNull ("eventSink", eventSink);

      _persistenceStrategy = persistenceStrategy;
      _clientTransaction = clientTransaction;
      _eventSink = eventSink;
      _fetcher = fetcher;
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public DomainObject LoadObject (ObjectID id, IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("id", id);
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);

      var dataContainer = _persistenceStrategy.LoadDataContainer (id);
      return LoadObject (dataContainer, dataManager);
    }

    public DomainObject[] LoadObjects (IList<ObjectID> idsToBeLoaded, bool throwOnNotFound, IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("idsToBeLoaded", idsToBeLoaded);
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);

      var dataContainers = _persistenceStrategy.LoadDataContainers (idsToBeLoaded, throwOnNotFound);
      LoadObjects (dataContainers, dataManager);

      var loadedDomainObjects = (from id in idsToBeLoaded
                                 let dataContainer = dataContainers[id]
                                 select dataContainer != null ? dataContainer.DomainObject : null).ToArray ();
      return loadedDomainObjects;
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

      if (relatedDataContainer != null)
      {
        CheckRelatedDataContainerForConflicts (relationEndPointID, relatedDataContainer, dataManager);
        return LoadObject (relatedDataContainer, dataManager);
      }
      else
      {
        return null;
      }
    }

    public DomainObject[] LoadRelatedObjects (RelationEndPointID relationEndPointID, IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);

      if (relationEndPointID.Definition.Cardinality != CardinalityType.Many)
        throw new ArgumentException ("LoadRelatedObjects can only be used with many-valued end points.", "relationEndPointID");

      var relatedDataContainers = _persistenceStrategy.LoadRelatedDataContainers (relationEndPointID);
      return MergeQueryResult<DomainObject> (relatedDataContainers, dataManager);
    }

    public T[] LoadCollectionQueryResult<T> (IQuery query, IDataManager dataManager) where T : DomainObject
    {
      ArgumentUtility.CheckNotNull ("query", query);
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);

      var dataContainers = _persistenceStrategy.LoadDataContainersForQuery (query);
      var resultArray = MergeQueryResult<T> (dataContainers, dataManager);
      
      if (resultArray.Length > 0)
      {
        foreach (var fetchQuery in query.EagerFetchQueries)
          _fetcher.PerformEagerFetching (resultArray, fetchQuery.Key, fetchQuery.Value, this, dataManager);
      }

      return resultArray;
    }

    private void RaiseLoadingNotificiations (ReadOnlyCollection<ObjectID> objectIDs)
    {
      if (objectIDs.Count != 0)
        _eventSink.ObjectsLoading (_clientTransaction, objectIDs);
    }

    private void RaiseLoadedNotifications (ReadOnlyCollection<DomainObject> loadedObjects)
    {
      if (loadedObjects.Count != 0)
      {
        using (_clientTransaction.EnterNonDiscardingScope ())
        {
          foreach (var loadedDomainObject in loadedObjects)
            loadedDomainObject.OnLoaded();

          _eventSink.ObjectsLoaded (_clientTransaction, loadedObjects);

          _clientTransaction.OnLoaded (new ClientTransactionEventArgs (loadedObjects));
        }
      }
    }

    private T[] MergeQueryResult<T> (IEnumerable<DataContainer> queryResult, IDataManager dataManager) 
        where T : DomainObject
    {
      FindNewDataContainersAndInitialize (queryResult, dataManager);

      var relatedObjects = from loadedDataContainer in queryResult
                           let maybeDataContainer = 
                              Maybe // loadedDataContainer is null when the query returned null at this position
                                .ForValue (loadedDataContainer)
                                .Select (dc => Assertion.IsNotNull (dataManager.DataContainerMap[dc.ID]))
                           let maybeDomainObject = maybeDataContainer.Select (dc => GetCastQueryResultObject<T> (dc.DomainObject))
                           select maybeDomainObject.ValueOrDefault();

      return relatedObjects.ToArray ();
    }

    private void FindNewDataContainersAndInitialize (IEnumerable<DataContainer> dataContainers, IDataManager dataManager)
    {
      var newlyLoadedDataContainers = (from dataContainer in dataContainers
                                       where dataContainer != null && dataManager.DataContainerMap[dataContainer.ID] == null
                                       select dataContainer).ToList ();

      LoadObjects (newlyLoadedDataContainers, dataManager);
    }

    private T GetCastQueryResultObject<T> (DomainObject domainObject) where T : DomainObject
    {
      var castDomainObject = domainObject as T;
      if (castDomainObject != null)
        return castDomainObject;
      else
      {
        string message = string.Format (
            "The query returned an object of type '{0}', but a query result of type '{1}' was expected.",
            domainObject.ID.ClassDefinition.ClassType.FullName,
            typeof (T).FullName);

        throw new UnexpectedQueryResultException (message);
      }
    }

    private DomainObject LoadObject (DataContainer dataContainer, IDataManager dataManager)
    {
      RaiseLoadingNotificiations (new ReadOnlyCollection<ObjectID> (new[] { dataContainer.ID }));

      var loadedDomainObject = InitializeLoadedDataContainer (dataContainer, dataManager);
      RaiseLoadedNotifications (new ReadOnlyCollection<DomainObject> (new[] { loadedDomainObject }));

      return loadedDomainObject;
    }

    private void LoadObjects (IList<DataContainer> dataContainers, IDataManager dataManager)
    {
      var newlyLoadedIDs = ListAdapter.AdaptReadOnly (dataContainers, dc => dc.ID);
      RaiseLoadingNotificiations (newlyLoadedIDs);

      var loadedDomainObjects = new List<DomainObject> ();
      try
      {
        // Leave forech loop (instead of LINQ query + AddRange) for readability
        // ReSharper disable LoopCanBeConvertedToQuery
        foreach (var dataContainer in dataContainers)
          loadedDomainObjects.Add (InitializeLoadedDataContainer (dataContainer, dataManager));
        // ReSharper restore LoopCanBeConvertedToQuery
      }
      finally
      {
        RaiseLoadedNotifications (loadedDomainObjects.AsReadOnly ());
      }
    }

    private DomainObject InitializeLoadedDataContainer (DataContainer dataContainer, IDataManager dataManager)
    {
      var domainObjectReference = _clientTransaction.GetObjectReference (dataContainer.ID);

      dataContainer.SetDomainObject (domainObjectReference);
      try
      {
        dataManager.RegisterDataContainer (dataContainer);
      }
      catch (InvalidOperationException ex)
      {
        if (s_log.IsWarnEnabled)
          s_log.Warn (ex.Message);

        throw new LoadConflictException (ex.Message, ex);
      }

      Assertion.IsTrue (dataContainer.DomainObject.ID == dataContainer.ID);
      Assertion.IsTrue (dataContainer.ClientTransaction == _clientTransaction);
      Assertion.IsTrue (dataManager.DataContainerMap[dataContainer.ID] == dataContainer);

      return domainObjectReference;
    }

    private void CheckRelatedDataContainerForConflicts (RelationEndPointID relationEndPointID, DataContainer relatedDataContainer, IDataManager dataManager)
    {
      var existingDataContainer = dataManager.GetDataContainerWithoutLoading (relatedDataContainer.ID);
      if (existingDataContainer != null)
      {
        var existingOppositeEndPointID = RelationEndPointID.Create(existingDataContainer.ID, relationEndPointID.Definition.GetOppositeEndPointDefinition ());
        var existingBackPointer = dataManager.RelationEndPointMap.GetRelatedObject (existingOppositeEndPointID, true);

        if (s_log.IsWarnEnabled)
        {
          s_log.WarnFormat ("Cannot load the related '{0}' of '{1}': The database returned related object '{2}', "
            +"but that object already exists in the current ClientTransaction (and points to a different object '{3}')",
            relationEndPointID.Definition.PropertyName,
            relationEndPointID.ObjectID,
            relatedDataContainer.ID,
            existingBackPointer != null ? existingBackPointer.ID.ToString () : "null");
        }

        var message = string.Format (
            "Cannot load the related '{0}' of '{1}': The database returned related object '{2}', but that object already exists in the current "
            + "ClientTransaction (and points to a different object '{3}').",
            relationEndPointID.Definition.PropertyName,
            relationEndPointID.ObjectID,
            relatedDataContainer.ID,
            existingBackPointer != null ? existingBackPointer.ID.ToString () : "null");
        throw new LoadConflictException (message);
      }
    }
  }
}