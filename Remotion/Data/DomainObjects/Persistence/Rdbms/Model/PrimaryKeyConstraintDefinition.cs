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
using System.Collections.ObjectModel;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// The <see cref="PrimaryKeyConstraintDefinition"/> represents a primary key constraint in a relational database management system.
  /// </summary>
  public class PrimaryKeyConstraintDefinition : ITableConstraintDefinition
  {
    private readonly string _constraintName;
    private readonly bool _isClustered;
    private readonly ReadOnlyCollection<SimpleColumnDefinition> _columns;

    public PrimaryKeyConstraintDefinition (string constraintName, bool isClustered, IEnumerable<SimpleColumnDefinition> columns)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("constraintName", constraintName);
      ArgumentUtility.CheckNotNull ("columns", columns);

      _constraintName = constraintName;
      _isClustered = isClustered;
      _columns = columns.ToList().AsReadOnly();
    }

    public string ConstraintName
    {
      get { return _constraintName; }
    }

    public bool IsClustered
    {
      get { return _isClustered; }
    }

    public ReadOnlyCollection<SimpleColumnDefinition> Columns
    {
      get { return _columns; }
    }

    public void Accept (ITableConstraintDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.VisitPrimaryKeyConstraintDefinition (this);
    }
  }
}