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
using System.Text;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class TableBuilderTest : SchemaGenerationTestBase
  {
    private enum Int32Enum
    {
    }

    private enum Int16Enum : short
    {
    }

    [InternalStorageGroup]
    [DBTable]
    [ClassID("TableBuilderTest_AbstractClass")]
    private abstract class AbstractClass : DomainObject
    {
    }

    private abstract class DerivedAbstractClass : AbstractClass
    {
    }

    private class DerivedConcreteClass : DerivedAbstractClass
    {
    }

    private TableBuilder _tableBuilder;
    private ReflectionBasedClassDefinition _classDefintion;

    public override void SetUp ()
    {
      base.SetUp();

      _tableBuilder = new TableBuilder();
      _classDefintion = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (Order));
    }

    [Test]
    public void GetSqlDataType ()
    {
      Assert.AreEqual ("bit", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Boolean), null, null)));
      Assert.AreEqual ("tinyint", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Byte), null, null)));
      Assert.AreEqual ("datetime", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (DateTime), null, null)));
      Assert.AreEqual ("decimal (38, 3)", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Decimal), null, null)));
      Assert.AreEqual ("float", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Double), null, null)));
      Assert.AreEqual ("uniqueidentifier", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Guid), null, null)));
      Assert.AreEqual ("smallint", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Int16), null, null)));
      Assert.AreEqual ("int", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Int32), null, null)));
      Assert.AreEqual ("bigint", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Int64), null, null)));
      Assert.AreEqual ("real", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Single), null, null)));

      Assert.AreEqual ("int", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Int32Enum), null, null)));
      Assert.AreEqual ("smallint", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Int16Enum), null, null)));

      Assert.AreEqual ("nvarchar (200)", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (String), false, 200)));
      Assert.AreEqual ("nvarchar (max)", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (String), false, null)));

      Assert.AreEqual ("varbinary (200)", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Byte[]), false, 200)));
      Assert.AreEqual ("varbinary (max)", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Byte[]), false, null)));
    }

    [Test]
    public void GetSqlDataType_ForNullableValueTypes ()
    {
      Assert.AreEqual ("bit", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Nullable<Boolean>), null, null)));
      Assert.AreEqual ("tinyint", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Nullable<Byte>), null, null)));
      Assert.AreEqual ("datetime", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Nullable<DateTime>), null, null)));
      Assert.AreEqual ("decimal (38, 3)", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Nullable<Decimal>), null, null)));
      Assert.AreEqual ("float", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Nullable<Double>), null, null)));
      Assert.AreEqual ("uniqueidentifier", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Nullable<Guid>), null, null)));
      Assert.AreEqual ("smallint", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Nullable<Int16>), null, null)));
      Assert.AreEqual ("int", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Nullable<Int32>), null, null)));
      Assert.AreEqual ("bigint", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Nullable<Int64>), null, null)));
      Assert.AreEqual ("real", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Nullable<Single>), null, null)));

      Assert.AreEqual ("int", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Nullable<Int32Enum>), null, null)));
      Assert.AreEqual ("smallint", _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Nullable<Int16Enum>), null, null)));
    }

    //TODO: Copy to TableBuilderBaseTest
    [Test]
    public void GetSqlDataTypeForSpecialCulumns ()
    {
      Assert.AreEqual (
          "uniqueidentifier",
          _tableBuilder.GetSqlDataType (
              MappingConfiguration.ClassDefinitions[typeof (OrderItem)].GetMandatoryPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration.TestDomain.OrderItem.Order")));
      Assert.AreEqual (
          "varchar (255)",
          _tableBuilder.GetSqlDataType (
              MappingConfiguration.ClassDefinitions[typeof (Customer)].GetMandatoryPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration.TestDomain.Customer.PrimaryOfficial")));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Data type 'System.Char' is not supported.\r\nDeclaring type: 'Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration.TestDomain.Order'\r\nProperty: 'Name'")]
    public void GetSqlDataType_WithNotSupportedType ()
    {
      _tableBuilder.GetSqlDataType (CreatePropertyDefinition (typeof (Char), null, null));
    }

    //TODO: Move to TableBuilderBaseTest
    [Test]
    public void AddToCreateTableScript ()
    {
      string expectedStatement =
          "CREATE TABLE [dbo].[SchemaGeneration_Ceo]\r\n"
          + "(\r\n"
          + "  [ID] uniqueidentifier NOT NULL,\r\n"
          + "  [ClassID] varchar (100) NOT NULL,\r\n"
          + "  [Timestamp] rowversion NOT NULL,\r\n\r\n"
          + "  -- SchemaGeneration_Ceo columns\r\n"
          + "  [Name] nvarchar (100) NOT NULL,\r\n"
          + "  [CompanyID] uniqueidentifier NULL,\r\n"
          + "  [CompanyIDClassID] varchar (100) NULL,\r\n\r\n"
          + "  CONSTRAINT [PK_SchemaGeneration_Ceo] PRIMARY KEY CLUSTERED ([ID])\r\n"
          + ")\r\n";
      StringBuilder stringBuilder = new StringBuilder();

      _tableBuilder.AddToCreateTableScript (MappingConfiguration.ClassDefinitions[typeof (Ceo)], stringBuilder);

      Assert.AreEqual (expectedStatement, stringBuilder.ToString());
    }

    //TODO: Move to TableBuilderBaseTest
    [Test]
    public void AddToCreateTableScriptWithConcreteClass ()
    {
      string expectedStatement =
          "CREATE TABLE [dbo].[SchemaGeneration_Customer]\r\n"
          + "(\r\n"
          + "  [ID] uniqueidentifier NOT NULL,\r\n"
          + "  [ClassID] varchar (100) NOT NULL,\r\n"
          + "  [Timestamp] rowversion NOT NULL,\r\n\r\n"
          + "  -- SchemaGeneration_Company columns\r\n"
          + "  [Name] nvarchar (100) NOT NULL,\r\n"
          + "  [PhoneNumber] nvarchar (100) NULL,\r\n"
          + "  [AddressID] uniqueidentifier NULL,\r\n\r\n"
          + "  -- SchemaGeneration_Customer columns\r\n"
          + "  [CustomerType] int NOT NULL,\r\n"
          + "  [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches] nvarchar (100) NOT NULL,\r\n"
          + "  [PrimaryOfficialID] varchar (255) NULL,\r\n"
          + "  [LicenseCode] nvarchar (max) NULL,\r\n\r\n"
          + "  CONSTRAINT [PK_SchemaGeneration_Customer] PRIMARY KEY CLUSTERED ([ID])\r\n"
          + ")\r\n";
      StringBuilder stringBuilder = new StringBuilder();

      _tableBuilder.AddToCreateTableScript (MappingConfiguration.ClassDefinitions[typeof (Customer)], stringBuilder);

      Assert.AreEqual (expectedStatement, stringBuilder.ToString());
    }

    //TODO: Move to TableBuilderBaseTest
    [Test]
    public void AddToCreateTableScriptWithTwoAbstractBaseClasses ()
    {
      var storageProviderDefinition = new RdbmsProviderDefinition ("DefaultStorageProvider", typeof (SqlStorageObjectFactory), "dummy");
      var abstractClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (AbstractClass));
      abstractClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{
          CreatePropertyDefinition (abstractClass, "PropertyInAbstractClass", "PropertyInAbstractClass", typeof (string), true, 100, StorageClass.Persistent)}, true));
      
      var derivedAbstractClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (DerivedAbstractClass), abstractClass);
      derivedAbstractClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{
          CreatePropertyDefinition (
              derivedAbstractClass,
              "PropertyInAbstractDerivedClass",
              "PropertyInAbstractDerivedClass",
              typeof (string),
              false,
              101,
              StorageClass.Persistent)}, true));
      
      var derivedConcreteClass = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          typeof (DerivedConcreteClass), derivedAbstractClass);
      derivedConcreteClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[]{
          CreatePropertyDefinition (
              derivedConcreteClass,
              "PropertyInDerivedConcreteClass",
              "PropertyInDerivedConcreteClass",
              typeof (string),
              true,
              102,
              StorageClass.Persistent)}, true));
      derivedConcreteClass.SetStorageEntity (new TableDefinition (storageProviderDefinition, "EntityName", "ViewName", new ColumnDefinition[0]));

      abstractClass.SetDerivedClasses (new ClassDefinitionCollection (new[] { derivedAbstractClass }, true, true));
      derivedAbstractClass.SetDerivedClasses (new ClassDefinitionCollection (new[] { derivedConcreteClass }, true, true));
      derivedConcreteClass.SetDerivedClasses (new ClassDefinitionCollection());

      abstractClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      derivedAbstractClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      derivedConcreteClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());

      string expectedStatement =
          "CREATE TABLE [dbo].[EntityName]\r\n"
          + "(\r\n"
          + "  [ID] uniqueidentifier NOT NULL,\r\n"
          + "  [ClassID] varchar (100) NOT NULL,\r\n"
          + "  [Timestamp] rowversion NOT NULL,\r\n\r\n"
          + "  -- AbstractClass columns\r\n"
          + "  [PropertyInAbstractClass] nvarchar (100) NULL,\r\n\r\n"
          + "  -- DerivedAbstractClass columns\r\n"
          + "  [PropertyInAbstractDerivedClass] nvarchar (101) NOT NULL,\r\n\r\n"
          + "  -- DerivedConcreteClass columns\r\n"
          + "  [PropertyInDerivedConcreteClass] nvarchar (102) NULL,\r\n\r\n"
          + "  CONSTRAINT [PK_EntityName] PRIMARY KEY CLUSTERED ([ID])\r\n"
          + ")\r\n";
      StringBuilder stringBuilder = new StringBuilder();

      _tableBuilder.AddToCreateTableScript (derivedConcreteClass, stringBuilder);

      Assert.AreEqual (expectedStatement, stringBuilder.ToString());
    }

    //TODO: Move to TableBuilderBaseTest
    [Test]
    public void AddToCreateTableScriptWithDerivedClasses ()
    {
      string expectedStatement = "CREATE TABLE [dbo].[SchemaGeneration_ConcreteClass]\r\n"
                                 + "(\r\n"
                                 + "  [ID] uniqueidentifier NOT NULL,\r\n"
                                 + "  [ClassID] varchar (100) NOT NULL,\r\n"
                                 + "  [Timestamp] rowversion NOT NULL,\r\n\r\n"
                                 + "  -- SchemaGeneration_ConcreteClass columns\r\n"
                                 + "  [PropertyInConcreteClass] nvarchar (100) NOT NULL,\r\n\r\n"
                                 + "  -- SchemaGeneration_DerivedClass columns\r\n"
                                 + "  [PropertyInDerivedClass] nvarchar (100) NULL,\r\n"
                                 + "  [PersistentProperty] nvarchar (max) NULL,\r\n\r\n"
                                 + "  -- SchemaGeneration_DerivedOfDerivedClass columns\r\n"
                                 + "  [PropertyInDerivedOfDerivedClass] nvarchar (100) NULL,\r\n"
                                 + "  [ClassWithRelationsInDerivedOfDerivedClassID] uniqueidentifier NULL,\r\n\r\n"
                                 + "  -- SchemaGeneration_SecondDerivedClass columns\r\n"
                                 + "  [PropertyInSecondDerivedClass] nvarchar (100) NULL,\r\n"
                                 + "  [ClassWithRelationsInSecondDerivedClassID] uniqueidentifier NULL,\r\n\r\n"
                                 + "  CONSTRAINT [PK_SchemaGeneration_ConcreteClass] PRIMARY KEY CLUSTERED ([ID])\r\n"
                                 + ")\r\n";
      StringBuilder stringBuilder = new StringBuilder();

      _tableBuilder.AddToCreateTableScript (MappingConfiguration.ClassDefinitions.GetMandatory ("SchemaGeneration_ConcreteClass"), stringBuilder);

      Assert.AreEqual (expectedStatement, stringBuilder.ToString());
    }

    //TODO: Move to TableBuilderBaseTest
    [Test]
    public void AddToCreateTableScriptWithRelationToClassWithoutInheritance ()
    {
      string expectedStatement = "CREATE TABLE [dbo].[SchemaGeneration_OrderItem]\r\n"
                                 + "(\r\n"
                                 + "  [ID] uniqueidentifier NOT NULL,\r\n"
                                 + "  [ClassID] varchar (100) NOT NULL,\r\n"
                                 + "  [Timestamp] rowversion NOT NULL,\r\n\r\n"
                                 + "  -- SchemaGeneration_OrderItem columns\r\n"
                                 + "  [Position] int NOT NULL,\r\n"
                                 + "  [Product] nvarchar (100) NOT NULL,\r\n"
                                 + "  [OrderID] uniqueidentifier NULL,\r\n\r\n"
                                 + "  CONSTRAINT [PK_SchemaGeneration_OrderItem] PRIMARY KEY CLUSTERED ([ID])\r\n"
                                 + ")\r\n";
      StringBuilder stringBuilder = new StringBuilder();

      _tableBuilder.AddToCreateTableScript (MappingConfiguration.ClassDefinitions[typeof (OrderItem)], stringBuilder);

      Assert.AreEqual (expectedStatement, stringBuilder.ToString());
    }

    [Test]
    public void AddToCreateTableScriptWithClassWithoutProperties ()
    {
      string expectedStatement =
          "CREATE TABLE [dbo].[TableWithoutProperties]\r\n"
          + "(\r\n"
          + "  [ID] uniqueidentifier NOT NULL,\r\n"
          + "  [ClassID] varchar (100) NOT NULL,\r\n"
          + "  [Timestamp] rowversion NOT NULL,\r\n\r\n"
          + "  -- SchemaGeneration_ClassWithoutProperties columns\r\n\r\n"
          + "  CONSTRAINT [PK_TableWithoutProperties] PRIMARY KEY CLUSTERED ([ID])\r\n"
          + ")\r\n";
      StringBuilder stringBuilder = new StringBuilder();

      _tableBuilder.AddToCreateTableScript (MappingConfiguration.ClassDefinitions[typeof (ClassWithoutProperties)], stringBuilder);

      Assert.AreEqual (expectedStatement, stringBuilder.ToString());
    }

    [Test]
    public void AddToDropTableScript ()
    {
      string expectedScript = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SchemaGeneration_Customer' AND TABLE_SCHEMA = 'dbo')\r\n"
                              + "  DROP TABLE [dbo].[SchemaGeneration_Customer]\r\n";
      StringBuilder stringBuilder = new StringBuilder();

      _tableBuilder.AddToDropTableScript (MappingConfiguration.ClassDefinitions[typeof (Customer)], stringBuilder);

      Assert.AreEqual (expectedScript, stringBuilder.ToString());
    }

    [Test]
    public void IntegrationTest ()
    {
      ClassDefinitionCollection classes = new ClassDefinitionCollection (false);
      classes.Add (MappingConfiguration.ClassDefinitions[typeof (Customer)]);
      classes.Add (MappingConfiguration.ClassDefinitions[typeof (Order)]);

      _tableBuilder.AddTables (classes);

      string expectedCreateTableScript = "CREATE TABLE [dbo].[SchemaGeneration_Customer]\r\n"
                                         + "(\r\n"
                                         + "  [ID] uniqueidentifier NOT NULL,\r\n"
                                         + "  [ClassID] varchar (100) NOT NULL,\r\n"
                                         + "  [Timestamp] rowversion NOT NULL,\r\n\r\n"
                                         + "  -- SchemaGeneration_Company columns\r\n"
                                         + "  [Name] nvarchar (100) NOT NULL,\r\n"
                                         + "  [PhoneNumber] nvarchar (100) NULL,\r\n"
                                         + "  [AddressID] uniqueidentifier NULL,\r\n\r\n"
                                         + "  -- SchemaGeneration_Customer columns\r\n"
                                         + "  [CustomerType] int NOT NULL,\r\n"
                                         + "  [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches] nvarchar (100) NOT NULL,\r\n"
                                         + "  [PrimaryOfficialID] varchar (255) NULL,\r\n"
                                         + "  [LicenseCode] nvarchar (max) NULL,\r\n\r\n"
                                         + "  CONSTRAINT [PK_SchemaGeneration_Customer] PRIMARY KEY CLUSTERED ([ID])\r\n"
                                         + ")\r\n\r\n"
                                         + "CREATE TABLE [dbo].[SchemaGeneration_Order]\r\n"
                                         + "(\r\n"
                                         + "  [ID] uniqueidentifier NOT NULL,\r\n"
                                         + "  [ClassID] varchar (100) NOT NULL,\r\n"
                                         + "  [Timestamp] rowversion NOT NULL,\r\n\r\n"
                                         + "  -- SchemaGeneration_Order columns\r\n"
                                         + "  [Number] int NOT NULL,\r\n"
                                         + "  [Priority] int NOT NULL,\r\n"
                                         + "  [CustomerID] uniqueidentifier NULL,\r\n"
                                         + "  [CustomerIDClassID] varchar (100) NULL,\r\n"
                                         + "  [OfficialID] varchar (255) NULL,\r\n\r\n"
                                         + "  CONSTRAINT [PK_SchemaGeneration_Order] PRIMARY KEY CLUSTERED ([ID])\r\n"
                                         + ")\r\n";

      Assert.AreEqual (expectedCreateTableScript, _tableBuilder.GetCreateTableScript());

      string expectedDropTableScript =
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SchemaGeneration_Customer' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP TABLE [dbo].[SchemaGeneration_Customer]\r\n\r\n"
          + "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'SchemaGeneration_Order' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP TABLE [dbo].[SchemaGeneration_Order]\r\n";

      Assert.AreEqual (expectedDropTableScript, _tableBuilder.GetDropTableScript());
    }

    private PropertyDefinition CreatePropertyDefinition (Type propertyType, bool? isNullable, int? maxLength)
    {
      return CreatePropertyDefinition (_classDefintion, "Name", "ColumnName", propertyType, isNullable, maxLength, StorageClass.Persistent);
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
      propertyDefinition.SetStorageProperty (new ColumnDefinition (columnName, propertyType, "dummyStorageType", isNullable.HasValue? isNullable.Value:true));
      return propertyDefinition;
    }
  }
}