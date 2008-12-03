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
using System.ComponentModel.Design;
using System.IO;
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.UnitTests.DomainObjects.Security.TestDomain;
using Remotion.Reflection;

namespace Remotion.Data.UnitTests.DomainObjects.Security
{
  [SetUpFixture]
  public class SetUpFixture
  {
    [SetUp]
    public void SetUp()
    {
      ProviderCollection<StorageProviderDefinition> providers = new ProviderCollection<StorageProviderDefinition>();
      providers.Add (new RdbmsProviderDefinition ("StorageProvider", typeof (StubStorageProvider), "NonExistingRdbms"));
      StorageConfiguration storageConfiguration = new StorageConfiguration (providers, providers["StorageProvider"]);
      DomainObjectsConfiguration.SetCurrent (new FakeDomainObjectsConfiguration (new MappingLoaderConfiguration (), storageConfiguration,
          new QueryConfiguration (GetFullPath (@"DomainObjects\Security\Remotion.Data.UnitTests.DomainObjects.Security.Queries.xml"))));

      ITypeDiscoveryService typeDiscoveryService = new AssemblyFinderTypeDiscoveryService (
            new AssemblyFinder (ApplicationAssemblyFinderFilter.Instance, GetType().Assembly));
      MappingConfiguration.SetCurrent (new MappingConfiguration (new MappingReflector (typeDiscoveryService)));
    }

    private string GetFullPath (string fileName)
    {
      return Path.Combine (AppDomain.CurrentDomain.BaseDirectory, fileName);
    }
  }
}
