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
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.DataReaders;
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
      var dbCommandBuilder = _dbCommandBuilderFactory.CreateForSingleIDLookupFromTable (
          table,
          AllSelectedColumnsSpecification.Instance,
          objectID);
      // TODO Review 4113: Use DelegateBasedStorageProviderCommand to create a lookup result from objectID and the loaded DataContainer (requires RM-4146)
      return new SingleDataContainerLoadCommand (dbCommandBuilder, _dataContainerReader);
    }

    public IStorageProviderCommand<IEnumerable<DataContainerLookupResult>, IRdbmsProviderCommandExecutionContext> CreateForMultiIDLookup (
        IEnumerable<ObjectID> objectIDs)
    {
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

      var dbCommandBuilders = from id in objectIDs
                              let tableDefinition = GetTableDefinition (id)
                              group id by tableDefinition
                              into idsByTable
                              select CreateIDLookupDbCommandBuilder (idsByTable.Key, idsByTable.ToArray());
      var multiDataContainerLoadCommand = new MultiDataContainerLoadCommand (dbCommandBuilders, false, _dataContainerReader);
      return new MultiDataContainerSortCommand (objectIDs, multiDataContainerLoadCommand);
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

      return new MultiDataContainerLoadCommand (new[] { _dbCommandBuilderFactory.CreateForQuery (query) }, true, _dataContainerReader);
    }

    public IStorageProviderCommand<IRdbmsProviderCommandExecutionContext> CreateForSave (DataContainer[] dataContainers)
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

    private IDbCommandBuilder CreateIDLookupDbCommandBuilder (TableDefinition tableDefinition, ObjectID[] objectIDs)
    {
      if (objectIDs.Length > 1)
        return _dbCommandBuilderFactory.CreateForMultiIDLookupFromTable (tableDefinition, AllSelectedColumnsSpecification.Instance, objectIDs);
      else
        return _dbCommandBuilderFactory.CreateForSingleIDLookupFromTable (tableDefinition, AllSelectedColumnsSpecification.Instance, objectIDs[0]);
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
      return new MultiDataContainerLoadCommand (new[] { dbCommandBuilder }, false, _dataContainerReader);
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
      return
          new SelectStorageProviderCommand<DataContainerLookupResult, DataContainer, IRdbmsProviderCommandExecutionContext>
              (
              indirectDataContainerLoadCommand,
              result =>
              {
                Assertion.IsNotNull (
                    result.LocatedDataContainer,
                    "Because ID lookup and DataContainer lookup are executed within the same database transaction, the DataContainer can never be null.");
                return result.LocatedDataContainer;
              });
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
  }
}