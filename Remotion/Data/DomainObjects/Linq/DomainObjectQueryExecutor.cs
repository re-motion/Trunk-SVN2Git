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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.EagerFetching;
using Remotion.Linq.SqlBackend.MappingResolution;
using Remotion.Linq.SqlBackend.SqlGeneration;
using Remotion.Linq.SqlBackend.SqlPreparation;
using Remotion.Linq.SqlBackend.SqlStatementModel;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Reflection;
using Remotion.Utilities;
using SortOrder = Remotion.Data.DomainObjects.Mapping.SortExpressions.SortOrder;
using SqlCommandBuilder = Remotion.Linq.SqlBackend.SqlGeneration.SqlCommandBuilder;

namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  /// Provides an implementation of <see cref="IQueryExecutor"/> for <see cref="DomainObject"/> queries.
  /// </summary>
  public class DomainObjectQueryExecutor : IQueryExecutor
  {
    private readonly ISqlPreparationStage _preparationStage;
    private readonly IMappingResolutionStage _resolutionStage;
    private readonly ISqlGenerationStage _generationStage;
    private readonly IMappingResolutionContext _mappingResolutionContext;
    private readonly ClassDefinition _startingClassDefinition;
    private readonly IStorageTypeInformationProvider _storageTypeInformationProvider;
    private readonly TypeConversionProvider _typeConversionProvider;

    /// <summary>
    /// Initializes a new instance of this <see cref="DomainObjectQueryExecutor"/> class.
    /// </summary>
    /// <param name="startingClassDefinition">The <see cref="ClassDefinition"/> of the <see cref="DomainObject"/> type the query is started 
    ///   with. This determines the <see cref="StorageProvider"/> used for the query.</param>
    /// <param name="preparationStage">The <see cref="ISqlPreparationStage"/> provides methods to prepare the <see cref="SqlStatement"/> based on a <see cref="QueryModel"/>.</param>
    /// <param name="resolutionStage">The <see cref="IMappingResolutionStage"/> provides methods to resolve the expressions in the <see cref="SqlStatement"/>.</param>
    /// <param name="generationStage">The <see cref="ISqlGenerationStage"/> provides methods to generate sql text for the given <see cref="SqlStatement"/>.</param>
    /// <param name="storageTypeInformationProvider">The <see cref="IStorageTypeInformationProvider"/> provides methods to determine the storage-specific type for a domain object property type.</param>
    /// <param name="typeConversionProvider">The <see cref="TypeConversionProvider"/> provides functionality to get the <see cref="TypeConverter"/> for a <see cref="Type"/> and to convert a value from a source <see cref="Type"/> into a destination <see cref="Type"/>.</param>
    public DomainObjectQueryExecutor (
        ClassDefinition startingClassDefinition,
        ISqlPreparationStage preparationStage,
        IMappingResolutionStage resolutionStage,
        ISqlGenerationStage generationStage,
        IStorageTypeInformationProvider storageTypeInformationProvider,
        TypeConversionProvider typeConversionProvider)
    {
      ArgumentUtility.CheckNotNull ("startingClassDefinition", startingClassDefinition);
      ArgumentUtility.CheckNotNull ("preparationStage", preparationStage);
      ArgumentUtility.CheckNotNull ("resolutionStage", resolutionStage);
      ArgumentUtility.CheckNotNull ("generationStage", generationStage);
      ArgumentUtility.CheckNotNull ("storageTypeInformationProvider", storageTypeInformationProvider);
      ArgumentUtility.CheckNotNull ("typeConversionProvider", typeConversionProvider);

      _startingClassDefinition = startingClassDefinition;
      _generationStage = generationStage;
      _resolutionStage = resolutionStage;
      _preparationStage = preparationStage;
      _mappingResolutionContext = new MappingResolutionContext();
      _storageTypeInformationProvider = storageTypeInformationProvider;
      _typeConversionProvider = typeConversionProvider;
    }

    /// <summary>
    /// Gets the <see cref="Persistence.Configuration.StorageProviderDefinition"/> that is used for the query.
    /// </summary>
    public StorageProviderDefinition StorageProviderDefinition
    {
      get { return _startingClassDefinition.StorageEntityDefinition.StorageProviderDefinition; }
    }

    /// <summary>
    /// Gets the <see cref="ISqlPreparationStage"/> used for generating SQL.
    /// </summary>
    public ISqlPreparationStage PreparationStage
    {
      get { return _preparationStage; }
    }

    /// <summary>
    /// Gets the <see cref="IMappingResolutionStage"/> used for generating SQL.
    /// </summary>
    public IMappingResolutionStage ResolutionStage
    {
      get { return _resolutionStage; }
    }

    /// <summary>
    /// Gets the <see cref="GenerationStage"/> used for generating SQL.
    /// </summary>
    public ISqlGenerationStage GenerationStage
    {
      get { return _generationStage; }
    }

    public IMappingResolutionContext MappingResolutionContext
    {
      get { return _mappingResolutionContext; }
    }

    public ClassDefinition StartingClassDefinition
    {
      get { return _startingClassDefinition; }
    }

    public IStorageTypeInformationProvider StorageTypeInformationProvider
    {
      get { return _storageTypeInformationProvider; }
    }

    public TypeConversionProvider TypeConversionProvider
    {
      get { return _typeConversionProvider; }
    }

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

      var fetchQueryModelBuilders = RemoveTrailingFetchRequests (queryModel);

      IQuery query = CreateQuery ("<dynamic query>", queryModel, fetchQueryModelBuilders, QueryType.Scalar);
      object scalarValue = ClientTransaction.Current.QueryManager.GetScalar (query);

      if (scalarValue == DBNull.Value)
        return (T) (object) null;

      if (scalarValue is T)
        return (T) scalarValue;

      // If we can't cast, there's still the chance that we can convert. This is especially needed for booleans, which will be integers when coming 
      // from the database.
      return (T) Convert.ChangeType (scalarValue, typeof (T));
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

      // Natively supported types can be executed as scalar queries
      if (_storageTypeInformationProvider.IsTypeSupported (typeof (T)))
        return ExecuteScalar<T> (queryModel);

      var sequence = ExecuteCollection<T> (queryModel);

      if (returnDefaultWhenEmpty)
        return sequence.SingleOrDefault();
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

      var fetchQueryModelBuilders = RemoveTrailingFetchRequests (queryModel);

      IQuery query = CreateQuery ("<dynamic query>", queryModel, fetchQueryModelBuilders, QueryType.Collection);
      return ClientTransaction.Current.QueryManager.GetCollection (query).AsEnumerable().Cast<T>();
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
    public virtual IQuery CreateQuery (
        string id, QueryModel queryModel, IEnumerable<FetchQueryModelBuilder> fetchQueryModelBuilders, QueryType queryType)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);
      ArgumentUtility.CheckNotNull ("fetchQueryModelBuilders", fetchQueryModelBuilders);

      return CreateQuery (id, queryModel, fetchQueryModelBuilders, queryType, _startingClassDefinition);
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
    /// <returns>
    /// An <see cref="IQuery"/> object corresponding to the given <paramref name="queryModel"/>.
    /// </returns>
    protected virtual IQuery CreateQuery (
        string id,
        QueryModel queryModel,
        IEnumerable<FetchQueryModelBuilder> fetchQueryModelBuilders,
        QueryType queryType,
        ClassDefinition classDefinitionOfResult)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);
      ArgumentUtility.CheckNotNull ("fetchQueryModelBuilders", fetchQueryModelBuilders);
      ArgumentUtility.CheckNotNull ("classDefinitionOfResult", classDefinitionOfResult);

      var command = CreateSqlCommand (queryModel, queryType == QueryType.Collection); // check result for DomainObjects unless it's a scalar query
      var statement = command.CommandText;

      var query = CreateQuery (id, StorageProviderDefinition, statement, command.Parameters, queryType);
      CreateEagerFetchQueries (query, classDefinitionOfResult, fetchQueryModelBuilders);
      return query;
    }

    /// <summary>
    /// Creates a SQL query from a given <see cref="QueryModel"/>.
    /// </summary>
    /// <param name="queryModel">
    /// The <see cref="QueryModel"/> a sql query is generated for. The query must not contain any eager fetch result operators.
    /// </param>
    /// <param name="checkResultIsDomainObject">If <see langword="true" />, the method will check whether the query returns <see cref="DomainObject"/>
    /// instances and throw an exception if not. If <see langword="false" />, no such check will be made.</param>
    /// <returns>A <see cref="SqlCommandData"/> instance containing the SQL text, parameters, and an in-memory projection for the given query model.</returns>
    public virtual SqlCommandData CreateSqlCommand (QueryModel queryModel, bool checkResultIsDomainObject)
    {
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      SqlStatement sqlStatement;
      try
      {
        sqlStatement = TransformAndResolveQueryModel (queryModel);
      }
      catch (NotSupportedException ex)
      {
        var message = string.Format ("There was an error preparing or resolving query '{0}' for SQL generation. {1}", queryModel, ex.Message);
        throw new NotSupportedException (message, ex);
      }
      catch (UnmappedItemException ex)
      {
        var message = string.Format ("Query '{0}' contains an unmapped item. {1}", queryModel, ex.Message);
        throw new UnmappedItemException (message, ex);
      }

      if (checkResultIsDomainObject)
        CheckQueryReturnsDomainObject (sqlStatement, queryModel);

      try
      {
        return CreateSqlCommand (sqlStatement);
      }
      catch (NotSupportedException ex)
      {
        var message = string.Format ("There was an error generating SQL for the query '{0}'. {1}", queryModel, ex.Message);
        throw new NotSupportedException (message, ex);
      }
    }

    /// <summary>
    /// Creates a <see cref="IQuery"/> object.
    /// </summary>
    /// <param name="id">The identifier for the linq query.</param>
    /// <param name="storageProviderDefinition">The <see cref="Persistence.Configuration.StorageProviderDefinition"/> to be used for the query.</param>
    /// <param name="statement">The sql statement of the query.</param>
    /// <param name="commandParameters">The parameters of the sql statement.</param>
    /// <param name="queryType">The type of query to create.</param>
    /// <returns>A <see cref="IQuery"/> object.</returns>
    public virtual IQuery CreateQuery (
        string id, StorageProviderDefinition storageProviderDefinition, string statement, CommandParameter[] commandParameters, QueryType queryType)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("statement", statement);
      ArgumentUtility.CheckNotNull ("commandParameters", commandParameters);

      var queryParameters = new QueryParameterCollection();
      foreach (CommandParameter commandParameter in commandParameters)
        queryParameters.Add (commandParameter.Name, commandParameter.Value, QueryParameterType.Value);

      if (queryType == QueryType.Scalar)
        return QueryFactory.CreateScalarQuery (id, storageProviderDefinition, statement, queryParameters);
      else
        return QueryFactory.CreateCollectionQuery (id, storageProviderDefinition, statement, queryParameters, typeof (DomainObjectCollection));
    }

    private void CreateEagerFetchQueries (IQuery query, ClassDefinition classDefinition, IEnumerable<FetchQueryModelBuilder> fetchQueryModelBuilders)
    {
      foreach (var fetchQueryModelBuilder in fetchQueryModelBuilders)
      {
        var relationEndPointDefinition = GetEagerFetchRelationEndPointDefinition (fetchQueryModelBuilder.FetchRequest, classDefinition);

        // clone the fetch query model because we don't want to modify the source model of all inner requests
        var fetchQueryModel = fetchQueryModelBuilder.GetOrCreateFetchQueryModel().Clone();
        
        // for re-store, fetch queries must always be distinct even when the query would return duplicates
        // e.g., for (from o in Orders select o).FetchOne (o => o.Customer)
        // when two orders have the same customer, re-store gives an error, unless we add a DISTINCT clause
        fetchQueryModel.ResultOperators.Add (new DistinctResultOperator());

        var sortExpression = GetSortExpressionForRelation (relationEndPointDefinition);
        if (sortExpression != null)
        {
          // If we have a SortExpression, we need to add the ORDER BY clause _after_ the DISTINCT (because re-linq strips out ORDER BY clauses when
          // seeing a DISTINCT operator); therefore, we put the DISTINCT query into a sub-query, then append the ORDER BY clause
          fetchQueryModel = fetchQueryModel.ConvertToSubQuery ("#fetch");

          var orderByClause = new OrderByClause ();
          foreach (var sortedPropertySpecification in sortExpression.SortedProperties)
            orderByClause.Orderings.Add (GetOrdering (fetchQueryModel, sortedPropertySpecification));

          fetchQueryModel.BodyClauses.Add (orderByClause);
        }

        var fetchQuery = CreateQuery (
            "<fetch query for " + fetchQueryModelBuilder.FetchRequest.RelationMember.Name + ">",
            fetchQueryModel,
            fetchQueryModelBuilder.CreateInnerBuilders(),
            QueryType.Collection,
            relationEndPointDefinition.GetOppositeClassDefinition());

        query.EagerFetchQueries.Add (relationEndPointDefinition, fetchQuery);
      }
    }

    private SortExpressionDefinition GetSortExpressionForRelation (IRelationEndPointDefinition relationEndPointDefinition)
    {
      var virtualEndPointDefinition = relationEndPointDefinition as VirtualRelationEndPointDefinition;
      if (virtualEndPointDefinition != null)
        return virtualEndPointDefinition.GetSortExpression ();
      else
        return null;
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

      var propertyName = MappingConfiguration.Current.NameResolver.GetPropertyName (PropertyInfoAdapter.Create(propertyInfo));
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

    // TODO Review 2800: Write an integration test similar to the one in the bug report (i.e., with a Cast to an interface). The test should now work.
    private void CheckQueryReturnsDomainObject (SqlStatement sqlStatement, QueryModel queryModel)
    {
      var expression = sqlStatement.SelectProjection;
      while (expression is UnaryExpression)
        expression = ((UnaryExpression) expression).Operand;
      if (expression is SqlEntityExpression)
        return;

      string message = string.Format (
          "This query provider does not support the given query ('{0}'). "
          + "re-store only supports queries selecting a scalar value, a single DomainObject, or a collection of DomainObjects.",
          queryModel);

      if (expression is SqlGroupingSelectExpression)
        message += " GroupBy must be executed in memory, for example by issuing AsEnumerable() before performing the grouping operation.";

      throw new NotSupportedException (message);
    }

    /// <summary>
    /// Transforms and resolves <see cref="QueryModel"/> to build a <see cref="SqlStatement"/> which represents an AST to generate query text.
    /// </summary>
    /// <param name="queryModel">The <see cref="QueryModel"/> which should be transformed.</param>
    /// <returns>the generated <see cref="SqlStatement"/></returns>
    protected virtual SqlStatement TransformAndResolveQueryModel (QueryModel queryModel)
    {
      SqlStatement sqlStatement = _preparationStage.PrepareSqlStatement (queryModel, null);
      return _resolutionStage.ResolveSqlStatement (sqlStatement, _mappingResolutionContext);
    }

    /// <summary>
    /// Creates a SQL command based on a given <see cref="SqlStatement"/>.
    /// </summary>
    /// <param name="sqlStatement">The <see cref="SqlStatement"/> a SQL query has to be generated for.</param>
    /// <returns><see cref="SqlCommand"/> which represents the sql query.</returns>
    protected virtual SqlCommandData CreateSqlCommand (SqlStatement sqlStatement)
    {
      var commandBuilder = new SqlCommandBuilder();
      _generationStage.GenerateTextForOuterSqlStatement (commandBuilder, sqlStatement);
      return commandBuilder.GetCommand();
    }

    private Ordering GetOrdering (QueryModel queryModel, SortedPropertySpecification sortedPropertySpecification)
    {
      var propertyInfo = (PropertyInfo) _typeConversionProvider.Convert (
          sortedPropertySpecification.PropertyDefinition.PropertyInfo.GetType (),
          typeof (PropertyInfo),
          sortedPropertySpecification.PropertyDefinition.PropertyInfo);

      var memberExpression = Expression.MakeMemberAccess (queryModel.SelectClause.Selector, propertyInfo);
      var orderingDirection = sortedPropertySpecification.Order == SortOrder.Ascending ? OrderingDirection.Asc : OrderingDirection.Desc;
      return new Ordering (memberExpression, orderingDirection);
    }

    private static ICollection<FetchQueryModelBuilder> RemoveTrailingFetchRequests (QueryModel queryModel)
    {
      var result = new List<FetchQueryModelBuilder>();
      for (int i = queryModel.ResultOperators.Count - 1; i >= 0 && queryModel.ResultOperators[i] is FetchRequestBase; --i)
      {
        result.Add (new FetchQueryModelBuilder (queryModel.ResultOperators[i] as FetchRequestBase, queryModel, i));
        queryModel.ResultOperators.RemoveAt (i);
      }
      return result;
    }
  }
}