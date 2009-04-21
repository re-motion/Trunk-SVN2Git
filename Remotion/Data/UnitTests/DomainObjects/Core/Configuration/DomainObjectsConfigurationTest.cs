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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Resources;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Development.UnitTesting.IO;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration
{
  [TestFixture]
  public class DomainObjectsConfigurationTest
  {
    [SetUp]
    public void SetUp()
    {
      DomainObjectsConfiguration.SetCurrent (null);
    }

    [Test]
    public void GetAndSet()
    {
      IDomainObjectsConfiguration configuration =
          new FakeDomainObjectsConfiguration (new MappingLoaderConfiguration(), new StorageConfiguration(), new QueryConfiguration());
      DomainObjectsConfiguration.SetCurrent (configuration);

      Assert.That (DomainObjectsConfiguration.Current, Is.SameAs (configuration));
    }

    [Test]
    public void Get()
    {
      Assert.That (DomainObjectsConfiguration.Current, Is.Not.Null);
    }

    [Test]
    public void Initialize()
    {
      DomainObjectsConfiguration domainObjectsConfiguration = new DomainObjectsConfiguration();

      Assert.That (domainObjectsConfiguration.MappingLoader, Is.Not.Null);
      Assert.That (domainObjectsConfiguration.Storage, Is.Not.Null);
    }

    [Test]
    public void Initialize_WithConfigurationHavingMinimumSettings()
    {
      using (TempFile configFile = new TempFile())
      {
        SetUpConfigurationWrapper (ConfigurationFactory.LoadConfigurationFromFile (configFile, ResourceManager.GetDomainObjectsConfigurationWithMinimumSettings()));

        DomainObjectsConfiguration domainObjectsConfiguration = new DomainObjectsConfiguration();

        Assert.That (domainObjectsConfiguration.MappingLoader, Is.Not.Null);
        Assert.That (domainObjectsConfiguration.Storage, Is.Not.Null);
        Assert.That (domainObjectsConfiguration.Storage.DefaultStorageProviderDefinition, Is.Not.Null);
        Assert.That (domainObjectsConfiguration.Storage.StorageProviderDefinitions.Count, Is.EqualTo (1));
        Assert.That (domainObjectsConfiguration.Storage.StorageGroups, Is.Empty);
      }
    }

    [Test]
    public void Initialize_WithConfigurationHavingCustomMappingLoader()
    {
      using (TempFile configFile = new TempFile())
      {
        SetUpConfigurationWrapper (ConfigurationFactory.LoadConfigurationFromFile (configFile, ResourceManager.GetDomainObjectsConfigurationWithFakeMappingLoader()));

        DomainObjectsConfiguration domainObjectsConfiguration = new DomainObjectsConfiguration();

        Assert.That (domainObjectsConfiguration.MappingLoader, Is.Not.Null);
        Assert.That (domainObjectsConfiguration.MappingLoader.MappingLoaderType, Is.SameAs (typeof (FakeMappingLoader)));

        Assert.That (domainObjectsConfiguration.Storage, Is.Not.Null);
        Assert.That (domainObjectsConfiguration.Storage.DefaultStorageProviderDefinition, Is.Not.Null);
        Assert.That (domainObjectsConfiguration.Storage.StorageProviderDefinitions.Count, Is.EqualTo (1));
        Assert.That (domainObjectsConfiguration.Storage.StorageGroups, Is.Empty);
      }
    }

    [Test]
    public void Initialize_WithConfigurationHavingCustomSectionGroupName()
    {
      using (TempFile configFile = new TempFile())
      {
        System.Configuration.Configuration configuration =
            ConfigurationFactory.LoadConfigurationFromFile (configFile, ResourceManager.GetDomainObjectsConfigurationWithCustomSectionGroupName());
        SetUpConfigurationWrapper (configuration);

        DomainObjectsConfiguration domainObjectsConfiguration = (DomainObjectsConfiguration) configuration.GetSectionGroup ("domainObjects");

        Assert.That (domainObjectsConfiguration.SectionGroupName, Is.EqualTo ("domainObjects"));
        Assert.That (domainObjectsConfiguration.MappingLoader, Is.Not.Null);
        Assert.That (domainObjectsConfiguration.Storage, Is.Not.Null);
        Assert.That (domainObjectsConfiguration.Storage.DefaultStorageProviderDefinition, Is.Not.Null);
        Assert.That (domainObjectsConfiguration.Storage.StorageProviderDefinitions.Count, Is.EqualTo (1));
        Assert.That (domainObjectsConfiguration.Storage.StorageGroups, Is.Empty);
      }
    }

    [Test]
    public void GetMappingLoader_SameInstanceTwice()
    {
      DomainObjectsConfiguration domainObjectsConfiguration = new DomainObjectsConfiguration();

      Assert.That (domainObjectsConfiguration.MappingLoader, Is.SameAs (domainObjectsConfiguration.MappingLoader));
    }

    [Test]
    public void GetStorage_SameInstanceTwice()
    {
      DomainObjectsConfiguration domainObjectsConfiguration = new DomainObjectsConfiguration();

      Assert.That (domainObjectsConfiguration.Storage, Is.SameAs (domainObjectsConfiguration.Storage));
    }

    private void SetUpConfigurationWrapper (System.Configuration.Configuration configuration)
    {
      ConfigurationWrapper.SetCurrent (ConfigurationWrapper.CreateFromConfigurationObject (configuration));
    }
  }
}
