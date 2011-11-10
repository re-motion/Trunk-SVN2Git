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
using System.Collections.Generic;
using System.Linq.Expressions;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.Parsing;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions;
using Remotion.Linq.Utilities;

namespace Remotion.Linq.SqlBackend.SqlStatementModel
{
  /// <summary>
  /// <see cref="SqlSubStatementExpression"/> represents a SQL database subquery. The <see cref="QueryModel"/> of the subquery is translated to 
  /// this model, and the <see cref="SqlSubStatementExpression"/> is transformed several times until it can easily be translated to SQL text.
  /// </summary>
  public class SqlSubStatementExpression : ExtensionExpression
  {
    private readonly SqlStatement _sqlStatement;

    public SqlSubStatementExpression (SqlStatement sqlStatement)
        : base (sqlStatement.DataInfo.DataType)
    {
      ArgumentUtility.CheckNotNull ("sqlStatement", sqlStatement);

      _sqlStatement = sqlStatement;
    }

    public SqlStatement SqlStatement
    {
      get { return _sqlStatement; }
    }

    protected override Expression VisitChildren (ExpressionTreeVisitor visitor)
    {
      return this;
    }

    public override Expression Accept (ExpressionTreeVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      var specificVisitor = visitor as ISqlSubStatementVisitor;
      if (specificVisitor != null)
        return specificVisitor.VisitSqlSubStatementExpression (this);
      else
        return base.Accept (visitor);
    }

    public override string ToString ()
    {
      return "(" + _sqlStatement + ")";
    }
    
    public SqlTable ConvertToSqlTable (string uniqueIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("uniqueIdentifier", uniqueIdentifier);
      ArgumentUtility.CheckNotNullOrEmpty ("uniqueIdentifier", uniqueIdentifier);
      
      var joinSemantic = CalculateJoinSemantic();

      SqlStatement sequenceStatement;
      if (SqlStatement.DataInfo is StreamedSequenceInfo)
        sequenceStatement = SqlStatement;
      else
        sequenceStatement = ConvertValueStatementToSequenceStatement ();

      var resolvedSubStatementTableInfo = new ResolvedSubStatementTableInfo (uniqueIdentifier, sequenceStatement);
      return new SqlTable (resolvedSubStatementTableInfo, joinSemantic);
    }

    private JoinSemantics CalculateJoinSemantic ()
    {
      var dataInfoAsStreamedSingleValueInfo = SqlStatement.DataInfo as StreamedSingleValueInfo;
      if (dataInfoAsStreamedSingleValueInfo != null && dataInfoAsStreamedSingleValueInfo.ReturnDefaultWhenEmpty)
        return JoinSemantics.Left;
      else
        return JoinSemantics.Inner;
    }

    private SqlStatement ConvertValueStatementToSequenceStatement ()
    {
      var newDataInfo = new StreamedSequenceInfo (typeof (IEnumerable<>).MakeGenericType (SqlStatement.DataInfo.DataType), SqlStatement.SelectProjection);

      var adjustedStatementBuilder = new SqlStatementBuilder (SqlStatement) { DataInfo = newDataInfo };
      if (SqlStatement.DataInfo is StreamedSingleValueInfo && SqlStatement.SqlTables.Count!=0)
      {
        // A sub-statement might use a different TopExpression than 1 (or none at all) in order to provoke a SQL error when more than one item is 
        // returned. When we convert the statement to a sequence statement, however, we must ensure that the exact "only 1 value is returned" 
        // semantics is ensured because we can't provoke a SQL error (but instead would return strange result sets).
        adjustedStatementBuilder.TopExpression = new SqlLiteralExpression (1);
      }

      return adjustedStatementBuilder.GetSqlStatement();
    }
  }
}