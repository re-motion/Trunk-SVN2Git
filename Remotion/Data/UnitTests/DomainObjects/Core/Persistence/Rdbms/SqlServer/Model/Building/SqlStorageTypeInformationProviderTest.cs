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
      _storageTypeInformationProvider = new SqlStorageTypeInformationProvider();
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
          typeof (bool),
          "bit", DbType.Boolean, 
          typeof (bool), 
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (bool)));
      CheckGetStorageType (
          typeof (Byte),
          null,
          typeof (Byte),
          "tinyint",
          DbType.Byte,
          typeof (byte),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (byte)));
      CheckGetStorageType (
          typeof (DateTime),
          null,
          typeof (DateTime),
          "datetime",
          DbType.DateTime,
          typeof (DateTime),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (DateTime)));
      CheckGetStorageType (
          typeof (Decimal),
          null,
          typeof (Decimal),
          "decimal (38, 3)",
          DbType.Decimal,
          typeof (decimal),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (Decimal)));
      CheckGetStorageType (
          typeof (Double),
          null,
          typeof (Double),
          "float",
          DbType.Double,
          typeof (double),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (Double)));
      CheckGetStorageType (
          typeof (Guid),
          null,
          typeof (Guid),
          "uniqueidentifier",
          DbType.Guid,
          typeof (Guid),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (Guid)));
      CheckGetStorageType (
          typeof (Int16),
          null,
          typeof (Int16),
          "smallint",
          DbType.Int16,
          typeof (short),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (Int16)));
      CheckGetStorageType (
          typeof (Int32),
          null,
          typeof (Int32),
          "int",
          DbType.Int32,
          typeof (int),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (Int32)));
      CheckGetStorageType (
          typeof (Int64),
          null,
          typeof (Int64),
          "bigint",
          DbType.Int64,
          typeof (long),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (Int64)));
      CheckGetStorageType (
          typeof (Single),
          null,
          typeof (Single),
          "real",
          DbType.Single,
          typeof (float),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (Single)));
      CheckGetStorageType (
          typeof (Int32Enum),
          null,
          typeof (Int32),
          "int",
          DbType.Int32,
          typeof (Int32Enum),
          Is.TypeOf (typeof (AdvancedEnumConverter)).With.Property ("EnumType").EqualTo (typeof (Int32Enum)));
      CheckGetStorageType (
          typeof (Int16Enum),
          null,
          typeof (Int16),
          "smallint",
          DbType.Int16,
          typeof (Int16Enum),
          Is.TypeOf (typeof (AdvancedEnumConverter)).With.Property ("EnumType").EqualTo (typeof (Int16Enum)));
      CheckGetStorageType (
          typeof (Color),
          null,
          typeof (string),
          "varchar (" + Color.Values.Green().ID.Length + ")",
          DbType.String,
          typeof (Color),
          Is.TypeOf (typeof (ExtensibleEnumConverter)).With.Property ("ExtensibleEnumType").EqualTo (typeof (Color)));
      CheckGetStorageType (
          typeof (String),
          200,
          typeof (string),
          "nvarchar (200)",
          DbType.String,
          typeof (string),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (string)));
      CheckGetStorageType (
          typeof (String),
          null,
          typeof (string),
          "nvarchar (max)",
          DbType.String,
          typeof (string),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (string)));
      CheckGetStorageType (
          typeof (Byte[]),
          200,
          typeof (Byte[]),
          "varbinary (200)",
          DbType.Binary,
          typeof (byte[]),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (byte[])));
      CheckGetStorageType (
          typeof (Byte[]),
          null,
          typeof (Byte[]),
          "varbinary (max)",
          DbType.Binary,
          typeof (byte[]),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (byte[])));
    }

    [Test]
    public void GetStorageType_PropertyDefinition_ForNullableValueTypes ()
    {
      CheckGetStorageType (
          typeof (bool?),
          null,
          typeof (bool?),
          "bit",
          DbType.Boolean,
          typeof (bool?),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").SameAs (typeof (bool?)));
      CheckGetStorageType (
          typeof (Int32Enum?),
          null,
          typeof (int?),
          "int",
          DbType.Int32,
          typeof (Int32Enum?),
          Is.TypeOf (typeof (AdvancedEnumConverter)));
      CheckGetStorageType (
          typeof (Int16Enum?),
          null,
          typeof (Int16?),
          "smallint",
          DbType.Int16,
          typeof (Int16Enum?),
          Is.TypeOf (typeof (AdvancedEnumConverter)));
    }

    [Test]
    public void GetStorageTypeForSpecialColumns ()
    {
      var storageTypeForObjectID = (StorageTypeInformation) _storageTypeInformationProvider.GetStorageTypeForID();
      Assert.That (storageTypeForObjectID.StorageTypeName, Is.EqualTo ("uniqueidentifier"));
      Assert.That (storageTypeForObjectID.StorageDbType, Is.EqualTo (DbType.Guid));
      Assert.That (storageTypeForObjectID.StorageType, Is.EqualTo (typeof (Guid?)));
      Assert.That (storageTypeForObjectID.DotNetTypeConverter, Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").SameAs (typeof (Guid?)));

      var storageTypeForSerializedObjectID = (StorageTypeInformation) _storageTypeInformationProvider.GetStorageTypeForSerializedObjectID();
      Assert.That (storageTypeForSerializedObjectID.StorageTypeName, Is.EqualTo ("varchar (255)"));
      Assert.That (storageTypeForSerializedObjectID.StorageDbType, Is.EqualTo (DbType.String));
      Assert.That (storageTypeForSerializedObjectID.StorageType, Is.EqualTo (typeof (String)));
      Assert.That (
          storageTypeForSerializedObjectID.DotNetTypeConverter, 
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (string)));

      var storageTypeForClassID = (StorageTypeInformation) _storageTypeInformationProvider.GetStorageTypeForClassID();
      Assert.That (storageTypeForClassID.StorageTypeName, Is.EqualTo ("varchar (100)"));
      Assert.That (storageTypeForClassID.StorageDbType, Is.EqualTo (DbType.String));
      Assert.That (storageTypeForClassID.StorageType, Is.EqualTo (typeof (String)));
      Assert.That (storageTypeForClassID.DotNetTypeConverter, Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (string)));

      var storageTypeForTimestamp = (StorageTypeInformation) _storageTypeInformationProvider.GetStorageTypeForTimestamp();
      Assert.That (storageTypeForTimestamp.StorageTypeName, Is.EqualTo ("rowversion"));
      Assert.That (storageTypeForTimestamp.StorageDbType, Is.EqualTo (DbType.Binary));
      Assert.That (storageTypeForTimestamp.StorageType, Is.EqualTo (typeof (Byte[])));
      Assert.That (storageTypeForTimestamp.DotNetTypeConverter, Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (byte[])));
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
      var result = (StorageTypeInformation) _storageTypeInformationProvider.GetStorageType (typeof (string));

      Assert.That (result, Is.Not.Null);
      Assert.That (result.StorageTypeName, Is.EqualTo ("nvarchar (max)"));
      Assert.That (result.StorageDbType, Is.EqualTo (DbType.String));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Type 'System.Char' is not supported by this storage provider.")]
    public void GetStorageType_Type_UnsupportedType ()
    {
      _storageTypeInformationProvider.GetStorageType (typeof (Char));
    }

    [Test]
    public void GetStorageTypeInformation_ValueNotNull ()
    {
      var result = (StorageTypeInformation) _storageTypeInformationProvider.GetStorageType (14);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.StorageTypeName, Is.EqualTo ("int"));
      Assert.That (result.StorageDbType, Is.EqualTo (DbType.Int32));
    }

    [Test]
    public void GetStorageTypeInformation_ValueNull ()
    {
      var result = (StorageTypeInformation) _storageTypeInformationProvider.GetStorageType ((object)null);

      CheckStorageTypeInformation (
          result,
          typeof (object),
          "nvarchar(max)",
          DbType.String,
          typeof (object),
          Is.TypeOf (typeof (DefaultConverter)).With.Property ("Type").EqualTo (typeof (object)));
    }

    private void CheckGetStorageType (
        Type propertyType,
        int? maxLength,
        Type expectedStorageType,
        string expectedStorageTypeName,
        DbType expectedStorageDbType,
        Type expectedDotNetType,
        IResolveConstraint expectedDotNetTypeConverterConstraint)
    {
      var propertyDefinition = CreatePropertyDefinition (propertyType, maxLength);
      var info = (StorageTypeInformation) _storageTypeInformationProvider.GetStorageType (propertyDefinition);
      CheckStorageTypeInformation (
          info,
          expectedStorageType,
          expectedStorageTypeName,
          expectedStorageDbType,
          expectedDotNetType,
          expectedDotNetTypeConverterConstraint);
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
        Type expectedStorageType,
        string expectedStorageTypeName,
        DbType expectedStorageDbType,
        Type expectedDotNetType,
        IResolveConstraint dotNetTypeConverterConstraint)
    {
      Assert.That (storageTypeInformation.StorageType, Is.SameAs (expectedStorageType));
      Assert.That (storageTypeInformation.StorageTypeName, Is.EqualTo (expectedStorageTypeName));
      Assert.That (storageTypeInformation.StorageDbType, Is.EqualTo (expectedStorageDbType));
      Assert.That (storageTypeInformation.DotNetType, Is.SameAs (expectedDotNetType));
      Assert.That (storageTypeInformation.DotNetTypeConverter, dotNetTypeConverterConstraint);
    }
  }
}