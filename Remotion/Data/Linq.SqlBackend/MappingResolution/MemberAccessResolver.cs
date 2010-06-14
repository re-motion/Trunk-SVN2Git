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
using System.Reflection;
using Remotion.Data.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Data.Linq.Parsing;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.MappingResolution
{
  /// <summary>
  /// <see cref="MemberAccessResolver"/> is used by <see cref="DefaultMappingResolutionStage"/> to resolve <see cref="MemberInfo"/>s applied to
  /// expressions. The <see cref="MemberAccessResolver"/> class assumes that its input expression has already been resolved, and it may return a
  /// result that itself needs to be resolved again.
  /// </summary>
  public class MemberAccessResolver : ThrowingExpressionTreeVisitor, IUnresolvedSqlExpressionVisitor, INamedExpressionVisitor, IResolvedSqlExpressionVisitor
  {
    private readonly MemberInfo _memberInfo;
    private readonly IMappingResolver _mappingResolver;
    private readonly IMappingResolutionStage _stage;
    private readonly IMappingResolutionContext _context;

    public static Expression ResolveMemberAccess (Expression resolvedSourceExpression, MemberInfo memberInfo, IMappingResolver mappingResolver, IMappingResolutionStage mappingResolutionStage, IMappingResolutionContext mappingResolutionContext)
    {
      ArgumentUtility.CheckNotNull ("resolvedSourceExpression", resolvedSourceExpression);
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);
      ArgumentUtility.CheckNotNull ("mappingResolver", mappingResolver);
      ArgumentUtility.CheckNotNull ("mappingResolutionStage", mappingResolutionStage);
      ArgumentUtility.CheckNotNull ("mappingResolutionContext", mappingResolutionContext);

      var resolver = new MemberAccessResolver (memberInfo, mappingResolver, mappingResolutionStage, mappingResolutionContext);
      return resolver.VisitExpression (resolvedSourceExpression);
    }

    protected MemberAccessResolver (MemberInfo memberInfo, IMappingResolver mappingResolver, IMappingResolutionStage stage, IMappingResolutionContext context)
    {
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);
      ArgumentUtility.CheckNotNull ("mappingResolver", mappingResolver);
      ArgumentUtility.CheckNotNull ("stage", stage);
      ArgumentUtility.CheckNotNull ("context", context);

      _memberInfo = memberInfo;
      _mappingResolver = mappingResolver;
      _stage = stage;
      _context = context;
    }

    protected override Exception CreateUnhandledItemException<T> (T unhandledItem, string visitMethod)
    {
      throw new NotSupportedException (
          String.Format (
              "Cannot resolve member '{0}' applied to expression '{1}'; the expression type '{2}' is not supported in member expressions.",
              _memberInfo.Name,
              FormattingExpressionTreeVisitor.Format ((Expression) (object) unhandledItem),
              unhandledItem.GetType ().Name));
    }

    protected override Expression VisitUnaryExpression (UnaryExpression expression)
    {
      if (expression.NodeType == ExpressionType.Convert)
        return VisitExpression (expression.Operand);
      else
        return base.VisitUnaryExpression (expression);
    }

    public Expression VisitSqlEntityRefMemberExpression (SqlEntityRefMemberExpression expression)
    {
      var unresolvedJoinInfo = new UnresolvedJoinInfo (expression.OriginatingEntity, expression.MemberInfo, JoinCardinality.One);
      var entityExpression = _stage.ResolveEntityRefMemberExpression (expression, unresolvedJoinInfo, _context);
      return VisitExpression (entityExpression);
    }

    public Expression VisitNamedExpression (NamedExpression expression)
    {
      return VisitExpression (expression.Expression);
    }

    protected override Expression VisitNewExpression (NewExpression expression)
    {
      var binding = Parsing.ExpressionTreeVisitors.MemberBindings.MemberBinding.Bind (_memberInfo, expression);
      var membersAndAssignedExpressions = expression.Members.Select ((m, i) => new { Member = m, Argument = expression.Arguments[i] });
      return membersAndAssignedExpressions.Single (c => binding.MatchesReadAccess(c.Member)).Argument;
    }

    public Expression VisitSqlEntityExpression (SqlEntityExpression expression)
    {
      var type = ReflectionUtility.GetFieldOrPropertyType (_memberInfo);
      if (typeof (IEnumerable).IsAssignableFrom (type) && type != typeof (string))
      {
        var message = string.Format (
            "The member '{0}.{1}' describes a collection and can only be used in places where collections are allowed.",
            _memberInfo.DeclaringType.Name,
            _memberInfo.Name);
        throw new NotSupportedException (message);
      }

      return _mappingResolver.ResolveMemberExpression (expression, _memberInfo);
    }

    public Expression VisitSqlColumnExpression (SqlColumnExpression expression)
    {
      return _mappingResolver.ResolveMemberExpression (expression, _memberInfo);
    }
    
    Expression IUnresolvedSqlExpressionVisitor.VisitSqlTableReferenceExpression (SqlTableReferenceExpression expression)
    {
      return VisitUnknownExpression (expression);
    }

    Expression IUnresolvedSqlExpressionVisitor.VisitSqlEntityConstantExpression (SqlEntityConstantExpression expression)
    {
      return VisitUnknownExpression (expression);
    }


   
  }
}