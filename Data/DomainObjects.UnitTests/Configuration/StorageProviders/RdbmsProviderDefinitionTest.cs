using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Configuration.StorageProviders;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.StorageProviders
{
  [TestFixture]
  public class RdbmsProviderDefinitionTest
  {
    [Test]
    public void Initialize()
    {
      RdbmsProviderDefinition providerDefinition = new RdbmsProviderDefinition ("Provider", typeof (SqlProvider), "ConnectionString");

      Assert.That (providerDefinition.Name, Is.EqualTo ("Provider"));
      Assert.That (providerDefinition.StorageProviderType, Is.SameAs (typeof (SqlProvider)));
      Assert.That (providerDefinition.ConnectionString, Is.EqualTo ("ConnectionString"));
    }

    [Test]
    public void GetTypeConversionProvider ()
    {
      RdbmsProviderDefinition providerDefinition = new RdbmsProviderDefinition ("Provider", typeof (SqlProvider), "ConnectionString");
      Assert.That (providerDefinition.TypeConversionProvider, Is.InstanceOfType (typeof (TypeConversionProvider)));
    }

    [Test]
    public void GetTypeProvider ()
    {
      RdbmsProviderDefinition providerDefinition = new RdbmsProviderDefinition ("Provider", typeof (SqlProvider), "ConnectionString");
      Assert.That (providerDefinition.TypeProvider, Is.InstanceOfType (typeof (TypeProvider)));
    }
  }
}