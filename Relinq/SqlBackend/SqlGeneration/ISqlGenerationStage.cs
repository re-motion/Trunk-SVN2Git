// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Linq.Clauses;
using Remotion.Linq.SqlBackend.SqlStatementModel;

namespace Remotion.Linq.SqlBackend.SqlGeneration
{
  /// <summary>
  /// Provides entry points for all sql text generation that occur during the SQL generation process.
  /// </summary>
  public interface ISqlGenerationStage
  {
    void GenerateTextForFromTable (ISqlCommandBuilder commandBuilder, SqlTableBase table, bool isFirstTable);
    void GenerateTextForSelectExpression (ISqlCommandBuilder commandBuilder, Expression expression);
    void GenerateTextForOuterSelectExpression (ISqlCommandBuilder commandBuilder, Expression expression);
    void GenerateTextForWhereExpression (ISqlCommandBuilder commandBuilder, Expression expression);
    void GenerateTextForOrderByExpression (ISqlCommandBuilder commandBuilder, Expression expression);
    void GenerateTextForTopExpression (ISqlCommandBuilder commandBuilder, Expression expression);
    void GenerateTextForSqlStatement (ISqlCommandBuilder commandBuilder, SqlStatement sqlStatement);
    void GenerateTextForOuterSqlStatement (ISqlCommandBuilder commandBuilder, SqlStatement sqlStatement);
    void GenerateTextForJoinKeyExpression (ISqlCommandBuilder commandBuilder, Expression expression);
    void GenerateTextForGroupByExpression (ISqlCommandBuilder commandBuilder, Expression expression);
    void GenerateTextForOrdering (ISqlCommandBuilder commandBuilder, Ordering ordering);
  }
}