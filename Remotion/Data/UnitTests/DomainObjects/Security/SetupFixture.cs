// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.UnitTests.DomainObjects.Security.TestDomain;
using Remotion.Development.UnitTesting.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;

namespace Remotion.Data.UnitTests.DomainObjects.Security
{
  [SetUpFixture]
  public class SetUpFixture
  {
    [SetUp]
    public void SetUp ()
    {
      try
      {
        ProviderCollection<StorageProviderDefinition> providers = new ProviderCollection<StorageProviderDefinition>();
        providers.Add (new RdbmsProviderDefinition ("StorageProvider", typeof (StubStorageFactory), "NonExistingRdbms"));
        StorageConfiguration storageConfiguration = new StorageConfiguration (providers, providers["StorageProvider"]);
        DomainObjectsConfiguration.SetCurrent (
            new FakeDomainObjectsConfiguration (
                new MappingLoaderConfiguration(),
                storageConfiguration,
                new QueryConfiguration (GetFullPath (@"DomainObjects\Security\Remotion.Data.UnitTests.DomainObjects.Security.Queries.xml"))));

        MappingConfiguration.SetCurrent (
            new MappingConfiguration (
                new MappingReflector (GetTypeDiscoveryService (GetType().Assembly)),
                new PersistenceModelLoader(new StorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage))));
      }
      catch (Exception ex)
      {
        Console.WriteLine ("SetUpFixture failed: " + ex);
        Console.WriteLine();
        throw;
      }
    }

    private string GetFullPath (string fileName)
    {
      return Path.Combine (AppDomain.CurrentDomain.BaseDirectory, fileName);
    }

    private ITypeDiscoveryService GetTypeDiscoveryService (params Assembly[] rootAssemblies)
    {
      var rootAssemblyFinder = new FixedRootAssemblyFinder (rootAssemblies.Select (asm => new RootAssembly (asm, true)).ToArray());
      var assemblyLoader = new FilteringAssemblyLoader (ApplicationAssemblyLoaderFilter.Instance);
      var assemblyFinder = new AssemblyFinder (rootAssemblyFinder, assemblyLoader);
      ITypeDiscoveryService typeDiscoveryService = new AssemblyFinderTypeDiscoveryService (assemblyFinder);

      return FilteringTypeDiscoveryService.CreateFromNamespaceWhitelist (
          typeDiscoveryService, "Remotion.Data.UnitTests.DomainObjects.Security.TestDomain");
    }
  }
}