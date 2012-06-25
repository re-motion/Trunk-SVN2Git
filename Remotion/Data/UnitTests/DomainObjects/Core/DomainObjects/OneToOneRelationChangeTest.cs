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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public class OneToOneRelationChangeTest : RelationChangeBaseTest
  {
    private Order _order;
    private OrderTicket _oldOrderTicket;
    private OrderTicket _newOrderTicket;
    private Order _oldOrderOfNewOrderTicket;

    private DomainObjectEventReceiver _orderEventReceiver;
    private DomainObjectEventReceiver _oldOrderTicketEventReceiver;
    private DomainObjectEventReceiver _newOrderTicketEventReceiver;
    private DomainObjectEventReceiver _oldOrderOfNewOrderTicketEventReceiver;

    public override void SetUp ()
    {
      base.SetUp ();

      _order = Order.GetObject (DomainObjectIDs.Order1);
      _oldOrderTicket = _order.OrderTicket;
      _newOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);
      _oldOrderOfNewOrderTicket = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);

      _orderEventReceiver = new DomainObjectEventReceiver (_order);
      _oldOrderTicketEventReceiver = new DomainObjectEventReceiver (_oldOrderTicket);
      _newOrderTicketEventReceiver = new DomainObjectEventReceiver (_newOrderTicket);
      _oldOrderOfNewOrderTicketEventReceiver = new DomainObjectEventReceiver (_oldOrderOfNewOrderTicket);
    }

    [Test]
    public void RelationChangeEvents ()
    {
      _orderEventReceiver.Cancel = false;
      _oldOrderTicketEventReceiver.Cancel = false;
      _newOrderTicketEventReceiver.Cancel = false;
      _oldOrderOfNewOrderTicketEventReceiver.Cancel = false;

      _order.OrderTicket = _newOrderTicket;

      Assert.AreEqual (true, _orderEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.AreEqual (true, _orderEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", _orderEventReceiver.ChangingRelationPropertyName);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", _orderEventReceiver.ChangedRelationPropertyName);
      Assert.AreSame (_oldOrderTicket, _orderEventReceiver.ChangingOldRelatedObject);
      Assert.AreSame (_newOrderTicket, _orderEventReceiver.ChangingNewRelatedObject);
      Assert.AreSame (_oldOrderTicket, _orderEventReceiver.ChangedOldRelatedObject);
      Assert.AreSame (_newOrderTicket, _orderEventReceiver.ChangedNewRelatedObject);

      Assert.AreEqual (true, _oldOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.AreEqual (true, _oldOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", _oldOrderTicketEventReceiver.ChangingRelationPropertyName);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", _oldOrderTicketEventReceiver.ChangedRelationPropertyName);
      Assert.AreSame (_order, _oldOrderTicketEventReceiver.ChangingOldRelatedObject);
      Assert.AreSame (null, _oldOrderTicketEventReceiver.ChangingNewRelatedObject);
      Assert.AreSame (_order, _oldOrderTicketEventReceiver.ChangedOldRelatedObject);
      Assert.AreSame (null, _oldOrderTicketEventReceiver.ChangedNewRelatedObject);

      Assert.AreEqual (true, _newOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.AreEqual (true, _newOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", _newOrderTicketEventReceiver.ChangingRelationPropertyName);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", _newOrderTicketEventReceiver.ChangedRelationPropertyName);
      Assert.AreSame (_oldOrderOfNewOrderTicket, _newOrderTicketEventReceiver.ChangingOldRelatedObject);
      Assert.AreSame (_order, _newOrderTicketEventReceiver.ChangingNewRelatedObject);
      Assert.AreSame (_oldOrderOfNewOrderTicket, _newOrderTicketEventReceiver.ChangedOldRelatedObject);
      Assert.AreSame (_order, _newOrderTicketEventReceiver.ChangedNewRelatedObject);

      Assert.AreEqual (true, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.AreEqual (true, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", _oldOrderOfNewOrderTicketEventReceiver.ChangingRelationPropertyName);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", _oldOrderOfNewOrderTicketEventReceiver.ChangedRelationPropertyName);
      Assert.AreSame (_newOrderTicket, _oldOrderOfNewOrderTicketEventReceiver.ChangingOldRelatedObject);
      Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangingNewRelatedObject);
      Assert.AreSame (_newOrderTicket, _oldOrderOfNewOrderTicketEventReceiver.ChangedOldRelatedObject);
      Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangedNewRelatedObject);

      Assert.AreEqual (StateType.Changed, _order.State);
      Assert.AreEqual (StateType.Changed, _newOrderTicket.State);
      Assert.AreEqual (StateType.Changed, _oldOrderTicket.State);
      Assert.AreEqual (StateType.Changed, _oldOrderOfNewOrderTicket.State);

      Assert.AreEqual (StateType.Unchanged, _order.InternalDataContainer.State);
      Assert.AreEqual (StateType.Changed, _newOrderTicket.InternalDataContainer.State);
      Assert.AreEqual (StateType.Changed, _oldOrderTicket.InternalDataContainer.State);
      Assert.AreEqual (StateType.Unchanged, _oldOrderOfNewOrderTicket.InternalDataContainer.State);

      Assert.AreSame (_newOrderTicket, _order.OrderTicket);
      Assert.AreSame (_order, _newOrderTicket.Order);
      Assert.IsNull (_oldOrderTicket.Order);
      Assert.IsNull (_oldOrderOfNewOrderTicket.OrderTicket);
    }

    [Test]
    public void OrderCancelsRelationChangeEvent ()
    {
      _orderEventReceiver.Cancel = true;
      _oldOrderTicketEventReceiver.Cancel = false;
      _newOrderTicketEventReceiver.Cancel = false;
      _oldOrderOfNewOrderTicketEventReceiver.Cancel = false;

      try
      {
        _order.OrderTicket = _newOrderTicket;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.AreEqual (true, _orderEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _orderEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", _orderEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_orderEventReceiver.ChangedRelationPropertyName);
        Assert.AreSame (_oldOrderTicket, _orderEventReceiver.ChangingOldRelatedObject);
        Assert.AreSame (_newOrderTicket, _orderEventReceiver.ChangingNewRelatedObject);

        Assert.AreEqual (false, _oldOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _oldOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.IsNull (_oldOrderTicketEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_oldOrderTicketEventReceiver.ChangedRelationPropertyName);
        Assert.IsNull (_oldOrderTicketEventReceiver.ChangingOldRelatedObject);
        Assert.IsNull (_oldOrderTicketEventReceiver.ChangingNewRelatedObject);

        Assert.AreEqual (false, _newOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _newOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.IsNull (_newOrderTicketEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_newOrderTicketEventReceiver.ChangedRelationPropertyName);
        Assert.IsNull (_newOrderTicketEventReceiver.ChangingOldRelatedObject);
        Assert.IsNull (_newOrderTicketEventReceiver.ChangingNewRelatedObject);

        Assert.AreEqual (false, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangedRelationPropertyName);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangingOldRelatedObject);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangingNewRelatedObject);

        Assert.AreEqual (StateType.Unchanged, _order.State);
        Assert.AreEqual (StateType.Unchanged, _newOrderTicket.State);
        Assert.AreEqual (StateType.Unchanged, _oldOrderTicket.State);
        Assert.AreEqual (StateType.Unchanged, _oldOrderOfNewOrderTicket.State);

        Assert.AreSame (_oldOrderTicket, _order.OrderTicket);
        Assert.AreSame (_oldOrderOfNewOrderTicket, _newOrderTicket.Order);
        Assert.AreSame (_order, _oldOrderTicket.Order);
        Assert.AreSame (_newOrderTicket, _oldOrderOfNewOrderTicket.OrderTicket);
      }
    }

    [Test]
    public void OldRelatedObjectCancelsRelationChange ()
    {
      _orderEventReceiver.Cancel = false;
      _oldOrderTicketEventReceiver.Cancel = true;
      _newOrderTicketEventReceiver.Cancel = false;
      _oldOrderOfNewOrderTicketEventReceiver.Cancel = false;

      try
      {
        _order.OrderTicket = _newOrderTicket;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.AreEqual (true, _orderEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _orderEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", _orderEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_orderEventReceiver.ChangedRelationPropertyName);
        Assert.AreSame (_oldOrderTicket, _orderEventReceiver.ChangingOldRelatedObject);
        Assert.AreSame (_newOrderTicket, _orderEventReceiver.ChangingNewRelatedObject);

        Assert.AreEqual (true, _oldOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _oldOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", _oldOrderTicketEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_oldOrderTicketEventReceiver.ChangedRelationPropertyName);
        Assert.AreSame (_order, _oldOrderTicketEventReceiver.ChangingOldRelatedObject);
        Assert.AreSame (null, _oldOrderTicketEventReceiver.ChangingNewRelatedObject);

        Assert.AreEqual (false, _newOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _newOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.IsNull (_newOrderTicketEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_newOrderTicketEventReceiver.ChangedRelationPropertyName);
        Assert.IsNull (_newOrderTicketEventReceiver.ChangingOldRelatedObject);
        Assert.IsNull (_newOrderTicketEventReceiver.ChangingNewRelatedObject);

        Assert.AreEqual (false, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangedRelationPropertyName);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangingOldRelatedObject);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangingNewRelatedObject);

        Assert.AreEqual (StateType.Unchanged, _order.State);
        Assert.AreEqual (StateType.Unchanged, _newOrderTicket.State);
        Assert.AreEqual (StateType.Unchanged, _oldOrderTicket.State);
        Assert.AreEqual (StateType.Unchanged, _oldOrderOfNewOrderTicket.State);

        Assert.AreSame (_oldOrderTicket, _order.OrderTicket);
        Assert.AreSame (_oldOrderOfNewOrderTicket, _newOrderTicket.Order);
        Assert.AreSame (_order, _oldOrderTicket.Order);
        Assert.AreSame (_newOrderTicket, _oldOrderOfNewOrderTicket.OrderTicket);
      }
    }

    [Test]
    public void NewRelatedObjectCancelsRelationChange ()
    {
      _orderEventReceiver.Cancel = false;
      _oldOrderTicketEventReceiver.Cancel = false;
      _newOrderTicketEventReceiver.Cancel = true;
      _oldOrderOfNewOrderTicketEventReceiver.Cancel = false;

      try
      {
        _order.OrderTicket = _newOrderTicket;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.AreEqual (true, _orderEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _orderEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", _orderEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_orderEventReceiver.ChangedRelationPropertyName);
        Assert.AreSame (_oldOrderTicket, _orderEventReceiver.ChangingOldRelatedObject);
        Assert.AreSame (_newOrderTicket, _orderEventReceiver.ChangingNewRelatedObject);

        Assert.AreEqual (true, _oldOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _oldOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", _oldOrderTicketEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_oldOrderTicketEventReceiver.ChangedRelationPropertyName);
        Assert.AreSame (_order, _oldOrderTicketEventReceiver.ChangingOldRelatedObject);
        Assert.AreSame (null, _oldOrderTicketEventReceiver.ChangingNewRelatedObject);

        Assert.AreEqual (true, _newOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _newOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", _newOrderTicketEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_newOrderTicketEventReceiver.ChangedRelationPropertyName);
        Assert.AreSame (_oldOrderOfNewOrderTicket, _newOrderTicketEventReceiver.ChangingOldRelatedObject);
        Assert.AreSame (_order, _newOrderTicketEventReceiver.ChangingNewRelatedObject);

        Assert.AreEqual (false, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangedRelationPropertyName);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangingOldRelatedObject);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangingNewRelatedObject);

        Assert.AreEqual (StateType.Unchanged, _order.State);
        Assert.AreEqual (StateType.Unchanged, _newOrderTicket.State);
        Assert.AreEqual (StateType.Unchanged, _oldOrderTicket.State);
        Assert.AreEqual (StateType.Unchanged, _oldOrderOfNewOrderTicket.State);

        Assert.AreSame (_oldOrderTicket, _order.OrderTicket);
        Assert.AreSame (_oldOrderOfNewOrderTicket, _newOrderTicket.Order);
        Assert.AreSame (_order, _oldOrderTicket.Order);
        Assert.AreSame (_newOrderTicket, _oldOrderOfNewOrderTicket.OrderTicket);
      }
    }

    [Test]
    public void OldRelatedObjectOfNewRelatedObjectCancelsRelationChange ()
    {
      _orderEventReceiver.Cancel = false;
      _oldOrderTicketEventReceiver.Cancel = false;
      _newOrderTicketEventReceiver.Cancel = false;
      _oldOrderOfNewOrderTicketEventReceiver.Cancel = true;

      try
      {
        _order.OrderTicket = _newOrderTicket;
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.AreEqual (true, _orderEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _orderEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", _orderEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_orderEventReceiver.ChangedRelationPropertyName);
        Assert.AreSame (_oldOrderTicket, _orderEventReceiver.ChangingOldRelatedObject);
        Assert.AreSame (_newOrderTicket, _orderEventReceiver.ChangingNewRelatedObject);

        Assert.AreEqual (true, _oldOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _oldOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", _oldOrderTicketEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_oldOrderTicketEventReceiver.ChangedRelationPropertyName);
        Assert.AreSame (_order, _oldOrderTicketEventReceiver.ChangingOldRelatedObject);
        Assert.AreSame (null, _oldOrderTicketEventReceiver.ChangingNewRelatedObject);

        Assert.AreEqual (true, _newOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _newOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", _newOrderTicketEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_newOrderTicketEventReceiver.ChangedRelationPropertyName);
        Assert.AreSame (_oldOrderOfNewOrderTicket, _newOrderTicketEventReceiver.ChangingOldRelatedObject);
        Assert.AreSame (_order, _newOrderTicketEventReceiver.ChangingNewRelatedObject);

        Assert.AreEqual (true, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", _oldOrderOfNewOrderTicketEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangedRelationPropertyName);
        Assert.AreSame (_newOrderTicket, _oldOrderOfNewOrderTicketEventReceiver.ChangingOldRelatedObject);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangingNewRelatedObject);

        Assert.AreEqual (StateType.Unchanged, _order.State);
        Assert.AreEqual (StateType.Unchanged, _newOrderTicket.State);
        Assert.AreEqual (StateType.Unchanged, _oldOrderTicket.State);
        Assert.AreEqual (StateType.Unchanged, _oldOrderOfNewOrderTicket.State);

        Assert.AreSame (_oldOrderTicket, _order.OrderTicket);
        Assert.AreSame (_oldOrderOfNewOrderTicket, _newOrderTicket.Order);
        Assert.AreSame (_order, _oldOrderTicket.Order);
        Assert.AreSame (_newOrderTicket, _oldOrderOfNewOrderTicket.OrderTicket);
      }
    }

    [Test]
    public void StateTracking ()
    {
      Assert.AreEqual (StateType.Unchanged, _order.State);
      Assert.AreEqual (StateType.Unchanged, _newOrderTicket.State);
      Assert.AreEqual (StateType.Unchanged, _oldOrderTicket.State);
      Assert.AreEqual (StateType.Unchanged, _oldOrderOfNewOrderTicket.State);

      _order.OrderTicket = _newOrderTicket;

      Assert.AreEqual (StateType.Changed, _order.State);
      Assert.AreEqual (StateType.Changed, _newOrderTicket.State);
      Assert.AreEqual (StateType.Changed, _oldOrderTicket.State);
      Assert.AreEqual (StateType.Changed, _oldOrderOfNewOrderTicket.State);
    }

    [Test]
    public void OldObjectAndNewObjectAreSame ()
    {
      Assert.AreEqual (StateType.Unchanged, _order.State);
      Assert.AreEqual (StateType.Unchanged, _oldOrderTicket.State);

      _order.OrderTicket = _oldOrderTicket;

      Assert.AreEqual (StateType.Unchanged, _order.State);
      Assert.AreEqual (StateType.Unchanged, _oldOrderTicket.State);
    }

    [Test]
    public void ChangeRelationOverVirtualEndPoint ()
    {
      _order.OrderTicket = _newOrderTicket;

      Assert.IsNull (_oldOrderTicket.Properties[typeof (OrderTicket), "Order"].GetRelatedObjectID());
      Assert.AreEqual (_order.ID, _newOrderTicket.Properties[typeof (OrderTicket), "Order"].GetRelatedObjectID ());

      Assert.AreSame (_newOrderTicket, _order.OrderTicket);
      Assert.AreSame (_order, _newOrderTicket.Order);
      Assert.IsNull (_oldOrderTicket.Order);
      Assert.IsNull (_oldOrderOfNewOrderTicket.OrderTicket);
    }

    [Test]
    public void ChangeRelation ()
    {
      _newOrderTicket.Order = _order;

      Assert.IsNull (_oldOrderTicket.Properties[typeof (OrderTicket), "Order"].GetRelatedObjectID ());
      Assert.AreEqual (_order.ID, _newOrderTicket.Properties[typeof (OrderTicket), "Order"].GetRelatedObjectID ());

      Assert.AreSame (_order, _newOrderTicket.Order);
      Assert.AreSame (_newOrderTicket, _order.OrderTicket);
      Assert.IsNull (_oldOrderTicket.Order);
      Assert.IsNull (_oldOrderOfNewOrderTicket.OrderTicket);
    }

    [Test]
    public void ChangeRelationWithInheritance ()
    {
      Person person = Person.GetObject (DomainObjectIDs.Person1);
      Distributor distributor = Distributor.GetObject (DomainObjectIDs.Distributor1);

      person.AssociatedPartnerCompany = distributor;

      Assert.AreSame (distributor, person.AssociatedPartnerCompany);
      Assert.AreSame (person, distributor.ContactPerson);
    }

    [Test]
    public void ChangeRelationBackToOriginalValue ()
    {
      _order.OrderTicket = _newOrderTicket;
      Assert.AreEqual (StateType.Changed, _order.State);

      _order.OrderTicket = _oldOrderTicket;
      Assert.AreEqual (StateType.Unchanged, _order.State);
    }

    [Test]
    public void HasBeenTouched_VirtualSide ()
    {
      Order oldOrder = _newOrderTicket.Order;

      CheckTouching (delegate { _order.OrderTicket = _newOrderTicket; }, _newOrderTicket, "Order",
          RelationEndPointID.Create(_order.ID, typeof (Order).FullName + ".OrderTicket"),
          RelationEndPointID.Create(oldOrder.ID, typeof (Order).FullName + ".OrderTicket"),
          RelationEndPointID.Create(_oldOrderTicket.ID, typeof (OrderTicket).FullName + ".Order"),
          RelationEndPointID.Create(_newOrderTicket.ID, typeof (OrderTicket).FullName + ".Order"));
    }

    [Test]
    public void HasBeenTouched_RealSide ()
    {
      Order oldOrder = _newOrderTicket.Order;

      Assert.IsFalse (_oldOrderTicket.InternalDataContainer.PropertyValues[typeof (OrderTicket).FullName + ".Order"].HasBeenTouched);

      CheckTouching (delegate { _newOrderTicket.Order = _order; }, _newOrderTicket, "Order",
          RelationEndPointID.Create(_order.ID, typeof (Order).FullName + ".OrderTicket"),
          RelationEndPointID.Create(oldOrder.ID, typeof (Order).FullName + ".OrderTicket"),
          RelationEndPointID.Create(_oldOrderTicket.ID, typeof (OrderTicket).FullName + ".Order"),
          RelationEndPointID.Create(_newOrderTicket.ID, typeof (OrderTicket).FullName + ".Order"));

      Assert.IsTrue (_oldOrderTicket.InternalDataContainer.PropertyValues[typeof (OrderTicket).FullName + ".Order"].HasBeenTouched);
    }

    [Test]
    public void HasBeenTouched_VirtualSide_OriginalValue ()
    {
      CheckTouching (delegate { _order.OrderTicket = _order.OrderTicket; }, _order.OrderTicket, "Order",
          RelationEndPointID.Create(_order.ID, typeof (Order).FullName + ".OrderTicket"),
          RelationEndPointID.Create(_oldOrderTicket.ID, typeof (OrderTicket).FullName + ".Order"));
    }

    [Test]
    public void HasBeenTouched_RealSide_OriginalValue ()
    {
      CheckTouching (delegate { _oldOrderTicket.Order = _order; }, _oldOrderTicket, "Order",
          RelationEndPointID.Create(_order.ID, typeof (Order).FullName + ".OrderTicket"),
          RelationEndPointID.Create(_oldOrderTicket.ID, typeof (OrderTicket).FullName + ".Order"));
    }

    [Test]
    public void GetOriginalRelatedObject ()
    {
      Assert.AreSame (_oldOrderTicket, _order.GetOriginalRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"));

      _order.OrderTicket = _newOrderTicket;

      Assert.AreSame (_newOrderTicket, _order.OrderTicket);
      Assert.AreSame (_oldOrderTicket, _order.GetOriginalRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"));
    }

    [Test]
    public void GetOriginalRelatedObjectWithLazyLoad ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order2);

      Assert.AreEqual (DomainObjectIDs.OrderTicket3, order.GetOriginalRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket").ID);
    }

    [Test]
    public void GetNullOriginalRelatedObject ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      Assert.IsNull (computer.GetOriginalRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.Employee"));
    }

    [Test]
    public void OldObjectAndNewObjectAreSameRelationInherited ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer4);

      Ceo ceo = customer.Ceo;

      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (
          new DomainObject[] { customer, ceo },
          new DomainObjectCollection[0]);

      Assert.AreEqual (StateType.Unchanged, customer.State);
      Assert.AreEqual (StateType.Unchanged, ceo.State);

      customer.Ceo = ceo;

      Assert.AreEqual (StateType.Unchanged, customer.State);
      Assert.AreEqual (StateType.Unchanged, ceo.State);

      ChangeState[] expectedStates = new ChangeState[0];

      eventReceiver.Check (expectedStates);
    }

    [Test]
    public void SetRelatedObjectWithInvalidObjectClassOnRelationEndPoint ()
    {
      try
      {
        OrderTicket orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
        orderTicket.SetRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", Customer.GetObject (DomainObjectIDs.Customer1));

        Assert.Fail ("DataManagementException was expected");
      }
      catch (ArgumentTypeException ex)
      {
        string expectedMessage = string.Format (
            "DomainObject '{0}' cannot be assigned to property '{1}' of DomainObject '{2}',"
            + " because it is not compatible with the type of the property.\r\nParameter name: newRelatedObject",
            DomainObjectIDs.Customer1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", DomainObjectIDs.OrderTicket1);

        Assert.AreEqual (expectedMessage, ex.Message);
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void SetRelatedObjectWithInvalidObjectClassOnVirtualRelationEndPoint ()
    {
      _order.SetRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", Ceo.NewObject ());
    }
  }
}
