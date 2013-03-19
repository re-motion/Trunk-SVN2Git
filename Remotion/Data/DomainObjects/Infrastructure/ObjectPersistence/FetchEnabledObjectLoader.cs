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
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Extends an <see cref="IObjectLoader"/> with the ability to execute an eager fetch query.
  /// </summary>
  [Serializable]
  public class FetchEnabledObjectLoader : ObjectLoader, IFetchEnabledObjectLoader
  {
    private readonly IFetchEnabledPersistenceStrategy _persistenceStrategy;
    private readonly IEagerFetcher _eagerFetcher;

    public FetchEnabledObjectLoader (
        IFetchEnabledPersistenceStrategy persistenceStrategy,
        ILoadedObjectDataRegistrationAgent loadedObjectDataRegistrationAgent,
        ILoadedObjectDataProvider loadedObjectDataProvider,
        IEagerFetcher eagerFetcher)
        : base (persistenceStrategy, loadedObjectDataRegistrationAgent, loadedObjectDataProvider)
    {
      ArgumentUtility.CheckNotNull ("persistenceStrategy", persistenceStrategy);
      ArgumentUtility.CheckNotNull ("eagerFetcher", eagerFetcher);
      
      _persistenceStrategy = persistenceStrategy;
      _eagerFetcher = eagerFetcher;
    }

    public new IFetchEnabledPersistenceStrategy PersistenceStrategy
    {
      get { return _persistenceStrategy; }
    }

    public IEagerFetcher EagerFetcher
    {
      get { return _eagerFetcher; }
    }

    public override ICollection<ILoadedObjectData> GetOrLoadCollectionQueryResult (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      // TODO 5397: Create empty loaded objects context - this will track the data of objects loaded during this operation.

      // TODO 5397: This operation must not cause any loaded events to be raised. While object loading usually involves the full registration process,
      // in this case, only the first half (ObjectsLoading events and associating the DataContainers with DomainObjects) must be performed. Keep track 
      // of the second half in the context. This is probably most easily solved by overriding the ObjectLoader.GetOrLoadCollectionQueryResult method.
      var queryResult = base.GetOrLoadCollectionQueryResult (query);

      // TODO 5397: Pass in context.
      _eagerFetcher.PerformEagerFetching (queryResult, query.EagerFetchQueries, this);

      // TODO 5397: Finish second half of registration for all objects in the context.

      return queryResult;
    }

    // TODO 5397: Add context parameter.
    public ICollection<LoadedObjectDataWithDataSourceData> GetOrLoadFetchQueryResult (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);
      // TODO 5397: Unify the GetOrLoadFetchQueryResult implementation with this operation. Only perform first half of register operation. Add second 
      // half to context.

      var loadedObjectDataWithSource = _persistenceStrategy.ExecuteFetchQuery (query, LoadedObjectDataProvider).ConvertToCollection();
      var loadedObjectData = loadedObjectDataWithSource.Select (data => data.LoadedObjectData).ConvertToCollection ();

      LoadedObjectDataRegistrationAgent.RegisterIfRequired (loadedObjectData, true);

      // TODO 5397: Pass in context.
      _eagerFetcher.PerformEagerFetching (loadedObjectData, query.EagerFetchQueries, this);

      return loadedObjectDataWithSource;
    }
  }
}