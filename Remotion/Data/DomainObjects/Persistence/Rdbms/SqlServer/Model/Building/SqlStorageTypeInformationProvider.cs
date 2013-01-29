// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.ExtensibleEnums;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building
{
  /// <summary>
  /// <see cref="SqlStorageTypeInformationProvider"/> calculates the SQL Server-specific type for a column in a relational database.
  /// </summary>
  public class SqlStorageTypeInformationProvider : IStorageTypeInformationProvider
  {
    public virtual IStorageTypeInformation GetStorageTypeForID (bool isStorageTypeNullable)
    {
      return new StorageTypeInformation (
          typeof (Guid?),
          "uniqueidentifier",
          DbType.Guid,
          isStorageTypeNullable,
          typeof (Guid?),
          new DefaultConverter (typeof (Guid?)));
    }

    public virtual IStorageTypeInformation GetStorageTypeForSerializedObjectID (bool isStorageTypeNullable)
    {
      return new StorageTypeInformation (typeof (string), 
          "varchar (255)", 
          DbType.String, 
          isStorageTypeNullable, 
          typeof (string), 
          new DefaultConverter (typeof (string)));
    }

    public virtual IStorageTypeInformation GetStorageTypeForClassID (bool isStorageTypeNullable)
    {
      return new StorageTypeInformation (
          typeof (string),
          "varchar (100)",
          DbType.String,
          isStorageTypeNullable,
          typeof (string),
          new DefaultConverter (typeof (string)));
    }

    public virtual IStorageTypeInformation GetStorageTypeForTimestamp (bool isStorageTypeNullable)
    {
      return new StorageTypeInformation (
          typeof (byte[]),
          "rowversion",
          DbType.Binary,
          isStorageTypeNullable,
          typeof (byte[]),
          new DefaultConverter (typeof (byte[])));
    }

    /// <inheritdoc/>
    /// <remarks>If overridden in a derived class, <see cref="GetStorageType (Type)"/> must also be overridden based on the same semantics.</remarks>
    public virtual IStorageTypeInformation GetStorageType (PropertyDefinition propertyDefinition, bool forceNullable)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      var dotNetType = propertyDefinition.PropertyType;
      var isNullableInDatabase = propertyDefinition.IsNullable || forceNullable;
      var storageType = GetStorageType (dotNetType, propertyDefinition.MaxLength, isNullableInDatabase);
      if (storageType == null)
        throw new NotSupportedException (string.Format ("Type '{0}' is not supported by this storage provider.", dotNetType));
      return storageType;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// If overridden in a derived class, <see cref="GetStorageType (PropertyDefinition, bool)"/> must also be overridden based on the same semantics.
    /// </remarks>
    public virtual IStorageTypeInformation GetStorageType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var storageType = GetStorageType (type, null, IsNullSupported (type));
      if(storageType == null)
        throw new NotSupportedException (string.Format ("Type '{0}' is not supported by this storage provider.", type));
      return storageType;
    }

    public IStorageTypeInformation GetStorageType (object value)
    {
      if (value == null)
      {
        // NULL values of storage type nvarchar(max) seem to be compatible with columns of all other supported storage types, tested by
        // SqlProviderExecuteCollectionQueryTest.AllDataTypes
        return new StorageTypeInformation (typeof (object), "nvarchar (max)", DbType.String, true, typeof (object), NullValueConverter.Instance);
      }

      return GetStorageType (value.GetType());
    }

    private StorageTypeInformation GetStorageType (Type dotNetType, int? maxLength, bool isNullableInDatabase)
    {
      var underlyingTypeOfNullable = Nullable.GetUnderlyingType (dotNetType);
      if (underlyingTypeOfNullable != null)
        return GetStorageTypeForNullableValueType (dotNetType, underlyingTypeOfNullable, maxLength, isNullableInDatabase);

      if (dotNetType.IsEnum)
        return GetStorageTypeForEnumType (dotNetType, maxLength, isNullableInDatabase);

      if (ExtensibleEnumUtility.IsExtensibleEnumType (dotNetType))
        return GetStorageTypeForExtensibleEnumType (dotNetType, isNullableInDatabase);

      if (ReflectionUtility.IsStringPropertyValueType (dotNetType))
      {
        string storageTypeName = GetStorageTypeStringForVarType ("nvarchar", maxLength);
        return new StorageTypeInformation (typeof (string), storageTypeName, DbType.String, isNullableInDatabase, dotNetType, new DefaultConverter (dotNetType));
      }

      if (ReflectionUtility.IsBinaryPropertyValueType (dotNetType))
      {
        string storageTypeName = GetStorageTypeStringForVarType ("varbinary", maxLength);
        return new StorageTypeInformation (typeof (byte[]), storageTypeName, DbType.Binary, isNullableInDatabase, dotNetType, new DefaultConverter (dotNetType));
      }

      if (dotNetType == typeof (Boolean))
        return new StorageTypeInformation (typeof (Boolean), "bit", DbType.Boolean, isNullableInDatabase, dotNetType, new DefaultConverter (dotNetType));
      if (dotNetType == typeof (Byte))
        return new StorageTypeInformation (typeof (Byte), "tinyint", DbType.Byte, isNullableInDatabase, dotNetType, new DefaultConverter (dotNetType));
      if (dotNetType == typeof (DateTime))
        return new StorageTypeInformation (typeof (DateTime), "datetime", DbType.DateTime, isNullableInDatabase, dotNetType, new DefaultConverter (dotNetType));
      if (dotNetType == typeof (Decimal))
        return new StorageTypeInformation (typeof (Decimal), "decimal (38, 3)", DbType.Decimal, isNullableInDatabase, dotNetType, new DefaultConverter (dotNetType));
      if (dotNetType == typeof (Double))
        return new StorageTypeInformation (typeof (Double), "float", DbType.Double, isNullableInDatabase, dotNetType, new DefaultConverter (dotNetType));
      if (dotNetType == typeof (Guid))
        return new StorageTypeInformation (typeof (Guid), "uniqueidentifier", DbType.Guid, isNullableInDatabase, dotNetType, new DefaultConverter (dotNetType));
      if (dotNetType == typeof (Int16))
        return new StorageTypeInformation (typeof (Int16), "smallint", DbType.Int16, isNullableInDatabase, dotNetType, new DefaultConverter (dotNetType));
      if (dotNetType == typeof (Int32))
        return new StorageTypeInformation (typeof (Int32), "int", DbType.Int32, isNullableInDatabase, dotNetType, new DefaultConverter (dotNetType));
      if (dotNetType == typeof (Int64))
        return new StorageTypeInformation (typeof (Int64), "bigint", DbType.Int64, isNullableInDatabase, dotNetType, new DefaultConverter (dotNetType));
      if (dotNetType == typeof (Single))
        return new StorageTypeInformation (typeof (Single), "real", DbType.Single, isNullableInDatabase, dotNetType, new DefaultConverter (dotNetType));

      return null;
    }

    private StorageTypeInformation GetStorageTypeForExtensibleEnumType (Type extensibleEnumType, bool isNullableInDatabase)
    {
      var storageType = GetStorageTypeStringForVarType ("varchar", GetColumnWidthForExtensibleEnum (extensibleEnumType));
      return new StorageTypeInformation (typeof (string), storageType, DbType.String, isNullableInDatabase, extensibleEnumType, new ExtensibleEnumConverter (extensibleEnumType));
    }

    private StorageTypeInformation GetStorageTypeForNullableValueType (Type nullableValueType, Type underlyingType, int? maxLength, bool isNullableInDatabase)
    {
      var underlyingStorageInformation = GetStorageType (underlyingType, maxLength, false);
      if (underlyingType.IsEnum)
      {
        return new StorageTypeInformation (
            typeof (Nullable<>).MakeGenericType (underlyingStorageInformation.StorageType),
            underlyingStorageInformation.StorageTypeName,
            underlyingStorageInformation.StorageDbType,
            isNullableInDatabase,
            nullableValueType,
            new AdvancedEnumConverter (nullableValueType));
      }

      return new StorageTypeInformation (
          typeof (Nullable<>).MakeGenericType (underlyingStorageInformation.StorageType),
          underlyingStorageInformation.StorageTypeName,
          underlyingStorageInformation.StorageDbType,
          isNullableInDatabase,
          nullableValueType,
          new DefaultConverter (nullableValueType));
    }

    private StorageTypeInformation GetStorageTypeForEnumType (Type enumType, int? maxLength, bool isNullableInDatabase)
    {
      var underlyingStorageType = GetStorageType (Enum.GetUnderlyingType (enumType), maxLength, isNullableInDatabase);
      return new StorageTypeInformation (
          underlyingStorageType.StorageType,
          underlyingStorageType.StorageTypeName,
          underlyingStorageType.StorageDbType,
          underlyingStorageType.IsStorageTypeNullable,
          enumType,
          new AdvancedEnumConverter (enumType));
    }

    private string GetStorageTypeStringForVarType (string varType, int? maxLength)
    {
      return string.Format ("{0} ({1})", varType, maxLength.HasValue ? maxLength.ToString() : "max");
    }

    private int GetColumnWidthForExtensibleEnum (Type extensibleEnumType)
    {
      return ExtensibleEnumUtility.GetDefinition (extensibleEnumType).GetValueInfos().Max (info => info.Value.ID.Length);
    }

    private bool IsNullSupported (Type dotNetType)
    {
      return NullableTypeUtility.IsNullableType (dotNetType);
    }
  }
}