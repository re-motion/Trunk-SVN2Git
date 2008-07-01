using System.Linq.Expressions;
using Remotion.Utilities;

namespace Remotion.Data.Linq.Parsing.Structure
{
  public class LetExpressionData : BodyExpressionDataBase<Expression>
  {
    public ParameterExpression Identifier { get; private set; }

    public LetExpressionData (ParameterExpression identifier, Expression expression)
        : base (expression)
    {
      ArgumentUtility.CheckNotNull ("identifier", identifier);
      ArgumentUtility.CheckNotNull ("expression", expression);

      Identifier = identifier;
    }
  }
}