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
using System.Reflection;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.EagerFetching;
using Remotion.Linq.Utilities;

namespace Remotion.Linq.UnitTests.Linq.Core.EagerFetching
{
  public class TestFetchRequest : FetchRequestBase
  {
    public TestFetchRequest (MemberInfo relationMember)
        : base (relationMember)
    {
    }

    public new Expression GetFetchedMemberExpression (Expression source)
    {
      return base.GetFetchedMemberExpression (source);
    }

    protected override void ModifyFetchQueryModel (QueryModel fetchQueryModel)
    {
      // do nothing
    }

    public override ResultOperatorBase Clone (CloneContext cloneContext)
    {
      ArgumentUtility.CheckNotNull ("cloneContext", cloneContext);

      var clone = new TestFetchRequest (RelationMember);
      foreach (var innerFetchRequest in clone.InnerFetchRequests)
        clone.GetOrAddInnerFetchRequest ((FetchRequestBase) innerFetchRequest.Clone (cloneContext));

      return clone;
    }

    public override void TransformExpressions (Func<System.Linq.Expressions.Expression, System.Linq.Expressions.Expression> transformation)
    {
      throw new NotImplementedException ();
    }
  }
}
