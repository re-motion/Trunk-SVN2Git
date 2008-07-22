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

    protected PersistenceConfiguration GetPersistenceConfiguration ()
    {
      PersistenceConfiguration persistenceConfiguration = DomainObjectsConfiguration.Current.Storage;
      if (persistenceConfiguration.StorageProviderDefinition == null)
      {
        ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = new ProviderCollection<StorageProviderDefinition>();
        RdbmsProviderDefinition providerDefinition = new RdbmsProviderDefinition ("Default", typeof (SqlProvider), "Initial Catalog=DatabaseName;");
        storageProviderDefinitionCollection.Add (providerDefinition);

        persistenceConfiguration = new PersistenceConfiguration (storageProviderDefinitionCollection, providerDefinition);
      }

      return persistenceConfiguration;
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
