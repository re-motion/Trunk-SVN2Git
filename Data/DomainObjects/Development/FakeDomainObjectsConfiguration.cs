// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Development
{
  /// <summary>
  /// Fake implementation of the <see cref="IDomainObjectsConfiguration"/> interface. Use this class for programmatically setting up the configuration 
  /// in unit test scenarios.
  /// </summary>
  public class FakeDomainObjectsConfiguration: IDomainObjectsConfiguration
  {
    private readonly StorageConfiguration _storage;
    private readonly MappingLoaderConfiguration _mappingLoader;
    private readonly QueryConfiguration _query;

    public FakeDomainObjectsConfiguration (MappingLoaderConfiguration mappingLoader, StorageConfiguration storage, QueryConfiguration query)
    {
      ArgumentUtility.CheckNotNull ("mappingLoader", mappingLoader);
      ArgumentUtility.CheckNotNull ("storage", storage);
      ArgumentUtility.CheckNotNull ("query", query);

      _mappingLoader = mappingLoader;
      _storage = storage;
      _query = query;
    }

    public MappingLoaderConfiguration MappingLoader
    {
      get { return _mappingLoader; }
    }

    public StorageConfiguration Storage
    {
      get { return _storage; }
    }

    public QueryConfiguration Query
    {
      get { return _query; }
    }
  }
}
