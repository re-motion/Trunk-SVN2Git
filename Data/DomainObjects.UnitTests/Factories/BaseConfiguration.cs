/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Reflection;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.UnitTests.DomainObjects.TableInheritance;
using Remotion.Data.DomainObjects.UnitTests.DomainObjects.TableInheritance.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Factories
{
  public abstract class BaseConfiguration
  {
    public static AssemblyFinderTypeDiscoveryService GetTypeDiscoveryService (params Assembly[] rootAssemblies)
    {
      return new AssemblyFinderTypeDiscoveryService (new AssemblyFinder (ApplicationAssemblyFinderFilter.Instance, rootAssemblies));
    }

    private readonly PersistenceConfiguration _persistenceConfiguration;
    private readonly MappingLoaderConfiguration _mappingLoaderConfiguration;
    private readonly QueryConfiguration _queryConfiguration;
    private readonly MappingConfiguration _mappingConfiguration;

    protected BaseConfiguration ()
    {
      ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = StorageProviderDefinitionFactory.Create ();
      _persistenceConfiguration = new PersistenceConfiguration (storageProviderDefinitionCollection, storageProviderDefinitionCollection[DatabaseTest.DefaultStorageProviderID]);
      _persistenceConfiguration.StorageGroups.Add (new StorageGroupElement (new TestDomainAttribute (), DatabaseTest.c_testDomainProviderID));
      _persistenceConfiguration.StorageGroups.Add (new StorageGroupElement (new StorageProviderStubAttribute (), DatabaseTest.c_unitTestStorageProviderStubID));
      _persistenceConfiguration.StorageGroups.Add (new StorageGroupElement (new TableInheritanceTestDomainAttribute (), TableInheritanceMappingTest.TableInheritanceTestDomainProviderID));

      _mappingLoaderConfiguration = new MappingLoaderConfiguration ();
      _queryConfiguration = new QueryConfiguration ();
      DomainObjectsConfiguration.SetCurrent (new FakeDomainObjectsConfiguration (_mappingLoaderConfiguration, _persistenceConfiguration, _queryConfiguration));

      _mappingConfiguration = new MappingConfiguration (new MappingReflector (GetTypeDiscoveryService (GetType ().Assembly)));
      MappingConfiguration.SetCurrent (_mappingConfiguration);
    }

    public MappingConfiguration GetMappingConfiguration ()
    {
      return _mappingConfiguration;
    }

    public PersistenceConfiguration GetPersistenceConfiguration ()
    {
      return _persistenceConfiguration;
    }

    public FakeDomainObjectsConfiguration GetDomainObjectsConfiguration()
    {
      return new FakeDomainObjectsConfiguration (_mappingLoaderConfiguration, _persistenceConfiguration, _queryConfiguration);
    }
  }
}
