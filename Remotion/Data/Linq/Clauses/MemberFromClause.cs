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
using Remotion.Data.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Utilities;

namespace Remotion.Data.Linq.Clauses
{
  /// <summary>
  /// Extends <see cref="FromClauseBase"/>. Similar to <see cref="AdditionalFromClause"/> but in contrast its expression body must contain a <see cref="MemberExpression"/>
  /// example: from a in queryable.Queryable
  /// </summary>
  public class MemberFromClause : AdditionalFromClause
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MemberFromClause"/> class.
    /// </summary>
    /// <param name="itemName">A name describing the items generated by the from clause.</param>
    /// <param name="itemType">The type of the items generated by the from clause.</param>
    /// <param name="fromExpression">The <see cref="MemberExpression"/> generating the items of this from clause.</param>
    public MemberFromClause (string itemName, Type itemType, MemberExpression fromExpression)
      : base (ArgumentUtility.CheckNotNullOrEmpty ("itemName", itemName),
          ArgumentUtility.CheckNotNull ("itemType", itemType),
          ArgumentUtility.CheckNotNull ("fromExpression", fromExpression))
    {
    }

    [DebuggerDisplay ("{Remotion.Data.Linq.StringBuilding.FormattingExpressionTreeVisitor.Format (MemberExpression),nq}")]
    public MemberExpression MemberExpression
    {
      get { return (MemberExpression) FromExpression; }
      set { FromExpression = ArgumentUtility.CheckNotNull ("value", value); }
    }

    [DebuggerDisplay ("{Remotion.Data.Linq.StringBuilding.FormattingExpressionTreeVisitor.Format (FromExpression),nq}")]
    public override Expression FromExpression
    {
      get { return base.FromExpression; }
      set
      {
        ArgumentUtility.CheckNotNullAndType ("value", value, typeof (MemberExpression));
        base.FromExpression = value;
      }
    }

    public override AdditionalFromClause Clone (CloneContext cloneContext)
    {
      ArgumentUtility.CheckNotNull ("cloneContext", cloneContext);

      var newFromExpression = CloneExpressionTreeVisitor.ReplaceClauseReferences (FromExpression, cloneContext);
      var result = new MemberFromClause (ItemName, ItemType, (MemberExpression) newFromExpression);
      cloneContext.ClonedClauseMapping.AddMapping (this, result);
      result.AddClonedJoinClauses (JoinClauses, cloneContext);
      return result;
    }
  }
}
