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

namespace Remotion.Data.Linq.SqlBackend.MappingResolution
{
  /// <summary>
  /// <see cref="ResolvingSelectExpressionVisitor"/> is used to resolve sql select projection expressions in the mapping resolutin stage.
  /// </summary>
  public class ResolvingSelectExpressionVisitor : ResolvingExpressionVisitor
  {
    public new static Expression ResolveExpression (
        Expression expression, IMappingResolver resolver, IMappingResolutionStage stage, IMappingResolutionContext context)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      ArgumentUtility.CheckNotNull ("resolver", resolver);
      ArgumentUtility.CheckNotNull ("stage", stage);
      ArgumentUtility.CheckNotNull ("context", context);

      var visitor = new ResolvingSelectExpressionVisitor (resolver, stage, context);
      var result = visitor.VisitExpression (expression);
      return result;
    }

    protected ResolvingSelectExpressionVisitor (IMappingResolver resolver, IMappingResolutionStage stage, IMappingResolutionContext context)
        : base(resolver, stage, context)
    {
    }

    public override Expression VisitSqlSubStatementExpression (SqlSubStatementExpression expression)
    {
      return base.VisitSqlSubStatementExpression (expression);
    }

  }
}