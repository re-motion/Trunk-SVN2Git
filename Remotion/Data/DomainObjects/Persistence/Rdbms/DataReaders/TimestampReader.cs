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
using System.Collections.Generic;
using System.Data;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders
{
  /// <summary>
  /// Reads data from an <see cref="IDataReader"/> and converts it into timestamp instances.
  /// </summary>
  public class TimestampReader : IObjectReader<Tuple<ObjectID, object>>
  {
    private readonly IRdbmsStoragePropertyDefinition _idProperty;
    private readonly IRdbmsStoragePropertyDefinition _timestampProperty;
    private readonly IColumnOrdinalProvider _columnOrdinalProvider;

    public TimestampReader (
        IRdbmsStoragePropertyDefinition idProperty, IRdbmsStoragePropertyDefinition timestampProperty, IColumnOrdinalProvider columnOrdinalProvider)
    {
      ArgumentUtility.CheckNotNull ("idProperty", idProperty);
      ArgumentUtility.CheckNotNull ("timestampProperty", timestampProperty);
      ArgumentUtility.CheckNotNull ("columnOrdinalProvider", columnOrdinalProvider);

      _idProperty = idProperty;
      _timestampProperty = timestampProperty;
      _columnOrdinalProvider = columnOrdinalProvider;
    }

    public IRdbmsStoragePropertyDefinition IDProperty
    {
      get { return _idProperty; }
    }

    public IRdbmsStoragePropertyDefinition TimestampProperty
    {
      get { return _timestampProperty; }
    }

    public IColumnOrdinalProvider ColumnOrdinalProvider
    {
      get { return _columnOrdinalProvider; }
    }

    public Tuple<ObjectID, object> Read (IDataReader dataReader)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);

      if (dataReader.Read ())
        return GetTimestampTuple(dataReader);
      else
        return null;
    }

    public IEnumerable<Tuple<ObjectID, object>> ReadSequence (IDataReader dataReader)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);

      while (dataReader.Read ())
        yield return GetTimestampTuple (dataReader);
    }

    private Tuple<ObjectID, object> GetTimestampTuple (IDataReader dataReader)
    {
      var objectIDValue = (ObjectID) _idProperty.Read (dataReader, _columnOrdinalProvider);
      if (objectIDValue == null)
        return null;

      var timestampValue = _timestampProperty.Read (dataReader, _columnOrdinalProvider);
      return Tuple.Create (objectIDValue, timestampValue);
    }
  }
}