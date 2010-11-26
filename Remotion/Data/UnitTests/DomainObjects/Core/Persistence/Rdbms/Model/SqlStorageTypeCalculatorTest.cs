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
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class SqlStorageTypeCalculatorTest
  {
    private SqlStorageTypeCalculator _typeCalculator;
    private ReflectionBasedClassDefinition _classDefinition;
    private ClassDefinition _orderItemClass;
    private ClassDefinition _orderClass;
    private StorageProviderDefinitionFinder _storageProviderDefinitionFinder;

    private enum Int32Enum : int
    {
    }

    private enum Int16Enum : short
    {
    }

    [SetUp]
    public void SetUp ()
    {
      _storageProviderDefinitionFinder = new StorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage);
      _typeCalculator = new SqlStorageTypeCalculator ();
      _classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition(typeof(Order));
      _orderItemClass = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem));
      _orderClass = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order));
    }

    [Test]
    public void GetStorageType ()
    {
      Assert.AreEqual ("bit", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Boolean), null, null), _storageProviderDefinitionFinder));
      Assert.AreEqual ("tinyint", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Byte), null, null), _storageProviderDefinitionFinder));
      Assert.AreEqual ("datetime", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (DateTime), null, null), _storageProviderDefinitionFinder));
      Assert.AreEqual ("decimal (38, 3)", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Decimal), null, null), _storageProviderDefinitionFinder));
      Assert.AreEqual ("float", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Double), null, null), _storageProviderDefinitionFinder));
      Assert.AreEqual ("uniqueidentifier", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Guid), null, null), _storageProviderDefinitionFinder));
      Assert.AreEqual ("smallint", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int16), null, null), _storageProviderDefinitionFinder));
      Assert.AreEqual ("int", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int32), null, null), _storageProviderDefinitionFinder));
      Assert.AreEqual ("bigint", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int64), null, null), _storageProviderDefinitionFinder));
      Assert.AreEqual ("real", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Single), null, null), _storageProviderDefinitionFinder));

      Assert.AreEqual ("int", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int32Enum), null, null), _storageProviderDefinitionFinder));
      Assert.AreEqual ("smallint", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int16Enum), null, null), _storageProviderDefinitionFinder));

      Assert.AreEqual ("nvarchar (200)", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (String), false, 200), _storageProviderDefinitionFinder));
      Assert.AreEqual ("nvarchar (max)", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (String), false, null), _storageProviderDefinitionFinder));

      Assert.AreEqual ("varbinary (200)", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Byte[]), false, 200), _storageProviderDefinitionFinder));
      Assert.AreEqual ("varbinary (max)", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Byte[]), false, null), _storageProviderDefinitionFinder));
    }

    [Test]
    public void GetStorageType_ForNullableValueTypes ()
    {
      Assert.AreEqual ("bit", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Boolean>), null, null), _storageProviderDefinitionFinder));
      Assert.AreEqual ("tinyint", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Byte>), null, null), _storageProviderDefinitionFinder));
      Assert.AreEqual ("datetime", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<DateTime>), null, null), _storageProviderDefinitionFinder));
      Assert.AreEqual ("decimal (38, 3)", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Decimal>), null, null), _storageProviderDefinitionFinder));
      Assert.AreEqual ("float", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Double>), null, null), _storageProviderDefinitionFinder));
      Assert.AreEqual ("uniqueidentifier", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Guid>), null, null), _storageProviderDefinitionFinder));
      Assert.AreEqual ("smallint", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Int16>), null, null), _storageProviderDefinitionFinder));
      Assert.AreEqual ("int", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Int32>), null, null), _storageProviderDefinitionFinder));
      Assert.AreEqual ("bigint", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Int64>), null, null), _storageProviderDefinitionFinder));
      Assert.AreEqual ("real", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Single>), null, null), _storageProviderDefinitionFinder));

      Assert.AreEqual ("int", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Int32Enum>), null, null), _storageProviderDefinitionFinder));
      Assert.AreEqual ("smallint", _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Nullable<Int16Enum>), null, null), _storageProviderDefinitionFinder));
    }

    [Test]
    public void GetStorageTypeForSpecialCulumns ()
    {
      Assert.AreEqual (
          "uniqueidentifier",
          _typeCalculator.GetStorageType (
              _orderItemClass.GetMandatoryPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order"), _storageProviderDefinitionFinder));
      Assert.AreEqual (
          "varchar (255)",
          _typeCalculator.GetStorageType (
          _orderClass.GetMandatoryPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Official"), _storageProviderDefinitionFinder));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Data type 'System.Char' is not supported.\r\nDeclaring type: 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order'\r\nProperty: 'Name'")]
    public void GettorageType_WithNotSupportedType ()
    {
      _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Char), null, null), _storageProviderDefinitionFinder);
    }

    private PropertyDefinition CreatePropertyDefinition (Type propertyType, bool? isNullable, int? maxLength)
    {
      return ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (
          _classDefinition, "Name", "ColumnName", propertyType, isNullable, maxLength, StorageClass.Persistent);
    }
  }
}