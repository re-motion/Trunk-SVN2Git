// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Development.UnitTesting.Data.SqlClient;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Persistence;

namespace Remotion.SecurityManager.UnitTests
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private const string c_testDomainConnectionString = "Integrated Security=SSPI;Initial Catalog=RemotionSecurityManager;Data Source=localhost";
    private const string c_masterConnectionString = "Integrated Security=SSPI;Initial Catalog=master;Data Source=localhost";

    [SetUp]
    public void SetUp()
    {
      try
      {
        ServiceLocator.SetLocatorProvider (() => null);

        ProviderCollection<StorageProviderDefinition> providers = new ProviderCollection<StorageProviderDefinition> ();
        providers.Add (new RdbmsProviderDefinition ("SecurityManager", typeof (SecurityManagerSqlProvider), c_testDomainConnectionString));
        StorageConfiguration storageConfiguration = new StorageConfiguration (providers, providers["SecurityManager"]);
        storageConfiguration.StorageGroups.Add (new StorageGroupElement (new SecurityManagerStorageGroupAttribute(), "SecurityManager"));

        DomainObjectsConfiguration.SetCurrent (
            new FakeDomainObjectsConfiguration (
                new MappingLoaderConfiguration(),
                storageConfiguration,
                new QueryConfiguration (GetFullPath (@"SecurityManagerQueries.xml"))));

        var rootAssemblyFinder = new FixedRootAssemblyFinder (new RootAssembly (typeof (BaseSecurityManagerObject).Assembly, true));
        var assemblyLoader = new FilteringAssemblyLoader (ApplicationAssemblyLoaderFilter.Instance);
        var assemblyFinder = new AssemblyFinder (rootAssemblyFinder, assemblyLoader);
        ITypeDiscoveryService typeDiscoveryService = new AssemblyFinderTypeDiscoveryService (assemblyFinder);

        MappingConfiguration.SetCurrent (new MappingConfiguration (new MappingReflector (typeDiscoveryService)));

        SqlConnection.ClearAllPools();

        DatabaseAgent masterAgent = new DatabaseAgent (c_masterConnectionString);
        masterAgent.ExecuteBatch ("SecurityManagerCreateDB.sql", false);
        DatabaseAgent databaseAgent = new DatabaseAgent (c_testDomainConnectionString);
        databaseAgent.ExecuteBatch ("SecurityManagerSetupDB.sql", true);
        databaseAgent.ExecuteBatch ("SecurityManagerSetupConstraints.sql", true);
        databaseAgent.ExecuteBatch ("SecurityManagerSetupDBSpecialTables.sql", true);
      }
      catch (Exception e)
      {
        Console.WriteLine (e);
      }
    }

    [TearDown]
    public void TearDown()
    {
      SqlConnection.ClearAllPools();
    }

    private string GetFullPath (string fileName)
    {
      return Path.Combine (AppDomain.CurrentDomain.BaseDirectory, fileName);
    }
  }
}
