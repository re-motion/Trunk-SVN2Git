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
using System.Linq.Expressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using Remotion.Linq.Utilities;
using System.Linq;

namespace Remotion.Linq.EagerFetching.Parsing
{
  public class FetchOneExpressionNode : FetchExpressionNodeBase
  {
    public FetchOneExpressionNode (MethodCallExpressionParseInfo parseInfo, LambdaExpression relatedObjectSelector)
        : base (parseInfo, ArgumentUtility.CheckNotNull ("relatedObjectSelector", relatedObjectSelector))
    {
    }

    // TODO 4564
    //protected override QueryModel ApplyNodeSpecificSemantics (QueryModel queryModel, ClauseGenerationContext clauseGenerationContext)
    //{
    //  var existingMatchingFetchRequest =
    //      queryModel.ResultOperators.OfType<FetchOneRequest>().FirstOrDefault (r => r.RelationMember.Equals (RelationMember));
    //  if (existingMatchingFetchRequest != null)
    //  {
    //    clauseGenerationContext.AddContextInfo (this, existingMatchingFetchRequest);
    //    return queryModel;
    //  }
    //  else
    //    return base.ApplyNodeSpecificSemantics (queryModel, clauseGenerationContext);
    //}
    
    protected override ResultOperatorBase CreateResultOperator (QueryModel queryModel, ClauseGenerationContext clauseGenerationContext)
    {
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      var resultOperator = new FetchOneRequest (RelationMember);
      // Store a mapping between this node and the resultOperator so that a later ThenFetch... node may add its request to the resultOperator.
      clauseGenerationContext.AddContextInfo (this, resultOperator);
      return resultOperator;
    }
  }
}
