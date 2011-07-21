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
    public SqlStorageTypeCalculator ()
    {
    }

    public override StorageTypeInformation ObjectIDStorageType
    {
      get { return new StorageTypeInformation ("uniqueidentifier", DbType.Guid, typeof (Guid?), new DefaultConverter (typeof(Guid?))); }
    }

    public override StorageTypeInformation SerializedObjectIDStorageType
    {
      get { return new StorageTypeInformation ("varchar (255)", DbType.String, typeof (string), new DefaultConverter (typeof (string))); }
    }

    public override StorageTypeInformation ClassIDStorageType
    {
      get { return new StorageTypeInformation ("varchar (100)", DbType.String, typeof (string), new DefaultConverter (typeof (string))); }
    }

    public override StorageTypeInformation TimestampStorageType
    {
      get { return new StorageTypeInformation ("rowversion", DbType.Binary, typeof (byte[]), new DefaultConverter(typeof(byte[]))); }
    }

    public override StorageTypeInformation GetStorageType (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      return GetStorageType (propertyDefinition.PropertyType, propertyDefinition.MaxLength);
    }

    private StorageTypeInformation GetStorageType (Type propertyType, int? maxLength)
    {
      var underlyingTypeOfNullable = Nullable.GetUnderlyingType (propertyType);
      if (underlyingTypeOfNullable != null)
        return GetStorageTypeForNullableValueType (propertyType, underlyingTypeOfNullable, maxLength);

      if (propertyType.IsEnum)
      {
        var underlyingStorageType = GetStorageType (Enum.GetUnderlyingType (propertyType), maxLength);
        return new StorageTypeInformation (
            underlyingStorageType.StorageType,
            underlyingStorageType.DbType,
            underlyingStorageType.ParameterValueType,
            new AdvancedEnumConverter (propertyType));
      }

      if (ExtensibleEnumUtility.IsExtensibleEnumType (propertyType))
      {
        var storageType = GetStorageTypeStringForVarType ("varchar", GetColumnWidthForExtensibleEnum (propertyType));
        return new StorageTypeInformation (storageType, DbType.String, typeof (string), new ExtensibleEnumConverter (propertyType));
      }

      if (ReflectionUtility.IsStringPropertyValueType (propertyType))
      {
        string storageType = GetStorageTypeStringForVarType ("nvarchar", maxLength);
        return new StorageTypeInformation (storageType, DbType.String, typeof (string), new DefaultConverter (typeof (string)));
      }

      if (ReflectionUtility.IsBinaryPropertyValueType (propertyType))
      {
        string storageType = GetStorageTypeStringForVarType ("varbinary", maxLength);
        return new StorageTypeInformation (storageType, DbType.Binary, typeof (byte[]), new DefaultConverter (typeof (Byte[])));
      }

      if (propertyType == typeof (Boolean))
        return new StorageTypeInformation ("bit", DbType.Boolean, typeof (bool), new DefaultConverter (typeof (bool)));
      if (propertyType == typeof (Byte))
        return new StorageTypeInformation ("tinyint", DbType.Byte, typeof (byte), new DefaultConverter (typeof (byte)));
      if (propertyType == typeof (DateTime))
        return new StorageTypeInformation ("datetime", DbType.DateTime, typeof (DateTime), new DefaultConverter (typeof (DateTime)));
      if (propertyType == typeof (Decimal))
        return new StorageTypeInformation ("decimal (38, 3)", DbType.Decimal, typeof (Decimal), new DefaultConverter (typeof (Decimal)));
      if (propertyType == typeof (Double))
        return new StorageTypeInformation ("float", DbType.Double, typeof (Double), new DefaultConverter (typeof (Double)));
      if (propertyType == typeof (Guid))
        return new StorageTypeInformation ("uniqueidentifier", DbType.Guid, typeof (Guid), new DefaultConverter (typeof (Guid)));
      if (propertyType == typeof (Int16))
        return new StorageTypeInformation ("smallint", DbType.Int16, typeof (Int16), new DefaultConverter (typeof (Int16)));
      if (propertyType == typeof (Int32))
        return new StorageTypeInformation ("int", DbType.Int32, typeof (Int32), new DefaultConverter (typeof (Int32)));
      if (propertyType == typeof (Int64))
        return new StorageTypeInformation ("bigint", DbType.Int64, typeof (Int64), new DefaultConverter (typeof (Int64)));
      if (propertyType == typeof (Single))
        return new StorageTypeInformation ("real", DbType.Single, typeof (Single), new DefaultConverter (typeof (Single)));

      return null; // TODO Review 4167: Throw NotSupportedException ("Type '...' is not supported by this storage provider.");
    }

    private StorageTypeInformation GetStorageTypeForNullableValueType (Type type, Type underlyingType, int? maxLength)
    {
      var underlyingStorageInformation = GetStorageType (underlyingType, maxLength);
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