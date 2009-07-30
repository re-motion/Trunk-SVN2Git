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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.EagerFetching;
using Remotion.Data.UnitTests.Linq.TestDomain;

namespace Remotion.Data.UnitTests.Linq.EagerFetching
{
  [TestFixture]
  public class FetchFilteringQueryModelVisitorTest
  {
    private FetchFilteringQueryModelVisitor _visitor;
    private QueryModel _queryModel;
    
    private FetchOneRequest _fetchOneRequest;
    private FetchManyRequest _fetchManyRequest;
    private FetchManyRequest _innerFetchManyRequest;

    private DistinctResultOperator _distinctResultOperator;
    private CountResultOperator _countResultOperator;
    

    [SetUp]
    public void SetUp ()
    {
      _visitor = new FetchFilteringQueryModelVisitor ();
      _queryModel = ExpressionHelper.CreateQueryModel ();

      _distinctResultOperator = new DistinctResultOperator ();
      _countResultOperator = new CountResultOperator ();

      _fetchOneRequest = new FetchOneRequest (typeof (Student).GetProperty ("OtherStudent"));
      _fetchManyRequest = new FetchManyRequest (typeof (Student).GetProperty ("Friends"));

      _innerFetchManyRequest = new FetchManyRequest (typeof (Student).GetProperty ("Scores"));
      _fetchOneRequest.GetOrAddInnerFetchRequest (_innerFetchManyRequest);

      _queryModel.ResultOperators.Add (_distinctResultOperator);
      _queryModel.ResultOperators.Add (_fetchOneRequest);
      _queryModel.ResultOperators.Add (_fetchManyRequest);
      _queryModel.ResultOperators.Add (_countResultOperator);
    }

    [Test]
    public void VisitResultOperator_IgnoresOrdinaryOperator ()
    {
      _visitor.VisitResultOperator (_distinctResultOperator, _queryModel, 0);

      Assert.That (_queryModel.ResultOperators, 
          Is.EqualTo (new ResultOperatorBase[] { _distinctResultOperator, _fetchOneRequest, _fetchManyRequest, _countResultOperator }));
    }

    [Test]
    public void VisitResultOperator_CapturesFetchRequest ()
    {
      _visitor.VisitResultOperator (_fetchOneRequest, _queryModel, 1);

      Assert.That (_queryModel.ResultOperators,
          Is.EqualTo (new ResultOperatorBase[] { _distinctResultOperator, _fetchManyRequest, _countResultOperator }));
      Assert.That (_visitor.FetchRequests,
          Is.EqualTo (new FetchRequestBase[] { _fetchOneRequest }));
    }

    [Test]
    public void IntegrationTest ()
    {
      _visitor.VisitQueryModel (_queryModel);

      Assert.That (_queryModel.ResultOperators, Is.EqualTo (new ResultOperatorBase[] { _distinctResultOperator, _countResultOperator }));
      Assert.That (_visitor.FetchRequests, Is.EqualTo (new FetchRequestBase[] { _fetchOneRequest, _fetchManyRequest }));
    }
  }
}