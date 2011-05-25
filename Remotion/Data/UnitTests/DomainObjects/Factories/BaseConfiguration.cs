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
using System.Linq;
using System.Reflection;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;

namespace Remotion.Data.UnitTests.DomainObjects.Factories
{
  public abstract class BaseConfiguration
  {
    public static ITypeDiscoveryService GetTypeDiscoveryService (params Assembly[] rootAssemblies)
    {
      var rootAssemblyFinder = new FixedRootAssemblyFinder (rootAssemblies.Select (asm => new RootAssembly (asm, true)).ToArray());
      var assemblyLoader = new FilteringAssemblyLoader (ApplicationAssemblyLoaderFilter.Instance);
      var assemblyFinder = new AssemblyFinder (rootAssemblyFinder, assemblyLoader);
      ITypeDiscoveryService typeDiscoveryService = new AssemblyFinderTypeDiscoveryService (assemblyFinder);

      return FilteringTypeDiscoveryService.CreateFromNamespaceBlacklist (
          typeDiscoveryService,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain",
          "Remotion.Data.UnitTests.DomainObjects.Security.TestDomain",
          "Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGenerationTestDomain"
          );
    }

    private readonly StorageConfiguration _storageConfiguration;
    private readonly MappingLoaderConfiguration _mappingLoaderConfiguration;
    private readonly QueryConfiguration _queryConfiguration;
    private readonly MappingConfiguration _mappingConfiguration;

    protected BaseConfiguration ()
    {
      ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = StorageProviderDefinitionFactory.Create();
      
      _storageConfiguration = new StorageConfiguration (
          storageProviderDefinitionCollection, 
          storageProviderDefinitionCollection[DatabaseTest.DefaultStorageProviderID]);
      
      _storageConfiguration.StorageGroups.Add (
          new StorageGroupElement (
              new TestDomainAttribute(), 
              DatabaseTest.c_testDomainProviderID));
      _storageConfiguration.StorageGroups.Add (
          new StorageGroupElement (
              new StorageProviderStubAttribute(), 
              DatabaseTest.c_unitTestStorageProviderStubID));
      _storageConfiguration.StorageGroups.Add (
          new StorageGroupElement (
              new TableInheritanceTestDomainAttribute(), 
              TableInheritanceMappingTest.TableInheritanceTestDomainProviderID));
     
      _mappingLoaderConfiguration = new MappingLoaderConfiguration();
      _queryConfiguration = new QueryConfiguration ("DomainObjects\\QueriesForStandardMapping.xml");
     
      var typeDiscoveryService = GetTypeDiscoveryService (GetType().Assembly);

      _mappingConfiguration = new MappingConfiguration (
          MappingReflectorFactory.CreateMappingReflector (typeDiscoveryService), 
          new PersistenceModelLoader (new StorageProviderDefinitionFinder (_storageConfiguration)));
    }

    public MappingConfiguration GetMappingConfiguration ()
    {
      return _mappingConfiguration;
    }

    public StorageConfiguration GetPersistenceConfiguration ()
    {
      return _storageConfiguration;
    }

    public FakeDomainObjectsConfiguration GetDomainObjectsConfiguration ()
    {
      return new FakeDomainObjectsConfiguration (_mappingLoaderConfiguration, _storageConfiguration, _queryConfiguration);
    }
  }
}