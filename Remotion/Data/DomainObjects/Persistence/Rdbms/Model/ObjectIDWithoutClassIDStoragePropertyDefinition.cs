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

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// The <see cref="ObjectIDStoragePropertyDefinition"/> represents an <see cref="ObjectID"/> property that is stored as an ID column without a 
  /// ClassID column. This can only be used when the <see cref="ClassDefinition"/> of the referenced <see cref="ObjectID"/> is known in advance
  /// (i.e., if there is no inheritance involved).
  /// </summary>
  public class ObjectIDWithoutClassIDStoragePropertyDefinition : IRdbmsStoragePropertyDefinition
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

    public string Name
    {
      get { return _valueProperty.Name; }
    }

    public IRdbmsStoragePropertyDefinition ValueProperty
    {
      get { return _valueProperty; }
    }

    public ClassDefinition ClassDefinition
    {
      get { return _classDefinition; }
    }

    public ColumnDefinition GetColumnForLookup ()
    {
      return _valueProperty.GetColumnForLookup();
    }

    public ColumnDefinition GetColumnForForeignKey ()
    {
      return _valueProperty.GetColumnForForeignKey();
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
      if (objectID == null)
        return _valueProperty.SplitValue (null);

      if (objectID.ClassDefinition != _classDefinition)
        throw new ArgumentException ("The specified ObjectID has an invalid ClassDefinition.", "value");

      return _valueProperty.SplitValue (objectID.Value);
    }

    public IEnumerable<ColumnValue> SplitValueForComparison (object value)
    {
      var objectID = ArgumentUtility.CheckType<ObjectID> ("value", value);

      if (objectID == null)
        return _valueProperty.SplitValueForComparison (null);

      // TODO Review 4183: Test case where ClassDefinition is invalid
      if (objectID.ClassDefinition != _classDefinition)
        throw new ArgumentException ("The specified ObjectID has an invalid ClassDefinition.", "value");

      return _valueProperty.SplitValueForComparison (objectID.Value);
    }
  }
}