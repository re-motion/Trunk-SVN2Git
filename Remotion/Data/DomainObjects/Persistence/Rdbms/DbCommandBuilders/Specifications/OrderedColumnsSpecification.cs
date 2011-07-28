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
using System.Text;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Text;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications
{
  /// <summary>
  /// The <see cref="OrderedColumnsSpecification"/> defines that the selected data should be ordered by the given columns.
  /// </summary>
  public class OrderedColumnsSpecification : IOrderedColumnsSpecification
  {
    private readonly Tuple<ColumnDefinition, SortOrder>[] _columns;

    public OrderedColumnsSpecification (IEnumerable<Tuple<ColumnDefinition, SortOrder>> columns)
    {
      ArgumentUtility.CheckNotNull ("columns", columns);

      _columns = columns.ToArray();
    }

    public ReadOnlyCollection<Tuple<ColumnDefinition, SortOrder>> Columns
    {
      get { return Array.AsReadOnly(_columns); }
    }

    public bool IsEmpty
    {
      get { return false; }
    }

    public void AppendOrderings (StringBuilder stringBuilder, ISqlDialect sqlDialect)
    {
      ArgumentUtility.CheckNotNull ("stringBuilder", stringBuilder);
      ArgumentUtility.CheckNotNull ("sqlDialect", sqlDialect);

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