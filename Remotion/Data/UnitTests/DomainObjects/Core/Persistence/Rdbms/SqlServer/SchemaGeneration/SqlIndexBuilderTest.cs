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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class SqlIndexBuilderTest : SchemaGenerationTestBase
  {
    private SqlIndexBuilder _indexBuilder;
    private SimpleColumnDefinition _column1;
    private SimpleColumnDefinition _column2;
    private SimpleColumnDefinition _column3;
    private EntityNameDefinition _tableName;
    private EntityNameDefinition _viewName;
    private IndexDefinition _indexDefinition;
    private TableDefinition _tableDefinition;

    // TODO Review 3883: Refactor tests: Rewrite to test the Visit...Definition methods instead of the base class methods. Remove TableDefinition etc. => not needed
    // TODO Review 3883: Add tests with non-default schemas
    // TODO Review 3883: Add tests for option values: one with all options on, one with all options off (later: one with all options undefined)

    public override void SetUp ()
    {
      base.SetUp ();

      _indexBuilder = new SqlIndexBuilder ();

      _column1 = new SimpleColumnDefinition ("ID", typeof (int), "integer", false, true);
      _column2 = new SimpleColumnDefinition ("Name", typeof (string), "varchar(100)", true, false);
      _column3 = new SimpleColumnDefinition ("Test", typeof (string), "varchar(100)", true, false);
      _tableName = new EntityNameDefinition (null, "TableName");
      _viewName = new EntityNameDefinition (null, "ViewName");
      
      _indexDefinition = new IndexDefinition (
          "Index1", _tableName, new[] { _column1, _column2 }, null, true, true, false, false);
      _tableDefinition = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          _tableName,
          _viewName,
          new[] { _column1, _column2 },
          new ITableConstraintDefinition[0],
          new IIndexDefinition[] { _indexDefinition }, new EntityNameDefinition[0]);
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (_indexBuilder.GetCreateIndexScript (), Is.Empty);
      Assert.That (_indexBuilder.GetCreateIndexScript (), Is.Empty);
    }

    [Test]
    public void CreateViewSeparator ()
    {
      var result = _indexBuilder.IndexStatementSeparator;

      Assert.That (result, Is.EqualTo ("GO\r\n\r\n"));
    }

    [Test]
    public void GetCreateIndexScript_TableDefinition_IndexDefinitionWithoutIncludedColumns ()
    {
      _indexBuilder.AddIndexes (_tableDefinition);

      var result = _indexBuilder.GetCreateIndexScript();

      var expectedScript =
          "CREATE UNIQUE CLUSTERED INDEX [Index1]\r\n"
          + "  ON [dbo].[TableName] ([ID], [Name])\r\n";
      Assert.That (result, Is.EqualTo (expectedScript));
    }

    [Test]
    public void GetCreateIndexScript_FilterViewDefinition_IndexDefinitionWithIncludedColumns ()
    {
      var indexDefinition = new IndexDefinition ("Index1", _tableName, new[] { _column1, _column2 }, new[] { _column3 }, false, false, true, true);

      var filterViewDefinition = new FilterViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          _viewName,
          _tableDefinition,
          new[] { "ClassID" },
          new IColumnDefinition[0],
          new[] { indexDefinition }, new EntityNameDefinition[0]);

      _indexBuilder.AddIndexes (filterViewDefinition);

      var result = _indexBuilder.GetCreateIndexScript ();

      var expectedScript =
          "CREATE NONCLUSTERED INDEX [Index1]\r\n"
          + "  ON [dbo].[TableName] ([ID], [Name])\r\n"
          + "  INCLUDE ([Test])\r\n"
          + "  WITH IGNORE_DUP_KEY, ONLINE\r\n";
      Assert.That (result, Is.EqualTo (expectedScript));
    }

    [Test]
    public void GetCreateIndexScript_UnionViewDefinition_OneXmlIndex ()
    {
      var indexDefinition = new PrimaryXmlIndexDefinition ("Index1", _tableName, _column3);
      var unionViewDefinition = new UnionViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          _viewName,
          new[] { _tableDefinition },
          new IColumnDefinition[0],
          new IIndexDefinition[] { indexDefinition }, new EntityNameDefinition[0]);

      _indexBuilder.AddIndexes (unionViewDefinition);

      var result = _indexBuilder.GetCreateIndexScript ();

      var expectedScript =
          "CREATE PRIMARY XML INDEX [Index1]\r\n"
          + "  ON [dbo].[TableName] ([Test])\r\n";
      Assert.That (result, Is.EqualTo (expectedScript));
    }

    [Test]
    public void GetCreateIndexScript_UnionViewDefinition_TwoXmlIndex ()
    {
      var primaryIndexDefinition = new PrimaryXmlIndexDefinition ("Index1", _tableName, _column3);
      var secondaryIndexDefinition = new SecondaryXmlIndexDefinition (
          "SecondaryName",
          _tableName,
          _column2,
          "PrimaryIndexName",
          SecondaryXmlIndexKind.Property);

      var unionViewDefinition = new UnionViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          _viewName,
          new[] { _tableDefinition },
          new IColumnDefinition[0],
          new IIndexDefinition[] { primaryIndexDefinition, secondaryIndexDefinition }, new EntityNameDefinition[0]);

      _indexBuilder.AddIndexes (unionViewDefinition);

      var result = _indexBuilder.GetCreateIndexScript ();

      var expectedScript =
          "CREATE PRIMARY XML INDEX [Index1]\r\n"
          +"  ON [dbo].[TableName] ([Test])\r\n"
          +"GO\r\n\r\n"
          +"CREATE XML INDEX [SecondaryName]\r\n"
          +"  ON [dbo].[TableName] ([Name])\r\n"
          +"  USING XML INDEX [PrimaryIndexName]\r\n"
          +"  FOR Property\r\n";
      Assert.That (result, Is.EqualTo (expectedScript));
    }

    [Test]
    public void GetDropIndexScript_OneIndex ()
    {
      _indexBuilder.AddIndexes (_tableDefinition);

      var result = _indexBuilder.GetDropIndexScript();

      var expectedScript = 
        "IF EXISTS (SELECT * FROM sys.objects so JOIN sysindexes si ON so.[object_id] = si.[id] "
        +"WHERE so.[name] = 'TableName' and schema_name (so.schema_id)='dbo' and si.[name] = 'Index1')\r\n"
        +"  DROP INDEX [Index1] ON [dbo].[TableName]\r\n";
      Assert.That (result, Is.EqualTo (expectedScript));
    }

    [Test]
    public void GetDropIndexScript_TwoIndexes ()
    {
      _indexBuilder.AddIndexes (_tableDefinition);
      _indexBuilder.AddIndexes (_tableDefinition);
      
      var result = _indexBuilder.GetDropIndexScript ();

      var expectedScript = 
        "IF EXISTS (SELECT * FROM sys.objects so JOIN sysindexes si ON so.[object_id] = si.[id] "
        +"WHERE so.[name] = 'TableName' and schema_name (so.schema_id)='dbo' and si.[name] = 'Index1')\r\n"
        +"  DROP INDEX [Index1] ON [dbo].[TableName]\r\n"
        +"GO\r\n\r\n"
        +"IF EXISTS (SELECT * FROM sys.objects so JOIN sysindexes si ON so.[object_id] = si.[id] "
        +"WHERE so.[name] = 'TableName' and schema_name (so.schema_id)='dbo' and si.[name] = 'Index1')\r\n"
        +"  DROP INDEX [Index1] ON [dbo].[TableName]\r\n";
      Assert.That (result, Is.EqualTo (expectedScript));
    }
  }
}