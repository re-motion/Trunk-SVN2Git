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
using Remotion.Development.UnitTesting.Resources;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class SqlCompositeScriptBuilderTest : SchemaGenerationTestBase
  {
    private SqlCompositeScriptBuilder _scriptBuilderForFirstStorageProvider;
    private SqlCompositeScriptBuilder _scriptBuilderForSecondStorageProvider;
    private string _firstStorageProviderSetupDBScriptWithoutTables;
    private string _firstStorageProviderTearDownDBScriptWithoutTables;
    private SqlTableScriptBuilder _tableBuilderMock;
    private SqlViewScriptBuilder _viewBuilderMock;
    private SqlConstraintScriptBuilder _constraintBuilderMock;
    private SqlIndexScriptBuilder _indexBuilderMock;
    private SqlSynonymScriptBuilder _synonymBuilderMock;
    
    public override void SetUp ()
    {
      base.SetUp();

      _tableBuilderMock = MockRepository.GenerateStrictMock<SqlTableScriptBuilder>();
      _viewBuilderMock = MockRepository.GenerateStrictMock<SqlViewScriptBuilder>();
      _constraintBuilderMock = MockRepository.GenerateStrictMock<SqlConstraintScriptBuilder>();
      _indexBuilderMock = MockRepository.GenerateStrictMock<SqlIndexScriptBuilder>();
      _synonymBuilderMock = MockRepository.GenerateStrictMock<SqlSynonymScriptBuilder>();

      _scriptBuilderForFirstStorageProvider = new SqlCompositeScriptBuilder (
          SchemaGenerationFirstStorageProviderDefinition, _tableBuilderMock, _constraintBuilderMock, _viewBuilderMock, _indexBuilderMock, _synonymBuilderMock);
      _scriptBuilderForSecondStorageProvider = new SqlCompositeScriptBuilder (
          SchemaGenerationSecondStorageProviderDefinition, _tableBuilderMock, _constraintBuilderMock, _viewBuilderMock, _indexBuilderMock, _synonymBuilderMock);
      _firstStorageProviderSetupDBScriptWithoutTables = ResourceUtility.GetResourceString (
          typeof (SqlCompositeScriptBuilderTest), "TestData.SetupDB_FirstStorageProviderWithoutTables.sql");
      _firstStorageProviderTearDownDBScriptWithoutTables = ResourceUtility.GetResourceString (
          typeof (SqlCompositeScriptBuilderTest), "TestData.TearDownDB_FirstStorageProviderWithoutTables.sql");
    }

    [Test]
    public void GetDatabaseName ()
    {
      Assert.AreEqual ("SchemaGenerationTestDomain1", _scriptBuilderForFirstStorageProvider.GetDatabaseName());
      Assert.AreEqual ("SchemaGenerationTestDomain2", _scriptBuilderForSecondStorageProvider.GetDatabaseName());
    }

    //TODO: Test for GetScript with entities and AddEntityDefinition as soon as interfaces can be used for partial script builders

    [Test]
    public void GetCreateScript_GetDropScript_NoEntities ()
    {
      _tableBuilderMock.Replay();
      _viewBuilderMock.Replay();
      _constraintBuilderMock.Replay();
      _indexBuilderMock.Replay();
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