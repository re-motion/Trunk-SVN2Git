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

    public FetchEnabledObjectLoader (
        IFetchEnabledPersistenceStrategy persistenceStrategy,
        ILoadedObjectDataRegistrationAgent loadedObjectDataRegistrationAgent,
        ILoadedObjectDataProvider loadedObjectDataProvider)
        : base (persistenceStrategy, loadedObjectDataRegistrationAgent, loadedObjectDataProvider)
    {
      ArgumentUtility.CheckNotNull ("persistenceStrategy", persistenceStrategy);

      _persistenceStrategy = persistenceStrategy;
    }

    public new IFetchEnabledPersistenceStrategy PersistenceStrategy
    {
      get { return _persistenceStrategy; }
    }

    public ICollection<LoadedObjectDataWithDataSourceData> GetOrLoadFetchQueryResult (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      var loadedObjectData = _persistenceStrategy.ExecuteFetchQuery (query, LoadedObjectDataProvider).ConvertToCollection();
      LoadedObjectDataRegistrationAgent.RegisterIfRequired (loadedObjectData.Select (data => data.LoadedObjectData), true);
      return loadedObjectData;
    }
  }
}