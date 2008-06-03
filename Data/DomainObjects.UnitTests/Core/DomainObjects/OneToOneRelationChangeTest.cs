/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.Core.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.DomainObjects
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
      Assert.AreEqual ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", _orderEventReceiver.ChangingRelationPropertyName);
      Assert.AreEqual ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", _orderEventReceiver.ChangedRelationPropertyName);
      Assert.AreSame (_oldOrderTicket, _orderEventReceiver.OldRelatedObject);
      Assert.AreSame (_newOrderTicket, _orderEventReceiver.NewRelatedObject);

      Assert.AreEqual (true, _oldOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.AreEqual (true, _oldOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreEqual ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", _oldOrderTicketEventReceiver.ChangingRelationPropertyName);
      Assert.AreEqual ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", _oldOrderTicketEventReceiver.ChangedRelationPropertyName);
      Assert.AreSame (_order, _oldOrderTicketEventReceiver.OldRelatedObject);
      Assert.AreSame (null, _oldOrderTicketEventReceiver.NewRelatedObject);

      Assert.AreEqual (true, _newOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.AreEqual (true, _newOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreEqual ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", _newOrderTicketEventReceiver.ChangingRelationPropertyName);
      Assert.AreEqual ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", _newOrderTicketEventReceiver.ChangedRelationPropertyName);
      Assert.AreSame (_oldOrderOfNewOrderTicket, _newOrderTicketEventReceiver.OldRelatedObject);
      Assert.AreSame (_order, _newOrderTicketEventReceiver.NewRelatedObject);

      Assert.AreEqual (true, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.AreEqual (true, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreEqual ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", _oldOrderOfNewOrderTicketEventReceiver.ChangingRelationPropertyName);
      Assert.AreEqual ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", _oldOrderOfNewOrderTicketEventReceiver.ChangedRelationPropertyName);
      Assert.AreSame (_newOrderTicket, _oldOrderOfNewOrderTicketEventReceiver.OldRelatedObject);
      Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.NewRelatedObject);

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
        Assert.AreEqual ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", _orderEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_orderEventReceiver.ChangedRelationPropertyName);
        Assert.AreSame (_oldOrderTicket, _orderEventReceiver.OldRelatedObject);
        Assert.AreSame (_newOrderTicket, _orderEventReceiver.NewRelatedObject);

        Assert.AreEqual (false, _oldOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _oldOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.IsNull (_oldOrderTicketEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_oldOrderTicketEventReceiver.ChangedRelationPropertyName);
        Assert.IsNull (_oldOrderTicketEventReceiver.OldRelatedObject);
        Assert.IsNull (_oldOrderTicketEventReceiver.NewRelatedObject);

        Assert.AreEqual (false, _newOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _newOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.IsNull (_newOrderTicketEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_newOrderTicketEventReceiver.ChangedRelationPropertyName);
        Assert.IsNull (_newOrderTicketEventReceiver.OldRelatedObject);
        Assert.IsNull (_newOrderTicketEventReceiver.NewRelatedObject);

        Assert.AreEqual (false, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangedRelationPropertyName);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.OldRelatedObject);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.NewRelatedObject);

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
        Assert.AreEqual ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", _orderEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_orderEventReceiver.ChangedRelationPropertyName);
        Assert.AreSame (_oldOrderTicket, _orderEventReceiver.OldRelatedObject);
        Assert.AreSame (_newOrderTicket, _orderEventReceiver.NewRelatedObject);

        Assert.AreEqual (true, _oldOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _oldOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", _oldOrderTicketEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_oldOrderTicketEventReceiver.ChangedRelationPropertyName);
        Assert.AreSame (_order, _oldOrderTicketEventReceiver.OldRelatedObject);
        Assert.AreSame (null, _oldOrderTicketEventReceiver.NewRelatedObject);

        Assert.AreEqual (false, _newOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _newOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.IsNull (_newOrderTicketEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_newOrderTicketEventReceiver.ChangedRelationPropertyName);
        Assert.IsNull (_newOrderTicketEventReceiver.OldRelatedObject);
        Assert.IsNull (_newOrderTicketEventReceiver.NewRelatedObject);

        Assert.AreEqual (false, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangedRelationPropertyName);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.OldRelatedObject);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.NewRelatedObject);

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
        Assert.AreEqual ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", _orderEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_orderEventReceiver.ChangedRelationPropertyName);
        Assert.AreSame (_oldOrderTicket, _orderEventReceiver.OldRelatedObject);
        Assert.AreSame (_newOrderTicket, _orderEventReceiver.NewRelatedObject);

        Assert.AreEqual (true, _oldOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _oldOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", _oldOrderTicketEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_oldOrderTicketEventReceiver.ChangedRelationPropertyName);
        Assert.AreSame (_order, _oldOrderTicketEventReceiver.OldRelatedObject);
        Assert.AreSame (null, _oldOrderTicketEventReceiver.NewRelatedObject);

        Assert.AreEqual (true, _newOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _newOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", _newOrderTicketEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_newOrderTicketEventReceiver.ChangedRelationPropertyName);
        Assert.AreSame (_oldOrderOfNewOrderTicket, _newOrderTicketEventReceiver.OldRelatedObject);
        Assert.AreSame (_order, _newOrderTicketEventReceiver.NewRelatedObject);

        Assert.AreEqual (false, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangedRelationPropertyName);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.OldRelatedObject);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.NewRelatedObject);

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
        Assert.AreEqual ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", _orderEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_orderEventReceiver.ChangedRelationPropertyName);
        Assert.AreSame (_oldOrderTicket, _orderEventReceiver.OldRelatedObject);
        Assert.AreSame (_newOrderTicket, _orderEventReceiver.NewRelatedObject);

        Assert.AreEqual (true, _oldOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _oldOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", _oldOrderTicketEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_oldOrderTicketEventReceiver.ChangedRelationPropertyName);
        Assert.AreSame (_order, _oldOrderTicketEventReceiver.OldRelatedObject);
        Assert.AreSame (null, _oldOrderTicketEventReceiver.NewRelatedObject);

        Assert.AreEqual (true, _newOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _newOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", _newOrderTicketEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_newOrderTicketEventReceiver.ChangedRelationPropertyName);
        Assert.AreSame (_oldOrderOfNewOrderTicket, _newOrderTicketEventReceiver.OldRelatedObject);
        Assert.AreSame (_order, _newOrderTicketEventReceiver.NewRelatedObject);

        Assert.AreEqual (true, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.AreEqual (false, _oldOrderOfNewOrderTicketEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", _oldOrderOfNewOrderTicketEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.ChangedRelationPropertyName);
        Assert.AreSame (_newOrderTicket, _oldOrderOfNewOrderTicketEventReceiver.OldRelatedObject);
        Assert.IsNull (_oldOrderOfNewOrderTicketEventReceiver.NewRelatedObject);

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

      Assert.IsNull (_oldOrderTicket.InternalDataContainer.GetValue ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"));
      Assert.AreEqual (_order.ID, _newOrderTicket.InternalDataContainer.GetValue ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"));

      Assert.AreSame (_newOrderTicket, _order.OrderTicket);
      Assert.AreSame (_order, _newOrderTicket.Order);
      Assert.IsNull (_oldOrderTicket.Order);
      Assert.IsNull (_oldOrderOfNewOrderTicket.OrderTicket);
    }

    [Test]
    public void ChangeRelation ()
    {
      _newOrderTicket.Order = _order;

      Assert.IsNull (_oldOrderTicket.InternalDataContainer.GetValue ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"));
      Assert.AreEqual (_order.ID, _newOrderTicket.InternalDataContainer.GetValue ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"));

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
          new RelationEndPointID (_order.ID, typeof (Order).FullName + ".OrderTicket"),
          new RelationEndPointID (oldOrder.ID, typeof (Order).FullName + ".OrderTicket"),
          new RelationEndPointID (_oldOrderTicket.ID, typeof (OrderTicket).FullName + ".Order"),
          new RelationEndPointID (_newOrderTicket.ID, typeof (OrderTicket).FullName + ".Order"));
    }

    [Test]
    public void HasBeenTouched_RealSide ()
    {
      Order oldOrder = _newOrderTicket.Order;

      Assert.IsFalse (_oldOrderTicket.InternalDataContainer.PropertyValues[typeof (OrderTicket).FullName + ".Order"].HasBeenTouched);

      CheckTouching (delegate { _newOrderTicket.Order = _order; }, _newOrderTicket, "Order",
          new RelationEndPointID (_order.ID, typeof (Order).FullName + ".OrderTicket"),
          new RelationEndPointID (oldOrder.ID, typeof (Order).FullName + ".OrderTicket"),
          new RelationEndPointID (_oldOrderTicket.ID, typeof (OrderTicket).FullName + ".Order"),
          new RelationEndPointID (_newOrderTicket.ID, typeof (OrderTicket).FullName + ".Order"));

      Assert.IsTrue (_oldOrderTicket.InternalDataContainer.PropertyValues[typeof (OrderTicket).FullName + ".Order"].HasBeenTouched);
    }

    [Test]
    public void HasBeenTouched_VirtualSide_OriginalValue ()
    {
      CheckTouching (delegate { _order.OrderTicket = _order.OrderTicket; }, _order.OrderTicket, "Order",
          new RelationEndPointID (_order.ID, typeof (Order).FullName + ".OrderTicket"),
          new RelationEndPointID (_oldOrderTicket.ID, typeof (OrderTicket).FullName + ".Order"));
    }

    [Test]
    public void HasBeenTouched_RealSide_OriginalValue ()
    {
      CheckTouching (delegate { _oldOrderTicket.Order = _order; }, _oldOrderTicket, "Order",
          new RelationEndPointID (_order.ID, typeof (Order).FullName + ".OrderTicket"),
          new RelationEndPointID (_oldOrderTicket.ID, typeof (OrderTicket).FullName + ".Order"));
    }

    [Test]
    public void GetOriginalRelatedObject ()
    {
      Assert.AreSame (_oldOrderTicket, _order.GetOriginalRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));

      _order.OrderTicket = _newOrderTicket;

      Assert.AreSame (_newOrderTicket, _order.OrderTicket);
      Assert.AreSame (_oldOrderTicket, _order.GetOriginalRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));
    }

    [Test]
    public void GetOriginalRelatedObjectWithLazyLoad ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order2);

      Assert.AreEqual (DomainObjectIDs.OrderTicket3, order.GetOriginalRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket").ID);
    }

    [Test]
    public void GetNullOriginalRelatedObject ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      Assert.IsNull (computer.GetOriginalRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"));
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
        orderTicket.SetRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", Customer.GetObject (DomainObjectIDs.Customer1));

        Assert.Fail ("DataManagementException was expected");
      }
      catch (DataManagementException ex)
      {
        string expectedMessage = string.Format (
            "DomainObject '{0}' cannot be assigned to property '{1}' of DomainObject '{2}',"
            + " because it is not compatible with the type of the property.",
            DomainObjectIDs.Customer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", DomainObjectIDs.OrderTicket1);

        Assert.AreEqual (expectedMessage, ex.Message);
      }
    }

    [Test]
    [ExpectedException (typeof (DataManagementException))]
    public void SetRelatedObjectWithInvalidObjectClassOnVirtualRelationEndPoint ()
    {
      _order.SetRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", Ceo.NewObject ());
    }
  }
}
