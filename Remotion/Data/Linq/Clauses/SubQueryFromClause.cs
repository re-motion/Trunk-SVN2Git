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
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Data.Linq.DataObjectModel;
using Remotion.Data.Linq.Parsing;
using Remotion.Utilities;

namespace Remotion.Data.Linq.Clauses
{
  /// <summary>
  /// Extends the <see cref="FromClauseBase"/>. <see cref="SubQueryFromClause"/> is used for from clauses which are part of a subquery.
  /// Represents a from clause which is part of a subquery in another from clause.
  /// example: from identifier in (from identifier in datasource where ... select ...)
  /// </summary>
  public class SubQueryFromClause : AdditionalFromClause
  {
    private SubQuery _fromSource;
    private QueryModel _subQueryModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubQueryFromClause"/> class.
    /// </summary>
    /// <param name="itemName">A name describing the items generated by the from clause.</param>
    /// <param name="itemType">The type of the items generated by the from clause.</param>
    /// <param name="subQueryExpression">The sub query generating the items of this from clause.</param>
    public SubQueryFromClause (string itemName, Type itemType, SubQueryExpression subQueryExpression)
        : base (itemName, itemType, subQueryExpression)
    {
      ArgumentUtility.CheckNotNull ("subQueryExpression", subQueryExpression);

      SubQueryModel = subQueryExpression.QueryModel;
    }

    /// <summary>
    /// The subquery which contains the <see cref="SubQueryFromClause"/>with is represented by a new <see cref="QueryModel"/>.<see cref="QueryModel"/>
    /// </summary>
    public QueryModel SubQueryModel
    {
      get { return _subQueryModel; }
      set
      {
        _subQueryModel = ArgumentUtility.CheckNotNull ("value", value);
        _fromSource = new SubQuery (value, ParseMode.SubQueryInFrom, ItemName);
      }
    }

    public override Type GetQuerySourceType ()
    {
      return null;
    }

    public override IColumnSource GetColumnSource (IDatabaseInfo databaseInfo)
    {
      return _fromSource;
    }

    public override AdditionalFromClause Clone (CloneContext cloneContext)
    {
      ArgumentUtility.CheckNotNull ("cloneContext", cloneContext);

      var clonedSubQueryExpression = CloneExpressionTreeVisitor.ReplaceClauseReferences (FromExpression, cloneContext);
      var result = new SubQueryFromClause (ItemName, ItemType, (SubQueryExpression) clonedSubQueryExpression);
      cloneContext.ClonedClauseMapping.AddMapping (this, result);
      result.AddClonedJoinClauses (JoinClauses, cloneContext);
      return result;
    }
  }
}