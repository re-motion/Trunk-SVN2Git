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
using Remotion.Data.Linq.Parsing;
using Remotion.Utilities;
using Remotion.Data.Linq.DataObjectModel;

namespace Remotion.Data.Linq.Clauses
{
  /// <summary>
  /// Extends the <see cref="FromClauseBase"/>. <see cref="SubQueryFromClause"/> is used for from clauses which are part of a subquery.
  /// Represents a from clause which is part of a subquery in another from clause.
  /// example: from identifier in (from identifier in datasource where ... select ...)
  /// </summary>
  public class SubQueryFromClause : FromClauseBase, IBodyClause
  {
    private readonly SubQuery _fromSource;

    /// <summary>
    /// Initialize a new instance of <see cref="QueryModel"/>
    /// </summary>
    /// <param name="previousClause">The previous clause of type <see cref="IClause"/> in the <see cref="QueryModel"/>.</param>
    /// <param name="identifier">The identifier of the expression which represents the subquery.</param>
    /// <param name="subQuery">The subquery which contains the <see cref="SubQueryFromClause"/>with is represented by a new <see cref="QueryModel"/>.<see cref="QueryModel"/>.</param>
    public SubQueryFromClause (IClause previousClause, ParameterExpression identifier, QueryModel subQuery)
        : base (previousClause, identifier)
    {
      ArgumentUtility.CheckNotNull ("previousClause", previousClause);
      ArgumentUtility.CheckNotNull ("identifier", identifier);
      ArgumentUtility.CheckNotNull ("subQuery", subQuery);

      SubQueryModel = subQuery;

      _fromSource = new SubQuery (SubQueryModel, ParseMode.SubQueryInFrom, Identifier.Name);
    }

    /// <summary>
    /// The subquery which contains the <see cref="SubQueryFromClause"/>with is represented by a new <see cref="QueryModel"/>.<see cref="QueryModel"/>
    /// </summary>
    public QueryModel SubQueryModel { get; private set; }

    public override void Accept (IQueryVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.VisitSubQueryFromClause (this);
    }

    public override Type GetQuerySourceType ()
    {
      return null;
    }

    public override IColumnSource GetColumnSource (IDatabaseInfo databaseInfo)
    {
      return _fromSource;
    }

    /// <summary>
    /// The <see cref="QueryModel"/> of the entire linq query.
    /// </summary>
    public QueryModel QueryModel { get; private set; }

    public void SetQueryModel (QueryModel model)
    {
      ArgumentUtility.CheckNotNull ("model", model);
      if (QueryModel != null)
        throw new InvalidOperationException ("QueryModel is already set");

      SubQueryModel.SetParentQuery (model);
      QueryModel = model;
    }

    public SubQueryFromClause Clone (CloneContext cloneContext)
    {
      ArgumentUtility.CheckNotNull ("cloneContext", cloneContext);

      var newPreviousClause = cloneContext.ClonedClauseMapping.GetClause<IClause> (PreviousClause);
      var clonedSubQueryModel = SubQueryModel.Clone (cloneContext.ClonedClauseMapping);
      var result = new SubQueryFromClause (newPreviousClause, Identifier, clonedSubQueryModel);
      cloneContext.ClonedClauseMapping.AddMapping (this, result);
      result.AddClonedJoinClauses (JoinClauses, cloneContext);
      return result;
    }

    IBodyClause IBodyClause.Clone (CloneContext cloneContext)
    {
      return Clone (cloneContext);
    }
  }
}
