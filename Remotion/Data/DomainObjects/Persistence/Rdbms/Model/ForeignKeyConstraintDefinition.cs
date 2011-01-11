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
  public class ForeignKeyConstraintDefinition : ITableConstraintDefinition
  {
    private readonly string _constraintName;
    private readonly TableDefinition _referencedTable;
    private readonly ReadOnlyCollection<IColumnDefinition> _referencingColumns;
    private readonly ReadOnlyCollection<IColumnDefinition> _referencedColumns;

    public ForeignKeyConstraintDefinition (
        string constraintName,
        TableDefinition referencedTable,
        IEnumerable<IColumnDefinition> referencingColumns,
        IEnumerable<IColumnDefinition> referencedColumns)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("constraintName", constraintName);
      ArgumentUtility.CheckNotNull ("referencedTable", referencedTable);
      ArgumentUtility.CheckNotNull ("referencingColumns", referencingColumns);
      ArgumentUtility.CheckNotNull ("referencedColumns", referencedColumns);

      _constraintName = constraintName;
      _referencedTable = referencedTable;
      _referencingColumns = referencingColumns.ToList().AsReadOnly();
      _referencedColumns = referencedColumns.ToList().AsReadOnly();

      if (_referencingColumns.Count != _referencedColumns.Count)
        throw new ArgumentException ("The referencing and referenced column sets must have the same number of items.", "referencingColumns");
    }

    public string ConstraintName
    {
      get { return _constraintName; }
    }

    public TableDefinition ReferencedTable
    {
      get { return _referencedTable; }
    }

    public ReadOnlyCollection<IColumnDefinition> ReferencingColumns
    {
      get { return _referencingColumns; }
    }

    public ReadOnlyCollection<IColumnDefinition> ReferencedColumns
    {
      get { return _referencedColumns; }
    }

    public void Accept (ITableConstraintDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.VisitForeignKeyConstraintDefinition (this);
    }
  }
}