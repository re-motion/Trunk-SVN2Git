/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
    private readonly PersistenceConfiguration _storage;
    private readonly MappingLoaderConfiguration _mappingLoader;
    private readonly QueryConfiguration _query;

    public FakeDomainObjectsConfiguration (MappingLoaderConfiguration mappingLoader, PersistenceConfiguration storage, QueryConfiguration query)
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

    public PersistenceConfiguration Storage
    {
      get { return _storage; }
    }

    public QueryConfiguration Query
    {
      get { return _query; }
    }
  }
}
