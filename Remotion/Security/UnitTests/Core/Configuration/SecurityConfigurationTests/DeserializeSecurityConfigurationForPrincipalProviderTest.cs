// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
      Assert.IsInstanceOf (typeof (ThreadPrincipalProvider), Configuration.PrincipalProvider);
    }

    [Test]
    public void Test_UserProviderIsAlwaysSameInstance ()
    {
      string xmlFragment = @"<remotion.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.That (Configuration.PrincipalProvider, Is.SameAs (Configuration.PrincipalProvider));
    }

    [Test]
    public void Test_WithThreadUserProvider ()
    {
      string xmlFragment = @"<remotion.security defaultPrincipalProvider=""Thread"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.IsInstanceOf (typeof (ThreadPrincipalProvider), Configuration.PrincipalProvider);
    }

    [Test]
    [Explicit]
    public void Test_WithSecurityManagerPrincipalProvider ()
    {
      string xmlFragment = @"<remotion.security defaultPrincipalProvider=""SecurityManager"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Type expectedType = TypeUtility.GetType ("Remotion.SecurityManager::Domain.SecurityManagerPrincipalService", true);

      Assert.IsInstanceOf (expectedType, Configuration.SecurityProvider);
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

      Assert.IsInstanceOf (typeof (PrincipalProviderMock), Configuration.PrincipalProvider);
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

      Assert.That (Configuration.PrincipalProviders.Count, Is.EqualTo (2));
      Assert.IsInstanceOf (typeof (PrincipalProviderMock), Configuration.PrincipalProviders["Custom"]);
      Assert.IsInstanceOf (typeof (ThreadPrincipalProvider), Configuration.PrincipalProvider);
      Assert.That (Configuration.PrincipalProviders["Thread"], Is.SameAs (Configuration.PrincipalProvider));
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
    [ExpectedException (typeof (ConfigurationErrorsException), ExpectedMessage = 
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
