// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Linq;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.SqlBackend.MappingResolution;
using Remotion.Linq.SqlBackend.SqlGeneration;
using Remotion.Linq.SqlBackend.SqlPreparation;
using Remotion.Linq.Utilities;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  public class LinqPerformanceTestHelper<T>
  {
    private readonly IQueryParser _queryParser = QueryParser.CreateDefault();
    private readonly CompoundMethodCallTransformerProvider _methodCallTransformerProvider = CompoundMethodCallTransformerProvider.CreateDefault();
    private readonly ResultOperatorHandlerRegistry _resultOperatorHandlerRegistry = ResultOperatorHandlerRegistry.CreateDefault();

    private readonly Func<IQueryable<T>> _queryGenerator;

    public LinqPerformanceTestHelper (Func<IQueryable<T>> queryGenerator)
    {
      ArgumentUtility.CheckNotNull ("queryGenerator", queryGenerator);

      _queryGenerator = queryGenerator;
    }

    public bool GenerateQueryModel ()
    {
      var queryable = _queryGenerator();
      return _queryParser.GetParsedQuery (queryable.Expression) != null;
    }

    public bool GenerateQueryModelAndSQL ()
    {
      var generator = new UniqueIdentifierGenerator();
      var sqlPreparationStage = new DefaultSqlPreparationStage (_methodCallTransformerProvider, _resultOperatorHandlerRegistry, generator);
      var mappingResolutionStage =
          new DefaultMappingResolutionStage (
              new MappingResolver (new StorageSpecificExpressionResolver (new RdbmsPersistenceModelProvider ())),
              generator);
      var sqlGenerationStage = new DefaultSqlGenerationStage();
      var mappingResolutionContext = new MappingResolutionContext();

      var queryable = _queryGenerator();
      var queryModel = _queryParser.GetParsedQuery (queryable.Expression);

      var sqlStatement = sqlPreparationStage.PrepareSqlStatement (queryModel, null);
      var resolvedSqlStatement = mappingResolutionStage.ResolveSqlStatement (sqlStatement, mappingResolutionContext);

      var commandBuilder = new SqlCommandBuilder();
      sqlGenerationStage.GenerateTextForOuterSqlStatement (commandBuilder, resolvedSqlStatement);
      var sqlCommandData = commandBuilder.GetCommand();

      return !string.IsNullOrEmpty (sqlCommandData.CommandText);
    }

    public bool GenerateQueryModelAndSQLAndIQuery ()
    {
      var query = _queryGenerator();
      return QueryFactory.CreateQuery<T> ("perftest", query) != null;
    }

    public bool GenerateAndExecuteQueryDBOnly ()
    {
      var query = _queryGenerator();
      var restoreQuery = QueryFactory.CreateQuery<T> ("perftest", query);

      using (var manager = new StorageProviderManager (NullPersistenceExtension.Instance))
      {
        return manager.GetMandatory ("PerformanceTestDomain").ExecuteCollectionQuery (restoreQuery).ToArray().Length > 100;
      }
    }

    public bool GenerateAndExecuteQuery ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var query = _queryGenerator();
        var recordCount = query.ToList().Count;
        return recordCount > 100;
      }
    }
  }
}