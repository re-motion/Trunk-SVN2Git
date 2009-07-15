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
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Parsing;
using Remotion.Utilities;

namespace Remotion.Data.Linq.Clauses
{
  /// <summary>
  /// Represents the join part of a query, adding new data items and joining them with data items from previous clauses. In contrast to 
  /// <see cref="Clauses.JoinClause"/>, the <see cref="GroupJoinClause"/> does not provide access to the individual items of the joined query source.
  /// Instead, it provides access to all joined items for each item coming from the previous clauses, thus grouping them together. The semantics
  /// of this join is so that for all input items, a joined sequence is returned. That sequence can be empty if no joined items are available.
  /// </summary>
  /// <example>
  /// In C#, the "into" clause in the following sample corresponds to a <see cref="GroupJoinClause"/>. The "join" part before that is encapsulated
  /// as a <see cref="Clauses.JoinClause"/> held in <see cref="JoinClause"/>. The <see cref="JoinClause"/> adds a new query source to the query 
  /// ("addresses"), but the item type of that query source is <see cref="IEnumerable{T}"/>, not "Address". Therefore, it can be
  /// used in the <see cref="FromClauseBase.FromExpression"/> of an <see cref="AdditionalFromClause"/> to extract the single items.
  /// <code>
  /// var query = from s in Students
  ///             join a in Addresses on s.AdressID equals a.ID into addresses
  ///             from a in addresses
  ///             select new { s, a };
  /// </code>
  /// </example>
  public class GroupJoinClause : IQuerySource, IBodyClause
  {
    private string _itemName;
    private Type _itemType;
    private JoinClause _joinClause;

    public GroupJoinClause (string itemName, Type itemType, JoinClause joinClause)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemName", itemName);
      ArgumentUtility.CheckNotNull ("itemType", itemType);
      ArgumentUtility.CheckNotNull ("joinClause", joinClause);

      ItemName = itemName;
      ItemType = itemType;
      JoinClause = joinClause;
    }

    /// <summary>
    /// Gets or sets a name describing the items generated by this <see cref="GroupJoinClause"/>. This is usually an <see cref="IEnumerable{T}"/>.
    /// </summary>
    public string ItemName
    {
      get { return _itemName; }
      set { _itemName = ArgumentUtility.CheckNotNull ("value", value); }
    }
    
    /// <summary>
    /// Gets or sets the type of the items generated by this <see cref="GroupJoinClause"/>.
    /// </summary>
    /// <note type="warning">
    /// Changing the <see cref="ItemType"/> of a <see cref="IQuerySource"/> can make all <see cref="QuerySourceReferenceExpression"/> objects that
    /// point to that <see cref="IQuerySource"/> invalid, so the property setter should be used with care.
    /// </note>
    public Type ItemType
    {
      get { return _itemType; }
      set 
      {
        ArgumentUtility.CheckNotNull ("value", value);
        try
        {
          ParserUtility.GetItemTypeOfIEnumerable (value);
        }
        catch (ArgumentTypeException)
        {
          var message = string.Format ("Expected a type implementing IEnumerable<T>, but found '{0}'.", value.FullName);
          throw new ArgumentTypeException (message, "value", typeof (IEnumerable<>), value);
        }

        _itemType = value;
      }
    }

    /// <summary>
    /// Gets or sets the inner join clause of this <see cref="GroupJoinClause"/>. The <see cref="JoinClause"/> represents the actual join operation
    /// performed by this clause; its results are then grouped by this clause before streaming them to subsequent clauses. 
    /// <see cref="QuerySourceReferenceExpression"/> objects outside the <see cref="GroupJoinClause"/> must not point to <see cref="JoinClause"/> 
    /// because the items generated by it are only available in grouped form from outside this clause.
    /// </summary>
    public JoinClause JoinClause
    {
      get { return _joinClause; }
      set { _joinClause = ArgumentUtility.CheckNotNull ("value", value); }
    }

    /// <summary>
    /// Transforms all the expressions in this clause and its child objects via the given <paramref name="transformation"/> delegate.
    /// </summary>
    /// <param name="transformation">The transformation object. This delegate is called for each <see cref="Expression"/> within this
    /// clause, and those expressions will be replaced with what the delegate returns.</param>
    public void TransformExpressions (Func<Expression, Expression> transformation)
    {
      ArgumentUtility.CheckNotNull ("transformation", transformation);
      JoinClause.TransformExpressions (transformation);
    }

    /// <summary>
    /// Accepts the specified visitor by calling its <see cref="IQueryModelVisitor.VisitGroupJoinClause"/> method.
    /// </summary>
    /// <param name="visitor">The visitor to accept.</param>
    /// <param name="queryModel">The query model in whose context this clause is visited.</param>
    /// <param name="index">The index of this clause in the <paramref name="queryModel"/>'s <see cref="QueryModel.BodyClauses"/> collection.</param>
    public void Accept (IQueryModelVisitor visitor, QueryModel queryModel, int index)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      visitor.VisitGroupJoinClause (this, queryModel, index);
    }

    /// <summary>
    /// Clones this clause, registering its clone with the <paramref name="cloneContext"/>.
    /// </summary>
    /// <param name="cloneContext">The clones of all query source clauses are registered with this <see cref="CloneContext"/>.</param>
    /// <returns>A clone of this clause.</returns>
    public GroupJoinClause Clone (CloneContext cloneContext)
    {
      ArgumentUtility.CheckNotNull ("cloneContext", cloneContext);

      var clone = new GroupJoinClause (ItemName, ItemType, JoinClause.Clone (cloneContext));
      cloneContext.QuerySourceMapping.AddMapping (this, new QuerySourceReferenceExpression (clone));
      return clone;
    }

    IBodyClause IBodyClause.Clone (CloneContext cloneContext)
    {
      return Clone (cloneContext);
    }

    public override string ToString ()
    {
      return string.Format ("{0} into {1} {2}", JoinClause, ItemType.Name, ItemName);
    }
  }
}