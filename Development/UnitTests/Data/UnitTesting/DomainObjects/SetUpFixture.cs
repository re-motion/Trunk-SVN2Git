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
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Development.UnitTests.Data.UnitTesting.DomainObjects.TestDomain;
using Remotion.Reflection;

namespace Remotion.Development.UnitTests.Data.UnitTesting.DomainObjects
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
        providers.Add (new RdbmsProviderDefinition ("Development.Data.DomainObjects", typeof (RdbmsProviderDefinition), "ConnectionString"));
        StorageConfiguration storageConfiguration = new StorageConfiguration (providers, providers["Development.Data.DomainObjects"]);

        DomainObjectsConfiguration.SetCurrent (
            new FakeDomainObjectsConfiguration (
                new MappingLoaderConfiguration(),
                storageConfiguration,
                new QueryConfiguration()));

        ITypeDiscoveryService typeDiscoveryService = new AssemblyFinderTypeDiscoveryService (
            new AssemblyFinder (ApplicationAssemblyFinderFilter.Instance, typeof (SimpleDomainObject).Assembly));
        MappingConfiguration.SetCurrent (new MappingConfiguration (new MappingReflector (typeDiscoveryService)));
      }
      catch (Exception e)
      {
        Console.WriteLine (e);
        throw;
      }
    }
  }
}