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
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.SqlBackend.SqlPreparation.ResultOperatorHandlers;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.SqlPreparation.ResultOperatorHandlers
{
  /// <summary>
  /// <see cref="DefaultIfEmptyResultOperatorHandler"/> handles the <see cref="DefaultIfEmptyResultOperator"/>.
  /// </summary>
  public class DefaultIfEmptyResultOperatorHandler : ResultOperatorHandler<DefaultIfEmptyResultOperator>
  {
    public override void HandleResultOperator (DefaultIfEmptyResultOperator resultOperator, QueryModel queryModel, SqlStatementBuilder sqlStatementBuilder, UniqueIdentifierGenerator generator, ISqlPreparationStage stage)
    {
      ArgumentUtility.CheckNotNull ("resultOperator", resultOperator);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);
      ArgumentUtility.CheckNotNull ("sqlStatementBuilder", sqlStatementBuilder);
      ArgumentUtility.CheckNotNull ("generator", generator);
      ArgumentUtility.CheckNotNull ("stage", stage);

      var sqlStatement = sqlStatementBuilder.GetStatementAndResetBuilder();
      var subStatementTableInfo = new ResolvedSubStatementTableInfo (generator.GetUniqueIdentifier ("q"), sqlStatement);
      var leftJoinInfo = new ResolvedLeftJoinInfo (subStatementTableInfo, new SqlLiteralExpression (1), new SqlLiteralExpression (1));
      var joinedTable = new SqlJoinedTable (leftJoinInfo);

      sqlStatementBuilder.SqlTables.Add (joinedTable);
      sqlStatementBuilder.SelectProjection = new SqlTableReferenceExpression (joinedTable);
      // the new statement is an identity query that selects the result of its subquery, so it starts with the same data type
      sqlStatementBuilder.DataInfo = sqlStatement.DataInfo;
    }
  }
}