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
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainImplementation
{
  [TestFixture]
  public class BidirectionalRelationSyncServiceTest : StandardMappingTest
  {
    private ClientTransaction _transaction;
    private Order _order;

    public override void SetUp ()
    {
      base.SetUp();

      _transaction = ClientTransaction.CreateRootTransaction();
      _order = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));
    }
    
    [Test]
    public void IsSynchronized_True_OneMany ()
    {
      _transaction.Execute (() => _order.OrderItems.EnsureDataComplete());

      var orderItem = _transaction.Execute (() => _order.OrderItems[0]);

      Assert.That (BidirectionalRelationSyncService.IsSynchronized (_transaction, RelationEndPointID.Create (_order, o => o.OrderItems)), Is.True);
      Assert.That (BidirectionalRelationSyncService.IsSynchronized (_transaction, RelationEndPointID.Create (orderItem, oi => oi.Order)), Is.True);
    }

    [Test]
    public void IsSynchronized_True_OneOne ()
    {
      var orderTicket = _transaction.Execute (() => _order.OrderTicket);

      Assert.That (BidirectionalRelationSyncService.IsSynchronized (_transaction, RelationEndPointID.Create (_order, o => o.OrderTicket)), Is.True);
      Assert.That (BidirectionalRelationSyncService.IsSynchronized (_transaction, RelationEndPointID.Create (orderTicket, ot => ot.Order)), Is.True);
    }

    [Test]
    public void IsSynchronized_False_OneMany ()
    {
      _transaction.Execute (() => _order.OrderItems.EnsureDataComplete ());

      var orderItem = _transaction.Execute (() => _order.OrderItems[0]);

      SetDatabaseModifyable ();

      var newOrderItemID = CreateOrderItemAndSetOrderInOtherTransaction (_order.ID);
      var newOrderItem = _transaction.Execute (() => OrderItem.GetObject (newOrderItemID));

      Assert.That (_transaction.Execute (() => newOrderItem.Order), Is.SameAs (_order));
      Assert.That (_transaction.Execute (() => _order.OrderItems), List.Not.Contains (newOrderItem));

      Assert.That (BidirectionalRelationSyncService.IsSynchronized (_transaction, RelationEndPointID.Create (newOrderItem, oi => oi.Order)), Is.False);
      Assert.That (BidirectionalRelationSyncService.IsSynchronized (_transaction, RelationEndPointID.Create (_order, o => o.OrderItems)), Is.False);
      
      Assert.That (BidirectionalRelationSyncService.IsSynchronized (_transaction, RelationEndPointID.Create (orderItem, oi => oi.Order)), Is.True);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The relation property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket' of object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' has not yet been loaded into the given ClientTransaction.")]
    public void IsSynchronized_RelationNotLoaded ()
    {
      BidirectionalRelationSyncService.IsSynchronized (_transaction, RelationEndPointID.Create (_order, o => o.OrderTicket));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "IsSynchronized cannot be called for unidirectional relation end-points.\r\nParameter name: endPointID")]
    public void IsSynchronized_UnidirectionalRelationEndPoint ()
    {
      BidirectionalRelationSyncService.IsSynchronized (_transaction, RelationEndPointID.Create (DomainObjectIDs.Location1, typeof (Location), "Client"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "IsSynchronized cannot be called for unidirectional relation end-points.\r\nParameter name: endPointID")]
    public void IsSynchronized_AnonymousRelationEndPoint ()
    {
      var locationClientEndPoint = RelationEndPointID.Create (DomainObjectIDs.Location1, typeof (Location), "Client");
      var oppositeEndPoint = RelationEndPointID.Create (DomainObjectIDs.Client1, locationClientEndPoint.Definition.GetOppositeEndPointDefinition());
      BidirectionalRelationSyncService.IsSynchronized (_transaction, oppositeEndPoint);
    }

    private ObjectID CreateOrderItemAndSetOrderInOtherTransaction (ObjectID orderID)
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var newItem = OrderItem.NewObject();
        var order = Order.GetObject (orderID);
        order.OrderItems.Add (newItem);
        ClientTransaction.Current.Commit ();
        return newItem.ID;
      }
    }
  }
}