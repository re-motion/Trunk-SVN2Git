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
using System.Linq.Expressions;
using Remotion.Data.Linq.Clauses;
using Remotion.Utilities;

namespace Remotion.Data.Linq.EagerFetching
{
  public class FetchOneRequest : FetchRequestBase
  {
    public FetchOneRequest (LambdaExpression relatedObjectSelector)
        : base(relatedObjectSelector)
    {
    }

    /// <summary>
    /// Modifies the given query model for fetching, changing the <see cref="SelectClause.Selector"/> to the fetch source expression.
    /// For example, a fetch request such as <c>FetchOne (x => x.Customer)</c> will be transformed into a <see cref="SelectClause"/> selecting
    /// <c>y.Customer</c> (where <c>y</c> is what the query model originally selected).
    /// This method is called by <see cref="ModifyFetchQueryModel"/> in the process of creating the new fetch query model.
    /// </summary>
    protected override void ModifyFetchQueryModel (QueryModel fetchQueryModel)
    {
      ArgumentUtility.CheckNotNull ("fetchQueryModel", fetchQueryModel);

      var newSelectProjection = CreateFetchSourceExpression ((SelectClause) fetchQueryModel.SelectOrGroupClause);
      
      var selectClause = ((SelectClause) fetchQueryModel.SelectOrGroupClause);
      selectClause.Selector = newSelectProjection;
    }
  }
}