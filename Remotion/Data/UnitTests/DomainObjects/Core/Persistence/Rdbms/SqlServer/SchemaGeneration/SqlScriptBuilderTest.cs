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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Development.UnitTesting.Resources;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class SqlScriptBuilderTest : SchemaGenerationTestBase
  {
    private SqlScriptBuilder _scriptBuilderForFirstStorageProvider;
    private SqlScriptBuilder _scriptBuilderForSecondStorageProvider;
    private string _firstStorageProviderSetupDBScriptWithoutTables;
    private SqlTableBuilder _tableBuilderMock;
    private SqlViewBuilder _viewBuilderMock;
    private SqlConstraintBuilder _constraintBuilderMock;
    private SqlIndexBuilder _indexBuilderMock;
    private SqlSynonymBuilder _synonymBuilderMock;

    public override void SetUp ()
    {
      base.SetUp();

      _tableBuilderMock = MockRepository.GenerateStrictMock<SqlTableBuilder>();
      _viewBuilderMock = MockRepository.GenerateStrictMock<SqlViewBuilder>();
      _constraintBuilderMock = MockRepository.GenerateStrictMock<SqlConstraintBuilder>();
      _indexBuilderMock = MockRepository.GenerateStrictMock<SqlIndexBuilder>();
      _synonymBuilderMock = MockRepository.GenerateStrictMock<SqlSynonymBuilder>();

      _scriptBuilderForFirstStorageProvider = new SqlScriptBuilder (
          SchemaGenerationFirstStorageProviderDefinition, _tableBuilderMock, _viewBuilderMock, _constraintBuilderMock, _indexBuilderMock, _synonymBuilderMock);
      _scriptBuilderForSecondStorageProvider = new SqlScriptBuilder (
          SchemaGenerationSecondStorageProviderDefinition, _tableBuilderMock, _viewBuilderMock, _constraintBuilderMock, _indexBuilderMock, _synonymBuilderMock);
      _firstStorageProviderSetupDBScriptWithoutTables = ResourceUtility.GetResourceString (
          typeof (SqlScriptBuilderTest), "TestData.SetupDB_FirstStorageProviderWithoutTables.sql");
    }

    [Test]
    public void GetDatabaseName ()
    {
      Assert.AreEqual ("SchemaGenerationTestDomain1", _scriptBuilderForFirstStorageProvider.GetDatabaseName());
      Assert.AreEqual ("SchemaGenerationTestDomain2", _scriptBuilderForSecondStorageProvider.GetDatabaseName());
    }

    [Test]
    public void GetScript_NoEntities ()
    {
      _tableBuilderMock.Replay();
      _viewBuilderMock.Replay();
      _constraintBuilderMock.Replay();
      _indexBuilderMock.Replay();
      _synonymBuilderMock.Replay();

      var result = _scriptBuilderForFirstStorageProvider.GetScript (new IEntityDefinition[0]);

      Assert.That (result, Is.EqualTo (_firstStorageProviderSetupDBScriptWithoutTables));
      _tableBuilderMock.VerifyAllExpectations();
      _viewBuilderMock.VerifyAllExpectations();
      _constraintBuilderMock.VerifyAllExpectations();
      _indexBuilderMock.VerifyAllExpectations();
      _synonymBuilderMock.VerifyAllExpectations();
    }
  }
}