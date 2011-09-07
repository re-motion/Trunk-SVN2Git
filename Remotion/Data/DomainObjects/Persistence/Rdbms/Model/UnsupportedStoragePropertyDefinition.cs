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
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  public class UnsupportedStoragePropertyDefinition : IRdbmsStoragePropertyDefinition
  {
    private readonly Type _propertyType;
    private readonly string _message;

    public UnsupportedStoragePropertyDefinition (Type propertyType, string message)
    {
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);
      ArgumentUtility.CheckNotNullOrEmpty ("message", message);

      _propertyType = propertyType;
      _message = message;
    }

    public string Message
    {
      get { return _message; }
    }

    public Type PropertyType
    {
      get { return _propertyType; }
    }

    public IEnumerable<ColumnDefinition> GetColumns ()
    {
      return new ColumnDefinition[0];
    }

    public IEnumerable<ColumnDefinition> GetColumnsForComparison ()
    {
      throw new NotSupportedException();
    }

    public object Read (IDataReader dataReader, IColumnOrdinalProvider ordinalProvider)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);
      ArgumentUtility.CheckNotNull ("ordinalProvider", ordinalProvider);

      throw new NotSupportedException();
    }

    public IEnumerable<ColumnValue> SplitValue (object value)
    {
      throw new NotSupportedException();
    }

    public IEnumerable<ColumnValue> SplitValueForComparison (object value)
    {
      throw new NotSupportedException ();
    }

    public ColumnValueTable SplitValuesForComparison (IEnumerable<object> values)
    {
      throw new NotSupportedException();
    }

    public object CombineValue (IColumnValueProvider columnValueProvider)
    {
      throw new NotSupportedException();
    }
  }
}