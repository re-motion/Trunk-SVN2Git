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
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.DataObjectModel;
using Remotion.Data.Linq.Parsing;
using Remotion.Data.Linq.Parsing.Details;
using Remotion.Data.Linq.Parsing.Details.WhereConditionParsing;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  /// Parses expressions of the form <code>where o.OrderItems.ContainsObject (myOrderItem)</code>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This parser parses parts of where conditions that contains calls to <see cref="DomainObjectCollection.ContainsObject"/>. It does so by
  /// constructing an equivalent subquery.
  /// </para>
  /// <para>
  /// source: where o.OrderItems.ContainsObject (myOrderItem)
  /// </para>
  /// <para>
  /// transformed: where (from oi in QueryFactory.CreateLinqQuery[OrderItem] () where oi.Order == o select oi).Contains (myOrderItem)
  /// </para>
  /// </remarks>
  public class ContainsObjectParser : IWhereConditionParser
  {
    private static readonly MethodInfo s_genericContainsMethod =
        ParserUtility.GetMethod (() => Queryable.Contains (null, (object) null)).GetGenericMethodDefinition();

// ReSharper disable PossibleNullReferenceException
    private static readonly MethodInfo s_containsObjectMethod =
        ParserUtility.GetMethod (() => ((DomainObjectCollection) null).ContainsObject (null));
// ReSharper restore PossibleNullReferenceException

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
      var methodCallExpression = expression as MethodCallExpression;
      return methodCallExpression != null && methodCallExpression.Method == s_containsObjectMethod;
    }

    public ICriterion Parse (Expression expression, ParseContext parseContext)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      ArgumentUtility.CheckNotNull ("fieldDescriptors", parseContext);

      var containsObjectCallExpression = ParserUtility.GetTypedExpression<MethodCallExpression> (expression, "ContainsObject parser");

      SubQueryExpression subQueryExpression = CreateEquivalentSubQuery (
          containsObjectCallExpression, parseContext.QueryModel);
      MethodCallExpression containsExpression = CreateExpressionForContainsParser (subQueryExpression, containsObjectCallExpression.Arguments[0]);
      return _registry.GetParser (containsExpression).Parse (containsExpression, parseContext);
    }

    public SubQueryExpression CreateEquivalentSubQuery (MethodCallExpression containsObjectCallExpression, QueryModel parentQuery)
    {
      ArgumentUtility.CheckNotNull ("containsObjectCallExpression", containsObjectCallExpression);
      ArgumentUtility.CheckNotNull ("parentQuery", parentQuery);

      QueryModel queryModel = CreateQueryModel (containsObjectCallExpression, parentQuery);
      var subQuery = new SubQueryExpression (queryModel);
      return subQuery;
    }

    public QueryModel CreateQueryModel (MethodCallExpression methodCallExpression, QueryModel parentQuery)
    {
      ArgumentUtility.CheckNotNull ("methodCallExpression", methodCallExpression);
      ArgumentUtility.CheckNotNull ("parentQuery", parentQuery);

      Type collectionElementType = methodCallExpression.Arguments[0].Type;
      var collectionExpression = ParserUtility.GetTypedExpression<MemberExpression> (
          methodCallExpression.Object, "object on which ContainsObject is called");
      var collectionProperty = ParserUtility.GetTypedExpression<PropertyInfo> (collectionExpression.Member, "member on which ContainsObject is called");
      PropertyInfo foreignKeyProperty = GetForeignKeyProperty (collectionProperty);

      MainFromClause mainFromClause = CreateFromClause (collectionElementType);
      WhereClause whereClause = CreateWhereClause (mainFromClause, foreignKeyProperty, collectionExpression.Expression);
      SelectClause selectClause = CreateSelectClause (whereClause, mainFromClause);

      var queryModel = new QueryModel (typeof (IQueryable<>).MakeGenericType (collectionElementType), mainFromClause, selectClause);
      queryModel.BodyClauses.Add (whereClause);

      return queryModel;
    }

    // from oi in QueryFactory.CreateLinqQuery<OrderItem>
    public MainFromClause CreateFromClause (Type containsParameterType)
    {
      ArgumentUtility.CheckNotNull ("containsParameterType", containsParameterType);
      string itemName = "<<generated>>" + Guid.NewGuid().ToString ("N");

      MethodInfo entityMethod = s_genericCreateQueryableMethod.MakeGenericMethod (containsParameterType);
      object queryable = entityMethod.Invoke (null, null);
      Expression querySource = Expression.Constant (queryable);

      return new MainFromClause (itemName, containsParameterType, querySource);
    }

    public WhereClause CreateWhereClause (MainFromClause fromClause, PropertyInfo foreignKeyProperty, Expression queriedObject)
    {
      ArgumentUtility.CheckNotNull ("fromClause", fromClause);
      ArgumentUtility.CheckNotNull ("foreignKeyProperty", foreignKeyProperty);
      ArgumentUtility.CheckNotNull ("queriedObject", queriedObject);

      Expression left = Expression.MakeMemberAccess (new QuerySourceReferenceExpression (fromClause), foreignKeyProperty);
      Expression right = queriedObject;
      BinaryExpression comparison = Expression.Equal (left, right);
      
      return new WhereClause (fromClause, comparison);
    }

    public SelectClause CreateSelectClause (WhereClause whereClause, MainFromClause mainFromClause)
    {
      ArgumentUtility.CheckNotNull ("whereClause", whereClause);
      ArgumentUtility.CheckNotNull ("mainFromClause", mainFromClause);

      return new SelectClause (whereClause, new QuerySourceReferenceExpression (mainFromClause));
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
