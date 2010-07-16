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
using System.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.Linq.Parsing.Structure;
using Remotion.Data.Linq.SqlBackend.MappingResolution;
using Remotion.Data.Linq.SqlBackend.SqlGeneration;
using Remotion.Data.Linq.SqlBackend.SqlPreparation;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  public class LinqPerformanceTestHelper<T>
  {
    private readonly Func<IQueryable<T>> _queryGenerator;
    private readonly MethodCallExpressionNodeTypeRegistry _nodeTypeRegistry = MethodCallExpressionNodeTypeRegistry.CreateDefault();
    private readonly ISqlPreparationStage _sqlPreparationStage;
    private readonly IMappingResolutionStage _mappingResolutionStage;
    private readonly ISqlGenerationStage _sqlGenerationStage;
    private readonly IMappingResolutionContext _mappingResolutionContext;

    public LinqPerformanceTestHelper (
        Func<IQueryable<T>> queryGenerator,
        ISqlPreparationStage sqlPreparationStage,
        IMappingResolutionStage mappingResolutionStage,
        ISqlGenerationStage sqlGenerationStage,
        IMappingResolutionContext mappingResolutionContext)
    {
      ArgumentUtility.CheckNotNull ("queryGenerator", queryGenerator);
      ArgumentUtility.CheckNotNull ("sqlPreparationStage", sqlPreparationStage);
      ArgumentUtility.CheckNotNull ("mappingResolutionStage", mappingResolutionStage);
      ArgumentUtility.CheckNotNull ("sqlGenerationStage", sqlGenerationStage);
      ArgumentUtility.CheckNotNull ("mappingResolutionContext", mappingResolutionContext);
      
      _queryGenerator = queryGenerator;
      _sqlPreparationStage = sqlPreparationStage;
      _mappingResolutionStage = mappingResolutionStage;
      _sqlGenerationStage = sqlGenerationStage;
      _mappingResolutionContext = mappingResolutionContext;
    }

    public bool GenerateQueryModel ()
    {
      var expressionTreeParser = new ExpressionTreeParser (_nodeTypeRegistry);
      var queryParser = new QueryParser (expressionTreeParser);
      var queryable = _queryGenerator();
      return queryParser.GetParsedQuery (queryable.Expression) != null;
    }

    public bool GenerateQueryModelAndSQL ()
    {
      var expressionTreeParser = new ExpressionTreeParser (_nodeTypeRegistry);
      var queryParser = new QueryParser (expressionTreeParser);
      var queryable = _queryGenerator();
      var queryModel = queryParser.GetParsedQuery (queryable.Expression);

      var sqlStatement = _sqlPreparationStage.PrepareSqlStatement (queryModel, null);
      var resolvedSqlStatement = _mappingResolutionStage.ResolveSqlStatement (sqlStatement, _mappingResolutionContext);

      var commandBuilder = new SqlCommandBuilder();
      _sqlGenerationStage.GenerateTextForOuterSqlStatement (commandBuilder, resolvedSqlStatement);
      return !string.IsNullOrEmpty(commandBuilder.GetCommand().CommandText);
    }

    public bool GenerateQueryModelAndSQLAndIQuery ()
    {
      var query = _queryGenerator ();
      return QueryFactory.CreateQuery ("perftest", query) != null;
    }

    public bool GenerateAndExecuteQuery ()
    {
      var query = _queryGenerator ();
      return query.ToList ().Count > 100;
    }  


  }
}