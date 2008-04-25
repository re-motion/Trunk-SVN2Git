using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration
{
  [TestFixture]
  public class FakeDomainObjectsConfigurationTest
  {
    [Test]
    public void Initialize()
    {
      PersistenceConfiguration storage = new PersistenceConfiguration ();
      MappingLoaderConfiguration mappingLoader = new MappingLoaderConfiguration ();
      QueryConfiguration query = new QueryConfiguration ();
      IDomainObjectsConfiguration configuration = new FakeDomainObjectsConfiguration (mappingLoader, storage, query);
    
      Assert.AreSame (mappingLoader, configuration.MappingLoader);
      Assert.AreSame (storage, configuration.Storage);
      Assert.AreSame (query, configuration.Query);
    }
  }
}