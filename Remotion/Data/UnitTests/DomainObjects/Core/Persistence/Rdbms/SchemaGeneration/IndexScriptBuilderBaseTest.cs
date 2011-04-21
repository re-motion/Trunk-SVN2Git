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
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration
{
  [TestFixture]
  public class IndexScriptBuilderBaseTest : SchemaGenerationTestBase
  {
    private ISqlDialect _sqlDialectStub;
    private TestableIndexBuilder _indexBuilder;
    private SqlIndexDefinition _sqlIndexDefinition;
    private SqlPrimaryXmlIndexDefinition _primaryXmlIndexDefinition;
    private SqlSecondaryXmlIndexDefinition _secondaryXmlIndexDefinition;
    private TableDefinition _tableDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect>();
      _indexBuilder = new TestableIndexBuilder (_sqlDialectStub);

      var objectName = new EntityNameDefinition (null, "ObjectName");
      var xmlColumn = new SqlIndexedColumnDefinition(new SimpleColumnDefinition ("XmlColumn", typeof (string), "xml", false, false));
      _sqlIndexDefinition = new SqlIndexDefinition ("IndexDefinitionName", objectName, new[] { xmlColumn }, null, false, false, false, false);
      _primaryXmlIndexDefinition = new SqlPrimaryXmlIndexDefinition ("PrimaryIndexName", objectName, xmlColumn);
      _secondaryXmlIndexDefinition = new SqlSecondaryXmlIndexDefinition (
          "SecondaryIndexName", objectName, xmlColumn, "PrimaryIndexName", SqlSecondaryXmlIndexKind.Property);

      _tableDefinition = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "tableName"),
          new EntityNameDefinition (null, "viewName"),
          new IColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[] { _sqlIndexDefinition, _primaryXmlIndexDefinition, _secondaryXmlIndexDefinition }, new EntityNameDefinition[0]);
    }

    [Test]
    public void AddEntityDefinition_TableDefinition ()
    {
      _indexBuilder.AddEntityDefinition (_tableDefinition);

      var createIndexScript = _indexBuilder.GetCreateScript();
      var dropIndexScript = _indexBuilder.GetDropScript();

      Assert.That (
          createIndexScript,
          Is.EqualTo (
              "-- Create indexes for tables that were created above\r\n"
              +"CREATE INDEX IndexDefinitionName (IndexDefinition)\r\n" 
              +"CREATE INDEX PrimaryIndexName (PrimaryXmlIndexDefinition)\r\n" 
              +"CREATE INDEX SecondaryIndexName (SecondaryXmlIndexDefinition)\r\n"));
      Assert.That (
          dropIndexScript,
          Is.EqualTo (
              "-- Drop all indexes that will be created below\r\n"
              +"DROP INDEX IndexDefinitionName (IndexDefinition)\r\n" 
              +"DROP INDEX PrimaryIndexName (PrimaryXmlIndexDefinition)\r\n" 
              +"DROP INDEX SecondaryIndexName (SecondaryXmlIndexDefinition)\r\n"));
    }

    [Test]
    public void AddEntityDefinition_FilterViewDefinition ()
    {
      var filterViewDefinition = new FilterViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "filterViewName"),
          _tableDefinition,
          new[] { "ClassID" },
          new IColumnDefinition[0],
          new IIndexDefinition[] { _sqlIndexDefinition, _primaryXmlIndexDefinition, _secondaryXmlIndexDefinition }, new EntityNameDefinition[0]);

      _indexBuilder.AddEntityDefinition (filterViewDefinition);

      var createIndexScript = _indexBuilder.GetCreateScript();
      var dropIndexScript = _indexBuilder.GetDropScript();

      Assert.That (
          createIndexScript,
          Is.EqualTo (
              "-- Create indexes for tables that were created above\r\n"
              +"CREATE INDEX IndexDefinitionName (IndexDefinition)\r\n" 
              +"CREATE INDEX PrimaryIndexName (PrimaryXmlIndexDefinition)\r\n" 
              +"CREATE INDEX SecondaryIndexName (SecondaryXmlIndexDefinition)\r\n"));
      Assert.That (
          dropIndexScript,
          Is.EqualTo (
              "-- Drop all indexes that will be created below\r\n"
              +"DROP INDEX IndexDefinitionName (IndexDefinition)\r\n" 
              +"DROP INDEX PrimaryIndexName (PrimaryXmlIndexDefinition)\r\n" 
              +"DROP INDEX SecondaryIndexName (SecondaryXmlIndexDefinition)\r\n"));
    }

    [Test]
    public void AddEntityDefinition_UnionViewDefinition ()
    {
      var unionViewDefinition = new UnionViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "UnionViewName"),
          new[] { _tableDefinition },
          new IColumnDefinition[0],
          new IIndexDefinition[] { _sqlIndexDefinition, _primaryXmlIndexDefinition, _secondaryXmlIndexDefinition }, new EntityNameDefinition[0]);

      _indexBuilder.AddEntityDefinition (unionViewDefinition);

      var createIndexScript = _indexBuilder.GetCreateScript();
      var dropIndexScript = _indexBuilder.GetDropScript();

      Assert.That (
          createIndexScript,
          Is.EqualTo (
              "-- Create indexes for tables that were created above\r\n"
              +"CREATE INDEX IndexDefinitionName (IndexDefinition)\r\n" 
              +"CREATE INDEX PrimaryIndexName (PrimaryXmlIndexDefinition)\r\n" 
              +"CREATE INDEX SecondaryIndexName (SecondaryXmlIndexDefinition)\r\n"));
      Assert.That (
          dropIndexScript,
          Is.EqualTo (
              "-- Drop all indexes that will be created below\r\n"
              +"DROP INDEX IndexDefinitionName (IndexDefinition)\r\n" 
              +"DROP INDEX PrimaryIndexName (PrimaryXmlIndexDefinition)\r\n" 
              +"DROP INDEX SecondaryIndexName (SecondaryXmlIndexDefinition)\r\n"));
    }
  }
}