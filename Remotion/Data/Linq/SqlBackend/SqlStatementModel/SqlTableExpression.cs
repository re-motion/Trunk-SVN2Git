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
using System.Linq.Expressions;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Parsing;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.SqlStatementModel
{
  /// <summary>
  /// <see cref="SqlTableExpression"/> holds source of a from expression.
  /// </summary>
  public class SqlTableExpression : ExtensionExpression // TODO: Consider making this SqlTable (without Expression)
  {
    private AbstractTableSource _tableSource;

    public SqlTableExpression (Type type, AbstractTableSource tableSource)
        : base (type)
    {
      ArgumentUtility.CheckNotNull ("tableSource", tableSource);
      _tableSource = tableSource;
    }

    public AbstractTableSource TableSource
    {
      get { return _tableSource; }
      set { _tableSource = ArgumentUtility.CheckNotNull ("value", value); }
    }

    protected internal override Expression VisitChildren (ExpressionTreeVisitor visitor)
    {
      return this;
    }

    public override Expression Accept (ExpressionTreeVisitor visitor)
    {
      var specificVisitor = visitor as ISqlExpressionVisitor;
      if (specificVisitor != null)
        return specificVisitor.VisitSqlTableExpression (this);
      else
        return base.Accept (visitor);
    }
  }
}