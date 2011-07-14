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
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.ExtensibleEnums;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building
{
  /// <summary>
  /// <see cref="SqlStorageTypeCalculator"/> calculates the SQL Server-specific type for a column in a relational database.
  /// </summary>
  public class SqlStorageTypeCalculator : StorageTypeCalculator
  {
    public SqlStorageTypeCalculator (IStorageProviderDefinitionFinder storageProviderDefinitionFinder)
        : base (storageProviderDefinitionFinder)
    {
    }

    public override StorageTypeInformation ObjectIDStorageType
    {
      get { return new StorageTypeInformation ("uniqueidentifier", DbType.Guid, typeof (Guid), new GuidConverter()); }
    }

    public override StorageTypeInformation SerializedObjectIDStorageType
    {
      get { return new StorageTypeInformation ("varchar (255)", DbType.String, typeof (string), new StringConverter()); }
    }

    public override StorageTypeInformation ClassIDStorageType
    {
      get { return new StorageTypeInformation ("varchar (100)", DbType.String, typeof (string), new StringConverter()); }
    }

    public override StorageTypeInformation TimestampStorageType
    {
      get { return new StorageTypeInformation ("rowversion", DbType.Binary, typeof (byte[]), new ArrayConverter()); }
    }

    public override StorageTypeInformation GetStorageType (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      var sqlDataType = GetSqlDataType (propertyDefinition.PropertyType);
      if (!string.IsNullOrEmpty (sqlDataType.StorageType))
        return sqlDataType;

      if (ReflectionUtility.IsStringPropertyValueType (propertyDefinition.PropertyType))
      {
        return
            new StorageTypeInformation (
                string.Format ("nvarchar ({0})", propertyDefinition.MaxLength.HasValue ? propertyDefinition.MaxLength.ToString() : "max"),
                DbType.String,
                typeof (string),
                new StringConverter());
      }

      if (ReflectionUtility.IsBinaryPropertyValueType (propertyDefinition.PropertyType))
      {
        return
            new StorageTypeInformation (
                string.Format ("varbinary ({0})", propertyDefinition.MaxLength.HasValue ? propertyDefinition.MaxLength.ToString() : "max"),
                DbType.Binary,
                typeof (byte[]),
                new ArrayConverter());
      }

      return new StorageTypeInformation();
    }

    private static StorageTypeInformation GetSqlDataType (Type type)
    {
      var underlyingType = Nullable.GetUnderlyingType (type);
      if (underlyingType != null)
        return GetSqlDataTypeForNullableValueType (type, underlyingType);

      if (type == typeof (Boolean))
        return new StorageTypeInformation ("bit", DbType.Boolean, typeof (bool), new BooleanConverter());
      if (type == typeof (Byte))
        return new StorageTypeInformation ("tinyint", DbType.Byte, typeof (byte), new ByteConverter());
      if (type == typeof (DateTime))
        return new StorageTypeInformation ("datetime", DbType.DateTime, typeof (DateTime), new DateTimeConverter());
      if (type == typeof (Decimal))
        return new StorageTypeInformation ("decimal (38, 3)", DbType.Decimal, typeof (Decimal), new DecimalConverter());
      if (type == typeof (Double))
        return new StorageTypeInformation ("float", DbType.Double, typeof (Double), new DoubleConverter());
      if (type == typeof (Guid))
        return new StorageTypeInformation ("uniqueidentifier", DbType.Guid, typeof (Guid), new GuidConverter());
      if (type == typeof (Int16))
        return new StorageTypeInformation ("smallint", DbType.Int16, typeof (Int16), new Int16Converter());
      if (type == typeof (Int32))
        return new StorageTypeInformation ("int", DbType.Int32, typeof (Int32), new Int32Converter());
      if (type == typeof (Int64))
        return new StorageTypeInformation ("bigint", DbType.Int64, typeof (Int64), new Int64Converter());
      if (type == typeof (Single))
        return new StorageTypeInformation ("real", DbType.Single, typeof (Single), new SingleConverter());

      if (type.IsEnum)
      {
        var underlyingStorageInformation = GetSqlDataType (Enum.GetUnderlyingType (type));
        return new StorageTypeInformation (
            underlyingStorageInformation.StorageType,
            underlyingStorageInformation.DbType,
            underlyingStorageInformation.ParameterValueType,
            new AdvancedEnumConverter (type));
      }

      if (ExtensibleEnumUtility.IsExtensibleEnumType (type))
      {
        return new StorageTypeInformation (
            string.Format ("varchar ({0})", GetColumnWidthForExtensibleEnum (type)), DbType.String, typeof (string), new StringConverter());
      }

      return new StorageTypeInformation(); // TODO: Consider throwing exception here
    }

    private static StorageTypeInformation GetSqlDataTypeForNullableValueType (Type type, Type underlyingType)
    {
      var underlyingStorageInformation = GetSqlDataType (underlyingType);
      if (underlyingType.IsEnum)
      {
        return new StorageTypeInformation (
            underlyingStorageInformation.StorageType,
            underlyingStorageInformation.DbType,
            typeof (Nullable<>).MakeGenericType (underlyingStorageInformation.ParameterValueType),
            new AdvancedEnumConverter (type));
      }

      return new StorageTypeInformation (
          underlyingStorageInformation.StorageType,
          underlyingStorageInformation.DbType,
          typeof (Nullable<>).MakeGenericType (underlyingStorageInformation.ParameterValueType),
          new NullableConverter (type));
    }

    private static int GetColumnWidthForExtensibleEnum (Type extensibleEnumType)
    {
      return ExtensibleEnumUtility.GetDefinition (extensibleEnumType).GetValueInfos().Max (info => info.Value.ID.Length);
    }
  }
}