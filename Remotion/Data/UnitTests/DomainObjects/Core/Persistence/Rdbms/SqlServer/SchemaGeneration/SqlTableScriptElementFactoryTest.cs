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
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class SqlTableScriptElementFactoryTest : SchemaGenerationTestBase
  {
    private SqlTableScriptElementFactory _factory;
    private TableDefinition _tableDefinitionWithoutPrimaryKeyConstraint;
    private TableDefinition _tableDefinitionWithClusteredPrimaryKeyConstraint;
    private TableDefinition _tableDefinitionWithNonClusteredPrimaryKeyConstraint;
    private ColumnDefinition _column1;
    private ColumnDefinition _column2;

    public override void SetUp ()
    {
      base.SetUp();

      _factory = new SqlTableScriptElementFactory();

      _column1 = new ColumnDefinition (
          "Column1", typeof (string), StorageTypeInformationObjectMother.CreateVarchar100StorageTypeInformation(), false, true);
      _column2 = new ColumnDefinition (
          "Column2", typeof (bool), StorageTypeInformationObjectMother.CreateBitStorageTypeInformation(), true, false);

      _tableDefinitionWithoutPrimaryKeyConstraint = TableDefinitionObjectMother.Create (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition ("SchemaName", "EntityName"), null,
          ColumnDefinitionObjectMother.IDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn,
          _column1);

      _tableDefinitionWithClusteredPrimaryKeyConstraint = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition ("SchemaName", "EntityName"),
          null,
          ColumnDefinitionObjectMother.IDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn,
          new[] { _column1, _column2 },
          new ITableConstraintDefinition[] { new PrimaryKeyConstraintDefinition ("PKName", true, new[] { _column1 }) },
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);

      _tableDefinitionWithNonClusteredPrimaryKeyConstraint = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "EntityName"),
          null,
          ColumnDefinitionObjectMother.IDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn,
          ColumnDefinitionObjectMother.TimestampColumn,
          new[] { _column1, _column2 },
          new ITableConstraintDefinition[] { new PrimaryKeyConstraintDefinition ("PKName", false, new[] { _column1, _column2 }) },
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
    }

    [Test]
    public void GetCreateElement_TableDefinitionWithoutPrimaryKeyConstraint ()
    {
      var result = _factory.GetCreateElement (_tableDefinitionWithoutPrimaryKeyConstraint);

      var expectedResult =
          "CREATE TABLE [SchemaName].[EntityName]\r\n"
          + "(\r\n"
          + "  [ID] uniqueidentifier NOT NULL,\r\n"
          + "  [ClassID] varchar(100) NULL,\r\n"
          + "  [Timestamp] datetime NULL,\r\n"
          + "  [Column1] varchar(100) NOT NULL\r\n"
          + ")";

      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetCreateElement_TableDefinitionWithClusteredPrimaryKeyConstraint ()
    {
      var result = _factory.GetCreateElement (_tableDefinitionWithClusteredPrimaryKeyConstraint);

      var expectedResult =
          "CREATE TABLE [SchemaName].[EntityName]\r\n"
          + "(\r\n"
          + "  [ID] uniqueidentifier NOT NULL,\r\n"
          + "  [ClassID] varchar(100) NULL,\r\n"
          + "  [Timestamp] datetime NULL,\r\n"
          + "  [Column1] varchar(100) NOT NULL,\r\n"
          + "  [Column2] bit NULL,\r\n"
          + "  CONSTRAINT [PKName] PRIMARY KEY CLUSTERED ([Column1])\r\n"
          + ")";

      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetCreateElement_TableDefinitionWithNonClusteredPrimaryKeyConstraint ()
    {
      var result = _factory.GetCreateElement (_tableDefinitionWithNonClusteredPrimaryKeyConstraint);

      var expectedResult =
          "CREATE TABLE [dbo].[EntityName]\r\n"
          + "(\r\n"
          + "  [ID] uniqueidentifier NOT NULL,\r\n"
          + "  [ClassID] varchar(100) NULL,\r\n"
          + "  [Timestamp] datetime NULL,\r\n"
          + "  [Column1] varchar(100) NOT NULL,\r\n"
          + "  [Column2] bit NULL,\r\n"
          + "  CONSTRAINT [PKName] PRIMARY KEY NONCLUSTERED ([Column1], [Column2])\r\n"
          + ")";

      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetDropElement ()
    {
      var result = _factory.GetDropElement (_tableDefinitionWithClusteredPrimaryKeyConstraint);

      var expectedResult =
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'EntityName' AND TABLE_SCHEMA = 'SchemaName')\r\n"
          + "  DROP TABLE [SchemaName].[EntityName]";

      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetDropElement_DefaultSchema ()
    {
      var result = _factory.GetDropElement (_tableDefinitionWithNonClusteredPrimaryKeyConstraint);

      var expectedResult =
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'EntityName' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP TABLE [dbo].[EntityName]";

      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }
  }
}