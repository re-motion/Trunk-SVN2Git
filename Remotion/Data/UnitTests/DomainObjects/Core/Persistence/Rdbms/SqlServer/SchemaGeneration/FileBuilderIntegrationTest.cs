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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Development.UnitTesting.Resources;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class FileBuilderIntegrationTest : SchemaGenerationTestBase
  {
    private string _firstStorageProviderSetupDBScript;
    private string _secondStorageProviderSetupDBScript;
    private string _thirdStorageProviderSetupDBScript;
    private string _firstStorageProviderTearDownDBScript;
    private string _secondStorageProviderTearDownDBScript;
    private string _thirdStorageProviderTearDownDBScript;
    private FileBuilder _sqlFileBuilderForFirstStorageProvider;
    private FileBuilder _sqlFileBuilderForSecondStorageProvider;
    private FileBuilder _sqlFileBuilderForThirdStorageProvider;
    private ClassDefinition[] _classesInFirstStorageProvider;
    private ClassDefinition[] _classesInSecondStorageProvider;
    private ClassDefinition[] _classesInThirdStorageProvider;

    public override void SetUp ()
    {
      base.SetUp();

      var tableBuilder = new SqlTableScriptBuilder();
      var viewBuilder = new ExtendedViewBuilder();
      var constraintBuilder = new SqlConstraintScriptBuilder();
      var indexBuilder = new SqlIndexScriptBuilder();
      var synonymBuilder = new SqlSynonymScriptBuilder();

      _sqlFileBuilderForFirstStorageProvider =
          new FileBuilder (
              () => new CompositeScriptBuilder (
                        SchemaGenerationFirstStorageProviderDefinition,
                        SqlDialect.Instance,
                        tableBuilder,
                        constraintBuilder,
                        viewBuilder,
                        indexBuilder,
                        synonymBuilder),
              new EntityDefinitionProvider());
      _sqlFileBuilderForSecondStorageProvider =
          new FileBuilder (
              () => new CompositeScriptBuilder (
                        SchemaGenerationSecondStorageProviderDefinition,
                        SqlDialect.Instance,
                        tableBuilder,
                        constraintBuilder,
                        viewBuilder,
                        indexBuilder,
                        synonymBuilder),
              new EntityDefinitionProvider());
      _sqlFileBuilderForThirdStorageProvider =
          new ExtendedFileBuilder (
              () => new CompositeScriptBuilder (
                        SchemaGenerationThirdStorageProviderDefinition,
                        SqlDialect.Instance,
                        tableBuilder,
                        constraintBuilder,
                        viewBuilder,
                        indexBuilder,
                        synonymBuilder));

      _firstStorageProviderSetupDBScript = ResourceUtility.GetResourceString (GetType(), "TestData.SetupDB_FirstStorageProvider.sql");
      _firstStorageProviderTearDownDBScript = ResourceUtility.GetResourceString (GetType(), "TestData.TearDownDB_FirstStorageProvider.sql");

      _secondStorageProviderSetupDBScript = ResourceUtility.GetResourceString (GetType(), "TestData.SetupDB_SecondStorageProvider.sql");
      _secondStorageProviderTearDownDBScript = ResourceUtility.GetResourceString (GetType(), "TestData.TearDownDB_SecondStorageProvider.sql");

      _thirdStorageProviderSetupDBScript = ResourceUtility.GetResourceString (GetType(), "TestData.SetupDB_ThirdStorageProvider.sql");
      _thirdStorageProviderTearDownDBScript = ResourceUtility.GetResourceString (GetType(), "TestData.TearDownDB_ThirdStorageProvider.sql");

      _classesInFirstStorageProvider = MappingConfiguration.ClassDefinitions.Values
          .Where (cd => cd.StorageEntityDefinition.StorageProviderDefinition == SchemaGenerationFirstStorageProviderDefinition)
          .ToArray();
      _classesInSecondStorageProvider = MappingConfiguration.ClassDefinitions.Values
          .Where (cd => cd.StorageEntityDefinition.StorageProviderDefinition == SchemaGenerationSecondStorageProviderDefinition)
          .ToArray();
      _classesInThirdStorageProvider = MappingConfiguration.ClassDefinitions.Values
          .Where (cd => cd.StorageEntityDefinition.StorageProviderDefinition == SchemaGenerationThirdStorageProviderDefinition)
          .ToArray();
    }

    [Test]
    public void GetScriptForFirstStorageProvider ()
    {
      var scripts = _sqlFileBuilderForFirstStorageProvider.GetScript (_classesInFirstStorageProvider);

      Assert.AreEqual (_firstStorageProviderSetupDBScript, scripts.CreateScript);
      Assert.AreEqual (_firstStorageProviderTearDownDBScript, scripts.DropScript);
    }

    [Test]
    public void GetScriptForSecondStorageProvider ()
    {
      var scripts = _sqlFileBuilderForSecondStorageProvider.GetScript (_classesInSecondStorageProvider);

      Assert.AreEqual (_secondStorageProviderSetupDBScript, scripts.CreateScript);
      Assert.AreEqual (_secondStorageProviderTearDownDBScript, scripts.DropScript);
    }

    [Test]
    public void GetScriptForThirdStorageProvider ()
    {
      var scripts = _sqlFileBuilderForThirdStorageProvider.GetScript (_classesInThirdStorageProvider);

      Assert.AreEqual (_thirdStorageProviderSetupDBScript, scripts.CreateScript);
      Assert.AreEqual (_thirdStorageProviderTearDownDBScript, scripts.DropScript);
    }
  }
}