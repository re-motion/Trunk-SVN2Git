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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// Defines a column in a relational database.
  /// </summary>
  public class ColumnDefinition : IEquatable<ColumnDefinition>
  {
    private readonly string _name;
    private readonly Type _propertyType;
    private readonly bool _isNullable;
    private readonly bool _isPartOfPrimaryKey;
    private readonly IColumnTypeInformation _storageTypeInfo;

    public ColumnDefinition (string name, Type propertyType, IColumnTypeInformation storageTypeInfo, bool isNullable, bool isPartOfPrimaryKey)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);
      ArgumentUtility.CheckNotNull ("storageTypeInfo", storageTypeInfo);
      
      _name = name;
      _propertyType = propertyType;
      _storageTypeInfo = storageTypeInfo;
      _isNullable = isNullable;
      _isPartOfPrimaryKey = isPartOfPrimaryKey;
    }

    public string Name
    {
      get { return _name; }
    }

    public Type PropertyType
    {
      get { return _propertyType; }
    }

    public IColumnTypeInformation StorageTypeInfo
    {
      get { return _storageTypeInfo; }
    }

    public bool IsNullable
    {
      get { return _isNullable; }
    }

    public bool IsPartOfPrimaryKey
    {
      get { return _isPartOfPrimaryKey; }
    }

    public bool Equals (ColumnDefinition other)
    {
      if (other == null)
        return false;

      // TODO Review 4126: Refactor so that other.StorageTypeInfo is compared with StorageTypeInfo
      return other.Name == Name
          && other.PropertyType == PropertyType
          && other.StorageTypeInfo.StorageType == StorageTypeInfo.StorageType
          && other.StorageTypeInfo.DbType == StorageTypeInfo.DbType
          && other.IsNullable == IsNullable;
    }

    public override bool Equals (object obj)
    {
      return Equals (obj as ColumnDefinition);
    }

    public override int GetHashCode ()
    {
      // TODO Review 4126: Change to include full StorageTypeInfo
      return EqualityUtility.GetRotatedHashCode (Name, PropertyType, StorageTypeInfo.StorageType, IsNullable);
    }

    public override string ToString ()
    {
      // TODO Review 4126: Add test for ToString
      // TODO Review 4126: use StorageTypeInfo.ToString
      return string.Format ("{0} {1} {2}", Name, StorageTypeInfo, IsNullable ? "NULL" : "NOT NULL");
    }

    // TODO Review 4125: Remove this member
    public bool IsNull
    {
      get { return false; }
    }
  }
}