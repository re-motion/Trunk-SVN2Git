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
using System.Reflection;
using Remotion.Configuration;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Reflection;

namespace Remotion.Data.UnitTests.DomainObjects.Factories
{
  public abstract class BaseConfiguration
  {
    public static AssemblyFinderTypeDiscoveryService GetTypeDiscoveryService (params Assembly[] rootAssemblies)
    {
      return new AssemblyFinderTypeDiscoveryService (new AssemblyFinder (ApplicationAssemblyLoaderFilter.Instance, rootAssemblies));
    }

    private readonly StorageConfiguration _storageConfiguration;
    private readonly MappingLoaderConfiguration _mappingLoaderConfiguration;
    private readonly QueryConfiguration _queryConfiguration;
    private readonly MappingConfiguration _mappingConfiguration;

    protected BaseConfiguration ()
    {
      ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = StorageProviderDefinitionFactory.Create ();
      _storageConfiguration = new StorageConfiguration (storageProviderDefinitionCollection, storageProviderDefinitionCollection[DatabaseTest.DefaultStorageProviderID]);
      _storageConfiguration.StorageGroups.Add (new StorageGroupElement (new TestDomainAttribute (), DatabaseTest.c_testDomainProviderID));
      _storageConfiguration.StorageGroups.Add (new StorageGroupElement (new StorageProviderStubAttribute (), DatabaseTest.c_unitTestStorageProviderStubID));
      _storageConfiguration.StorageGroups.Add (new StorageGroupElement (new TableInheritanceTestDomainAttribute (), TableInheritanceMappingTest.TableInheritanceTestDomainProviderID));

      _mappingLoaderConfiguration = new MappingLoaderConfiguration ();
      _queryConfiguration = new QueryConfiguration ("DomainObjects\\QueriesForStandardMapping.xml");
      DomainObjectsConfiguration.SetCurrent (new FakeDomainObjectsConfiguration (_mappingLoaderConfiguration, _storageConfiguration, _queryConfiguration));

      _mappingConfiguration = new MappingConfiguration (new MappingReflector (GetTypeDiscoveryService (GetType ().Assembly)));
      MappingConfiguration.SetCurrent (_mappingConfiguration);
    }

    public MappingConfiguration GetMappingConfiguration ()
    {
      return _mappingConfiguration;
    }

    public StorageConfiguration GetPersistenceConfiguration ()
    {
      return _storageConfiguration;
    }

    public FakeDomainObjectsConfiguration GetDomainObjectsConfiguration()
    {
      return new FakeDomainObjectsConfiguration (_mappingLoaderConfiguration, _storageConfiguration, _queryConfiguration);
    }
  }
}
