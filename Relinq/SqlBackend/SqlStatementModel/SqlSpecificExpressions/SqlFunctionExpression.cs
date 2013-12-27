// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Linq.Parsing;
using Remotion.Utilities;

namespace Remotion.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions
{
  /// <summary>
  /// <see cref="SqlFunctionExpression"/> holds the sql specific function with its parameters.
  /// </summary>
  public class SqlFunctionExpression : ExtensionExpression
  {
    private readonly string _sqlFunctioName;
    private readonly ReadOnlyCollection<Expression> _args;

    public SqlFunctionExpression (Type type, string sqlFunctioName, params Expression[] args)
        : base (type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("sqlFunctioName", sqlFunctioName);
      ArgumentUtility.CheckNotNull ("args", args);

      _args = Array.AsReadOnly(args);
      _sqlFunctioName = sqlFunctioName;
    }

    public string SqlFunctioName
    {
      get { return _sqlFunctioName; }
    }

    public ReadOnlyCollection<Expression> Args
    {
      get { return _args; }
    }

    protected override Expression VisitChildren (ExpressionTreeVisitor visitor)
    {
      var newArgs = visitor.VisitAndConvert (_args, "SqlFunctionExpression.VisitChildren");

      if (_args != newArgs)
        return new SqlFunctionExpression (Type, _sqlFunctioName, newArgs.ToArray());
      else
        return this;
    }

    public override Expression Accept (ExpressionTreeVisitor visitor)
    {
      var specificVisitor = visitor as ISqlSpecificExpressionVisitor;
      if (specificVisitor != null)
        return specificVisitor.VisitSqlFunctionExpression (this);
      else
        return base.Accept (visitor);
    }

    public override string ToString ()
    {
      return string.Format ("{0}({1})", _sqlFunctioName, String.Join (",", _args.Select (arg => FormattingExpressionTreeVisitor.Format(arg)).ToArray()));
    }
  }
}