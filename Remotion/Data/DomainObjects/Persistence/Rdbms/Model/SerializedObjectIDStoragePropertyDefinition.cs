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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// The <see cref="ObjectIDStoragePropertyDefinition"/> represents an <see cref="ObjectID"/> property that is stored in a single string-typed column.
  /// </summary>
  public class SerializedObjectIDStoragePropertyDefinition : IObjectIDStoragePropertyDefinition
  {
    private readonly SimpleStoragePropertyDefinition _serializedIDProperty;

    public SerializedObjectIDStoragePropertyDefinition (SimpleStoragePropertyDefinition serializedIDProperty)
    {
      ArgumentUtility.CheckNotNull ("serializedIDProperty", serializedIDProperty);

      _serializedIDProperty = serializedIDProperty;
    }

    public SimpleStoragePropertyDefinition SerializedIDProperty
    {
      get { return _serializedIDProperty; }
    }

    public ColumnDefinition GetColumnForLookup ()
    {
      return _serializedIDProperty.ColumnDefinition;
    }

    public ColumnDefinition GetColumnForForeignKey ()
    {
      return null;
    }

    public IEnumerable<ColumnDefinition> GetColumns ()
    {
      return _serializedIDProperty.GetColumns();
    }

    public string Name
    {
      get { return _serializedIDProperty.Name; }
    }

    public bool Equals (IRdbmsStoragePropertyDefinition other)
    {
      if (other == null || other.GetType () != GetType ())
        return false;

      return _serializedIDProperty.Equals (((SerializedObjectIDStoragePropertyDefinition) other).SerializedIDProperty);
    }
    
  }
}