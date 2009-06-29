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
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Utilities;

namespace Remotion.Data.Linq.Clauses
{
  /// <summary>
  /// Extends <see cref="FromClauseBase"/>. <see cref="AdditionalFromClause"/> is used for from clauses which is no <see cref="MainFromClause"/>.
  /// example:from a in queryable1 from b in queryable (the additional <see cref="AdditionalFromClause"/> is the second from)
  /// </summary>
  public class AdditionalFromClause : FromClauseBase, IBodyClause
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="AdditionalFromClause"/> class.
    /// </summary>
    /// <param name="itemName">A name describing the items generated by the from clause.</param>
    /// <param name="itemType">The type of the items generated by the from clause.</param>
    /// <param name="fromExpression">The <see cref="Expression"/> generating the items of this from clause.</param>
    public AdditionalFromClause (string itemName, Type itemType, Expression fromExpression)
        : base (
            ArgumentUtility.CheckNotNullOrEmpty ("itemName", itemName),
            ArgumentUtility.CheckNotNull ("itemType", itemType),
            ArgumentUtility.CheckNotNull ("fromExpression", fromExpression))
    {
    }
    
    public override void Accept (IQueryModelVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.VisitAdditionalFromClause (this);
    }

    public virtual AdditionalFromClause Clone (CloneContext cloneContext)
    {
      ArgumentUtility.CheckNotNull ("cloneContext", cloneContext);

      var newFromExpression = ReferenceReplacingExpressionTreeVisitor.ReplaceClauseReferences (FromExpression, cloneContext);
      var result = new AdditionalFromClause (ItemName, ItemType, newFromExpression);
      cloneContext.ClauseMapping.AddMapping (this, new QuerySourceReferenceExpression(result));
      result.AddClonedJoinClauses (JoinClauses, cloneContext);
      return result;
    }

    IBodyClause IBodyClause.Clone (CloneContext cloneContext)
    {
      return Clone (cloneContext);
    }
  }
}
