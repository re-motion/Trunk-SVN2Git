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
using Remotion.Data.Linq.Backend.DataObjectModel;
using Remotion.Data.Linq.Parsing;
using Remotion.Data.Linq.Backend.DetailParsing;
using Remotion.Data.Linq.Backend.DetailParsing.WhereConditionParsing;
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
    private static T GetTypedExpression<T> (object expression, string context)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      ArgumentUtility.CheckNotNull ("context", context);

      if (!(expression is T))
        throw new ParserException (typeof (T).Name, expression, context);
      else
        return (T) expression;
    }

    private static readonly MethodInfo s_genericContainsMethod =
        Data.Linq.ReflectionUtility.GetMethod (() => Queryable.Contains (null, (object) null)).GetGenericMethodDefinition();

// ReSharper disable PossibleNullReferenceException
    private static readonly MethodInfo s_containsObjectMethod =
        Data.Linq.ReflectionUtility.GetMethod (() => ((DomainObjectCollection) null).ContainsObject (null));
// ReSharper restore PossibleNullReferenceException

    private static readonly MethodInfo s_genericCreateQueryableMethod =
        Data.Linq.ReflectionUtility.GetMethod (() => QueryFactory.CreateLinqQuery<DomainObject>()).GetGenericMethodDefinition();

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

      var containsObjectCallExpression = GetTypedExpression<MethodCallExpression> (expression, "ContainsObject parser");

      SubQueryExpression subQueryExpression = CreateEquivalentSubQuery (
          containsObjectCallExpression, parseContext.QueryModel);
      MethodCallExpression containsExpression = CreateExpressionForContainsParser (subQueryExpression, containsObjectCallExpression.Arguments[0]);
      return _registry.GetParser (containsExpression).Parse (containsExpression, parseContext);
    }

    public SubQueryExpression CreateEquivalentSubQuery (MethodCallExpression containsObjectCallExpression, QueryModel parentQuery)
    {
      ArgumentUtility.CheckNotNull ("containsObjectCallExpression", containsObjectCallExpression);
      ArgumentUtility.CheckNotNull ("parentQuery", parentQuery);
      ArgumentUtility.CheckNotNull ("methodCallExpression", containsObjectCallExpression);
      ArgumentUtility.CheckNotNull ("parentQuery", parentQuery);

      Type collectionElementType = containsObjectCallExpression.Arguments[0].Type;
      var collectionExpression = GetTypedExpression<MemberExpression> (
          containsObjectCallExpression.Object, "object on which ContainsObject is called");
      var collectionProperty = GetTypedExpression<PropertyInfo> (collectionExpression.Member, "member on which ContainsObject is called");
      PropertyInfo foreignKeyProperty = GetForeignKeyProperty (collectionProperty);

      var mainFromClause = CreateFromClause (collectionElementType);
      var whereClause = CreateWhereClause (mainFromClause, foreignKeyProperty, collectionExpression.Expression);
      var selectClause = CreateSelectClause (mainFromClause);

      var queryModel1 = new QueryModel (mainFromClause, selectClause);
      queryModel1.BodyClauses.Add (whereClause);
      QueryModel queryModel = queryModel1;
      var subQuery = new SubQueryExpression (queryModel);
      return subQuery;
    }

    // from oi in QueryFactory.CreateLinqQuery<OrderItem>
    private MainFromClause CreateFromClause (Type containsParameterType)
    {
      string itemName = "<<generated>>" + Guid.NewGuid().ToString ("N");
      MethodInfo entityMethod = s_genericCreateQueryableMethod.MakeGenericMethod (containsParameterType);
      object queryable = entityMethod.Invoke (null, null);
      Expression querySource = Expression.Constant (queryable);

      return new MainFromClause (itemName, containsParameterType, querySource);
    }

    private WhereClause CreateWhereClause (MainFromClause fromClause, PropertyInfo foreignKeyProperty, Expression queriedObject)
    {
      Expression left = Expression.MakeMemberAccess (new QuerySourceReferenceExpression (fromClause), foreignKeyProperty);
      Expression right = queriedObject;
      BinaryExpression comparison = Expression.Equal (left, right);
      
      return new WhereClause (comparison);
    }

    private SelectClause CreateSelectClause (MainFromClause mainFromClause)
    {
      return new SelectClause (new QuerySourceReferenceExpression (mainFromClause));
    }

    public PropertyInfo GetForeignKeyProperty (PropertyInfo collectionProperty) // Order.OrderItems
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (collectionProperty.DeclaringType);
      string propertyName = MappingConfiguration.Current.NameResolver.GetPropertyName (collectionProperty);
      var collectionEndPoint = classDefinition.GetMandatoryRelationEndPointDefinition (propertyName); // Order.OrderItems
      var foreignKeyEndPoint = collectionEndPoint.GetOppositeEndPointDefinition(); // OrderItem.Order

      return MappingConfiguration.Current.NameResolver.GetProperty (foreignKeyEndPoint.ClassDefinition.ClassType, foreignKeyEndPoint.PropertyName);
    }

    public MethodCallExpression CreateExpressionForContainsParser (SubQueryExpression subQueryExpression, Expression queryParameterExpression)
    {
      var concreteContainsObjectMethod = s_genericContainsMethod.MakeGenericMethod (queryParameterExpression.Type);
      return Expression.Call (concreteContainsObjectMethod, subQueryExpression, queryParameterExpression);
    }
  }
}
