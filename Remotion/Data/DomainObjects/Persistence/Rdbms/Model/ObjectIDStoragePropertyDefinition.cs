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
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// The <see cref="ObjectIDStoragePropertyDefinition"/> represents an <see cref="ObjectID"/> property that is stored as an ID column and a ClassID
  /// column.
  /// </summary>
  public class ObjectIDStoragePropertyDefinition : IObjectIDStoragePropertyDefinition
  {
    private readonly SimpleStoragePropertyDefinition _valueProperty;
    private readonly SimpleStoragePropertyDefinition _classIDProperty;

    public ObjectIDStoragePropertyDefinition (SimpleStoragePropertyDefinition valueProperty, SimpleStoragePropertyDefinition classIDProperty)
    {
      ArgumentUtility.CheckNotNull ("valueProperty", valueProperty);
      ArgumentUtility.CheckNotNull ("classIDProperty", classIDProperty);

      _valueProperty = valueProperty;
      _classIDProperty = classIDProperty;
    }

    public SimpleStoragePropertyDefinition ValueProperty
    {
      get { return _valueProperty; }
    }

    public SimpleStoragePropertyDefinition ClassIDProperty
    {
      get { return _classIDProperty; }
    }

    string IStoragePropertyDefinition.Name
    {
      get { return _valueProperty.Name; }
    }

    public ColumnDefinition GetColumnForLookup ()
    {
      return _valueProperty.ColumnDefinition;
    }

    public ColumnDefinition GetColumnForForeignKey ()
    {
      return _valueProperty.ColumnDefinition;
    }

    public IEnumerable<ColumnDefinition> GetColumns ()
    {
      yield return _valueProperty.ColumnDefinition;
      yield return _classIDProperty.ColumnDefinition;
    }

    public object Read (IDataReader dataReader, IColumnOrdinalProvider ordinalProvider)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);
      ArgumentUtility.CheckNotNull ("ordinalProvider", ordinalProvider);

      var value = _valueProperty.Read (dataReader, ordinalProvider);
      if (value == null)
        return null;
      return new ObjectID (_classIDProperty.Name, value);

    }

    public IEnumerable<IDataParameter> CreateDataParameters (IDbCommand command, object value, string key)
    {
      ArgumentUtility.CheckNotNull ("command", command);
      ArgumentUtility.CheckNotNullOrEmpty ("key", key);

      var objectID = ArgumentUtility.CheckNotNullAndType<ObjectID> ("value", value);

      foreach (var dataParameter in _valueProperty.CreateDataParameters (command, objectID.Value.ToString (), key))
        yield return dataParameter;
      foreach (var dataParameter in _classIDProperty.CreateDataParameters (command, objectID.ClassID, key + "ClassID"))
        yield return dataParameter;
    }
  }
}