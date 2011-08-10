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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories
{
  /// <summary>
  /// The <see cref="QueryCommandFactory"/> is responsible for creating query commands for a relational database.
  /// </summary>
  public class QueryCommandFactory
  {
    private readonly StorageProviderDefinition _storageProviderDefinition;
    private readonly IStorageTypeInformationProvider _storageTypeInformationProvider;
    private readonly IObjectReaderFactory _objectReaderFactory;
    private readonly IDbCommandBuilderFactory _dbCommandBuilderFactory;

    public QueryCommandFactory (
        StorageProviderDefinition storageProviderDefinition,
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IObjectReaderFactory objectReaderFactory,
        IDbCommandBuilderFactory dbCommandBuilderFactory)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("storageTypeInformationProvider", storageTypeInformationProvider);
      ArgumentUtility.CheckNotNull ("objectReaderFactory", objectReaderFactory);
      ArgumentUtility.CheckNotNull ("dbCommandBuilderFactory", dbCommandBuilderFactory);

      _storageProviderDefinition = storageProviderDefinition;
      _storageTypeInformationProvider = storageTypeInformationProvider;
      _objectReaderFactory = objectReaderFactory;
      _dbCommandBuilderFactory = dbCommandBuilderFactory;
    }

    public IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext> CreateForDataContainerQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      var dataContainerReader = _objectReaderFactory.CreateDataContainerReader ();

      var dbCommandBuilder = CreateDbCommandBuilder(query);
      return new MultiObjectLoadCommand<DataContainer> (new[] { Tuple.Create (dbCommandBuilder, dataContainerReader) });
    }

    public IStorageProviderCommand<object, IRdbmsProviderCommandExecutionContext> CreateForScalarQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      var dbCommandBuilder = CreateDbCommandBuilder (query);
      return new ScalarValueLoadCommand (dbCommandBuilder);
    }

    private IDbCommandBuilder CreateDbCommandBuilder (IQuery query)
    {
      var queryParametersWithType = query.Parameters
          .Cast<QueryParameter> ()
          .Select (GetQueryParameterWithType);

      return _dbCommandBuilderFactory.CreateForQuery (query.Statement, queryParametersWithType);
    }

    private QueryParameterWithType GetQueryParameterWithType (QueryParameter parameter)
    {
      if (parameter.Value is ObjectID)
      {
        var objectID = (ObjectID) parameter.Value;
        if (objectID.StorageProviderDefinition != _storageProviderDefinition)
          parameter = new QueryParameter (parameter.Name, objectID.ToString (), parameter.ParameterType);
        else
          parameter = new QueryParameter (parameter.Name, objectID.Value, parameter.ParameterType);
      }

      return new QueryParameterWithType (parameter, _storageTypeInformationProvider.GetStorageType (parameter.Value));
    }
  }
}