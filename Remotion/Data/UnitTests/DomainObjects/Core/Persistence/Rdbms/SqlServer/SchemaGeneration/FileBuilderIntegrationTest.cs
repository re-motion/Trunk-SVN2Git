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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration;
using Remotion.Development.UnitTesting.Resources;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class FileBuilderIntegrationTest : SchemaGenerationTestBase
  {
    private string _firstStorageProviderSetupDBScript;
    private string _secondStorageProviderSetupDBScript;
    private string _thirdStorageProviderSetupDBScript;
    private FileBuilder _sqlFileBuilderForFirstStorageProvider;
    private FileBuilder _sqlFileBuilderForSecondStorageProvider;
    private FileBuilder _sqlFileBuilderForThirdStorageProvider;
    private ClassDefinition[] _classesInFirstStorageProvider;
    private ClassDefinition[] _classesInSecondStorageProvider;
    private ClassDefinition[] _classesInThirdStorageProvider;

    public override void SetUp ()
    {
      base.SetUp();

      var tableBuilder = new SqlTableBuilder();
      var viewBuilder = new ExtendedViewBuilder();
      var constraintBuilder = new SqlConstraintBuilder();
      var indexBuilder = new SqlIndexBuilder();

      _sqlFileBuilderForFirstStorageProvider =
          new FileBuilder (
              new SqlScriptBuilder (SchemaGenerationFirstStorageProviderDefinition, tableBuilder, viewBuilder, constraintBuilder, indexBuilder));
      _sqlFileBuilderForSecondStorageProvider =
          new FileBuilder (
              new SqlScriptBuilder (SchemaGenerationSecondStorageProviderDefinition, tableBuilder, viewBuilder, constraintBuilder, indexBuilder));
      _sqlFileBuilderForThirdStorageProvider =
          new ExtendedFileBuilder (
              new SqlScriptBuilder (SchemaGenerationThirdStorageProviderDefinition, tableBuilder, viewBuilder, constraintBuilder, indexBuilder));

      _firstStorageProviderSetupDBScript = ResourceUtility.GetResourceString (GetType(), "TestData.SetupDB_FirstStorageProvider.sql");
      _secondStorageProviderSetupDBScript = ResourceUtility.GetResourceString (GetType(), "TestData.SetupDB_SecondStorageProvider.sql");
      _thirdStorageProviderSetupDBScript = ResourceUtility.GetResourceString (GetType(), "TestData.SetupDB_ThirdStorageProvider.sql");

      _classesInFirstStorageProvider = MappingConfiguration.ClassDefinitions.Cast<ClassDefinition>()
          .Where (cd => cd.StorageEntityDefinition.StorageProviderDefinition == SchemaGenerationFirstStorageProviderDefinition)
          .ToArray();
      _classesInSecondStorageProvider = MappingConfiguration.ClassDefinitions.Cast<ClassDefinition>()
          .Where (cd => cd.StorageEntityDefinition.StorageProviderDefinition == SchemaGenerationSecondStorageProviderDefinition)
          .ToArray();
      _classesInThirdStorageProvider = MappingConfiguration.ClassDefinitions.Cast<ClassDefinition>()
          .Where (cd => cd.StorageEntityDefinition.StorageProviderDefinition == SchemaGenerationThirdStorageProviderDefinition)
          .ToArray();
    }

    [Test]
    public void GetScriptForFirstStorageProvider ()
    {
      Assert.AreEqual (_firstStorageProviderSetupDBScript, _sqlFileBuilderForFirstStorageProvider.GetScript (_classesInFirstStorageProvider));
    }

    [Test]
    public void GetScriptForSecondStorageProvider ()
    {
      Assert.AreEqual (_secondStorageProviderSetupDBScript, _sqlFileBuilderForSecondStorageProvider.GetScript (_classesInSecondStorageProvider));
    }

    [Test]
    public void GetScriptForThirdStorageProvider ()
    {
      Assert.AreEqual (_thirdStorageProviderSetupDBScript, _sqlFileBuilderForThirdStorageProvider.GetScript (_classesInThirdStorageProvider));
    }
  }
}