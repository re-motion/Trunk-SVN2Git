// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 

using System;
using System.ComponentModel.Design;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Practices.ServiceLocation;
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
using Remotion.Development.UnitTesting.Data.SqlClient;
using Remotion.Reflection;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Persistence;
using Remotion.ServiceLocation;

namespace Remotion.SecurityManager.UnitTests
{
  [SetUpFixture]
  public class SetUpFixture
  {
    public static string TestDomainConnectionString
    {
      get { return string.Format ("Integrated Security=SSPI;Initial Catalog=RemotionSecurityManager;Data Source={0}", DatabaseConfiguration.DataSource); }
    }

    public static string MasterConnectionString
    {
      get { return string.Format ("Integrated Security=SSPI;Initial Catalog=master;Data Source={0}", DatabaseConfiguration.DataSource); }
    }

    [SetUp]
    public void SetUp ()
    {
      try
      {
        var serviceLocator = new DefaultServiceLocator();
        serviceLocator.Register (typeof (IGlobalAccessTypeCache), () => new NullGlobalAccessTypeCache());
        ServiceLocator.SetLocatorProvider (() => serviceLocator);

        ProviderCollection<StorageProviderDefinition> providers = new ProviderCollection<StorageProviderDefinition>();
        providers.Add (new RdbmsProviderDefinition ("SecurityManager", new SecurityManagerSqlStorageObjectFactory(), TestDomainConnectionString));
        StorageConfiguration storageConfiguration = new StorageConfiguration (providers, providers["SecurityManager"]);
        storageConfiguration.StorageGroups.Add (new StorageGroupElement (new SecurityManagerStorageGroupAttribute(), "SecurityManager"));

        DomainObjectsConfiguration.SetCurrent (
            new FakeDomainObjectsConfiguration (new MappingLoaderConfiguration(), storageConfiguration, new QueryConfiguration()));

        var rootAssemblyFinder = new FixedRootAssemblyFinder (new RootAssembly (typeof (BaseSecurityManagerObject).Assembly, true));
        var assemblyLoader = new FilteringAssemblyLoader (ApplicationAssemblyLoaderFilter.Instance);
        var assemblyFinder = new CachingAssemblyFinderDecorator (new AssemblyFinder (rootAssemblyFinder, assemblyLoader));
        ITypeDiscoveryService typeDiscoveryService = new AssemblyFinderTypeDiscoveryService (assemblyFinder);

        MappingConfiguration.SetCurrent (
            new MappingConfiguration (
                new MappingReflector (
                    typeDiscoveryService,
                    new ClassIDProvider(),
                    new DomainModelConstraintProvider(),
                    new ReflectionBasedMemberInfoNameResolver(),
                    MappingReflector.CreateDomainObjectCreator()),
                new PersistenceModelLoader (new StorageGroupBasedStorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage))));

        SqlConnection.ClearAllPools();

        DatabaseAgent masterAgent = new DatabaseAgent (MasterConnectionString);
        masterAgent.ExecuteBatchFile ("SecurityManagerCreateDB.sql", false, DatabaseConfiguration.GetReplacementDictionary());
        DatabaseAgent databaseAgent = new DatabaseAgent (TestDomainConnectionString);
        databaseAgent.ExecuteBatchFile ("SecurityManagerSetupDB.sql", true);
        databaseAgent.ExecuteBatchFile ("SecurityManagerSetupConstraints.sql", true);
        databaseAgent.ExecuteBatchFile ("SecurityManagerSetupDBSpecialTables.sql", true);
      }
      catch (Exception e)
      {
        Console.WriteLine (e);
      }
    }

    [TearDown]
    public void TearDown ()
    {
      SqlConnection.ClearAllPools();
    }

    private string GetFullPath (string fileName)
    {
      return Path.Combine (AppDomain.CurrentDomain.BaseDirectory, fileName);
    }
  }
}