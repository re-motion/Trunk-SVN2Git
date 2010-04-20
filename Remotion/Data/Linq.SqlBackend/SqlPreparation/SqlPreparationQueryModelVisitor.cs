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
using System.Linq;
using System.Linq.Expressions;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.SqlPreparation
{
  /// <summary>
  /// <see cref="SqlPreparationQueryModelVisitor"/> generates a <see cref="SqlStatement"/> from a query model.
  /// </summary>
  public class SqlPreparationQueryModelVisitor : QueryModelVisitorBase
  {
    public static SqlStatement TransformQueryModel (
        QueryModel queryModel, SqlPreparationContext preparationContext, ISqlPreparationStage stage, UniqueIdentifierGenerator generator)
    {
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);
      ArgumentUtility.CheckNotNull ("preparationContext", preparationContext);
      ArgumentUtility.CheckNotNull ("stage", stage);
      ArgumentUtility.CheckNotNull ("generator", generator);

      var visitor = new SqlPreparationQueryModelVisitor (preparationContext, stage, generator);
      queryModel.Accept (visitor);

      return visitor.GetSqlStatement();
    }

    private readonly SqlPreparationContext _context;
    private readonly ISqlPreparationStage _stage;

    private SqlStatementBuilder _sqlStatementBuilder;
    private readonly UniqueIdentifierGenerator _generator;

    protected SqlPreparationQueryModelVisitor (SqlPreparationContext context, ISqlPreparationStage stage, UniqueIdentifierGenerator generator)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("stage", stage);
      ArgumentUtility.CheckNotNull ("generator", generator);

      _context = context;
      _stage = stage;
      _generator = generator;

      _sqlStatementBuilder = new SqlStatementBuilder();
    }

    public SqlPreparationContext Context
    {
      get { return _context; }
    }

    protected ISqlPreparationStage Stage
    {
      get { return _stage; }
    }

    protected SqlStatementBuilder SqlStatementBuilder
    {
      get { return _sqlStatementBuilder; }
    }

    public SqlStatement GetSqlStatement ()
    {
      return SqlStatementBuilder.GetSqlStatement();
    }

    public override void VisitMainFromClause (MainFromClause fromClause, QueryModel queryModel)
    {
      ArgumentUtility.CheckNotNull ("fromClause", fromClause);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      AddFromClause (fromClause, fromClause.FromExpression);
    }

    public override void VisitAdditionalFromClause (AdditionalFromClause fromClause, QueryModel queryModel, int index)
    {
      ArgumentUtility.CheckNotNull ("fromClause", fromClause);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      AddFromClause (fromClause, fromClause.FromExpression);
    }

    public override void VisitWhereClause (WhereClause whereClause, QueryModel queryModel, int index)
    {
      ArgumentUtility.CheckNotNull ("whereClause", whereClause);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      var translatedExpression = _stage.PrepareWhereExpression (whereClause.Predicate);
      SqlStatementBuilder.AddWhereCondition (translatedExpression);
    }

    public override void VisitSelectClause (SelectClause selectClause, QueryModel queryModel)
    {
      ArgumentUtility.CheckNotNull ("selectClause", selectClause);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      SqlStatementBuilder.SelectProjection = _stage.PrepareSelectExpression (selectClause.Selector);
    }

    public override void VisitOrderByClause (OrderByClause orderByClause, QueryModel queryModel, int index)
    {
      ArgumentUtility.CheckNotNull ("orderByClause", orderByClause);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      var orderings = from ordering in orderByClause.Orderings
                      let orderByExpression = _stage.PrepareOrderByExpression (ordering.Expression)
                      select new Ordering (orderByExpression, ordering.OrderingDirection);
      SqlStatementBuilder.Orderings.InsertRange (0, orderings);
    }

    public override void VisitJoinClause (JoinClause joinClause, QueryModel queryModel, int index)
    {
      ArgumentUtility.CheckNotNull ("joinClause", joinClause);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      AddFromClause (joinClause, joinClause.InnerSequence);

      var whereCondition = Expression.Equal (joinClause.OuterKeySelector, joinClause.InnerKeySelector);
      SqlStatementBuilder.AddWhereCondition (_stage.PrepareWhereExpression (whereCondition));
    }

    public override void VisitResultOperator (ResultOperatorBase resultOperator, QueryModel queryModel, int index)
    {
      ArgumentUtility.CheckNotNull ("resultOperator", resultOperator);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      if (resultOperator is ContainsResultOperator)
      {
        var sqlSubStatement = GetStatementAndResetBuilder();
        var itemExpression = ((ContainsResultOperator) resultOperator).Item;
        //TODO 2616: Type is not correct here, will be solved by COMMONS-2616
        var subStatementExpression = new SqlSubStatementExpression (sqlSubStatement, itemExpression.Type);
        var sqlInExpression = new SqlBinaryOperatorExpression ("IN", _stage.PrepareItemExpression (itemExpression), subStatementExpression);

        SqlStatementBuilder.SelectProjection = sqlInExpression;
        return;
      }
      else if (resultOperator is CastResultOperator)
        return;
      else if (resultOperator is OfTypeResultOperator)
      {
        SqlStatementBuilder.AddWhereCondition (
            Expression.TypeIs (SqlStatementBuilder.SelectProjection, ((OfTypeResultOperator) resultOperator).SearchedItemType));
        return;
      }

      if (SqlStatementBuilder.TopExpression != null)
      {
        var sqlStatement = SqlStatementBuilder.GetSqlStatement();

        var subStatementTableInfo = new ResolvedSubStatementTableInfo (
            sqlStatement.SelectProjection.Type, // TODO 2616: This type is potentially wrong if a result operator (e.g., Cast) changed the type of the items coming from the SelectProjection. Added a dedicated task for this (COMMONS-2616).
            _generator.GetUniqueIdentifier ("q"), 
            sqlStatement);
        var sqlTable = new SqlTable (subStatementTableInfo);

        GetStatementAndResetBuilder();
        SqlStatementBuilder.SqlTables.Add (sqlTable);
        SqlStatementBuilder.SelectProjection = new SqlTableReferenceExpression (sqlTable);
      }

      if (resultOperator is CountResultOperator)
        SqlStatementBuilder.IsCountQuery = true;
      else if (resultOperator is DistinctResultOperator)
        SqlStatementBuilder.IsDistinctQuery = true;
      else if (resultOperator is FirstResultOperator)
        SqlStatementBuilder.TopExpression = _stage.PrepareTopExpression (Expression.Constant (1));
      else if (resultOperator is SingleResultOperator)
        SqlStatementBuilder.TopExpression = _stage.PrepareTopExpression (Expression.Constant (2));
      else if (resultOperator is TakeResultOperator)
      {
        var expression = ((TakeResultOperator) resultOperator).Count;
        SqlStatementBuilder.TopExpression = _stage.PrepareTopExpression (expression);
      }
      else
        throw new NotSupportedException (string.Format ("{0} is not supported.", resultOperator));
    }

    protected virtual SqlStatement GetStatementAndResetBuilder ()
    {
      var sqlSubStatement = SqlStatementBuilder.GetSqlStatement();
      _sqlStatementBuilder = new SqlStatementBuilder();
      return sqlSubStatement;
    }

    private void AddFromClause (IQuerySource source, Expression fromExpression)
    {
      var preparedFromExpression = _stage.PrepareFromExpression (fromExpression);
      var sqlTableOrJoin = _stage.PrepareSqlTable (preparedFromExpression, source.ItemType);

      _context.AddQuerySourceMapping (source, sqlTableOrJoin);

      var sqlJoinedTable = sqlTableOrJoin as SqlJoinedTable;
      if (sqlJoinedTable != null)
        SqlStatementBuilder.AddWhereCondition (new JoinConditionExpression (sqlJoinedTable));
       
      SqlStatementBuilder.SqlTables.Add (sqlTableOrJoin);
    }
  }
}