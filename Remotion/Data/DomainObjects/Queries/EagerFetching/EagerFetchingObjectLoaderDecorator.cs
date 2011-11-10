// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.EagerFetching
{
  /// <summary>
  /// Decorates <see cref="IObjectLoader"/> to include eager fetching when executing <see cref="IObjectLoader.GetOrLoadCollectionQueryResult{T}"/>.
  /// </summary>
  [Serializable]
  public class EagerFetchingObjectLoaderDecorator : IObjectLoader
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (EagerFetchingObjectLoaderDecorator));

    private readonly IObjectLoader _decoratedObjectLoader;
    private readonly IFetchedRelationDataRegistrationAgent _registrationAgent;

    public EagerFetchingObjectLoaderDecorator (IObjectLoader decoratedObjectLoader, IFetchedRelationDataRegistrationAgent registrationAgent)
    {
      ArgumentUtility.CheckNotNull ("decoratedObjectLoader", decoratedObjectLoader);
      ArgumentUtility.CheckNotNull ("registrationAgent", registrationAgent);

      _decoratedObjectLoader = decoratedObjectLoader;
      _registrationAgent = registrationAgent;
    }

    public IObjectLoader DecoratedObjectLoader
    {
      get { return _decoratedObjectLoader; }
    }

    public IFetchedRelationDataRegistrationAgent RegistrationAgent
    {
      get { return _registrationAgent; }
    }

    public DomainObject LoadObject (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);
      return _decoratedObjectLoader.LoadObject (id);
    }

    public DomainObject[] LoadObjects (IEnumerable<ObjectID> idsToBeLoaded, bool throwOnNotFound)
    {
      ArgumentUtility.CheckNotNull ("idsToBeLoaded", idsToBeLoaded);
      return _decoratedObjectLoader.LoadObjects (idsToBeLoaded, throwOnNotFound);
    }

    public DomainObject GetOrLoadRelatedObject (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      return _decoratedObjectLoader.GetOrLoadRelatedObject (relationEndPointID);
    }

    public DomainObject[] GetOrLoadRelatedObjects (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      return _decoratedObjectLoader.GetOrLoadRelatedObjects (relationEndPointID);
    }

    public T[] GetOrLoadCollectionQueryResult<T> (IQuery query) where T : DomainObject
    {
      ArgumentUtility.CheckNotNull ("query", query);
      var queryResult = _decoratedObjectLoader.GetOrLoadCollectionQueryResult<T> (query);

      if (queryResult.Length > 0)
      {
        foreach (var fetchQuery in query.EagerFetchQueries)
        {
          PerformEagerFetching (
              queryResult,
              fetchQuery.Key,
              fetchQuery.Value);
        }
      }

      return queryResult;
    }

    private void PerformEagerFetching (
        DomainObject[] originalObjects,
        IRelationEndPointDefinition relationEndPointDefinition,
        IQuery fetchQuery)
    {
      s_log.DebugFormat (
          "Eager fetching objects for {0} via query {1} ('{2}').",
          relationEndPointDefinition.PropertyName,
          fetchQuery.ID,
          fetchQuery.Statement);

      // Since we are calling GetOrLoadCollectionQueryResult on this (rather than _decoratedObjectLoader), this will trigger nested eager fetching.
      var fetchedObjects = GetOrLoadCollectionQueryResult<DomainObject> (fetchQuery);
      s_log.DebugFormat (
          "The eager fetch query for {0} yielded {1} related objects for {2} original objects.",
          relationEndPointDefinition.PropertyName,
          fetchedObjects.Length,
          originalObjects.Length);

      try
      {
        _registrationAgent.GroupAndRegisterRelatedObjects (
            relationEndPointDefinition,
            originalObjects,
            fetchedObjects);
      }
      catch (InvalidOperationException ex)
      {
        throw new UnexpectedQueryResultException ("Eager fetching encountered an unexpected query result: " + ex.Message, ex);
      }
    }
  }
}