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
  /// <see cref="SqlStorageTypeCalculator"/> calculates the SQL Server-specific type for a column in a relational database.
  /// </summary>
  public class SqlStorageTypeCalculator : StorageTypeCalculator
  {
    public SqlStorageTypeCalculator (IStorageProviderDefinitionFinder storageProviderDefinitionFinder)
        : base (storageProviderDefinitionFinder)
    {
    }

    public override IColumnTypeInformation SqlDataTypeObjectID
    {
      get { return new StorageTypeInformation ("uniqueidentifier", DbType.Guid); }
    }

    public override IColumnTypeInformation SqlDataTypeSerializedObjectID
    {
      get { return new StorageTypeInformation ("varchar (255)", DbType.String); }
    }

    public override IColumnTypeInformation SqlDataTypeClassID
    {
      get { return new StorageTypeInformation ("varchar (100)", DbType.String); }
    }

    public override IColumnTypeInformation SqlDataTypeTimestamp
    {
      get { return new StorageTypeInformation("rowversion", DbType.String); } //TODO 4126: ok ??
    }

    public override IColumnTypeInformation GetStorageType (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      var sqlDataType = GetSqlDataType (propertyDefinition.PropertyType);
      if (sqlDataType != null)
        return sqlDataType;

      if (ReflectionUtility.IsStringPropertyValueType (propertyDefinition.PropertyType))
      {
        return
            new StorageTypeInformation (
                string.Format ("nvarchar ({0})", propertyDefinition.MaxLength.HasValue ? propertyDefinition.MaxLength.ToString() : "max"),
                DbType.String);
      }

      if (ReflectionUtility.IsBinaryPropertyValueType (propertyDefinition.PropertyType))
      {
        return
            new StorageTypeInformation (
                string.Format ("varbinary ({0})", propertyDefinition.MaxLength.HasValue ? propertyDefinition.MaxLength.ToString() : "max"),
                DbType.Binary);
      }

      return base.GetStorageType (propertyDefinition);
    }

    private static IColumnTypeInformation GetSqlDataType (Type type)
    {
      type = Nullable.GetUnderlyingType (type) ?? type;

      if (type == typeof (Boolean))
        return new StorageTypeInformation ("bit", DbType.Boolean);
      if (type == typeof (Byte))
        return new StorageTypeInformation ("tinyint", DbType.Int16); //TODO 4126: ok ??
      if (type == typeof (DateTime))
        return new StorageTypeInformation ("datetime", DbType.DateTime);
      if (type == typeof (Decimal))
        return new StorageTypeInformation ("decimal (38, 3)", DbType.Decimal);
      if (type == typeof (Double))
        return new StorageTypeInformation ("float", DbType.Double);
      if (type == typeof (Guid))
        return new StorageTypeInformation ("uniqueidentifier", DbType.Guid);
      if (type == typeof (Int16))
        return new StorageTypeInformation ("smallint", DbType.Int16);
      if (type == typeof (Int32))
        return new StorageTypeInformation ("int", DbType.Int32);
      if (type == typeof (Int64))
        return new StorageTypeInformation ("bigint", DbType.Int64);
      if (type == typeof (Single))
        return new StorageTypeInformation ("real", DbType.Single);
      if (type.IsEnum)
        return GetSqlDataType (Enum.GetUnderlyingType (type));

      if (ExtensibleEnumUtility.IsExtensibleEnumType (type))
        return new StorageTypeInformation (string.Format ("varchar ({0})", GetColumnWidthForExtensibleEnum (type)), DbType.String);

      return null;
    }

    private static int GetColumnWidthForExtensibleEnum (Type extensibleEnumType)
    {
      return ExtensibleEnumUtility.GetDefinition (extensibleEnumType).GetValueInfos().Max (info => info.Value.ID.Length);
    }
  }
}