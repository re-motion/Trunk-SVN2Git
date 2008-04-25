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
          new DoubleCheckedLockingContainer<PersistenceConfiguration> (delegate { return GetPersistenceConfiguration(); });

      _queryConfiguration =
          new DoubleCheckedLockingContainer<QueryConfiguration> (delegate { return GetQueryConfiguration (); });
    }

    private readonly DoubleCheckedLockingContainer<MappingLoaderConfiguration> _mappingLoaderConfiguration;
    private readonly DoubleCheckedLockingContainer<PersistenceConfiguration> _persistenceConfiguration;
    private readonly DoubleCheckedLockingContainer<QueryConfiguration> _queryConfiguration;

    [ConfigurationProperty (MappingLoaderPropertyName)]
    public MappingLoaderConfiguration MappingLoader
    {
      get { return _mappingLoaderConfiguration.Value; }
    }

    [ConfigurationProperty (StoragePropertyName)]
    public PersistenceConfiguration Storage
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

    private PersistenceConfiguration GetPersistenceConfiguration()
    {
      return
          (PersistenceConfiguration) ConfigurationWrapper.Current.GetSection (ConfigKey + "/" + StoragePropertyName, false) 
          ?? new PersistenceConfiguration();
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