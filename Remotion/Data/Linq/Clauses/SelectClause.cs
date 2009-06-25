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
using System.Collections.Generic;
using System.Diagnostics;
using Remotion.Collections;
using Remotion.Data.Linq.Clauses.ExecutionStrategies;
using Remotion.Data.Linq.Clauses.ExpressionTreeVisitors;
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
    private IClause _previousClause;
    private Expression _selector;

    /// <summary>
    /// Initialize a new instance of <see cref="SelectClause"/>.
    /// </summary>
    /// <param name="previousClause">The previous clause of type <see cref="IClause"/> in the <see cref="QueryModel"/>.</param>
    /// <param name="selector">The projection within the select part of the linq query.</param>
    public SelectClause (IClause previousClause, Expression selector)
    {
      ArgumentUtility.CheckNotNull ("selector", selector);

      PreviousClause = previousClause;
      _selector = selector;
      ResultModifications = new ObservableCollection<ResultModificationBase> ();
      ResultModifications.ItemInserted += CheckForNullValues;
      ResultModifications.ItemSet += CheckForNullValues;

    }
    
    /// <summary>
    /// The previous clause of type <see cref="IClause"/> in the <see cref="QueryModel"/>.
    /// </summary>
    public IClause PreviousClause
    {
      get { return _previousClause; }
      set { _previousClause = value; }
    }

    [DebuggerDisplay ("{Remotion.Data.Linq.StringBuilding.FormattingExpressionTreeVisitor.Format (Selector),nq}")]
    public Expression Selector 
    {
      get { return _selector; }
      set { _selector = ArgumentUtility.CheckNotNull ("value", value); }
    }

    public ObservableCollection<ResultModificationBase> ResultModifications { get; private set; }

    public virtual void Accept (IQueryVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.VisitSelectClause (this);
    }

    public SelectClause Clone (CloneContext cloneContext)
    {
      ArgumentUtility.CheckNotNull ("cloneContext", cloneContext);

      var newSelector = CloneExpressionTreeVisitor.ReplaceClauseReferences (Selector, cloneContext);
      var result = new SelectClause (null, newSelector);
      cloneContext.ClonedClauseMapping.AddMapping (this, result);
      foreach (var resultModification in ResultModifications)
      {
        var resultModificationClone = resultModification.Clone (cloneContext);
        result.ResultModifications.Add (resultModificationClone);
      }

      return result;
    }

    ISelectGroupClause ISelectGroupClause.Clone (CloneContext cloneContext)
    {
      return Clone (cloneContext);
    }

    public IExecutionStrategy GetExecutionStrategy ()
    {
      if (ResultModifications.Count > 0)
        return ResultModifications[ResultModifications.Count - 1].ExecutionStrategy;
      else
        return CollectionExecutionStrategy.Instance;
    }

    private void CheckForNullValues (object sender, ObservableCollectionChangedEventArgs<ResultModificationBase> e)
    {
      ArgumentUtility.CheckNotNull ("e.Item", e.Item);
    }
    
  }
}
