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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Development.UnitTesting.Data.SqlClient;
using Remotion.Development.UnitTesting.Resources;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class ScriptGeneratorDatabaseIntegrationTest : SchemaGenerationTestBase
  {
    private ScriptGenerator _scriptGeneratorForFirstStorageProvider;
    private ScriptGenerator _scriptGeneratorForSecondStorageProvider;
    private ScriptGenerator _scriptGeneratorForThirdStorageProvider;

    public override void SetUp ()
    {
      base.SetUp();

      _scriptGeneratorForFirstStorageProvider = new ScriptGenerator (
          pd => pd.Factory.CreateSchemaScriptBuilder (pd),
          new EntityDefinitionProvider(),
          new ScriptToStringConverter());

      _scriptGeneratorForSecondStorageProvider = new ScriptGenerator (
          pd => pd.Factory.CreateSchemaScriptBuilder (pd),
          new EntityDefinitionProvider(),
          new ScriptToStringConverter());

      _scriptGeneratorForThirdStorageProvider = new ScriptGenerator (
          pd => new SqlDatabaseSelectionScriptElementBuilder (
                    new CompositeScriptBuilder (
                        SchemaGenerationThirdStorageProviderDefinition,
                        CreateTableBuilder(),
                        CreateConstraintBuilder(),
                        CreateExtendedViewBuilder(),
                        CreateIndexBuilder(),
                        CreateSynonymBuilder()),
                    SchemaGenerationThirdStorageProviderDefinition.ConnectionString),
          new ExtendedEntityDefinitionProvider(),
          new ScriptToStringConverter());
    }

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();

      var createDBScript = ResourceUtility.GetResourceString (GetType(), "TestData.SchemaGeneration_CreateDB.sql");

      var masterAgent = new DatabaseAgent (MasterConnectionString);
      masterAgent.ExecuteBatchString (createDBScript, false);
    }

    [Test]
    public void ExecuteScriptForFirstStorageProvider ()
    {
      DatabaseAgent.SetConnectionString (SchemaGenerationConnectionString1);

      var scripts = _scriptGeneratorForFirstStorageProvider.GetScripts (MappingConfiguration.GetTypeDefinitions ())
          .Single (s => s.StorageProviderDefinition == SchemaGenerationFirstStorageProviderDefinition);

      DatabaseAgent.ExecuteBatchString (scripts.TearDownScript + scripts.SetUpScript, false);
    }

    [Test]
    public void ExecuteScriptForSecondStorageProvider ()
    {
      DatabaseAgent.SetConnectionString (SchemaGenerationConnectionString2);

      var scripts = _scriptGeneratorForSecondStorageProvider.GetScripts (MappingConfiguration.GetTypeDefinitions ())
          .Single (s => s.StorageProviderDefinition == SchemaGenerationSecondStorageProviderDefinition);

      DatabaseAgent.ExecuteBatchString (scripts.TearDownScript + scripts.SetUpScript, false);
    }

    [Test]
    public void ExecuteScriptForThirdStorageProvider ()
    {
      DatabaseAgent.SetConnectionString (SchemaGenerationConnectionString3);

      var scripts = _scriptGeneratorForThirdStorageProvider.GetScripts (MappingConfiguration.GetTypeDefinitions ())
          .Single (s => s.StorageProviderDefinition == SchemaGenerationThirdStorageProviderDefinition);

      DatabaseAgent.ExecuteBatchString (scripts.TearDownScript + scripts.SetUpScript, false);
    }
  }
}