// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.DataObjectModel;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Parsing;
using Remotion.Data.Linq.Parsing.Details;
using Remotion.Data.Linq.Parsing.Details.WhereConditionParsing;
using Remotion.Utilities;
using Remotion.Data.Linq;

namespace Remotion.Data.DomainObjects.Linq
{
  // source: 
  // transformed: where (from oi in QueryFactory.CreateLinqQuery<OrderItem> () where oi.Order == o select oi).Contains (myOrderItem)
  // SQL: WHERE @1 IN (SELECT [oi].[ID] FROM [OrderItem] [oi] WHERE (([oi].[OrderID] IS NULL AND [o].[ID] IS NULL) OR [oi].[OrderID] = [o].[ID]))
  /// <summary>
  /// Parses expressions of the form <code>where o.OrderItems.ContainsObject (myOrderItem)</code>.
  /// </summary>
  /// <remarks>
  /// This parser parses parts of where conditions that contains calls to <see cref="DomainObjectCollection.ContainsObject"/>. It does so by
  /// constructing an equivalent subquery
  /// </remarks>
  public class ContainsObjectParser : IWhereConditionParser
  {
    private static readonly MethodInfo s_genericContainsMethod =
        ParserUtility.GetMethod (() => Queryable.Contains (null, (object) null)).GetGenericMethodDefinition();
    private static readonly MethodInfo s_containsObjectMethod =
        ParserUtility.GetMethod (() => ((DomainObjectCollection) null).ContainsObject (null));
    private static readonly MethodInfo s_genericCreateQueryableMethod = 
        ParserUtility.GetMethod (() => QueryFactory.CreateLinqQuery<DomainObject>()).GetGenericMethodDefinition();

    private readonly WhereConditionParserRegistry _registry;

    public ContainsObjectParser (WhereConditionParserRegistry registry)
    {
      _registry = registry;
    }

    public bool CanParse (Expression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      MethodCallExpression methodCallExpression = expression as MethodCallExpression;
      return methodCallExpression != null && methodCallExpression.Method == s_containsObjectMethod;
    }

    public ICriterion Parse (Expression expression, ParseContext parseContext)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      ArgumentUtility.CheckNotNull ("fieldDescriptors", parseContext);

      MethodCallExpression containsObjectCallExpression = ParserUtility.GetTypedExpression<MethodCallExpression> (
          expression, "ContainsObject parser", parseContext.ExpressionTreeRoot);
      
      SubQueryExpression subQueryExpression = CreateEquivalentSubQuery (containsObjectCallExpression, parseContext.QueryModel,parseContext.ExpressionTreeRoot);
      MethodCallExpression containsExpression = CreateExpressionForContainsParser (subQueryExpression, containsObjectCallExpression.Arguments[0]);
      return _registry.GetParser (containsExpression).Parse (containsExpression, parseContext);
    }

    public SubQueryExpression CreateEquivalentSubQuery (MethodCallExpression containsObjectCallExpression, QueryModel parentQuery, Expression expressionTreeRoot)
    {
      ArgumentUtility.CheckNotNull ("containsObjectCallExpression", containsObjectCallExpression);
      ArgumentUtility.CheckNotNull ("parentQuery", parentQuery);

      QueryModel queryModel = CreateQueryModel (containsObjectCallExpression, parentQuery, expressionTreeRoot);
      SubQueryExpression subQuery = new SubQueryExpression (queryModel);
      return subQuery;
    }

    public QueryModel CreateQueryModel (MethodCallExpression methodCallExpression, QueryModel parentQuery, Expression expressionTreeRoot)
    {
      ArgumentUtility.CheckNotNull ("methodCallExpression", methodCallExpression);
      ArgumentUtility.CheckNotNull ("parentQuery", parentQuery);

      Type containsParameterType = methodCallExpression.Arguments[0].Type;
      MemberExpression collectionExpression = ParserUtility.GetTypedExpression<MemberExpression> (
          methodCallExpression.Object, "object on which ContainsObject is called", expressionTreeRoot);
      PropertyInfo collectionProperty = 
          ParserUtility.GetTypedExpression<PropertyInfo> (collectionExpression.Member, "member on which ContainsObject is called", methodCallExpression);
      PropertyInfo foreignKeyProperty = GetForeignKeyProperty (collectionProperty);

      MainFromClause mainFromClause = CreateFromClause(containsParameterType);
      WhereClause whereClause = CreateWhereClause (mainFromClause, foreignKeyProperty, collectionExpression.Expression);
      SelectClause selectClause = CreateSelectClause (whereClause, mainFromClause.Identifier);

      QueryModel queryModel = new QueryModel (typeof (IQueryable<>).MakeGenericType (containsParameterType), mainFromClause, selectClause);
      queryModel.AddBodyClause (whereClause);

      queryModel.SetParentQuery (parentQuery);
      return queryModel;
    }

    // from oi in QueryFactory.CreateLinqQuery<OrderItem>
    public MainFromClause CreateFromClause (Type containsParameterType)
    {
      ArgumentUtility.CheckNotNull ("containsParameterType", containsParameterType);
      string identifierName = "<<generated>>" + Guid.NewGuid().ToString("N");
      ParameterExpression identifier = Expression.Parameter (containsParameterType, identifierName);
    
      MethodInfo entityMethod = s_genericCreateQueryableMethod.MakeGenericMethod (containsParameterType);
      object queryable = entityMethod.Invoke(null, null);
      Expression querySource = Expression.Constant (queryable);

      return new MainFromClause (identifier, querySource);
    }

    public WhereClause CreateWhereClause (MainFromClause fromClause, PropertyInfo foreignKeyProperty, Expression queriedObject)
    {
      ArgumentUtility.CheckNotNull ("fromClause", fromClause);
      ArgumentUtility.CheckNotNull ("foreignKeyProperty", foreignKeyProperty);
      ArgumentUtility.CheckNotNull ("queriedObject", queriedObject);
      var comparison = CreateWhereComparison(fromClause.Identifier, foreignKeyProperty, queriedObject);
      return new WhereClause (fromClause, comparison);
    }

    public BinaryExpression CreateWhereComparison (ParameterExpression fromIdentifier, PropertyInfo foreignKeyProperty, Expression queriedObject)
    {
      ArgumentUtility.CheckNotNull ("fromIdentifier", fromIdentifier);
      ArgumentUtility.CheckNotNull ("foreignKeyProperty", foreignKeyProperty);
      ArgumentUtility.CheckNotNull ("queriedObject", queriedObject);
      Expression left = Expression.MakeMemberAccess (fromIdentifier, foreignKeyProperty);
      Expression right = queriedObject;
      BinaryExpression binaryExpression = Expression.Equal (left, right);
      return binaryExpression;
    }

    public SelectClause CreateSelectClause (WhereClause whereClause, ParameterExpression fromIdentifier)
    {
      ArgumentUtility.CheckNotNull ("whereClause", whereClause);
      ArgumentUtility.CheckNotNull ("fromIdentifier", fromIdentifier);

      return new SelectClause (whereClause, Expression.Lambda(fromIdentifier), fromIdentifier);
    }
    
    public PropertyInfo GetForeignKeyProperty (PropertyInfo collectionProperty) // Order.OrderItems
    {
      ArgumentUtility.CheckNotNull ("collectionProperty", collectionProperty);
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (collectionProperty.DeclaringType);
      string propertyName = MappingConfiguration.Current.NameResolver.GetPropertyName (collectionProperty);
      IRelationEndPointDefinition collectionEndPoint = classDefinition.GetMandatoryRelationEndPointDefinition (propertyName); // Order.OrderItems
      IRelationEndPointDefinition foreignKeyEndPoint = collectionEndPoint.GetOppositeEndPointDefinition(); // OrderItem.Order

      return MappingConfiguration.Current.NameResolver.GetProperty (foreignKeyEndPoint.ClassDefinition.ClassType, foreignKeyEndPoint.PropertyName);
      
    }

    public MethodCallExpression CreateExpressionForContainsParser (SubQueryExpression subQueryExpression, Expression queryParameterExpression)
    {
      MethodInfo concreteContainsObjectMethod = s_genericContainsMethod.MakeGenericMethod (queryParameterExpression.Type);
      return Expression.Call (concreteContainsObjectMethod, subQueryExpression, queryParameterExpression);
    }
  }
}
