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
namespace Remotion.Data.Linq.Clauses
{
  /// <summary>
  /// Represents a clause within the <see cref="QueryModel"/>. Implemented by <see cref="MainFromClause"/>, <see cref="ISelectGroupClause"/>, 
  /// <see cref="IBodyClause"/>, and <see cref="JoinClause"/>.
  /// </summary>
  public interface IClause
  {
    /// <summary>
    /// Transforms all the expressions in this clause and its child objects via the given <paramref name="transformation"/> delegate.
    /// </summary>
    /// <param name="transformation">The transformation object. This delegate is called for each <see cref="Expression"/> within this 
    /// clause, and those expressions will be replaced with what the delegate returns.</param>
    void TransformExpressions (Func<Expression, Expression> transformation);
  }
}
