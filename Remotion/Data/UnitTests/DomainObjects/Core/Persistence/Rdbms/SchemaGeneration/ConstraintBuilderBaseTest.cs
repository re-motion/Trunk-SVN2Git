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
  public class ConstraintBuilderBaseTest : SchemaGenerationTestBase
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
          new ITableConstraintDefinition[0]);
      _tableDefinition2 = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "Customer"),
          null,
          new IColumnDefinition[0],
          new ITableConstraintDefinition[0]);
      _unionViewDefinition = new UnionViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "Test"),
          new[] { _tableDefinition1 },
          new IColumnDefinition[0]);
      _filterViewDefinition = new FilterViewDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "Test"),
          _tableDefinition1,
          new[] { "ClassID" },
          new IColumnDefinition[0]);
      _nullEntityDefinition = new NullEntityDefinition (SchemaGenerationFirstStorageProviderDefinition);
    }

    [Test]
    public void AddConstraint ()
    {
      _constraintBuilder.AddConstraint (_tableDefinition1);

      var createTableScript = _constraintBuilder.GetAddConstraintScript();
      var dropTableScript = _constraintBuilder.GetDropConstraintScript();

      Assert.AreEqual (createTableScript, "ADD CONSTRAINT [FK_Order_ID]");
      Assert.AreEqual (dropTableScript, "DROP CONSTRAINT [Order]");
    }

    [Test]
    public void AddConstraint_Twice ()
    {
      _constraintBuilder.AddConstraint (_tableDefinition1);
      _constraintBuilder.AddConstraint (_tableDefinition2);

      var createTableScript = _constraintBuilder.GetAddConstraintScript();
      var dropTableScript = _constraintBuilder.GetDropConstraintScript();

      Assert.AreEqual (createTableScript, "ADD CONSTRAINT [FK_Order_ID]\r\nADD CONSTRAINT [FK_Customer_ID]");
      Assert.AreEqual (dropTableScript, "DROP CONSTRAINT [Order, Customer]");
    }

    [Test]
    public void AddConstraint_UnionViewDefinition ()
    {
      _constraintBuilder.AddConstraint (_unionViewDefinition);

      var createTableScript = _constraintBuilder.GetAddConstraintScript();
      var dropTableScript = _constraintBuilder.GetDropConstraintScript();

      Assert.IsEmpty (createTableScript);
      Assert.IsEmpty (dropTableScript);
    }

    [Test]
    public void AddConstraint_FilterViewDefinition ()
    {
      _constraintBuilder.AddConstraint (_filterViewDefinition);

      var createTableScript = _constraintBuilder.GetAddConstraintScript();
      var dropTableScript = _constraintBuilder.GetDropConstraintScript();

      Assert.IsEmpty (createTableScript);
      Assert.IsEmpty (dropTableScript);
    }

    [Test]
    public void AddConstraint_NullEntityDefinition ()
    {
      _constraintBuilder.AddConstraint (_nullEntityDefinition);

      var createTableScript = _constraintBuilder.GetAddConstraintScript();
      var dropTableScript = _constraintBuilder.GetDropConstraintScript();

      Assert.IsEmpty (createTableScript);
      Assert.IsEmpty (dropTableScript);
    }

    [Test]
    public void GetCreateScript_GetDropScript_NoConstraintsAdded ()
    {
      var createTableScript = _constraintBuilder.GetAddConstraintScript();
      var dropTableScript = _constraintBuilder.GetDropConstraintScript();

      Assert.IsEmpty (createTableScript);
      Assert.IsEmpty (dropTableScript);
    }
  }
}