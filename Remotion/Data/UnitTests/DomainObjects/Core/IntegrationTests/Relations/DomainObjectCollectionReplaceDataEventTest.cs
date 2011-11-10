// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Relations
{
  [TestFixture]
  public class DomainObjectCollectionReplaceDataEventTest : ClientTransactionBaseTest
  {
    private Customer _customer;
    private OrderCollection.ICollectionEventReceiver _eventReceiverMock;

    private Order _itemA;
    private Order _itemB;

    public override void SetUp ()
    {
      base.SetUp();

      _customer = Customer.GetObject (DomainObjectIDs.Customer1);
      _eventReceiverMock = MockRepository.GenerateStrictMock<OrderCollection.ICollectionEventReceiver>();

      _itemA = Order.GetObject (DomainObjectIDs.Order1);
      _itemB = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);
    }

    [Test]
    public void Load ()
    {
      var orderCollection = _customer.Orders;
      _eventReceiverMock
          .Expect (mock => mock.OnReplaceData())
          .WhenCalled (mi => Assert.That (orderCollection, Is.EqualTo (new[] { _itemA, _itemB })));
      _eventReceiverMock.Replay();

      UnloadService.UnloadVirtualEndPoint (ClientTransactionMock, _customer.Orders.AssociatedEndPointID);
      orderCollection.SetEventReceiver (_eventReceiverMock);

      _customer.Orders.EnsureDataComplete();

      _eventReceiverMock.VerifyAllExpectations();
    }

    [Test]
    public void Rollback ()
    {
      _customer.Orders.Clear();

      var orderCollection = _customer.Orders;
      _eventReceiverMock
          .Expect (mock => mock.OnReplaceData())
          .WhenCalled (mi => Assert.That (orderCollection, Is.EqualTo (new[] { _itemA, _itemB })));
      _eventReceiverMock.Replay();
      _customer.Orders.SetEventReceiver (_eventReceiverMock);

      ClientTransactionMock.Rollback();

      _eventReceiverMock.VerifyAllExpectations();
    }

    [Test]
    public void Synchronize ()
    {
      var orderCollection = _customer.Orders;
      _customer.Orders.EnsureDataComplete();

      _eventReceiverMock
          .Expect (mock => mock.OnReplaceData())
          .WhenCalled (mi => Assert.That (orderCollection, Is.EqualTo (new[] { _itemA, _itemB })));
      _eventReceiverMock.Replay();
      _customer.Orders.SetEventReceiver (_eventReceiverMock);

      BidirectionalRelationSyncService.Synchronize (ClientTransactionMock, _customer.Orders.AssociatedEndPointID);

      _eventReceiverMock.VerifyAllExpectations();
    }

    [Test]
    public void SynchronizeOppositeEndPoint ()
    {
      // Prepare an item in unsynchronized state
      Order unsynchronizedOrder = PrepareUnsynchronizedOrder(DomainObjectIDs.Order3, _customer.ID);

      var orderCollection = _customer.Orders;
      orderCollection.EnsureDataComplete();

      _eventReceiverMock
          .Expect (mock => mock.OnReplaceData())
          .WhenCalled (mi => Assert.That (orderCollection, Is.EqualTo (new[] { _itemA, _itemB, unsynchronizedOrder })));
      _eventReceiverMock.Replay();
      _customer.Orders.SetEventReceiver (_eventReceiverMock);

      BidirectionalRelationSyncService.Synchronize (ClientTransactionMock, RelationEndPointID.Create (unsynchronizedOrder, o => o.Customer));

      _eventReceiverMock.VerifyAllExpectations();
    }

    [Test]
    public void CommitSubTransaction ()
    {
      _customer.Orders.EnsureDataComplete();

      var orderCollection = _customer.Orders;
      _eventReceiverMock
          .Expect (mock => mock.OnReplaceData())
          .WhenCalled (mi => Assert.That (orderCollection.Count, Is.EqualTo (3)));
      _eventReceiverMock.Replay();
      _customer.Orders.SetEventReceiver (_eventReceiverMock);

      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        var newOrder = Order.NewObject();
        newOrder.OrderTicket = OrderTicket.NewObject();
        newOrder.OrderItems.Add (OrderItem.NewObject ());
        newOrder.Official = Official.NewObject();

        _customer.Orders.Add (newOrder);
        ClientTransaction.Current.Commit();
      }

      _eventReceiverMock.VerifyAllExpectations();
    }

    private Order PrepareUnsynchronizedOrder (ObjectID orderID, ObjectID relatedCustomerID)
    {
      var unsynchronizedOrder = (Order) LifetimeService.GetObjectReference (ClientTransactionMock, orderID);
      var dataContainer = DataContainer.CreateForExisting (
          unsynchronizedOrder.ID,
          null,
          pd => pd.PropertyName.EndsWith ("Customer") ? relatedCustomerID : pd.DefaultValue);
      dataContainer.SetDomainObject (unsynchronizedOrder);
      ClientTransactionMock.DataManager.RegisterDataContainer (dataContainer);

      var endPointID = RelationEndPointID.Create (unsynchronizedOrder, o => o.Customer);
      var endPoint = (IRealObjectEndPoint) ClientTransactionMock.DataManager.GetRelationEndPointWithoutLoading (endPointID);

      var oppositeID = RelationEndPointID.CreateOpposite (endPoint.Definition, relatedCustomerID);
      var oppositeEndPoint = ClientTransactionMock.DataManager.GetOrCreateVirtualEndPoint (oppositeID);
      oppositeEndPoint.EnsureDataComplete ();

      Assert.That (endPoint.IsSynchronized, Is.False);
      return unsynchronizedOrder;
    }
  }
}