﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Diagnostics;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using DomainObjectIDs = Remotion.Data.UnitTests.DomainObjects.Factories.DomainObjectIDs;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  public class TestMappingConfiguration
  {
    private static TestMappingConfiguration s_instance;

    private readonly StorageConfiguration _storageConfiguration;
    private readonly MappingLoaderConfiguration _mappingLoaderConfiguration;
    private readonly QueryConfiguration _queryConfiguration;
    private readonly MappingConfiguration _mappingConfiguration;
    private readonly DomainObjectIDs _domainObjectIDs;

    public static TestMappingConfiguration Instance
    {
      get
      {
        if (s_instance == null)
        {
          Debugger.Break ();
          throw new InvalidOperationException ("TestMappingConfiguration has not been Initialized by invoking Initialize()");
        }
        return s_instance;
      }
    }

    public static void Initialize ()
    {
      s_instance = new TestMappingConfiguration ();
    }

    protected TestMappingConfiguration ()
    {
      ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = StorageProviderDefinitionFactory.Create();
      _storageConfiguration = new StorageConfiguration (
          storageProviderDefinitionCollection, storageProviderDefinitionCollection[DatabaseTest.DefaultStorageProviderID]);
      _storageConfiguration.StorageGroups.Add (new StorageGroupElement (new TestDomainAttribute(), DatabaseTest.c_testDomainProviderID));
      _storageConfiguration.StorageGroups.Add (
          new StorageGroupElement (new StorageProviderStubAttribute(), DatabaseTest.c_unitTestStorageProviderStubID));
      _storageConfiguration.StorageGroups.Add (
          new StorageGroupElement (new TableInheritanceTestDomainAttribute(), TableInheritanceMappingTest.TableInheritanceTestDomainProviderID));

      _mappingLoaderConfiguration = new MappingLoaderConfiguration();
      _queryConfiguration = new QueryConfiguration ("DomainObjects\\QueriesForStandardMapping.xml");
      DomainObjectsConfiguration.SetCurrent (
          new FakeDomainObjectsConfiguration (_mappingLoaderConfiguration, _storageConfiguration, _queryConfiguration));

      //var mappingNamespace = GetType().Namespace;

      var typeDiscoveryService = GetTypeDiscoveryService ();
      //typeDiscoveryService = FilteringTypeDiscoveryService.CreateFromNamespaceWhitelist (typeDiscoveryService, mappingNamespace);

      _mappingConfiguration = new MappingConfiguration (new MappingReflector (typeDiscoveryService));
      MappingConfiguration.SetCurrent (_mappingConfiguration);

      _domainObjectIDs = new DomainObjectIDs();
    }

    private ITypeDiscoveryService GetTypeDiscoveryService ()
    {
      var rootAssemlbies = new[] { new RootAssembly (GetType ().Assembly, true) };
      var rootAssemblyFinder = new FixedRootAssemblyFinder (rootAssemlbies);
      var assemblyLoader = new FilteringAssemblyLoader (ApplicationAssemblyLoaderFilter.Instance);
      var assemblyFinder = new AssemblyFinder (rootAssemblyFinder, assemblyLoader);
      return new AssemblyFinderTypeDiscoveryService (assemblyFinder);
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
 
    public DomainObjectIDs GetDomainObjectIDs()
    {
      return _domainObjectIDs;
    }
  }
}