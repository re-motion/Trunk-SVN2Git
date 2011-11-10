// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using System.Linq.Expressions;
using System.Text;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Linq.Parsing;
using Remotion.Linq.Utilities;

namespace Remotion.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions
{
  /// <summary>
  /// Represents a sql 'LIKE' command
  /// </summary>
  public class SqlLikeExpression : ExtensionExpression
  {
    private readonly Expression _left;
    private readonly Expression _right;
    private readonly Expression _escapeExpression;

    public static Expression Create (Expression searchedText, Expression unescapedSearchText, string likePrefix, string likePostfix)
    {
      var rightExpression = BuildRightExpression (unescapedSearchText, likePrefix, likePostfix);
      if (rightExpression == null)
        return Expression.Constant (false);

      return new SqlLikeExpression (searchedText, rightExpression, new SqlLiteralExpression (@"\"));
    }

    public static string Escape (string text, string escapeSequence)
    {
      ArgumentUtility.CheckNotNull ("text", text);
      ArgumentUtility.CheckNotNull ("escapeSequence", escapeSequence);

      var escapedString = new StringBuilder (text);
      escapedString.Replace (escapeSequence, escapeSequence + escapeSequence);
      escapedString.Replace ("%", string.Format (@"{0}%", escapeSequence));
      escapedString.Replace ("_", string.Format (@"{0}_", escapeSequence));
      escapedString.Replace ("[", string.Format (@"{0}[", escapeSequence));
      return escapedString.ToString ();
    }

    public static Expression Escape (Expression expression, string escapeSequence)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      ArgumentUtility.CheckNotNull ("escapeSequence", escapeSequence);

      SqlFunctionExpression result = Escape (expression, escapeSequence, escapeSequence);
      result = Escape (result, "%", escapeSequence);
      result = Escape (result, "_", escapeSequence);
      result = Escape (result, "[", escapeSequence);

      return result;
    }

    private static SqlFunctionExpression Escape (Expression expression, string replacedSequence, string escapeSequence)
    {
      return new SqlFunctionExpression (
          typeof (string),
          "REPLACE",
          expression,
          new SqlLiteralExpression (replacedSequence),
          new SqlLiteralExpression (escapeSequence + replacedSequence));
    }

    private static Expression BuildRightExpression (Expression unescapedSearchText, string likePrefix, string likePostfix)
    {
      Expression rightExpression;
      var argumentAsConstantExpression = unescapedSearchText as ConstantExpression;
      if (argumentAsConstantExpression != null)
      {
        if (argumentAsConstantExpression.Value == null)
          rightExpression = null;
        else
        {
          var escapedSearchText = SqlLikeExpression.Escape ((string) argumentAsConstantExpression.Value, @"\");
          rightExpression = Expression.Constant (string.Format ("{0}{1}{2}", likePrefix, escapedSearchText, likePostfix));
        }
      }
      else
      {
        var escapedSearchExpression = SqlLikeExpression.Escape (unescapedSearchText, @"\");
        var concatMethod = typeof (string).GetMethod ("Concat", new[] { typeof (string), typeof (string) });
        var patternWithPrefix =
            !string.IsNullOrEmpty (likePrefix)
                ? Expression.Add (new SqlLiteralExpression (likePrefix), escapedSearchExpression, concatMethod)
                : escapedSearchExpression;
        var patternWithPrefixAndPostfix =
            !string.IsNullOrEmpty (likePostfix)
                ? Expression.Add (patternWithPrefix, new SqlLiteralExpression (likePostfix), concatMethod)
                : patternWithPrefix;
        rightExpression = patternWithPrefixAndPostfix;
      }
      return rightExpression;
    }

    public SqlLikeExpression (Expression left, Expression right, Expression escapeExpression)
        : base (typeof (bool))
    {
      ArgumentUtility.CheckNotNull ("left", left);
      ArgumentUtility.CheckNotNull ("right", right);
      ArgumentUtility.CheckNotNull ("escapeExpression", escapeExpression);

      _left = left;
      _right = right;
      _escapeExpression = escapeExpression;
    }

    public Expression Left
    {
      get { return _left; }
    }

    public Expression Right
    {
      get { return _right; }
    }

    public Expression EscapeExpression
    {
      get { return _escapeExpression; }
    }

    protected override Expression VisitChildren (ExpressionTreeVisitor visitor)
    {
      var newLeftExpression = visitor.VisitExpression (_left);
      var newRightExpression = visitor.VisitExpression (_right);
      var newEscapeExpression = visitor.VisitExpression (_escapeExpression);

      if (newLeftExpression != _left || newRightExpression != _right || newEscapeExpression != _escapeExpression)
        return new SqlLikeExpression (newLeftExpression, newRightExpression, newEscapeExpression);
      return this;
    }

    public override Expression Accept (ExpressionTreeVisitor visitor)
    {
      var specificVisitor = visitor as ISqlSpecificExpressionVisitor;
      if (specificVisitor != null)
        return specificVisitor.VisitSqlLikeExpression (this);
      else
        return base.Accept (visitor);
    }

    public override string ToString ()
    {
      return string.Format (
          "{0} LIKE {1} ESCAPE {2}",
          FormattingExpressionTreeVisitor.Format (_left),
          FormattingExpressionTreeVisitor.Format (_right),
          FormattingExpressionTreeVisitor.Format (_escapeExpression));
    }
  }
}