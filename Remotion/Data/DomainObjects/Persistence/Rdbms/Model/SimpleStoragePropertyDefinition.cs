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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.ValueSplitting;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// Defines that a property maps directly to a simple column in a relational database.
  /// </summary>
  public class SimpleStoragePropertyDefinition : IRdbmsStoragePropertyDefinition
  {
    private readonly ColumnDefinition _columnDefinition;
    private readonly IValueSplitter _valueSplitter;

    public SimpleStoragePropertyDefinition (ColumnDefinition columnDefinition)
    {
      ArgumentUtility.CheckNotNull ("columnDefinition", columnDefinition);

      _columnDefinition = columnDefinition;
      _valueSplitter = new SimpleValueSplitter (columnDefinition.StorageTypeInfo);
    }

    public ColumnDefinition ColumnDefinition
    {
      get { return _columnDefinition; }
    }

    public IValueSplitter ValueSplitter
    {
      get { return _valueSplitter; }
    }

    public IEnumerable<ColumnDefinition> GetColumns ()
    {
      yield return _columnDefinition;
    }

    public ColumnDefinition GetColumnForLookup ()
    {
      return _columnDefinition;
    }

    public ColumnDefinition GetColumnForForeignKey ()
    {
      return _columnDefinition;
    }

    public object Read (IDataReader dataReader, IColumnOrdinalProvider ordinalProvider)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);
      ArgumentUtility.CheckNotNull ("ordinalProvider", ordinalProvider);

      int ordinal = ordinalProvider.GetOrdinal (_columnDefinition, dataReader);
      // Optimization: Do not use a value splitter to combine the value, it's only one value anyway.
      var readValue = _columnDefinition.StorageTypeInfo.Read (dataReader, ordinal);
      return readValue;
    }

    public IEnumerable<ColumnValue> SplitValue (object value)
    {
      // Optimization: Do not use a value splitter to split the value, it's only one value anyway.
      return new[] { new ColumnValue (_columnDefinition, value) };
    }

    public IEnumerable<ColumnValue> SplitValueForComparison (object value)
    {
      // Optimization: Do not use a value splitter to split the value, it's only one value anyway.
      return SplitValue (value);
    }
  }
}