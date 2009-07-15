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
  /// <summary>
  /// Represents the join part of a query, adding new data items and joining them with data items from previous clauses. This can either
  /// be part of <see cref="QueryModel.BodyClauses"/> or of <see cref="GroupJoinClause"/>. The semantics of the <see cref="JoinClause"/>
  /// is that of an inner join, i.e. only combinations where both an input item and a joined item exist are returned.
  /// </summary>
  /// <example>
  /// In C#, the "join" clause in the following sample corresponds to a <see cref="JoinClause"/>. The <see cref="JoinClause"/> adds a new
  /// query source to the query, selecting addresses (called "a") from the source "Addresses". It associates addresses and students by
  /// comparing the students' "AddressID" properties with the addresses' "ID" properties. "a" corresponds to <see cref="ItemName"/> and 
  /// <see cref="ItemType"/>, "Addresses" is <see cref="InnerSequence"/> and the left and right side of the "equals" operator are held by
  /// <see cref="OuterKeySelector"/> and <see cref="InnerKeySelector"/>, respectively:
  /// <code>
  /// var query = from s in Students
  ///             join a in Addresses on s.AdressID equals a.ID
  ///             select new { s, a };
  /// </code>
  /// </example>
  public class JoinClause : IBodyClause, IQuerySource
  {
    private Type _itemType;
    private string _itemName;
    private Expression _innerSequence;
    private Expression _outerKeySelector;
    private Expression _innerKeySelector;

    /// <summary>
    /// Initializes a new instance of the <see cref="JoinClause"/> class.
    /// </summary>
    /// <param name="itemName">A name describing the items generated by this <see cref="JoinClause"/>.</param>
    /// <param name="itemType">The type of the items generated by this <see cref="JoinClause"/>.</param>
    /// <param name="innerSequence">The expression that generates the inner sequence, i.e. the items of this <see cref="JoinClause"/>.</param>
    /// <param name="outerKeySelector">An expression that selects the left side of the comparison by which source items and inner items are joined.</param>
    /// <param name="innerKeySelector">An expression that selects the right side of the comparison by which source items and inner items are joined.</param>
    public JoinClause (string itemName, Type itemType, Expression innerSequence, Expression outerKeySelector, Expression innerKeySelector)
    {
      ArgumentUtility.CheckNotNull ("itemName", itemName);
      ArgumentUtility.CheckNotNull ("itemType", itemType);
      ArgumentUtility.CheckNotNull ("innerSequence", innerSequence);
      ArgumentUtility.CheckNotNull ("outerKeySelector", outerKeySelector);
      ArgumentUtility.CheckNotNull ("innerKeySelector", innerKeySelector);

      _itemName = itemName;
      _itemType = itemType;
      _innerSequence = innerSequence;
      _outerKeySelector = outerKeySelector;
      _innerKeySelector = innerKeySelector;
    }

    /// <summary>
    /// Gets or sets the type of the items generated by this <see cref="JoinClause"/>.
    /// </summary>
    /// <note type="warning">
    /// Changing the <see cref="ItemType"/> of a <see cref="IQuerySource"/> can make all <see cref="QuerySourceReferenceExpression"/> objects that
    /// point to that <see cref="IQuerySource"/> invalid, so the property setter should be used with care.
    /// </note>
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

    /// <summary>
    /// Gets or sets the inner sequence, the expression that generates the inner sequence, i.e. the items of this <see cref="JoinClause"/>.
    /// </summary>
    /// <value>The inner sequence.</value>
    [DebuggerDisplay ("{Remotion.Data.Linq.StringBuilding.FormattingExpressionTreeVisitor.Format (InnerSequence),nq}")]
    public Expression InnerSequence
    {
      get { return _innerSequence; }
      set { _innerSequence = ArgumentUtility.CheckNotNull ("value", value); }
    }

    /// <summary>
    /// Gets or sets the outer key selector, an expression that selects the right side of the comparison by which source items and inner items are joined.
    /// </summary>
    /// <value>The outer key selector.</value>
    [DebuggerDisplay ("{Remotion.Data.Linq.StringBuilding.FormattingExpressionTreeVisitor.Format (OuterKeySelector),nq}")]
    public Expression OuterKeySelector
    {
      get { return _outerKeySelector; }
      set { _outerKeySelector = ArgumentUtility.CheckNotNull ("value", value); }
    }

    /// <summary>
    /// Gets or sets the inner key selector, an expression that selects the left side of the comparison by which source items and inner items are joined.
    /// </summary>
    /// <value>The inner key selector.</value>
    [DebuggerDisplay ("{Remotion.Data.Linq.StringBuilding.FormattingExpressionTreeVisitor.Format (InnerKeySelector),nq}")]
    public Expression InnerKeySelector
    {
      get { return _innerKeySelector; }
      set { _innerKeySelector = ArgumentUtility.CheckNotNull ("value", value); }
    }

    /// <summary>
    /// Accepts the specified visitor by calling one its <see cref="IQueryModelVisitor.VisitJoinClause"/> method.
    /// </summary>
    /// <param name="visitor">The visitor to accept.</param>
    /// <param name="queryModel">The query model in whose context this clause is visited.</param>
    /// <param name="index">The index of this clause in the <paramref name="queryModel"/>'s <see cref="QueryModel.BodyClauses"/> collection.</param>
   public virtual void Accept (IQueryModelVisitor visitor, QueryModel queryModel, int index)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      visitor.VisitJoinClause (this, queryModel, index);
    }

    /// <summary>
    /// Clones this clause, registering its clone with the <paramref name="cloneContext"/>.
    /// </summary>
    /// <param name="cloneContext">The clones of all query source clauses are registered with this <see cref="CloneContext"/>.</param>
    /// <returns>A clone of this clause.</returns>
    public JoinClause Clone (CloneContext cloneContext)
    {
      ArgumentUtility.CheckNotNull ("cloneContext", cloneContext);

      var clone = new JoinClause (ItemName, ItemType, InnerSequence, OuterKeySelector, InnerKeySelector);
      cloneContext.QuerySourceMapping.AddMapping (this, new QuerySourceReferenceExpression (clone));
      return clone;
    }

    IBodyClause IBodyClause.Clone (CloneContext cloneContext)
    {
      return Clone (cloneContext);
    }

    /// <summary>
    /// Transforms all the expressions in this clause and its child objects via the given <paramref name="transformation"/> delegate.
    /// </summary>
    /// <param name="transformation">The transformation object. This delegate is called for each <see cref="Expression"/> within this
    /// clause, and those expressions will be replaced with what the delegate returns.</param>
    public virtual void TransformExpressions (Func<Expression, Expression> transformation)
    {
      ArgumentUtility.CheckNotNull ("transformation", transformation);
      InnerSequence = transformation (InnerSequence);
      OuterKeySelector = transformation (OuterKeySelector);
      InnerKeySelector = transformation (InnerKeySelector);
    }

    public override string ToString ()
    {
      return string.Format (
          "join {0} {1} in {2} on {3} equals {4}",
          ItemType.Name,
          ItemName,
          FormattingExpressionTreeVisitor.Format (InnerSequence),
          FormattingExpressionTreeVisitor.Format (OuterKeySelector),
          FormattingExpressionTreeVisitor.Format (InnerKeySelector));
    }
  }
}