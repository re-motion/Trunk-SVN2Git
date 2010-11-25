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
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.RdbmsTools.SchemaGeneration.SqlServer;
using Remotion.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.RdbmsTools.UnitTests.SchemaGeneration.SqlServer
{
  [TestFixture]
  public class ConstraintBuilderTest : StandardMappingTest
  {
    private ConstraintBuilder _constraintBuilder;

    public override void SetUp ()
    {
      base.SetUp();

      _constraintBuilder = new ConstraintBuilder();
    }

    [Test]
    public void AddConstraintWithRelationToSameStorageProvider ()
    {
      _constraintBuilder.AddConstraint (OrderItemClass);

      string expectedScript =
          "ALTER TABLE [dbo].[OrderItem] ADD\r\n"
          + "  CONSTRAINT [FK_OrderItem_OrderID] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void AddConstraintWithRelationToOtherStorageProvider ()
    {
      _constraintBuilder.AddConstraint (OrderClass);

      string expectedScript =
          "ALTER TABLE [dbo].[Order] ADD\r\n"
          + "  CONSTRAINT [FK_Order_CustomerID] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void AddConstraintMultipleTimes ()
    {
      _constraintBuilder.AddConstraint (OrderItemClass);
      _constraintBuilder.AddConstraint (OrderClass);

      string expectedScript =
          "ALTER TABLE [dbo].[OrderItem] ADD\r\n"
          + "  CONSTRAINT [FK_OrderItem_OrderID] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order] ([ID])\r\n\r\n"
          + "ALTER TABLE [dbo].[Order] ADD\r\n"
          + "  CONSTRAINT [FK_Order_CustomerID] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void AddConstraintWithTwoConstraints ()
    {
      var firstClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (Company));
      firstClass.MyPropertyDefinitions.Add (
          CreatePropertyDefinition(firstClass, "SecondClass", "SecondClassID", typeof (ObjectID), true, null, StorageClass.Persistent));
      firstClass.MyPropertyDefinitions.Add (
          CreatePropertyDefinition (firstClass, "ThirdClass", "ThirdClassID", typeof (ObjectID), true, null, StorageClass.Persistent));

      var secondClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (Address));
      var thirdClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (Employee));
      
      RelationDefinition relationDefinition1 = new RelationDefinition (
          "FirstClassToSecondClass",
          new RelationEndPointDefinition (firstClass, "SecondClass", false),
          new ReflectionBasedVirtualRelationEndPointDefinition (secondClass, "FirstClass", false, CardinalityType.Many,typeof (DomainObjectCollection),"sort", typeof(Employee).GetProperty("Name")));
      firstClass.MyRelationDefinitions.Add (relationDefinition1);
      secondClass.MyRelationDefinitions.Add (relationDefinition1);

      RelationDefinition relationDefinition2 = new RelationDefinition (
          "FirstClassToThirdClass",
          new RelationEndPointDefinition (firstClass, "ThirdClass", false),
          new ReflectionBasedVirtualRelationEndPointDefinition (thirdClass, "FirstClass", false, CardinalityType.Many, typeof (DomainObjectCollection), "sort", typeof (Employee).GetProperty ("Name")));
      firstClass.MyRelationDefinitions.Add (relationDefinition2);
      thirdClass.MyRelationDefinitions.Add (relationDefinition2);

      firstClass.SetReadOnly ();
      secondClass.SetReadOnly ();
      thirdClass.SetReadOnly ();

      firstClass.SetStorageEntity (new TableDefinition ("DefaultStorageProvider", "FirstEntity", "FirstEntityView", new ColumnDefinition[0]));
      secondClass.SetStorageEntity (new TableDefinition ("DefaultStorageProvider", "SecondEntity", "SecondEntityView", new ColumnDefinition[0]));
      thirdClass.SetStorageEntity (new TableDefinition ("DefaultStorageProvider", "ThirdEntity", "ThirdEntityView", new ColumnDefinition[0]));

      firstClass.SetStorageProviderDefinition (new StorageProviderStubDefinition ("DefaultStorageProvider", typeof (StorageObjectFactoryStub)));
      secondClass.SetStorageProviderDefinition (new StorageProviderStubDefinition ("DefaultStorageProvider", typeof (StorageObjectFactoryStub)));
      thirdClass.SetStorageProviderDefinition (new StorageProviderStubDefinition ("DefaultStorageProvider", typeof (StorageObjectFactoryStub)));

      _constraintBuilder.AddConstraint (firstClass);

      string expectedScript =
          "ALTER TABLE [dbo].[FirstEntity] ADD\r\n"
          + "  CONSTRAINT [FK_FirstEntity_SecondClassID] FOREIGN KEY ([SecondClassID]) REFERENCES [dbo].[SecondEntity] ([ID]),\r\n"
          + "  CONSTRAINT [FK_FirstEntity_ThirdClassID] FOREIGN KEY ([ThirdClassID]) REFERENCES [dbo].[ThirdEntity] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript());
    }

    private PropertyDefinition CreatePropertyDefinition (ReflectionBasedClassDefinition classDefinition, string propertyName, string columnName,
        Type propertyType, bool? isNullable, int? maxLength, StorageClass storageClass)
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
      propertyDefinition.SetStorageProperty (new ColumnDefinition (columnName, propertyType, "dummyStorageType", isNullable.HasValue?isNullable.Value:true));
      return propertyDefinition;
    }

    [Test]
    public void AddConstraintWithNoConstraintNecessary ()
    {
      _constraintBuilder.AddConstraint (MappingConfiguration.ClassDefinitions.GetMandatory ("Official"));
      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void AddConstraintWithRelationInDerivedClass ()
    {
      var baseClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (Company));
      var derivedClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (Customer), baseClass);
      derivedClass.MyPropertyDefinitions.Add (
          CreatePropertyDefinition (derivedClass, "OtherClass", "OtherClassID", typeof (ObjectID), true, null, StorageClass.Persistent));

      ReflectionBasedClassDefinition otherClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (DevelopmentPartner));
      RelationDefinition relationDefinition1 = new RelationDefinition (
          "OtherClassToDerivedClass",
          new RelationEndPointDefinition (derivedClass, "OtherClass", false),
          new ReflectionBasedVirtualRelationEndPointDefinition (otherClass, "DerivedClass", false, CardinalityType.Many, typeof (DomainObjectCollection), "sort", typeof (Employee).GetProperty ("Name")));

      derivedClass.MyRelationDefinitions.Add (relationDefinition1);
      otherClass.MyRelationDefinitions.Add (relationDefinition1);

      baseClass.SetReadOnly ();
      derivedClass.SetReadOnly ();
      otherClass.SetReadOnly ();

      baseClass.SetStorageEntity (new TableDefinition ("DefaultStorageProvider", "BaseClassEntity", "BaseClassEntityView", new ColumnDefinition[0]));
      derivedClass.SetStorageEntity (new TableDefinition ("DefaultStorageProvider", "DerivedClassEntity", "DerivedClassEntityView", new ColumnDefinition[0]));
      otherClass.SetStorageEntity (new TableDefinition ("DefaultStorageProvider", "OtherClassEntity", "OtherClassEntityView", new ColumnDefinition[0]));

      baseClass.SetStorageProviderDefinition (new StorageProviderStubDefinition ("DefaultStorageProvider", typeof (StorageObjectFactoryStub)));
      derivedClass.SetStorageProviderDefinition (new StorageProviderStubDefinition ("DefaultStorageProvider", typeof (StorageObjectFactoryStub)));
      otherClass.SetStorageProviderDefinition (new StorageProviderStubDefinition ("DefaultStorageProvider", typeof (StorageObjectFactoryStub)));
      
      _constraintBuilder.AddConstraint (baseClass);

      string expectedScript =
          "ALTER TABLE [dbo].[BaseClassEntity] ADD\r\n"
          + "  CONSTRAINT [FK_BaseClassEntity_OtherClassID] FOREIGN KEY ([OtherClassID]) REFERENCES [dbo].[OtherClassEntity] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void AddConstraintWithRelationToDerivedOfConcreteClass ()
    {
      _constraintBuilder.AddConstraint (ClassWithRelations);

      string expectedScript =
          "ALTER TABLE [dbo].[TableWithRelations] ADD\r\n"
          + "  CONSTRAINT [FK_TableWithRelations_DerivedClassID] FOREIGN KEY ([DerivedClassID]) REFERENCES [dbo].[ConcreteClass] ([ID])\r\n";

      Assert.AreEqual (expectedScript, _constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void AddConstraintWithRelationToAbstractClass ()
    {
      _constraintBuilder.AddConstraint (CeoClass);

      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript());
    }

    [Test]
    public void AddConstraintWithAbstractClass ()
    {
      _constraintBuilder.AddConstraint (CompanyClass);

      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript());
      Assert.IsEmpty (_constraintBuilder.GetDropConstraintScript());
    }

    [Test]
    public void AddConstraintWithDerivedClassWithEntityName ()
    {
      _constraintBuilder.AddConstraint (SecondDerivedClass);

      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript());
      Assert.IsEmpty (_constraintBuilder.GetDropConstraintScript());
    }

    [Test]
    public void AddConstraintWithDerivedOfDerivedClassWithEntityName ()
    {
      _constraintBuilder.AddConstraint (DerivedOfDerivedClass);

      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript());
      Assert.IsEmpty (_constraintBuilder.GetDropConstraintScript());
    }

    [Test]
    public void AddConstraints ()
    {
      ClassDefinitionCollection classes = new ClassDefinitionCollection (false);
      classes.Add (OrderItemClass);
      classes.Add (OrderClass);

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

      Assert.IsEmpty (_constraintBuilder.GetAddConstraintScript ());
    }

    [Test]
    public void GetDropConstraintsScript ()
    {
      _constraintBuilder.AddConstraint (ClassWithRelations);

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

      Assert.IsEmpty (_constraintBuilder.GetDropConstraintScript ());
    }

    [Test]
    public void GetDropConstraintsScriptWithMultipleEntities ()
    {
      _constraintBuilder.AddConstraint (ClassWithRelations);
      _constraintBuilder.AddConstraint (ConcreteClass);

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
