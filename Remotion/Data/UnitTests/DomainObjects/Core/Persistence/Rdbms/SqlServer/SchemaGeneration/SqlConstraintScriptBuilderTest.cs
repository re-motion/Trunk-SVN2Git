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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class SqlConstraintScriptBuilderTest : SchemaGenerationTestBase
  {
    private SqlConstraintScriptBuilder _constraintBuilder;
    private SimpleColumnDefinition _referencedColumn1;
    private SimpleColumnDefinition _referencedColumn2;
    private SimpleColumnDefinition _referencingColumn;
    private ForeignKeyConstraintDefinition _foreignKeyConstraintDefinition1;
    private ForeignKeyConstraintDefinition _foreignKeyConstraintDefinition2;
    private TableDefinition _tableDefinition1;
    private TableDefinition _tableDefinition2;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _constraintBuilder = new SqlConstraintScriptBuilder();

      _referencedColumn1 = new SimpleColumnDefinition ("OrderID", typeof (int), "integer", true, false);
      _referencedColumn2 = new SimpleColumnDefinition ("CustomerID", typeof (int), "integer", true, false);

      _referencingColumn = new SimpleColumnDefinition ("ID", typeof (int), "integer", false, true);
      _foreignKeyConstraintDefinition1 = new ForeignKeyConstraintDefinition (
          "FK_OrderItem_OrderID", new EntityNameDefinition(null, "Order"), new[] { _referencingColumn }, new[] { _referencedColumn1 });
      _foreignKeyConstraintDefinition2 = new ForeignKeyConstraintDefinition (
          "FK_OrderItem_CustomerID", new EntityNameDefinition(null, "Customer"), new[] { _referencingColumn }, new[] { _referencedColumn2 });
      _tableDefinition1 = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "OrderItem"),
          null,
          new[] { _referencingColumn },
          new[] { _foreignKeyConstraintDefinition1 },
          new IIndexDefinition[0], new EntityNameDefinition[0]);
      _tableDefinition2 = new TableDefinition (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition("Test", "Customer"),
          null,
          new[] { _referencingColumn },
          new[] { _foreignKeyConstraintDefinition1, _foreignKeyConstraintDefinition2 },
          new IIndexDefinition[0], new EntityNameDefinition[0]);
    }

    [Test]
    public void GetAddConstraintScript_OneConstraint ()
    {
      _constraintBuilder.AddEntityDefinition (_tableDefinition1);

      var expectedScript =
          "-- Create constraints for tables that were created above\r\n"
          +"ALTER TABLE [dbo].[OrderItem] ADD\r\n"
          + "  CONSTRAINT [FK_OrderItem_OrderID] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetCreateScript());
    }

    [Test]
    public void GetAddConstraintScript_MultipleConstraints ()
    {
      _constraintBuilder.AddEntityDefinition (_tableDefinition2);

      var expectedScript =
          "-- Create constraints for tables that were created above\r\n"
          +"ALTER TABLE [Test].[Customer] ADD\r\n"
          + "  CONSTRAINT [FK_OrderItem_OrderID] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order] ([ID]),\r\n"
          + "  CONSTRAINT [FK_OrderItem_CustomerID] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([ID])\r\n"; //TODO 3862: wrong references schema - must be also changed in ExtendedFileBuilder!?

      Assert.AreEqual (expectedScript, _constraintBuilder.GetCreateScript());
    }

    [Test]
    public void GetAddConstraintScript_NoConstraint ()
    {
      Assert.That (_constraintBuilder.GetCreateScript (), Is.EqualTo ("-- Create constraints for tables that were created above\r\n"));
    }

    // TODO Review 3936: Duplicate test; should be changed to GetDropScript?
    [Test]
    public void GetAddConstraintScript_NoConstraintsAdded ()
    {
      Assert.That (_constraintBuilder.GetCreateScript (), Is.EqualTo ("-- Create constraints for tables that were created above\r\n"));
    }

    // TODO Review 3936: Also add tests for GetDropConstraintsScript_OneConstraint, MultipleConstraints
    // TODO Review 3936: Make the tests for GetAddConstraintScript and GetDropConstraintScript symmetrical (equal test cases, equal names for both script variants)

    // TODO Review 3936: Also add test for GetAddConstraintsScript_DefaultSchema
    [Test]
    public void GetDropConstraintScript_DefaultSchema ()
    {
      _constraintBuilder.AddEntityDefinition (_tableDefinition1);

      var expectedScript =
          "-- Drop foreign keys of all tables that will be created below\r\n"
          +"IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id where fk.type = 'F' AND "
          +"fk.name = 'FK_OrderItem_OrderID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'OrderItem')\r\n"
          +"  ALTER TABLE [dbo].[OrderItem] DROP CONSTRAINT FK_OrderItem_OrderID\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetDropScript());
    }

    // TODO Review 3936: Also add test for GetAddConstraintsScript_WithMultipleEntities
    [Test]
    public void GetDropConstraintsScript_WithMultipleEntities ()
    {
      _constraintBuilder.AddEntityDefinition (_tableDefinition1);
      _constraintBuilder.AddEntityDefinition (_tableDefinition2);

      var expectedScript =
          "-- Drop foreign keys of all tables that will be created below\r\n"
          +"IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id where fk.type = 'F' AND "
          +"fk.name = 'FK_OrderItem_OrderID' AND schema_name (t.schema_id) = 'dbo' AND t.name = 'OrderItem')\r\n"
          +"  ALTER TABLE [dbo].[OrderItem] DROP CONSTRAINT FK_OrderItem_OrderID\r\n\r\n"
          +"IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id where fk.type = 'F' AND "
          +"fk.name = 'FK_OrderItem_OrderID' AND schema_name (t.schema_id) = 'Test' AND t.name = 'Customer')\r\n"
          +"  ALTER TABLE [Test].[Customer] DROP CONSTRAINT FK_OrderItem_OrderID\r\n\r\n"
          +"IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id where fk.type = 'F' AND "
          +"fk.name = 'FK_OrderItem_CustomerID' AND schema_name (t.schema_id) = 'Test' AND t.name = 'Customer')\r\n"
          +"  ALTER TABLE [Test].[Customer] DROP CONSTRAINT FK_OrderItem_CustomerID\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetDropScript());
    }
  }
}