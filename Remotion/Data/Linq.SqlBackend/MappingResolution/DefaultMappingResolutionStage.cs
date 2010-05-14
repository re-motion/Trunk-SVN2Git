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
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.MappingResolution
{
  /// <summary>
  /// Provides a default implementation of <see cref="IMappingResolutionStage"/>.
  /// </summary>
  public class DefaultMappingResolutionStage : IMappingResolutionStage
  {
    private readonly IMappingResolver _resolver;
    private readonly UniqueIdentifierGenerator _uniqueIdentifierGenerator;

    public DefaultMappingResolutionStage (IMappingResolver resolver, UniqueIdentifierGenerator uniqueIdentifierGenerator)
    {
      ArgumentUtility.CheckNotNull ("resolver", resolver);
      ArgumentUtility.CheckNotNull ("uniqueIdentifierGenerator", uniqueIdentifierGenerator);
      
      _uniqueIdentifierGenerator = uniqueIdentifierGenerator;
      _resolver = resolver;
    }

    public virtual Expression ResolveSelectExpression (Expression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      var resolvedExpression = ResolveExpression (expression);
      return ApplyContext (resolvedExpression, SqlExpressionContext.ValueRequired);
    }

    public virtual Expression ResolveWhereExpression (Expression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      var resolvedExpression = ResolveExpression (expression);
      return ApplyContext (resolvedExpression, SqlExpressionContext.PredicateRequired);
    }

    public virtual Expression ResolveOrderingExpression (Expression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      var resolvedExpression = ResolveExpression (expression);
      return ApplyContext (resolvedExpression, SqlExpressionContext.SingleValueRequired);
    }

    public virtual Expression ResolveTopExpression (Expression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      var resolvedExpression = ResolveExpression (expression);
      return ApplyContext (resolvedExpression, SqlExpressionContext.SingleValueRequired);
    }

    public virtual IResolvedTableInfo ResolveTableInfo (ITableInfo tableInfo)
    {
      ArgumentUtility.CheckNotNull ("tableInfo", tableInfo);

      var resolvedTableInfo = ResolvingTableInfoVisitor.ResolveTableInfo (tableInfo, _resolver, _uniqueIdentifierGenerator, this);
      return (IResolvedTableInfo) ApplyContext (resolvedTableInfo, SqlExpressionContext.ValueRequired);
    }

    public virtual ResolvedJoinInfo ResolveJoinInfo (IJoinInfo joinInfo)
    {
      ArgumentUtility.CheckNotNull ("joinInfo", joinInfo);

      var resolvedJoinInfo = ResolvingJoinInfoVisitor.ResolveJoinInfo (joinInfo, _resolver, _uniqueIdentifierGenerator, this);
      return (ResolvedJoinInfo) ApplyContext (resolvedJoinInfo, SqlExpressionContext.ValueRequired);
    }

    public virtual SqlStatement ResolveSqlStatement (SqlStatement sqlStatement)
    {
      ArgumentUtility.CheckNotNull ("sqlStatement", sqlStatement);

      return SqlStatementResolver.ResolveExpressions (this, sqlStatement);
    }

    public virtual SqlEntityExpression ResolveCollectionSourceExpression (Expression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      var resolvedExpression = ResolveExpression (expression);
      return (SqlEntityExpression) ApplyContext (resolvedExpression, SqlExpressionContext.ValueRequired);
    }

    public virtual SqlEntityExpression ResolveEntityRefMemberExpression (SqlEntityRefMemberExpression expression, IJoinInfo joinInfo)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      ArgumentUtility.CheckNotNull ("joinInfo", joinInfo);

      var join = expression.SqlTable.GetOrAddLeftJoin (joinInfo, expression.MemberInfo);
      join.JoinInfo = ResolveJoinInfo (join.JoinInfo);
      var sqlTableReferenceExpression = new SqlTableReferenceExpression (join);
      
      return (SqlEntityExpression) ResolveExpression (sqlTableReferenceExpression);
    }

    public Expression ResolveTableReferenceExpression (SqlTableReferenceExpression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      return SqlTableReferenceResolver.ResolveTableReference (expression, _resolver, _uniqueIdentifierGenerator);
    }

    public virtual Expression ApplyContext (Expression expression, SqlExpressionContext context)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      return SqlContextExpressionVisitor.ApplySqlExpressionContext (expression, context, this);
    }

    public virtual SqlStatement ApplySelectionContext (SqlStatement sqlStatement, SqlExpressionContext context)
    {
      ArgumentUtility.CheckNotNull ("sqlStatement", sqlStatement);

      return SqlContextSelectionAdjuster.ApplyContext (sqlStatement, context, this);
    }

    public virtual ITableInfo ApplyContext (ITableInfo tableInfo, SqlExpressionContext context)
    {
      ArgumentUtility.CheckNotNull ("tableInfo", tableInfo);

      return SqlContextTableInfoVisitor.ApplyContext (tableInfo, context, this);
    }

    public virtual IJoinInfo ApplyContext (IJoinInfo joinInfo, SqlExpressionContext context)
    {
      ArgumentUtility.CheckNotNull ("joinInfo", joinInfo);

      return SqlContextJoinInfoVisitor.ApplyContext (joinInfo, context, this);
    }

    protected virtual Expression ResolveExpression (Expression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      return ResolvingExpressionVisitor.ResolveExpression (expression, _resolver, _uniqueIdentifierGenerator, this);
    }
  }
}