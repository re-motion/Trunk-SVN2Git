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
using System.Collections.Generic;
using System.Linq;
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  /// <summary>
  /// The <see cref="LookupCommandFactory"/> is responsible to reate lookup commands for a relational database.
  /// </summary>
  public class LookupCommandFactory
  {
    private readonly IDbCommandBuilderFactory _dbCommandBuilderFactory;
    private readonly IRdbmsPersistenceModelProvider _rdbmsPersistenceModelProvider;
    private readonly IInfrastructureStoragePropertyDefinitionProvider _infrastructureStoragePropertyDefinitionProvider;
    private readonly IObjectReaderFactory _objectReaderFactory;
    private readonly ITableDefinitionFinder _tableDefinitionFinder;
    private readonly IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> _storageProviderCommandFactory;

    public LookupCommandFactory (
        IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> storageProviderCommandFactory,
        IDbCommandBuilderFactory dbCommandBuilderFactory,
        IRdbmsPersistenceModelProvider rdbmsPersistenceModelProvider,
        IInfrastructureStoragePropertyDefinitionProvider infrastructureStoragePropertyDefinitionProvider,
        IObjectReaderFactory objectReaderFactory,
        ITableDefinitionFinder tableDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull ("storageProviderCommandFactory", storageProviderCommandFactory);
      ArgumentUtility.CheckNotNull ("dbCommandBuilderFactory", dbCommandBuilderFactory);
      ArgumentUtility.CheckNotNull ("rdbmsPersistenceModelProvider", rdbmsPersistenceModelProvider);
      ArgumentUtility.CheckNotNull ("infrastructureStoragePropertyDefinitionProvider", infrastructureStoragePropertyDefinitionProvider);
      ArgumentUtility.CheckNotNull ("objectReaderFactory", objectReaderFactory);
      ArgumentUtility.CheckNotNull ("tableDefinitionFinder", tableDefinitionFinder);

      _storageProviderCommandFactory = storageProviderCommandFactory;
      _dbCommandBuilderFactory = dbCommandBuilderFactory;
      _rdbmsPersistenceModelProvider = rdbmsPersistenceModelProvider;
      _infrastructureStoragePropertyDefinitionProvider = infrastructureStoragePropertyDefinitionProvider;
      _objectReaderFactory = objectReaderFactory;
      _tableDefinitionFinder = tableDefinitionFinder;
    }

    public IStorageProviderCommand<ObjectLookupResult<DataContainer>, IRdbmsProviderCommandExecutionContext> CreateForSingleIDLookup (
        ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      var tableDefinition = _tableDefinitionFinder.GetTableDefinition (objectID);
      var selectedColumns = tableDefinition.GetAllColumns ().ToArray ();
      var dataContainerReader = _objectReaderFactory.CreateDataContainerReader (tableDefinition, selectedColumns);
      var dbCommandBuilder = _dbCommandBuilderFactory.CreateForSingleIDLookupFromTable (tableDefinition, selectedColumns, objectID);

      var singleDataContainerLoadCommand = new SingleObjectLoadCommand<DataContainer> (dbCommandBuilder, dataContainerReader);
      return DelegateBasedStorageProviderCommand.Create (
          singleDataContainerLoadCommand, result => new ObjectLookupResult<DataContainer> (objectID, result));
    }

    public IStorageProviderCommand<IEnumerable<ObjectLookupResult<DataContainer>>, IRdbmsProviderCommandExecutionContext> CreateForSortedMultiIDLookup (
        IEnumerable<ObjectID> objectIDs)
    {
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

      var objectIDList = objectIDs.ToList ();
      var dbCommandBuildersAndReaders =
          from id in objectIDList
          let tableDefinition = _tableDefinitionFinder.GetTableDefinition (id)
          group id by tableDefinition
            into idsByTable
            let selectedColumns = idsByTable.Key.GetAllColumns ().ToArray ()
            let dataContainerReader = _objectReaderFactory.CreateDataContainerReader (idsByTable.Key, selectedColumns)
            let dbCommandBuilder = CreateIDLookupDbCommandBuilder (idsByTable.Key, selectedColumns, idsByTable.ToArray ())
            select Tuple.Create (dbCommandBuilder, dataContainerReader);

      var loadCommand = new MultiObjectLoadCommand<DataContainer> (dbCommandBuildersAndReaders);
      return new MultiDataContainerSortCommand (objectIDList, loadCommand);
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
          (nullEntity, continuation) => CreateForNullRelationLookup ());
    }

    public IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext> CreateForDataContainerQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      // TODO Review 4207: Move to ObjectReaderFactory.
      var ordinalProvider = new NameBasedColumnOrdinalProvider ();
      var objectIDStoragePropertyDefinition = new ObjectIDStoragePropertyDefinition (
          new SimpleStoragePropertyDefinition (_infrastructureStoragePropertyDefinitionProvider.GetIDColumnDefinition ()),
          new SimpleStoragePropertyDefinition (_infrastructureStoragePropertyDefinitionProvider.GetClassIDColumnDefinition ()));
      var timestampPropertyDefinition =
          new SimpleStoragePropertyDefinition (_infrastructureStoragePropertyDefinitionProvider.GetTimestampColumnDefinition ());
      IObjectReader<DataContainer> dataContainerReader = new DataContainerReader (
          objectIDStoragePropertyDefinition, timestampPropertyDefinition, ordinalProvider, _rdbmsPersistenceModelProvider);

      return new MultiObjectLoadCommand<DataContainer> (new[] { Tuple.Create (_dbCommandBuilderFactory.CreateForQuery (query), dataContainerReader) });
    }

    public IStorageProviderCommand<IEnumerable<ObjectLookupResult<object>>, IRdbmsProviderCommandExecutionContext> CreateForMultiTimestampLookup (
        IEnumerable<ObjectID> objectIDs)
    {
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

      var dbCommandBuildersAndReaders =
          from id in objectIDs
          let tableDefinition = _tableDefinitionFinder.GetTableDefinition (id)
          group id by tableDefinition
            into idsByTable
            let selectedColumns = new[] { idsByTable.Key.IDColumn, idsByTable.Key.ClassIDColumn, idsByTable.Key.TimestampColumn }
            let timestampReader = _objectReaderFactory.CreateTimestampReader (idsByTable.Key, selectedColumns)
            let dbCommandBuilder = CreateIDLookupDbCommandBuilder (idsByTable.Key, selectedColumns, idsByTable.ToArray ())
            select Tuple.Create (dbCommandBuilder, timestampReader);

      var loadCommand = new MultiObjectLoadCommand<Tuple<ObjectID, object>> (dbCommandBuildersAndReaders);
      return DelegateBasedStorageProviderCommand.Create (
          loadCommand,
          lookupResults => lookupResults.Select (
              result =>
              {
                Assertion.IsNotNull (
                    result,
                    "Because we included IDColumn into the projection and used it for the lookup, every row in the result set certainly has an ID.");
                return new ObjectLookupResult<object> (result.Item1, result.Item2);
              }));
    }

    private IDbCommandBuilder CreateIDLookupDbCommandBuilder (
        TableDefinition tableDefinition,
        IEnumerable<ColumnDefinition> selectedColumns,
        ObjectID[] objectIDs)
    {
      if (objectIDs.Length > 1)
      {
        return _dbCommandBuilderFactory.CreateForMultiIDLookupFromTable (tableDefinition, selectedColumns, objectIDs);
      }
      else
      {
        return _dbCommandBuilderFactory.CreateForSingleIDLookupFromTable (tableDefinition, selectedColumns, objectIDs[0]);
      }
    }

    private IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext> CreateForDirectRelationLookup (
        TableDefinition tableDefinition,
        RelationEndPointDefinition foreignKeyEndPoint,
        ObjectID foreignKeyValue,
        SortExpressionDefinition sortExpression)
    {
      var selectProjection = tableDefinition.GetAllColumns ();
      var dataContainerReader = _objectReaderFactory.CreateDataContainerReader (tableDefinition, selectProjection);

      var dbCommandBuilder = _dbCommandBuilderFactory.CreateForRelationLookupFromTable (
          tableDefinition,
          selectProjection,
          _rdbmsPersistenceModelProvider.GetStoragePropertyDefinition (foreignKeyEndPoint.PropertyDefinition),
          foreignKeyValue,
          GetOrderedColumnsFromSortExpression (sortExpression));
      return new MultiObjectLoadCommand<DataContainer> (new[] { Tuple.Create (dbCommandBuilder, dataContainerReader) });
    }

    private IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext> CreateForIndirectRelationLookup (
        UnionViewDefinition unionViewDefinition,
        RelationEndPointDefinition foreignKeyEndPoint,
        ObjectID foreignKeyValue,
        SortExpressionDefinition sortExpression)
    {
      var selectedColumns = new[] { unionViewDefinition.IDColumn, unionViewDefinition.ClassIDColumn };
      var dbCommandBuilder = _dbCommandBuilderFactory.CreateForRelationLookupFromUnionView (
          unionViewDefinition,
          selectedColumns,
          _rdbmsPersistenceModelProvider.GetStoragePropertyDefinition (foreignKeyEndPoint.PropertyDefinition),
          foreignKeyValue,
          GetOrderedColumnsFromSortExpression (sortExpression));

      var objectIDReader = _objectReaderFactory.CreateObjectIDReader (unionViewDefinition, selectedColumns);

      var objectIDLoadCommand = new MultiObjectIDLoadCommand (new[] { dbCommandBuilder }, objectIDReader);
      var indirectDataContainerLoadCommand = new IndirectDataContainerLoadCommand (objectIDLoadCommand, _storageProviderCommandFactory);
      return DelegateBasedStorageProviderCommand.Create (
          indirectDataContainerLoadCommand,
          lookupResults => lookupResults.Select (
              result =>
              {
                Assertion.IsNotNull (
                    result.LocatedObject,
                    "Because ID lookup and DataContainer lookup are executed within the same database transaction, the DataContainer can never be null.");
                return result.LocatedObject;
              }));
    }

    private FixedValueStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext> CreateForNullRelationLookup ()
    {
      return
          new FixedValueStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext> (Enumerable.Empty<DataContainer>());
    }

    private IEnumerable<OrderedColumn> GetOrderedColumnsFromSortExpression (SortExpressionDefinition sortExpression)
    {
      if (sortExpression == null)
        return new OrderedColumn[0];

      Assertion.IsTrue (sortExpression.SortedProperties.Count > 0, "The sort-epression must have at least one sorted property.");

      return from sortedProperty in sortExpression.SortedProperties
             let storagePropertyDefinition = _rdbmsPersistenceModelProvider.GetStoragePropertyDefinition (sortedProperty.PropertyDefinition)
             from column in storagePropertyDefinition.GetColumns()
             select new OrderedColumn (column, sortedProperty.Order);
    }
  }
}