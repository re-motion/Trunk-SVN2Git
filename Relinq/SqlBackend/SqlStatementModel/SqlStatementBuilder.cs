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
using System.Text;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.SqlStatementModel
{
  /// <summary>
  /// <see cref="SqlStatementBuilder"/> holds the specific SQL statement data and populates a build method.
  /// </summary>
  public class SqlStatementBuilder
  {
    private ValueHolder _valueHolder;

    public SqlStatementBuilder ()
    {
      _valueHolder = new ValueHolder();
    }

    public SqlStatementBuilder (SqlStatement sqlStatement)
    {
      ArgumentUtility.CheckNotNull ("sqlStatement", sqlStatement);

      _valueHolder = new ValueHolder (sqlStatement);
    }

    public IStreamedDataInfo DataInfo
    {
      get { return _valueHolder.DataInfo; }
      set { _valueHolder.DataInfo = value; }
    }

    public bool IsDistinctQuery
    {
      get { return _valueHolder.IsDistinctQuery; }
      set { _valueHolder.IsDistinctQuery = value; }
    }

    public Expression TopExpression
    {
      get { return _valueHolder.TopExpression; }
      set { _valueHolder.TopExpression = value; }
    }

    public Expression SelectProjection
    {
      get { return _valueHolder.SelectProjection; }
      set { _valueHolder.SelectProjection = value; }
    }

    public List<SqlTableBase> SqlTables
    {
      get { return _valueHolder.SqlTables; }
    }

    public Expression WhereCondition
    {
      get { return _valueHolder.WhereCondition; }
      set { _valueHolder.WhereCondition = value; }
    }

    public Expression GroupByExpression
    {
      get { return _valueHolder.GroupByExpression; }
      set { _valueHolder.GroupByExpression = value; }
    }

    public List<Ordering> Orderings
    {
      get { return _valueHolder.Orderings; }
    }

    public Expression RowNumberSelector
    {
      get { return _valueHolder.RowNumberSelector; }
      set { _valueHolder.RowNumberSelector = value; }
    }

    public Expression CurrentRowNumberOffset
    {
      get { return _valueHolder.CurrentRowNumberOffset; }
      set { _valueHolder.CurrentRowNumberOffset = value; }
    }

    public SqlStatement GetSqlStatement ()
    {
      if (DataInfo == null)
        throw new InvalidOperationException ("A DataInfo must be set before the SqlStatement can be retrieved.");
      return new SqlStatement (
          DataInfo,
          SelectProjection,
          SqlTables, WhereCondition, GroupByExpression, Orderings, TopExpression, IsDistinctQuery, RowNumberSelector, CurrentRowNumberOffset);
    }

    public void AddWhereCondition (Expression translatedExpression)
    {
      if (WhereCondition != null)
        WhereCondition = Expression.AndAlso (WhereCondition, translatedExpression);
      else
        WhereCondition = translatedExpression;
    }

    public SqlStatement GetStatementAndResetBuilder ()
    {
      var sqlSubStatement = GetSqlStatement();
      _valueHolder = new ValueHolder();
      return sqlSubStatement;
    }

    public void RecalculateDataInfo (Expression previousSelectProjection)
    {
      if (SelectProjection.Type != previousSelectProjection.Type) // TODO: Consider removing this check and the parameter
        DataInfo = GetNewDataInfo();
    }

    public override string ToString ()
    {
      var sb = new StringBuilder ("SELECT ");
      if (IsDistinctQuery)
        sb.Append ("DISTINCT ");
      if (TopExpression != null)
        sb.Append ("TOP (").Append (FormattingExpressionTreeVisitor.Format (TopExpression)).Append (") ");
      if (SelectProjection != null)
        sb.Append (FormattingExpressionTreeVisitor.Format (SelectProjection));
      if (SqlTables.Count > 0)
      {
        sb.Append (" FROM ");
        SqlTables.Aggregate (sb, (builder, table) => builder.Append (table));
      }
      if (WhereCondition != null)
        sb.Append (" WHERE ").Append (FormattingExpressionTreeVisitor.Format (WhereCondition));
      if (GroupByExpression != null)
        sb.Append (" GROUP BY ").Append (FormattingExpressionTreeVisitor.Format (GroupByExpression));
      if (Orderings.Count > 0)
      {
        sb.Append (" ORDER BY ");
        Orderings.Aggregate (
            sb,
            (builder, ordering) => builder
                                       .Append (FormattingExpressionTreeVisitor.Format (ordering.Expression))
                                       .Append (" ")
                                       .Append (ordering.OrderingDirection.ToString().ToUpper()));
      }

      return sb.ToString();
    }

    private IStreamedDataInfo GetNewDataInfo ()
    {
      var sequenceInfo = DataInfo as StreamedSequenceInfo;
      if (sequenceInfo != null)
        return new StreamedSequenceInfo (typeof (IQueryable<>).MakeGenericType (SelectProjection.Type), SelectProjection);

      var singleValueInfo = DataInfo as StreamedSingleValueInfo;
      if (singleValueInfo != null)
        return new StreamedSingleValueInfo (SelectProjection.Type, singleValueInfo.ReturnDefaultWhenEmpty);

      return DataInfo;
    }

    private class ValueHolder
    {
      public ValueHolder ()
      {
        SqlTables = new List<SqlTableBase>();
        Orderings = new List<Ordering>();
      }

      public ValueHolder (SqlStatement sqlStatement)
      {
        DataInfo = sqlStatement.DataInfo;
        SelectProjection = sqlStatement.SelectProjection;
        WhereCondition = sqlStatement.WhereCondition;
        IsDistinctQuery = sqlStatement.IsDistinctQuery;
        TopExpression = sqlStatement.TopExpression;
        GroupByExpression = sqlStatement.GroupByExpression;
        RowNumberSelector = sqlStatement.RowNumberSelector;
        CurrentRowNumberOffset = sqlStatement.CurrentRowNumberOffset;

        SqlTables = new List<SqlTableBase> (sqlStatement.SqlTables);
        Orderings = new List<Ordering> (sqlStatement.Orderings);
      }

      public IStreamedDataInfo DataInfo { get; set; }
      public bool IsDistinctQuery { get; set; }
      public Expression TopExpression { get; set; }
      public Expression SelectProjection { get; set; }
      public List<SqlTableBase> SqlTables { get; private set; }
      public Expression WhereCondition { get; set; }
      public Expression GroupByExpression { get; set; }
      public List<Ordering> Orderings { get; private set; }
      public Expression RowNumberSelector { get; set; }
      public Expression CurrentRowNumberOffset { get; set; }
    }
  }
}