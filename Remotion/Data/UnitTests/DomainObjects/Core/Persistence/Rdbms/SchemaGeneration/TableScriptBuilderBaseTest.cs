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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration
{
  [TestFixture]
  public class TableScriptBuilderBaseTest : SchemaGenerationTestBase
  {
    private TableScriptBuilderBase _tableBuilder;
    private ISqlDialect _sqlDialectStub;
    private TableDefinition _tableDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect>();
      _tableBuilder = new TestableTableBuilder (_sqlDialectStub);

      _tableDefinition = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "Order"),
          null,
          new IColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0], new EntityNameDefinition[0]);
    }

    [Test]
    public void AddTable ()
    {
      _tableBuilder.AddEntityDefinition (_tableDefinition);

      var createTableScript = _tableBuilder.GetCreateScript();
      var dropTableScript = _tableBuilder.GetDropScript();

      Assert.AreEqual (createTableScript, "CREATE TABLE [Order]");
      Assert.AreEqual (dropTableScript, "DROP TABLE [Order]");
    }

    [Test]
    public void AddTableTwice ()
    {
      _tableBuilder.AddEntityDefinition (_tableDefinition);
      _tableBuilder.AddEntityDefinition (_tableDefinition);

      var createTableScript = _tableBuilder.GetCreateScript();
      var dropTableScript = _tableBuilder.GetDropScript();

      Assert.AreEqual (createTableScript, "CREATE TABLE [Order]\r\nCREATE TABLE [Order]");
      Assert.AreEqual (dropTableScript, "DROP TABLE [Order]\r\nDROP TABLE [Order]");
    }

    [Test]
    public void AddTable_WithUnionViewDefinition ()
    {
      var unionViewDefinition = new UnionViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition(null, "Test"),
          new[] { _tableDefinition },
          new IColumnDefinition[0],
          new IIndexDefinition[0], new EntityNameDefinition[0]);
      _tableBuilder.AddEntityDefinition (unionViewDefinition);

      var actualCreateTableScript = _tableBuilder.GetCreateScript();
      var actualDropTableScript = _tableBuilder.GetDropScript();

      Assert.IsEmpty (actualCreateTableScript);
      Assert.IsEmpty (actualDropTableScript);
    }

    [Test]
    public void GetCreateTableScript_GetDropTableScript_NoTableAdded ()
    {
      var createTableScript = _tableBuilder.GetCreateScript ();
      var dropTableScript = _tableBuilder.GetDropScript ();

      Assert.IsEmpty (createTableScript);
      Assert.IsEmpty (dropTableScript);
    }
  }
}