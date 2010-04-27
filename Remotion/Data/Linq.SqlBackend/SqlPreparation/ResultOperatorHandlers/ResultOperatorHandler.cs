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
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.SqlPreparation.ResultOperatorHandlers
{
  /// <summary>
  /// <see cref="ResultOperatorHandler{T}"/> handles implementations of <see cref="ResultOperatorBase"/>.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public abstract class ResultOperatorHandler<T> : IResultOperatorHandler where T: ResultOperatorBase
  {
    protected abstract void HandleResultOperator (T resultOperator, ref SqlStatementBuilder sqlStatementBuilder, UniqueIdentifierGenerator generator, ISqlPreparationStage stage);

    protected void EnsureNoTopExpressionAndSetDataInfo (ResultOperatorBase resultOperator, ref SqlStatementBuilder sqlStatementBuilder, UniqueIdentifierGenerator generator, ISqlPreparationStage stage)
    {
      if (sqlStatementBuilder.TopExpression != null)
      {
        var sqlStatement = GetStatementAndResetBuilder (ref sqlStatementBuilder);
        sqlStatementBuilder = new SqlStatementBuilder();

        var subStatementTableInfo = new ResolvedSubStatementTableInfo (
            generator.GetUniqueIdentifier ("q"),
            sqlStatement);
        var sqlTable = new SqlTable (subStatementTableInfo);

        sqlStatementBuilder.SqlTables.Add (sqlTable);
        sqlStatementBuilder.SelectProjection = new SqlTableReferenceExpression (sqlTable);
        // the new statement is an identity query that selects the result of its subquery, so it starts with the same data type
        sqlStatementBuilder.DataInfo = sqlStatement.DataInfo;
      }
      sqlStatementBuilder.DataInfo = resultOperator.GetOutputDataInfo (sqlStatementBuilder.DataInfo);
    }

    public void HandleResultOperator (ResultOperatorBase resultOperator, ref SqlStatementBuilder sqlStatementBuilder, UniqueIdentifierGenerator generator, ISqlPreparationStage stage)
    {
      HandleResultOperator ((T) resultOperator, ref sqlStatementBuilder, generator, stage);
    }

    protected virtual SqlStatement GetStatementAndResetBuilder (ref SqlStatementBuilder sqlStatementBuilder)
    {
      var sqlSubStatement = sqlStatementBuilder.GetSqlStatement ();
      sqlStatementBuilder = new SqlStatementBuilder ();
      return sqlSubStatement;
    }
  }
}