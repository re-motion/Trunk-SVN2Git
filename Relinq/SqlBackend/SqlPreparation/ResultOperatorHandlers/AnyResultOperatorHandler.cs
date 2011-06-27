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
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.SqlBackend.SqlStatementModel;
using Remotion.Linq.SqlBackend.SqlStatementModel.SqlSpecificExpressions;
using Remotion.Linq.Utilities;

namespace Remotion.Linq.SqlBackend.SqlPreparation.ResultOperatorHandlers
{
  /// <summary>
  /// Handles the <see cref="AnyResultOperatorHandler"/>.
  /// </summary>
  public class AnyResultOperatorHandler : ResultOperatorHandler<AnyResultOperator>
  {
    public override void HandleResultOperator (AnyResultOperator resultOperator, SqlStatementBuilder sqlStatementBuilder, UniqueIdentifierGenerator generator, ISqlPreparationStage stage, ISqlPreparationContext context)
    {
      ArgumentUtility.CheckNotNull ("resultOperator", resultOperator);
      ArgumentUtility.CheckNotNull ("sqlStatementBuilder", sqlStatementBuilder);
      ArgumentUtility.CheckNotNull ("generator", generator);
      ArgumentUtility.CheckNotNull ("stage", stage);
      ArgumentUtility.CheckNotNull ("context", context);

      // No name required for the select projection inside of an EXISTS expression
      var sqlSubStatement = sqlStatementBuilder.GetStatementAndResetBuilder();
      var subStatementExpression = new SqlSubStatementExpression (sqlSubStatement);

      var sqlExistsExpression = new SqlExistsExpression (subStatementExpression);

      sqlStatementBuilder.SelectProjection = stage.PrepareSelectExpression (sqlExistsExpression, context);
      UpdateDataInfo (resultOperator, sqlStatementBuilder, sqlSubStatement.DataInfo);
    }
  }
}