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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// The <see cref="ObjectIDStoragePropertyDefinition"/> represents an <see cref="ObjectID"/> property that is stored as an ID column without a 
  /// ClassID column. This can only be used when the <see cref="ClassDefinition"/> of the referenced <see cref="ObjectID"/> is known in advance
  /// (i.e., if there is no inheritance involved).
  /// </summary>
  public class ObjectIDWithoutClassIDStoragePropertyDefinition : IObjectIDStoragePropertyDefinition
  {
    private readonly IRdbmsStoragePropertyDefinition _valueProperty;
    private readonly ClassDefinition _classDefinition;

    public ObjectIDWithoutClassIDStoragePropertyDefinition (IRdbmsStoragePropertyDefinition valueProperty, ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("valueProperty", valueProperty);
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      _valueProperty = valueProperty;
      _classDefinition = classDefinition;
    }

    public IRdbmsStoragePropertyDefinition ValueProperty
    {
      get { return _valueProperty; }
    }

    public ClassDefinition ClassDefinition
    {
      get { return _classDefinition; }
    }

    public IEnumerable<ColumnDefinition> GetColumnsForComparison ()
    {
      return _valueProperty.GetColumnsForComparison();
    }

    public IEnumerable<ColumnDefinition> GetColumns ()
    {
      return _valueProperty.GetColumns();
    }

    public object Read (IDataReader dataReader, IColumnOrdinalProvider ordinalProvider)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);
      ArgumentUtility.CheckNotNull ("ordinalProvider", ordinalProvider);

      var value = _valueProperty.Read (dataReader, ordinalProvider);
      if (value == null)
        return null;
      return new ObjectID (_classDefinition, value);
    }

    public IEnumerable<ColumnValue> SplitValue (object value)
    {
      var objectID = ArgumentUtility.CheckType<ObjectID> ("value", value);
      CheckClassDefinition (objectID, "value");

      var innerValue = GetValueOrNull (objectID);
      return _valueProperty.SplitValue (innerValue);
    }

    public IEnumerable<ColumnValue> SplitValueForComparison (object value)
    {
      var objectID = ArgumentUtility.CheckType<ObjectID> ("value", value);
      CheckClassDefinition (objectID, "value");

      var innerValue = GetValueOrNull (objectID);
      return _valueProperty.SplitValueForComparison (innerValue);
    }

    public ColumnValueTable SplitValuesForComparison (IEnumerable<object> values)
    {
      ArgumentUtility.CheckNotNull ("values", values);

      var innerValues = values.Select (
          v =>
          {
            var objectID = (ObjectID) v;
            CheckClassDefinition (objectID, "values");
            return GetValueOrNull (objectID);
          });

      return _valueProperty.SplitValuesForComparison (innerValues);
    }

    public ForeignKeyConstraintDefinition CreateForeignKeyConstraint (
        Func<IEnumerable<ColumnDefinition>, string> nameProvider,
        EntityNameDefinition referencedTableName,
        ObjectIDStoragePropertyDefinition referencedObjectIDProperty)
    {
      ArgumentUtility.CheckNotNull ("nameProvider", nameProvider);
      ArgumentUtility.CheckNotNull ("referencedTableName", referencedTableName);
      ArgumentUtility.CheckNotNull ("referencedObjectIDProperty", referencedObjectIDProperty);

      var referencingColumns = ValueProperty.GetColumnsForComparison();
      var referencedColumns = referencedObjectIDProperty.ValueProperty.GetColumnsForComparison ();
      return new ForeignKeyConstraintDefinition (nameProvider (referencingColumns), referencedTableName, referencingColumns, referencedColumns);
    }

    private void CheckClassDefinition (ObjectID objectID, string paramName)
    {
      if (objectID != null && objectID.ClassDefinition != _classDefinition)
        throw new ArgumentException ("The specified ObjectID has an invalid ClassDefinition.", paramName);
    }

    private object GetValueOrNull (ObjectID objectID)
    {
      return objectID == null ? null : objectID.Value;
    }

  }
}