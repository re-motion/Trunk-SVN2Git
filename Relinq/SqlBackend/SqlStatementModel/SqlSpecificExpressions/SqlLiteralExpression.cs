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
using System.Linq.Expressions;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;
using Remotion.Linq.Utilities;

namespace Remotion.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions
{
  public class SqlLiteralExpression : ExtensionExpression
  {
    private readonly object _value;

    public SqlLiteralExpression (int value, bool nullable = false)
        : this (value, nullable ? typeof (int?) : typeof (int))
    {
    }

    public SqlLiteralExpression (string value)
      : this (ArgumentUtility.CheckNotNull ("value", value), typeof (string))
    {
    }

    public SqlLiteralExpression (double value, bool nullable = false)
      : this (value, nullable ? typeof (double?) : typeof (double))
    {
    }

    private SqlLiteralExpression (object value, Type type)
        : base (type)
    {
      _value = value;
    }

    public object Value
    {
      get { return _value; }
    }

    protected override Expression VisitChildren (ExpressionTreeVisitor visitor)
    {
      return this;
    }

    public override Expression Accept (ExpressionTreeVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      var specificVisitor = visitor as ISqlSpecificExpressionVisitor;
      if (specificVisitor != null)
        return specificVisitor.VisitSqlLiteralExpression (this);
      else
        return base.Accept (visitor);
    }

    public override string ToString ()
    {
      if (Value is string)
        return "\"" + Value + "\"";
      else
        return Value.ToString();
    }
  }
}