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
using System.Reflection;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Parsing.Structure.IntermediateModel;
using Remotion.Utilities;

namespace Remotion.Data.Linq.EagerFetching
{
  public class FetchManyExpressionNode : FetchExpressionNodeBase
  {
    public static readonly MethodInfo[] SupportedMethods = new[] { typeof (ExtensionMethods).GetMethod ("FetchMany") };

    public FetchManyExpressionNode (MethodCallExpressionParseInfo parseInfo, LambdaExpression relatedObjectSelector)
        : base (parseInfo, ArgumentUtility.CheckNotNull ("relatedObjectSelector", relatedObjectSelector))
    {
    }

    protected override ResultOperatorBase CreateResultOperator (ClauseGenerationContext clauseGenerationContext)
    {
      var resultOperator = new FetchManyRequest (RelationMember);
      clauseGenerationContext.AddContextInfo (this, resultOperator);
      return resultOperator;
    }
  }
}