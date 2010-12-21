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
using System.Diagnostics;
using System.Reflection;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2005;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGenerationTestDomain;
using Remotion.Development.UnitTesting.Reflection.TypeDiscovery;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  public class SchemaGenerationConfiguration
  {
    private static SchemaGenerationConfiguration s_instance;

    public static SchemaGenerationConfiguration Instance
    {
      get
      {
        if (s_instance == null)
        {
          Debugger.Break ();
          throw new InvalidOperationException ("SchemaGenerationConfiguration has not been Initialized by invoking Initialize()");
        }
        return s_instance;
      }
    }

    public static void Initialize ()
    {
      s_instance = new SchemaGenerationConfiguration ();
    }

    private readonly StorageConfiguration _storageConfiguration;
    private readonly MappingLoaderConfiguration _mappingLoaderConfiguration;
    private readonly MappingConfiguration _mappingConfiguration;
    private readonly QueryConfiguration _queryConfiguration;
    private readonly FakeDomainObjectsConfiguration _domainObjectsConfiguration;

    public SchemaGenerationConfiguration ()
    {
      var storageProviderDefinitionCollection = new ProviderCollection<StorageProviderDefinition>
                                                {
                                                    new RdbmsProviderDefinition (
                                                        DatabaseTest.SchemaGenerationFirstStorageProviderID,
                                                        typeof (SqlStorageObjectFactory),
                                                        DatabaseTest.TestDomainConnectionString),
                                                    new RdbmsProviderDefinition (
                                                        DatabaseTest.SchemaGenerationSecondStorageProviderID,
                                                        typeof (SqlStorageObjectFactory),
                                                        DatabaseTest.TestDomainConnectionString),
                                                    new RdbmsProviderDefinition (
                                                        DatabaseTest.SchemaGenerationInternalStorageProviderID,
                                                        typeof (SqlStorageObjectFactory),
                                                        DatabaseTest.TestDomainConnectionString)
                                                };

      _storageConfiguration = new StorageConfiguration (
          storageProviderDefinitionCollection, 
          null);
      _storageConfiguration.StorageGroups.Add (
          new StorageGroupElement (
              new FirstStorageGroupAttribute (),
              DatabaseTest.SchemaGenerationFirstStorageProviderID));
      _storageConfiguration.StorageGroups.Add (
          new StorageGroupElement (
              new SecondStorageGroupAttribute (),
              DatabaseTest.SchemaGenerationSecondStorageProviderID));
      _storageConfiguration.StorageGroups.Add (
          new StorageGroupElement (
              new InternalStorageGroupAttribute (),
              DatabaseTest.SchemaGenerationInternalStorageProviderID));

      _mappingLoaderConfiguration = new MappingLoaderConfiguration ();
      _queryConfiguration = new QueryConfiguration ();

      var typeDiscoveryService = GetTypeDiscoveryService (GetType ().Assembly);

      _mappingConfiguration = new MappingConfiguration (
          new MappingReflector (typeDiscoveryService), 
          new PersistenceModelLoader (new StorageProviderDefinitionFinder (_storageConfiguration)));
      _domainObjectsConfiguration = new FakeDomainObjectsConfiguration (_mappingLoaderConfiguration, _storageConfiguration, _queryConfiguration);
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
      return _domainObjectsConfiguration;
    }

    public ITypeDiscoveryService GetTypeDiscoveryService (params Assembly[] rootAssemblies)
    {
      var baseTypeDiscoveryService = new FixedTypeDiscoveryService (Assembly.GetExecutingAssembly ().GetTypes());

      return FilteringTypeDiscoveryService.CreateFromNamespaceWhitelist(
          baseTypeDiscoveryService,
          "Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGenerationTestDomain");
    }

  }
}