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
  public class ContainsFullTextParser : IWhereConditionParser
  {
    private readonly WhereConditionParserRegistry _parserRegistry;

    public ContainsFullTextParser (WhereConditionParserRegistry parserRegistry)
    {
      ArgumentUtility.CheckNotNull ("parserRegistry", parserRegistry);
      _parserRegistry = parserRegistry;
    }

    public ICriterion Parse (MethodCallExpression methodCallExpression, ParseContext parseContext)
    {
      return CreateContainsFulltext (methodCallExpression, (string) ((ConstantExpression) methodCallExpression.Arguments[1]).Value, parseContext);
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
        if (methodCallExpression.Method.Name == "ContainsFulltext")
          return true;
      }
      return false;
    }

    private BinaryCondition CreateContainsFulltext (MethodCallExpression expression, string pattern, ParseContext parseContext)
    {
      return new BinaryCondition (_parserRegistry.GetParser (expression.Arguments[0]).Parse (expression.Arguments[0], parseContext), new Constant (pattern), BinaryCondition.ConditionKind.ContainsFulltext);
    }

  }
}
