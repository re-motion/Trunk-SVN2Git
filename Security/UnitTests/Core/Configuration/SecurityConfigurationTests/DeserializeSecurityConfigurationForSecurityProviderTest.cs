/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
      Assert.IsInstanceOfType (typeof (NullSecurityProvider), Configuration.SecurityProvider);
    }

    [Test]
    public void Test_SecurityProviderIsAlwaysSameInstance ()
    {
      string xmlFragment = @"<remotion.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.AreSame (Configuration.SecurityProvider, Configuration.SecurityProvider);
    }

    [Test]
    public void Test_WithNullSecurityProvider ()
    {
      string xmlFragment = @"<remotion.security defaultSecurityProvider=""None"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (NullSecurityProvider), Configuration.SecurityProvider);
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
    public void Test_WithSecurityManagerService ()
    {
      string xmlFragment = @"<remotion.security defaultSecurityProvider=""SecurityManagerService"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Type expectedType = TypeUtility.GetType ("Remotion.SecurityManager::SecurityService", true);

      Assert.IsInstanceOfType (expectedType, Configuration.SecurityProvider);
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

      Assert.IsInstanceOfType (typeof (SecurityProviderMock), Configuration.SecurityProvider);
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

      Assert.AreEqual (2, Configuration.SecurityProviders.Count);
      Assert.IsInstanceOfType (typeof (SecurityProviderMock), Configuration.SecurityProviders["Custom"]);
      Assert.IsInstanceOfType (typeof (NullSecurityProvider), Configuration.SecurityProvider);
      Assert.AreSame (Configuration.SecurityProvider, Configuration.SecurityProviders["None"]);
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
    [ExpectedExceptionAttribute (typeof (ConfigurationErrorsException), ExpectedMessage = "Provider must implement the interface 'Remotion.Security.ISecurityProvider'.")]
    public void InstantiateProvider_WithTypeNotImplementingRequiredInterface ()
    {
      string xmlFragment =
          @"
          <remotion.security>
            <securityProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.UserProviderMock"" />
            </securityProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.SecurityProvider;
    }
  }
}
