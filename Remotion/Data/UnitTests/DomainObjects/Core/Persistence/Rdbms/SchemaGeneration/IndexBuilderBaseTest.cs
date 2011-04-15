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
  public class IndexBuilderBaseTest : SchemaGenerationTestBase
  {
    private ISqlDialect _sqlDialectStub;
    private TestableIndexBuilder _indexBuilder;
    private IndexDefinition _indexDefinition;
    private PrimaryXmlIndexDefinition _primaryXmlIndexDefinition;
    private SecondaryXmlIndexDefinition _secondaryXmlIndexDefinition;
    private TableDefinition _tableDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect>();
      _indexBuilder = new TestableIndexBuilder (_sqlDialectStub);

      var objectName = new EntityNameDefinition (null, "ObjectName");
      var xmlColumn = new SimpleColumnDefinition ("XmlColumn", typeof (string), "xml", false, false);
      _indexDefinition = new IndexDefinition ("IndexDefinitionName", objectName, new[] { xmlColumn }, null, false, false, false, false);
      _primaryXmlIndexDefinition = new PrimaryXmlIndexDefinition ("PrimaryIndexName", objectName, xmlColumn);
      _secondaryXmlIndexDefinition = new SecondaryXmlIndexDefinition (
          "SecondaryIndexName", objectName, xmlColumn, "PrimaryIndexName", SecondaryXmlIndexKind.Property);

      _tableDefinition = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "tableName"),
          new EntityNameDefinition (null, "viewName"),
          new IColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[] { _indexDefinition, _primaryXmlIndexDefinition, _secondaryXmlIndexDefinition }, new EntityNameDefinition[0]);
    }

    [Test]
    public void AddIndexes_TableDefinition ()
    {
      _indexBuilder.AddIndexes (_tableDefinition);

      var createIndexScript = _indexBuilder.GetCreateIndexScript();
      var dropIndexScript = _indexBuilder.GetDropIndexScript();

      Assert.That (
          createIndexScript,
          Is.EqualTo (
              "CREATE INDEX IndexDefinitionName (IndexDefinition)\r\n" +
              "CREATE INDEX PrimaryIndexName (PrimaryXmlIndexDefinition)\r\n" +
              "CREATE INDEX SecondaryIndexName (SecondaryXmlIndexDefinition)\r\n"));
      Assert.That (
          dropIndexScript,
          Is.EqualTo (
              "DROP INDEX IndexDefinitionName (IndexDefinition)\r\n" +
              "DROP INDEX PrimaryIndexName (PrimaryXmlIndexDefinition)\r\n" +
              "DROP INDEX SecondaryIndexName (SecondaryXmlIndexDefinition)\r\n"));
    }

    [Test]
    public void AddIndexes_FilterViewDefinition ()
    {
      var filterViewDefinition = new FilterViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "filterViewName"),
          _tableDefinition,
          new[] { "ClassID" },
          new IColumnDefinition[0],
          new IIndexDefinition[] { _indexDefinition, _primaryXmlIndexDefinition, _secondaryXmlIndexDefinition }, new EntityNameDefinition[0]);

      _indexBuilder.AddIndexes (filterViewDefinition);

      var createIndexScript = _indexBuilder.GetCreateIndexScript();
      var dropIndexScript = _indexBuilder.GetDropIndexScript();

      Assert.That (
          createIndexScript,
          Is.EqualTo (
              "CREATE INDEX IndexDefinitionName (IndexDefinition)\r\n" +
              "CREATE INDEX PrimaryIndexName (PrimaryXmlIndexDefinition)\r\n" +
              "CREATE INDEX SecondaryIndexName (SecondaryXmlIndexDefinition)\r\n"));
      Assert.That (
          dropIndexScript,
          Is.EqualTo (
              "DROP INDEX IndexDefinitionName (IndexDefinition)\r\n" +
              "DROP INDEX PrimaryIndexName (PrimaryXmlIndexDefinition)\r\n" +
              "DROP INDEX SecondaryIndexName (SecondaryXmlIndexDefinition)\r\n"));
    }

    [Test]
    public void AddIndexes_UnionViewDefinition ()
    {
      var unionViewDefinition = new UnionViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "UnionViewName"),
          new[] { _tableDefinition },
          new IColumnDefinition[0],
          new IIndexDefinition[] { _indexDefinition, _primaryXmlIndexDefinition, _secondaryXmlIndexDefinition }, new EntityNameDefinition[0]);

      _indexBuilder.AddIndexes (unionViewDefinition);

      var createIndexScript = _indexBuilder.GetCreateIndexScript();
      var dropIndexScript = _indexBuilder.GetDropIndexScript();

      Assert.That (
          createIndexScript,
          Is.EqualTo (
              "CREATE INDEX IndexDefinitionName (IndexDefinition)\r\n" +
              "CREATE INDEX PrimaryIndexName (PrimaryXmlIndexDefinition)\r\n" +
              "CREATE INDEX SecondaryIndexName (SecondaryXmlIndexDefinition)\r\n"));
      Assert.That (
          dropIndexScript,
          Is.EqualTo (
              "DROP INDEX IndexDefinitionName (IndexDefinition)\r\n" +
              "DROP INDEX PrimaryIndexName (PrimaryXmlIndexDefinition)\r\n" +
              "DROP INDEX SecondaryIndexName (SecondaryXmlIndexDefinition)\r\n"));
    }
  }
}