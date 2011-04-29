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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class SqlUnionViewScriptElementFactoryTest : SchemaGenerationTestBase
  {
    private SqlUnionViewScriptElementFactory _factory;
    private UnionViewDefinition _unionViewDefinitionWithCustomSchema;
    private UnionViewDefinition _unionViewDefinitionWithDefaultSchema;

    public override void SetUp ()
    {
      base.SetUp ();

      _factory = new SqlUnionViewScriptElementFactory();

      var column1 = new SimpleColumnDefinition ("Column1", typeof (string), "varchar", false, true);
      var column2 = new SimpleColumnDefinition ("Column2", typeof (int), "integer", true, false);

      var tableDefinition1 = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition ("SchemaName", "TableName1"),
          null,
          new[]{column1},
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
      var tableDefinition2 = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "TableName2"),
          null,
          new[]{column1, column2},
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);

      _unionViewDefinitionWithCustomSchema = new UnionViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition ("SchemaName", "UnionView1"),
          new[] { tableDefinition1 },
          new[]{column1},
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
      _unionViewDefinitionWithDefaultSchema = new UnionViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "UnionView2"),
          new[] { tableDefinition1, tableDefinition2 },
          new[]{column1, column2},
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
    }

    [Test]
    public void GetCreateElement_OneUnionedEntity_CustomSchemaWithSchemaBinding ()
    {
      var result = _factory.GetCreateElement (_unionViewDefinitionWithCustomSchema);

      Assert.That (result, Is.TypeOf (typeof (ScriptElementCollection)));
      var elements = ((ScriptElementCollection) result).Elements;
      Assert.That (elements.Count, Is.EqualTo (3));
      Assert.That (elements[0], Is.TypeOf (typeof (BatchDelimiterStatement)));
      Assert.That (elements[2], Is.TypeOf (typeof (BatchDelimiterStatement)));
      Assert.That (elements[1], Is.TypeOf (typeof (ScriptStatement)));
      var expectedResult =
          "CREATE VIEW [SchemaName].[UnionView1] ([Column1])\r\n"
          +"  WITH SCHEMABINDING AS\r\n"
          +"  SELECT [Column1]\r\n"
          +"    FROM [SchemaName].[TableName1]\r\n"
          +"  WITH CHECK OPTION";
      Assert.That (((ScriptStatement) elements[1]).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetCreateElement_OneUnionedEntity_CustomSchemaWithoutSchemaBinding ()
    {
      var factory = new ExtendedSqlUnionViewScriptElementFactory();

      var result = factory.GetCreateElement (_unionViewDefinitionWithCustomSchema);

      Assert.That (result, Is.TypeOf (typeof (ScriptElementCollection)));
      var elements = ((ScriptElementCollection) result).Elements;
      Assert.That (elements.Count, Is.EqualTo (3));
      Assert.That (elements[0], Is.TypeOf (typeof (BatchDelimiterStatement)));
      Assert.That (elements[2], Is.TypeOf (typeof (BatchDelimiterStatement)));
      Assert.That (elements[1], Is.TypeOf (typeof (ScriptStatement)));
      var expectedResult =
          "CREATE VIEW [SchemaName].[UnionView1] ([Column1])\r\n"
          + "  AS\r\n"
          + "  SELECT [Column1]\r\n"
          + "    FROM [SchemaName].[TableName1]\r\n"
          + "  WITH CHECK OPTION";
      Assert.That (((ScriptStatement) elements[1]).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetCreateElement_SeveralUnionedEntities_DefaultSchemaWithSchemaBinding ()
    {
      var result = _factory.GetCreateElement (_unionViewDefinitionWithDefaultSchema);

      Assert.That (result, Is.TypeOf (typeof (ScriptElementCollection)));
      var elements = ((ScriptElementCollection) result).Elements;
      Assert.That (elements.Count, Is.EqualTo (3));
      Assert.That (elements[0], Is.TypeOf (typeof (BatchDelimiterStatement)));
      Assert.That (elements[2], Is.TypeOf (typeof (BatchDelimiterStatement)));
      Assert.That (elements[1], Is.TypeOf (typeof (ScriptStatement)));

      var expectedResult =
          "CREATE VIEW [dbo].[UnionView2] ([Column1], [Column2])\r\n"
          +"  WITH SCHEMABINDING AS\r\n"
          +"  SELECT [Column1], NULL\r\n"
          +"    FROM [SchemaName].[TableName1]\r\n"
          +"  UNION ALL\r\n"
          +"  SELECT [Column1], [Column2]\r\n"
          +"    FROM [dbo].[TableName2]";
      Assert.That (((ScriptStatement) elements[1]).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetCreateElement_SeveralUnionedEntities_DefaultSchemaWithoutSchemaBinding ()
    {
      var factory = new ExtendedSqlUnionViewScriptElementFactory ();

      var result = factory.GetCreateElement (_unionViewDefinitionWithDefaultSchema);

      Assert.That (result, Is.TypeOf (typeof (ScriptElementCollection)));
      var elements = ((ScriptElementCollection) result).Elements;
      Assert.That (elements.Count, Is.EqualTo (3));
      Assert.That (elements[0], Is.TypeOf (typeof (BatchDelimiterStatement)));
      Assert.That (elements[2], Is.TypeOf (typeof (BatchDelimiterStatement)));
      Assert.That (elements[1], Is.TypeOf (typeof (ScriptStatement)));

      var expectedResult =
          "CREATE VIEW [dbo].[UnionView2] ([Column1], [Column2])\r\n"
          + "  AS\r\n"
          + "  SELECT [Column1], NULL\r\n"
          + "    FROM [SchemaName].[TableName1]\r\n"
          + "  UNION ALL\r\n"
          + "  SELECT [Column1], [Column2]\r\n"
          + "    FROM [dbo].[TableName2]";
      Assert.That (((ScriptStatement) elements[1]).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetDropElement_CustomSchema ()
    {
      var result = _factory.GetDropElement (_unionViewDefinitionWithCustomSchema);

      var expectedResult =
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'UnionView1' AND TABLE_SCHEMA = 'SchemaName')\r\n"
          + "  DROP VIEW [SchemaName].[UnionView1]";

      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetDropElement_DefaultSchema ()
    {
      var result = _factory.GetDropElement (_unionViewDefinitionWithDefaultSchema);

      var expectedResult =
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'UnionView2' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP VIEW [dbo].[UnionView2]";

      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }
  }
}