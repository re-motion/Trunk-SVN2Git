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
using System.Linq;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// The <see cref="CompoundStoragePropertyDefinition"/> can be used to read and store complex values.
  /// </summary>
  public class CompoundStoragePropertyDefinition : IRdbmsStoragePropertyDefinition
  {
    public struct NestedPropertyInfo
    {
      private readonly IRdbmsStoragePropertyDefinition _storagePropertyDefinition;
      private readonly Func<object, object> _valueAccessor;

      public NestedPropertyInfo (IRdbmsStoragePropertyDefinition storagePropertyDefinition, Func<object, object> valueAccessor)
      {
        ArgumentUtility.CheckNotNull ("storagePropertyDefinition", storagePropertyDefinition);
        ArgumentUtility.CheckNotNull ("valueAccessor", valueAccessor);

        _storagePropertyDefinition = storagePropertyDefinition;
        _valueAccessor = valueAccessor;
      }

      public IRdbmsStoragePropertyDefinition StoragePropertyDefinition
      {
        get { return _storagePropertyDefinition; }
      }

      public Func<object, object> ValueAccessor
      {
        get { return _valueAccessor; }
      }
    }

    private readonly NestedPropertyInfo[] _properties;
    private readonly Func<object[], object> _valueCombinator;
    private readonly Type _propertyType;

    public CompoundStoragePropertyDefinition (Type propertyType, IEnumerable<NestedPropertyInfo> properties, Func<object[], object> valueCombinator)
    {
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);
      ArgumentUtility.CheckNotNull ("properties", properties);
      ArgumentUtility.CheckNotNull ("valueCombinator", valueCombinator);

      _propertyType = propertyType;
      _properties = properties.ToArray ();
      _valueCombinator = valueCombinator;
    }

    public Type PropertyType
    {
      get { return _propertyType; }
    }

    public IEnumerable<ColumnDefinition> GetColumns ()
    {
      return _properties.SelectMany (p => p.StoragePropertyDefinition.GetColumns());
    }

    public IEnumerable<ColumnDefinition> GetColumnsForComparison ()
    {
      return _properties.SelectMany (p => p.StoragePropertyDefinition.GetColumnsForComparison());
    }

    public object Read (IDataReader dataReader, IColumnOrdinalProvider ordinalProvider)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);
      ArgumentUtility.CheckNotNull ("ordinalProvider", ordinalProvider);

      var values = _properties.Select (p => p.StoragePropertyDefinition.Read (dataReader, ordinalProvider)).ToArray();
      return _valueCombinator (values);
    }

    public IEnumerable<ColumnValue> SplitValue (object value)
    {
      return _properties.SelectMany (p => p.StoragePropertyDefinition.SplitValue (p.ValueAccessor (value)));
    }

    public IEnumerable<ColumnValue> SplitValueForComparison (object value)
    {
      return _properties.SelectMany (p => p.StoragePropertyDefinition.SplitValueForComparison (p.ValueAccessor (value)));
    }

    public ColumnValueTable SplitValuesForComparison (IEnumerable<object> values)
    {
      ArgumentUtility.CheckNotNull ("values", values);

      var valueList = values.ToList();
      return ColumnValueTable.Combine (
          _properties.Select (p => p.StoragePropertyDefinition.SplitValuesForComparison (valueList.Select (v => p.ValueAccessor (v)))));
    }
  }
}