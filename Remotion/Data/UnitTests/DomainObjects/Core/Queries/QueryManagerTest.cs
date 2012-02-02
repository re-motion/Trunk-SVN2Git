// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
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

    private IPersistenceStrategy _persistenceStrategyMock;
    private IObjectLoader _objectLoaderMock;
    private ClientTransaction _clientTransaction;
    private ClientTransactionEventSinkWithMock _transactionEventSinkWithMock;
    
    private IQuery _collectionQuery;
    private IQuery _scalarQuery;

    private Order _fakeOrder1;
    private Order _fakeOrder2;
    private ILoadedObjectData _loadedObjectDataStub1;
    private ILoadedObjectData _loadedObjectDataStub2;

    public override void SetUp ()
    {
      base.SetUp ();

      _persistenceStrategyMock = MockRepository.GenerateStrictMock<IPersistenceStrategy> ();
      _objectLoaderMock = MockRepository.GenerateStrictMock<IObjectLoader> ();
      _clientTransaction = ClientTransaction.CreateRootTransaction();
      _transactionEventSinkWithMock = new ClientTransactionEventSinkWithMock (_clientTransaction);

      _queryManager = new QueryManager (
          _persistenceStrategyMock,
          _objectLoaderMock,
          _clientTransaction,
          _transactionEventSinkWithMock);

      _collectionQuery =  QueryFactory.CreateQueryFromConfiguration ("OrderQuery");
      _scalarQuery = QueryFactory.CreateQueryFromConfiguration ("OrderNoSumByCustomerNameQuery");

      _fakeOrder1 = DomainObjectMother.CreateFakeObject<Order> ();
      _fakeOrder2 = DomainObjectMother.CreateFakeObject<Order>();

      _loadedObjectDataStub1 = MockRepository.GenerateStub<ILoadedObjectData> ();
      _loadedObjectDataStub2 = MockRepository.GenerateStub<ILoadedObjectData> ();
    }

    [Test]
    public void GetScalar ()
    {
      _persistenceStrategyMock.Expect (mock => mock.ExecuteScalarQuery (_scalarQuery)).Return (27);
      _persistenceStrategyMock.Replay ();

      var result = _queryManager.GetScalar (_scalarQuery);

      _persistenceStrategyMock.VerifyAllExpectations ();
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
      _loadedObjectDataStub1.Stub (stub => stub.GetDomainObjectReference ()).Return (_fakeOrder1);
      _loadedObjectDataStub2.Stub (stub => stub.GetDomainObjectReference ()).Return (_fakeOrder2);

      _transactionEventSinkWithMock
          .Stub (stub => stub.FilterQueryResult (Arg<ClientTransaction>.Is.Anything, Arg<QueryResult<Order>>.Is.Anything))
          .Return (null)
          .WhenCalled (mi => mi.ReturnValue = mi.Arguments[1]);
      _transactionEventSinkWithMock.Replay ();

      _objectLoaderMock
          .Expect (mock => mock.GetOrLoadCollectionQueryResult (_collectionQuery))
          .Return (new[] { _loadedObjectDataStub1, _loadedObjectDataStub2 });
      _objectLoaderMock.Replay ();

      var result = _queryManager.GetCollection<Order> (_collectionQuery);

      _objectLoaderMock.VerifyAllExpectations ();
      Assert.That (result.AsEnumerable(), Is.EqualTo (new[] { _fakeOrder1, _fakeOrder2 }));
    }

    [Test]
    public void GetCollection_WithNull ()
    {
      _loadedObjectDataStub1.Stub (stub => stub.GetDomainObjectReference ()).Return (_fakeOrder1);
      _loadedObjectDataStub2.Stub (stub => stub.GetDomainObjectReference ()).Return (null);

      _transactionEventSinkWithMock
          .Stub (stub => stub.FilterQueryResult (Arg<ClientTransaction>.Is.Anything, Arg<QueryResult<Order>>.Is.Anything))
          .Return (null)
          .WhenCalled (mi => mi.ReturnValue = mi.Arguments[1]);
      _transactionEventSinkWithMock.Replay();

      _objectLoaderMock
          .Expect (mock => mock.GetOrLoadCollectionQueryResult (_collectionQuery))
          .Return (new[] { _loadedObjectDataStub1, _loadedObjectDataStub2 });
      _objectLoaderMock.Replay ();

      var result = _queryManager.GetCollection<Order> (_collectionQuery);

      _objectLoaderMock.VerifyAllExpectations ();
      Assert.That (result.AsEnumerable (), Is.EqualTo (new[] { _fakeOrder1, null }));
    }

    [Test]
    public void GetCollection_WithCastProblem ()
    {
      _loadedObjectDataStub1.Stub (stub => stub.GetDomainObjectReference ()).Return (_fakeOrder1);

      _objectLoaderMock
          .Expect (mock => mock.GetOrLoadCollectionQueryResult (_collectionQuery))
          .Return (new[] { _loadedObjectDataStub1 });
      _objectLoaderMock.Replay ();

      Assert.That (
          () => _queryManager.GetCollection<Customer> (_collectionQuery), 
          Throws.TypeOf<UnexpectedQueryResultException> ().With.Message.EqualTo (
            "The query returned an object of type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order', but a query result of type "
            + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer' was expected."));
    }

    [Test]
    public void GetCollection_CallsFilterQueryResult ()
    {
      _loadedObjectDataStub1.Stub (stub => stub.GetDomainObjectReference ()).Return (_fakeOrder1);
      _objectLoaderMock
          .Stub (
              mock =>
              mock.GetOrLoadCollectionQueryResult (_collectionQuery))
          .Return (new[] { _loadedObjectDataStub1 });
      _objectLoaderMock.Replay ();

      var filteredResult = new QueryResult<Order> (_collectionQuery, new[] { _fakeOrder2 });
      _transactionEventSinkWithMock
          .Expect (mock => mock.FilterQueryResult (
              Arg.Is (_clientTransaction), 
              Arg<QueryResult<Order>>.Matches (qr => qr.ToArray().SequenceEqual (new[] { _fakeOrder1 }))))
          .Return (filteredResult);
      _transactionEventSinkWithMock.Replay ();

      var result = _queryManager.GetCollection<Order> (_collectionQuery);

      _transactionEventSinkWithMock.VerifyAllExpectations ();
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
      _loadedObjectDataStub1.Stub (stub => stub.GetDomainObjectReference ()).Return (_fakeOrder1);
      _loadedObjectDataStub2.Stub (stub => stub.GetDomainObjectReference ()).Return (_fakeOrder2);

      _objectLoaderMock
          .Expect (mock => mock.GetOrLoadCollectionQueryResult (_collectionQuery))
          .Return (new[] { _loadedObjectDataStub1, _loadedObjectDataStub2 });
      _objectLoaderMock.Replay ();

      var filteredResult = new QueryResult<DomainObject> (_collectionQuery, new[] { _fakeOrder2 });
      _transactionEventSinkWithMock
          .Expect (mock => mock.FilterQueryResult (
              Arg.Is (_clientTransaction), 
              Arg<QueryResult<DomainObject>>.Matches (qr => qr.ToArray().SequenceEqual (new[] { _fakeOrder1, _fakeOrder2 }))))
          .Return (filteredResult);
      _transactionEventSinkWithMock.Replay ();

      var result = _queryManager.GetCollection (_collectionQuery);

      _objectLoaderMock.VerifyAllExpectations ();
      _transactionEventSinkWithMock.VerifyAllExpectations ();

      Assert.That (result, Is.SameAs (filteredResult));
    }
   
  }
}
