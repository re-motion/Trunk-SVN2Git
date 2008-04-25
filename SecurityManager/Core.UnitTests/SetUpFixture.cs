using System;
using System.ComponentModel.Design;
using System.Data.SqlClient;
using System.IO;
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
using Remotion.Development.UnitTesting.Data.SqlClient;
using Remotion.Reflection;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Persistence;

namespace Remotion.SecurityManager.UnitTests
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private const string c_testDomainConnectionString = "Integrated Security=SSPI;Initial Catalog=RemotionSecurityManager;Data Source=localhost";
    private const string c_masterConnectionString = "Integrated Security=SSPI;Initial Catalog=master;Data Source=localhost";

    [SetUp]
    public void SetUp()
    {
      ProviderCollection<StorageProviderDefinition> providers = new ProviderCollection<StorageProviderDefinition> ();
      providers.Add (new RdbmsProviderDefinition ("SecurityManager", typeof (SecurityManagerSqlProvider), c_testDomainConnectionString));
      PersistenceConfiguration persistenceConfiguration = new PersistenceConfiguration (providers, providers["SecurityManager"]);
      persistenceConfiguration.StorageGroups.Add (new StorageGroupElement (new SecurityManagerStorageGroupAttribute(), "SecurityManager"));

      DomainObjectsConfiguration.SetCurrent (
          new FakeDomainObjectsConfiguration (
              new MappingLoaderConfiguration(),
              persistenceConfiguration,
              new QueryConfiguration (GetFullPath (@"SecurityManagerQueries.xml"))));

      ITypeDiscoveryService typeDiscoveryService = new AssemblyFinderTypeDiscoveryService (
            new AssemblyFinder (ApplicationAssemblyFinderFilter.Instance, typeof (BaseSecurityManagerObject).Assembly));
      MappingConfiguration.SetCurrent (new MappingConfiguration (new MappingReflector (typeDiscoveryService)));

      SqlConnection.ClearAllPools();

      DatabaseAgent masterAgent = new DatabaseAgent (c_masterConnectionString);
      masterAgent.ExecuteBatch ("SecurityManagerCreateDB.sql", false);
      DatabaseAgent databaseAgent = new DatabaseAgent (c_testDomainConnectionString);
      databaseAgent.ExecuteBatch ("SecurityManagerSetupDB.sql", true);
      databaseAgent.ExecuteBatch ("SecurityManagerSetupConstraints.sql", true);
      databaseAgent.ExecuteBatch ("SecurityManagerSetupDBSpecialTables.sql", true);
    }

    [TearDown]
    public void TearDown()
    {
      SqlConnection.ClearAllPools();
    }

    private string GetFullPath (string fileName)
    {
      return Path.Combine (AppDomain.CurrentDomain.BaseDirectory, fileName);
    }
  }
}