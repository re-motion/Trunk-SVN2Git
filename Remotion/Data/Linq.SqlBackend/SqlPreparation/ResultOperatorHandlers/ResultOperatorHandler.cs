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
using System.Diagnostics;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.StreamedData;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.SqlPreparation.ResultOperatorHandlers
{
  /// <summary>
  /// Default implementation of <see cref="IResultOperatorHandler"/> providing commonly needed functionality.
  /// </summary>
  /// <typeparam name="T">The result operator type handled by the concrete subclass of <see cref="ResultOperatorHandler{T}"/>.</typeparam>
  public abstract class ResultOperatorHandler<T> : IResultOperatorHandler
      where T: ResultOperatorBase
  {
    public Type SupportedResultOperatorType
    {
      get { return typeof (T); }
    }

    public abstract void HandleResultOperator (
        T resultOperator,
        SqlStatementBuilder sqlStatementBuilder,
        UniqueIdentifierGenerator generator,
        ISqlPreparationStage stage,
        ISqlPreparationContext context);

    protected void EnsureNoTopExpression (
        ResultOperatorBase resultOperator,
        SqlStatementBuilder sqlStatementBuilder,
        UniqueIdentifierGenerator generator,
        ISqlPreparationStage stage,
        ISqlPreparationContext context)
    {
      ArgumentUtility.CheckNotNull ("resultOperator", resultOperator);
      ArgumentUtility.CheckNotNull ("sqlStatementBuilder", sqlStatementBuilder);
      ArgumentUtility.CheckNotNull ("generator", generator);
      ArgumentUtility.CheckNotNull ("stage", stage);

      if (sqlStatementBuilder.TopExpression != null)
        MoveCurrentStatementToSqlTable (sqlStatementBuilder, generator, context, info => new SqlTable (info));
    }

    protected void EnsureNoDistinctQuery (
        ResultOperatorBase resultOperator,
        SqlStatementBuilder sqlStatementBuilder,
        UniqueIdentifierGenerator generator,
        ISqlPreparationStage stage,
        ISqlPreparationContext context)
    {
      ArgumentUtility.CheckNotNull ("resultOperator", resultOperator);
      ArgumentUtility.CheckNotNull ("sqlStatementBuilder", sqlStatementBuilder);
      ArgumentUtility.CheckNotNull ("generator", generator);
      ArgumentUtility.CheckNotNull ("stage", stage);

      if (sqlStatementBuilder.IsDistinctQuery)
        MoveCurrentStatementToSqlTable (sqlStatementBuilder, generator, context, info => new SqlTable (info));
    }

    protected void MoveCurrentStatementToSqlTable (
        SqlStatementBuilder sqlStatementBuilder,
        UniqueIdentifierGenerator generator,
        ISqlPreparationContext context,
        Func<ResolvedSubStatementTableInfo, SqlTableBase> tableGenerator)
    {
      var sqlStatement = sqlStatementBuilder.GetStatementAndResetBuilder();

      var subStatementTableInfo = new ResolvedSubStatementTableInfo (
          generator.GetUniqueIdentifier ("q"),
          sqlStatement);
      var sqlTable = tableGenerator (subStatementTableInfo);

      var newSqlTableReferenceExpression = new SqlTableReferenceExpression (sqlTable);

      sqlStatementBuilder.SqlTables.Add (sqlTable);
      sqlStatementBuilder.SelectProjection = new NamedExpression (null, newSqlTableReferenceExpression);

      // the new statement is an identity query that selects the result of its subquery, so it starts with the same data type
      sqlStatementBuilder.DataInfo = sqlStatement.DataInfo;

      Debug.Assert (sqlStatement.DataInfo is StreamedSequenceInfo);

      // Later ResultOperatorHandlers might have expressions that access the value streaming out from this result operator. These expressions must 
      // be updated to get their input expression (the ItemExpression of sqlStatement.DataInfo) from the sub-statement table we just created.
      // Therefore, register an expression mapping from the ItemExpression to the new SqlTable.
      // (We cannot use the sqlStatement.SelectExpression for the mapping because that expression has already been transformed and therefore will 
      // not compare equal to the expressions of the later result operators as long as we can only compare expressions by reference. The 
      // ItemExpression, on the other hand, should compare fine because it is inserted by reference into the result operators' expressions during 
      // the front-end's lambda resolution process.)

      var itemExpressionNowInSqlTable = ((StreamedSequenceInfo) sqlStatement.DataInfo).ItemExpression;
      context.AddExpressionMapping (itemExpressionNowInSqlTable, newSqlTableReferenceExpression);
    }

    protected void UpdateDataInfo (ResultOperatorBase resultOperator, SqlStatementBuilder sqlStatementBuilder, IStreamedDataInfo dataInfo)
    {
      sqlStatementBuilder.DataInfo = resultOperator.GetOutputDataInfo (dataInfo);
    }

    void IResultOperatorHandler.HandleResultOperator (
        ResultOperatorBase resultOperator,
        SqlStatementBuilder sqlStatementBuilder,
        UniqueIdentifierGenerator generator,
        ISqlPreparationStage stage,
        ISqlPreparationContext context)
    {
      ArgumentUtility.CheckNotNull ("resultOperator", resultOperator);
      ArgumentUtility.CheckNotNull ("sqlStatementBuilder", sqlStatementBuilder);
      ArgumentUtility.CheckNotNull ("generator", generator);
      ArgumentUtility.CheckNotNull ("stage", stage);
      ArgumentUtility.CheckNotNull ("context", context);

      var castOperator = ArgumentUtility.CheckNotNullAndType<T> ("resultOperator", resultOperator);
      HandleResultOperator (castOperator, sqlStatementBuilder, generator, stage, context);
    }
  }
}