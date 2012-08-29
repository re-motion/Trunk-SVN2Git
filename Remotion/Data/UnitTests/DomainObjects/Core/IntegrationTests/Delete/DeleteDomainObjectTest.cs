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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Delete
{
  [TestFixture]
  public class DeleteDomainObjectTest : ClientTransactionBaseTest
  {
    Order _order;
    OrderTicket _orderTicket;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      SetDatabaseModifyable ();
    }

    public override void SetUp ()
    {
      base.SetUp ();

      _order = Order.GetObject (DomainObjectIDs.Order2);
      _orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
    }

    [Test]
    public void Delete ()
    {
      _orderTicket.Delete ();

      Assert.AreEqual (StateType.Deleted, _orderTicket.State);
      Assert.AreEqual (StateType.Deleted, _orderTicket.InternalDataContainer.State);
    }

    [Test]
    public void DeleteTwice ()
    {
      _orderTicket.Delete ();

      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (_orderTicket);
      _orderTicket.Delete ();

      Assert.AreEqual (0, eventReceiver.Count);
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void GetObject ()
    {
      _orderTicket.Delete ();

      OrderTicket.GetObject (_orderTicket.ID);
    }

    [Test]
    public void GetObjectAndIncludeDeleted ()
    {
      _orderTicket.Delete ();

      Assert.IsNotNull (OrderTicket.GetObject (_orderTicket.ID, true));
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void ModifyDeletedObject ()
    {
      var dataContainer = _order.InternalDataContainer;

      _order.Delete ();

      SetPropertyValue (dataContainer, typeof (Order), "OrderNumber", 10);
    }

    [Test]
    public void AccessDeletedObject ()
    {
      _order.Delete ();

      Assert.AreEqual (DomainObjectIDs.Order2, _order.ID);
      Assert.AreEqual (3, _order.OrderNumber);
      Assert.AreEqual (new DateTime (2005, 3, 1), _order.DeliveryDate);
      Assert.IsNotNull (_order.InternalDataContainer.Timestamp);
      Assert.IsNotNull (GetPropertyValue (_order.InternalDataContainer, typeof (Order), "OrderNumber"));
    }

    [Test]
    public void CascadedDelete ()
    {
      Employee supervisor = Employee.GetObject (DomainObjectIDs.Employee1);
      supervisor.DeleteWithSubordinates ();

      DomainObject deletedSubordinate4 = Employee.GetObject (DomainObjectIDs.Employee4, true);
      DomainObject deletedSubordinate5 = Employee.GetObject (DomainObjectIDs.Employee5, true);

      Assert.AreEqual (StateType.Deleted, supervisor.State);
      Assert.AreEqual (StateType.Deleted, deletedSubordinate4.State);
      Assert.AreEqual (StateType.Deleted, deletedSubordinate5.State);

      TestableClientTransaction.Commit ();
      ReInitializeTransaction ();

      CheckIfObjectIsDeleted (DomainObjectIDs.Employee1);
      CheckIfObjectIsDeleted (DomainObjectIDs.Employee4);
      CheckIfObjectIsDeleted (DomainObjectIDs.Employee5);
    }

    [Test]
    public void CascadedDeleteForNewObjects ()
    {
      Order newOrder = Order.NewObject ();
      OrderTicket newOrderTicket = OrderTicket.NewObject (newOrder);
      Assert.AreSame (newOrderTicket, newOrder.OrderTicket);
      OrderItem newOrderItem = OrderItem.NewObject (newOrder);
      Assert.Contains (newOrderItem, newOrder.OrderItems);

      newOrder.Deleted += delegate
      {
        newOrderTicket.Delete ();
        newOrderItem.Delete ();
      };

      newOrder.Delete ();

      //Expectation: no exception

      Assert.IsTrue (newOrder.IsInvalid);
      Assert.IsTrue (newOrderTicket.IsInvalid);
      Assert.IsTrue (newOrderItem.IsInvalid);
    }
  }
}
