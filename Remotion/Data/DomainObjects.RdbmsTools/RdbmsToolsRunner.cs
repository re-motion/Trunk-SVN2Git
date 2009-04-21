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
using System.IO;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.RdbmsTools.SchemaGeneration;
using Remotion.Utilities;
using Remotion.Data.DomainObjects.Queries.Configuration;

namespace Remotion.Data.DomainObjects.RdbmsTools
{
  /// <summary>
  /// The <see cref="RdbmsToolsRunner"/> type contains the encapsulates the execution of the various functionality provided by the 
  /// <b>Remotion.Data.DomainObjects.RdbmsTools</b> assembly.
  /// </summary>
  [Serializable]
  public class RdbmsToolsRunner : AppDomainRunnerBase
  {
    public static RdbmsToolsRunner Create (RdbmsToolsParameter rdbmsToolsParameter)
    {
      AppDomainSetup appDomainSetup = CreateAppDomainSetup(rdbmsToolsParameter);
      return new RdbmsToolsRunner (appDomainSetup, rdbmsToolsParameter);
    }

    public static AppDomainSetup CreateAppDomainSetup (RdbmsToolsParameter rdbmsToolsParameter)
    {
      AppDomainSetup appDomainSetup = new AppDomainSetup();
      appDomainSetup.ApplicationName = "RdbmsTools";
      appDomainSetup.ApplicationBase = rdbmsToolsParameter.BaseDirectory;
      appDomainSetup.DynamicBase = Path.Combine (Path.GetTempPath(), "Remotion");
      if (!string.IsNullOrEmpty (rdbmsToolsParameter.ConfigFile))
      {
        appDomainSetup.ConfigurationFile = Path.GetFullPath (rdbmsToolsParameter.ConfigFile);
        if (!File.Exists (appDomainSetup.ConfigurationFile))
        {
          throw new FileNotFoundException (
              string.Format (
                  "The configuration file supplied by the 'config' parameter was not found.\r\nFile: {0}", 
                  appDomainSetup.ConfigurationFile),
              appDomainSetup.ConfigurationFile);
        }
      }
      return appDomainSetup;
    }

    private readonly RdbmsToolsParameter _rdbmsToolsParameter;

    protected RdbmsToolsRunner (AppDomainSetup appDomainSetup, RdbmsToolsParameter rdbmsToolsParameter)
        : base (appDomainSetup)
    {
      _rdbmsToolsParameter = rdbmsToolsParameter;
    }

    protected override void CrossAppDomainCallbackHandler ()
    {
      InitializeConfiguration();

      if ((_rdbmsToolsParameter.Mode & OperationMode.BuildSchema) != 0)
        BuildSchema();
    }

    protected virtual void InitializeConfiguration ()
    {
      DomainObjectsConfiguration.SetCurrent (
          new FakeDomainObjectsConfiguration (DomainObjectsConfiguration.Current.MappingLoader, GetPersistenceConfiguration(), new QueryConfiguration()));

      MappingConfiguration.SetCurrent (new MappingConfiguration (DomainObjectsConfiguration.Current.MappingLoader.CreateMappingLoader()));
    }

    protected StorageConfiguration GetPersistenceConfiguration ()
    {
      StorageConfiguration storageConfiguration = DomainObjectsConfiguration.Current.Storage;
      if (storageConfiguration.StorageProviderDefinitions.Count == 0)
      {
        ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = new ProviderCollection<StorageProviderDefinition>();
        RdbmsProviderDefinition providerDefinition = new RdbmsProviderDefinition ("Default", typeof (SqlProvider), "Initial Catalog=DatabaseName;");
        storageProviderDefinitionCollection.Add (providerDefinition);

        storageConfiguration = new StorageConfiguration (storageProviderDefinitionCollection, providerDefinition);
      }

      return storageConfiguration;
    }

    protected virtual void BuildSchema ()
    {
      Type sqlFileBuilderType = TypeUtility.GetType (_rdbmsToolsParameter.SchemaFileBuilderTypeName, true);
      FileBuilderBase.Build (
          sqlFileBuilderType,
          MappingConfiguration.Current,
          DomainObjectsConfiguration.Current.Storage,
          _rdbmsToolsParameter.SchemaOutputDirectory);
    }
  }
}
