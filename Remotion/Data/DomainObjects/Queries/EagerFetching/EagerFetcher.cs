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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.EagerFetching
{
  /// <summary>
  /// Used by the <see cref="ObjectLoader"/> in order to perform eager fetching of collection queries.
  /// See <see cref="IQuery.EagerFetchQueries"/> for more information on eager fetching.
  /// </summary>
  [Serializable]
  public class EagerFetcher : IEagerFetcher
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (EagerFetcher));

    private readonly IFetchedRelationDataRegistrationAgent _registrationAgent;
    private readonly ILoadedDataContainerProvider _loadedDataContainerProvider;
    private readonly IVirtualEndPointProvider _virtualEndPointProvider;

    public EagerFetcher (
        IFetchedRelationDataRegistrationAgent registrationAgent, 
        ILoadedDataContainerProvider loadedDataContainerProvider,
        IVirtualEndPointProvider virtualEndPointProvider)
    {
      ArgumentUtility.CheckNotNull ("registrationAgent", registrationAgent);
      ArgumentUtility.CheckNotNull ("loadedDataContainerProvider", loadedDataContainerProvider);
      ArgumentUtility.CheckNotNull ("virtualEndPointProvider", virtualEndPointProvider);
      
      _registrationAgent = registrationAgent;
      _loadedDataContainerProvider = loadedDataContainerProvider;
      _virtualEndPointProvider = virtualEndPointProvider;
    }

    public IFetchedRelationDataRegistrationAgent RegistrationAgent
    {
      get { return _registrationAgent; }
    }

    public ILoadedDataContainerProvider LoadedDataContainerProvider
    {
      get { return _loadedDataContainerProvider; }
    }

    public IVirtualEndPointProvider VirtualEndPointProvider
    {
      get { return _virtualEndPointProvider; }
    }

    public void PerformEagerFetching (
        DomainObject[] originalObjects,
        IRelationEndPointDefinition relationEndPointDefinition,
        IQuery fetchQuery,
        IObjectLoader fetchQueryResultLoader)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("originalObjects", originalObjects);
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.CheckNotNull ("fetchQuery", fetchQuery);
      ArgumentUtility.CheckNotNull ("fetchQueryResultLoader", fetchQueryResultLoader);

      s_log.DebugFormat (
          "Eager fetching objects for {0} via query {1} ('{2}').",
          relationEndPointDefinition.PropertyName,
          fetchQuery.ID,
          fetchQuery.Statement);

      // Executing the query will automatically register all loaded DomainObjects. End-points will be explicitly marked complete below.
      var fetchedObjects = fetchQueryResultLoader.GetOrLoadCollectionQueryResult<DomainObject> (fetchQuery);
      s_log.DebugFormat (
          "The eager fetch query yielded {0} related objects for {1} original objects.",
          fetchedObjects.Length,
          originalObjects.Length);

      try
      {
        _registrationAgent.GroupAndRegisterRelatedObjects (
            relationEndPointDefinition,
            originalObjects,
            fetchedObjects,
            _loadedDataContainerProvider,
            _virtualEndPointProvider);
      }
      catch (InvalidOperationException ex)
      {
        throw new UnexpectedQueryResultException ("Eager fetching encountered an unexpected query result: " + ex.Message, ex);
      }
    }
  }
}