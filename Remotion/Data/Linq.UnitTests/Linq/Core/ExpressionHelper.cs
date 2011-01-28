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
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.Parsing.ExpressionTreeVisitors;
using Remotion.Data.Linq.Parsing.Structure;
using Remotion.Data.Linq.UnitTests.Linq.Core.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.Linq.UnitTests.Linq.Core
{
  public static class ExpressionHelper
  {
    private static readonly IQueryExecutor s_executor = CreateExecutor();

    public static Expression CreateExpression ()
    {
      return CreateNewIntArrayExpression();
    }

    public static LambdaExpression CreateLambdaExpression ()
    {
      return Expression.Lambda (Expression.Constant (0));
    }

    public static Expression<Func<TSource, TResult>> CreateLambdaExpression<TSource, TResult> (Expression<Func<TSource, TResult>> expression)
    {
      return expression;
    }

    public static Expression<Func<TSource1, TSource2, TResult>> CreateLambdaExpression<TSource1, TSource2, TResult> (Expression<Func<TSource1, TSource2, TResult>> expression)
    {
      return expression;
    }

    public static Expression<Func<TSource1, TSource2, TSource3, TResult>> CreateLambdaExpression<TSource1, TSource2, TSource3, TResult> (Expression<Func<TSource1, TSource2, TSource3, TResult>> expression)
    {
      return expression;
    }

    public static Expression CreateNewIntArrayExpression ()
    {
      return Expression.NewArrayInit (typeof (int));
    }

    public static ParameterExpression CreateParameterExpression ()
    {
      return CreateParameterExpression ("i");
    }

    public static ParameterExpression CreateParameterExpression (string identifier)
    {
      return Expression.Parameter (typeof (int), identifier);
    }

    public static JoinClause CreateJoinClause ()
    {
      Expression innerSequence = CreateExpression ();
      Expression outerKeySelector = CreateExpression ();
      Expression innerKeySelector = CreateExpression ();

      return new JoinClause ("x", typeof(Cook), innerSequence, outerKeySelector, innerKeySelector);
    }

    public static GroupJoinClause CreateGroupJoinClause ()
    {
      return CreateGroupJoinClause (CreateJoinClause ());
    }

    public static GroupJoinClause CreateGroupJoinClause (JoinClause joinClause)
    {
      return new GroupJoinClause ("xs", typeof (IEnumerable<Cook>), joinClause);
    }

    public static QueryModel CreateQueryModel (MainFromClause mainFromClause)
    {
      var selectClause = new SelectClause (new QuerySourceReferenceExpression (mainFromClause));
      return new QueryModel (mainFromClause, selectClause);
    }

    public static QueryModel CreateQueryModel_Cook ()
    {
      return CreateQueryModel (CreateMainFromClause_Int("s", typeof (Cook), CreateCookQueryable()));
    }

    public static QueryModel CreateQueryModel_Int ()
    {
      return CreateQueryModel (CreateMainFromClause_Int ("i", typeof (int), CreateIntQueryable ()));
    }

    public static MainFromClause CreateMainFromClause_Int ()
    {
      IQueryable querySource = CreateIntQueryable (); 
      return CreateMainFromClause_Int("main", typeof (int), querySource);
    }

    public static MainFromClause CreateMainFromClause_Cook ()
    {
      return CreateMainFromClause_Int ("s", typeof (Cook), CreateCookQueryable());
    }

    public static MainFromClause CreateMainFromClause_Bool ()
    {
      return CreateMainFromClause_Int ("s", typeof (bool), CreateCookQueryable ());
    }

    public static MainFromClause CreateMainFromClause_Kitchen ()
    {
      return CreateMainFromClause_Int ("sd", typeof (Kitchen), CreateKitchenQueryable ());
    }

    public static MainFromClause CreateMainFromClause_Detail_Detail ()
    {
      return CreateMainFromClause_Int ("sdd", typeof (Company), CreateCompanyQueryable());
    }

    public static AdditionalFromClause CreateAdditionalFromClause ()
    {
      return CreateAdditionalFromClause ("additional", typeof (int));
    }

    public static AdditionalFromClause CreateAdditionalFromClause (string itemName, Type itemType)
    {
      return new AdditionalFromClause (itemName, itemType, CreateExpression ());
    }

    public static GroupResultOperator CreateGroupResultOperator ()
    {
      MainFromClause fromClause1 = CreateMainFromClause_Int ("i", typeof (int), CreateIntQueryable());
      MainFromClause fromClause2 = CreateMainFromClause_Int ("j", typeof (int), CreateIntQueryable());

      var keySelector = Resolve<int, string> (fromClause2, j => (j % 3).ToString());
      var elementSelector = Resolve<int, string> (fromClause1, i => i.ToString ());

      return new GroupResultOperator ("groupings", keySelector, elementSelector);
    }

    public static Ordering CreateOrdering ()
    {
      return new Ordering (CreateExpression (), OrderingDirection.Asc);
    }

    public static OrderByClause CreateOrderByClause()
    {
      return new OrderByClause ();
    }

    public static SelectClause CreateSelectClause ()
    {
      var selector = Expression.Constant (0);
      return new SelectClause (selector);
    }

    public static SelectClause CreateSelectClause (MainFromClause referencedClause)
    {
      return new SelectClause (new QuerySourceReferenceExpression (referencedClause));
    }

    public static MethodCallExpression CreateMethodCallExpression (IQueryable<Cook> query)
    {
      var methodInfo = ReflectionUtility.GetMethod (() => query.Count ());
      return Expression.Call (methodInfo, query.Expression);
    }

    public static MethodCallExpression CreateMethodCallExpression ()
    {
      return CreateMethodCallExpression (CreateCookQueryable ());
    }

    public static WhereClause CreateWhereClause ()
    {
      var predicate = Expression.MakeBinary (ExpressionType.Equal, Expression.Constant (1), Expression.Constant (2));
      return new WhereClause (predicate);
    }

    public static IClause CreateClause()
    {
      return CreateMainFromClause_Int();
    }

    public static IQueryable<int> CreateIntQueryable ()
    {
      return new TestQueryable<int> (s_executor);
    }

    public static IQueryable<Cook> CreateCookQueryable()
    {
      return CreateCookQueryable (s_executor);
    }

    public static IQueryable<Chef> CreateChefQueryable ()
    {
      return CreateChefQueryable (s_executor);
    }
        
    public static IQueryable<Cook> CreateCookQueryable (IQueryExecutor executor)
    {
      return new TestQueryable<Cook> (executor);
    }

    public static IQueryable<Chef> CreateChefQueryable (IQueryExecutor executor)
    {
      return new TestQueryable<Chef> (executor);
    }

    public static IQueryable<Kitchen> CreateKitchenQueryable()
    {
      return CreateKitchenQueryable (s_executor);
    }

    public static IQueryable<Kitchen> CreateKitchenQueryable (IQueryExecutor executor)
    {
      return new TestQueryable<Kitchen> (executor);
    }

    public static IQueryable<Company> CreateCompanyQueryable ()
    {
      return CreateCompanyQueryable (s_executor);
    }

    public static IQueryable<Company> CreateCompanyQueryable (IQueryExecutor executor)
    {
      return new TestQueryable<Company> (executor);
    }

    public static  IQueryable<Restaurant> CreateRestaurantQueryable ()
    {
      return CreateRestaurantQueryable (s_executor);
    }

    public static IQueryable<Restaurant> CreateRestaurantQueryable( IQueryExecutor executor)
    {
      return new TestQueryable<Restaurant> (executor);
    }

    public static IQueryExecutor CreateExecutor()
    {
      return new StubQueryExecutor();
    }

    public static object ExecuteLambda (LambdaExpression lambdaExpression, params object[] args)
    {
      return lambdaExpression.Compile().DynamicInvoke (args);
    }

    public static QueryModel ParseQuery<T> (IQueryable<T> query)
    {
      return ParseQuery (query.Expression);
    }

    public static QueryModel ParseQuery (Expression queryExpression)
    {
      var parser = QueryParser.CreateDefault();
      return parser.GetParsedQuery (queryExpression);
    }

    public static MainFromClause CreateMainFromClause_Int (string itemName, Type itemType, IQueryable querySource)
    {
      return new MainFromClause (itemName, itemType, Expression.Constant (querySource));
    }

    public static Expression MakeExpression<TRet> (Expression<Func<TRet>> expression)
    {
      return expression.Body;
    }

    public static Expression MakeExpression<TArg, TRet> (Expression<Func<TArg, TRet>> expression)
    {
      return expression.Body;
    }

    public static MemberInfo GetMember<T> (Expression<Func<T, object>> memberAccess)
    {
      Expression expression = memberAccess.Body;
      while (expression is UnaryExpression)
        expression = ((UnaryExpression) expression).Operand; // strip casts
      return ((MemberExpression) expression).Member;
    }

    public static ResultOperatorBase CreateResultOperator ()
    {
      return new DistinctResultOperator ();
    }

    public static Expression Resolve<TParameter, TResult> (IQuerySource sourceToReference, Expression<Func<TParameter, TResult>> expressionToBeResolved)
    {
      return ReplacingExpressionTreeVisitor.Replace (expressionToBeResolved.Parameters[0], new QuerySourceReferenceExpression (sourceToReference), expressionToBeResolved.Body);
    }

    public static Expression Resolve<TParameter1, TParameter2, TResult> (IQuerySource sourceToReference1, IQuerySource sourceToReference2, Expression<Func<TParameter1, TParameter2, TResult>> expressionToBeResolved)
    {
      var expressionMapping = new Dictionary<Expression, Expression> (2)
                              {
                                  { expressionToBeResolved.Parameters[0], new QuerySourceReferenceExpression (sourceToReference1) },
                                  { expressionToBeResolved.Parameters[1], new QuerySourceReferenceExpression (sourceToReference2) }
                              };
      var result = MultiReplacingExpressionTreeVisitor.Replace (expressionMapping, expressionToBeResolved.Body);
      return result;
    }

    public static Expression Resolve<TParameter1, TParameter2, TParameter3, TResult> (IQuerySource sourceToReference1, IQuerySource sourceToReference2, IQuerySource sourceToReference3, Expression<Func<TParameter1, TParameter2, TParameter3, TResult>> expressionToBeResolved)
    {
      var expressionMapping = new Dictionary<Expression, Expression> (3)
                              {
                                  { expressionToBeResolved.Parameters[0], new QuerySourceReferenceExpression (sourceToReference1) },
                                  { expressionToBeResolved.Parameters[1], new QuerySourceReferenceExpression (sourceToReference2) },
                                  { expressionToBeResolved.Parameters[2], new QuerySourceReferenceExpression (sourceToReference3) },
                              };
      var result = MultiReplacingExpressionTreeVisitor.Replace (expressionMapping, expressionToBeResolved.Body);
      return result;
    }

    public static Expression ResolveLambdaParameter<TParameter1, TParameter2, TResult> (
        int parameterToResolveIndex, 
        IQuerySource source, 
        Expression<Func<TParameter1, TParameter2, TResult>> expressionToBeResolved)
    {
      var parameterToResolve = expressionToBeResolved.Parameters[parameterToResolveIndex];

      var resolvedBody = ReplacingExpressionTreeVisitor.Replace (
          parameterToResolve, 
          new QuerySourceReferenceExpression (source),
          expressionToBeResolved.Body);

      var remainingParameters = new List<ParameterExpression> (expressionToBeResolved.Parameters);
      remainingParameters.Remove (parameterToResolve);

      return Expression.Lambda (resolvedBody, remainingParameters.ToArray());
    }
  }
}