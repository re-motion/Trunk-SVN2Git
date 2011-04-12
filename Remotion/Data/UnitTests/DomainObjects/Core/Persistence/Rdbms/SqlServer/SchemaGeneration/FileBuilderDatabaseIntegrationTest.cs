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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Development.UnitTesting.Data.SqlClient;
using Remotion.Development.UnitTesting.Resources;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class FileBuilderDatabaseIntegrationTest : SchemaGenerationTestBase
  {
    private string _firstStorageProviderSetupDBScript;
    private string _secondStorageProviderSetupDBScript;
    private FileBuilder _sqlFileBuilderForFirstStorageProvider;
    private FileBuilder _sqlFileBuilderForSecondStorageProvider;

    public override void SetUp ()
    {
      base.SetUp();

      _sqlFileBuilderForFirstStorageProvider = new FileBuilder (SchemaGenerationFirstStorageProviderDefinition);
      _sqlFileBuilderForSecondStorageProvider = new FileBuilder (SchemaGenerationSecondStorageProviderDefinition);
      _firstStorageProviderSetupDBScript = ResourceUtility.GetResourceString (GetType (), "TestData.SetupDB_FirstStorageProvider.sql");
      _secondStorageProviderSetupDBScript = ResourceUtility.GetResourceString (GetType (), "TestData.SetupDB_SecondStorageProvider.sql");
    }

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();

      var createDBScript = ResourceUtility.GetResourceString (GetType(), "TestData.SchemaGeneration_CreateDB.sql");

      var masterAgent = new DatabaseAgent (DatabaseTest.MasterConnectionString);
      masterAgent.ExecuteBatchString (createDBScript, false);

    }

    [Test]
    public void ExecuteScriptForFirstStorageProvider ()
    {
      DatabaseAgent.SetConnectionString (DatabaseTest.SchemaGenerationConnectionString1);

      var sqlScript =
          _sqlFileBuilderForFirstStorageProvider.GetScript (
              FileBuilder.GetClassesInStorageProvider (MappingConfiguration.ClassDefinitions, SchemaGenerationFirstStorageProviderDefinition));

      DatabaseAgent.ExecuteBatchString (sqlScript, false);
    }

    [Test]
    public void ExecuteScriptForSecondStorageProvider ()
    {
      DatabaseAgent.SetConnectionString (DatabaseTest.SchemaGenerationConnectionString2);

      var sqlScript =
          _sqlFileBuilderForSecondStorageProvider.GetScript (
              FileBuilder.GetClassesInStorageProvider (MappingConfiguration.ClassDefinitions, SchemaGenerationSecondStorageProviderDefinition));

      DatabaseAgent.ExecuteBatchString (sqlScript, false);
    }

    [Test]
    public void GetScriptForFirstStorageProvider ()
    {
      Assert.AreEqual (
          _firstStorageProviderSetupDBScript,
          _sqlFileBuilderForFirstStorageProvider.GetScript (
              FileBuilder.GetClassesInStorageProvider (MappingConfiguration.ClassDefinitions, SchemaGenerationFirstStorageProviderDefinition)));
    }

    [Test]
    public void GetScriptForSecondStorageProvider ()
    {
      Assert.AreEqual (
          _secondStorageProviderSetupDBScript,
          _sqlFileBuilderForSecondStorageProvider.GetScript (
              FileBuilder.GetClassesInStorageProvider (MappingConfiguration.ClassDefinitions, SchemaGenerationSecondStorageProviderDefinition)));
    }
  }
}