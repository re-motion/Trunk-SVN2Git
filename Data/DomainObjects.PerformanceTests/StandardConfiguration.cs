using System.ComponentModel.Design;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  public class StandardConfiguration
  {
    public const string ConnectionString = "Integrated Security=SSPI;Initial Catalog=PerformanceTestDomain;Data Source=localhost";

    public static void Initialize ()
    {
      ProviderCollection<StorageProviderDefinition> providers = new ProviderCollection<StorageProviderDefinition> ();
      providers.Add (new RdbmsProviderDefinition ("PerformanceTestDomain", typeof (SqlProvider), ConnectionString));
      PersistenceConfiguration persistenceConfiguration = new PersistenceConfiguration (providers, providers["PerformanceTestDomain"]);

      DomainObjectsConfiguration.SetCurrent (new FakeDomainObjectsConfiguration (new MappingLoaderConfiguration (), persistenceConfiguration, new QueryConfiguration()));


      ITypeDiscoveryService typeDiscoveryService = new AssemblyFinderTypeDiscoveryService (
            new AssemblyFinder (ApplicationAssemblyFinderFilter.Instance, typeof (StandardConfiguration).Assembly));
      MappingConfiguration mappingConfiguration = new MappingConfiguration (new MappingReflector (typeDiscoveryService));
      MappingConfiguration.SetCurrent (mappingConfiguration);
    }
  }
}