using System;
using System.ComponentModel.Design;
using System.Reflection;
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.RdbmsTools.UnitTests
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private const string c_firstStorageProviderConnectionString = "Integrated Security=SSPI;Initial Catalog=RdbmsToolsUnitTests1;Data Source=localhost;";
    private const string c_secondStorageProviderConnectionString = "Integrated Security=SSPI;Initial Catalog=RdbmsToolsUnitTests2;Data Source=localhost;";
    private const string c_internalStorageProviderConnectionString = "Integrated Security=SSPI;Initial Catalog=RdbmsToolsUnitTestsInternal;Data Source=localhost;";
    
    private const string c_firstStorageProvider = "FirstStorageProvider";
    private const string c_secondStorageProvider = "SecondStorageProvider";
    private const string c_internalStorageProvider = "Internal";

    [SetUp]
    public void SetUp ()
    {
      ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = new ProviderCollection<StorageProviderDefinition>();
      storageProviderDefinitionCollection.Add (
          new RdbmsProviderDefinition (c_firstStorageProvider, typeof (SqlProvider), c_firstStorageProviderConnectionString));
      storageProviderDefinitionCollection.Add (
          new RdbmsProviderDefinition (c_secondStorageProvider, typeof (SqlProvider), c_secondStorageProviderConnectionString));
      storageProviderDefinitionCollection.Add (
          new RdbmsProviderDefinition (c_internalStorageProvider, typeof (SqlProvider), c_internalStorageProviderConnectionString));

      PersistenceConfiguration persistenceConfiguration =
          new PersistenceConfiguration (storageProviderDefinitionCollection, storageProviderDefinitionCollection[c_firstStorageProvider]);

      persistenceConfiguration.StorageGroups.Add (new StorageGroupElement (new FirstStorageGroupAttribute(), c_firstStorageProvider));
      persistenceConfiguration.StorageGroups.Add (new StorageGroupElement (new SecondStorageGroupAttribute(), c_secondStorageProvider));
      persistenceConfiguration.StorageGroups.Add (new StorageGroupElement (new InternalStorageGroupAttribute (), c_internalStorageProvider));

      DomainObjectsConfiguration.SetCurrent (new FakeDomainObjectsConfiguration (new MappingLoaderConfiguration(), persistenceConfiguration,
          new QueryConfiguration()));

      ITypeDiscoveryService typeDiscoveryService = new AssemblyFinderTypeDiscoveryService (
            new AssemblyFinder (ApplicationAssemblyFinderFilter.Instance, typeof (Ceo).Assembly));
      MappingConfiguration.SetCurrent (new MappingConfiguration (new MappingReflector (typeDiscoveryService)));
    }
  }
}