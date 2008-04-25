using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Development.UnitTesting.Configuration;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class MappingLoaderConfigurationTest
  {
    private MappingLoaderConfiguration _configuration;

    [SetUp]
    public void SetUp()
    {
      _configuration = new MappingLoaderConfiguration();
    }

    [Test]
    public void Deserialize_WithCustomMappingLoader()
    {
      string xmlFragment =
          @"<mapping>
            <loader type=""Remotion.Data.DomainObjects.UnitTests::Configuration.Mapping.FakeMappingLoader""/>
          </mapping>";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (FakeMappingLoader), _configuration.CreateMappingLoader());
    }

    [Test]
    public void Deserialize_WithDefaultMappingLoader ()
    {
      string xmlFragment = @"<mapping />";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (MappingReflector), _configuration.CreateMappingLoader());
    }

    [Test]
    public void Deserialize_WithCustomDomainObjectFactory ()
    {
      string xmlFragment =
          @"<mapping>
            <domainObjectFactory type=""Remotion.Data.DomainObjects.UnitTests::Configuration.Mapping.FakeDomainObjectFactory""/>
          </mapping>";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (FakeDomainObjectFactory), _configuration.DomainObjectFactory);
    }

    [Test]
    public void Deserialize_WithDefaultDomainObjectFactory()
    {
      string xmlFragment = @"<mapping />";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (InterceptedDomainObjectFactory), _configuration.DomainObjectFactory);
    }
  }
}