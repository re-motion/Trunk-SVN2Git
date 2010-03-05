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

namespace Remotion.Data.Linq.SqlBackend.SqlStatementModel
{
  /// <summary>
  /// <see cref="SqlStatementVisitorBase"/> provides methods to visit sql-statement classes.
  /// </summary>
  public abstract class SqlStatementVisitorBase
  {
    public virtual void VisitSqlStatement (SqlStatement sqlStatement)
    {
      sqlStatement.SelectProjection = VisitSelectProjection (sqlStatement.SelectProjection, sqlStatement.UniqueIdentifierGenerator);
      VisitSqlTable (sqlStatement.FromExpression);
      if (sqlStatement.WhereCondition != null)
        VisitWhereCondition (sqlStatement.WhereCondition, sqlStatement.UniqueIdentifierGenerator);
      if (sqlStatement.TopExpression != null)
        VisitTopExpression (sqlStatement.TopExpression, sqlStatement.UniqueIdentifierGenerator);
    }

    protected abstract Expression VisitSelectProjection (Expression selectProjection, UniqueIdentifierGenerator uniqueIdentifierGenerator);
    protected abstract void VisitSqlTable (SqlTable sqlTable);
    protected abstract Expression VisitTopExpression (Expression topExpression, UniqueIdentifierGenerator uniqueIdentifierGenerator);
    protected abstract Expression VisitWhereCondition (Expression whereCondition, UniqueIdentifierGenerator uniqueIdentifierGenerator);
  }
}