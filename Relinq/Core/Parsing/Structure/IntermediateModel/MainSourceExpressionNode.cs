// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// re-linq is free software; you can redistribute it and/or modify it under 
// the terms of the GNU Lesser General Public License as published by the 
// Free Software Foundation; either version 2.1 of the License, 
// or (at your option) any later version.
// 
// re-linq is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-linq; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Linq.Expressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Utilities;

namespace Remotion.Linq.Parsing.Structure.IntermediateModel
{
  /// <summary>
  /// Represents the first expression in a LINQ query, which acts as the main query source.
  /// It is generated by <see cref="ExpressionTreeParser"/> when an <see cref="ParsedExpression"/> tree is parsed.
  /// This node usually marks the end (i.e. the first node) of an <see cref="IExpressionNode"/> chain that represents a query.
  /// </summary>
  public class MainSourceExpressionNode : IQuerySourceExpressionNode
  {
    public MainSourceExpressionNode (string associatedIdentifier, Expression expression)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("associatedIdentifier", associatedIdentifier);
      ArgumentUtility.CheckNotNull ("expression", expression);

      QuerySourceType = expression.Type;
      QuerySourceElementType = ReflectionUtility.GetItemTypeOfIEnumerable (expression.Type, "expression");

      AssociatedIdentifier = associatedIdentifier;
      ParsedExpression = expression;
    }

    public Type QuerySourceElementType { get; private set; }
    public Type QuerySourceType { get; set; }
    public Expression ParsedExpression { get; private set; }
    public string AssociatedIdentifier { get; set; }

    public IExpressionNode Source
    {
      get { return null; }
    }

    public Expression Resolve (ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
    {
      ArgumentUtility.CheckNotNull ("inputParameter", inputParameter);
      ArgumentUtility.CheckNotNull ("expressionToBeResolved", expressionToBeResolved);

      // query sources resolve into references that point back to the respective clauses
      return QuerySourceExpressionNodeUtility.ReplaceParameterWithReference (
          this, 
          inputParameter, 
          expressionToBeResolved, 
          clauseGenerationContext);
    }

    public QueryModel Apply (QueryModel queryModel, ClauseGenerationContext clauseGenerationContext)
    {
      if (queryModel != null)
        throw new ArgumentException ("QueryModel has to be null because MainSourceExpressionNode marks the start of a query.", "queryModel");

      var mainFromClause = CreateMainFromClause (clauseGenerationContext);
      var defaultSelectClause = new SelectClause (new QuerySourceReferenceExpression (mainFromClause));
      return new QueryModel (mainFromClause, defaultSelectClause) { ResultTypeOverride = QuerySourceType };
    }

    private MainFromClause CreateMainFromClause (ClauseGenerationContext clauseGenerationContext)
    {
      var fromClause = new MainFromClause (
          AssociatedIdentifier,
          QuerySourceElementType,
          ParsedExpression);

      clauseGenerationContext.AddContextInfo (this, fromClause);
      return fromClause;
    }
  }
}
