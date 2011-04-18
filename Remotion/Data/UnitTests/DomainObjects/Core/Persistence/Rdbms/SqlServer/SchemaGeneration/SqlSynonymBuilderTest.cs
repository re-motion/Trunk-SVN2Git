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
using System.Text;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class SqlSynonymBuilderTest : SchemaGenerationTestBase
  {
    private SqlSynonymBuilder _synonymBuilder;
    private StringBuilder _stringBuilder;
    private TableDefinition _tableDefinition1;
    private TableDefinition _tableDefinition2;

    public override void SetUp ()
    {
      base.SetUp();

      _synonymBuilder = new SqlSynonymBuilder();
      _stringBuilder = new StringBuilder();

      _tableDefinition1 = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "Order"),
          new EntityNameDefinition (null, "OrderView"),
          new IColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0],
          new[] { new EntityNameDefinition (null, "Synonym1") });

      _tableDefinition2 = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition ("test", "Order"),
          new EntityNameDefinition ("test", "OrderView"),
          new IColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0],
          new[]
          {
              new EntityNameDefinition ("test", "Synonym1"), 
              new EntityNameDefinition ("test", "Synonym2"),
              new EntityNameDefinition ("test", "Synonym3")
          });
    }

    [Test]
    public void AddToCreateSynonymScript_OneSynonym ()
    {
      _synonymBuilder.AddToCreateSynonymScript (_tableDefinition1, _stringBuilder);

      var expectedResult = "CREATE SYNONYM [dbo].[Synonym1] FOR [dbo].[OrderView]\r\n";
      Assert.That (_stringBuilder.ToString(), Is.EqualTo (expectedResult));
    }

    [Test]
    public void AddToCreateSynonymScript_SeveralSynonyms ()
    {
      _synonymBuilder.AddToCreateSynonymScript (_tableDefinition2, _stringBuilder);

      var expectedResult =
          "CREATE SYNONYM [test].[Synonym1] FOR [test].[OrderView]\r\n\r\n"
          + "CREATE SYNONYM [test].[Synonym2] FOR [test].[OrderView]\r\n\r\n"
          + "CREATE SYNONYM [test].[Synonym3] FOR [test].[OrderView]\r\n";
      Assert.That (_stringBuilder.ToString(), Is.EqualTo (expectedResult));
    }

    [Test]
    public void AddToDropSynonymScript_OneSynonym ()
    {
      _synonymBuilder.AddToDropSynonymScript (_tableDefinition1, _stringBuilder);

      var expectedResult = "IF EXISTS (SELECT * FROM SYS.SYNONYMS WHERE NAME = 'dbo' AND SCHEMA_NAME(schema_id) = 'Synonym1')\r\n"
                          + "  DROP SYNONYM [dbo].[Synonym1]\r\n";
      Assert.That (_stringBuilder.ToString (), Is.EqualTo (expectedResult));
    }

    [Test]
    public void AddToDropSynonymScript_SeveralSynonyms ()
    {
      _synonymBuilder.AddToDropSynonymScript (_tableDefinition2, _stringBuilder);

      var expectedResult =
         "IF EXISTS (SELECT * FROM SYS.SYNONYMS WHERE NAME = 'test' AND SCHEMA_NAME(schema_id) = 'Synonym1')\r\n"
       + "  DROP SYNONYM [test].[Synonym1]\r\n\r\n"+
         "IF EXISTS (SELECT * FROM SYS.SYNONYMS WHERE NAME = 'test' AND SCHEMA_NAME(schema_id) = 'Synonym2')\r\n"
       + "  DROP SYNONYM [test].[Synonym2]\r\n\r\n"+
         "IF EXISTS (SELECT * FROM SYS.SYNONYMS WHERE NAME = 'test' AND SCHEMA_NAME(schema_id) = 'Synonym3')\r\n"
       + "  DROP SYNONYM [test].[Synonym3]\r\n";
      Assert.That (_stringBuilder.ToString (), Is.EqualTo (expectedResult));
    }
  }
}