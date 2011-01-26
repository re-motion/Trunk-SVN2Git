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
using System.Linq.Expressions;
using Remotion.Data.Linq.Parsing.ExpressionTreeVisitors.ExpressionTransformation;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.Parsing.ExpressionTreeVisitors
{
  /// <summary>
  /// Applies <see cref="ExpressionTransformation"/> delegates obtained from an <see cref="IExpressionTranformationProvider"/> to an expression tree. 
  /// The transformations occur in post-order (transforming child <see cref="Expression"/> nodes before parent nodes). When a transformation changes 
  /// the current <see cref="Expression"/>, its child nodes and itself will be revisited (and may be transformed again).
  /// </summary>
  public class TransformingExpressionTreeVisitor : ExpressionTreeVisitor
  {
    public static Expression Transform (Expression expression, IExpressionTranformationProvider tranformationProvider)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      ArgumentUtility.CheckNotNull ("tranformationProvider", tranformationProvider);
      
      var visitor = new TransformingExpressionTreeVisitor (tranformationProvider);
      return visitor.VisitExpression (expression);
    }

    private readonly IExpressionTranformationProvider _tranformationProvider;
    
    protected TransformingExpressionTreeVisitor (IExpressionTranformationProvider tranformationProvider)
    {
      ArgumentUtility.CheckNotNull ("tranformationProvider", tranformationProvider);

      _tranformationProvider = tranformationProvider;
    }

    public override Expression VisitExpression (Expression expression)
    {
      var newExpression = base.VisitExpression (expression);

      var transformations = _tranformationProvider.GetTransformations (newExpression);

      foreach (var transformation in transformations)
      {
        var transformedExpression = transformation (newExpression);
        if (transformedExpression != newExpression)
          return VisitExpression (transformedExpression);
      }

      return newExpression;
    }
  }
}