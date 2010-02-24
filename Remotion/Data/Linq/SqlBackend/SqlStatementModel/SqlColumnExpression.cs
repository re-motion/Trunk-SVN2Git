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
using Remotion.Data.Linq.SqlBackend.SqlGeneration;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.SqlStatementModel
{
  /// <summary>
  /// <see cref="SqlColumnExpression"/> represents a sql-specific column expression.
  /// </summary>
  public class SqlColumnExpression : ExtensionExpression
  {
    private SqlTable _sqlTable;
    private string _columnName;

    public SqlColumnExpression (Type type, SqlTable sqlTable, string columnName)
        : base(type)
    {
      ArgumentUtility.CheckNotNull ("sqlTable", sqlTable);
      ArgumentUtility.CheckNotNullOrEmpty ("columnName", columnName);

      ColumnName = columnName;
      SqlTable = sqlTable;
    }

    public SqlTable SqlTable
    {
      get { return _sqlTable; }
      set { _sqlTable = value; }
    }

    public string ColumnName
    {
      get { return _columnName; }
      set { _columnName = value; }
    }

    protected internal override Expression VisitChildren (ExpressionTreeVisitor visitor)
    {
      return this;
    }

    public override Expression Accept (ExpressionTreeVisitor visitor)
    {
      var specificVisitor = visitor as ISqlTextExpressionVisitor;
      if(specificVisitor!=null)
        return specificVisitor.VisitSqlColumnExpression (this);
      else
        return base.Accept (visitor);
    }
  }
}