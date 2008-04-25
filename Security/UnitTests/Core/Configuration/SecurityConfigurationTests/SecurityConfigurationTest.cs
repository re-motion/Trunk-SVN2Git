using System;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Configuration;
using Remotion.Security.Configuration;
using Remotion.Security.Metadata;

namespace Remotion.Security.UnitTests.Core.Configuration.SecurityConfigurationTests
{
  [TestFixture]
  public class SecurityConfigurationTest: TestBase
  {
    [Test]
    public void GetSecurityConfigurationWithoutConfigurationSection()
    {
      SecurityConfiguration configuration = SecurityConfiguration.Current;

      Assert.IsNotNull (configuration);
      Assert.IsInstanceOfType (typeof (NullSecurityProvider), configuration.SecurityProvider);
      Assert.IsInstanceOfType (typeof (ThreadUserProvider), configuration.UserProvider);
      Assert.IsInstanceOfType (typeof (FunctionalSecurityStrategy), configuration.FunctionalSecurityStrategy);
      Assert.IsInstanceOfType (typeof (PermissionReflector), configuration.PermissionProvider);
      Assert.IsInstanceOfType (typeof (NullGlobalAccessTypeCacheProvider), configuration.GlobalAccessTypeCacheProvider);
    }

    [Test]
    public void DeserializeSecurityConfiguration_WithNamespace()
    {
      string xmlFragment = @"<remotion.security xmlns=""http://www.re-motion.org/Security/Configuration"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      // Succeeded
    }

    [Test]
    public void DeserializeSecurityConfiguration_WithDefaultFunctionalSecurityStrategy()
    {
      string xmlFragment = @"<remotion.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (FunctionalSecurityStrategy), Configuration.FunctionalSecurityStrategy);
    }

    [Test]
    public void FunctionalSecurityStrategyIsAlwaysSameInstance()
    {
      string xmlFragment = @"<remotion.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.AreSame (Configuration.FunctionalSecurityStrategy, Configuration.FunctionalSecurityStrategy);
    }

    [Test]
    public void DeserializeSecurityConfiguration_WithCustomFunctionalSecurityStrategy()
    {
      string xmlFragment =
          @"
          <remotion.security>
            <functionalSecurityStrategy type=""Remotion.Security.UnitTests::Core.Configuration.FunctionalSecurityStrategyMock"" />
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (FunctionalSecurityStrategyMock), Configuration.FunctionalSecurityStrategy);
    }
  }
}