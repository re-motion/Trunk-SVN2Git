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
using System.ComponentModel;
using System.Data;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.Model.Building
{
  [TestFixture]
  public class SqlStorageTypeCalculatorTest
  {
    private SqlStorageTypeCalculator _typeCalculator;
    private StorageProviderDefinitionFinder _storageProviderDefinitionFinder;

    // We explicitly want an _int_ enum
    // ReSharper disable EnumUnderlyingTypeIsInt
    private enum Int32Enum : int
    {
    }
    // ReSharper restore EnumUnderlyingTypeIsInt

    private enum Int16Enum : short
    {
    }

    [SetUp]
    public void SetUp ()
    {
      _storageProviderDefinitionFinder = new StorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage);
      _typeCalculator = new SqlStorageTypeCalculator (_storageProviderDefinitionFinder);
    }

    [Test]
    public void GetStorageType ()
    {
      CheckGetStorageType (typeof (Boolean), null, "bit", DbType.Boolean, typeof (bool), Is.TypeOf (typeof (BooleanConverter)));

      // TODO Review 4150: Refactor tests to use CheckGetStorageType

      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Byte))).StorageType, Is.EqualTo ("tinyint"));
      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Byte))).DbType, Is.EqualTo (DbType.Byte));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Byte))).ParameterValueType, Is.EqualTo (typeof (byte)));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Byte))).TypeConverter,
          Is.TypeOf (typeof (ByteConverter)));

      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (DateTime))).StorageType, Is.EqualTo ("datetime"));
      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (DateTime))).DbType, Is.EqualTo (DbType.DateTime));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (DateTime))).ParameterValueType,
          Is.EqualTo (typeof (DateTime)));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (DateTime))).TypeConverter,
          Is.TypeOf (typeof (DateTimeConverter)));

      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Decimal))).StorageType, Is.EqualTo ("decimal (38, 3)"));
      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Decimal))).DbType, Is.EqualTo (DbType.Decimal));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Decimal))).ParameterValueType, Is.EqualTo (typeof (Decimal)));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Decimal))).TypeConverter,
          Is.TypeOf (typeof (DecimalConverter)));

      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Double))).StorageType, Is.EqualTo ("float"));
      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Double))).DbType, Is.EqualTo (DbType.Double));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Double))).ParameterValueType, Is.EqualTo (typeof (Double)));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Double))).TypeConverter,
          Is.TypeOf (typeof (DoubleConverter)));

      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Guid))).StorageType, Is.EqualTo ("uniqueidentifier"));
      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Guid))).DbType, Is.EqualTo (DbType.Guid));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Guid))).ParameterValueType, Is.EqualTo (typeof (Guid)));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Guid))).TypeConverter,
          Is.TypeOf (typeof (GuidConverter)));

      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int16))).StorageType, Is.EqualTo ("smallint"));
      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int16))).DbType, Is.EqualTo (DbType.Int16));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int16))).ParameterValueType, Is.EqualTo (typeof (Int16)));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int16))).TypeConverter,
          Is.TypeOf (typeof (Int16Converter)));

      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int32))).StorageType, Is.EqualTo ("int"));
      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int32))).DbType, Is.EqualTo (DbType.Int32));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int32))).ParameterValueType, Is.EqualTo (typeof (int)));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int32))).TypeConverter,
          Is.TypeOf (typeof (Int32Converter)));

      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int64))).StorageType, Is.EqualTo ("bigint"));
      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int64))).DbType, Is.EqualTo (DbType.Int64));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int64))).ParameterValueType, Is.EqualTo (typeof (Int64)));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int64))).TypeConverter,
          Is.TypeOf (typeof (Int64Converter)));

      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Single))).StorageType, Is.EqualTo ("real"));
      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Single))).DbType, Is.EqualTo (DbType.Single));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Single))).ParameterValueType, Is.EqualTo (typeof (Single)));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Single))).TypeConverter,
          Is.TypeOf (typeof (SingleConverter)));

      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int32Enum))).StorageType, Is.EqualTo ("int"));
      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int32Enum))).DbType, Is.EqualTo (DbType.Int32));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int32Enum))).ParameterValueType, Is.EqualTo (typeof (Int32)));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int32Enum))).TypeConverter,
          Is.TypeOf (typeof (AdvancedEnumConverter))); // TODO Review 4150: Also check EnumType

      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int16Enum))).StorageType, Is.EqualTo ("smallint"));
      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int16Enum))).DbType, Is.EqualTo (DbType.Int16));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int16Enum))).ParameterValueType, Is.EqualTo (typeof (Int16)));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int16Enum))).TypeConverter,
          Is.TypeOf (typeof (AdvancedEnumConverter))); // TODO Review 4150: Also check EnumType

      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Color))).StorageType,
          Is.EqualTo ("varchar (" + Color.Values.Green ().ID.Length + ")"));
      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Color))).DbType, Is.EqualTo (DbType.String));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Color))).ParameterValueType, Is.EqualTo (typeof (string)));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Color))).TypeConverter,
          Is.TypeOf (typeof (StringConverter))); // TODO Review 4150: Should be ExtensibleEnumConverter, also check ExtensibleEnumType

      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (String), 200)).StorageType, Is.EqualTo ("nvarchar (200)"));
      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (String), 200)).DbType, Is.EqualTo (DbType.String));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (String))).ParameterValueType, Is.EqualTo (typeof (string)));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (String))).TypeConverter,
          Is.TypeOf (typeof (StringConverter)));

      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (String))).StorageType, Is.EqualTo ("nvarchar (max)"));
      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (String))).DbType, Is.EqualTo (DbType.String));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (String))).ParameterValueType, Is.EqualTo (typeof (string)));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (String))).TypeConverter,
          Is.TypeOf (typeof (StringConverter)));

      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Byte[]), 200)).StorageType, Is.EqualTo ("varbinary (200)"));
      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Byte[]), 200)).DbType, Is.EqualTo (DbType.Binary));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Byte[]))).ParameterValueType, Is.EqualTo (typeof (byte[])));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Byte[]))).TypeConverter,
          Is.TypeOf (typeof (ArrayConverter)));

      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Byte[]))).StorageType, Is.EqualTo ("varbinary (max)"));
      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Byte[]))).DbType, Is.EqualTo (DbType.Binary));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Byte[]))).ParameterValueType, Is.EqualTo (typeof (byte[])));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Byte[]))).TypeConverter,
          Is.TypeOf (typeof (ArrayConverter)));
    }

    [Test]
    public void GetStorageType_ForNullableValueTypes ()
    {
      // TODO Review 4150: Refactor tests to use CheckGetStorageType

      Assert.That (_typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (bool?))).StorageType, Is.EqualTo ("bit"));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (bool?))).DbType, Is.EqualTo (DbType.Boolean));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (bool?))).ParameterValueType,
          Is.EqualTo (typeof (bool?)));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (bool?))).TypeConverter,
          Is.TypeOf (typeof (NullableConverter)));

      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int32Enum?))).StorageType, Is.EqualTo ("int"));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int32Enum?))).DbType, Is.EqualTo (DbType.Int32));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int32Enum?))).ParameterValueType,
          Is.EqualTo (typeof (int?)));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int32Enum?))).TypeConverter,
          Is.TypeOf (typeof (AdvancedEnumConverter)));

      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int16Enum?))).StorageType, Is.EqualTo ("smallint"));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int16Enum?))).DbType, Is.EqualTo (DbType.Int16));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int16Enum?))).ParameterValueType,
          Is.EqualTo (typeof (Int16?)));
      Assert.That (
          _typeCalculator.GetStorageType (CreatePropertyDefinition (typeof (Int16Enum?))).TypeConverter,
          Is.TypeOf (typeof (AdvancedEnumConverter)));
    }

    [Test]
    public void GetStorageTypeForSpecialColumns ()
    {
      // TODO Review 4150: Refactor tests to use CheckStorageType

      Assert.That (_typeCalculator.ObjectIDStorageType.StorageType, Is.EqualTo ("uniqueidentifier"));
      Assert.That (_typeCalculator.ObjectIDStorageType.DbType, Is.EqualTo (DbType.Guid));
      Assert.That (_typeCalculator.ObjectIDStorageType.ParameterValueType, Is.EqualTo (typeof (Guid)));
      Assert.That (_typeCalculator.ObjectIDStorageType.TypeConverter, Is.TypeOf (typeof (GuidConverter)));

      Assert.That (_typeCalculator.SerializedObjectIDStorageType.StorageType, Is.EqualTo ("varchar (255)"));
      Assert.That (_typeCalculator.SerializedObjectIDStorageType.DbType, Is.EqualTo (DbType.String));
      Assert.That (_typeCalculator.SerializedObjectIDStorageType.ParameterValueType, Is.EqualTo (typeof (String)));
      Assert.That (_typeCalculator.SerializedObjectIDStorageType.TypeConverter, Is.TypeOf (typeof (StringConverter)));

      Assert.That (_typeCalculator.ClassIDStorageType.StorageType, Is.EqualTo ("varchar (100)"));
      Assert.That (_typeCalculator.ClassIDStorageType.DbType, Is.EqualTo (DbType.String));
      Assert.That (_typeCalculator.ClassIDStorageType.ParameterValueType, Is.EqualTo (typeof (String)));
      Assert.That (_typeCalculator.ClassIDStorageType.TypeConverter, Is.TypeOf (typeof (StringConverter)));

      Assert.That (_typeCalculator.TimestampStorageType.StorageType, Is.EqualTo ("rowversion"));
      Assert.That (_typeCalculator.TimestampStorageType.DbType, Is.EqualTo (DbType.Binary));
      Assert.That (_typeCalculator.TimestampStorageType.ParameterValueType, Is.EqualTo (typeof (Byte[])));
      Assert.That (_typeCalculator.TimestampStorageType.TypeConverter, Is.TypeOf (typeof (ArrayConverter)));
    }

    [Test]
    public void GettorageType_WithNotSupportedType ()
    {
      var propertyDefinition = CreatePropertyDefinition (typeof (Char));

      var result = _typeCalculator.GetStorageType (propertyDefinition);

      Assert.That (result.StorageType, Is.Null);
    }

    private void CheckGetStorageType (
        Type propertyType,
        int? maxLength,
        string expectedStorageTypeString,
        DbType expectedDbType,
        Type expectedParameterValueType,
        IResolveConstraint expectedTypeConverterConstraint)
    {
      var propertyDefinition = CreatePropertyDefinition (propertyType, maxLength);
      var info = _typeCalculator.GetStorageType (propertyDefinition);
      CheckStorageTypeInformation (info, expectedStorageTypeString, expectedDbType, expectedParameterValueType, expectedTypeConverterConstraint);
    }

    private PropertyDefinition CreatePropertyDefinition (Type propertyType, int? maxLength = null)
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (typeof (Order), null);
      return PropertyDefinitionFactory.CreateForFakePropertyInfo (
          classDefinition,
          "Name",
          "ColumnName",
          propertyType,
          false,
          maxLength,
          StorageClass.Persistent);
    }

    private void CheckStorageTypeInformation (
        StorageTypeInformation storageTypeInformation,
        string expectedStorageTypeString,
        DbType expectedDbType,
        Type expectedParameterValueType,
        IResolveConstraint typeConverterConstraint)
    {
      Assert.That (storageTypeInformation.StorageType, Is.EqualTo (expectedStorageTypeString));
      Assert.That (storageTypeInformation.DbType, Is.EqualTo (expectedDbType));
      Assert.That (storageTypeInformation.ParameterValueType, Is.EqualTo (expectedParameterValueType));
      Assert.That (storageTypeInformation.TypeConverter, typeConverterConstraint);
    }
  }
}