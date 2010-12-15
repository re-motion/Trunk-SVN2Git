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
  /// The <see cref="ObjectIDWithClassIDColumnDefinition"/> represents an <see cref="ObjectID"/>-column with a class id.
  /// </summary>
  public class ObjectIDWithClassIDColumnDefinition : IColumnDefinition
  {
    private readonly SimpleColumnDefinition _objectIDColumn;
    private readonly SimpleColumnDefinition _classIDColumn;

    public ObjectIDWithClassIDColumnDefinition (SimpleColumnDefinition objectIDColumn, SimpleColumnDefinition classIDColumn)
    {
      ArgumentUtility.CheckNotNull ("objectIDColumn", objectIDColumn);
      ArgumentUtility.CheckNotNull ("classIDColumn", classIDColumn);

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

    public void Accept (IColumnDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.VisitObjectIDWithClassIDColumnDefinition (this);
    }
  }
}