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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders
{
  public class QueryResultRow : IQueryResultRow
  {
    private readonly IDataReader _dataReader;
    private readonly IStorageTypeInformationProvider _storageTypeInformationProvider;

    public QueryResultRow (IDataReader dataReader, IStorageTypeInformationProvider storageTypeInformationProvider)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);
      ArgumentUtility.CheckNotNull ("storageTypeInformationProvider", storageTypeInformationProvider);

      _storageTypeInformationProvider = storageTypeInformationProvider;
      _dataReader = dataReader;
    }

    public IDataReader DataReader
    {
      get { return _dataReader; }
    }

    public IStorageTypeInformationProvider StorageTypeInformationProvider
    {
      get { return _storageTypeInformationProvider; }
    }

    public int ValueCount
    {
      get { return _dataReader.FieldCount; }
    }

    public object GetRawValue (int position)
    {
      return _dataReader.GetValue (position);
    }

    public object GetConvertedValue (int position, Type type)
    {
      try
      {
        var storageTypeInformation = _storageTypeInformationProvider.GetStorageType (type);
        return storageTypeInformation.Read (_dataReader, position);
      }
      catch (NotSupportedException)
      {
        if (type == typeof (ObjectID))
        {
          throw new NotSupportedException (
              "Type 'ObjectID' ist not supported by this storage provider.\n" +
              "Please select the ID and ClassID values separately (ID.Value, ID.ClassID), then create an ObjectID with it in memory.");
        }

        throw;
      }
    }

    public T GetConvertedValue<T> (int position)
    {
      return (T) GetConvertedValue (position, typeof(T));
    }
  }
}