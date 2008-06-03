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
