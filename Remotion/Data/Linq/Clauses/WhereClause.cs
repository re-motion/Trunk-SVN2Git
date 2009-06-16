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
using Remotion.Data.Linq.Parsing.ExpressionTreeVisitors;
using Remotion.Utilities;

namespace Remotion.Data.Linq.Clauses
{
  /// <summary>
  /// Represents the where part of a linq query.
  /// example: where a.A = "something useful"
  /// </summary>
  public class WhereClause : IBodyClause
  {
    private readonly LambdaExpression _legacyPredicate;
    private LambdaExpression _simplifiedBoolExpression;
    
    /// <summary>
    /// Initialize a new instance of <see cref="WhereClause"/>
    /// </summary>
    /// <param name="previousClause">The previous clause of type <see cref="IClause"/> in the <see cref="QueryModel"/>.</param>
    /// <param name="legacyPredicate">To be removed.</param>
    /// <param name="predicate">The expression which represents the where conditions.</param>
    public WhereClause (IClause previousClause, LambdaExpression legacyPredicate, Expression predicate)
    {
      // TODO 1219: null check
      ArgumentUtility.CheckNotNull ("predicate", legacyPredicate);
      ArgumentUtility.CheckNotNull ("previousClause", previousClause);
      _legacyPredicate = legacyPredicate;
      Predicate = predicate;
      PreviousClause = previousClause;
    }

    /// <summary>
    /// The previous clause of type <see cref="IClause"/> in the <see cref="QueryModel"/>.
    /// </summary>
    public IClause PreviousClause { get; private set; }

    /// <summary>
    /// The expression which represents the where conditions.
    /// </summary>
    public LambdaExpression LegacyPredicate
    {
      get { return _legacyPredicate; }
    }

    /// <summary>
    /// The expression which represents the where conditions.
    /// </summary>
    public Expression Predicate
    {
      get; private set;
    }

    // TODO 1219: Remove
    public LambdaExpression GetSimplifiedPredicate ()
    {
      if (_simplifiedBoolExpression == null)
        _simplifiedBoolExpression = (LambdaExpression) PartialTreeEvaluatingVisitor.EvaluateIndependentSubtrees (LegacyPredicate);
      return _simplifiedBoolExpression;
    }

    public virtual void Accept (IQueryVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.VisitWhereClause (this);
    }

    /// <summary>
    /// The <see cref="QueryModel"/> to which the <see cref="WhereClause"/> belongs.
    /// </summary>
    public QueryModel QueryModel { get; private set; }

    public void SetQueryModel (QueryModel model)
    {
      ArgumentUtility.CheckNotNull ("model", model);
      if (QueryModel != null)
        throw new InvalidOperationException ("QueryModel is already set");

      QueryModel = model;
    }

    public WhereClause Clone (IClause newPreviousClause)
    {
      return new WhereClause (newPreviousClause, LegacyPredicate, Predicate);
    }

    IBodyClause IBodyClause.Clone (IClause newPreviousClause)
    {
      return Clone (newPreviousClause);
    }
  }
}
