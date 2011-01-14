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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2005;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
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

    // TODO Review 3267: Add to SchemaGeneration testdomain as integration test (FileBuilderTest)
    [Test]
    [Ignore("TODO 3627: Add to SchemaGeneration testdomain as integration test!?")]
    public void AddConstraintWithRelationInDerivedClass ()
    {
      var storageProviderDefinition = new RdbmsProviderDefinition ("DefaultStorageProvider", typeof (SqlStorageObjectFactory), "dummy");
      var baseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (Company), SchemaGenerationFirstStorageProviderDefinition);
      baseClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new PropertyDefinition[0], true));
      var derivedClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          typeof (Customer), SchemaGenerationFirstStorageProviderDefinition, baseClass);
      derivedClass.SetPropertyDefinitions (
          new PropertyDefinitionCollection (
              new[] { CreatePropertyDefinition (derivedClass, "OtherClass", "OtherClassID", typeof (ObjectID), true, null, StorageClass.Persistent) },
              true));

      ReflectionBasedClassDefinition otherClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          typeof (DevelopmentPartner), SchemaGenerationFirstStorageProviderDefinition);
      var endPointDefinition1 = new RelationEndPointDefinition (derivedClass, "OtherClass", false);
      var endPointDefinition2 = new ReflectionBasedVirtualRelationEndPointDefinition (
          otherClass, "DerivedClass", false, CardinalityType.Many, typeof (DomainObjectCollection), "sort", typeof (Employee).GetProperty ("Name"));
      new RelationDefinition ("OtherClassToDerivedClass", endPointDefinition1, endPointDefinition2);

      baseClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection ());
      derivedClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { endPointDefinition1 }, true));
      otherClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { endPointDefinition2 }, true));

      baseClass.SetStorageEntity (
          new TableDefinition (
              storageProviderDefinition, "BaseClassEntity", "BaseClassEntityView", new SimpleColumnDefinition[0], new ITableConstraintDefinition[0]));
      derivedClass.SetStorageEntity (
          new TableDefinition (
              storageProviderDefinition,
              "DerivedClassEntity",
              "DerivedClassEntityView",
              new SimpleColumnDefinition[0],
              new ITableConstraintDefinition[0]));
      otherClass.SetStorageEntity (
          new TableDefinition (
              storageProviderDefinition, "OtherClassEntity", "OtherClassEntityView", new SimpleColumnDefinition[0], new ITableConstraintDefinition[0]));

      baseClass.SetDerivedClasses (new ClassDefinitionCollection (new[] { derivedClass }, true, true));
      derivedClass.SetDerivedClasses (new ClassDefinitionCollection ());
      otherClass.SetDerivedClasses (new ClassDefinitionCollection ());

      _constraintBuilder.AddConstraint (baseClass);

      string expectedScript =
          "ALTER TABLE [dbo].[BaseClassEntity] ADD\r\n"
          + "  CONSTRAINT [FK_BaseClassEntity_OtherClassID] FOREIGN KEY ([OtherClassID]) REFERENCES [dbo].[OtherClassEntity] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript ());
    }

    private PropertyDefinition CreatePropertyDefinition (
        ReflectionBasedClassDefinition classDefinition,
        string propertyName,
        string columnName,
        Type propertyType,
        bool? isNullable,
        int? maxLength,
        StorageClass storageClass)
    {
      PropertyInfo dummyPropertyInfo = typeof (Order).GetProperty ("Number");
      var propertyDefinition = new ReflectionBasedPropertyDefinition (
          classDefinition,
          dummyPropertyInfo,
          propertyName,
          propertyType,
          isNullable,
          maxLength,
          storageClass);
      propertyDefinition.SetStorageProperty (
          new SimpleColumnDefinition (columnName, propertyType, "dummyStorageType", isNullable.HasValue ? isNullable.Value : true, false));
      return propertyDefinition;
    }
  }
}