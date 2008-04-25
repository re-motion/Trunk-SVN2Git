using System;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.UnitTests.TableInheritance;

namespace Remotion.Data.DomainObjects.UnitTests.Factories
{
  public static class StorageProviderDefinitionFactory
  {
    public static ProviderCollection<StorageProviderDefinition> Create()
    {
      ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = new ProviderCollection<StorageProviderDefinition>();

      storageProviderDefinitionCollection.Add (
          new RdbmsProviderDefinition (DatabaseTest.c_testDomainProviderID, typeof (SqlProvider), DatabaseTest.TestDomainConnectionString));

      storageProviderDefinitionCollection.Add (
          new RdbmsProviderDefinition (DatabaseTest.DefaultStorageProviderID, typeof (SqlProvider), DatabaseTest.TestDomainConnectionString));

      storageProviderDefinitionCollection.Add (
          new UnitTestStorageProviderStubDefinition (DatabaseTest.c_unitTestStorageProviderStubID, typeof (UnitTestStorageProviderStub)));

      storageProviderDefinitionCollection.Add (
          new RdbmsProviderDefinition (
              TableInheritanceMappingTest.TableInheritanceTestDomainProviderID, typeof (SqlProvider), DatabaseTest.TestDomainConnectionString));

      return storageProviderDefinitionCollection;
    }
  }
}