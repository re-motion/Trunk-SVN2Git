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

    public ObjectLoader (ClientTransaction clientTransaction, IPersistenceStrategy persistenceStrategy, IClientTransactionListener eventSink, IEagerFetcher fetcher)
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

    public DomainObject LoadObject (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      var dataContainer = _persistenceStrategy.LoadDataContainer (id);
      RaiseLoadingNotificiations (new ReadOnlyCollection<ObjectID> (new[] { id }));

      InitializeLoadedDataContainer (dataContainer);

      var loadedDomainObject = dataContainer.DomainObject;
      RaiseLoadedNotifications (new ReadOnlyCollection<DomainObject> (new[] { loadedDomainObject }));

      return loadedDomainObject;
    }

    public DomainObject[] LoadObjects (IList<ObjectID> idsToBeLoaded, bool throwOnNotFound)
    {
      ArgumentUtility.CheckNotNull ("idsToBeLoaded", idsToBeLoaded);

      var dataContainers = _persistenceStrategy.LoadDataContainers (idsToBeLoaded, throwOnNotFound);
      RaiseLoadingNotificiations (new ReadOnlyCollection<ObjectID> (idsToBeLoaded));

      foreach (DataContainer dataContainer in dataContainers)
        InitializeLoadedDataContainer (dataContainer);

      // The dataContainers collection contains no items for those ids that couldn't be found
      var loadedDomainObjectsWithoutNulls = ListAdapter.AdaptReadOnly (dataContainers, dc => dc.DomainObject);
      RaiseLoadedNotifications (loadedDomainObjectsWithoutNulls);

      var loadedDomainObjects = (from id in idsToBeLoaded
                                 let dataContainer = dataContainers[id]
                                 select dataContainer != null ? dataContainer.DomainObject : null).ToArray ();
      return loadedDomainObjects;
    }

    public DomainObject LoadRelatedObject (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      if (!relationEndPointID.Definition.IsVirtual)
        throw new ArgumentException ("LoadRelatedObject can only be used with virtual end points.", "relationEndPointID");

      if (relationEndPointID.Definition.Cardinality != CardinalityType.One)
        throw new ArgumentException ("LoadRelatedObject can only be used with one-valued end points.", "relationEndPointID");

      var originatingDataContainer = _clientTransaction.DataManager.GetDataContainerWithLazyLoad (relationEndPointID.ObjectID);
      var relatedDataContainer = _persistenceStrategy.LoadRelatedDataContainer (originatingDataContainer, relationEndPointID);

      // This assertion is only true if single related objects are never loaded lazily; otherwise, a "merge" would be necessary.
      // (Like in MergeLoadedDomainObjects.)
      Assertion.IsTrue (
          relatedDataContainer == null || _clientTransaction.DataManager.DataContainerMap[relatedDataContainer.ID] == null,
          "ObjectEndPoints are created eagerly, so this related object can't have been loaded so far. "
          + "(Otherwise LoadRelatedDataContainer wouldn't have been called.)");

      if (relatedDataContainer != null)
      {
        RaiseLoadingNotificiations (new ReadOnlyCollection<ObjectID> (new[] { relatedDataContainer.ID }));

        InitializeLoadedDataContainer (relatedDataContainer);

        var loadedDomainObjects = new ReadOnlyCollection<DomainObject> (new[] { relatedDataContainer.DomainObject });
        RaiseLoadedNotifications (loadedDomainObjects);

        return relatedDataContainer.DomainObject;
      }
      else
      {
        return null;
      }
    }

    public DomainObject[] LoadRelatedObjects (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

      if (relationEndPointID.Definition.Cardinality != CardinalityType.Many)
        throw new ArgumentException ("LoadRelatedObjects can only be used with many-valued end points.", "relationEndPointID");

      var relatedDataContainers = _persistenceStrategy.LoadRelatedDataContainers (relationEndPointID);
      return MergeQueryResult<DomainObject> (relatedDataContainers);
    }

    public T[] LoadCollectionQueryResult<T> (IQuery query) where T : DomainObject
    {
      ArgumentUtility.CheckNotNull ("query", query);

      var dataContainers = _persistenceStrategy.LoadDataContainersForQuery (query);
      var resultArray = MergeQueryResult<T> (dataContainers);
      
      if (resultArray.Length > 0)
      {
        foreach (var fetchQuery in query.EagerFetchQueries)
          PerformEagerFetching (resultArray, fetchQuery.Key, fetchQuery.Value);
      }

      return resultArray;
    }

    private void PerformEagerFetching (DomainObject[] originalObjects, IRelationEndPointDefinition relationEndPointDefinition, IQuery fetchQuery)
    {
      s_log.DebugFormat (
          "Eager fetching objects for {0} via query {1} ('{2}').", 
          relationEndPointDefinition.PropertyName, 
          fetchQuery.ID, 
          fetchQuery.Statement);

      var fetchedObjects = LoadCollectionQueryResult<DomainObject> (fetchQuery);
      s_log.DebugFormat (
          "The eager fetch query yielded {0} related objects for {1} original objects.", 
          fetchedObjects.Length, 
          originalObjects.Length);

      _fetcher.CorrelateAndRegisterFetchResults(originalObjects, fetchedObjects, relationEndPointDefinition);
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

    private T[] MergeQueryResult<T> (IEnumerable<DataContainer> queryResult) 
        where T : DomainObject
    {
      FindNewDataContainersAndInitialize (queryResult);

      var relatedObjects = from loadedDataContainer in queryResult
                           let maybeDataContainer = 
                              Maybe // loadedDataContainer is null when the query returned null at this position
                                .ForValue (loadedDataContainer)
                                .Select (dc => Assertion.IsNotNull (_clientTransaction.DataManager.DataContainerMap[dc.ID]))
                           let maybeDomainObject = maybeDataContainer.Select (dc => GetCastQueryResultObject<T> (dc.DomainObject))
                           select maybeDomainObject.ValueOrDefault();

      return relatedObjects.ToArray ();
    }

    private void FindNewDataContainersAndInitialize (IEnumerable<DataContainer> dataContainers)
    {
      var newlyLoadedDataContainers = (from dataContainer in dataContainers
                                       where dataContainer != null && _clientTransaction.DataManager.DataContainerMap[dataContainer.ID] == null
                                       select dataContainer).ToList ();

      var newlyLoadedIDs = ListAdapter.AdaptReadOnly (newlyLoadedDataContainers, dc => dc.ID);
      RaiseLoadingNotificiations (newlyLoadedIDs);

      foreach (var dataContainer in newlyLoadedDataContainers)
        InitializeLoadedDataContainer (dataContainer);

      var newlyLoadedDomainObjects = ListAdapter.AdaptReadOnly (newlyLoadedDataContainers, dc => dc.DomainObject);
      RaiseLoadedNotifications (newlyLoadedDomainObjects);
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
            domainObject.GetPublicDomainObjectType ().FullName,
            typeof (T).FullName);

        throw new UnexpectedQueryResultException (message);
      }
    }

    private void InitializeLoadedDataContainer (DataContainer dataContainer)
    {
      var domainObjectReference = _clientTransaction.GetObjectReference (dataContainer.ID);

      dataContainer.SetDomainObject (domainObjectReference);
      _clientTransaction.DataManager.RegisterDataContainer (dataContainer);

      Assertion.IsTrue (dataContainer.DomainObject.ID == dataContainer.ID);
      Assertion.IsTrue (dataContainer.ClientTransaction == _clientTransaction);
      Assertion.IsTrue (_clientTransaction.DataManager.DataContainerMap[dataContainer.ID] == dataContainer);
    }
  }
}