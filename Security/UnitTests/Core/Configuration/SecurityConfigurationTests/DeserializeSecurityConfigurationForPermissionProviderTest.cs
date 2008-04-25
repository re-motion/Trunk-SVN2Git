using System;
using System.Configuration;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Configuration;
using Remotion.Security.Metadata;

namespace Remotion.Security.UnitTests.Core.Configuration.SecurityConfigurationTests
{
  [TestFixture]
  public class DeserializeSecurityConfigurationForPermissionProviderTest: TestBase
  {
    [Test]
    public void Test_FallbackToDefaultWellKnownDefaultPermissionProvider()
    {
      string xmlFragment = @"<remotion.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.AreEqual (1, Configuration.PermissionProviders.Count);
      Assert.IsInstanceOfType (typeof (PermissionReflector), Configuration.PermissionProvider);
      Assert.AreSame (Configuration.PermissionProvider, Configuration.PermissionProviders["Reflection"]);
    }

    [Test]
    public void Test_PermissionProviderIsAlwaysSameInstance()
    {
      string xmlFragment = @"<remotion.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.AreSame (Configuration.PermissionProvider, Configuration.PermissionProvider);
    }

    [Test]
    public void Test_CustomPermissionProvider()
    {
      string xmlFragment =
          @"
          <remotion.security defaultPermissionProvider=""Custom"">
            <permissionProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.PermissionProviderMock"" />
            </permissionProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (PermissionProviderMock), Configuration.PermissionProvider);
      Assert.AreSame (Configuration.PermissionProvider, Configuration.PermissionProviders["Custom"]);
    }

    [Test]
    public void Test_WithPermissionProvidersAndFallbackToDefaultWellKnownDefaultPermissionProvider()
    {
      string xmlFragment =
          @"
          <remotion.security>
            <permissionProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.PermissionProviderMock"" />
            </permissionProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.AreEqual (2, Configuration.PermissionProviders.Count);
      Assert.IsInstanceOfType (typeof (PermissionProviderMock), Configuration.PermissionProviders["Custom"]);
      Assert.IsInstanceOfType (typeof (PermissionReflector), Configuration.PermissionProvider);
      Assert.AreSame (Configuration.PermissionProvider, Configuration.PermissionProviders["Reflection"]);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "The provider 'Invalid' specified for the defaultPermissionProvider does not exist in the providers collection.")]
    public void Test_WithCustomPermissionProviderAndInvalidName()
    {
      string xmlFragment =
          @"
          <remotion.security defaultPermissionProvider=""Invalid"">
            <permissionProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.PermissionProviderMock"" />
            </permissionProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.PermissionProvider;
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "The name of the entry 'Reflection' identifies a well known provider and cannot be reused for custom providers.")]
    public void Test_DuplicateWellKnownPermissionProviderForPermissionReflector()
    {
      string xmlFragment =
          @"
          <remotion.security defaultPermissionProvider=""Reflection"">
            <permissionProviders>
              <add name=""Reflection"" type=""Remotion.Security.UnitTests::Core.Configuration.PermissionProviderMock"" />
            </permissionProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "The value for the property 'defaultPermissionProvider' is not valid. The error is: The string must be at least 1 characters long.")]
    public void Test_WithCustomPermissionProviderNameEmpty()
    {
      string xmlFragment =
          @"
          <remotion.security defaultPermissionProvider="""">
            <permissionProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.PermissionProviderMock"" />
            </permissionProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.PermissionProvider;
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Test_WithPermissionProvidersReadOnly()
    {
      string xmlFragment =
          @"
          <remotion.security>
            <permissionProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.PermissionProviderMock"" />
            </permissionProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Configuration.PermissionProviders.Clear();
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ConfigurationErrorsException),
        ExpectedMessage = "Provider must implement the interface 'Remotion.Security.Metadata.IPermissionProvider'.")]
    public void InstantiateProvider_WithTypeNotImplementingRequiredInterface()
    {
      string xmlFragment =
          @"
          <remotion.security>
            <permissionProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.UserProviderMock"" />
            </permissionProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.PermissionProvider;
    }
  }
}