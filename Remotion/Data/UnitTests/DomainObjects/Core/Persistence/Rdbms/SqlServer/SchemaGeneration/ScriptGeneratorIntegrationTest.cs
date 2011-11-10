// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Development.UnitTesting.Resources;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class ScriptGeneratorIntegrationTest : SchemaGenerationTestBase
  {
    private string _firstStorageProviderSetupDBScript;
    private string _secondStorageProviderSetupDBScript;
    private string _thirdStorageProviderSetupDBScript;
    private string _firstStorageProviderTearDownDBScript;
    private string _secondStorageProviderTearDownDBScript;
    private string _thirdStorageProviderTearDownDBScript;
    private ScriptGenerator _standardScriptGenerator;
    private ScriptGenerator _extendedScriptGenerator;

    public override void SetUp ()
    {
      base.SetUp();

      _standardScriptGenerator = new ScriptGenerator (
          pd => pd.Factory.CreateSchemaScriptBuilder (pd),
          new RdbmsStorageEntityDefinitionProvider(),
          new ScriptToStringConverter());

      _extendedScriptGenerator = new ScriptGenerator (
          pd => new SqlDatabaseSelectionScriptElementBuilder (
                    new CompositeScriptBuilder (
                        SchemaGenerationThirdStorageProviderDefinition,
                        CreateTableBuilder(),
                        CreateConstraintBuilder(),
                        CreateExtendedViewBuilder(),
                        CreateIndexBuilder(),
                        CreateSynonymBuilder()),
                    SchemaGenerationThirdStorageProviderDefinition.ConnectionString),
          new RdbmsStorageExtendedEntityDefinitionProvider(),
          new ScriptToStringConverter());

      _firstStorageProviderSetupDBScript = ResourceUtility.GetResourceString (GetType(), "TestData.SetupDB_FirstStorageProvider.sql");
      _firstStorageProviderTearDownDBScript = ResourceUtility.GetResourceString (GetType(), "TestData.TearDownDB_FirstStorageProvider.sql");

      _secondStorageProviderSetupDBScript = ResourceUtility.GetResourceString (GetType(), "TestData.SetupDB_SecondStorageProvider.sql");
      _secondStorageProviderTearDownDBScript = ResourceUtility.GetResourceString (GetType(), "TestData.TearDownDB_SecondStorageProvider.sql");

      _thirdStorageProviderSetupDBScript = ResourceUtility.GetResourceString (GetType(), "TestData.SetupDB_ThirdStorageProvider.sql");
      _thirdStorageProviderTearDownDBScript = ResourceUtility.GetResourceString (GetType(), "TestData.TearDownDB_ThirdStorageProvider.sql");
    }

    [Test]
    public void GetScriptForFirstStorageProvider ()
    {
      var scripts = _standardScriptGenerator.GetScripts (MappingConfiguration.GetTypeDefinitions ())
          .Single (s => s.StorageProviderDefinition == SchemaGenerationFirstStorageProviderDefinition);

      Assert.AreEqual (_firstStorageProviderSetupDBScript, scripts.SetUpScript);
      Assert.AreEqual (_firstStorageProviderTearDownDBScript, scripts.TearDownScript);
    }

    [Test]
    public void GetScriptForSecondStorageProvider ()
    {
      var scripts = _standardScriptGenerator.GetScripts (MappingConfiguration.GetTypeDefinitions ())
          .Single (s => s.StorageProviderDefinition == SchemaGenerationSecondStorageProviderDefinition);

      Assert.AreEqual (_secondStorageProviderSetupDBScript, scripts.SetUpScript);
      Assert.AreEqual (_secondStorageProviderTearDownDBScript, scripts.TearDownScript);
    }

    [Test]
    public void GetScriptForThirdStorageProvider ()
    {
      var scripts = _extendedScriptGenerator.GetScripts (MappingConfiguration.GetTypeDefinitions ())
          .Single (s => s.StorageProviderDefinition == SchemaGenerationThirdStorageProviderDefinition);

      Assert.AreEqual (_thirdStorageProviderSetupDBScript, scripts.SetUpScript);
      Assert.AreEqual (_thirdStorageProviderTearDownDBScript, scripts.TearDownScript);
    }
  }
}