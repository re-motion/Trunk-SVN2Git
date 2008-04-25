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
