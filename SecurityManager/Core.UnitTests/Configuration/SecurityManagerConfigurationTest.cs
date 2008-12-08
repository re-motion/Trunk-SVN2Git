// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Configuration;
using NUnit.Framework;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Configuration
{
  [TestFixture]
  public class SecurityManagerConfigurationTest
  {
    private TestSecurityManagerConfiguration _configuration;

    [SetUp]
    public void SetUp ()
    {
      _configuration = new TestSecurityManagerConfiguration();
    }

    [Test]
    public void DeserializeSection_DefaultFactory ()
    {
      string xmlFragment = @"<remotion.securityManager />";
      _configuration.DeserializeSection (xmlFragment);

      Assert.IsNotNull (_configuration.OrganizationalStructureFactory);
      Assert.IsInstanceOfType (typeof (OrganizationalStructureFactory), _configuration.OrganizationalStructureFactory);
    }

    [Test]
    public void DeserializeSection_WithNamespace ()
    {
      string xmlFragment = @"<remotion.securityManager xmlns=""http://www.re-motion.org/SecurityManager/Configuration"" />";
      _configuration.DeserializeSection (xmlFragment);

      Assert.IsNotNull (_configuration.OrganizationalStructureFactory);
      Assert.IsInstanceOfType (typeof (OrganizationalStructureFactory), _configuration.OrganizationalStructureFactory);
    }

    [Test]
    public void DeserializeSection_CustomFactory ()
    {
      string xmlFragment =
          @"
          <remotion.securityManager xmlns=""http://www.re-motion.org/SecurityManager/Configuration"">
            <organizationalStructureFactory type=""Remotion.SecurityManager.UnitTests::Configuration.TestOrganizationalStructureFactory"" />
          </remotion.securityManager>";
      _configuration.DeserializeSection (xmlFragment);

      Assert.IsNotNull (_configuration.OrganizationalStructureFactory);
      Assert.IsInstanceOfType (typeof (TestOrganizationalStructureFactory), _configuration.OrganizationalStructureFactory);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException))]
    public void DeserializeSection_InvalidFactoryType ()
    {
      string xmlFragment =
          @"
          <remotion.securityManager>
            <organizationalStructureFactory type=""Invalid"" />
          </remotion.securityManager>";
      _configuration.DeserializeSection (xmlFragment);
      IOrganizationalStructureFactory factory = _configuration.OrganizationalStructureFactory;
    }

    [Test]
    public void DeserializeSection_DefaultAccessControl ()
    {
      string xmlFragment = @"
          <remotion.securityManager xmlns=""http://www.re-motion.org/SecurityManager/Configuration""/>";
      _configuration.DeserializeSection (xmlFragment);

      Assert.IsNotNull (_configuration.AccessControl);
      Assert.IsFalse (_configuration.AccessControl.DisableSpecificUser);
    }

    [Test]
    public void DeserializeSection_DisableSpecificUser_True ()
    {
      string xmlFragment =
          @"
          <remotion.securityManager xmlns=""http://www.re-motion.org/SecurityManager/Configuration"">
            <accessControl disableSpecificUser=""true"" />
          </remotion.securityManager>";
      _configuration.DeserializeSection (xmlFragment);

      Assert.IsTrue (_configuration.AccessControl.DisableSpecificUser);
    }

    [Test]
    public void DeserializeSection_DisableSpecificUser_DefaultToFalse ()
    {
      string xmlFragment =
          @"
          <remotion.securityManager xmlns=""http://www.re-motion.org/SecurityManager/Configuration"">
            <accessControl />
          </remotion.securityManager>";
      _configuration.DeserializeSection (xmlFragment);

      Assert.IsFalse (_configuration.AccessControl.DisableSpecificUser);
    }
  }
}
