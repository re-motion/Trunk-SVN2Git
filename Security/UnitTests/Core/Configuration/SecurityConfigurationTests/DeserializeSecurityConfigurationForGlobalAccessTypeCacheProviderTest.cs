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

namespace Remotion.Security.UnitTests.Core.Configuration.SecurityConfigurationTests
{
  [TestFixture]
  public class DeserializeSecurityConfigurationForGlobalAccessTypeCacheProviderTest: TestBase
  {
    [Test]
    public void Test_FallbackToDefaultWellKnownDefaultGlobalAccessTypeCacheProvider()
    {
      string xmlFragment = @"<remotion.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.AreEqual (2, Configuration.GlobalAccessTypeCacheProviders.Count);
      Assert.IsInstanceOfType (typeof (NullGlobalAccessTypeCacheProvider), Configuration.GlobalAccessTypeCacheProvider);
      Assert.AreSame (Configuration.GlobalAccessTypeCacheProvider, Configuration.GlobalAccessTypeCacheProviders["None"]);
    }

    [Test]
    public void Test_GlobalAccessTypeCacheProviderIsAlwaysSameInstance()
    {
      string xmlFragment = @"<remotion.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.AreSame (Configuration.GlobalAccessTypeCacheProvider, Configuration.GlobalAccessTypeCacheProvider);
    }

    [Test]
    public void Test_WithNoGlobalAccessTypeCacheProvider()
    {
      string xmlFragment = @"<remotion.security defaultGlobalAccessTypeCacheProvider=""None"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (NullGlobalAccessTypeCacheProvider), Configuration.GlobalAccessTypeCacheProvider);
    }

    [Test]
    public void Test_WithRevisionBasedAccessTypeCacheProvider()
    {
      string xmlFragment = @"<remotion.security defaultGlobalAccessTypeCacheProvider=""RevisionBased"" />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (RevisionBasedAccessTypeCacheProvider), Configuration.GlobalAccessTypeCacheProvider);
    }

    [Test]
    public void Test_WithCustomGlobalAccessTypeCacheProvider()
    {
      string xmlFragment =
          @"
          <remotion.security defaultGlobalAccessTypeCacheProvider=""Custom"">
            <globalAccessTypeCacheProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.GlobalAccessTypeCacheProviderMock"" />
            </globalAccessTypeCacheProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (GlobalAccessTypeCacheProviderMock), Configuration.GlobalAccessTypeCacheProvider);
      Assert.AreSame (Configuration.GlobalAccessTypeCacheProvider, Configuration.GlobalAccessTypeCacheProviders["Custom"]);
    }

    [Test]
    public void Test_WithGlobalAccessTypeCacheProvidersAndFallbackToDefaultWellKnownDefaultGlobalAccessTypeCacheProvider()
    {
      string xmlFragment =
          @"
          <remotion.security>
            <globalAccessTypeCacheProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.GlobalAccessTypeCacheProviderMock"" />
            </globalAccessTypeCacheProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.AreEqual (3, Configuration.GlobalAccessTypeCacheProviders.Count);
      Assert.IsInstanceOfType (typeof (GlobalAccessTypeCacheProviderMock), Configuration.GlobalAccessTypeCacheProviders["Custom"]);
      Assert.IsInstanceOfType (typeof (NullGlobalAccessTypeCacheProvider), Configuration.GlobalAccessTypeCacheProvider);
      Assert.AreSame (Configuration.GlobalAccessTypeCacheProvider, Configuration.GlobalAccessTypeCacheProviders["None"]);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "The provider 'Invalid' specified for the defaultGlobalAccessTypeCacheProvider does not exist in the providers collection.")]
    public void Test_WithCustomGlobalAccessTypeCacheProviderAndInvalidName()
    {
      string xmlFragment =
          @"
          <remotion.security defaultGlobalAccessTypeCacheProvider=""Invalid"">
            <globalAccessTypeCacheProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.GlobalAccessTypeCacheProviderMock"" />
            </globalAccessTypeCacheProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.GlobalAccessTypeCacheProvider;
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "The name of the entry 'None' identifies a well known provider and cannot be reused for custom providers.")]
    public void Test_DuplicateWellKnownGlobalAccessTypeCacheProviderForGlobalAccessTypeCacheNone()
    {
      string xmlFragment =
          @"
          <remotion.security defaultGlobalAccessTypeCacheProvider=""None"">
            <globalAccessTypeCacheProviders>
              <add name=""None"" type=""Remotion.Security.UnitTests::Core.Configuration.GlobalAccessTypeCacheProviderMock"" />
            </globalAccessTypeCacheProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "The name of the entry 'RevisionBased' identifies a well known provider and cannot be reused for custom providers.")]
    public void Test_DuplicateWellKnownGlobalAccessTypeCacheProviderForGlobalAccessTypeCacheRevisionBased()
    {
      string xmlFragment =
          @"
          <remotion.security defaultGlobalAccessTypeCacheProvider=""RevisionBased"">
            <globalAccessTypeCacheProviders>
              <add name=""RevisionBased"" type=""Remotion.Security.UnitTests::Core.Configuration.GlobalAccessTypeCacheProviderMock"" />
            </globalAccessTypeCacheProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "The value for the property 'defaultGlobalAccessTypeCacheProvider' is not valid. The error is: The string must be at least 1 characters long."
        )]
    public void Test_WithCustomGlobalAccessTypeCacheProviderNameEmpty()
    {
      string xmlFragment =
          @"
          <remotion.security defaultGlobalAccessTypeCacheProvider="""">
            <globalAccessTypeCacheProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.GlobalAccessTypeCacheProviderMock"" />
            </globalAccessTypeCacheProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.GlobalAccessTypeCacheProvider;
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Test_WithGlobalAccessTypeCacheProvidersReadOnly()
    {
      string xmlFragment =
          @"
          <remotion.security>
            <globalAccessTypeCacheProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.GlobalAccessTypeCacheProviderMock"" />
            </globalAccessTypeCacheProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Configuration.GlobalAccessTypeCacheProviders.Clear();
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ConfigurationErrorsException),
        ExpectedMessage = "Provider must implement the interface 'Remotion.Security.IGlobalAccessTypeCacheProvider'.")]
    public void InstantiateProvider_WithTypeNotImplementingRequiredInterface()
    {
      string xmlFragment =
          @"
          <remotion.security>
            <globalAccessTypeCacheProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.UserProviderMock"" />
            </globalAccessTypeCacheProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.GlobalAccessTypeCacheProvider;
    }
  }
}
