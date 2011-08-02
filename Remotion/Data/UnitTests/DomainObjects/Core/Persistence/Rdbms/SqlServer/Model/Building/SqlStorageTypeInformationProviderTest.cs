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
using System.Data;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.ExtensibleEnums;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.Model.Building
{
  [TestFixture]
  public class SqlStorageTypeInformationProviderTest
  {
    private SqlStorageTypeInformationProvider _storageTypeInformationProvider;

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
      _storageTypeInformationProvider = new SqlStorageTypeInformationProvider ();
    }

    [Test]
    public void IsTypeSupported_SupportedType ()
    {
      var result = _storageTypeInformationProvider.IsTypeSupported (typeof (string));

      Assert.That (result, Is.True);
    }

    [Test]
    public void IsTypeSupported_UnsupportedType ()
    {
      var result = _storageTypeInformationProvider.IsTypeSupported (typeof (Char));

      Assert.That (result, Is.False);
    }

    [Test]
    public void GetStorageType ()
    {
      CheckGetStorageType (
          typeof (Boolean),
          null,
          "bit",
          DbType.Boolean,
          typeof (bool),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (bool)));
      CheckGetStorageType (
          typeof (Byte),
          null,
          "tinyint",
          DbType.Byte,
          typeof (Byte),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (byte)));
      CheckGetStorageType (
          typeof (DateTime),
          null,
          "datetime",
          DbType.DateTime,
          typeof (DateTime),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (DateTime)));
      CheckGetStorageType (
          typeof (Decimal),
          null,
          "decimal (38, 3)",
          DbType.Decimal,
          typeof (Decimal),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (Decimal)));
      CheckGetStorageType (
          typeof (Double),
          null,
          "float",
          DbType.Double,
          typeof (Double),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (Double)));
      CheckGetStorageType (
          typeof (Guid),
          null,
          "uniqueidentifier",
          DbType.Guid,
          typeof (Guid),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (Guid)));
      CheckGetStorageType (
          typeof (Int16),
          null,
          "smallint",
          DbType.Int16,
          typeof (Int16),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (Int16)));
      CheckGetStorageType (
          typeof (Int32),
          null,
          "int",
          DbType.Int32,
          typeof (Int32),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (Int32)));
      CheckGetStorageType (
          typeof (Int64),
          null,
          "bigint",
          DbType.Int64,
          typeof (Int64),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (Int64)));
      CheckGetStorageType (
          typeof (Single),
          null,
          "real",
          DbType.Single,
          typeof (Single),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (Single)));
      CheckGetStorageType (
          typeof (Int32Enum),
          null,
          "int",
          DbType.Int32,
          typeof (Int32),
          Is.TypeOf (typeof (AdvancedEnumConverter)).With.Property ("EnumType").EqualTo (typeof (Int32Enum)));
      CheckGetStorageType (
          typeof (Int16Enum),
          null,
          "smallint",
          DbType.Int16,
          typeof (Int16),
          Is.TypeOf (typeof (AdvancedEnumConverter)).With.Property ("EnumType").EqualTo (typeof (Int16Enum)));
      CheckGetStorageType (
          typeof (Color),
          null,
          "varchar (" + Color.Values.Green().ID.Length + ")",
          DbType.String,
          typeof (string),
          Is.TypeOf (typeof (ExtensibleEnumConverter)).With.Property ("ExtensibleEnumType").EqualTo (typeof (Color)));
      CheckGetStorageType (
          typeof (String),
          200,
          "nvarchar (200)",
          DbType.String,
          typeof (string),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (string)));
      CheckGetStorageType (
          typeof (String),
          null,
          "nvarchar (max)",
          DbType.String,
          typeof (string),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (string)));
      CheckGetStorageType (
          typeof (Byte[]),
          200,
          "varbinary (200)",
          DbType.Binary,
          typeof (Byte[]),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (byte[])));
      CheckGetStorageType (
          typeof (Byte[]),
          null,
          "varbinary (max)",
          DbType.Binary,
          typeof (Byte[]),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (byte[])));
    }

    [Test]
    public void GetStorageType_PropertyDefinition_ForNullableValueTypes ()
    {
      CheckGetStorageType (
          typeof (bool?),
          null,
          "bit",
          DbType.Boolean,
          typeof (bool?),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").SameAs (typeof (bool?)));
      CheckGetStorageType (typeof (Int32Enum?), null, "int", DbType.Int32, typeof (int?), Is.TypeOf (typeof (AdvancedEnumConverter)));
      CheckGetStorageType (typeof (Int16Enum?), null, "smallint", DbType.Int16, typeof (Int16?), Is.TypeOf (typeof (AdvancedEnumConverter)));
    }

    [Test]
    public void GetStorageTypeForSpecialColumns ()
    {
      Assert.That (_storageTypeInformationProvider.ObjectIDStorageType.StorageType, Is.EqualTo ("uniqueidentifier"));
      Assert.That (_storageTypeInformationProvider.ObjectIDStorageType.DbType, Is.EqualTo (DbType.Guid));
      Assert.That (_storageTypeInformationProvider.ObjectIDStorageType.ParameterValueType, Is.EqualTo (typeof (Guid?)));
      Assert.That (_storageTypeInformationProvider.ObjectIDStorageType.TypeConverter, Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").SameAs (typeof (Guid?)));

      Assert.That (_storageTypeInformationProvider.SerializedObjectIDStorageType.StorageType, Is.EqualTo ("varchar (255)"));
      Assert.That (_storageTypeInformationProvider.SerializedObjectIDStorageType.DbType, Is.EqualTo (DbType.String));
      Assert.That (_storageTypeInformationProvider.SerializedObjectIDStorageType.ParameterValueType, Is.EqualTo (typeof (String)));
      Assert.That (
          _storageTypeInformationProvider.SerializedObjectIDStorageType.TypeConverter,
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (string)));

      Assert.That (_storageTypeInformationProvider.ClassIDStorageType.StorageType, Is.EqualTo ("varchar (100)"));
      Assert.That (_storageTypeInformationProvider.ClassIDStorageType.DbType, Is.EqualTo (DbType.String));
      Assert.That (_storageTypeInformationProvider.ClassIDStorageType.ParameterValueType, Is.EqualTo (typeof (String)));
      Assert.That (
          _storageTypeInformationProvider.ClassIDStorageType.TypeConverter, Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (string)));

      Assert.That (_storageTypeInformationProvider.TimestampStorageType.StorageType, Is.EqualTo ("rowversion"));
      Assert.That (_storageTypeInformationProvider.TimestampStorageType.DbType, Is.EqualTo (DbType.Binary));
      Assert.That (_storageTypeInformationProvider.TimestampStorageType.ParameterValueType, Is.EqualTo (typeof (Byte[])));
      Assert.That (
          _storageTypeInformationProvider.TimestampStorageType.TypeConverter, Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (byte[])));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Type 'System.Char' is not supported by this storage provider.")]
    public void GetStorageType_PropertyDefinition_WithNotSupportedType ()
    {
      var propertyDefinition = CreatePropertyDefinition (typeof (Char));

      _storageTypeInformationProvider.GetStorageType (propertyDefinition);
    }

    [Test]
    public void GetStorageType_Type_SupportedType ()
    {
      var result = _storageTypeInformationProvider.GetStorageType (typeof (string));

      Assert.That (result, Is.Not.Null);
      Assert.That (result.StorageType, Is.EqualTo ("nvarchar (max)"));
      Assert.That (result.DbType, Is.EqualTo (DbType.String));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Type 'System.Char' is not supported by this storage provider.")]
    public void GetStorageType_Type_UnsupportedType ()
    {
      _storageTypeInformationProvider.GetStorageType (typeof (Char));
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
      var info = _storageTypeInformationProvider.GetStorageType (propertyDefinition);
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