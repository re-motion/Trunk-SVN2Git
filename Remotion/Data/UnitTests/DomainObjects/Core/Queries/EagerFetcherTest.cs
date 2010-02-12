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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Queries
{
  [TestFixture]
  public class EagerFetcherTest : StandardMappingTest
  {
    private RelationEndPointMap _relationEndPointMap;
    private IRelationEndPointDefinition _orderOrderItemsRelationEndPointDefinition;
    private IRelationEndPointDefinition _objectEndPointDefinitionReal;
    private IRelationEndPointDefinition _objectEndPointDefinitionVirtual;
    private IRelationEndPointDefinition _objectEndPointDefinitionOneMany;
    private Order _order1;
    private Order _order2;
    private Order _order3;
    private OrderItem _orderItem1; // Order1
    private OrderItem _orderItem2; // Order1
    private OrderItem _orderItem3; // Order2
    private OrderItem _orderItem4; // Order3
    private OrderItem _orderItemWithoutOrder; // no Order
    private OrderTicket _orderTicket1;
    private EagerFetcher _fetcher;

    public override void SetUp ()
    {
      base.SetUp ();

      var transaction = ClientTransaction.CreateRootTransaction ();
      var dataManager = ClientTransactionTestHelper.GetDataManager (transaction);
      _relationEndPointMap = dataManager.RelationEndPointMap;

      _fetcher = new EagerFetcher (dataManager);

      _orderOrderItemsRelationEndPointDefinition = DomainObjectIDs.Order1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");
      _objectEndPointDefinitionVirtual = DomainObjectIDs.Order1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");
      _objectEndPointDefinitionReal = DomainObjectIDs.OrderTicket1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (OrderTicket).FullName + ".Order");
      _objectEndPointDefinitionOneMany = DomainObjectIDs.OrderItem1.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (OrderItem).FullName + ".Order");

      _order1 = _relationEndPointMap.ClientTransaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));
      _order2 = _relationEndPointMap.ClientTransaction.Execute (() => Order.GetObject (DomainObjectIDs.Order2));
      _order3 = _relationEndPointMap.ClientTransaction.Execute (() => Order.GetObject (DomainObjectIDs.Order3));

      _orderItem1 = _relationEndPointMap.ClientTransaction.Execute (() => OrderItem.GetObject (DomainObjectIDs.OrderItem1));
      _orderItem2 = _relationEndPointMap.ClientTransaction.Execute (() => OrderItem.GetObject (DomainObjectIDs.OrderItem2));
      _orderItem3 = _relationEndPointMap.ClientTransaction.Execute (() => OrderItem.GetObject (DomainObjectIDs.OrderItem3));
      _orderItem4 = _relationEndPointMap.ClientTransaction.Execute (() => OrderItem.GetObject (DomainObjectIDs.OrderItem4));
      _orderItemWithoutOrder = _relationEndPointMap.ClientTransaction.Execute (() => OrderItem.NewObject ());

      _orderTicket1 = _relationEndPointMap.ClientTransaction.Execute (() => OrderTicket.GetObject (DomainObjectIDs.OrderTicket1));
    }

    [Test]
    public void RelationLoad_Normally_RegistersEndPoint ()
    {
      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_relationEndPointMap.ClientTransaction);

      Assert.That (GetOrderItemsInFetchTransaction (_order1), Is.EquivalentTo (new[] { _orderItem1, _orderItem2 }));

      listenerMock.AssertWasCalled (mock => mock.RelationEndPointMapRegistering (Arg<RelationEndPoint>.Matches (ep => ep.ObjectID == _order1.ID)));
    }

    [Test]
    public void RelationLoad_AfterFetching_DoesntRegisterEndPoint ()
    {
      _fetcher.CorrelateAndRegisterFetchResults (new[] { _order1 }, new[] { _orderItem1 }, _orderOrderItemsRelationEndPointDefinition);

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_relationEndPointMap.ClientTransaction);

      Assert.That (GetOrderItemsInFetchTransaction (_order1), Is.EqualTo (new[] { _orderItem1 })); // Order1 actually has two items, but we only return one of them

      listenerMock.AssertWasNotCalled (mock => mock.RelationEndPointMapRegistering (Arg<RelationEndPoint>.Matches (ep => ep.ObjectID == _order1.ID)));
    }

    [Test]
    public void CorrelateAndRegisterFetchResults_RegistersQueryResult ()
    {
      var id = new RelationEndPointID (_order1.ID, _orderOrderItemsRelationEndPointDefinition);
      Assert.That (_relationEndPointMap[id], Is.Null);

      _fetcher.CorrelateAndRegisterFetchResults (new[] { _order1 }, new[] { _orderItem1, _orderItem2 }, _orderOrderItemsRelationEndPointDefinition);

      Assert.That (_relationEndPointMap[id], Is.Not.Null);
      Assert.That (
          ((CollectionEndPoint) _relationEndPointMap[id]).OppositeDomainObjects, 
          Is.EqualTo (new[] { _orderItem1, _orderItem2 }));
    }

    [Test]
    public void CorrelateAndRegisterFetchResults_RegistersQueryResult_WithCorrectCollectionType ()
    {
      var id = new RelationEndPointID (_order1.ID, _orderOrderItemsRelationEndPointDefinition);
      Assert.That (_relationEndPointMap[id], Is.Null);

      _fetcher.CorrelateAndRegisterFetchResults (new[] { _order1 }, new[] { _orderItem1, _orderItem2 }, _orderOrderItemsRelationEndPointDefinition);

      Assert.That (_relationEndPointMap[id], Is.Not.Null);
      Assert.That (((CollectionEndPoint) _relationEndPointMap[id]).OppositeDomainObjects.GetType(), Is.EqualTo (typeof (ObjectList<OrderItem>)));
      Assert.That (((CollectionEndPoint) _relationEndPointMap[id]).OppositeDomainObjects.RequiredItemType, Is.EqualTo (typeof (OrderItem)));
    }
    
    [Test]
    public void CorrelateAndRegisterFetchResults_CollatesQueryResult ()
    {
      _fetcher.CorrelateAndRegisterFetchResults (
          new[] { _order1, _order2, _order3 },
          new[] { _orderItem4, _orderItem1, _orderItem3, _orderItem2 },
          _orderOrderItemsRelationEndPointDefinition);

      Assert.That (GetOrderItemsInFetchTransaction (_order1), Is.EqualTo (new[] { _orderItem1, _orderItem2 }));
      Assert.That (GetOrderItemsInFetchTransaction (_order2), Is.EqualTo (new[] { _orderItem3 }));
      Assert.That (GetOrderItemsInFetchTransaction (_order3), Is.EqualTo (new[] { _orderItem4 }));
    }

    [Test]
    public void CorrelateAndRegisterFetchResults_IgnoresResultsWithoutOriginalObject ()
    {
      var id1 = new RelationEndPointID (_order1.ID, _orderOrderItemsRelationEndPointDefinition);

      Assert.That (_relationEndPointMap[id1], Is.Null);

      _fetcher.CorrelateAndRegisterFetchResults (new[] { _order2 }, new[] { _orderItem2, _orderItem3 }, _orderOrderItemsRelationEndPointDefinition);
      
      Assert.That (_relationEndPointMap[id1], Is.Null);
    }

    [Test]
    public void CorrelateAndRegisterFetchResults_IgnoresResultsWithNullObject ()
    {
      var id1 = new RelationEndPointID (_order1.ID, _orderOrderItemsRelationEndPointDefinition);

      _fetcher.CorrelateAndRegisterFetchResults (new[] { _order1 }, new[] { null, _orderItem1 }, _orderOrderItemsRelationEndPointDefinition);

      Assert.That (_relationEndPointMap[id1], Is.Not.Null);
      Assert.That (((CollectionEndPoint) _relationEndPointMap[id1]).OppositeDomainObjects, Is.EqualTo (new[] { _orderItem1 }));
    }

    [Test]
    public void CorrelateAndRegisterFetchResults_IgnoresResultsWithDuplicates ()
    {
      var id1 = new RelationEndPointID (_order1.ID, _orderOrderItemsRelationEndPointDefinition);
      
      _fetcher.CorrelateAndRegisterFetchResults (new[] { _order1 }, new[] { _orderItem1, _orderItem1 }, _orderOrderItemsRelationEndPointDefinition);

      Assert.That (_relationEndPointMap[id1], Is.Not.Null);
      Assert.That (((CollectionEndPoint) _relationEndPointMap[id1]).OppositeDomainObjects, Is.EqualTo (new[] { _orderItem1 }));
    }

    [Test]
    public void CorrelateAndRegisterFetchResults_IgnoresOriginalNullObject ()
    {
      var id1 = new RelationEndPointID (_order1.ID, _orderOrderItemsRelationEndPointDefinition);

      _fetcher.CorrelateAndRegisterFetchResults (new[] { null, _order1 }, new[] { null, _orderItem1 }, _orderOrderItemsRelationEndPointDefinition);

      Assert.That (_relationEndPointMap[id1], Is.Not.Null);
      Assert.That (((CollectionEndPoint) _relationEndPointMap[id1]).OppositeDomainObjects, Is.EqualTo (new[] { _orderItem1 }));
    }

    [Test]
    public void CorrelateAndRegisterFetchResults_WithoutOriginalObject_CausesRelationToBeLoadedOnAccess ()
    {
      _fetcher.CorrelateAndRegisterFetchResults (new[] { _order2 }, new[] { _orderItem2, _orderItem3 }, _orderOrderItemsRelationEndPointDefinition);

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_relationEndPointMap.ClientTransaction);
      
      Assert.That (GetOrderItemsInFetchTransaction (_order1), Is.EquivalentTo (new[] { _orderItem1, _orderItem2 })); // this was loaded from the database
      Assert.That (GetOrderItemsInFetchTransaction (_order2), Is.EqualTo (new[] { _orderItem3 })); // this was prefetched

      listenerMock.AssertWasCalled (mock => mock.RelationEndPointMapRegistering (Arg<RelationEndPoint>.Matches (ep => ep.ObjectID == _order1.ID)));
      listenerMock.AssertWasNotCalled (mock => mock.RelationEndPointMapRegistering (Arg<RelationEndPoint>.Matches (ep => ep.ObjectID == _order2.ID)));
    }

    [Test]
    public void CorrelateAndRegisterFetchResults_RegistersEmptyCollectionIfNoRelated ()
    {
      _fetcher.CorrelateAndRegisterFetchResults (new[] { _order1, _order2, _order3 }, new[] { _orderItem1 }, _orderOrderItemsRelationEndPointDefinition);

      Assert.That (GetOrderItemsInFetchTransaction (_order1), Is.EqualTo (new[] { _orderItem1 }));
      Assert.That (GetOrderItemsInFetchTransaction (_order2), Is.Empty);
      Assert.That (GetOrderItemsInFetchTransaction (_order3), Is.Empty);
    }

    [Test]
    [ExpectedException (typeof (UnexpectedQueryResultException), ExpectedMessage = "An eager fetch query returned an "
        + "object of an unexpected type. For relation end point 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems', an object of "
        + "class 'OrderItem' was expected, but an object of class 'Order' was returned.")]
    public void CorrelateAndRegisterFetchResults_ThrowsOnInvalidResultType ()
    {
      _fetcher.CorrelateAndRegisterFetchResults (new[] { _order3 }, new[] { _order1 }, _orderOrderItemsRelationEndPointDefinition);
    }

    [Test]
    [ExpectedException (typeof (UnexpectedQueryResultException), ExpectedMessage = "Eager fetching cannot be performed for query result object "
        + "'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid' and relation end point 'Remotion.Data.UnitTests.DomainObjects.TestDomain."
        + "Order.OrderItems'. The end point belongs to an object of class 'Order' but the query result has class 'OrderItem'.")]
    public void CorrelateAndRegisterFetchResults_ThrowsOnInvalidOriginalType ()
    {
      _fetcher.CorrelateAndRegisterFetchResults (new[] { _orderItem1 }, new[] { _orderItem3 }, _orderOrderItemsRelationEndPointDefinition);
    }

    [Test]
    public void CorrelateAndRegisterFetchResults_IgnoresResultObjectsWithoutOriginalObject ()
    {
      var id1 = new RelationEndPointID (_order1.ID, _orderOrderItemsRelationEndPointDefinition);

      var fetcher = _fetcher;
      fetcher.CorrelateAndRegisterFetchResults (new[] { _order1 }, new[] { _orderItemWithoutOrder, _orderItem1 }, _orderOrderItemsRelationEndPointDefinition);

      Assert.That (((CollectionEndPoint) _relationEndPointMap[id1]).OppositeDomainObjects, Is.EqualTo (new[] { _orderItem1 }));
    }

    [Test]
    public void CorrelateAndRegisterFetchResults_IgnoresResultObjectsWithAlreadyRegisteredEndPoints ()
    {
      var id1 = new RelationEndPointID (_order1.ID, _orderOrderItemsRelationEndPointDefinition);
      var id2 = new RelationEndPointID (_order2.ID, _orderOrderItemsRelationEndPointDefinition);

      Assert.That (GetOrderItemsInFetchTransaction (_order1), Is.EquivalentTo (new[] { _orderItem1, _orderItem2 })); // preloaded from db
      Assert.That (_relationEndPointMap[id1], Is.Not.Null);
      Assert.That (_relationEndPointMap[id2], Is.Null);

      var fetcher = _fetcher;
      fetcher.CorrelateAndRegisterFetchResults (new[] { _order1, _order2 }, new[] { _orderItem1, _orderItem3 }, _orderOrderItemsRelationEndPointDefinition);

      Assert.That (
          ((CollectionEndPoint) _relationEndPointMap[id1]).OppositeDomainObjects,
          Is.EquivalentTo (new[] { _orderItem1, _orderItem2 })); // not replaced
      Assert.That (
          ((CollectionEndPoint) _relationEndPointMap[id2]).OppositeDomainObjects,
          Is.EqualTo (new[] { _orderItem3 })); // fetched
    }

    [Test]
    public void CorrelateAndRegisterFetchResults_ObjectEndPoint_Virtual_EndPointIsRegistered ()
    {
      var endPointID = new RelationEndPointID (_order1.ID, _objectEndPointDefinitionVirtual);

      _fetcher.CorrelateAndRegisterFetchResults (new[] { _order1 }, new[] { _orderTicket1 }, _objectEndPointDefinitionVirtual);

      Assert.That (_relationEndPointMap[endPointID], Is.Not.Null);
      Assert.That (((ObjectEndPoint) _relationEndPointMap[endPointID]).OppositeObjectID, Is.EqualTo (_orderTicket1.ID));
    }

    [Test]
    public void CorrelateAndRegisterFetchResults_ObjectEndPoint_Real_EndPointIsRegistered ()
    {
      var endPointID = new RelationEndPointID (_orderTicket1.ID, _objectEndPointDefinitionReal);

      _fetcher.CorrelateAndRegisterFetchResults (new[] { _orderTicket1 }, new[] { _order1 }, _objectEndPointDefinitionReal);

      Assert.That (_relationEndPointMap[endPointID], Is.Not.Null);
      Assert.That (((ObjectEndPoint) _relationEndPointMap[endPointID]).OppositeObjectID, Is.EqualTo (_order1.ID));
    }

    [Test]
    public void CorrelateAndRegisterFetchResults_ObjectEndPoint_OneMany_EndPointIsRegistered ()
    {
      var endPointID = new RelationEndPointID (_orderItem1.ID, _objectEndPointDefinitionOneMany);

      _fetcher.CorrelateAndRegisterFetchResults (new[] { _orderItem1 }, new[] { _order1 }, _objectEndPointDefinitionOneMany);

      Assert.That (_relationEndPointMap[endPointID], Is.Not.Null);
      Assert.That (((ObjectEndPoint) _relationEndPointMap[endPointID]).OppositeObjectID, Is.EqualTo (_order1.ID));
    }

    private ObjectList<OrderItem> GetOrderItemsInFetchTransaction (Order order)
    {
      return _relationEndPointMap.ClientTransaction.Execute (() => order.OrderItems);
    }
  }
}
