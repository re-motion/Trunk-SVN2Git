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
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;

namespace Remotion.Data.Linq.SqlBackend.SqlGeneration
{
  /// <summary>
  /// Provides entry points for all sql text generation that occur during the SQL generation process.
  /// </summary>
  public interface ISqlGenerationStage
  {
    void GenerateTextForFromTable (SqlCommandBuilder commandBuilder, SqlTable table, bool isFirstTable);
    void GenerateTextForSelectExpression (SqlCommandBuilder commandBuilder, Expression expression);
    void GenerateTextForWhereExpression (SqlCommandBuilder commandBuilder, Expression expression);
    void GenerateTextForOrderByExpression (SqlCommandBuilder commandBuilder, Expression expression);
    void GenerateTextForTopExpression (SqlCommandBuilder commandBuilder, Expression expression);
    void GenerateTextForSqlStatement (SqlCommandBuilder commandBuilder, SqlStatement sqlStatement);
    void GenerateTextForJoinKeyExpression (SqlCommandBuilder commandBuilder, Expression expression);
  }
}