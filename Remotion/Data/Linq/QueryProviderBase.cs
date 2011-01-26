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
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Data.Linq.Parsing.ExpressionTreeVisitors.Transformation;
using Remotion.Data.Linq.Parsing.Structure;
using Remotion.Data.Linq.Parsing.Structure.ExpressionTreeProcessingSteps;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq
{
  /// <summary>
  /// Provides a default implementation of <see cref="IQueryProvider"/> that executes queries (subclasses of <see cref="QueryableBase{T}"/>) by
  /// first parsing them into a <see cref="QueryModel"/> and then passing that to a given implementation of <see cref="IQueryExecutor"/>.
  /// Usually, <see cref="DefaultQueryProvider"/> should be used unless <see cref="CreateQuery{T}"/> must be manually implemented.
  /// </summary>
  public abstract class QueryProviderBase : IQueryProvider
  {
    private readonly QueryParser _queryParser;

    private static readonly MethodInfo s_genericCreateQueryMethod =
        typeof (QueryProviderBase).GetMethods().Where (m => m.Name == "CreateQuery" && m.IsGenericMethod).Single();

    /// <summary>
    /// Initializes a new instance of <see cref="QueryProviderBase"/> using the default <see cref="MethodCallExpressionNodeTypeRegistry"/>.
    /// </summary>
    /// <param name="executor">The <see cref="IQueryExecutor"/> used to execute queries against a specific query backend.</param>
    protected QueryProviderBase (IQueryExecutor executor)
        : this (executor, MethodCallExpressionNodeTypeRegistry.CreateDefault())
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="QueryProviderBase"/> using a custom <see cref="MethodCallExpressionNodeTypeRegistry"/>. Use this
    /// constructor to specify a specific set of parsers to use when analyzing the query.
    /// </summary>
    /// <param name="executor">The <see cref="IQueryExecutor"/> used to execute queries against a specific query backend.</param>
    /// <param name="nodeTypeRegistry">The <see cref="MethodCallExpressionNodeTypeRegistry"/> containing the <see cref="MethodCallExpression"/>
    /// parsers that should be used when parsing queries.</param>
    protected QueryProviderBase (IQueryExecutor executor, MethodCallExpressionNodeTypeRegistry nodeTypeRegistry)
    {
      ArgumentUtility.CheckNotNull ("executor", executor);
      Executor = executor;
      // TODO 3693: Extract IQueryParser interface, inject here
      _queryParser = new QueryParser (new ExpressionTreeParser (nodeTypeRegistry, ExpressionTreeParser.CreateDefaultProcessingSteps ()));
    }

    /// <summary>
    /// Gets or sets the implementation of <see cref="IQueryExecutor"/> used to execute queries created via <see cref="CreateQuery{T}"/>.
    /// </summary>
    /// <value>The executor used to execute queries.</value>
    public IQueryExecutor Executor { get; private set; }

    [Obsolete ("This property has been replaced by the QueryParser property. Use QueryParser.ExpresionTreeParser instead. (1.13.92)")]
    public ExpressionTreeParser ExpressionTreeParser
    {
      get { return _queryParser.ExpressionTreeParser; }
    }

    /// <summary>
    /// Gets the <see cref="QueryParser"/> used by this <see cref="QueryProviderBase"/> to parse LINQ queries.
    /// </summary>
    /// <value>The query parser.</value>
    public QueryParser QueryParser
    {
      get { return _queryParser; }
    }

    /// <summary>
    /// Constructs an <see cref="IQueryable"/> object that can evaluate the query represented by a specified expression tree. This
    /// method delegates to <see cref="CreateQuery{T}"/>.
    /// </summary>
    /// <param name="expression">An expression tree that represents a LINQ query.</param>
    /// <returns>
    /// An <see cref="IQueryable"/> that can evaluate the query represented by the specified expression tree.
    /// </returns>
    public IQueryable CreateQuery (Expression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      Type elementType = ReflectionUtility.GetItemTypeOfIEnumerable (expression.Type, "expression");
      return (IQueryable) s_genericCreateQueryMethod.MakeGenericMethod (elementType).Invoke (this, new object[] { expression });
    }

    /// <summary>
    /// Constructs an <see cref="IQueryable{T}"/> object that can evaluate the query represented by a specified expression tree. This method is 
    /// called by the standard query operators defined by the <see cref="Queryable"/> class.
    /// </summary>
    /// <param name="expression">An expression tree that represents a LINQ query.</param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> that can evaluate the query represented by the specified expression tree.
    /// </returns>
    public abstract IQueryable<T> CreateQuery<T> (Expression expression);

    /// <summary>
    /// Executes the query defined by the specified expression by parsing it with a 
    /// <see cref="QueryParser"/> and then running it through the <see cref="Executor"/>.
    /// This method is invoked through the <see cref="IQueryProvider"/> interface by methods such as 
    /// <see cref="Queryable.First{TSource}(System.Linq.IQueryable{TSource})"/> and 
    /// <see cref="Queryable.Count{TSource}(System.Linq.IQueryable{TSource})"/>, and it's also invoked by <see cref="QueryableBase{T}"/>
    /// when the <see cref="IQueryable{T}"/> is enumerated.
    /// </summary>
    public virtual TResult Execute<TResult> (Expression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      var queryModel = GenerateQueryModel (expression);

      var result = queryModel.Execute (Executor);
      return (TResult) result.Value;
    }

    object IQueryProvider.Execute (Expression expression)
    {
      var executeMethod =
          typeof (QueryProviderBase).GetMethod ("Execute", BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod (expression.Type);
      return executeMethod.Invoke (this, new object[] { expression });
    }

    /// <summary>
    /// The method generates a <see cref="QueryModel"/>.
    /// </summary>
    /// <param name="expression">The query as expression chain.</param>
    /// <returns>a <see cref="QueryModel"/></returns>
    public virtual QueryModel GenerateQueryModel (Expression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      return _queryParser.GetParsedQuery (expression);
    }
  }
}
