/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.ComponentModel.Design;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  public class StandardConfiguration
  {
    public const string ConnectionString = "Integrated Security=SSPI;Initial Catalog=PerformanceTestDomain;Data Source=localhost";

    public static void Initialize ()
    {
      ProviderCollection<StorageProviderDefinition> providers = new ProviderCollection<StorageProviderDefinition> ();
      providers.Add (new RdbmsProviderDefinition ("PerformanceTestDomain", typeof (SqlProvider), ConnectionString));
      StorageConfiguration storageConfiguration = new StorageConfiguration (providers, providers["PerformanceTestDomain"]);

      DomainObjectsConfiguration.SetCurrent (new FakeDomainObjectsConfiguration (new MappingLoaderConfiguration (), storageConfiguration, new QueryConfiguration()));


      ITypeDiscoveryService typeDiscoveryService = new AssemblyFinderTypeDiscoveryService (
            new AssemblyFinder (ApplicationAssemblyFinderFilter.Instance, typeof (StandardConfiguration).Assembly));
      MappingConfiguration mappingConfiguration = new MappingConfiguration (new MappingReflector (typeDiscoveryService));
      MappingConfiguration.SetCurrent (mappingConfiguration);
    }
  }
}
