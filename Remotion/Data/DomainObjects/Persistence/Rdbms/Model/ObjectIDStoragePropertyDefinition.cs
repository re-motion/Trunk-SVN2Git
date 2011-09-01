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
using System.Linq;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// The <see cref="ObjectIDStoragePropertyDefinition"/> represents an <see cref="ObjectID"/> property that is stored as an ID column and a ClassID
  /// column.
  /// </summary>
  public class ObjectIDStoragePropertyDefinition : IObjectIDStoragePropertyDefinition
  {
    private readonly IRdbmsStoragePropertyDefinition _valueProperty;
    private readonly IRdbmsStoragePropertyDefinition _classIDProperty;

    public ObjectIDStoragePropertyDefinition (IRdbmsStoragePropertyDefinition valueProperty, IRdbmsStoragePropertyDefinition classIDProperty)
    {
      ArgumentUtility.CheckNotNull ("valueProperty", valueProperty);
      ArgumentUtility.CheckNotNull ("classIDProperty", classIDProperty);

      _valueProperty = valueProperty;
      _classIDProperty = classIDProperty;
    }

    public IRdbmsStoragePropertyDefinition ValueProperty
    {
      get { return _valueProperty; }
    }

    public IRdbmsStoragePropertyDefinition ClassIDProperty
    {
      get { return _classIDProperty; }
    }

    public ColumnDefinition GetColumnForLookup ()
    {
      // TODO in case of integer primary keys: 
      // If RdbmsProvider or one of its derived classes needs to support integer primary keys in addition to GUIDs,
      // two lookup columns should be used: ID and ClassID (because int IDs wouldn't be globally unique).
      // For GUID keys, we don't want to include the ClassID, however.

      return _valueProperty.GetColumnForLookup();
    }

    public ColumnDefinition GetColumnForForeignKey ()
    {
      return _valueProperty.GetColumnForForeignKey();
    }

    public IEnumerable<ColumnDefinition> GetColumns ()
    {
      return _valueProperty.GetColumns().Concat (_classIDProperty.GetColumns());
    }

    public object Read (IDataReader dataReader, IColumnOrdinalProvider ordinalProvider)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);
      ArgumentUtility.CheckNotNull ("ordinalProvider", ordinalProvider);

      var value = _valueProperty.Read (dataReader, ordinalProvider);
      var classID = (string) _classIDProperty.Read (dataReader, ordinalProvider);
      if (value == null)
      {
        if (classID != null)
        {
          throw new RdbmsProviderException (
              string.Format (
                  "Incorrect database value encountered. The value read from '{0}' must contain null.",
                  SeparatedStringBuilder.Build (", ", _classIDProperty.GetColumns(), c => c.Name)));
        }

        return null;
      }

      if (classID == null)
      {
        throw new RdbmsProviderException (
            string.Format (
                "Incorrect database value encountered. The value read from '{0}' must not contain null.",
                SeparatedStringBuilder.Build (", ", _classIDProperty.GetColumns(), c => c.Name)));
      }

      return new ObjectID (classID, value);
    }

    public IEnumerable<ColumnValue> SplitValue (object value)
    {
      var objectID = ArgumentUtility.CheckType<ObjectID> ("value", value);
      if (objectID == null)
        return _valueProperty.SplitValue (null).Concat (_classIDProperty.SplitValue (null));

      return _valueProperty.SplitValue (objectID.Value).Concat (_classIDProperty.SplitValue (objectID.ClassID));
    }

    public IEnumerable<ColumnValue> SplitValueForComparison (object value)
    {
      var objectID = ArgumentUtility.CheckType<ObjectID> ("value", value);

      return _valueProperty.SplitValueForComparison (GetValueOrNull (objectID));
    }

    public ColumnValueTable SplitValuesForComparison (IEnumerable<object> values)
    {
      ArgumentUtility.CheckNotNull ("values", values);

      return _valueProperty.SplitValuesForComparison (values.Select (v => GetValueOrNull ((ObjectID) v)));
    }

    public ForeignKeyConstraintDefinition CreateForeignKeyConstraint (
        Func<IEnumerable<ColumnDefinition>, string> nameProvider,
        EntityNameDefinition referencedTableName,
        ObjectIDStoragePropertyDefinition referencedObjectIDProperty)
    {
      ArgumentUtility.CheckNotNull ("nameProvider", nameProvider);
      ArgumentUtility.CheckNotNull ("referencedTableName", referencedTableName);
      ArgumentUtility.CheckNotNull ("referencedObjectIDProperty", referencedObjectIDProperty);

      var referencingColumns = new[] { GetColumnForLookup () };
      var referencedColumns = new[] { referencedObjectIDProperty.GetColumnForLookup () };
      return new ForeignKeyConstraintDefinition (nameProvider (referencingColumns),  referencedTableName,  referencingColumns, referencedColumns);
    }

    private object GetValueOrNull (ObjectID objectID)
    {
      return objectID != null ? objectID.Value : null;
    }
  }
}