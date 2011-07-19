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
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  /// <summary>
  /// Creates <see cref="IStorageProviderCommand{TExecutionContext}"/> instances for use with <see cref="RdbmsProvider"/>.
  /// </summary>
  public class RdbmsProviderCommandFactory : IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext>
  {
    private readonly IDbCommandBuilderFactory _dbCommandBuilderFactory;
    private readonly IDataContainerReader _dataContainerReader;
    private readonly IObjectIDReader _objectIDReader;
    private readonly IRdbmsPersistenceModelProvider _rdbmsPersistenceModelProvider;

    public RdbmsProviderCommandFactory (
        IDbCommandBuilderFactory dbCommandBuilderFactory,
        IDataContainerReader dataContainerReader,
        IObjectIDReader objectIDReader,
        IRdbmsPersistenceModelProvider rdbmsPersistenceModelProvider)
    {
      ArgumentUtility.CheckNotNull ("dbCommandBuilderFactory", dbCommandBuilderFactory);
      ArgumentUtility.CheckNotNull ("dataContainerReader", dataContainerReader);
      ArgumentUtility.CheckNotNull ("objectIDReader", objectIDReader);
      ArgumentUtility.CheckNotNull ("rdbmsPersistenceModelProvider", rdbmsPersistenceModelProvider);

      _dbCommandBuilderFactory = dbCommandBuilderFactory;
      _dataContainerReader = dataContainerReader;
      _objectIDReader = objectIDReader;
      _rdbmsPersistenceModelProvider = rdbmsPersistenceModelProvider;
    }

    public IStorageProviderCommand<DataContainerLookupResult, IRdbmsProviderCommandExecutionContext> CreateForSingleIDLookup (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      var table = GetTableDefinition (objectID);
      var selectProjection = table.GetAllColumns();
      var columnOrdinalProvider = CreateOrdinalProviderForKnownProjection (selectProjection);
      var dataContainerReader = CreateDataContainerReader (table, columnOrdinalProvider);
      var dbCommandBuilder = _dbCommandBuilderFactory.CreateForSingleIDLookupFromTable (
          table, new SelectedColumnsSpecification (selectProjection), objectID);
      var singleDataContainerLoadCommand = new SingleDataContainerLoadCommand (dbCommandBuilder, dataContainerReader);
      return DelegateBasedStorageProviderCommand.Create (singleDataContainerLoadCommand, result => new DataContainerLookupResult (objectID, result));
    }

    public IStorageProviderCommand<IEnumerable<DataContainerLookupResult>, IRdbmsProviderCommandExecutionContext> CreateForMultiIDLookup (
        IEnumerable<ObjectID> objectIDs)
    {
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

      var objectIDList = objectIDs.ToList();
      var dbCommandBuilders = from id in objectIDList
                              let tableDefinition = GetTableDefinition (id)
                              group id by tableDefinition
                              into idsByTable
                              select CreateIDLookupDbCommandBuilder (idsByTable.Key, idsByTable.ToArray());
      var multiDataContainerLoadCommand = new MultiDataContainerLoadCommand (dbCommandBuilders, false);
      return new MultiDataContainerSortCommand (objectIDList, multiDataContainerLoadCommand);
    }

    public IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext> CreateForRelationLookup (
        RelationEndPointDefinition foreignKeyEndPoint, ObjectID foreignKeyValue, SortExpressionDefinition sortExpressionDefinition)
    {
      ArgumentUtility.CheckNotNull ("foreignKeyEndPoint", foreignKeyEndPoint);
      ArgumentUtility.CheckNotNull ("foreignKeyValue", foreignKeyValue);

      return InlineEntityDefinitionVisitor.Visit<IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>> (
          _rdbmsPersistenceModelProvider.GetEntityDefinition (foreignKeyEndPoint.ClassDefinition),
          (table, continuation) => CreateForDirectRelationLookup (table, foreignKeyEndPoint, foreignKeyValue, sortExpressionDefinition),
          (filterView, continuation) => continuation (filterView.BaseEntity),
          (unionView, continuation) => CreateForIndirectRelationLookup (unionView, foreignKeyEndPoint, foreignKeyValue, sortExpressionDefinition),
          (nullEntity, continuation) => CreateForNullRelationLookup());
    }

    public IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext> CreateForDataContainerQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      return new MultiDataContainerLoadCommand (new[] { Tuple.Create (_dbCommandBuilderFactory.CreateForQuery (query), _dataContainerReader) }, true);
    }

    public IStorageProviderCommand<IRdbmsProviderCommandExecutionContext> CreateForSave (IEnumerable<DataContainer> dataContainers)
    {
      ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

      return new MultiDataContainerSaveCommand (CreateDbCommandsForSave (dataContainers));
    }

    private IEnumerable<Tuple<ObjectID, IDbCommandBuilder>> CreateDbCommandsForSave (IEnumerable<DataContainer> dataContainers)
    {
      var dataContainersByState = dataContainers.ToLookup (dc => dc.State);

      foreach (var dataContainer in dataContainersByState[StateType.New])
        yield return Tuple.Create (dataContainer.ID, _dbCommandBuilderFactory.CreateForInsert (dataContainer));

      var changedContainers =
          dataContainersByState[StateType.New].Concat (dataContainersByState[StateType.Changed]).Concat (dataContainersByState[StateType.Deleted]);
      foreach (var dataContainer in changedContainers)
        yield return Tuple.Create (dataContainer.ID, _dbCommandBuilderFactory.CreateForUpdate (dataContainer));

      foreach (var dataContainer in dataContainersByState[StateType.Deleted])
        yield return Tuple.Create (dataContainer.ID, _dbCommandBuilderFactory.CreateForDelete (dataContainer));
    }

    private TableDefinition GetTableDefinition (ObjectID objectID)
    {
      return InlineEntityDefinitionVisitor.Visit<TableDefinition> (
          _rdbmsPersistenceModelProvider.GetEntityDefinition (objectID.ClassDefinition),
          (table, continuation) => table,
          (filterView, continuation) => continuation (filterView.BaseEntity),
          (unionView, continuation) => { throw new InvalidOperationException ("An ObjectID's EntityDefinition cannot be a UnionViewDefinition."); },
          (nullEntity, continuation) => { throw new InvalidOperationException ("An ObjectID's EntityDefinition cannot be a NullEntityDefinition."); });
    }

    private Tuple<IDbCommandBuilder, IDataContainerReader> CreateIDLookupDbCommandBuilder (TableDefinition tableDefinition, ObjectID[] objectIDs)
    {
      if (objectIDs.Length > 1)
      {
        return
            Tuple.Create (
                _dbCommandBuilderFactory.CreateForMultiIDLookupFromTable (tableDefinition, AllSelectedColumnsSpecification.Instance, objectIDs),
                _dataContainerReader);
      }
      else
      {
        return
            Tuple.Create (
                _dbCommandBuilderFactory.CreateForSingleIDLookupFromTable (tableDefinition, AllSelectedColumnsSpecification.Instance, objectIDs[0]),
                _dataContainerReader);
      }
    }

    private IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext> CreateForDirectRelationLookup (
        TableDefinition tableDefinition,
        RelationEndPointDefinition foreignKeyEndPoint,
        ObjectID foreignKeyValue,
        SortExpressionDefinition sortExpression)
    {
      var dbCommandBuilder = _dbCommandBuilderFactory.CreateForRelationLookupFromTable (
          tableDefinition,
          AllSelectedColumnsSpecification.Instance,
          _rdbmsPersistenceModelProvider.GetIDColumnDefinition (foreignKeyEndPoint),
          foreignKeyValue,
          GetOrderedColumns (sortExpression));
      return new MultiDataContainerLoadCommand (new[] { Tuple.Create(dbCommandBuilder, _dataContainerReader) }, false);
    }

    private IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext> CreateForIndirectRelationLookup (
        UnionViewDefinition unionViewDefinition,
        RelationEndPointDefinition foreignKeyEndPoint,
        ObjectID foreignKeyValue,
        SortExpressionDefinition sortExpression)
    {
      var dbCommandBuilder = _dbCommandBuilderFactory.CreateForRelationLookupFromUnionView (
          unionViewDefinition,
          new SelectedColumnsSpecification (new[] { unionViewDefinition.ObjectIDColumn, unionViewDefinition.ClassIDColumn }),
          _rdbmsPersistenceModelProvider.GetIDColumnDefinition (foreignKeyEndPoint),
          foreignKeyValue,
          GetOrderedColumns (sortExpression));

      var objectIDLoadCommand = new MultiObjectIDLoadCommand (new[] { dbCommandBuilder }, _objectIDReader);
      var indirectDataContainerLoadCommand = new IndirectDataContainerLoadCommand (objectIDLoadCommand, this);
      return DelegateBasedStorageProviderCommand.Create (
          indirectDataContainerLoadCommand,
          lookupResults => lookupResults.Select (
              result =>
              {
                Assertion.IsNotNull (
                    result.LocatedDataContainer,
                    "Because ID lookup and DataContainer lookup are executed within the same database transaction, the DataContainer can never be null.");
                return result.LocatedDataContainer;
              }));
    }

    private FixedValueStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext> CreateForNullRelationLookup ()
    {
      return
          new FixedValueStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext> (Enumerable.Empty<DataContainer>());
    }

    private IOrderedColumnsSpecification GetOrderedColumns (SortExpressionDefinition sortExpression)
    {
      if (sortExpression == null)
        return EmptyOrderedColumnsSpecification.Instance;

      Assertion.IsTrue (sortExpression.SortedProperties.Count > 0, "The sort-epression must have at least one sorted property.");

      var columns = from spec in sortExpression.SortedProperties
                    let column = _rdbmsPersistenceModelProvider.GetColumnDefinition (spec.PropertyDefinition)
                    from simpleColumn in column.GetColumns()
                    select Tuple.Create (simpleColumn, spec.Order);

      return new OrderedColumnsSpecification (columns);
    }

    private IDataContainerReader CreateDataContainerReader (TableDefinition tableDefinition, IColumnOrdinalProvider columnOrdinalProvider)
    {
      var objectIDStoragePropertyDefinition =
          new ObjectIDStoragePropertyDefinition (
              new SimpleStoragePropertyDefinition (tableDefinition.ObjectIDColumn),
              new SimpleStoragePropertyDefinition (tableDefinition.ClassIDColumn));
      var timestampPropertyDefinition = new SimpleStoragePropertyDefinition (tableDefinition.TimestampColumn);

      return new DataContainerReader (
          objectIDStoragePropertyDefinition, timestampPropertyDefinition, columnOrdinalProvider, _rdbmsPersistenceModelProvider);
    }

    private IColumnOrdinalProvider CreateOrdinalProviderForKnownProjection (IEnumerable<ColumnDefinition> selectedColumns)
    {
      var columnOrdinalsDictionary = selectedColumns.Select ((column, index) => new { column, index }).ToDictionary (t => t.column, t => t.index);
      return new DictionaryBasedColumnOrdinalProvider (columnOrdinalsDictionary);
    }
  }
}