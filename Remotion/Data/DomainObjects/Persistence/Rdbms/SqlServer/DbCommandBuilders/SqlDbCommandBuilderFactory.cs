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
using System.Linq;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders
{
  /// <summary>
  /// The <see cref="SqlDbCommandBuilderFactory"/> creates SQL Server-specific <see cref="IDbCommandBuilder"/> instances.
  /// </summary>
  public class SqlDbCommandBuilderFactory : IDbCommandBuilderFactory
  {
    private readonly ISqlDialect _sqlDialect;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDbCommandBuilderFactory"/> class.
    /// </summary>
    /// <param name="sqlDialect">The SQL dialect.</param>
    public SqlDbCommandBuilderFactory (ISqlDialect sqlDialect)
    {
      ArgumentUtility.CheckNotNull ("sqlDialect", sqlDialect);
      _sqlDialect = sqlDialect;
    }

    public IDbCommandBuilder CreateForSelect (
        TableDefinition table,
        IEnumerable<ColumnDefinition> selectedColumns,
        IEnumerable<ColumnValue> comparedColumnValues,
        IEnumerable<OrderedColumn> orderedColumns)
    {
      ArgumentUtility.CheckNotNull ("table", table);
      ArgumentUtility.CheckNotNull ("selectedColumns", selectedColumns);
      ArgumentUtility.CheckNotNull ("comparedColumnValues", comparedColumnValues);
      ArgumentUtility.CheckNotNull ("orderedColumns", orderedColumns);

      return new SelectDbCommandBuilder (
          table,
          new SelectedColumnsSpecification (selectedColumns),
          new ComparedColumnsSpecification (comparedColumnValues),
          CreateOrderedColumnsSpecification (orderedColumns),
          _sqlDialect);
    }
    
    public IDbCommandBuilder CreateForSelect (
        TableDefinition table, 
        IEnumerable<ColumnDefinition> selectedColumns, 
        ColumnDefinition comparedColumn, 
        IEnumerable<object> comparedValues)
    {
      ArgumentUtility.CheckNotNull ("table", table);
      ArgumentUtility.CheckNotNull ("selectedColumns", selectedColumns);
      ArgumentUtility.CheckNotNull ("comparedColumn", comparedColumn);
      ArgumentUtility.CheckNotNull ("comparedValues", comparedValues);

      return new SelectDbCommandBuilder (
          table,
          new SelectedColumnsSpecification (selectedColumns),
          new SqlXmlSetComparedColumnSpecification (comparedColumn, comparedValues),
          EmptyOrderedColumnsSpecification.Instance,
          _sqlDialect);
    }

    public IDbCommandBuilder CreateForSelect (
        UnionViewDefinition view,
        IEnumerable<ColumnDefinition> selectedColumns,
        IEnumerable<ColumnValue> comparedColumnValues,
        IEnumerable<OrderedColumn> orderedColumns)
    {
      ArgumentUtility.CheckNotNull ("view", view);
      ArgumentUtility.CheckNotNull ("selectedColumns", selectedColumns);
      ArgumentUtility.CheckNotNull ("comparedColumnValues", comparedColumnValues);
      ArgumentUtility.CheckNotNull ("orderedColumns", orderedColumns);

      return new UnionSelectDbCommandBuilder (
          view,
          new SelectedColumnsSpecification (selectedColumns),
          new ComparedColumnsSpecification (comparedColumnValues),
          CreateOrderedColumnsSpecification (orderedColumns),
          _sqlDialect);
    }

    public IDbCommandBuilder CreateForQuery (string statement, IEnumerable<QueryParameterWithType> parametersWithType)
    {
      ArgumentUtility.CheckNotNull ("statement", statement);
      ArgumentUtility.CheckNotNull ("parametersWithType", parametersWithType);

      return new QueryDbCommandBuilder (statement, parametersWithType, _sqlDialect);
    }

    public IDbCommandBuilder CreateForInsert (TableDefinition tableDefinition, IEnumerable<ColumnValue> insertedColumns)
    {
      ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);
      ArgumentUtility.CheckNotNull ("insertedColumns", insertedColumns);

      return new InsertDbCommandBuilder (tableDefinition, new InsertedColumnsSpecification (insertedColumns), _sqlDialect);
    }

    public IDbCommandBuilder CreateForUpdate (
        TableDefinition tableDefinition,
        IEnumerable<ColumnValue> updatedColumns,
        IEnumerable<ColumnValue> comparedColumnValues)
    {
      ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);
      ArgumentUtility.CheckNotNull ("updatedColumns", updatedColumns);
      ArgumentUtility.CheckNotNull ("comparedColumnValues", comparedColumnValues);

      return new UpdateDbCommandBuilder (
          tableDefinition,
          new UpdatedColumnsSpecification (updatedColumns),
          new ComparedColumnsSpecification (comparedColumnValues),
          _sqlDialect);
    }

    public IDbCommandBuilder CreateForDelete (TableDefinition tableDefinition, IEnumerable<ColumnValue> comparedColumnValues)
    {
      ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);
      ArgumentUtility.CheckNotNull ("comparedColumnValues", comparedColumnValues);

      return new DeleteDbCommandBuilder (tableDefinition, new ComparedColumnsSpecification (comparedColumnValues), _sqlDialect);
    }
    
    private IOrderedColumnsSpecification CreateOrderedColumnsSpecification (IEnumerable<OrderedColumn> orderedColumns)
    {
      return orderedColumns.Any ()
                 ? (IOrderedColumnsSpecification) new OrderedColumnsSpecification (orderedColumns)
                 : EmptyOrderedColumnsSpecification.Instance;
    }
  }
}