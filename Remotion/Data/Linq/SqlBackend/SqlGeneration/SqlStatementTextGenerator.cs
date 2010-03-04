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
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.SqlGeneration
{
  /// <summary>
  /// <see cref="SqlStatementTextGenerator"/> generates sql-text from a given <see cref="SqlStatement"/>.
  /// </summary>
  public class SqlStatementTextGenerator
  {
    public SqlCommand Build (SqlStatement sqlStatement)
    {
      ArgumentUtility.CheckNotNull ("sqlStatement", sqlStatement);
      
      //TODO: register IMethodCallSqlGenerators
      
      var commandBuilder = new SqlCommandBuilder();
      commandBuilder.Append ("SELECT ");
      BuildSelectPart (sqlStatement.SelectProjection, commandBuilder);
      commandBuilder.Append (" FROM ");
      BuildFromPart (sqlStatement.FromExpression, commandBuilder);

      return  new SqlCommand(commandBuilder.GetCommandText(), commandBuilder.GetCommandParameters());
    }

    protected void BuildSelectPart (Expression expression, SqlCommandBuilder commandBuilder)
    {
      SqlGeneratingExpressionVisitor.GenerateSql (expression, commandBuilder);
    }

    protected void BuildFromPart (SqlTable sqlTable, SqlCommandBuilder commandBuilder)
    {
      SqlTableSourceVisitor.GenerateSql (sqlTable, commandBuilder);
    }


    
  }
}