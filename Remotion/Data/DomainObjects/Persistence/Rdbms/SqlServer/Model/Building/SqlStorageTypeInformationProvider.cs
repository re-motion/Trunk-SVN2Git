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
    public virtual IStorageTypeInformation GetStorageTypeForID ()
    {
      return new StorageTypeInformation (typeof (Guid?), "uniqueidentifier", DbType.Guid, typeof (Guid?), new DefaultConverter (typeof (Guid?)));
    }

    public virtual IStorageTypeInformation GetStorageTypeForSerializedObjectID ()
    {
      return new StorageTypeInformation (typeof (string), "varchar (255)", DbType.String, typeof (string), new DefaultConverter (typeof (string)));
    }

    public virtual IStorageTypeInformation GetStorageTypeForClassID ()
    {
      return new StorageTypeInformation (typeof (string), "varchar (100)", DbType.String, typeof (string), new DefaultConverter (typeof (string)));
    }

    public virtual IStorageTypeInformation GetStorageTypeForTimestamp ()
    {
      return new StorageTypeInformation (typeof (byte[]), "rowversion", DbType.Binary, typeof (byte[]), new DefaultConverter (typeof (byte[])));
    }

    public virtual bool IsTypeSupported(Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return GetStorageType (type, null) != null;
    }

    public virtual IStorageTypeInformation GetStorageType (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      var storageType = GetStorageType (propertyDefinition.PropertyType, propertyDefinition.MaxLength);
      if (storageType == null)
        throw new NotSupportedException (string.Format ("Type '{0}' is not supported by this storage provider.", propertyDefinition.PropertyType));
      return storageType;
    }

    public virtual IStorageTypeInformation GetStorageType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var storageType= GetStorageType (type, null);
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
        return new StorageTypeInformation (typeof (object), "nvarchar(max)", DbType.String, typeof (object), new DefaultConverter (typeof (object)));
      }

      return GetStorageType (value.GetType());
    }

    private StorageTypeInformation GetStorageType (Type type, int? maxLength)
    {
      var underlyingTypeOfNullable = Nullable.GetUnderlyingType (type);
      if (underlyingTypeOfNullable != null)
        return GetStorageTypeForNullableValueType (type, underlyingTypeOfNullable, maxLength);

      if (type.IsEnum)
      {
        var underlyingStorageType = GetStorageType (Enum.GetUnderlyingType (type), maxLength);
        return new StorageTypeInformation (underlyingStorageType.StorageType,
            underlyingStorageType.StorageTypeName, underlyingStorageType.StorageDbType, type, new AdvancedEnumConverter (type));
      }

      if (ExtensibleEnumUtility.IsExtensibleEnumType (type))
      {
        var storageType = GetStorageTypeStringForVarType ("varchar", GetColumnWidthForExtensibleEnum (type));
        return new StorageTypeInformation (typeof (string), storageType, DbType.String, type, new ExtensibleEnumConverter (type));
      }

      if (ReflectionUtility.IsStringPropertyValueType (type))
      {
        string storageType = GetStorageTypeStringForVarType ("nvarchar", maxLength);
        return new StorageTypeInformation (typeof (string), storageType, DbType.String, type, new DefaultConverter (type));
      }

      if (ReflectionUtility.IsBinaryPropertyValueType (type))
      {
        string storageType = GetStorageTypeStringForVarType ("varbinary", maxLength);
        return new StorageTypeInformation (typeof (byte[]), storageType, DbType.Binary, type, new DefaultConverter (type));
      }

      if (type == typeof (Boolean))
        return new StorageTypeInformation (typeof (bool), "bit", DbType.Boolean, type, new DefaultConverter (type));
      if (type == typeof (Byte))
        return new StorageTypeInformation (typeof (byte), "tinyint", DbType.Byte, type, new DefaultConverter (type));
      if (type == typeof (DateTime))
        return new StorageTypeInformation (typeof (DateTime), "datetime", DbType.DateTime, type, new DefaultConverter (type));
      if (type == typeof (Decimal))
        return new StorageTypeInformation (typeof (Decimal), "decimal (38, 3)", DbType.Decimal, type, new DefaultConverter (type));
      if (type == typeof (Double))
        return new StorageTypeInformation (typeof (Double), "float", DbType.Double, type, new DefaultConverter (type));
      if (type == typeof (Guid))
        return new StorageTypeInformation (typeof (Guid), "uniqueidentifier", DbType.Guid, type, new DefaultConverter (type));
      if (type == typeof (Int16))
        return new StorageTypeInformation (typeof (Int16), "smallint", DbType.Int16, type, new DefaultConverter (type));
      if (type == typeof (Int32))
        return new StorageTypeInformation (typeof (Int32), "int", DbType.Int32, type, new DefaultConverter (type));
      if (type == typeof (Int64))
        return new StorageTypeInformation (typeof (Int64), "bigint", DbType.Int64, type, new DefaultConverter (type));
      if (type == typeof (Single))
        return new StorageTypeInformation (typeof (Single), "real", DbType.Single, type, new DefaultConverter (type));

      return null;
    }

    private StorageTypeInformation GetStorageTypeForNullableValueType (Type type, Type underlyingType, int? maxLength)
    {
      var underlyingStorageInformation = GetStorageType (underlyingType, maxLength);
      if (underlyingType.IsEnum)
      {
        return new StorageTypeInformation (
            typeof (Nullable<>).MakeGenericType (underlyingStorageInformation.StorageType),
            underlyingStorageInformation.StorageTypeName,
            underlyingStorageInformation.StorageDbType,
            type,
            new AdvancedEnumConverter (type));
      }

      return new StorageTypeInformation (
          typeof (Nullable<>).MakeGenericType (underlyingStorageInformation.StorageType),
          underlyingStorageInformation.StorageTypeName,
          underlyingStorageInformation.StorageDbType,
          type,
          new DefaultConverter (type));
    }

    private string GetStorageTypeStringForVarType (string varType, int? maxLength)
    {
      return string.Format ("{0} ({1})", varType, maxLength.HasValue ? maxLength.ToString() : "max");
    }

    private int GetColumnWidthForExtensibleEnum (Type extensibleEnumType)
    {
      return ExtensibleEnumUtility.GetDefinition (extensibleEnumType).GetValueInfos().Max (info => info.Value.ID.Length);
    }
  }
}