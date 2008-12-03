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
using System.Configuration;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;

namespace Remotion.Data.DomainObjects.Configuration
{
  /// <summary>
  /// <see cref="ConfigurationSectionGroup"/> for grouping the <see cref="ConfigurationSection"/> in the <b>Remotion.Data.DomainObjects</b> namespace.
  /// </summary>
  public sealed class DomainObjectsConfiguration: ConfigurationSectionGroup, IDomainObjectsConfiguration
  {
    private const string MappingLoaderPropertyName = "mapping";
    private const string StoragePropertyName = "storage";
    private const string QueryPropertyName = "query";

    private static readonly DoubleCheckedLockingContainer<IDomainObjectsConfiguration> s_current =
        new DoubleCheckedLockingContainer<IDomainObjectsConfiguration> (delegate { return new DomainObjectsConfiguration(); });

    public static IDomainObjectsConfiguration Current
    {
      get { return s_current.Value; }
    }

    public static void SetCurrent (IDomainObjectsConfiguration configuration)
    {
      s_current.Value = configuration;
    }

    public DomainObjectsConfiguration()
    {
      _mappingLoaderConfiguration =
          new DoubleCheckedLockingContainer<MappingLoaderConfiguration> (delegate { return GetMappingLoaderConfiguration(); });

      _persistenceConfiguration =
          new DoubleCheckedLockingContainer<StorageConfiguration> (delegate { return GetPersistenceConfiguration(); });

      _queryConfiguration =
          new DoubleCheckedLockingContainer<QueryConfiguration> (delegate { return GetQueryConfiguration (); });
    }

    private readonly DoubleCheckedLockingContainer<MappingLoaderConfiguration> _mappingLoaderConfiguration;
    private readonly DoubleCheckedLockingContainer<StorageConfiguration> _persistenceConfiguration;
    private readonly DoubleCheckedLockingContainer<QueryConfiguration> _queryConfiguration;

    [ConfigurationProperty (MappingLoaderPropertyName)]
    public MappingLoaderConfiguration MappingLoader
    {
      get { return _mappingLoaderConfiguration.Value; }
    }

    [ConfigurationProperty (StoragePropertyName)]
    public StorageConfiguration Storage
    {
      get { return _persistenceConfiguration.Value; }
    }

    [ConfigurationProperty (QueryPropertyName)]
    public QueryConfiguration Query
    {
      get { return _queryConfiguration.Value; }
    }

    private MappingLoaderConfiguration GetMappingLoaderConfiguration()
    {
      return
          (MappingLoaderConfiguration) ConfigurationWrapper.Current.GetSection (ConfigKey + "/" + MappingLoaderPropertyName, false)
          ?? new MappingLoaderConfiguration();
    }

    private StorageConfiguration GetPersistenceConfiguration()
    {
      return
          (StorageConfiguration) ConfigurationWrapper.Current.GetSection (ConfigKey + "/" + StoragePropertyName, false) 
          ?? new StorageConfiguration();
    }

    private QueryConfiguration GetQueryConfiguration ()
    {
      return
          (QueryConfiguration) ConfigurationWrapper.Current.GetSection (ConfigKey + "/" + QueryPropertyName, false)
          ?? new QueryConfiguration ();
    }

    private string ConfigKey
    {
      get { return string.IsNullOrEmpty (SectionGroupName) ? "remotion.data.domainObjects" : SectionGroupName; }
    }
  }
}
