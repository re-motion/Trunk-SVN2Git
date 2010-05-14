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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.StreamedData;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.MappingResolution
{
  /// <summary>
  /// <see cref="SqlContextSelectionAdjuster"/> applies <see cref="SqlExpressionContext"/> to a <see cref="SqlStatement"/>.
  /// </summary>
  public class SqlContextSelectionAdjuster
  {
    private readonly IMappingResolutionStage _stage;

    public static SqlStatement ApplyContext (SqlStatement sqlStatement, SqlExpressionContext context, IMappingResolutionStage stage)
    {
      ArgumentUtility.CheckNotNull ("sqlStatement", sqlStatement);
      ArgumentUtility.CheckNotNull ("stage", stage);

      var visitor = new SqlContextSelectionAdjuster (stage);
      return visitor.VisitSqlStatement (sqlStatement, context);
    }

    private SqlContextSelectionAdjuster (IMappingResolutionStage stage)
    {
      ArgumentUtility.CheckNotNull ("stage", stage);

      _stage = stage;
    }
    
    public SqlStatement VisitSqlStatement (SqlStatement sqlStatement, SqlExpressionContext context)
    {
      ArgumentUtility.CheckNotNull ("sqlStatement", sqlStatement);

      if (context == SqlExpressionContext.PredicateRequired)
        throw new InvalidOperationException ("A SqlStatement cannot be used as a predicate.");

      var statementBuilder = new SqlStatementBuilder (sqlStatement);

      var newSelectProjection = _stage.ApplyContext (sqlStatement.SelectProjection, context);
      statementBuilder.SelectProjection = newSelectProjection;
      statementBuilder.RecalculateDataInfo (sqlStatement.SelectProjection);

      // TODO Review 2765: Don't keep commented code, also remove unused methods
      //statementBuilder.DataInfo = sqlStatement.DataInfo;
      //statementBuilder.IsDistinctQuery = sqlStatement.IsDistinctQuery;

      //VisitSelectProjection(sqlStatement, context, statementBuilder);
      //VisitWhereCondition(sqlStatement.WhereCondition, statementBuilder);
      //VisitOrderings (sqlStatement.Orderings, statementBuilder);
      //VisitTopExpression(sqlStatement.TopExpression, statementBuilder);
      //VisitSqlTables (sqlStatement.SqlTables, statementBuilder);

      var newSqlStatement = statementBuilder.GetSqlStatement();
      return newSqlStatement.Equals (sqlStatement) ? sqlStatement : newSqlStatement;
    }

    private void VisitSelectProjection (SqlStatement sqlStatement  , SqlExpressionContext selectContext, SqlStatementBuilder statementBuilder)
    {
      var newSelectProjection = _stage.ApplyContext (sqlStatement.SelectProjection, selectContext);
      statementBuilder.SelectProjection = newSelectProjection;
      statementBuilder.RecalculateDataInfo (sqlStatement.SelectProjection);
    }

    //private void VisitWhereCondition (Expression whereCondition, SqlStatementBuilder statementBuilder)
    //{
    //  if (whereCondition != null)
    //    statementBuilder.WhereCondition = _stage.ApplyContext (whereCondition, SqlExpressionContext.PredicateRequired);
    //}

    //private void VisitOrderings (IEnumerable<Ordering> orderings, SqlStatementBuilder statementBuilder)
    //{
    //  foreach (var ordering in orderings)
    //  {
    //    var newExpression = _stage.ApplyContext (ordering.Expression, SqlExpressionContext.SingleValueRequired);
    //    statementBuilder.Orderings.Add (new Ordering (newExpression, ordering.OrderingDirection));
    //  }
    //}

    //private void VisitTopExpression (Expression topExpression, SqlStatementBuilder statementBuilder)
    //{
    //  if (topExpression != null)
    //    statementBuilder.TopExpression = _stage.ApplyContext (topExpression, SqlExpressionContext.SingleValueRequired);
    //}

    //private void VisitSqlTables (IEnumerable<SqlTableBase> tables, SqlStatementBuilder statementBuilder)
    //{
    //  foreach (var table in tables)
    //  {
    //    //_stage.ApplyContext (table, SqlExpressionContext.ValueRequired);
    //    statementBuilder.SqlTables.Add (table);
    //  }
    //}
   
  }
}