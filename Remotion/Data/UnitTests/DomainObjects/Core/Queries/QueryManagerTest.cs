// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Queries
{
  [TestFixture]
  public class QueryManagerTest : ClientTransactionBaseTest
  {
    private QueryManager _queryManager;

    private IDataSource _dataSourceMock;
    private IObjectLoader _objectLoaderMock;
    private IClientTransactionListener _transactionEventSinkMock;
    
    private IQuery _collectionQuery;
    private IQuery _scalarQuery;

    private Order _fakeOrder1;
    private Order _fakeOrder2;

    public override void SetUp ()
    {
      base.SetUp ();

      _dataSourceMock = MockRepository.GenerateMock<IDataSource> ();
      _objectLoaderMock = MockRepository.GenerateMock<IObjectLoader> ();
      _transactionEventSinkMock = MockRepository.GenerateMock<IClientTransactionListener> ();

      _queryManager = new QueryManager (_dataSourceMock, _objectLoaderMock, _transactionEventSinkMock);

      _collectionQuery =  QueryFactory.CreateQueryFromConfiguration ("OrderQuery");
      _scalarQuery = QueryFactory.CreateQueryFromConfiguration ("OrderNoSumByCustomerNameQuery");

      _fakeOrder1 = DomainObjectMother.CreateFakeObject<Order> ();
      _fakeOrder2 = DomainObjectMother.CreateFakeObject<Order>();
    }

    [Test]
    public void GetScalar ()
    {
      _dataSourceMock.Expect (mock => mock.LoadScalarForQuery (_scalarQuery)).Return (27);
      _dataSourceMock.Replay ();

      var result = _queryManager.GetScalar (_scalarQuery);

      _dataSourceMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (27));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void GetScalar_WithCollectionQuery ()
    {
      _queryManager.GetScalar (_collectionQuery);
    }

    [Test]
    public void GetCollection ()
    {
      _objectLoaderMock.Expect (mock => mock.LoadCollectionQueryResult<Order> (_collectionQuery)).Return (new[] { _fakeOrder1, _fakeOrder2 });
      _objectLoaderMock.Replay ();

      _queryManager.GetCollection<Order> (_collectionQuery);

      _objectLoaderMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetCollection_CallsFilterQueryResult ()
    {
      var originalResult = new[] { _fakeOrder1 };
      var filteredResult = new QueryResult<Order> (_collectionQuery, new[] { _fakeOrder2 });
      
      _objectLoaderMock.Stub (mock => mock.LoadCollectionQueryResult<Order> (_collectionQuery)).Return (originalResult);
      _objectLoaderMock.Replay ();

      _transactionEventSinkMock
          .Expect (mock => mock.FilterQueryResult (Arg<QueryResult<Order>>.Matches (qr => qr.ToArray().SequenceEqual (originalResult))))
          .Return (filteredResult);
      _transactionEventSinkMock.Replay ();

      var result = _queryManager.GetCollection<Order> (_collectionQuery);

      _transactionEventSinkMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (filteredResult));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void GetCollection_WithScalarQuery ()
    {
      _queryManager.GetCollection (_scalarQuery);
    }

    [Test]
    public void GetCollection_NonGeneric ()
    {
      var filteredResult = new QueryResult<DomainObject> (_collectionQuery, new[] { _fakeOrder2 });
      
      _objectLoaderMock.Expect (mock => mock.LoadCollectionQueryResult<DomainObject> (_collectionQuery)).Return (new[] { _fakeOrder1, _fakeOrder2 });
      _objectLoaderMock.Replay ();

      _transactionEventSinkMock
          .Expect (mock => mock.FilterQueryResult (Arg<QueryResult<DomainObject>>.Is.Anything))
          .Return (filteredResult);
      _transactionEventSinkMock.Replay ();

      var result = _queryManager.GetCollection (_collectionQuery);

      _objectLoaderMock.VerifyAllExpectations ();
      _transactionEventSinkMock.VerifyAllExpectations ();

      Assert.That (result, Is.SameAs (filteredResult));
    }
   
  }
}
