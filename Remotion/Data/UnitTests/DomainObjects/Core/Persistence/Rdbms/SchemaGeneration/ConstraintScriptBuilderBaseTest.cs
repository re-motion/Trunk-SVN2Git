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
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration
{
  [TestFixture]
  public class ConstraintScriptBuilderBaseTest : SchemaGenerationTestBase
  {
    private ISqlDialect _sqlDialectStub;
    private TestableConstraintBuilder _constraintBuilder;
    private TableDefinition _tableDefinition1;
    private TableDefinition _tableDefinition2;
    private UnionViewDefinition _unionViewDefinition;
    private FilterViewDefinition _filterViewDefinition;
    private NullEntityDefinition _nullEntityDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect>();
      _constraintBuilder = new TestableConstraintBuilder (_sqlDialectStub);
      _tableDefinition1 = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "Order"),
          null,
          new IColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0], new EntityNameDefinition[0]);
      _tableDefinition2 = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "Customer"),
          null,
          new IColumnDefinition[0],
          new ITableConstraintDefinition[0],
          new IIndexDefinition[0], new EntityNameDefinition[0]);
      _unionViewDefinition = new UnionViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "Test"),
          new[] { _tableDefinition1 },
          new IColumnDefinition[0],
          new IIndexDefinition[0], new EntityNameDefinition[0]);
      _filterViewDefinition = new FilterViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "Test"),
          _tableDefinition1,
          new[] { "ClassID" },
          new IColumnDefinition[0],
          new IIndexDefinition[0], new EntityNameDefinition[0]);
      _nullEntityDefinition = new NullEntityDefinition (SchemaGenerationFirstStorageProviderDefinition);
    }

    [Test]
    public void AddEntityDefinition ()
    {
      _constraintBuilder.AddEntityDefinition (_tableDefinition1);

      var createTableScript = _constraintBuilder.GetCreateScript();
      var dropTableScript = _constraintBuilder.GetDropScript();

      Assert.AreEqual (createTableScript, "ADD CONSTRAINT [FK_Order_ID]");
      Assert.AreEqual (dropTableScript, "DROP CONSTRAINT [FK_Order_ID]");
    }

    [Test]
    public void AddEntityDefinition_Twice ()
    {
      _constraintBuilder.AddEntityDefinition (_tableDefinition1);
      _constraintBuilder.AddEntityDefinition (_tableDefinition2);

      var createTableScript = _constraintBuilder.GetCreateScript();
      var dropTableScript = _constraintBuilder.GetDropScript();

      Assert.AreEqual (createTableScript, "ADD CONSTRAINT [FK_Order_ID]\r\nADD CONSTRAINT [FK_Customer_ID]");
      Assert.AreEqual (dropTableScript, "DROP CONSTRAINT [FK_Order_ID]\r\nDROP CONSTRAINT [FK_Customer_ID]");
    }

    [Test]
    public void AddEntityDefinition_UnionViewDefinition ()
    {
      _constraintBuilder.AddEntityDefinition (_unionViewDefinition);

      var createTableScript = _constraintBuilder.GetCreateScript();
      var dropTableScript = _constraintBuilder.GetDropScript();

      Assert.IsEmpty (createTableScript);
      Assert.IsEmpty (dropTableScript);
    }

    [Test]
    public void AddEntityDefinition_FilterViewDefinition ()
    {
      _constraintBuilder.AddEntityDefinition (_filterViewDefinition);

      var createTableScript = _constraintBuilder.GetCreateScript();
      var dropTableScript = _constraintBuilder.GetDropScript();

      Assert.IsEmpty (createTableScript);
      Assert.IsEmpty (dropTableScript);
    }

    [Test]
    public void AddEntityDefinition_NullEntityDefinition ()
    {
      _constraintBuilder.AddEntityDefinition (_nullEntityDefinition);

      var createTableScript = _constraintBuilder.GetCreateScript();
      var dropTableScript = _constraintBuilder.GetDropScript();

      Assert.IsEmpty (createTableScript);
      Assert.IsEmpty (dropTableScript);
    }

    [Test]
    public void GetCreateScript_GetDropScript_NoConstraintsAdded ()
    {
      var createTableScript = _constraintBuilder.GetCreateScript();
      var dropTableScript = _constraintBuilder.GetDropScript();

      Assert.IsEmpty (createTableScript);
      Assert.IsEmpty (dropTableScript);
    }
  }
}