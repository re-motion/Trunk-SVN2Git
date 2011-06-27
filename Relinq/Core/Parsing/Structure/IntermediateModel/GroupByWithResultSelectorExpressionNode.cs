// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-linq is free software; you can redistribute it and/or modify it under 
// the terms of the GNU Lesser General Public License as published by the 
// Free Software Foundation; either version 2.1 of the License, 
// or (at your option) any later version.
// 
// re-linq is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-linq; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.Parsing.ExpressionTreeVisitors;
using Remotion.Linq.Utilities;

namespace Remotion.Linq.Parsing.Structure.IntermediateModel
{
  /// <summary>
  /// Represents a <see cref="MethodCallExpression"/> for the different <see cref="O:Queryable.GroupBy"/> 
  /// overloads that do take a result selector. The overloads without a result selector are represented by 
  /// <see cref="GroupByExpressionNode"/>.
  /// It is generated by <see cref="ExpressionTreeParser"/> when an <see cref="Expression"/> tree is parsed.
  /// </summary>
  /// <remarks>
  /// The GroupBy overloads with result selector are parsed as if they were a <see cref="SelectExpressionNode"/> following a 
  /// <see cref="GroupByExpressionNode"/>:
  /// <code>
  /// x.GroupBy (k => key, e => element, (k, g) => result)
  /// </code>
  /// is therefore equivalent to:
  /// <code>
  /// c.GroupBy (k => key, e => element).Select (grouping => resultSub)
  /// </code>
  /// where resultSub is the same as result with k and g substituted with grouping.Key and grouping, respectively.
  /// </remarks>
  public class GroupByWithResultSelectorExpressionNode : SelectExpressionNode, IQuerySourceExpressionNode
  {
    public new static readonly MethodInfo[] SupportedMethods = new[]
                                                           {
                                                               GetSupportedMethod (() => Queryable.GroupBy<object, object, object, object> (null, o => null, o => null, (k, g) => null)),
                                                               GetSupportedMethod (() => Queryable.GroupBy<object, object, object> (null, o => null, (k, g) => null)),
                                                               GetSupportedMethod (() => Enumerable.GroupBy<object, object, object, object> (null, o => null, o => null, (k, g) => null)),
                                                               GetSupportedMethod (() => Enumerable.GroupBy<object, object, object> (null, o => null, (k, g) => null)),
                                                           };

    public GroupByWithResultSelectorExpressionNode (
        MethodCallExpressionParseInfo parseInfo, 
        LambdaExpression keySelector, 
        LambdaExpression elementSelectorOrResultSelector, 
        LambdaExpression resultSelectorOrNull)
      : base (
          CreateParseInfoWithGroupNode (
              parseInfo,
              ArgumentUtility.CheckNotNull ("keySelector", keySelector),
              ArgumentUtility.CheckNotNull ("elementSelectorOrResultSelector", elementSelectorOrResultSelector),
              resultSelectorOrNull), 
          CreateSelectorForSelectNode (
              keySelector,
              elementSelectorOrResultSelector,
              resultSelectorOrNull))
    {
    }

  
    private static MethodCallExpressionParseInfo CreateParseInfoWithGroupNode (
        MethodCallExpressionParseInfo parseInfo, 
        LambdaExpression keySelector, 
        LambdaExpression elementSelectorOrResultSelector, 
        LambdaExpression resultSelectorOrNull)
    {
      var optionalElementSelector = GetOptionalElementSelector (elementSelectorOrResultSelector, resultSelectorOrNull);

      var sourceItemType = ReflectionUtility.GetItemTypeOfIEnumerable (
          parseInfo.ParsedExpression.Arguments[0].Type, 
          "parseInfo.ParsedExpression.Arguments[0].Type");

      MethodCallExpression simulatedGroupByCallWithoutResultSelector;
      if (optionalElementSelector == null)
      {
        simulatedGroupByCallWithoutResultSelector = Expression.Call (
            typeof (Enumerable),
            "GroupBy",
            new[] { sourceItemType, keySelector.Body.Type },
            parseInfo.ParsedExpression.Arguments[0],
            keySelector);
      }
      else
      {
        simulatedGroupByCallWithoutResultSelector = Expression.Call (
            typeof (Enumerable),
            "GroupBy",
            new[] { sourceItemType, keySelector.Body.Type, optionalElementSelector.Body.Type },
            parseInfo.ParsedExpression.Arguments[0],
            keySelector,
            optionalElementSelector);
      }

      var simulatedParseInfo = new MethodCallExpressionParseInfo(parseInfo.AssociatedIdentifier, parseInfo.Source, simulatedGroupByCallWithoutResultSelector);
      var groupBySourceNode = new GroupByExpressionNode (simulatedParseInfo, keySelector, optionalElementSelector);
      return new MethodCallExpressionParseInfo (parseInfo.AssociatedIdentifier, groupBySourceNode, parseInfo.ParsedExpression);
    }

    private static LambdaExpression CreateSelectorForSelectNode (
        LambdaExpression keySelector, 
        LambdaExpression elementSelectorOrResultSelector, 
        LambdaExpression resultSelectorOrNull)
    {
      var resultSelector = GetResultSelector (elementSelectorOrResultSelector, resultSelectorOrNull);
      var optionalElementSelector = GetOptionalElementSelector (elementSelectorOrResultSelector, resultSelectorOrNull);

      // If there is an element selector, the element type will be that returned by the element selector. Otherwise, it will be the type flowing into
      // the key selector.
      var elementType = optionalElementSelector != null ? optionalElementSelector.Body.Type : keySelector.Parameters[0].Type;
      var groupingType = typeof (IGrouping<,>).MakeGenericType (keySelector.Body.Type, elementType);
      var keyProperty = groupingType.GetProperty ("Key");

      var groupParameter = Expression.Parameter (groupingType, "group");
      var keyExpression = Expression.MakeMemberAccess (groupParameter, keyProperty);

      var expressionMapping = new Dictionary<Expression, Expression> (2)
                              {
                                  { resultSelector.Parameters[1], groupParameter },
                                  { resultSelector.Parameters[0], keyExpression }
                              };
      var bodyWithGroupingAndKeyReplaced = MultiReplacingExpressionTreeVisitor.Replace (expressionMapping, resultSelector.Body);
      return Expression.Lambda (bodyWithGroupingAndKeyReplaced, groupParameter);
    }

    private static LambdaExpression GetOptionalElementSelector (LambdaExpression elementSelectorOrResultSelector, LambdaExpression resultSelectorOrNull)
    {
      return resultSelectorOrNull == null ? null : elementSelectorOrResultSelector;
    }

    private static LambdaExpression GetResultSelector (LambdaExpression elementSelectorOrResultSelector, LambdaExpression resultSelectorOrNull)
    {
      if (resultSelectorOrNull != null)
      {
        if (resultSelectorOrNull.Parameters.Count != 2)
          throw new ArgumentException ("ResultSelector must have exactly two parameters.", "resultSelectorOrNull");

        return resultSelectorOrNull;
      }
      else
      {
        if (elementSelectorOrResultSelector.Parameters.Count != 2)
          throw new ArgumentException ("ResultSelector must have exactly two parameters.", "elementSelectorOrResultSelector");
        return elementSelectorOrResultSelector;
      }
    }
  }
}
