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
  //TODO: Derive ClassWithAllDataTypes from an abstract class to ensure that all data types are selected in a UNION
  [TestFixture]
  public class ScriptBuilderTest : SchemaGenerationTestBase
  {
    private SqlScriptBuilder _scriptBuilderForFirstStorageProvider;
    private SqlScriptBuilder _scriptBuilderForSecondStorageProvider;
    private string _firstStorageProviderSetupDBScriptWithoutTables;
    private SqlTableBuilder _tableBuilderMock;
    private SqlViewBuilder _viewBuilderMock;
    private SqlConstraintBuilder _constraintBuilderMock;
    private SqlIndexBuilder _indexBuilderMock;

    public override void SetUp ()
    {
      base.SetUp();

      _tableBuilderMock = MockRepository.GenerateStrictMock<SqlTableBuilder>();
      _viewBuilderMock = MockRepository.GenerateStrictMock<SqlViewBuilder>();
      _constraintBuilderMock = MockRepository.GenerateStrictMock<SqlConstraintBuilder>();
      _indexBuilderMock = MockRepository.GenerateStrictMock<SqlIndexBuilder>();

      _scriptBuilderForFirstStorageProvider = new SqlScriptBuilder (
          SchemaGenerationFirstStorageProviderDefinition, _tableBuilderMock, _viewBuilderMock, _constraintBuilderMock, _indexBuilderMock);
      _scriptBuilderForSecondStorageProvider = new SqlScriptBuilder (
          SchemaGenerationSecondStorageProviderDefinition, _tableBuilderMock, _viewBuilderMock, _constraintBuilderMock, _indexBuilderMock);
      _firstStorageProviderSetupDBScriptWithoutTables = ResourceUtility.GetResourceString (
          typeof (ScriptBuilderTest), "TestData.SetupDB_FirstStorageProviderWithoutTables.sql");
    }

    [Test]
    public void GetDatabaseName ()
    {
      Assert.AreEqual ("SchemaGenerationTestDomain1", _scriptBuilderForFirstStorageProvider.GetDatabaseName());
      Assert.AreEqual ("SchemaGenerationTestDomain2", _scriptBuilderForSecondStorageProvider.GetDatabaseName());
    }

    //TODO RM-3874: crate ITableBuilder, IViewBuilder, IConstraintBuilder and IIndexBuilder interfaces to test with mocks?
    //[Test]
    //public void GetScript ()
    //{
    //  var tableDefinition1 = new TableDefinition (
    //      SchemaGenerationFirstStorageProviderDefinition,
    //      new EntityNameDefinition (null, "Table1"),
    //      new EntityNameDefinition (null, "View1"),
    //      new IColumnDefinition[0],
    //      new ITableConstraintDefinition[0],
    //      new IIndexDefinition[0]);
    //  var tableDefinition2 = new TableDefinition (
    //      SchemaGenerationFirstStorageProviderDefinition,
    //      new EntityNameDefinition (null, "Table2"),
    //      new EntityNameDefinition (null, "View2"),
    //      new IColumnDefinition[0],
    //      new ITableConstraintDefinition[0],
    //      new IIndexDefinition[0]);

    //  _tableBuilderMock.Expect (mock => mock.AddTable (tableDefinition1));
    //  _tableBuilderMock.Expect (mock => mock.AddTable (tableDefinition2));
    //  _tableBuilderMock.Replay ();
    //  _viewBuilderMock.Expect (mock => mock.AddView (tableDefinition1));
    //  _viewBuilderMock.Expect (mock => mock.AddView (tableDefinition2));
    //  _viewBuilderMock.Replay ();
    //  _constraintBuilderMock.Expect (mock => mock.AddConstraint(tableDefinition1));
    //  _constraintBuilderMock.Expect (mock => mock.AddConstraint(tableDefinition2));
    //  _constraintBuilderMock.Replay ();
    //  _indexBuilderMock.Expect (mock => mock.AddIndexes(tableDefinition1));
    //  _indexBuilderMock.Expect (mock => mock.AddIndexes(tableDefinition2));
    //  _indexBuilderMock.Replay ();

    //  _scriptBuilderForFirstStorageProvider.GetScript (new[]{tableDefinition1, tableDefinition2});

    //  _tableBuilderMock.VerifyAllExpectations ();
    //  _viewBuilderMock.VerifyAllExpectations ();
    //  _constraintBuilderMock.VerifyAllExpectations ();
    //  _indexBuilderMock.VerifyAllExpectations ();
    //}

    [Test]
    public void GetScript_NoEntities ()
    {
      _tableBuilderMock.Replay();
      _viewBuilderMock.Replay();
      _constraintBuilderMock.Replay();
      _indexBuilderMock.Replay();

      var result = _scriptBuilderForFirstStorageProvider.GetScript (new IEntityDefinition[0]);

      Assert.That (result, Is.EqualTo (_firstStorageProviderSetupDBScriptWithoutTables));
      _tableBuilderMock.VerifyAllExpectations();
      _viewBuilderMock.VerifyAllExpectations();
      _constraintBuilderMock.VerifyAllExpectations();
      _indexBuilderMock.VerifyAllExpectations();
    }
  }
}