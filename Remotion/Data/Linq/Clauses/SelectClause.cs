// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Remotion.Data.Linq.Clauses;
using Remotion.Utilities;
using System.Linq.Expressions;

namespace Remotion.Data.Linq.Clauses
{
  /// <summary>
  /// Represents the select part of a linq query.
  /// example: select expression
  /// </summary>
  public class SelectClause : ISelectGroupClause
  {
    private readonly LambdaExpression _projectionExpression;
    private readonly List<ResultModifierClause> _resultModifierData = new List<ResultModifierClause>();
    
    /// <summary>
    /// Initialize a new instance of <see cref="SelectClause"/>.
    /// </summary>
    /// <param name="previousClause">The previous clause of type <see cref="IClause"/> in the <see cref="QueryModel"/>.</param>
    /// <param name="projectionExpression">The projection within the select part of the linq query.</param>
    public SelectClause (IClause previousClause, LambdaExpression projectionExpression)
    {
      ArgumentUtility.CheckNotNull ("previousClause", previousClause);
      ArgumentUtility.CheckNotNull ("projectionExpression", projectionExpression);

      PreviousClause = previousClause;
      _projectionExpression = projectionExpression;
    }
    
    /// <summary>
    /// The previous clause of type <see cref="IClause"/> in the <see cref="QueryModel"/>.
    /// </summary>
    public IClause PreviousClause { get; private set; }

    /// <summary>
    /// The projection within the select part of the linq query.
    /// </summary>
    public LambdaExpression ProjectionExpression
    {
      get { return _projectionExpression; }
    }

    public ReadOnlyCollection<ResultModifierClause> ResultModifierClauses
    {
      get { return _resultModifierData.AsReadOnly(); }
    }

    public void AddResultModifierData (ResultModifierClause resultModifierData)
    {
      ArgumentUtility.CheckNotNull ("resultModifierData", resultModifierData);
      _resultModifierData.Add (resultModifierData);
    }

    public virtual void Accept (IQueryVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.VisitSelectClause (this);
    }

    public SelectClause Clone (IClause newPreviousClause)
    {
      var clone = new SelectClause (newPreviousClause, ProjectionExpression);
      IClause previousClause = clone;
      
      foreach (var resultModifierData in ResultModifierClauses)
      {
        var resultModifierClauseClone = resultModifierData.Clone (previousClause, clone);
        clone.AddResultModifierData (resultModifierClauseClone);
        previousClause = resultModifierClauseClone;
      }

      return clone;
    }

    ISelectGroupClause ISelectGroupClause.Clone (IClause newPreviousClause)
    {
      return Clone (newPreviousClause);
    }
  }
}
