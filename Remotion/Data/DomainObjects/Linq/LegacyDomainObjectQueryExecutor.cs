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
using Remotion.Collections;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Backend.DataObjectModel;
using Remotion.Data.Linq.Backend.SqlGeneration;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.Clauses.StreamedData;
using Remotion.Data.Linq.EagerFetching;
using Remotion.Logging;
using Remotion.Utilities;
using System.Reflection;
using System.Linq;
using Remotion.Data.DomainObjects.Queries.Configuration;

namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  /// Provides an implementation of <see cref="IQueryExecutor"/> for <see cref="DomainObject"/> queries.
  /// </summary>
  public class LegacyDomainObjectQueryExecutor : IQueryExecutor
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (LegacyDomainObjectQueryExecutor));

    /// <summary>
    /// Initializes a new instance of this <see cref="LegacyDomainObjectQueryExecutor"/> class.
    /// </summary>
    /// <param name="sqlGenerator">The sql generator <see cref="ISqlGenerator"/> which is used for querying re-store.</param>
    /// <param name="startingClassDefinition">The <see cref="ClassDefinition"/> of the <see cref="DomainObject"/> type the query is started 
    /// with. This determines the <see cref="StorageProvider"/> used for the query.</param>
    public LegacyDomainObjectQueryExecutor (ISqlGenerator sqlGenerator, ClassDefinition startingClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("sqlGenerator", sqlGenerator);
      ArgumentUtility.CheckNotNull ("startingClassDefinition", startingClassDefinition);

      StartingClassDefinition = startingClassDefinition;
      SqlGenerator = sqlGenerator;
    }

    public ISqlGenerator SqlGenerator { get; private set; }
    public ClassDefinition StartingClassDefinition { get; private set; }

    /// <summary>
    /// Creates and executes a given <see cref="QueryModel"/> as an <see cref="IQuery"/> using the current <see cref="ClientTransaction"/>'s
    /// <see cref="ClientTransaction.QueryManager"/>. The query is executed as a scalar query.
    /// </summary>
    /// <param name="queryModel">The generated <see cref="QueryModel"/> of the LINQ query.</param>
    /// <returns>
    /// The result of the executed query, cast to <typeparam name="T"/>.
    /// </returns>
    public T ExecuteScalar<T> (QueryModel queryModel)
    {
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      if (ClientTransaction.Current == null)
        throw new InvalidOperationException ("No ClientTransaction has been associated with the current thread.");

      var fetchQueryModelBuilders = FetchFilteringQueryModelVisitor.RemoveFetchRequestsFromQueryModel (queryModel);

      IQuery query = CreateQuery ("<dynamic query>", queryModel, fetchQueryModelBuilders, QueryType.Scalar);
      return (T) ClientTransaction.Current.QueryManager.GetScalar (query);
    }

    /// <summary>
    /// Creates and executes a given <see cref="QueryModel"/> as an <see cref="IQuery"/> using the current <see cref="ClientTransaction"/>'s
    /// <see cref="ClientTransaction.QueryManager"/>. The query is executed as a collection query, and its result set is expected in its result set.
    /// </summary>
    /// <param name="queryModel">The generated <see cref="QueryModel"/> of the LINQ query.</param>
    /// <param name="returnDefaultWhenEmpty">If <see langword="true" />, the executor returns a default value when the result set is empty; 
    /// if <see langword="false" />, it throws an <see cref="InvalidOperationException"/> when its result set is empty.</param>
    /// <returns>
    /// The result of the executed query, cast to <typeparam name="T"/>.
    /// </returns>
    public T ExecuteSingle<T> (QueryModel queryModel, bool returnDefaultWhenEmpty)
    {
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      if (ClientTransaction.Current == null)
        throw new InvalidOperationException ("No ClientTransaction has been associated with the current thread.");

      var sequence = ExecuteCollection<T> (queryModel);

      if (returnDefaultWhenEmpty)
        return sequence.SingleOrDefault ();
      else
        return sequence.Single();
    }

    /// <summary>
    /// Creates and executes a given <see cref="QueryModel"/> as an <see cref="IQuery"/> using the current <see cref="ClientTransaction"/>'s
    /// <see cref="ClientTransaction.QueryManager"/>. The query is executed as a collection query.
    /// </summary>
    /// <param name="queryModel">The generated <see cref="QueryModel"/> of the LINQ query.</param>
    /// <returns>
    /// The result of the executed query as an <see cref="IEnumerable{T}"/>.
    /// </returns>
    public IEnumerable<T> ExecuteCollection<T> (QueryModel queryModel)
    {
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      if (ClientTransaction.Current == null)
        throw new InvalidOperationException ("No ClientTransaction has been associated with the current thread.");

      var fetchQueryModelBuilders = FetchFilteringQueryModelVisitor.RemoveFetchRequestsFromQueryModel (queryModel);

      var groupResultOperator = queryModel.ResultOperators.OfType<GroupResultOperator>().FirstOrDefault();
      if (groupResultOperator != null)
      {
        if (fetchQueryModelBuilders.Any ())
        {
          var message = "Cannot execute a query with a GroupBy clause that specifies fetch requests because GroupBy is simulated in-memory.";
          throw new NotSupportedException (message);
        }

        return ExecuteCollectionWithGrouping<T> (queryModel, groupResultOperator);
      }
      else
      {
        IQuery query = CreateQuery ("<dynamic query>", queryModel, fetchQueryModelBuilders, QueryType.Collection);
        return ClientTransaction.Current.QueryManager.GetCollection (query).AsEnumerable().Cast<T>();
      }
    }

    private IEnumerable<T> ExecuteCollectionWithGrouping<T> (QueryModel queryModel, GroupResultOperator groupResultOperator)
    {
      if (s_log.IsWarnEnabled)
      {
        var message = string.Format (
            "Executing a group-by operation in memory. This can lead to performance problems if the grouping involves properties that "
            + "are loaded lazily. In such cases it can be better to execute a select query with respective Fetch hints on the database, and then "
            + "group in memory via LINQ-to-Objects. Executed query: '{0}'.",
            queryModel);
        s_log.Warn (message);
      }

      var lastResultOperatorIndex = queryModel.ResultOperators.Count - 1;
      if (queryModel.ResultOperators[lastResultOperatorIndex] != groupResultOperator)
      {
        var message = "Cannot execute a query with a GroupBy clause that contains other result operators after the GroupResultOperator because "
            + "GroupBy is simulated in-memory.";
        throw new NotSupportedException (message);
      }

      queryModel.ResultOperators.RemoveAt (lastResultOperatorIndex);

      var databaseResult = queryModel.Execute (this);
      var outputData = (StreamedSequence) groupResultOperator.ExecuteInMemory (databaseResult);
      return outputData.GetTypedSequence<T>();
    }

    /// <summary>
    /// Creates an <see cref="IQuery"/> object based on the given <see cref="QueryModel"/>.
    /// </summary>
    /// <param name="id">The identifier for the linq query.</param>
    /// <param name="queryModel">The <see cref="QueryModel"/> for the given query.</param>
    /// <param name="fetchQueryModelBuilders">The <see cref="FetchQueryModelBuilder"/> instances for the fetch requests to be executed together with 
    /// the query.</param>
    /// <param name="queryType">The type of query to create.</param>
    /// <returns>
    /// An <see cref="IQuery"/> object corresponding to the given <paramref name="queryModel"/>.
    /// </returns>
    public virtual IQuery CreateQuery (string id, QueryModel queryModel, IEnumerable<FetchQueryModelBuilder> fetchQueryModelBuilders, QueryType queryType)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);
      ArgumentUtility.CheckNotNull ("fetchQueryModelBuilders", fetchQueryModelBuilders);

      return CreateQuery (id, queryModel, fetchQueryModelBuilders, queryType, StartingClassDefinition, null);
    }

    /// <summary>
    /// Creates an <see cref="IQuery"/> object based on the given <see cref="QueryModel"/>.
    /// </summary>
    /// <param name="id">The identifier for the linq query.</param>
    /// <param name="queryModel">The <see cref="QueryModel"/> for the given query.</param>
    /// <param name="fetchQueryModelBuilders">The <see cref="FetchQueryModelBuilder"/> instances for the fetch requests to be executed together with 
    /// the query.</param>
    /// <param name="queryType">The type of query to create.</param>
    /// <param name="classDefinitionOfResult">The class definition of the result objects to be returned by the query. This is used to obtain the
    /// storage provider to execute the query and to resolve the relation properties of the fetch requests.</param>
    /// <param name="sortExpression">A SQL expression that is used in an ORDER BY clause to sort the query results.</param>
    /// <returns>
    /// An <see cref="IQuery"/> object corresponding to the given <paramref name="queryModel"/>.
    /// </returns>
    protected virtual IQuery CreateQuery (string id, QueryModel queryModel, IEnumerable<FetchQueryModelBuilder> fetchQueryModelBuilders, QueryType queryType, ClassDefinition classDefinitionOfResult, string sortExpression)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);
      ArgumentUtility.CheckNotNull ("fetchQueryModelBuilders", fetchQueryModelBuilders);
      ArgumentUtility.CheckNotNull ("classDefinitionOfResult", classDefinitionOfResult);

      CommandData commandData = CreateStatement (queryModel);
      CheckProjection (commandData.SqlGenerationData.SelectEvaluation);
      CheckNoResultOperatorsAfterFetch (fetchQueryModelBuilders);

      var statement = commandData.Statement;
      if (!string.IsNullOrEmpty (sortExpression))
      {
        Assertion.IsFalse (queryModel.BodyClauses.OfType<OrderByClause> ().Any (), 
            "We assume that fetch request query models cannot have OrderBy clauses.");
        statement = statement + " ORDER BY " + sortExpression;
      }

      var query = CreateQuery (id, classDefinitionOfResult.StorageProviderID, statement, commandData.Parameters, queryType);
      CreateEagerFetchQueries (query, classDefinitionOfResult, fetchQueryModelBuilders);
      return query;
    }

    /// <summary>
    /// Creates a <see cref="IQuery"/> object.
    /// </summary>
    /// <param name="id">The identifier for the linq query.</param>
    /// <param name="storageProviderID">The ID of the <see cref="StorageProvider"/> to be used for the query.</param>
    /// <param name="statement">The sql statement of the query.</param>
    /// <param name="commandParameters">The parameters of the sql statement.</param>
    /// <param name="queryType">The type of query to create.</param>
    /// <returns>A <see cref="IQuery"/> object.</returns>
    public virtual IQuery CreateQuery (string id, string storageProviderID, string statement, CommandParameter[] commandParameters, QueryType queryType)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNull ("storageProviderID", storageProviderID);
      ArgumentUtility.CheckNotNull ("statement", statement);
      ArgumentUtility.CheckNotNull ("commandParameters", commandParameters);

      var queryParameters = new QueryParameterCollection ();
      foreach (CommandParameter commandParameter in commandParameters)
        queryParameters.Add (commandParameter.Name, commandParameter.Value, QueryParameterType.Value);

      if (queryType == QueryType.Scalar)
        return QueryFactory.CreateScalarQuery (id, storageProviderID, statement, queryParameters);
      else
        return QueryFactory.CreateCollectionQuery (id, storageProviderID, statement, queryParameters, typeof (DomainObjectCollection));
    }

    private void CreateEagerFetchQueries (IQuery query, ClassDefinition classDefinition, IEnumerable<FetchQueryModelBuilder> fetchQueryModelBuilders)
    {
      foreach (var fetchQueryModelBuilder in fetchQueryModelBuilders)
      {
        IRelationEndPointDefinition relationEndPointDefinition = 
            GetEagerFetchRelationEndPointDefinition (fetchQueryModelBuilder.FetchRequest, classDefinition);

        string sortExpression = GetSortExpressionForRelation (relationEndPointDefinition);

        var fetchQueryModel = fetchQueryModelBuilder.GetOrCreateFetchQueryModel ().Clone(); // clone because we don't want to modify the source model of all inner requests
        fetchQueryModel.ResultOperators.Add (new DistinctResultOperator ()); // fetch queries should always be distinct even when the query would return duplicates

        var fetchQuery = CreateQuery (
            "<fetch query for " + fetchQueryModelBuilder.FetchRequest.RelationMember.Name + ">",
            fetchQueryModel,
            fetchQueryModelBuilder.CreateInnerBuilders(),
            QueryType.Collection,
            relationEndPointDefinition.GetOppositeClassDefinition(),
            sortExpression);

          query.EagerFetchQueries.Add (relationEndPointDefinition, fetchQuery);
      }
    }

    private IRelationEndPointDefinition GetEagerFetchRelationEndPointDefinition (FetchRequestBase fetchRequest, ClassDefinition classDefinition)
    {
      var propertyInfo = fetchRequest.RelationMember as PropertyInfo;
      if (propertyInfo == null)
      {
        var message = string.Format (
            "The member '{0}' is a '{1}', which cannot be fetched by this LINQ provider. Only properties can be fetched.",
            fetchRequest.RelationMember.Name,
            fetchRequest.RelationMember.MemberType);
        throw new NotSupportedException (message);
      }

      var propertyName = MappingConfiguration.Current.NameResolver.GetPropertyName (propertyInfo);
      try
      {
        return classDefinition.GetMandatoryRelationEndPointDefinition (propertyName);
      }
      catch (MappingException ex)
      {
        var message = string.Format (
            "The property '{0}' is not a relation end point. Fetching it is not supported by this LINQ provider.", 
            propertyName);
        throw new NotSupportedException (message, ex);
      }
    }

    private string GetSortExpressionForRelation (IRelationEndPointDefinition relationEndPointDefinition)
    {
      var virtualEndPointDefinition = relationEndPointDefinition as VirtualRelationEndPointDefinition;
      if (virtualEndPointDefinition != null)
        return virtualEndPointDefinition.SortExpression;
      else
        return null;
    }

    /// <summary>
    /// Uses the given <see cref="ISqlGenerator"/> to generate sql code for the linq query.
    /// </summary>
    /// <param name="queryModel">The generated <see cref="QueryModel"/> of the linq query.</param>
    /// <returns>A <see cref="CommandData"/> object for the given <see cref="QueryModel"/>.</returns>
    public virtual CommandData CreateStatement (QueryModel queryModel)
    {
      return SqlGenerator.BuildCommand (queryModel);
    }

    /// <summary>
    /// Check to avoid choosing a column in the select projection. This is needed because re-store does not support single columns.
    /// </summary>
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
    /// Check to avoid fetch requests that are followed by result operators. re-store cannot fetch without actually selecting the source objects.
    /// </summary>
    private void CheckNoResultOperatorsAfterFetch (IEnumerable<FetchQueryModelBuilder> fetchQueryModelBuilders)
    {
      foreach (var fetchQueryModelBuilder in fetchQueryModelBuilders)
      {
        if (fetchQueryModelBuilder.ResultOperatorPosition < fetchQueryModelBuilder.SourceItemQueryModel.ResultOperators.Count)
        {
          string message = "This query provider does not support result operators occurring after fetch requests. The objects on which the fetching "
              + "is performed must be the same objects that are returned from the query. Rewrite the query to perform the fetching after applying "
              + "all other result operators or call AsEnumerable after the last fetch request in order to execute all subsequent result operators in "
              + "memory.";
          throw new InvalidOperationException (message);
        }
      }
    }
  }
}
