/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.CodeDom;

namespace Remotion.Development.CodeDom
{

public class CodeOperatorChainExpression: CodeBinaryOperatorExpression
{
  public CodeOperatorChainExpression (CodeBinaryOperatorType binaryOperator, params CodeExpression[] expressions)
    : base ()
  {
    CodeBinaryOperatorExpression binaryOpEx = this;
    for (int i = 0; i < (expressions.Length - 2); ++i)
    {
      CodeExpression condition = expressions[i];
      binaryOpEx.Left = condition;
      binaryOpEx.Operator = binaryOperator;
      binaryOpEx.Right = new CodeBinaryOperatorExpression ();
      binaryOpEx = (CodeBinaryOperatorExpression) binaryOpEx.Right;
    }
    binaryOpEx.Left = expressions[expressions.Length - 2];
    binaryOpEx.Operator = binaryOperator;
    binaryOpEx.Right = expressions[expressions.Length - 1];
  }
}

public class CodeBooleanAndExpression: CodeOperatorChainExpression
{
  public CodeBooleanAndExpression (params CodeExpression[] conditions)
    : base (CodeBinaryOperatorType.BooleanAnd, conditions)
  {
  }
}

public class CodeBooleanOrExpression: CodeOperatorChainExpression
{
  public CodeBooleanOrExpression (params CodeExpression[] conditions)
    : base (CodeBinaryOperatorType.BooleanOr, conditions)
  {
  }
}

}
