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
using System.Text;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Text;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders
{
  /// <summary>
  /// The <see cref="NonEmptyOrderedColumnsSpecification"/> defines that the selected data should be ordered by the given columns.
  /// </summary>
  // TODO Review 4061: Rename to OrderedColumnsSpecification
  public class NonEmptyOrderedColumnsSpecification : IOrderedColumnsSpecification
  {
    // TODO Review 4061: Save as array
    private readonly IEnumerable<Tuple<SimpleColumnDefinition, SortOrder>> _columns;

    public NonEmptyOrderedColumnsSpecification (IEnumerable<Tuple<SimpleColumnDefinition, SortOrder>> columns)
    {
      ArgumentUtility.CheckNotNull ("columns", columns);

      _columns = columns;
    }

    // TODO Review 4061: ReadOnlyCollection
    public IEnumerable<Tuple<SimpleColumnDefinition, SortOrder>> Columns
    {
      get { return _columns; }
    }

    public void AppendOrderByClause (StringBuilder stringBuilder, ISqlDialect sqlDialect)
    {
      ArgumentUtility.CheckNotNull ("stringBuilder", stringBuilder);
      ArgumentUtility.CheckNotNull ("sqlDialect", sqlDialect);

      stringBuilder.Append (" ORDER BY ");
      stringBuilder.Append (
          SeparatedStringBuilder.Build (
              ", ", 
              _columns, tuple => sqlDialect.DelimitIdentifier (tuple.Item1.Name) + (tuple.Item2 == SortOrder.Ascending ? " ASC" : " DESC")));
    }

    public ISelectedColumnsSpecification UnionWithSelectedColumns (ISelectedColumnsSpecification selectedColumns)
    {
      ArgumentUtility.CheckNotNull ("selectedColumns", selectedColumns);

      return selectedColumns.Union (_columns.Select (tuple => tuple.Item1));
    }
  }
}