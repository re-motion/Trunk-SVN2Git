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
using System.Diagnostics;
using System.Linq.Expressions;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Utilities;

namespace Remotion.Data.Linq.Clauses
{
  public class JoinClause : IClause
  {
    private Type _itemType;
    private string _itemName;
    private Expression _inExpression;
    private Expression _equalityExpression;
    private Expression _onExpression;

    /// <summary>
    /// Initializes a new instance of the <see cref="JoinClause"/> class.
    /// </summary>
    /// <param name="itemName">A name describing the items generated by this <see cref="JoinClause"/>.</param>
    /// <param name="itemType">The type of the items generated by this <see cref="JoinClause"/>.</param>
    /// <param name="inExpression">The in expression that generates the items of this <see cref="JoinClause"/>.</param>
    /// <param name="onExpression">The on expression. TODO: Rename.</param>
    /// <param name="equalityExpression">The equality expression. TODO: Rename.</param>
    public JoinClause (string itemName, Type itemType, Expression inExpression, Expression onExpression, Expression equalityExpression)
    {
      ArgumentUtility.CheckNotNull ("itemName", itemName);
      ArgumentUtility.CheckNotNull ("itemType", itemType);
      ArgumentUtility.CheckNotNull ("inExpression", inExpression);
      ArgumentUtility.CheckNotNull ("onExpression", onExpression);
      ArgumentUtility.CheckNotNull ("equalityExpression", equalityExpression);

      _itemName = itemName;
      _itemType = itemType;
      _inExpression = inExpression;
      _onExpression = onExpression;
      _equalityExpression = equalityExpression;
    }

    /// <summary>
    /// Gets or sets the type of the items generated by this <see cref="JoinClause"/>.
    /// </summary>
    public Type ItemType
    {
      get { return _itemType; }
      set { _itemType = ArgumentUtility.CheckNotNull ("value", value); }
    }

    /// <summary>
    /// Gets or sets a name describing the items generated by this <see cref="JoinClause"/>.
    /// </summary>
    public string ItemName
    {
      get { return _itemName; }
      set { _itemName = ArgumentUtility.CheckNotNullOrEmpty ("value", value); }
    }

    [DebuggerDisplay ("{Remotion.Data.Linq.StringBuilding.FormattingExpressionTreeVisitor.Format (InExpression),nq}")]
    public Expression InExpression
    {
      get { return _inExpression; }
      set { _inExpression = ArgumentUtility.CheckNotNull ("value", value); }
    }

    [DebuggerDisplay ("{Remotion.Data.Linq.StringBuilding.FormattingExpressionTreeVisitor.Format (OnExpression),nq}")]
    public Expression OnExpression
    {
      get { return _onExpression; }
      set { _onExpression = ArgumentUtility.CheckNotNull ("value", value); }
    }

    [DebuggerDisplay ("{Remotion.Data.Linq.StringBuilding.FormattingExpressionTreeVisitor.Format (EqualityExpression),nq}")]
    public Expression EqualityExpression
    {
      get { return _equalityExpression; }
      set { _equalityExpression = ArgumentUtility.CheckNotNull ("value", value); }
    }

    /// <summary>
    /// Accepts the specified visitor by calling one its <see cref="IQueryModelVisitor.VisitJoinClause"/> method.
    /// </summary>
    /// <param name="visitor">The visitor to accept.</param>
    /// <param name="queryModel">The query model in whose context this clause is visited.</param>
    /// <param name="fromClause">The from clause in whose context this clause is visited.</param>
    /// <param name="index">The index of this clause in the <paramref name="fromClause"/>'s <see cref="FromClauseBase.JoinClauses"/> collection.</param>
    public virtual void Accept (IQueryModelVisitor visitor, QueryModel queryModel, FromClauseBase fromClause, int index)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);
      ArgumentUtility.CheckNotNull ("fromClause", fromClause);

      visitor.VisitJoinClause (this, queryModel, fromClause, index);
    }

    /// <summary>
    /// Clones this clause, registering its clone with the <paramref name="cloneContext"/>.
    /// </summary>
    /// <param name="cloneContext">The clones of all query source clauses are registered with this <see cref="CloneContext"/>.</param>
    /// <returns>A clone of this clause.</returns>
    public JoinClause Clone (CloneContext cloneContext)
    {
      ArgumentUtility.CheckNotNull ("cloneContext", cloneContext);

      var clone = new JoinClause (ItemName, ItemType, InExpression, OnExpression, EqualityExpression);
      return clone;
    }

    /// <summary>
    /// Transforms all the expressions in this clause and its child objects via the given <paramref name="transformation"/> delegate.
    /// </summary>
    /// <param name="transformation">The transformation object. This delegate is called for each <see cref="Expression"/> within this
    /// clause, and those expressions will be replaced with what the delegate returns.</param>
    public virtual void TransformExpressions (Func<Expression, Expression> transformation)
    {
      ArgumentUtility.CheckNotNull ("transformation", transformation);
      InExpression = transformation (InExpression);
      OnExpression = transformation (OnExpression);
      EqualityExpression = transformation (EqualityExpression);
    }

    public override string ToString ()
    {
      return string.Format (
          "join {0} {1} in {2} on {3} equals {4}",
          ItemType.Name,
          ItemName,
          FormattingExpressionTreeVisitor.Format (InExpression),
          FormattingExpressionTreeVisitor.Format (OnExpression),
          FormattingExpressionTreeVisitor.Format (EqualityExpression));
    }
  }
}