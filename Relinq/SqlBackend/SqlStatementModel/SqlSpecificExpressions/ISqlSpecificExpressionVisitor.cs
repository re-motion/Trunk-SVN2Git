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
using System.Linq.Expressions;
using Remotion.Linq.SqlBackend.SqlStatementModel.Unresolved;

namespace Remotion.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions
{
  /// <summary>
  /// This interface should be implemented by visitors that handle SQL-specific expressions.
  /// </summary>
  public interface ISqlSpecificExpressionVisitor
  {
    Expression VisitSqlLiteralExpression (SqlLiteralExpression expression);
    Expression VisitSqlBinaryOperatorExpression (SqlBinaryOperatorExpression expression);
    Expression VisitSqlIsNullExpression (SqlIsNullExpression expression);
    Expression VisitSqlIsNotNullExpression (SqlIsNotNullExpression expression);
    Expression VisitSqlFunctionExpression (SqlFunctionExpression expression);
    Expression VisitSqlConvertExpression (SqlConvertExpression expression);
    Expression VisitSqlExistsExpression (SqlExistsExpression expression);
    Expression VisitSqlRowNumberExpression (SqlRowNumberExpression expression);
    Expression VisitSqlLikeExpression (SqlLikeExpression expression);
    Expression VisitSqlLengthExpression (SqlLengthExpression expression);
  }
}