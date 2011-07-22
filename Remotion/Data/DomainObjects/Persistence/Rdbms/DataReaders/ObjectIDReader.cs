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
using System.Collections.Generic;
using System.Data;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders
{
  /// <summary>
  /// Reads data from an <see cref="IDataReader"/> and converts it into <see cref="ObjectID"/> instances.
  /// </summary>
  public class ObjectIDReader : IObjectReader<ObjectID>
  {
    private readonly IRdbmsStoragePropertyDefinition _idProperty;
    private readonly IColumnOrdinalProvider _columnOrdinalProvider;

    public ObjectIDReader (IRdbmsStoragePropertyDefinition idProperty, IColumnOrdinalProvider columnOrdinalProvider)
    {
      ArgumentUtility.CheckNotNull ("idProperty", idProperty);
      ArgumentUtility.CheckNotNull ("columnOrdinalProvider", columnOrdinalProvider);

      _idProperty = idProperty;
      _columnOrdinalProvider = columnOrdinalProvider;
    }

    public ObjectID Read (IDataReader dataReader)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);

      if (dataReader.Read ())
        return (ObjectID) _idProperty.Read(dataReader, _columnOrdinalProvider);
      else
        return null;
    }

    public IEnumerable<ObjectID> ReadSequence (IDataReader dataReader)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);

      while (dataReader.Read ())
        yield return (ObjectID)_idProperty.Read (dataReader, _columnOrdinalProvider);
    }
  }
}