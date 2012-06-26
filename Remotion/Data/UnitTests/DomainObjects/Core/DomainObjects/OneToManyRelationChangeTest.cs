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
  public class OneToManyRelationChangeTest : RelationChangeBaseTest
  {
    private Customer _oldCustomer;
    private Customer _newCustomer;
    private Order _order1;
    private Order _orderWithoutOrderItem;

    public override void SetUp ()
    {
      base.SetUp ();

      _oldCustomer = Customer.GetObject (DomainObjectIDs.Customer1);
      _newCustomer = Customer.GetObject (DomainObjectIDs.Customer2);
      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _orderWithoutOrderItem = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);
    }

    [Test]
    public void ChangeEvents ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] {
        _oldCustomer, _newCustomer, _order1};

      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] {
        _oldCustomer.Orders, _newCustomer.Orders};

      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (domainObjectEventSources, collectionEventSources);

      _newCustomer.Orders.Add (_order1);

      ChangeState[] expectedChangeStates = new ChangeState[]
    {
      new RelationChangeState (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", _oldCustomer, _newCustomer, "1. Changing event of order from old to new customer"),
      new CollectionChangeState (_newCustomer.Orders, _order1, "2. Adding event of new customer's order collection"),
      new RelationChangeState (_newCustomer, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders", null, _order1, "3. Changing event of new customer"),
      new CollectionChangeState (_oldCustomer.Orders, _order1, "4. Removing of orders of old customer"),
      new RelationChangeState (_oldCustomer, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders", _order1, null, "5. Changing event of old customer"),
      new RelationChangeState (_oldCustomer, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders", null, null, "6. Changed event of old customer"),
      new CollectionChangeState (_oldCustomer.Orders, _order1, "7. Removed event of old customer's order collection"),
      new RelationChangeState (_newCustomer, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders", null, null, "8. Changed event of new customer"),
      new CollectionChangeState (_newCustomer.Orders, _order1, "9. Added event of new customer's order collection"),
      new RelationChangeState (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", null, null, "10. Changed event of order from old to new customer"),
    };

      eventReceiver.Check (expectedChangeStates);

      Assert.AreEqual (StateType.Changed, _order1.State);
      Assert.AreEqual (StateType.Changed, _oldCustomer.State);
      Assert.AreEqual (StateType.Changed, _newCustomer.State);

      Assert.AreSame (_newCustomer, _order1.Customer);
      Assert.IsNull (_oldCustomer.Orders[_order1.ID]);
      Assert.AreSame (_order1, _newCustomer.Orders[_order1.ID]);

      Assert.AreEqual (StateType.Changed, _order1.InternalDataContainer.State);
      Assert.AreEqual (StateType.Unchanged, _oldCustomer.InternalDataContainer.State);
      Assert.AreEqual (StateType.Unchanged, _newCustomer.InternalDataContainer.State);
    }

    [Test]
    public void OrderCancelsChangeEvent ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] {_oldCustomer, _newCustomer, _order1};
      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] {_oldCustomer.Orders, _newCustomer.Orders};
      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (domainObjectEventSources, collectionEventSources, 1);

      try
      {
        _newCustomer.Orders.Add (_order1);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedChangeStates = new ChangeState[]
            { new RelationChangeState (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", _oldCustomer, _newCustomer, "1. Changing event of order from old to new customer") };

        eventReceiver.Check (expectedChangeStates);

        Assert.AreEqual (StateType.Unchanged, _order1.State);
        Assert.AreEqual (StateType.Unchanged, _oldCustomer.State);
        Assert.AreEqual (StateType.Unchanged, _newCustomer.State);

        Assert.AreSame (_oldCustomer, _order1.Customer);
        Assert.AreSame (_order1, _oldCustomer.Orders[_order1.ID]);
        Assert.IsNull (_newCustomer.Orders[_order1.ID]);
      }
    }

    [Test]
    public void NewCustomerCollectionCancelsChangeEvent ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] {
        _oldCustomer, _newCustomer, _order1};

      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] {
        _oldCustomer.Orders, _newCustomer.Orders};

      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (domainObjectEventSources, collectionEventSources, 2);

      try
      {
        _newCustomer.Orders.Add (_order1);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedChangeStates = new ChangeState[]
            {
              new RelationChangeState (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", _oldCustomer, _newCustomer, "1. Changing event of order from old to new customer"),
              new CollectionChangeState (_newCustomer.Orders, _order1, "2. Adding event of new customer's order collection")
            };

        eventReceiver.Check (expectedChangeStates);

        Assert.AreEqual (StateType.Unchanged, _order1.State);
        Assert.AreEqual (StateType.Unchanged, _oldCustomer.State);
        Assert.AreEqual (StateType.Unchanged, _newCustomer.State);

        Assert.AreSame (_oldCustomer, _order1.Customer);
        Assert.AreSame (_order1, _oldCustomer.Orders[_order1.ID]);
        Assert.IsNull (_newCustomer.Orders[_order1.ID]);
      }
    }

    [Test]
    public void NewCustomerCancelsChangeEvent ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] {
        _oldCustomer, _newCustomer, _order1};

      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] {
        _oldCustomer.Orders, _newCustomer.Orders};

      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (domainObjectEventSources, collectionEventSources, 3);

      try
      {
        _newCustomer.Orders.Add (_order1);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedChangeStates = new ChangeState[]
            {
              new RelationChangeState (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", _oldCustomer, _newCustomer, "1. Changing event of order from old to new customer"),
              new CollectionChangeState (_newCustomer.Orders, _order1, "2. Adding event of new customer's order collection"),
              new RelationChangeState (_newCustomer, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders", null, _order1, "3. Changing event of new customer")
            };

        eventReceiver.Check (expectedChangeStates);

        Assert.AreEqual (StateType.Unchanged, _order1.State);
        Assert.AreEqual (StateType.Unchanged, _oldCustomer.State);
        Assert.AreEqual (StateType.Unchanged, _newCustomer.State);

        Assert.AreSame (_oldCustomer, _order1.Customer);
        Assert.AreSame (_order1, _oldCustomer.Orders[_order1.ID]);
        Assert.IsNull (_newCustomer.Orders[_order1.ID]);
      }
    }

    [Test]
    public void OldCustomerCollectionCancelsChangeEvent ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] {
        _oldCustomer, _newCustomer, _order1};

      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] {
        _oldCustomer.Orders, _newCustomer.Orders};

      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (domainObjectEventSources, collectionEventSources, 4);

      try
      {
        _newCustomer.Orders.Add (_order1);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedChangeStates = new ChangeState[]
            {
              new RelationChangeState (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", _oldCustomer, _newCustomer, "1. Changing event of order from old to new customer"),
              new CollectionChangeState (_newCustomer.Orders, _order1, "2. Adding event of new customer's order collection"),
              new RelationChangeState (_newCustomer, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders", null, _order1, "3. Changing event of new customer"),
              new CollectionChangeState (_oldCustomer.Orders, _order1, "4. Removing of orders of old customer")
            };

        eventReceiver.Check (expectedChangeStates);

        Assert.AreEqual (StateType.Unchanged, _order1.State);
        Assert.AreEqual (StateType.Unchanged, _oldCustomer.State);
        Assert.AreEqual (StateType.Unchanged, _newCustomer.State);

        Assert.AreSame (_oldCustomer, _order1.Customer);
        Assert.AreSame (_order1, _oldCustomer.Orders[_order1.ID]);
        Assert.IsNull (_newCustomer.Orders[_order1.ID]);
      }
    }

    [Test]
    public void OldCustomerCancelsChangeEvent ()
    {
      DomainObject[] domainObjectEventSources = new DomainObject[] {
        _oldCustomer, _newCustomer, _order1};

      DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] {
        _oldCustomer.Orders, _newCustomer.Orders};

      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (domainObjectEventSources, collectionEventSources, 5);

      try
      {
        _newCustomer.Orders.Add (_order1);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedChangeStates = new ChangeState[]
            {
              new RelationChangeState (_order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", _oldCustomer, _newCustomer, "1. Changing event of order from old to new customer"),
              new CollectionChangeState (_newCustomer.Orders, _order1, "2. Adding event of new customer's order collection"),
              new RelationChangeState (_newCustomer, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders", null, _order1, "3. Changing event of new customer"),
              new CollectionChangeState (_oldCustomer.Orders, _order1, "4. Removing of orders of old customer"),
              new RelationChangeState (_oldCustomer, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders", _order1, null, "5. Changing event of old customer")
            };

        eventReceiver.Check (expectedChangeStates);

        Assert.AreEqual (StateType.Unchanged, _order1.State);
        Assert.AreEqual (StateType.Unchanged, _oldCustomer.State);
        Assert.AreEqual (StateType.Unchanged, _newCustomer.State);

        Assert.AreSame (_oldCustomer, _order1.Customer);
        Assert.AreSame (_order1, _oldCustomer.Orders[_order1.ID]);
        Assert.IsNull (_newCustomer.Orders[_order1.ID]);
      }
    }

    [Test]
    public void StateTracking ()
    {
      _newCustomer.Orders.Add (_order1);

      Assert.AreEqual (StateType.Changed, _order1.State);
      Assert.AreEqual (StateType.Changed, _oldCustomer.State);
      Assert.AreEqual (StateType.Changed, _newCustomer.State);
    }

    [Test]
    public void ChangeWithInheritance ()
    {
      IndustrialSector industrialSector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);
      Partner partner = Partner.GetObject (DomainObjectIDs.Partner2);

      Assert.IsNull (industrialSector.Companies[partner.ID]);
      Assert.IsFalse (ReferenceEquals (industrialSector, partner.IndustrialSector));

      industrialSector.Companies.Add (partner);

      Assert.IsNotNull (industrialSector.Companies[partner.ID]);
      Assert.AreSame (industrialSector, partner.IndustrialSector);
    }

    [Test]
    public void SetNewCustomerThroughOrder ()
    {
      _order1.Customer = _newCustomer;

      Assert.AreEqual (StateType.Changed, _order1.State);
      Assert.AreEqual (StateType.Changed, _oldCustomer.State);
      Assert.AreEqual (StateType.Changed, _newCustomer.State);

      Assert.AreSame (_newCustomer, _order1.Customer);
      Assert.IsNull (_oldCustomer.Orders[_order1.ID]);
      Assert.AreSame (_order1, _newCustomer.Orders[_order1.ID]);

      Assert.AreEqual (StateType.Changed, _order1.InternalDataContainer.State);
      Assert.AreEqual (StateType.Unchanged, _newCustomer.InternalDataContainer.State);
      Assert.AreEqual (StateType.Unchanged, _oldCustomer.InternalDataContainer.State);
    }

    [Test]
    public void ChangeRelationBackToOriginalValue ()
    {
      _order1.Customer = _newCustomer;
      Assert.AreEqual (StateType.Changed, _order1.State);
      Assert.AreEqual (StateType.Changed, _oldCustomer.State);
      Assert.AreEqual (StateType.Changed, _newCustomer.State);

      _order1.Customer = _oldCustomer;
      Assert.AreEqual (StateType.Unchanged, _order1.State);
      Assert.AreEqual (StateType.Unchanged, _oldCustomer.State);
      Assert.AreEqual (StateType.Unchanged, _newCustomer.State);
    }

    [Test]
    public void SetOriginalValue ()
    {
      _order1.Customer = _order1.Customer;
      Assert.AreEqual (StateType.Unchanged, _order1.State);
      Assert.AreEqual (StateType.Unchanged, _order1.Customer.State);

      Assert.AreEqual (StateType.Unchanged, _order1.InternalDataContainer.State);
      Assert.AreEqual (StateType.Unchanged, _order1.Customer.InternalDataContainer.State);
    }

    [Test]
    public void HasBeenTouched_FromOneProperty ()
    {
      CheckTouching (delegate { _order1.Customer = _newCustomer; }, _order1, "Customer", 
        RelationEndPointID.Create(_order1.ID, typeof (Order).FullName + ".Customer"),
        RelationEndPointID.Create(_newCustomer.ID, typeof (Customer).FullName + ".Orders"),
        RelationEndPointID.Create(_oldCustomer.ID, typeof (Customer).FullName + ".Orders"));
    }

    [Test]
    public void HasBeenTouched_FromManyPropertyAdd ()
    {
      CheckTouching (delegate { _newCustomer.Orders.Add (_order1); }, _order1, "Customer",
          RelationEndPointID.Create(_order1.ID, typeof (Order).FullName + ".Customer"),
          RelationEndPointID.Create(_newCustomer.ID, typeof (Customer).FullName + ".Orders"),
          RelationEndPointID.Create(_oldCustomer.ID, typeof (Customer).FullName + ".Orders"));
    }

    [Test]
    public void HasBeenTouched_FromManyPropertyRemove ()
    {
      CheckTouching (delegate { _oldCustomer.Orders.Remove (_order1); }, _order1, "Customer",
          RelationEndPointID.Create(_order1.ID, typeof (Order).FullName + ".Customer"),
          RelationEndPointID.Create(_oldCustomer.ID, typeof (Customer).FullName + ".Orders"));
    }

    [Test]
    public void HasBeenTouched_FromManyPropertyReplaceWithNull ()
    {
      CheckTouching (delegate { _oldCustomer.Orders[_oldCustomer.Orders.IndexOf (_order1)] = null; }, _order1, "Customer",
          RelationEndPointID.Create(_order1.ID, typeof (Order).FullName + ".Customer"),
          RelationEndPointID.Create(_oldCustomer.ID, typeof (Customer).FullName + ".Orders"));
    }

    [Test]
    public void HasBeenTouched_FromManyPropertyReplaceWithNew ()
    {
      Order newOrder = Order.NewObject ();
      
      Assert.IsFalse (newOrder.InternalDataContainer.HasValueBeenTouched (GetPropertyDefinition (typeof (Order), "Customer")), "newOrder ObjectID touched");

      CheckTouching (delegate { _oldCustomer.Orders[_oldCustomer.Orders.IndexOf (_order1)] = newOrder; }, _order1, "Customer",
        RelationEndPointID.Create(_order1.ID, typeof (Order).FullName + ".Customer"),
        RelationEndPointID.Create(newOrder.ID, typeof (Order).FullName + ".Customer"),
        RelationEndPointID.Create(_oldCustomer.ID, typeof (Customer).FullName + ".Orders"));

      Assert.IsTrue (newOrder.InternalDataContainer.HasValueBeenTouched (GetPropertyDefinition (typeof (Order), "Customer")), "newOrder ObjectID touched");
    }

    [Test]
    public void HasBeenTouched_FromOneProperty_OriginalValue ()
    {
      CheckTouching (delegate { _order1.Customer = _order1.Customer; }, _order1, "Customer",
          RelationEndPointID.Create(_order1.ID, typeof (Order).FullName + ".Customer"),
          RelationEndPointID.Create(_oldCustomer.ID, typeof (Customer).FullName + ".Orders"));
    }

    [Test]
    public void HasBeenTouched_FromManyPropertyReplace_OriginalValue ()
    {
      CheckTouching (delegate { _oldCustomer.Orders[_oldCustomer.Orders.IndexOf (_order1)] = _order1; }, _order1, "Customer",
          RelationEndPointID.Create(_order1.ID, typeof (Order).FullName + ".Customer"),
          RelationEndPointID.Create(_oldCustomer.ID, typeof (Customer).FullName + ".Orders"),
          RelationEndPointID.Create(_oldCustomer.ID, typeof (Customer).FullName + ".Orders"));
    }
    
    [Test]
    public void GetOriginalRelatedObject ()
    {
      _order1.Customer = _newCustomer;

      Assert.AreSame (_newCustomer, _order1.Customer);
      Assert.AreSame (_oldCustomer, _order1.GetOriginalRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"));
    }

    [Test]
    public void GetOriginalRelatedObjects ()
    {
      Assert.IsNull (_newCustomer.Orders[_order1.ID]);

      _newCustomer.Orders.Add (_order1);

      DomainObjectCollection oldOrders = _newCustomer.GetOriginalRelatedObjects ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");
      Assert.AreSame (_order1, _newCustomer.Orders[_order1.ID]);
      Assert.IsNull (oldOrders[_order1.ID]);
    }

    [Test]
    public void GetOriginalRelatedObjectsWithLazyLoad ()
    {
      Employee supervisor = Employee.GetObject (DomainObjectIDs.Employee1);
      DomainObjectCollection subordinates = supervisor.GetOriginalRelatedObjects ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Subordinates");

      Assert.AreEqual (2, subordinates.Count);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void CheckRequiredItemTypeForExisting ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      DomainObjectCollection orderItems = order.OrderItems;

      orderItems.Add (Customer.NewObject ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void CheckRequiredItemTypeForNew ()
    {
      Order order = Order.NewObject ();
      DomainObjectCollection orderItems = order.OrderItems;

      orderItems.Add (Customer.NewObject ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void SetRelatedObjectWithInvalidObjectClass ()
    {
      _order1.SetRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", Company.GetObject (DomainObjectIDs.Company1));  
    }

    [Test]
    public void Clear_Events ()
    {
      Assert.That (_oldCustomer.Orders, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem }));

      var eventReceiver = new SequenceEventReceiver (
          new DomainObject[] { _oldCustomer, _order1, _orderWithoutOrderItem }, 
          new[] { _oldCustomer.Orders });

      _oldCustomer.Orders.Clear ();

      var expectedStates = new ChangeState[]
      {
        new RelationChangeState (_orderWithoutOrderItem, typeof (Order).FullName + ".Customer", _oldCustomer, null, "1. Setting _orderWithoutOrderItem.Customer to null"),
        new CollectionChangeState (_oldCustomer.Orders, _orderWithoutOrderItem, "2. Removing _orderWithoutOrderItem from _oldCustomer.Orders"),
        new RelationChangeState  (_oldCustomer, typeof (Customer).FullName + ".Orders", _orderWithoutOrderItem, null, "3. Removing _orderWithoutOrderItem from _oldCustomer"),

        new RelationChangeState (_order1, typeof (Order).FullName + ".Customer", _oldCustomer, null, "4. Setting _order1.Customer to null"),
        new CollectionChangeState (_oldCustomer.Orders, _order1, "5. Removing _order1 from _oldCustomer.Orders"),
        new RelationChangeState  (_oldCustomer, typeof (Customer).FullName + ".Orders", _order1, null, "6. Removing _order1 from _oldCustomer"),

        new RelationChangeState  (_oldCustomer, typeof (Customer).FullName + ".Orders", null, null, "7. Removed _order1 from _oldCustomer"),
        new CollectionChangeState (_oldCustomer.Orders, _order1, "8. Removed _order1 from _oldCustomer.Orders"),
        new RelationChangeState (_order1, typeof (Order).FullName + ".Customer", null, null, "9. Setting _order1.Customer to null"),

        new RelationChangeState  (_oldCustomer, typeof (Customer).FullName + ".Orders", null, null, "10. Removed _orderWithoutOrderItem from _oldCustomer"),
        new CollectionChangeState (_oldCustomer.Orders, _orderWithoutOrderItem, "11. Removed _orderWithoutOrderItem from _oldCustomer.Orders"),
        new RelationChangeState (_orderWithoutOrderItem, typeof (Order).FullName + ".Customer", null, null, "12. Set _orderWithoutOrderItem.Customer to null"),
      };

      eventReceiver.Check (expectedStates);

      Assert.That (_oldCustomer.Orders, Is.Empty);
      Assert.That (_orderWithoutOrderItem.Customer, Is.Null);
      Assert.That (_order1.Customer, Is.Null);
      Assert.That (DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (_oldCustomer.Orders).HasBeenTouched, Is.True);
    }

    [Test]
    public void Clear_CancelAtSecondObject ()
    {
      Assert.That (_oldCustomer.Orders, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem }));

      var eventReceiver = new SequenceEventReceiver (
          new DomainObject[] { _oldCustomer, _order1, _orderWithoutOrderItem },
          new[] { _oldCustomer.Orders });

      eventReceiver.CancelEventNumber = 6;

      try
      {
        _oldCustomer.Orders.Clear ();
        Assert.Fail ("Expected cancellation");
      }
      catch (EventReceiverCancelException)
      {
        // ok
      }

      var expectedStates = new ChangeState[]
      {
        new RelationChangeState (_orderWithoutOrderItem, typeof (Order).FullName + ".Customer", _oldCustomer, null, "1. Setting _orderWithoutOrderItem.Customer to null"),
        new CollectionChangeState (_oldCustomer.Orders, _orderWithoutOrderItem, "2. Removing _orderWithoutOrderItem from _oldCustomer.Orders"),
        new RelationChangeState  (_oldCustomer, typeof (Customer).FullName + ".Orders", _orderWithoutOrderItem, null, "3. Removing _orderWithoutOrderItem from _oldCustomer"),

        new RelationChangeState (_order1, typeof (Order).FullName + ".Customer", _oldCustomer, null, "4. Setting _order1.Customer to null"),
        new CollectionChangeState (_oldCustomer.Orders, _order1, "5. Removing _order1 from _oldCustomer.Orders"),
        new RelationChangeState  (_oldCustomer, typeof (Customer).FullName + ".Orders", _order1, null, "6. Removing _order1 from _oldCustomer"),
      };

      eventReceiver.Check (expectedStates);

      Assert.That (_oldCustomer.Orders, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem }));
      Assert.That (_orderWithoutOrderItem.Customer, Is.SameAs (_oldCustomer));
      Assert.That (_order1.Customer, Is.SameAs (_oldCustomer));
      Assert.That (DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint (_oldCustomer.Orders).HasBeenTouched, Is.False);
    }
  }
}
