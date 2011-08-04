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
using ArgumentUtility = Remotion.Utilities.ArgumentUtility;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// The <see cref="StorageTypeInformation"/> provides information about the storage type of a value in a relational database.
  /// In addition, it can create an unnamed <see cref="IDbDataParameter"/> for a value convertible to <see cref="StorageTypeInMemory"/> via 
  /// <see cref="TypeConverter"/>, or read and convert a value from an <see cref="IDataReader"/>.
  /// </summary>
  /// <remarks>
  /// The <see cref="TypeConverter"/> must be associated with the in-memory .NET type of the stored value. It is used to convert to the database
  /// representation (represented by <see cref="StorageTypeInMemory"/>) when a <see cref="IDbDataParameter"/> is created, and it is used to convert
  /// values back to the .NET format when a value is read from an <see cref="IDataReader"/>.
  /// </remarks>
  public class StorageTypeInformation : IStorageTypeInformation
  {
    private readonly string _storageTypeName;
    private readonly DbType _storageDbType;
    private readonly Type _storageTypeInMemory;
    private readonly TypeConverter _typeConverter;

    public StorageTypeInformation (string storageTypeName, DbType storageDbType, Type storageTypeInMemory, TypeConverter typeConverter)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("storageTypeName", storageTypeName);
      ArgumentUtility.CheckNotNull ("storageTypeInMemory", storageTypeInMemory);
      ArgumentUtility.CheckNotNull ("typeConverter", typeConverter);

      _storageTypeName = storageTypeName;
      _storageDbType = storageDbType;
      _storageTypeInMemory = storageTypeInMemory;
      _typeConverter = typeConverter;
    }

    /// <summary>
    /// Gets the name of the storage type as understood by the underlying database.
    /// </summary>
    /// <value>The name of the storage type.</value>
    public string StorageTypeName
    {
      get { return _storageTypeName; }
    }

    /// <summary>
    /// Gets the <see cref="DbType"/> value corresponding to the storage type.
    /// </summary>
    /// <value>The <see cref="DbType"/> of the storage type.</value>
    public DbType StorageDbType
    {
      get { return _storageDbType; }
    }

    /// <summary>
    /// Gets the storage type as a CLR <see cref="Type"/>; this is the <see cref="Type"/> used to represent values of the storage type in memory.
    /// </summary>
    /// <value>The storage type as a CLR <see cref="Type"/>.</value>
    public Type StorageTypeInMemory
    {
      get { return _storageTypeInMemory; }
    }

    /// <summary>
    /// Gets a <see cref="System.ComponentModel.TypeConverter"/> that can converts a value from the actual .NET type (e.g., an enum type) to the 
    /// <see cref="StorageTypeInMemory"/> (e.g., <see cref="int"/>) and back.
    /// </summary>
    /// <value>The type converter for the actual .NET type.</value>
    /// <remarks>
    /// The <see cref="TypeConverter"/> is used to convert the values passed into <see cref="CreateDataParameter"/> to the underlying 
    /// <see cref="StorageTypeInMemory"/>. That way, an enum value passed into <see cref="CreateDataParameter"/> can be converted to the underlying
    /// <see cref="int"/> type when it is to be written into the database. Conversely, <see cref="Read"/> uses the <see cref="TypeConverter"/> to
    /// convert values read from the database (which should usually be of the <see cref="StorageTypeInMemory"/>) back to the expected .NET type. That
    /// way, e.g, an <see cref="int"/> value can become an enum value again.
    /// </remarks>
    public TypeConverter TypeConverter
    {
      get { return _typeConverter; }
    }

    public IDbDataParameter CreateDataParameter (IDbCommand command, object value)
    {
      ArgumentUtility.CheckNotNull ("command", command);

      var convertedValue = TypeConverter.ConvertTo (value, StorageTypeInMemory);

      var parameter = command.CreateParameter ();
      parameter.Value = convertedValue ?? DBNull.Value;
      parameter.DbType = StorageDbType;
      return parameter;
    }

    public object Read (IDataReader dataReader, int ordinal)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);

      var value = dataReader[ordinal];
      if (value == DBNull.Value)
        value = null;

      return TypeConverter.ConvertFrom (value);
    }
  }
}