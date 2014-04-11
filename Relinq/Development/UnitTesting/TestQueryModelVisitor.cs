// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using System.Collections.ObjectModel;
using Remotion.Linq.Clauses;

namespace Remotion.Linq.Development.UnitTesting
{
  public class TestQueryModelVisitor : QueryModelVisitorBase
  {
    //public void VisitCollection<T> (ObservableCollection<T> collection, Action<T> acceptAction, Action<int> indexSetter)
    //{
    //  var method = typeof (QueryModelVisitorBase)
    //      .GetMethod ("VisitCollection", BindingFlags.NonPublic | BindingFlags.Instance)
    //      .MakeGenericMethod (typeof (T));
    //  try
    //  {
    //    method.Invoke (this, new object[] { collection, acceptAction, indexSetter });
    //  }
    //  catch (TargetInvocationException ex)
    //  {
    //    throw ex.InnerException.PreserveStackTrace();
    //  }
    //}

    public new void VisitBodyClauses (ObservableCollection<IBodyClause> bodyClauses, QueryModel queryModel)
    {
      base.VisitBodyClauses (bodyClauses, queryModel);
    }

    public new void VisitOrderings (ObservableCollection<Ordering> orderings, QueryModel queryModel, OrderByClause orderByClause)
    {
      base.VisitOrderings (orderings, queryModel, orderByClause);
    }

    public new void VisitResultOperators (ObservableCollection<ResultOperatorBase> resultOperators, QueryModel queryModel)
    {
      base.VisitResultOperators (resultOperators, queryModel);
    }
  }
}
