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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Utilities;
using System.Linq;
using Remotion.FunctionalProgramming;

namespace Remotion.Data.DomainObjects.Queries.EagerFetching
{
  /// <summary>
  /// Decorates <see cref="IObjectLoader"/> to include eager fetching when executing <see cref="IObjectLoader.GetOrLoadCollectionQueryResult"/>.
  /// </summary>
  [Serializable]
  public class EagerFetchingObjectLoaderDecorator : IFetchEnabledObjectLoader
  {
    private readonly IFetchEnabledObjectLoader _decoratedObjectLoader;
    private readonly IEagerFetcher _eagerFetcher;

    public EagerFetchingObjectLoaderDecorator (IFetchEnabledObjectLoader decoratedObjectLoader, IEagerFetcher eagerFetcher)
    {
      ArgumentUtility.CheckNotNull ("decoratedObjectLoader", decoratedObjectLoader);
      ArgumentUtility.CheckNotNull ("eagerFetcher", eagerFetcher);

      _decoratedObjectLoader = decoratedObjectLoader;
      _eagerFetcher = eagerFetcher;
    }

    public IObjectLoader DecoratedObjectLoader
    {
      get { return _decoratedObjectLoader; }
    }

    public IEagerFetcher EagerFetcher
    {
      get { return _eagerFetcher; }
    }

    public ILoadedObjectData LoadObject (ObjectID id, bool throwOnNotFound)
    {
      ArgumentUtility.CheckNotNull ("id", id);
      return _decoratedObjectLoader.LoadObject (id, throwOnNotFound);
    }

    public ICollection<ILoadedObjectData> LoadObjects (IEnumerable<ObjectID> idsToBeLoaded, bool throwOnNotFound)
    {
      ArgumentUtility.CheckNotNull ("idsToBeLoaded", idsToBeLoaded);
      return _decoratedObjectLoader.LoadObjects (idsToBeLoaded, throwOnNotFound);
    }

    public ILoadedObjectData GetOrLoadRelatedObject (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      return _decoratedObjectLoader.GetOrLoadRelatedObject (relationEndPointID);
    }

    public ICollection<ILoadedObjectData> GetOrLoadRelatedObjects (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      return _decoratedObjectLoader.GetOrLoadRelatedObjects (relationEndPointID);
    }

    public ICollection<ILoadedObjectData> GetOrLoadCollectionQueryResult (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);
      var queryResult = _decoratedObjectLoader.GetOrLoadCollectionQueryResult (query);

      _eagerFetcher.PerformEagerFetching (queryResult, query.EagerFetchQueries, this);

      return queryResult;
    }

    public ICollection<LoadedObjectDataWithDataSourceData> GetOrLoadFetchQueryResult (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);
      var queryResult = _decoratedObjectLoader.GetOrLoadFetchQueryResult (query);

      var loadedObjectData = queryResult.Select (data => data.LoadedObjectData).ConvertToCollection ();
      _eagerFetcher.PerformEagerFetching (loadedObjectData, query.EagerFetchQueries, this);

      return queryResult;
    }
  }
}