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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Development.UnitTesting.Configuration;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Configuration.Mapping
{
  [TestFixture]
  public class MappingLoaderConfigurationTest
  {
    private MappingLoaderConfiguration _configuration;

    [SetUp]
    public void SetUp()
    {
      _configuration = new MappingLoaderConfiguration();
    }

    [Test]
    public void Deserialize_WithCustomMappingLoader()
    {
      string xmlFragment =
          @"<mapping>
            <loader type=""Remotion.Data.DomainObjects.UnitTests::DomainObjects.Configuration.Mapping.FakeMappingLoader""/>
          </mapping>";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (FakeMappingLoader), _configuration.CreateMappingLoader());
    }

    [Test]
    public void Deserialize_WithDefaultMappingLoader ()
    {
      string xmlFragment = @"<mapping />";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (MappingReflector), _configuration.CreateMappingLoader());
    }

    [Test]
    public void Deserialize_WithCustomDomainObjectFactory ()
    {
      string xmlFragment =
          @"<mapping>
            <domainObjectFactory type=""Remotion.Data.DomainObjects.UnitTests::DomainObjects.Configuration.Mapping.FakeDomainObjectFactory""/>
          </mapping>";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (FakeDomainObjectFactory), _configuration.DomainObjectFactory);
    }

    [Test]
    public void Deserialize_WithDefaultDomainObjectFactory()
    {
      string xmlFragment = @"<mapping />";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (InterceptedDomainObjectFactory), _configuration.DomainObjectFactory);
    }
  }
}
