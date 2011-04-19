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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class SqlIndexBuilderTest : SchemaGenerationTestBase
  {
    private SqlIndexBuilder _indexBuilder;
    private SqlIndexedColumnDefinition _column1;
    private SqlIndexedColumnDefinition _column2;
    private SimpleColumnDefinition _column3;
    private EntityNameDefinition _tableNameWithDefaultSchema;
    private EntityNameDefinition _tableNameWithoutDefaultSchema;

    public override void SetUp ()
    {
      base.SetUp();

      _indexBuilder = new SqlIndexBuilder();

      _column1 = new SqlIndexedColumnDefinition (new SimpleColumnDefinition ("ID", typeof (int), "integer", false, true), IndexOrder.Asc);
      _column2 = new SqlIndexedColumnDefinition (new SimpleColumnDefinition ("Name", typeof (string), "varchar(100)", true, false), IndexOrder.Desc);
      _column3 = new SimpleColumnDefinition ("Test", typeof (string), "varchar(100)", true, false);
      _tableNameWithDefaultSchema = new EntityNameDefinition (null, "TableName");
      _tableNameWithoutDefaultSchema = new EntityNameDefinition ("test", "TableName");
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (_indexBuilder.GetCreateIndexScript(), Is.Empty);
      Assert.That (_indexBuilder.GetCreateIndexScript(), Is.Empty);
    }

    [Test]
    public void CreateViewSeparator ()
    {
      var result = _indexBuilder.IndexStatementSeparator;

      Assert.That (result, Is.EqualTo ("GO\r\n\r\n"));
    }

    [Test]
    public void VisitIndexDefinition_Once ()
    {
      var indexDefinition = new SqlIndexDefinition ("Index1", _tableNameWithDefaultSchema, new[] { _column1, _column2 });

      ((ISqlIndexDefinitionVisitor) _indexBuilder).VisitIndexDefinition (indexDefinition);

      var expectedCreateIndexScript =
          "CREATE NONCLUSTERED INDEX [Index1]\r\n"
          + "  ON [dbo].[TableName] ([ID] ASC, [Name] DESC)\r\n";
      var expectedDropIndexScript =
          "IF EXISTS (SELECT * FROM sys.objects so JOIN sysindexes si ON so.[object_id] = si.[id] WHERE so.[name] = 'TableName' and "
          +"schema_name (so.schema_id)='dbo' and si.[name] = 'Index1')\r\n"
          +"  DROP INDEX [Index1] ON [dbo].[TableName]\r\n";
      
      Assert.That (_indexBuilder.GetCreateIndexScript (), Is.EqualTo (expectedCreateIndexScript));
      Assert.That (_indexBuilder.GetDropIndexScript (), Is.EqualTo (expectedDropIndexScript));
    }

    [Test]
    public void VisitIndexDefinition_Twice ()
    {
      var indexDefinition = new SqlIndexDefinition ("Index1", _tableNameWithoutDefaultSchema, new[] { _column1, _column2 }, null, true, true);

      ((ISqlIndexDefinitionVisitor) _indexBuilder).VisitIndexDefinition (indexDefinition);
      ((ISqlIndexDefinitionVisitor) _indexBuilder).VisitIndexDefinition (indexDefinition);

      var expectedCreateIndexScript =
          "CREATE UNIQUE CLUSTERED INDEX [Index1]\r\n"
          + "  ON [test].[TableName] ([ID] ASC, [Name] DESC)\r\n"
          + "GO\r\n\r\n"
          + "CREATE UNIQUE CLUSTERED INDEX [Index1]\r\n"
          + "  ON [test].[TableName] ([ID] ASC, [Name] DESC)\r\n";
      var expectedDropIndexScript =
          "IF EXISTS (SELECT * FROM sys.objects so JOIN sysindexes si ON so.[object_id] = si.[id] WHERE so.[name] = 'TableName' and "
          +"schema_name (so.schema_id)='test' and si.[name] = 'Index1')\r\n"
          +"  DROP INDEX [Index1] ON [test].[TableName]\r\n"
          +"GO\r\n\r\n"
          +"IF EXISTS (SELECT * FROM sys.objects so JOIN sysindexes si ON so.[object_id] = si.[id] WHERE so.[name] = 'TableName' and "
          +"schema_name (so.schema_id)='test' and si.[name] = 'Index1')\r\n"
          +"  DROP INDEX [Index1] ON [test].[TableName]\r\n";

      Assert.That (_indexBuilder.GetCreateIndexScript (), Is.EqualTo (expectedCreateIndexScript));
      Assert.That (_indexBuilder.GetDropIndexScript (), Is.EqualTo (expectedDropIndexScript));
    }

    [Test]
    public void VisitIndexDefinition_WithIncludedColumnsAndSomeOptions ()
    {
      var indexDefinition = new SqlIndexDefinition ("Index1", _tableNameWithDefaultSchema, new[] { _column1, _column2 }, new[] { _column3 }, false, false, true, true);

      ((ISqlIndexDefinitionVisitor) _indexBuilder).VisitIndexDefinition (indexDefinition);

      var result = _indexBuilder.GetCreateIndexScript();

      var expectedScript =
          "CREATE NONCLUSTERED INDEX [Index1]\r\n"
          + "  ON [dbo].[TableName] ([ID] ASC, [Name] DESC)\r\n"
          + "  INCLUDE ([Test])\r\n"
          + "  WITH (IGNORE_DUP_KEY = ON, ONLINE = ON)\r\n";
      Assert.That (result, Is.EqualTo (expectedScript));
    }

    [Test]
    public void VisitIndexDefinition_WithAllOptionsOn ()
    {
      var indexDefinition = new SqlIndexDefinition (
          "Index1", _tableNameWithDefaultSchema, new[] { _column1, _column2 }, new[] { _column3 }, true, true, true, true, true, 5, true, true, true, true, true, 2);

      ((ISqlIndexDefinitionVisitor) _indexBuilder).VisitIndexDefinition (indexDefinition);

      var result = _indexBuilder.GetCreateIndexScript();

      var expectedScript =
          "CREATE UNIQUE CLUSTERED INDEX [Index1]\r\n"
          + "  ON [dbo].[TableName] ([ID] ASC, [Name] DESC)\r\n"
          + "  INCLUDE ([Test])\r\n"
          + "  WITH (IGNORE_DUP_KEY = ON, ONLINE = ON, PAD_INDEX = ON, FILLFACTOR = 5, SORT_IN_TEMPDB = ON, STATISTICS_NORECOMPUTE = ON, "
          + "DROP_EXISTING = ON, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, MAXDOP = 2)\r\n";
      Assert.That (result, Is.EqualTo (expectedScript));
    }

    [Test]
    public void VisitIndexDefinition_WithAllOptionsOff ()
    {
      var indexDefinition = new SqlIndexDefinition (
          "Index1",
          _tableNameWithDefaultSchema,
          new[] { _column1, _column2 },
          new[] { _column3 },
          false,
          false,
          false,
          false,
          false,
          0,
          false,
          false,
          false,
          false,
          false,
          0);

      ((ISqlIndexDefinitionVisitor) _indexBuilder).VisitIndexDefinition (indexDefinition);

      var result = _indexBuilder.GetCreateIndexScript();

      var expectedScript =
          "CREATE NONCLUSTERED INDEX [Index1]\r\n"
          + "  ON [dbo].[TableName] ([ID] ASC, [Name] DESC)\r\n"
          + "  INCLUDE ([Test])\r\n"
          + "  WITH (IGNORE_DUP_KEY = OFF, ONLINE = OFF, PAD_INDEX = OFF, FILLFACTOR = 0, SORT_IN_TEMPDB = OFF, STATISTICS_NORECOMPUTE = OFF, "
          + "DROP_EXISTING = OFF, ALLOW_ROW_LOCKS = OFF, ALLOW_PAGE_LOCKS = OFF, MAXDOP = 0)\r\n";
      Assert.That (result, Is.EqualTo (expectedScript));
    }

    [Test]
    public void VisitPrimaryXmlIndexDefinition ()
    {
      var indexDefinition = new SqlPrimaryXmlIndexDefinition ("Index1", _tableNameWithoutDefaultSchema, _column3);

      ((ISqlIndexDefinitionVisitor) _indexBuilder).VisitPrimaryXmlIndexDefinition (indexDefinition);

      var result = _indexBuilder.GetCreateIndexScript();

      var expectedScript =
          "CREATE PRIMARY XML INDEX [Index1]\r\n"
          + "  ON [test].[TableName] ([Test])\r\n";
      Assert.That (result, Is.EqualTo (expectedScript));
    }

    [Test]
    public void VisitPrimaryXmlIndex_VisitSecondaryXmlIndex ()
    {
      var indexDefinition = new SqlPrimaryXmlIndexDefinition ("Index1", _tableNameWithDefaultSchema, _column3);
      var secondaryIndexDefinition = new SqlSecondaryXmlIndexDefinition (
          "SecondaryName",
          _tableNameWithDefaultSchema,
          _column2,
          "PrimaryIndexName",
          SqlSecondaryXmlIndexKind.Property);

      ((ISqlIndexDefinitionVisitor) _indexBuilder).VisitPrimaryXmlIndexDefinition (indexDefinition);
      ((ISqlIndexDefinitionVisitor) _indexBuilder).VisitSecondaryXmlIndexDefinition (secondaryIndexDefinition);

      var result = _indexBuilder.GetCreateIndexScript();

      var expectedScript =
          "CREATE PRIMARY XML INDEX [Index1]\r\n"
          + "  ON [dbo].[TableName] ([Test])\r\n"
          + "GO\r\n\r\n"
          + "CREATE XML INDEX [SecondaryName]\r\n"
          + "  ON [dbo].[TableName] ([Name])\r\n"
          + "  USING XML INDEX [PrimaryIndexName]\r\n"
          + "  FOR Property\r\n";
      Assert.That (result, Is.EqualTo (expectedScript));
    }

    [Test]
    public void VisitPrimaryXmlIndexDefinition_WithAllOptions ()
    {
      var indexDefinition = new SqlPrimaryXmlIndexDefinition ("Index1", _tableNameWithDefaultSchema, _column3, true, 10, true, false, true, false, false, 18);

      ((ISqlIndexDefinitionVisitor) _indexBuilder).VisitPrimaryXmlIndexDefinition (indexDefinition);

      var result = _indexBuilder.GetCreateIndexScript();

      var expectedScript =
          "CREATE PRIMARY XML INDEX [Index1]\r\n"
          +"  ON [dbo].[TableName] ([Test])\r\n"
          +"  WITH (PAD_INDEX = ON, FILLFACTOR = 10, SORT_IN_TEMPDB = ON, STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = ON, ALLOW_ROW_LOCKS = OFF, "
          +"ALLOW_PAGE_LOCKS = OFF, MAXDOP = 18)\r\n";
      Assert.That (result, Is.EqualTo (expectedScript));
    }

    [Test]
    public void VisitSecondaryXmlIndexDefinition_WithAllOptions ()
    {
      var secondaryIndexDefinition = new SqlSecondaryXmlIndexDefinition (
          "SecondaryName",
          _tableNameWithDefaultSchema,
          _column2,
          "PrimaryIndexName",
          SqlSecondaryXmlIndexKind.Property,
          false,
          20,
          false,
          true,
          true,
          true,
          false,
          100);

      ((ISqlIndexDefinitionVisitor) _indexBuilder).VisitSecondaryXmlIndexDefinition (secondaryIndexDefinition);

      var result = _indexBuilder.GetCreateIndexScript();

      var expectedScript =
          "CREATE XML INDEX [SecondaryName]\r\n"
          +"  ON [dbo].[TableName] ([Name])\r\n"
          +"  USING XML INDEX [PrimaryIndexName]\r\n"
          +"  FOR Property\r\n"
          +"  WITH (PAD_INDEX = OFF, FILLFACTOR = 20, SORT_IN_TEMPDB = OFF, STATISTICS_NORECOMPUTE = ON, DROP_EXISTING = ON, ALLOW_ROW_LOCKS = ON, "
          +"ALLOW_PAGE_LOCKS = OFF, MAXDOP = 100)\r\n";
      Assert.That (result, Is.EqualTo (expectedScript));
    }
  }
}