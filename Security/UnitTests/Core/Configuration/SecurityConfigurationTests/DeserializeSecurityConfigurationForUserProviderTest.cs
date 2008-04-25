using System;
using System.Configuration;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Configuration;

namespace Remotion.Security.UnitTests.Core.Configuration.SecurityConfigurationTests
{
  [TestFixture]
  public class DeserializeSecurityConfigurationForUserProviderTest : TestBase
  {
    [Test]
    public void Test_WithDefaultUserProvider ()
    {
      string xmlFragment = @"<remotion.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (ThreadUserProvider), Configuration.UserProvider);
    }

    [Test]
    public void Test_UserProviderIsAlwaysSameInstance ()
    {
      string xmlFragment = @"<remotion.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.AreSame (Configuration.UserProvider, Configuration.UserProvider);
    }

    [Test]
    public void Test_WithThreadUserProvider ()
    {
      string xmlFragment = @"<remotion.security defaultUserProvider=""Thread"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (ThreadUserProvider), Configuration.UserProvider);
    }

    [Test]
    public void Test_WithCustomUserProvider ()
    {
      string xmlFragment = @"
          <remotion.security defaultUserProvider=""Custom"">
            <userProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.UserProviderMock"" />
            </userProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (UserProviderMock), Configuration.UserProvider);
    }

    [Test]
    public void Test_WithUserProvidersAndFallbackToDefaultWellKnownDefaultUserProvider ()
    {
      string xmlFragment = @"
          <remotion.security>
            <userProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.UserProviderMock"" />
            </userProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.AreEqual (2, Configuration.UserProviders.Count);
      Assert.IsInstanceOfType (typeof (UserProviderMock), Configuration.UserProviders["Custom"]);
      Assert.IsInstanceOfType (typeof (ThreadUserProvider), Configuration.UserProvider);
      Assert.AreSame (Configuration.UserProvider, Configuration.UserProviders["Thread"]);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
         ExpectedMessage = "The provider 'Invalid' specified for the defaultUserProvider does not exist in the providers collection.")]
    public void Test_WithCustomUserProviderAndInvalidName ()
    {
      string xmlFragment = @"
          <remotion.security defaultUserProvider=""Invalid"">
            <userProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.UserProviderMock"" />
            </userProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.UserProvider;
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException), 
        ExpectedMessage = "The name of the entry 'Thread' identifies a well known provider and cannot be reused for custom providers.")]
    public void Test_DuplicateWellKnownUserProviderForThreadUserProvider ()
    {
      string xmlFragment = @"
          <remotion.security defaultUserProvider=""Thread"">
            <userProviders>
              <add name=""Thread"" type=""Remotion.Security.UnitTests::Core.Configuration.UserProviderMock"" />
            </userProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException), 
        ExpectedMessage = "The name of the entry 'HttpContext' identifies a well known provider and cannot be reused for custom providers.")]
    public void Test_DuplicateWellKnownUserProviderForHttpContextUserProvider ()
    {
      string xmlFragment = @"
          <remotion.security defaultUserProvider=""HttpContext"">
            <userProviders>
              <add name=""HttpContext"" type=""Remotion.Security.UnitTests::Core.Configuration.UserProviderMock"" />
            </userProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "The value for the property 'defaultUserProvider' is not valid. The error is: The string must be at least 1 characters long.")]
    public void Test_WithCustomUserProviderNameEmpty ()
    {
      string xmlFragment = @"
          <remotion.security defaultUserProvider="""">
            <userProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.UserProviderMock"" />
            </userProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.UserProvider;
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Test_WithUserProvidersReadOnly ()
    {
      string xmlFragment =
          @"
          <remotion.security>
            <userProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.UserProviderMock"" />
            </userProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Configuration.UserProviders.Clear ();
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ConfigurationErrorsException), ExpectedMessage = "Provider must implement the interface 'Remotion.Security.IUserProvider'.")]
    public void InstantiateProvider_WithTypeNotImplementingRequiredInterface ()
    {
      string xmlFragment =
          @"
          <remotion.security>
            <userProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.PermissionProviderMock"" />
            </userProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.UserProvider;
    }
  }
}