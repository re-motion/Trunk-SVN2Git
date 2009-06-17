// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Utilities;

namespace Remotion.Data.Linq.Parsing.Structure.IntermediateModel
{
  /// <summary>
  /// Caches a resolved expression in the <see cref="IExpressionNode"/> classes.
  /// </summary>
  public class ResolvedExpressionCache
  {
    private readonly ExpressionResolver _resolver;
    private Expression _cachedExpression;

    public ResolvedExpressionCache (IExpressionNode sourceNode)
    {
      ArgumentUtility.CheckNotNull ("sourceNode", sourceNode);

      _resolver = new ExpressionResolver (sourceNode);
      _cachedExpression = null;
    }

    public Expression GetOrCreate (Func<ExpressionResolver, Expression> generator)
    {
      if (_cachedExpression == null)
        _cachedExpression = generator (_resolver);

      return _cachedExpression;
    }
  }
}