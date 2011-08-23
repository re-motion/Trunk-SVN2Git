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
using Remotion.Data.DomainObjects.Persistence.Configuration;
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
    private readonly StorageProviderDefinition _storageProviderDefinition;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDbCommandBuilderFactory"/> class.
    /// </summary>
    /// <param name="sqlDialect">The SQL dialect.</param>
    /// <param name="storageProviderDefinition">The storage provider definition of the relational database.</param>
    public SqlDbCommandBuilderFactory (ISqlDialect sqlDialect, StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("sqlDialect", sqlDialect);
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      _sqlDialect = sqlDialect;
      _storageProviderDefinition = storageProviderDefinition;
    }

    public IDbCommandBuilder CreateForSingleIDLookupFromTable (
        TableDefinition table, IEnumerable<ColumnDefinition> selectedColumns, IEnumerable<ColumnValue> comparedColumns)
    {
      ArgumentUtility.CheckNotNull ("table", table);
      ArgumentUtility.CheckNotNull ("selectedColumns", selectedColumns);
      ArgumentUtility.CheckNotNull ("comparedColumns", comparedColumns);

      return new SelectDbCommandBuilder (
          table,
          new SelectedColumnsSpecification (selectedColumns),
          new ComparedColumnsSpecification (comparedColumns),
          EmptyOrderedColumnsSpecification.Instance,
          _sqlDialect);
    }

    public IDbCommandBuilder CreateForMultiIDLookupFromTable (
        TableDefinition table, IEnumerable<ColumnDefinition> selectedColumns, ObjectID[] objectIDs)
    {
      ArgumentUtility.CheckNotNull ("table", table);
      ArgumentUtility.CheckNotNull ("selectedColumns", selectedColumns);
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

      return new SelectDbCommandBuilder (
          table,
          new SelectedColumnsSpecification (selectedColumns),
          new SqlXmlSetComparedColumnSpecification (table.IDColumn, GetObjectIDValues(objectIDs)),
          EmptyOrderedColumnsSpecification.Instance,
          _sqlDialect);
    }

    public IDbCommandBuilder CreateForRelationLookupFromTable (
        TableDefinition table,
        IEnumerable<ColumnDefinition> selectedColumns,
        IEnumerable<ColumnValue> comparedColumns,
        IEnumerable<OrderedColumn> orderedColumns)
    {
      ArgumentUtility.CheckNotNull ("table", table);
      ArgumentUtility.CheckNotNull ("selectedColumns", selectedColumns);
      ArgumentUtility.CheckNotNull ("comparedColumns", comparedColumns);
      ArgumentUtility.CheckNotNull ("orderedColumns", orderedColumns);

      return new SelectDbCommandBuilder (
          table,
          new SelectedColumnsSpecification (selectedColumns),
          new ComparedColumnsSpecification (comparedColumns),
          CreateOrderedColumnsSpecification (orderedColumns),
          _sqlDialect);
    }

    public IDbCommandBuilder CreateForRelationLookupFromUnionView (
        UnionViewDefinition view,
        IEnumerable<ColumnDefinition> selectedColumns,
        IEnumerable<ColumnValue> comparedColumns,
        IEnumerable<OrderedColumn> orderedColumns)
    {
      ArgumentUtility.CheckNotNull ("view", view);
      ArgumentUtility.CheckNotNull ("selectedColumns", selectedColumns);
      ArgumentUtility.CheckNotNull ("comparedColumns", comparedColumns);
      ArgumentUtility.CheckNotNull ("orderedColumns", orderedColumns);

      var orderedColumnsSpecification = CreateOrderedColumnsSpecification (orderedColumns);
      
      return new UnionSelectDbCommandBuilder (
          view,
          new SelectedColumnsSpecification (selectedColumns),
          new ComparedColumnsSpecification (comparedColumns),
          orderedColumnsSpecification,
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
        IEnumerable<ColumnValue> comparedColumns)
    {
      ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);
      ArgumentUtility.CheckNotNull ("updatedColumns", updatedColumns);
      ArgumentUtility.CheckNotNull ("comparedColumns", comparedColumns);

      return new UpdateDbCommandBuilder (
          tableDefinition,
          new UpdatedColumnsSpecification (updatedColumns),
          new ComparedColumnsSpecification (comparedColumns),
          _sqlDialect);
    }

    public IDbCommandBuilder CreateForDelete (TableDefinition tableDefinition, IEnumerable<ColumnValue> comparedColumns)
    {
      ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);
      ArgumentUtility.CheckNotNull ("comparedColumns", comparedColumns);

      return new DeleteDbCommandBuilder (tableDefinition, new ComparedColumnsSpecification (comparedColumns), _sqlDialect);
    }

    
    private IEnumerable<object > GetObjectIDValues (IEnumerable<ObjectID> objectIDs)
    {
      foreach (var t in objectIDs)
      {
        if (t.StorageProviderDefinition!=_storageProviderDefinition)
          throw new ArgumentException ("Multi-ID lookups can only be performed for ObjectIDs from this storage provider.", "objectIDs");
        yield return t.Value;
      }
    }

    private IOrderedColumnsSpecification CreateOrderedColumnsSpecification (IEnumerable<OrderedColumn> orderedColumns)
    {
      return orderedColumns.Any ()
                 ? (IOrderedColumnsSpecification) new OrderedColumnsSpecification (orderedColumns)
                 : EmptyOrderedColumnsSpecification.Instance;
    }
  }
}