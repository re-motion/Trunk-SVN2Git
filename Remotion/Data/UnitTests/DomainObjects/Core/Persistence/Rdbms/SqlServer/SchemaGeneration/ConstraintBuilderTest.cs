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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGenerationTestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class ConstraintBuilderTest : SchemaGenerationTestBase
  {
    private ConstraintBuilder _constraintBuilder;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _constraintBuilder = new ConstraintBuilder();
    }

    [Test]
    public void AddConstraintWithRelationToSameStorageProvider ()
    {
      _constraintBuilder.AddConstraint (MappingConfiguration.ClassDefinitions[typeof (OrderItem)]);

      string expectedScript =
          "ALTER TABLE [dbo].[OrderItem] ADD\r\n"
          + "  CONSTRAINT [FK_OrderItem_OrderID] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void AddConstraintWithRelationToOtherStorageProvider ()
    {
      _constraintBuilder.AddConstraint (MappingConfiguration.ClassDefinitions[typeof (Order)]);

      string expectedScript =
          "ALTER TABLE [dbo].[Order] ADD\r\n"
          + "  CONSTRAINT [FK_Order_CustomerID] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void AddConstraintMultipleTimes ()
    {
      _constraintBuilder.AddConstraint (MappingConfiguration.ClassDefinitions[typeof (OrderItem)]);
      _constraintBuilder.AddConstraint (MappingConfiguration.ClassDefinitions[typeof (Order)]);

      string expectedScript =
          "ALTER TABLE [dbo].[OrderItem] ADD\r\n"
          + "  CONSTRAINT [FK_OrderItem_OrderID] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order] ([ID])\r\n\r\n"
          + "ALTER TABLE [dbo].[Order] ADD\r\n"
          + "  CONSTRAINT [FK_Order_CustomerID] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void AddConstraintWithNoConstraintNecessary ()
    {
      _constraintBuilder.AddConstraint (MappingConfiguration.ClassDefinitions.GetMandatory (typeof (Official)));
      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void AddConstraintWithRelationToDerivedOfConcreteClass ()
    {
      _constraintBuilder.AddConstraint (MappingConfiguration.ClassDefinitions[typeof (ClassWithRelations)]);

      string expectedScript =
          "ALTER TABLE [dbo].[TableWithRelations] ADD\r\n"
          + "  CONSTRAINT [FK_TableWithRelations_DerivedClassID] FOREIGN KEY ([DerivedClassID]) REFERENCES [dbo].[ConcreteClass] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void AddConstraintWithRelationToAbstractClass ()
    {
      _constraintBuilder.AddConstraint (MappingConfiguration.ClassDefinitions[typeof (Ceo)]);

      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void AddConstraintWithAbstractClass ()
    {
      _constraintBuilder.AddConstraint (MappingConfiguration.ClassDefinitions[typeof (Company)]);

      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript());
      Assert.IsEmpty (_constraintBuilder.GetDropConstraintScript());
    }

    [Test]
    public void AddConstraintWithDerivedClassWithEntityName ()
    {
      _constraintBuilder.AddConstraint (MappingConfiguration.ClassDefinitions[typeof (SecondDerivedClass)]);

      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript());
      Assert.IsEmpty (_constraintBuilder.GetDropConstraintScript());
    }

    [Test]
    public void AddConstraintWithDerivedOfDerivedClassWithEntityName ()
    {
      _constraintBuilder.AddConstraint (MappingConfiguration.ClassDefinitions[typeof (DerivedOfDerivedClass)]);

      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript());
      Assert.IsEmpty (_constraintBuilder.GetDropConstraintScript());
    }

    [Test]
    public void AddConstraints ()
    {
      ClassDefinitionCollection classes = new ClassDefinitionCollection (false);
      classes.Add (MappingConfiguration.ClassDefinitions[typeof (OrderItem)]);
      classes.Add (MappingConfiguration.ClassDefinitions[typeof (Order)]);

      _constraintBuilder.AddConstraints (classes);

      string expectedScript =
          "ALTER TABLE [dbo].[OrderItem] ADD\r\n"
          + "  CONSTRAINT [FK_OrderItem_OrderID] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order] ([ID])\r\n\r\n"
          + "ALTER TABLE [dbo].[Order] ADD\r\n"
          + "  CONSTRAINT [FK_Order_CustomerID] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void AddConstraints_WithoutClassDefinitions ()
    {
      ClassDefinitionCollection classes = new ClassDefinitionCollection (false);

      _constraintBuilder.AddConstraints (classes);

      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void GetDropConstraintsScript ()
    {
      _constraintBuilder.AddConstraint (MappingConfiguration.ClassDefinitions[typeof (ClassWithRelations)]);

      string expectedScript =
          "DECLARE @statement nvarchar (4000)\r\n"
          + "SET @statement = ''\r\n"
          + "SELECT @statement = @statement + 'ALTER TABLE [dbo].[' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' \r\n"
          + "    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id \r\n"
          + "    WHERE fk.xtype = 'F' AND t.name IN ('TableWithRelations')\r\n"
          + "    ORDER BY t.name, fk.name\r\n"
          + "exec sp_executesql @statement\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetDropConstraintScript());
    }

    [Test]
    public void GetDropConstraintsScript_WithoutClasses ()
    {
      ClassDefinitionCollection classes = new ClassDefinitionCollection (false);

      _constraintBuilder.AddConstraints (classes);

      Assert.IsEmpty (_constraintBuilder.GetDropConstraintScript());
    }

    [Test]
    public void GetDropConstraintsScriptWithMultipleEntities ()
    {
      _constraintBuilder.AddConstraint (MappingConfiguration.ClassDefinitions[typeof (ClassWithRelations)]);
      _constraintBuilder.AddConstraint (MappingConfiguration.ClassDefinitions[typeof (ConcreteClass)]);

      string expectedScript =
          "DECLARE @statement nvarchar (4000)\r\n"
          + "SET @statement = ''\r\n"
          + "SELECT @statement = @statement + 'ALTER TABLE [dbo].[' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' \r\n"
          + "    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id \r\n"
          + "    WHERE fk.xtype = 'F' AND t.name IN ('TableWithRelations', 'ConcreteClass')\r\n"
          + "    ORDER BY t.name, fk.name\r\n"
          + "exec sp_executesql @statement\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetDropConstraintScript());
    }
  }
}