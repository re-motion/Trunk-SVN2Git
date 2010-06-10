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
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Data.Linq.Parsing.ExpressionTreeVisitors;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
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
        QueryModel queryModel,
        ISqlPreparationContext parentPreparationContext,
        ISqlPreparationStage stage,
        UniqueIdentifierGenerator generator,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry)
    {
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);
      ArgumentUtility.CheckNotNull ("stage", stage);
      ArgumentUtility.CheckNotNull ("generator", generator);
      ArgumentUtility.CheckNotNull ("resultOperatorHandlerRegistry", resultOperatorHandlerRegistry);

      var visitor = new SqlPreparationQueryModelVisitor (parentPreparationContext, stage, generator, resultOperatorHandlerRegistry);
      queryModel.Accept (visitor);

      return visitor.GetSqlStatement();
    }

    private readonly ISqlPreparationContext _context;
    private readonly ISqlPreparationStage _stage;

    private readonly SqlStatementBuilder _sqlStatementBuilder;
    private readonly UniqueIdentifierGenerator _generator;
    private readonly ResultOperatorHandlerRegistry _resultOperatorHandlerRegistry;

    protected SqlPreparationQueryModelVisitor (
        ISqlPreparationContext parentContext,
        ISqlPreparationStage stage,
        UniqueIdentifierGenerator generator,
        ResultOperatorHandlerRegistry resultOperatorHandlerRegistry)
    {
      ArgumentUtility.CheckNotNull ("stage", stage);
      ArgumentUtility.CheckNotNull ("generator", generator);
      ArgumentUtility.CheckNotNull ("resultOperatorHandlerRegistry", resultOperatorHandlerRegistry);

      _context = new SqlPreparationContext (parentContext, this);
      _stage = stage;
      _generator = generator;
      _resultOperatorHandlerRegistry = resultOperatorHandlerRegistry;

      _sqlStatementBuilder = new SqlStatementBuilder();
    }

    public ISqlPreparationContext Context
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

    public override void VisitQueryModel (QueryModel queryModel)
    {
      var constantCollection = GetConstantCollectionValue (queryModel);
      if (constantCollection != null)
      {
        SqlStatementBuilder.SelectProjection = Expression.Constant (constantCollection);
        SqlStatementBuilder.DataInfo = queryModel.SelectClause.GetOutputDataInfo();
        VisitResultOperators (queryModel.ResultOperators, queryModel);
      }
      else
        base.VisitQueryModel (queryModel);
    }

    public override void VisitMainFromClause (MainFromClause fromClause, QueryModel queryModel)
    {
      ArgumentUtility.CheckNotNull ("fromClause", fromClause);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      AddQuerySource (fromClause, fromClause.FromExpression);
    }

    public override void VisitAdditionalFromClause (AdditionalFromClause fromClause, QueryModel queryModel, int index)
    {
      ArgumentUtility.CheckNotNull ("fromClause", fromClause);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      AddQuerySource (fromClause, fromClause.FromExpression);
    }

    public override void VisitWhereClause (WhereClause whereClause, QueryModel queryModel, int index)
    {
      ArgumentUtility.CheckNotNull ("whereClause", whereClause);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      var translatedExpression = _stage.PrepareWhereExpression (whereClause.Predicate, _context);
      SqlStatementBuilder.AddWhereCondition (translatedExpression);
    }

    public override void VisitSelectClause (SelectClause selectClause, QueryModel queryModel)
    {
      ArgumentUtility.CheckNotNull ("selectClause", selectClause);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      var preparedExpression = _stage.PrepareSelectExpression (selectClause.Selector, _context);
      if (!(preparedExpression is NamedExpression))
        preparedExpression = new NamedExpression (null, preparedExpression);

      SqlStatementBuilder.SelectProjection = preparedExpression;
      SqlStatementBuilder.DataInfo = selectClause.GetOutputDataInfo();
    }

    public override void VisitOrderByClause (OrderByClause orderByClause, QueryModel queryModel, int index)
    {
      ArgumentUtility.CheckNotNull ("orderByClause", orderByClause);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      var orderings = from ordering in orderByClause.Orderings
                      let orderByExpression = _stage.PrepareOrderByExpression (ordering.Expression, _context)
                      select new Ordering (orderByExpression, ordering.OrderingDirection);
      SqlStatementBuilder.Orderings.InsertRange (0, orderings);
    }

    public override void VisitJoinClause (JoinClause joinClause, QueryModel queryModel, int index)
    {
      ArgumentUtility.CheckNotNull ("joinClause", joinClause);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      AddJoinClause (joinClause);
    }

    public SqlTableBase AddJoinClause (JoinClause joinClause)
    {
      ArgumentUtility.CheckNotNull ("joinClause", joinClause);

      var table = AddQuerySource (joinClause, joinClause.InnerSequence);

      var whereCondition = Expression.Equal (joinClause.OuterKeySelector, joinClause.InnerKeySelector);
      SqlStatementBuilder.AddWhereCondition (_stage.PrepareWhereExpression (whereCondition, _context));

      return table;
    }

    public override void VisitGroupJoinClause (GroupJoinClause groupJoinClause, QueryModel queryModel, int index)
    {
      //the joins for the group join clauses ared added in SqlPreparationContextVisitor on demand
    }

    public override void VisitResultOperator (ResultOperatorBase resultOperator, QueryModel queryModel, int index)
    {
      ArgumentUtility.CheckNotNull ("resultOperator", resultOperator);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      var resultOperatorHandler = _resultOperatorHandlerRegistry.GetItem (resultOperator.GetType());
      resultOperatorHandler.HandleResultOperator (resultOperator, _sqlStatementBuilder, _generator, _stage, _context);
    }

    public SqlTableBase AddQuerySource (IQuerySource source, Expression fromExpression)
    {
      // TODO Review 2773: This is a bug: PrepareFromExpression will remove the orderings, and they will not be appened to the outer statement. Remove the call to PrepareFromExpression (and remove ISqlPreparationStage.PrepareFromExpression), PrepareSqlTable should to the same things anyway
      var preparedFromExpression = _stage.PrepareFromExpression (fromExpression, _context);
      
      // is from expression already a reference to an existing table?
      var existingTableReference = preparedFromExpression as SqlTableReferenceExpression;
      if (existingTableReference != null) // yes, table already exists
      {
        // TODO Review 2773: This must be moved to SqlPreparationFromExpressionVisitor.VisitSqlTableReferenceExpression; i.e., create the same FromExpressionInfo from VisitSqlTableReferenceExpression; also add a flag to FromExpressionInfo to indicate whether the SqlTable is a new table. Check the flag below (see below). The if statement can then be removed, and only the then part stays. Don't forget the unit tests for SqlPreparationFromExpressionVisitor.VisitSqlTableReferenceExpression
        var ret = new FromExpressionInfo (
            existingTableReference.SqlTable,
            new Ordering[0],
            new SqlTableReferenceExpression (existingTableReference.SqlTable),
            null);
        _context.AddExpressionMapping (new QuerySourceReferenceExpression (source), ret.ItemSelector);
        return ret.SqlTable;
      }
      else // no, a new table must be created
      {
        // TODO Review 2773: Rename to PrepareFromExpression
        var fromExpressionInfo = _stage.PrepareSqlTable (preparedFromExpression, source, _context);
        
        if (fromExpressionInfo.WhereCondition != null)
          SqlStatementBuilder.AddWhereCondition (fromExpressionInfo.WhereCondition);

        foreach (var ordering in fromExpressionInfo.ExtractedOrderings)
          SqlStatementBuilder.Orderings.Add (ordering);

        // TODO Review 2773: Check the flag here (i.e., if (fromExpressionInfo.IsNewTable) SqlStatementBuilder.SqlTables.Add (fromExpressionInfo.SqlTable);
        SqlStatementBuilder.SqlTables.Add (fromExpressionInfo.SqlTable);
        _context.AddExpressionMapping (new QuerySourceReferenceExpression (source), fromExpressionInfo.ItemSelector);
        return fromExpressionInfo.SqlTable;
      }
    }

    private ICollection GetConstantCollectionValue (QueryModel queryModel)
    {
      var fromExpressionAsConstant = (queryModel.MainFromClause.FromExpression) as ConstantExpression;
      if (queryModel.IsIdentityQuery() && fromExpressionAsConstant != null && typeof (ICollection).IsAssignableFrom (fromExpressionAsConstant.Type))
        return (ICollection) fromExpressionAsConstant.Value;
      else
        return null;
    }
  }
}