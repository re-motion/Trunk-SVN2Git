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
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// The <see cref="IDColumnDefinition"/> represents an <see cref="ObjectID"/>-column with a class id.
  /// </summary>
  public class IDColumnDefinition : IRdbmsStoragePropertyDefinition
  {
    private readonly SimpleColumnDefinition _objectIDColumn;
    private readonly SimpleColumnDefinition _classIDColumn;

    public IDColumnDefinition (SimpleColumnDefinition objectIDColumn, SimpleColumnDefinition classIDColumn)
    {
      ArgumentUtility.CheckNotNull ("objectIDColumn", objectIDColumn);

      _objectIDColumn = objectIDColumn;
      _classIDColumn = classIDColumn;
    }

    string IStoragePropertyDefinition.Name
    {
      get { return _objectIDColumn.Name; }
    }

    public SimpleColumnDefinition ObjectIDColumn
    {
      get { return _objectIDColumn; }
    }

    public SimpleColumnDefinition ClassIDColumn
    {
      get { return _classIDColumn; }
    }

    public bool HasClassIDColumn
    {
      get { return _classIDColumn != null; }
    }

    public IEnumerable<SimpleColumnDefinition> GetColumns ()
    {
      yield return _objectIDColumn;
      if(HasClassIDColumn)
        yield return _classIDColumn;
    }

    public bool Equals (IRdbmsStoragePropertyDefinition other)
    {
      if (other == null || other.GetType() != GetType())
        return false;

      var castOther = (IDColumnDefinition) other;
      return Equals (castOther.ObjectIDColumn, ObjectIDColumn) && Equals (castOther.ClassIDColumn, ClassIDColumn);
    }

    public override bool Equals (object obj)
    {
      return Equals (obj as IRdbmsStoragePropertyDefinition);
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (ObjectIDColumn, ClassIDColumn);
    }

    public bool IsNull
    {
      get { return false; }
    }
  }
}