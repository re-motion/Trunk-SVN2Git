// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.DataObjectModel;
using Remotion.Data.Linq.EagerFetching;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Utilities;
using System.Reflection;

namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  /// The class uses re-store to execute queries.
  /// </summary>
  public abstract class QueryExecutorBase : IQueryExecutor
  {
    /// <summary>
    /// Initializes a new instance of this <see cref="QueryExecutorBase"/> class.
    /// </summary>
    /// <param name="sqlGenerator">The sql generator <see cref="ISqlGenerator"/> which is used for querying re-store.</param>
    public QueryExecutorBase (ISqlGenerator sqlGenerator)
    {
      SqlGenerator = sqlGenerator;
    }

    public ISqlGenerator SqlGenerator { get; private set; }

    /// <summary>
    /// Creates and executes a given <see cref="QueryModel"/>.
    /// </summary>
    /// <param name="queryModel">The generated <see cref="QueryModel"/> of the linq query.</param>
    /// <returns>The result of the executed query as single object.</returns>
    public object ExecuteSingle (QueryModel queryModel)
    {
      IEnumerable results = ExecuteCollection (queryModel);
      var resultList = new ArrayList();
      foreach (object o in results)
        resultList.Add (o);
      if (resultList.Count == 1)
        return resultList[0];
      else
      {
        string message = string.Format ("ExecuteSingle must return a single object, but the query returned {0} objects.", resultList.Count);
        throw new InvalidOperationException (message);
      }
    }

    /// <summary>
    /// Creates and executes a given <see cref="IQuery"/>.
    /// </summary>
    /// <param name="queryModel">The generated <see cref="QueryModel"/> of the linq query.</param>
    /// <returns>The result of the executed query as <see cref="IEnumerable"/>.</returns>
    public IEnumerable ExecuteCollection (QueryModel queryModel)
    {
      if (ClientTransaction.Current == null)
        throw new InvalidOperationException ("No ClientTransaction has been associated with the current thread.");

      IQuery query = CreateQuery("<dynamic query>", queryModel);
      return ClientTransaction.Current.QueryManager.GetCollection (query).AsEnumerable();
    }

    /// <summary>
    /// Check to avoid choosing a column in the select projection. This is needed because re-store does not support single columns.
    /// </summary>
    /// <param name="evaluation"></param>
    private void CheckProjection (IEvaluation evaluation)
    {
      if (!(evaluation is Column))
      {
        string message = string.Format ("This query provider does not support the given select projection ('{0}'). The projection must select "
                                        + "single DomainObject instances, because re-store does not support this kind of select projection.", evaluation.GetType ().Name);
        throw new InvalidOperationException (message);
      }
      
      var column = (Column) evaluation;
      if (column.Name != "*")
      {
        string message = string.Format (
            "This query provider does not support selecting single columns ('{0}'). The projection must select whole DomainObject instances.",
            column.ColumnSource.AliasString + "." + column.Name);
        throw new InvalidOperationException (message);
      }
    }

    /// <summary>
    /// Creates a a <see cref="IQuery"/> object based on the given <see cref="QueryModel"/>.
    /// </summary>
    /// <param name="id">The identifier for the linq query.</param>
    /// <param name="queryModel">The <see cref="QueryModel"/> for the given query.</param>
    /// <returns>A <see cref="IQuery"/> object.</returns>
    public virtual IQuery CreateQuery (string id, QueryModel queryModel/*, IEnumerable<IFetchRequest> fetchRequests*/)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      ClassDefinition classDefinition = GetClassDefinition ();

      CommandData commandData = CreateStatement (queryModel);
      CheckProjection (commandData.SqlGenerationData.SelectEvaluation);

      var query = CreateQuery (id, classDefinition, commandData.Statement, commandData.Parameters);
      // CreateEagerFetchQueries (query, queryModel, classDefinition, fetchRequests);
      return query;
    }

    //private void CreateEagerFetchQueries (IQuery query, QueryModel queryModel, ClassDefinition classDefinition, IEnumerable<IFetchRequest> fetchRequests)
    //{
    //  foreach (var fetchRequest in fetchRequests)
    //  {
    //    var propertyInfo = fetchRequest.RelationMember as PropertyInfo;
    //    if (propertyInfo == null)
    //      throw new NotSupportedException ("Members of type " + fetchRequest.RelationMember.Name + " are not supported by this LINQ provider.");

    //    var propertyName = MappingConfiguration.Current.NameResolver.GetPropertyName (propertyInfo);
    //    var relationEndPointDefinition = classDefinition.GetMandatoryRelationEndPointDefinition (propertyName);

    //    var sourceParameter = Expression.Parameter (fetchRequest.OriginatingObjectType, "transparent");
    //    var collectionElementParameter = Expression.Parameter (fetchRequest.RelatedObjectType, "x");
    //    var newProjectionExpression = Expression.Lambda (collectionElementParameter, sourceParameter, collectionElementParameter);
    //    var newFromClause = new MemberFromClause (queryModel.SelectOrGroupClause.PreviousClause, collectionElementParameter, fetchRequest.RelatedObjectSelector, newProjectionExpression);

    //    queryModel.AddBodyClause (newFromClause);

    //    var fetchQuery = CreateQuery ("fetch query for " + propertyName, queryModel, fetchRequest.InnerFetchRequests);
    //    query.EagerFetchQueries.Add (relationEndPointDefinition, fetchQuery);
    //  }
    //}

    /// <summary>
    /// Creates a <see cref="IQuery"/> object.
    /// </summary>
    /// <param name="id">The identifier for the linq query.</param>
    /// <param name="classDefinition">The class definition for the type of the query.</param>
    /// <param name="statement">The sql statement of the query.</param>
    /// <param name="commandParameters">The parameters of the sql statement.</param>
    /// <returns>A <see cref="IQuery"/> object.</returns>
    public virtual IQuery CreateQuery(string id, ClassDefinition classDefinition, string statement, CommandParameter[] commandParameters)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("statement", statement);
      ArgumentUtility.CheckNotNull ("commandParameters", commandParameters);

      var queryParameters = new QueryParameterCollection();
      foreach (CommandParameter commandParameter in commandParameters)
        queryParameters.Add (commandParameter.Name, commandParameter.Value, QueryParameterType.Value);

      return QueryFactory.CreateCollectionQuery (id, classDefinition.StorageProviderID, statement, queryParameters, typeof (DomainObjectCollection));
    }

    public abstract ClassDefinition GetClassDefinition ();

    /// <summary>
    /// Uses the given <see cref="ISqlGenerator"/> to generate sql code for the linq query.
    /// </summary>
    /// <param name="queryModel">The generated <see cref="QueryModel"/> of the linq query.</param>
    /// <returns>A <see cref="CommandData"/> object for the given <see cref="QueryModel"/>.</returns>
    public virtual CommandData CreateStatement (QueryModel queryModel)
    {
      return SqlGenerator.BuildCommand (queryModel);
    }
  }
}
