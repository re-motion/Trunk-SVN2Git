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
  public class DeserializeSecurityConfigurationForSecurityProviderTest : TestBase
  {
    [Test]
    public void Test_WithDefaultService ()
    {
      string xmlFragment = @"<remotion.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.IsInstanceOf (typeof (NullSecurityProvider), Configuration.SecurityProvider);
    }

    [Test]
    public void Test_SecurityProviderIsAlwaysSameInstance ()
    {
      string xmlFragment = @"<remotion.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.That (Configuration.SecurityProvider, Is.SameAs (Configuration.SecurityProvider));
    }

    [Test]
    public void Test_WithNullSecurityProvider ()
    {
      string xmlFragment = @"<remotion.security defaultSecurityProvider=""None"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.IsInstanceOf (typeof (NullSecurityProvider), Configuration.SecurityProvider);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException))]
    public void Test_WithInvalidServiceType ()
    {
      string xmlFragment = @"<remotion.security defaultSecurityProvider=""Invalid"" />";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Dev.Null = Configuration.SecurityProvider;
    }

    [Test]
    [Explicit]
    public void Test_WithSecurityManagerSecurityService ()
    {
      string xmlFragment = @"<remotion.security defaultSecurityProvider=""SecurityManager"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Type expectedType = TypeUtility.GetType ("Remotion.SecurityManager::SecurityService", true);

      Assert.IsInstanceOf (expectedType, Configuration.SecurityProvider);
    }

    [Test]
    public void Test_WithCustomService ()
    {
      string xmlFragment = @"
          <remotion.security defaultSecurityProvider=""Custom"">
            <securityProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.SecurityProviderMock"" />
            </securityProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.IsInstanceOf (typeof (SecurityProviderMock), Configuration.SecurityProvider);
    }

    [Test]
    public void Test_WithSecurityProvidersAndFallbackToDefaultWellKnownDefaultSecurityProvider ()
    {
      string xmlFragment = @"
          <remotion.security>
            <securityProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.SecurityProviderMock"" />
            </securityProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.That (Configuration.SecurityProviders.Count, Is.EqualTo (2));
      Assert.IsInstanceOf (typeof (SecurityProviderMock), Configuration.SecurityProviders["Custom"]);
      Assert.IsInstanceOf (typeof (NullSecurityProvider), Configuration.SecurityProvider);
      Assert.That (Configuration.SecurityProviders["None"], Is.SameAs (Configuration.SecurityProvider));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "The provider 'Invalid' specified for the defaultSecurityProvider does not exist in the providers collection.")]
    public void Test_WithCustomSecurityProviderAndInvalidName ()
    {
      string xmlFragment = @"
          <remotion.security defaultSecurityProvider=""Invalid"">
            <securityProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.SecurityProviderMock"" />
            </securityProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.SecurityProvider;
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException), ExpectedMessage = "The name of the entry 'None' identifies a well known provider and cannot be reused for custom providers.")]
    public void Test_DuplicateWellKnownSecurityProviderForNullSecurityProvider ()
    {
      string xmlFragment = @"
          <remotion.security defaultSecurityProvider=""None"">
            <securityProviders>
              <add name=""None"" type=""Remotion.Security.UnitTests::Core.Configuration.SecurityProviderMock"" />
            </securityProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException), ExpectedMessage = "The name of the entry 'SecurityManager' identifies a well known provider and cannot be reused for custom providers.")]
    public void Test_DuplicateWellKnownSecurityProviderForSecurityManagerSecurityService ()
    {
      string xmlFragment = @"
          <remotion.security defaultSecurityProvider=""SecurityManager"">
            <securityProviders>
              <add name=""SecurityManager"" type=""Remotion.Security.UnitTests::Core.Configuration.SecurityProviderMock"" />
            </securityProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "The value for the property 'defaultSecurityProvider' is not valid. The error is: The string must be at least 1 characters long.")]
    public void Test_WithCustomSecurityProviderNameEmpty ()
    {
      string xmlFragment = @"
          <remotion.security defaultSecurityProvider="""">
            <securityProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.SecurityProviderMock"" />
            </securityProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.SecurityProvider;
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Test_WithSecurityProvidersReadOnly ()
    {
      string xmlFragment =
          @"
          <remotion.security>
            <securityProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.SecurityProviderMock"" />
            </securityProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Configuration.SecurityProviders.Clear ();
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException), ExpectedMessage = 
        "Provider must implement the interface 'Remotion.Security.ISecurityProvider'.")]
    public void InstantiateProvider_WithTypeNotImplementingRequiredInterface ()
    {
      string xmlFragment =
          @"
          <remotion.security>
            <securityProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.PrincipalProviderMock"" />
            </securityProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.SecurityProvider;
    }
  }
}
