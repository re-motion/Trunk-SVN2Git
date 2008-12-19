// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Configuration;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Configuration;
using Remotion.Utilities;

namespace Remotion.Security.UnitTests.Core.Configuration.SecurityConfigurationTests
{
  [TestFixture]
  public class DeserializeSecurityConfigurationForPrincipalProviderTest : TestBase
  {
    [Test]
    public void Test_WithDefaultUserProvider ()
    {
      string xmlFragment = @"<remotion.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (ThreadPrincipalProvider), Configuration.PrincipalProvider);
    }

    [Test]
    public void Test_UserProviderIsAlwaysSameInstance ()
    {
      string xmlFragment = @"<remotion.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.AreSame (Configuration.PrincipalProvider, Configuration.PrincipalProvider);
    }

    [Test]
    public void Test_WithThreadUserProvider ()
    {
      string xmlFragment = @"<remotion.security defaultPrincipalProvider=""Thread"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (ThreadPrincipalProvider), Configuration.PrincipalProvider);
    }

    [Test]
    [Explicit]
    public void Test_WithSecurityManagerPrincipalProvider ()
    {
      string xmlFragment = @"<remotion.security defaultPrincipalProvider=""SecurityManager"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Type expectedType = TypeUtility.GetType ("Remotion.SecurityManager::Domain.SecurityManagerPrincipalService", true);

      Assert.IsInstanceOfType (expectedType, Configuration.SecurityProvider);
    }

    [Test]
    public void Test_WithCustomUserProvider ()
    {
      string xmlFragment = @"
          <remotion.security defaultPrincipalProvider=""Custom"">
            <principalProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.PrincipalProviderMock"" />
            </principalProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (PrincipalProviderMock), Configuration.PrincipalProvider);
    }

    [Test]
    public void Test_WithUserProvidersAndFallbackToDefaultWellKnownDefaultUserProvider ()
    {
      string xmlFragment = @"
          <remotion.security>
            <principalProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.PrincipalProviderMock"" />
            </principalProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.AreEqual (2, Configuration.PrincipalProviders.Count);
      Assert.IsInstanceOfType (typeof (PrincipalProviderMock), Configuration.PrincipalProviders["Custom"]);
      Assert.IsInstanceOfType (typeof (ThreadPrincipalProvider), Configuration.PrincipalProvider);
      Assert.AreSame (Configuration.PrincipalProvider, Configuration.PrincipalProviders["Thread"]);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
         ExpectedMessage = "The provider 'Invalid' specified for the defaultPrincipalProvider does not exist in the providers collection.")]
    public void Test_WithCustomUserProviderAndInvalidName ()
    {
      string xmlFragment = @"
          <remotion.security defaultPrincipalProvider=""Invalid"">
            <principalProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.PrincipalProviderMock"" />
            </principalProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.PrincipalProvider;
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException), 
        ExpectedMessage = "The name of the entry 'Thread' identifies a well known provider and cannot be reused for custom providers.")]
    public void Test_DuplicateWellKnownUserProviderForThreadUserProvider ()
    {
      string xmlFragment = @"
          <remotion.security defaultPrincipalProvider=""Thread"">
            <principalProviders>
              <add name=""Thread"" type=""Remotion.Security.UnitTests::Core.Configuration.UserProviderMock"" />
            </principalProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException), 
        ExpectedMessage = "The name of the entry 'HttpContext' identifies a well known provider and cannot be reused for custom providers.")]
    public void Test_DuplicateWellKnownUserProviderForHttpContextUserProvider ()
    {
      string xmlFragment = @"
          <remotion.security defaultPrincipalProvider=""HttpContext"">
            <principalProviders>
              <add name=""HttpContext"" type=""Remotion.Security.UnitTests::Core.Configuration.UserProviderMock"" />
            </principalProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException), ExpectedMessage = 
        "The value for the property 'defaultPrincipalProvider' is not valid. The error is: The string must be at least 1 characters long.")]
    public void Test_WithCustomUserProviderNameEmpty ()
    {
      string xmlFragment = @"
          <remotion.security defaultPrincipalProvider="""">
            <principalProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.PrincipalProviderMock"" />
            </principalProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.PrincipalProvider;
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Test_WithUserProvidersReadOnly ()
    {
      string xmlFragment =
          @"
          <remotion.security>
            <principalProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.PrincipalProviderMock"" />
            </principalProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Configuration.PrincipalProviders.Clear ();
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ConfigurationErrorsException), ExpectedMessage = 
      "Provider must implement the interface 'Remotion.Security.IPrincipalProvider'.")]
    public void InstantiateProvider_WithTypeNotImplementingRequiredInterface ()
    {
      string xmlFragment =
          @"
          <remotion.security>
            <principalProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.PermissionProviderMock"" />
            </principalProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.PrincipalProvider;
    }
  }
}
