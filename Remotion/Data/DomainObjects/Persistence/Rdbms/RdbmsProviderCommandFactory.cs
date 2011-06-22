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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public class RdbmsProviderCommandFactory : IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext>
  {
    private readonly SingleDataContainerLookupCommandFactory _singleDataContainerLookupCommandFactory;
    private readonly MultiDataContainerLookupCommandFactory _multiDataContainerLookupCommandFactory;
    private readonly RelatedDataContainerLookupCommandFactory _relatedDataContainerLookupCommandFactory;
    private readonly IDbCommandBuilderFactory _commandBuilderFactory;
    private readonly IDataContainerReader _dataContainerReader;

    public RdbmsProviderCommandFactory (
        IDbCommandBuilderFactory commandBuilderFactory, IDataContainerReader dataContainerReader, IObjectIDReader objectIDReader)
    {
      ArgumentUtility.CheckNotNull ("commandBuilderFactory", commandBuilderFactory);
      ArgumentUtility.CheckNotNull ("dataContainerReader", dataContainerReader);
      ArgumentUtility.CheckNotNull ("objectIDReader", objectIDReader);

      _commandBuilderFactory = commandBuilderFactory;
      _dataContainerReader = dataContainerReader;
      _singleDataContainerLookupCommandFactory = new SingleDataContainerLookupCommandFactory (commandBuilderFactory, dataContainerReader);
      _multiDataContainerLookupCommandFactory = new MultiDataContainerLookupCommandFactory (commandBuilderFactory, dataContainerReader);
      _relatedDataContainerLookupCommandFactory = new RelatedDataContainerLookupCommandFactory (
          commandBuilderFactory, this, dataContainerReader, objectIDReader);
    }

    public SingleDataContainerLookupCommandFactory SingleDataContainerLookupCommandFactory
    {
      get { return _singleDataContainerLookupCommandFactory; }
    }

    public MultiDataContainerLookupCommandFactory MultiDataContainerLookupCommandFactory
    {
      get { return _multiDataContainerLookupCommandFactory; }
    }

    public RelatedDataContainerLookupCommandFactory RelatedDataContainerLookupCommandFactory
    {
      get { return _relatedDataContainerLookupCommandFactory; }
    }

    public IStorageProviderCommand<DataContainer, IRdbmsProviderCommandExecutionContext> CreateForSingleIDLookup (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      return _singleDataContainerLookupCommandFactory.CreateCommand (objectID);
    }

    public IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext> CreateForMultiIDLookup (ObjectID[] objectIDs)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("objectIDs", objectIDs);

      return _multiDataContainerLookupCommandFactory.CreateCommand (objectIDs);
    }

    public IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext> CreateForRelationLookup (
        RelationEndPointDefinition foreignKeyEndPoint, ObjectID foreignKeyValue, SortExpressionDefinition sortExpressionDefinition)
    {
      ArgumentUtility.CheckNotNull ("foreignKeyEndPoint", foreignKeyEndPoint);
      ArgumentUtility.CheckNotNull ("foreignKeyValue", foreignKeyValue);

      return _relatedDataContainerLookupCommandFactory.CreateCommand (foreignKeyEndPoint, foreignKeyValue, sortExpressionDefinition);
    }

    public IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext> CreateForDataContainerQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      return new MultiDataContainerLoadCommand (new[] { _commandBuilderFactory.CreateForQuery (query) }, true, _dataContainerReader);
    }
  }
}