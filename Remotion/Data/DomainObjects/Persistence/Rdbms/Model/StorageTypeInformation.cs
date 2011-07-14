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
using Remotion.Utilities;
using ArgumentUtility = Remotion.Utilities.ArgumentUtility;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// The <see cref="StorageTypeInformation"/> provides information of the storage-type and database-type for a column in a relational database.
  /// </summary>
  public struct StorageTypeInformation
  {
    private readonly string _storageType;
    private readonly DbType _dbType;
    private readonly Type _parameterValueType;
    private readonly TypeConverter _typeConverter;

    public StorageTypeInformation (string storageType, DbType dbType, Type parameterValueType, TypeConverter typeConverter)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("storageType", storageType);
      ArgumentUtility.CheckNotNull ("parameterValueType", parameterValueType);
      ArgumentUtility.CheckNotNull ("typeConverter", typeConverter);

      _storageType = storageType;
      _dbType = dbType;
      _parameterValueType = parameterValueType;
      _typeConverter = typeConverter;
    }

    public string StorageType
    {
      get { return _storageType; }
    }

    public DbType DbType
    {
      get { return _dbType; }
    }

    public Type ParameterValueType
    {
      get { return _parameterValueType; }
    }

    public TypeConverter TypeConverter
    {
      get { return _typeConverter; }
    }

    public override bool Equals (object obj)
    {
      if (obj == null || obj.GetType() != GetType())
        return false;

      var other = (StorageTypeInformation) obj;
      return other.StorageType == StorageType 
          && other.DbType == DbType 
          && other.ParameterValueType == ParameterValueType
          && other.TypeConverter.GetType() == TypeConverter.GetType();
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (StorageType, DbType, ParameterValueType, TypeConverter.GetType());
    }
  }
}