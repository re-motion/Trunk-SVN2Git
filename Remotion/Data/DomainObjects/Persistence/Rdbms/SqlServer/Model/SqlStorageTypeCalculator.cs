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
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.ExtensibleEnums;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model
{
  /// <summary>
  /// <see cref="SqlStorageTypeCalculator"/> calculates the SQL Server-specific type for a column in a relational database.
  /// </summary>
  public class SqlStorageTypeCalculator : StorageTypeCalculator
  {
    public override string SqlDataTypeObjectID
    {
      get { return "uniqueidentifier"; }
    }

    public override string SqlDataTypeSerializedObjectID
    {
      get { return "varchar (255)"; }
    }

    public override string SqlDataTypeClassID
    {
      get { return "varchar (100)"; }
    }

    public override string GetStorageType (PropertyDefinition propertyDefinition, IStorageProviderDefinitionFinder storageProviderDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      
      string sqlDataType = GetSqlDataType (propertyDefinition.PropertyType);
      if (!string.IsNullOrEmpty (sqlDataType))
        return sqlDataType;

      if (propertyDefinition.PropertyType == typeof (String))
        return string.Format ("nvarchar ({0})", propertyDefinition.MaxLength.HasValue ? propertyDefinition.MaxLength.ToString () : "max");

      if (propertyDefinition.PropertyType == typeof (Byte[]))
        return string.Format ("varbinary ({0})", propertyDefinition.MaxLength.HasValue ? propertyDefinition.MaxLength.ToString () : "max");

      return base.GetStorageType (propertyDefinition, storageProviderDefinitionFinder);
    }

    private static string GetSqlDataType (Type type)
    {
      type = Nullable.GetUnderlyingType (type) ?? type;

      if (type == typeof (Boolean))
        return "bit";
      if (type == typeof (Byte))
        return "tinyint";
      if (type == typeof (DateTime))
        return "datetime";
      if (type == typeof (Decimal))
        return "decimal (38, 3)";
      if (type == typeof (Double))
        return "float";
      if (type == typeof (Guid))
        return "uniqueidentifier";
      if (type == typeof (Int16))
        return "smallint";
      if (type == typeof (Int32))
        return "int";
      if (type == typeof (Int64))
        return "bigint";
      if (type == typeof (Single))
        return "real";
      if (type.IsEnum)
        return GetSqlDataType (Enum.GetUnderlyingType (type));

      if (ExtensibleEnumUtility.IsExtensibleEnumType (type))
        return string.Format ("varchar ({0})", GetColumnWidthForExtensibleEnum (type));

      return null;
    }

    private static int GetColumnWidthForExtensibleEnum (Type extensibleEnumType)
    {
      return ExtensibleEnumUtility.GetDefinition (extensibleEnumType).GetValueInfos ().Max (info => info.Value.ID.Length);
    }
  }
}