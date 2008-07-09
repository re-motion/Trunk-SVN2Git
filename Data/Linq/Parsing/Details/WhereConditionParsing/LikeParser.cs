/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Collections.Generic;
using System.Linq.Expressions;
using Remotion.Data.Linq.DataObjectModel;
using Remotion.Utilities;

namespace Remotion.Data.Linq.Parsing.Details.WhereConditionParsing
{
  public class LikeParser : IWhereConditionParser
  {
    private readonly WhereConditionParserRegistry _parserRegistry;

    public LikeParser (WhereConditionParserRegistry parserRegistry)
    {
      ArgumentUtility.CheckNotNull ("parserRegistry", parserRegistry);
      _parserRegistry = parserRegistry;
    }

    public ICriterion Parse (MethodCallExpression methodCallExpression, ParseContext parseContext)
    {
      if (methodCallExpression.Method.Name == "StartsWith")
      {
        ParserUtility.CheckNumberOfArguments (methodCallExpression, "StartsWith", 1, parseContext.ExpressionTreeRoot);
        ParserUtility.CheckParameterType<ConstantExpression> (methodCallExpression, "StartsWith", 0, parseContext.ExpressionTreeRoot);
        return CreateLike (methodCallExpression, ((ConstantExpression) methodCallExpression.Arguments[0]).Value + "%", parseContext);
      }
      else if (methodCallExpression.Method.Name == "EndsWith")
      {
        ParserUtility.CheckNumberOfArguments (methodCallExpression, "EndsWith", 1, parseContext.ExpressionTreeRoot);
        ParserUtility.CheckParameterType<ConstantExpression> (methodCallExpression, "EndsWith", 0, parseContext.ExpressionTreeRoot);
        return CreateLike (methodCallExpression, "%" + ((ConstantExpression) methodCallExpression.Arguments[0]).Value, parseContext);
      }
      else if (methodCallExpression.Method.Name == "Contains" && !methodCallExpression.Method.IsGenericMethod)
      {
        ParserUtility.CheckNumberOfArguments (methodCallExpression, "Contains", 1, parseContext.ExpressionTreeRoot);
        ParserUtility.CheckParameterType<ConstantExpression> (methodCallExpression, "Contains", 0, parseContext.ExpressionTreeRoot);
        return CreateLike (methodCallExpression, "%" + ((ConstantExpression) methodCallExpression.Arguments[0]).Value + "%", parseContext);
      }
      throw ParserUtility.CreateParserException ("StartsWith, EndsWith, Contains with no expression", methodCallExpression.Method.Name,
          "method call expression in where condition", parseContext.ExpressionTreeRoot);
    }

    ICriterion IWhereConditionParser.Parse (Expression expression, ParseContext parseContext)
    {
      return Parse ((MethodCallExpression) expression, parseContext);
    }

    public bool CanParse (Expression expression)
    {
      var methodCallExpression = expression as MethodCallExpression;
      if (methodCallExpression != null)
      {
        if (methodCallExpression.Method.Name == "StartsWith" ||
          methodCallExpression.Method.Name == "EndsWith" ||
          (methodCallExpression.Method.Name == "Contains" && !methodCallExpression.Method.IsGenericMethod))
          return true;
      }
      return false;
    }

    private BinaryCondition CreateLike (MethodCallExpression expression, string pattern, ParseContext parseContext)
    {
      return new BinaryCondition (_parserRegistry.GetParser (expression.Object).Parse (expression.Object, parseContext), new Constant (pattern), BinaryCondition.ConditionKind.Like);
    }
  }
}
