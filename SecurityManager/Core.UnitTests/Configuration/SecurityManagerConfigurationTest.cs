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
      _configuration = new TestSecurityManagerConfiguration ();
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
      string xmlFragment = @"
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
      string xmlFragment = @"
          <remotion.securityManager>
            <organizationalStructureFactory type=""Invalid"" />
          </remotion.securityManager>";
      _configuration.DeserializeSection (xmlFragment);
      IOrganizationalStructureFactory factory = _configuration.OrganizationalStructureFactory;
    }
  }
}
