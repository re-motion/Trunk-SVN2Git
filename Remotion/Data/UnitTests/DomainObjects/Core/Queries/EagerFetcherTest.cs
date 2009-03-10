// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// This framework is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this framework; if not, see http://www.gnu.org/licenses.
// 
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Queries
{
  [TestFixture]
  [Ignore ("TODO 1041")]
  public class EagerFetcherTest : ClientTransactionBaseTest
  {
    private IQueryManager _queryManagerMock;
    private IQuery _fetchTestQuery;
    private IQuery _fetchTestQuery2;
    private IRelationEndPointDefinition _orderOrderItemsRelationEndPointDefinition;
    private IRelationEndPointDefinition _objectEndPointDefinition;
    private Order _order1;
    private Order _order2;
    private Order _order3;
    private OrderItem _orderItem1; // Order1
    private OrderItem _orderItem2; // Order1
    private OrderItem _orderItem3; // Order2
    private OrderItem _orderItem4; // Order3

    public override void SetUp ()
    {
      base.SetUp ();
      _queryManagerMock = MockRepository.GenerateMock<IQueryManager> ();

      var storageProviderID = DomainObjectIDs.Official1.StorageProviderID;
      _fetchTestQuery = QueryFactory.CreateCollectionQuery ("fetch query", storageProviderID, "FETCH QUERY", new QueryParameterCollection (), typeof (DomainObjectCollection));
      _fetchTestQuery2 = QueryFactory.CreateCollectionQuery ("fetch query", storageProviderID, "FETCH QUERY", new QueryParameterCollection (), typeof (DomainObjectCollection));
      _orderOrderItemsRelationEndPointDefinition = DomainObjectIDs.Order1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");
      _objectEndPointDefinition = DomainObjectIDs.Order1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");

      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _order2 = Order.GetObject (DomainObjectIDs.Order2);
      _order3 = Order.GetObject (DomainObjectIDs.Order3);

      _orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      _orderItem2 = OrderItem.GetObject (DomainObjectIDs.OrderItem2);
      _orderItem3 = OrderItem.GetObject (DomainObjectIDs.OrderItem3);
      _orderItem4 = OrderItem.GetObject (DomainObjectIDs.OrderItem4);
    }

    [Test]
    public void PerformEagerFetching_ExecutesFetchQuery ()
    {
      _queryManagerMock.Expect (mock => mock.GetCollection (_fetchTestQuery)).Return (new QueryResult<DomainObject> (_fetchTestQuery, new DomainObject[0]));

      var fetcher = new EagerFetcher (_queryManagerMock, new DomainObject[] { _order1 });
      fetcher.PerformEagerFetching (_orderOrderItemsRelationEndPointDefinition, _fetchTestQuery);

      _queryManagerMock.VerifyAllExpectations ();
    }

    [Test]
    public void PerformEagerFetching_IgnoresFetchQuery_IfOriginalArrayIsEmpty ()
    {
      var fetcher = new EagerFetcher (_queryManagerMock, new DomainObject[0]);
      fetcher.PerformEagerFetching (_orderOrderItemsRelationEndPointDefinition, _fetchTestQuery);

      _queryManagerMock.AssertWasNotCalled(mock => mock.GetCollection (Arg<IQuery>.Is.Anything));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Eager fetching is only supported for collection-valued relation properties.\r\n"
        + "Parameter name: relationEndPointDefinition")]
    public void PerformEagerFetching_ObjectEndPoint ()
    {
      var fetcher = new EagerFetcher (_queryManagerMock, new DomainObject[0]);
      fetcher.PerformEagerFetching (_objectEndPointDefinition, _fetchTestQuery);
    }

    [Test]
    public void RelationLoad_Normally_RegistersEndPoint ()
    {
      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      Assert.That (_order1.OrderItems, Is.EqualTo (new[] { _orderItem1, _orderItem2 }));

      listenerMock.AssertWasCalled (mock => mock.RelationEndPointMapRegistering (Arg<RelationEndPoint>.Matches (ep => ep.ObjectID == _order1.ID)));
    }

    [Test]
    public void RelationLoad_AfterFetching_DoesntRegisterEndPoint ()
    {
      _queryManagerMock
          .Expect (mock => mock.GetCollection (_fetchTestQuery))
          .Return (new QueryResult<DomainObject> (_fetchTestQuery, new DomainObject[] { _orderItem1 }));

      var fetcher = new EagerFetcher (_queryManagerMock, new DomainObject[] { _order1 });
      fetcher.PerformEagerFetching (_orderOrderItemsRelationEndPointDefinition, _fetchTestQuery);

      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      Assert.That (_order1.OrderItems, Is.EqualTo (new[] { _orderItem1 })); // note that Order1 actually has two items, but we only return one of them

      listenerMock.AssertWasNotCalled (mock => mock.RelationEndPointMapRegistering (Arg<RelationEndPoint>.Matches (ep => ep.ObjectID == _order1.ID)));
    }

    [Test]
    public void PerformEagerFetching_RegistersQueryResult ()
    {
      var id = new RelationEndPointID (_order1.ID, _orderOrderItemsRelationEndPointDefinition);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[id], Is.Null);

      _queryManagerMock
        .Expect (mock => mock.GetCollection (_fetchTestQuery))
        .Return (new QueryResult<DomainObject> (_fetchTestQuery, new DomainObject[] { _orderItem1, _orderItem2 }));

      var fetcher = new EagerFetcher (_queryManagerMock, new DomainObject[] { _order1 });
      fetcher.PerformEagerFetching (_orderOrderItemsRelationEndPointDefinition, _fetchTestQuery);

      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[id], Is.Not.Null);
      Assert.That (((CollectionEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[id]).OppositeDomainObjects, 
          Is.EqualTo (new[] { _orderItem1, _orderItem2 }));
    }
    
    [Test]
    public void PerformEagerFetching_CollatesQueryResult ()
    {
      _queryManagerMock
          .Expect (mock => mock.GetCollection (_fetchTestQuery))
          .Return (new QueryResult<DomainObject> (_fetchTestQuery, new DomainObject[] { _orderItem4, _orderItem1, _orderItem3, _orderItem2 }));

      var fetcher = new EagerFetcher (_queryManagerMock, new DomainObject[] { _order1, _order2, _order3 });
      fetcher.PerformEagerFetching (_orderOrderItemsRelationEndPointDefinition, _fetchTestQuery);

      Assert.That (_order1.OrderItems, Is.EqualTo (new[] { _orderItem1, _orderItem2 }));
      Assert.That (_order2.OrderItems, Is.EqualTo (new[] { _orderItem3 }));
      Assert.That (_order3.OrderItems, Is.EqualTo (new[] { _orderItem4 }));
    }

    [Test]
    public void PerformEagerFetching_IgnoresResultsWithoutOriginalObject ()
    {
      var id1 = new RelationEndPointID (_order1.ID, _orderOrderItemsRelationEndPointDefinition);
      var id2 = new RelationEndPointID (_order2.ID, _orderOrderItemsRelationEndPointDefinition);

      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[id1], Is.Null);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[id2], Is.Null);

      _queryManagerMock
          .Expect (mock => mock.GetCollection (_fetchTestQuery))
          .Return (new QueryResult<DomainObject> (_fetchTestQuery, new DomainObject[] { _orderItem2, _orderItem3 }));

      var fetcher = new EagerFetcher (_queryManagerMock, new DomainObject[] { _order2 });
      fetcher.PerformEagerFetching (_orderOrderItemsRelationEndPointDefinition, _fetchTestQuery);

      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[id1], Is.Null);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[id2], Is.Not.Null);
      Assert.That (((CollectionEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[id2]).OppositeDomainObjects,
          Is.EqualTo (new[] {_orderItem3}));
    }

    [Test]
    public void PerformEagerFetching_IgnoresResultsWithNullObject ()
    {
      var id1 = new RelationEndPointID (_order1.ID, _orderOrderItemsRelationEndPointDefinition);

      _queryManagerMock
          .Expect (mock => mock.GetCollection (_fetchTestQuery))
          .Return (new QueryResult<DomainObject> (_fetchTestQuery, new DomainObject[] { null, _orderItem1 }));

      var fetcher = new EagerFetcher (_queryManagerMock, new DomainObject[] { _order1 });
      fetcher.PerformEagerFetching (_orderOrderItemsRelationEndPointDefinition, _fetchTestQuery);

      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[id1], Is.Not.Null);
      Assert.That (((CollectionEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[id1]).OppositeDomainObjects,
          Is.EqualTo (new[] { _orderItem1, _orderItem2 }));
    }

    [Test]
    public void PerformEagerFetching_IgnoresOriginalNullObject ()
    {
      var id1 = new RelationEndPointID (_order1.ID, _orderOrderItemsRelationEndPointDefinition);

      _queryManagerMock
          .Expect (mock => mock.GetCollection (_fetchTestQuery))
          .Return (new QueryResult<DomainObject> (_fetchTestQuery, new DomainObject[] { null, _orderItem1 }));

      var fetcher = new EagerFetcher (_queryManagerMock, new DomainObject[] { null, _order1 });
      fetcher.PerformEagerFetching (_orderOrderItemsRelationEndPointDefinition, _fetchTestQuery);

      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[id1], Is.Not.Null);
      Assert.That (((CollectionEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[id1]).OppositeDomainObjects,
          Is.EqualTo (new[] { _orderItem1, _orderItem2 }));
    }

    [Test]
    public void PerformEagerFetching_WithoutOriginalObject_CausesRelationToBeLoadedOnAccess ()
    {
      _queryManagerMock
          .Expect (mock => mock.GetCollection (_fetchTestQuery))
          .Return (new QueryResult<DomainObject> (_fetchTestQuery, new DomainObject[] { _orderItem2, _orderItem3 }));

      var fetcher = new EagerFetcher (_queryManagerMock, new DomainObject[] { _order2 });
      fetcher.PerformEagerFetching (_orderOrderItemsRelationEndPointDefinition, _fetchTestQuery);

      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      Assert.That (_order1.OrderItems, Is.EqualTo (new[] { _orderItem1, _orderItem2 })); // this was loaded from the database
      Assert.That (_order2.OrderItems, Is.EqualTo (new[] { _orderItem3 })); // this was prefetched

      listenerMock.AssertWasCalled (mock => mock.RelationEndPointMapRegistering (Arg<RelationEndPoint>.Matches (ep => ep.ObjectID == _order1.ID)));
      listenerMock.AssertWasNotCalled (mock => mock.RelationEndPointMapRegistering (Arg<RelationEndPoint>.Matches (ep => ep.ObjectID == _order2.ID)));
    }

    [Test]
    public void PerformEagerFetching_RegistersEmptyCollectionIfNoRelated ()
    {
      _queryManagerMock
          .Expect (mock => mock.GetCollection (_fetchTestQuery))
          .Return (new QueryResult<DomainObject> (_fetchTestQuery, new DomainObject[] { _orderItem1 }));

      var fetcher = new EagerFetcher (_queryManagerMock, new DomainObject[] { _order1, _order2, _order3 });
      fetcher.PerformEagerFetching (_orderOrderItemsRelationEndPointDefinition, _fetchTestQuery);

      Assert.That (_order1.OrderItems, Is.EqualTo (new[] { _orderItem1 }));
      Assert.That (_order2.OrderItems, Is.Empty);
      Assert.That (_order3.OrderItems, Is.Empty);
    }

    [Test]
    public void PerformEagerFetching_IsRecursive ()
    {
      _fetchTestQuery.EagerFetchQueries.Add (_orderOrderItemsRelationEndPointDefinition, _fetchTestQuery2);

      _queryManagerMock
          .Expect (mock => mock.GetCollection (_fetchTestQuery))
          .Return (new QueryResult<DomainObject> (_fetchTestQuery, new DomainObject[] { _orderItem1 }));
      _queryManagerMock
          .Expect (mock => mock.GetCollection (_fetchTestQuery2))
          .Return (new QueryResult<DomainObject> (_fetchTestQuery2, new DomainObject[0]));

      var fetcher = new EagerFetcher (_queryManagerMock, new DomainObject[] { _order1, _order2, _order3 });
      fetcher.PerformEagerFetching (_orderOrderItemsRelationEndPointDefinition, _fetchTestQuery);

      _queryManagerMock.VerifyAllExpectations ();
    }

    [Test]
    //[ExpectedException (typeof (UnexpectedQueryResultException), ExpectedMessage = "
    public void PerformEagerFetching_ThrowsOnInvalidResultType ()
    {
      _queryManagerMock
        .Expect (mock => mock.GetCollection (_fetchTestQuery))
        .Return (new QueryResult<DomainObject> (_fetchTestQuery, new DomainObject[] { _order3 }));

      var fetcher = new EagerFetcher (_queryManagerMock, new DomainObject[] { _order1 });
      fetcher.PerformEagerFetching (_orderOrderItemsRelationEndPointDefinition, _fetchTestQuery);

      Assert.Fail ("Expected exception");
    }

    [Test]
    public void PerformEagerFetching_ThrowsOnNullResult ()
    {
      _queryManagerMock
        .Expect (mock => mock.GetCollection (_fetchTestQuery))
        .Return (new QueryResult<DomainObject> (_fetchTestQuery, new DomainObject[] { null }));

      var fetcher = new EagerFetcher (_queryManagerMock, new DomainObject[] { _order1 });
      fetcher.PerformEagerFetching (_orderOrderItemsRelationEndPointDefinition, _fetchTestQuery);

      Assert.Fail ("Expected exception");
    }
  }
}