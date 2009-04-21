// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
      Assert.IsInstanceOfType (typeof (ThreadPrincipalProvider), configuration.PrincipalProvider);
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
