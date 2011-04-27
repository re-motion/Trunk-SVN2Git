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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Development.UnitTesting.Resources;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class CompositeScriptBuilderTest : SchemaGenerationTestBase
  {
    private CompositeScriptBuilder _scriptBuilderForFirstStorageProvider;
    private string _firstStorageProviderSetupDBScriptWithoutTables;
    private string _firstStorageProviderTearDownDBScriptWithoutTables;
    private IScriptBuilder _tableBuilderMock;
    private IScriptBuilder _viewBuilderMock;
    private IScriptBuilder _constraintBuilderMock;
    private IScriptBuilder _indexBuilderMock;
    private IScriptBuilder _synonymBuilderMock;
    
    public override void SetUp ()
    {
      base.SetUp();

      _tableBuilderMock = MockRepository.GenerateStrictMock<IScriptBuilder>();
      _viewBuilderMock = MockRepository.GenerateStrictMock<IScriptBuilder>();
      _constraintBuilderMock = MockRepository.GenerateStrictMock<IScriptBuilder>();
      _indexBuilderMock = MockRepository.GenerateStrictMock<IScriptBuilder>();
      _synonymBuilderMock = MockRepository.GenerateStrictMock<IScriptBuilder>();

      _scriptBuilderForFirstStorageProvider = new CompositeScriptBuilder (
          SchemaGenerationFirstStorageProviderDefinition, SqlDialect.Instance, _tableBuilderMock, _constraintBuilderMock, _viewBuilderMock, _indexBuilderMock, _synonymBuilderMock);
      _firstStorageProviderSetupDBScriptWithoutTables = ResourceUtility.GetResourceString (
          typeof (FileBuilderIntegrationTest), "TestData.SetupDB_FirstStorageProviderWithoutTables.sql");
      _firstStorageProviderTearDownDBScriptWithoutTables = ResourceUtility.GetResourceString (
          typeof (FileBuilderIntegrationTest), "TestData.TearDownDB_FirstStorageProviderWithoutTables.sql");
    }

    [Test]
    public void GetCreateScript ()
    {
      var entityDefinition1 = MockRepository.GenerateStub<IEntityDefinition>();
      var entityDefinition2 = MockRepository.GenerateStub<IEntityDefinition>();

      _tableBuilderMock.Expect (mock => mock.AddEntityDefinition (entityDefinition1));
      _tableBuilderMock.Expect (mock => mock.AddEntityDefinition (entityDefinition2));
      _tableBuilderMock.Expect (mock => mock.GetCreateScript ()).Return("CREATE TABLES\r\n");
      _tableBuilderMock.Replay ();

      _viewBuilderMock.Expect (mock => mock.AddEntityDefinition (entityDefinition1));
      _viewBuilderMock.Expect (mock => mock.AddEntityDefinition (entityDefinition2));
      _viewBuilderMock.Expect (mock => mock.GetCreateScript ()).Return ("CREATE VIEWS\r\n");
      _viewBuilderMock.Replay ();

      _constraintBuilderMock.Expect (mock => mock.AddEntityDefinition (entityDefinition1));
      _constraintBuilderMock.Expect (mock => mock.AddEntityDefinition (entityDefinition2));
      _constraintBuilderMock.Expect (mock => mock.GetCreateScript ()).Return ("CREATE CONSTRAINTS\r\n");
      _constraintBuilderMock.Replay ();

      _indexBuilderMock.Expect (mock => mock.AddEntityDefinition (entityDefinition1));
      _indexBuilderMock.Expect (mock => mock.AddEntityDefinition (entityDefinition2));
      _indexBuilderMock.Expect (mock => mock.GetCreateScript ()).Return ("CREATE INDEXES\r\n");
      _indexBuilderMock.Replay ();

      _synonymBuilderMock.Expect (mock => mock.AddEntityDefinition (entityDefinition1));
      _synonymBuilderMock.Expect (mock => mock.AddEntityDefinition (entityDefinition2));
      _synonymBuilderMock.Expect (mock => mock.GetCreateScript ()).Return ("CREATE SYNONYMS\r\n");
      _synonymBuilderMock.Replay ();
      
      _scriptBuilderForFirstStorageProvider.AddEntityDefinition (entityDefinition1);
      _scriptBuilderForFirstStorageProvider.AddEntityDefinition (entityDefinition2);

      var createScriptResult = _scriptBuilderForFirstStorageProvider.GetCreateScript ();

      _tableBuilderMock.VerifyAllExpectations ();
      _viewBuilderMock.VerifyAllExpectations ();
      _constraintBuilderMock.VerifyAllExpectations ();
      _indexBuilderMock.VerifyAllExpectations ();
      _synonymBuilderMock.VerifyAllExpectations ();

      var expectedResult =
          "USE SchemaGenerationTestDomain1\r\n"
          +"GO\r\n\r\n"
          +"CREATE TABLES\r\n"
          +"GO\r\n\r\n"
          +"CREATE CONSTRAINTS\r\n"
          +"GO\r\n\r\n"
          +"CREATE VIEWS\r\n"
          +"GO\r\n\r\n"
          +"CREATE INDEXES\r\n"
          +"GO\r\n\r\n"
          +"CREATE SYNONYMS\r\n"
          +"GO\r\n\r\n";
      Assert.That (createScriptResult, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetDropScript ()
    {
      var entityDefinition1 = MockRepository.GenerateStub<IEntityDefinition> ();
      var entityDefinition2 = MockRepository.GenerateStub<IEntityDefinition> ();

      _tableBuilderMock.Expect (mock => mock.AddEntityDefinition (entityDefinition1));
      _tableBuilderMock.Expect (mock => mock.AddEntityDefinition (entityDefinition2));
      _tableBuilderMock.Expect (mock => mock.GetDropScript ()).Return ("DROP TABLES\r\n");
      _tableBuilderMock.Replay ();

      _viewBuilderMock.Expect (mock => mock.AddEntityDefinition (entityDefinition1));
      _viewBuilderMock.Expect (mock => mock.AddEntityDefinition (entityDefinition2));
      _viewBuilderMock.Expect (mock => mock.GetDropScript ()).Return ("DROP VIEWS\r\n");
      _viewBuilderMock.Replay ();

      _constraintBuilderMock.Expect (mock => mock.AddEntityDefinition (entityDefinition1));
      _constraintBuilderMock.Expect (mock => mock.AddEntityDefinition (entityDefinition2));
      _constraintBuilderMock.Expect (mock => mock.GetDropScript ()).Return ("DROP CONSTRAINTS\r\n");
      _constraintBuilderMock.Replay ();

      _indexBuilderMock.Expect (mock => mock.AddEntityDefinition (entityDefinition1));
      _indexBuilderMock.Expect (mock => mock.AddEntityDefinition (entityDefinition2));
      _indexBuilderMock.Expect (mock => mock.GetDropScript ()).Return ("DROP INDEXES\r\n");
      _indexBuilderMock.Replay ();

      _synonymBuilderMock.Expect (mock => mock.AddEntityDefinition (entityDefinition1));
      _synonymBuilderMock.Expect (mock => mock.AddEntityDefinition (entityDefinition2));
      _synonymBuilderMock.Expect (mock => mock.GetDropScript ()).Return ("DROP SYNONYMS\r\n");
      _synonymBuilderMock.Replay ();

      _scriptBuilderForFirstStorageProvider.AddEntityDefinition (entityDefinition1);
      _scriptBuilderForFirstStorageProvider.AddEntityDefinition (entityDefinition2);

      var createScriptResult = _scriptBuilderForFirstStorageProvider.GetDropScript ();

      _tableBuilderMock.VerifyAllExpectations ();
      _viewBuilderMock.VerifyAllExpectations ();
      _constraintBuilderMock.VerifyAllExpectations ();
      _indexBuilderMock.VerifyAllExpectations ();
      _synonymBuilderMock.VerifyAllExpectations ();

      var expectedResult =
          "USE SchemaGenerationTestDomain1\r\n"
          + "GO\r\n\r\n"
          + "DROP SYNONYMS\r\n"
          + "GO\r\n\r\n"
          + "DROP INDEXES\r\n"
          + "GO\r\n\r\n"
          + "DROP VIEWS\r\n"
          + "GO\r\n\r\n"
          + "DROP CONSTRAINTS\r\n"
          + "GO\r\n\r\n"
          + "DROP TABLES\r\n"
          + "GO\r\n\r\n";
      Assert.That (createScriptResult, Is.EqualTo (expectedResult));
    }
    
    [Test]
    public void GetCreateScript_GetDropScript_NoEntities ()
    {
      _tableBuilderMock.Expect (mock => mock.GetCreateScript ()).Return ("-- Create all tables\r\n");
      _tableBuilderMock.Expect (mock => mock.GetDropScript ()).Return ("-- Drop all tables that will be created below\r\n");
      _tableBuilderMock.Replay();

      _viewBuilderMock.Expect (mock => mock.GetCreateScript ()).Return ("-- Create a view for every class\r\n");
      _viewBuilderMock.Expect (mock => mock.GetDropScript ()).Return ("-- Drop all views that will be created below\r\n");
      _viewBuilderMock.Replay();

      _constraintBuilderMock.Expect (mock => mock.GetCreateScript ()).Return ("-- Create constraints for tables that were created above\r\n");
      _constraintBuilderMock.Expect (mock => mock.GetDropScript ()).Return ("-- Drop foreign keys of all tables that will be created below\r\n");
      _constraintBuilderMock.Replay();

      _indexBuilderMock.Expect (mock => mock.GetCreateScript ()).Return ("-- Create indexes for tables that were created above\r\n");
      _indexBuilderMock.Expect (mock => mock.GetDropScript ()).Return ("-- Drop all indexes that will be created below\r\n");
      _indexBuilderMock.Replay();

      _synonymBuilderMock.Expect (mock => mock.GetCreateScript ()).Return ("-- Create synonyms for tables that were created above\r\n");
      _synonymBuilderMock.Expect (mock => mock.GetDropScript ()).Return ("-- Drop all synonyms that will be created below\r\n");
      _synonymBuilderMock.Replay();

      var createScriptResult = _scriptBuilderForFirstStorageProvider.GetCreateScript ();
      var dropScriptResult = _scriptBuilderForFirstStorageProvider.GetDropScript ();

      Assert.That (createScriptResult, Is.EqualTo (_firstStorageProviderSetupDBScriptWithoutTables));
      Assert.That (dropScriptResult, Is.EqualTo (_firstStorageProviderTearDownDBScriptWithoutTables));

      _tableBuilderMock.VerifyAllExpectations();
      _viewBuilderMock.VerifyAllExpectations();
      _constraintBuilderMock.VerifyAllExpectations();
      _indexBuilderMock.VerifyAllExpectations();
      _synonymBuilderMock.VerifyAllExpectations();
    }
  }
}