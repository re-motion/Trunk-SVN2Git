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
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories
{
  /// <summary>
  /// The <see cref="LookupCommandFactory"/> is responsible for creating lookup commands for a relational database.
  /// </summary>
  public class LookupCommandFactory
  {
    private readonly StorageProviderDefinition _storageProviderDefinition;
    private readonly IDbCommandBuilderFactory _dbCommandBuilderFactory;
    private readonly IObjectReaderFactory _objectReaderFactory;
    private readonly ITableDefinitionFinder _tableDefinitionFinder;

    public LookupCommandFactory (
        StorageProviderDefinition storageProviderDefinition,
        IDbCommandBuilderFactory dbCommandBuilderFactory,
        IObjectReaderFactory objectReaderFactory,
        ITableDefinitionFinder tableDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("dbCommandBuilderFactory", dbCommandBuilderFactory);
      ArgumentUtility.CheckNotNull ("objectReaderFactory", objectReaderFactory);
      ArgumentUtility.CheckNotNull ("tableDefinitionFinder", tableDefinitionFinder);

      _storageProviderDefinition = storageProviderDefinition;
      _dbCommandBuilderFactory = dbCommandBuilderFactory;
      _objectReaderFactory = objectReaderFactory;
      _tableDefinitionFinder = tableDefinitionFinder;
    }

    public IStorageProviderCommand<ObjectLookupResult<DataContainer>, IRdbmsProviderCommandExecutionContext> CreateForSingleIDLookup (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      var tableDefinition = _tableDefinitionFinder.GetTableDefinition (objectID);
      var selectedColumns = tableDefinition.GetAllColumns().ToArray();
      var dataContainerReader = _objectReaderFactory.CreateDataContainerReader (tableDefinition, selectedColumns);
      // TODO 4231: Use TableDefinition.ObjectIDProperty.SplitValueForComparison (objectID)
      var comparedColumns = new[] { new ColumnValue (tableDefinition.IDColumn, objectID.Value) };
      var dbCommandBuilder = _dbCommandBuilderFactory.CreateForSelect (tableDefinition, selectedColumns, comparedColumns, new OrderedColumn[0]);

      var singleDataContainerLoadCommand = new SingleObjectLoadCommand<DataContainer> (dbCommandBuilder, dataContainerReader);
      return DelegateBasedStorageProviderCommand.Create (
          singleDataContainerLoadCommand, result => new ObjectLookupResult<DataContainer> (objectID, result));
    }

    public IStorageProviderCommand<IEnumerable<ObjectLookupResult<DataContainer>>, IRdbmsProviderCommandExecutionContext> CreateForSortedMultiIDLookup (
        IEnumerable<ObjectID> objectIDs)
    {
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

      var objectIDList = objectIDs.ToList();
      var dbCommandBuildersAndReaders =
          from id in objectIDList
          let tableDefinition = _tableDefinitionFinder.GetTableDefinition (id)
          group id by tableDefinition
          into idsByTable
          let selectedColumns = idsByTable.Key.GetAllColumns().ToArray()
          let dataContainerReader = _objectReaderFactory.CreateDataContainerReader (idsByTable.Key, selectedColumns)
          let dbCommandBuilder = CreateIDLookupDbCommandBuilder (idsByTable.Key, selectedColumns, idsByTable)
          select Tuple.Create (dbCommandBuilder, dataContainerReader);

      var loadCommand = new MultiObjectLoadCommand<DataContainer> (dbCommandBuildersAndReaders);
      return new MultiDataContainerSortCommand (objectIDList, loadCommand);
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
          let dbCommandBuilder = CreateIDLookupDbCommandBuilder (idsByTable.Key, selectedColumns, idsByTable)
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
        IEnumerable<ObjectID> objectIDs)
    {
      var checkedIDValues = GetAndCheckObjectIDValues(objectIDs).ToArray();
      if (checkedIDValues.Length > 1)
      {
        // TODO 4231: Should we rewrite the multi-select so that it can work with multiple columns?
        return _dbCommandBuilderFactory.CreateForSelect (tableDefinition, selectedColumns, tableDefinition.IDColumn, checkedIDValues);
      }
      else
      {
        // TODO 4231: Use TableDefinition.ObjectIDProperty.SplitValueForComparison (objectID)
        var comparedColumns = new[] { new ColumnValue (tableDefinition.IDColumn, checkedIDValues[0]) };
        return _dbCommandBuilderFactory.CreateForSelect (tableDefinition, selectedColumns, comparedColumns, new OrderedColumn[0]);
      }
    }

    private IEnumerable<object> GetAndCheckObjectIDValues (IEnumerable<ObjectID> objectIDs)
    {
      // TODO 4231: Use TableDefinition.ObjectIDProperty.SplitValueForComparison (objectID)
      foreach (var t in objectIDs)
      {
        if (t.StorageProviderDefinition != _storageProviderDefinition)
          throw new NotSupportedException ("Multi-ID lookups can only be performed for ObjectIDs from this storage provider.");
        yield return t.Value;
      }
    }

  }
}