// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.FunctionalProgramming;
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
    private readonly ILoadedObjectDataRegistrationAgent _loadedObjectDataRegistrationAgent;
    private readonly IDataContainerLifetimeManager _dataContainerLifetimeManager;
    private readonly ILoadedObjectDataProvider _loadedObjectDataProvider;

    public ObjectLoader (
        IPersistenceStrategy persistenceStrategy,
        ILoadedObjectDataRegistrationAgent loadedObjectDataRegistrationAgent,
        IDataContainerLifetimeManager dataContainerLifetimeManager,
        ILoadedObjectDataProvider loadedObjectDataProvider)
    {
      ArgumentUtility.CheckNotNull ("persistenceStrategy", persistenceStrategy);
      ArgumentUtility.CheckNotNull ("loadedObjectDataRegistrationAgent", loadedObjectDataRegistrationAgent);
      ArgumentUtility.CheckNotNull ("dataContainerLifetimeManager", dataContainerLifetimeManager);
      ArgumentUtility.CheckNotNull ("loadedObjectDataProvider", loadedObjectDataProvider);

      _persistenceStrategy = persistenceStrategy;
      _loadedObjectDataRegistrationAgent = loadedObjectDataRegistrationAgent;
      _dataContainerLifetimeManager = dataContainerLifetimeManager;
      _loadedObjectDataProvider = loadedObjectDataProvider;
    }

    public IPersistenceStrategy PersistenceStrategy
    {
      get { return _persistenceStrategy; }
    }

    public ILoadedObjectDataRegistrationAgent LoadedObjectDataRegistrationAgent
    {
      get { return _loadedObjectDataRegistrationAgent; }
    }

    public IDataContainerLifetimeManager DataContainerLifetimeManager
    {
      get { return _dataContainerLifetimeManager; }
    }

    public ILoadedObjectDataProvider LoadedObjectDataProvider
    {
      get { return _loadedObjectDataProvider; }
    }

    public DomainObject LoadObject (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      var loadedObjectData = _persistenceStrategy.LoadObjectData (id);
      return _loadedObjectDataRegistrationAgent.RegisterIfRequired (loadedObjectData, _dataContainerLifetimeManager);
    }

    public DomainObject[] LoadObjects (IEnumerable<ObjectID> idsToBeLoaded, bool throwOnNotFound)
    {
      ArgumentUtility.CheckNotNull ("idsToBeLoaded", idsToBeLoaded);

      var idsToBeLoadedAsCollection = idsToBeLoaded.ConvertToCollection();
      var loadedObjectData = _persistenceStrategy.LoadObjectData (idsToBeLoadedAsCollection, throwOnNotFound);

      // TODO 4509
      //Assertion.IsTrue (dataContainers.Count == idsToBeLoaded.Count, "Persistence strategy must return exactly as many items as requested.");
      //Assertion.DebugAssert (
      //    dataContainers.Select ((dc, i) => dc != null ? dc.ID : idsToBeLoaded[i]).SequenceEqual (idsToBeLoaded), 
      //    "Persistence strategy result must be in the same order as the input IDs (with not found objects replaced with null).");

      var objectDictionary = _loadedObjectDataRegistrationAgent
          .RegisterIfRequired (loadedObjectData, _dataContainerLifetimeManager)
          .Select (ConvertLoadedDomainObject<DomainObject>)
          .Where (domainObject => domainObject != null)
          .ToDictionary (domainObject => domainObject.ID);
      return idsToBeLoadedAsCollection.Select (id => objectDictionary.GetValueOrDefault (id)).ToArray ();
    }

    public DomainObject GetOrLoadRelatedObject (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      
      if (!relationEndPointID.Definition.IsVirtual)
        throw new ArgumentException ("GetOrLoadRelatedObject can only be used with virtual end points.", "relationEndPointID");

      if (relationEndPointID.Definition.Cardinality != CardinalityType.One)
        throw new ArgumentException ("GetOrLoadRelatedObject can only be used with one-valued end points.", "relationEndPointID");

      var loadedObjectData = _persistenceStrategy.ResolveObjectRelationData (relationEndPointID, _loadedObjectDataProvider);
      return _loadedObjectDataRegistrationAgent.RegisterIfRequired (loadedObjectData, _dataContainerLifetimeManager);
    }

    public DomainObject[] GetOrLoadRelatedObjects (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

      if (relationEndPointID.Definition.Cardinality != CardinalityType.Many)
        throw new ArgumentException ("GetOrLoadRelatedObjects can only be used with many-valued end points.", "relationEndPointID");

      var loadedObjects = _persistenceStrategy.ResolveCollectionRelationData (relationEndPointID, _loadedObjectDataProvider);
      return _loadedObjectDataRegistrationAgent
          .RegisterIfRequired (loadedObjects, _dataContainerLifetimeManager)
          .Select (ConvertLoadedDomainObject<DomainObject>)
          .ToArray();
    }

    public T[] GetOrLoadCollectionQueryResult<T> (IQuery query) where T: DomainObject
    {
      ArgumentUtility.CheckNotNull ("query", query);

      var loadedObjectData = _persistenceStrategy.ExecuteCollectionQuery (query, _loadedObjectDataProvider);
      return _loadedObjectDataRegistrationAgent
          .RegisterIfRequired (loadedObjectData, _dataContainerLifetimeManager)
          .Select (ConvertLoadedDomainObject<T>)
          .ToArray ();
    }

    private T ConvertLoadedDomainObject<T> (DomainObject domainObject) where T : DomainObject
    {
      if (domainObject == null || domainObject is T)
        return (T) domainObject;
      else
      {
        var message = string.Format (
            "The query returned an object of type '{0}', but a query result of type '{1}' was expected.",
            domainObject.GetPublicDomainObjectType (),
            typeof (T));
        throw new UnexpectedQueryResultException (message);
      }
    }
  }
}