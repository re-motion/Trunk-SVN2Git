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
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// The <see cref="IDColumnDefinition"/> represents an <see cref="ObjectID"/>-column with a class id.
  /// </summary>
  public class IDColumnDefinition : IColumnDefinition
  {
    private readonly IColumnDefinition _objectIDColumn;
    private readonly IColumnDefinition _classIDColumn;

    public IDColumnDefinition (IColumnDefinition objectIDColumn, IColumnDefinition classIDColumn)
    {
      ArgumentUtility.CheckNotNull ("objectIDColumn", objectIDColumn);

      _objectIDColumn = objectIDColumn;
      _classIDColumn = classIDColumn;
    }

    string IStoragePropertyDefinition.Name
    {
      get { return _objectIDColumn.Name; }
    }

    public IColumnDefinition ObjectIDColumn
    {
      get { return _objectIDColumn; }
    }

    public IColumnDefinition ClassIDColumn
    {
      get { return _classIDColumn; }
    }

    public bool HasClassIDColumn
    {
      get { return _classIDColumn != null; }
    }

    public void Accept (IColumnDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.VisitIDColumnDefinition (this);
    }

    public bool Equals (IColumnDefinition other)
    {
      if (other == null || other.GetType() != GetType())
        return false;

      var castOther = (IDColumnDefinition) other;
      return Equals (castOther.ObjectIDColumn, ObjectIDColumn) && Equals (castOther.ClassIDColumn, ClassIDColumn);
    }

    public override bool Equals (object obj)
    {
      return Equals (obj as IColumnDefinition);
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