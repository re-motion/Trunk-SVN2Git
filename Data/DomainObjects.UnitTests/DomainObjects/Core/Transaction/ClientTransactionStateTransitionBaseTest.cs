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
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transaction
{
  public abstract class ClientTransactionStateTransitionBaseTest : ClientTransactionBaseTest
  {
    public Order GetDiscarded ()
    {
      Order discarded = Order.NewObject ();
      discarded.Delete ();
      return discarded;
    }

    public Location GetUnidirectionalWithDeletedNew ()
    {
      Location unidirectionalWithDeletedNew = Location.GetObject (DomainObjectIDs.Location3);
      unidirectionalWithDeletedNew.Client = Client.NewObject();
      unidirectionalWithDeletedNew.Client.Delete ();
      return unidirectionalWithDeletedNew;
    }

    public Location GetUnidirectionalWithDeleted ()
    {
      Location unidirectionalWithDeleted = Location.GetObject (DomainObjectIDs.Location1);
      unidirectionalWithDeleted.Client.Delete ();
      return unidirectionalWithDeleted;
    }

    public Order GetDeleted ()
    {
      Order deleted = Order.GetObject (DomainObjectIDs.Order4);
      FullyDeleteOrder (deleted);
      return deleted;
    }

    public ClassWithAllDataTypes GetNewChanged ()
    {
      ClassWithAllDataTypes newChanged = ClassWithAllDataTypes.NewObject ();
      newChanged.Int32Property = 13;
      return newChanged;
    }

    public ClassWithAllDataTypes GetNewUnchanged ()
    {
      return ClassWithAllDataTypes.NewObject ();
    }

    public Employee GetChangedThroughRelatedObjectVirtualSide ()
    {
      Employee changedThroughRelatedObjectVirtualSide = Employee.GetObject (DomainObjectIDs.Employee3);
      changedThroughRelatedObjectVirtualSide.Computer = Computer.GetObject (DomainObjectIDs.Computer3);
      return changedThroughRelatedObjectVirtualSide;
    }

    public Computer GetChangedThroughRelatedObjectRealSide ()
    {
      Computer changedThroughRelatedObjectRealSide = Computer.GetObject (DomainObjectIDs.Computer1);
      changedThroughRelatedObjectRealSide.Employee = Employee.GetObject (DomainObjectIDs.Employee1);
      return changedThroughRelatedObjectRealSide;
    }

    public Order GetChangedThroughRelatedObjects ()
    {
      Order changedThroughRelatedObjects = Order.GetObject (DomainObjectIDs.Order3);
      changedThroughRelatedObjects.OrderItems.Clear ();
      return changedThroughRelatedObjects;
    }

    public Order GetChangedThroughPropertyValue ()
    {
      Order changedThroughPropertyValue = Order.GetObject (DomainObjectIDs.Order2);
      changedThroughPropertyValue.OrderNumber = 74;
      return changedThroughPropertyValue;
    }

    public Order GetUnchanged ()
    {
      return Order.GetObject (DomainObjectIDs.Order1);
    }

    [Test]
    public void CheckInitialStates ()
    {
      Order unchanged = GetUnchanged();
      Order changedThroughPropertyValue = GetChangedThroughPropertyValue();
      Order changedThroughRelatedObjects = GetChangedThroughRelatedObjects();
      Computer changedThroughRelatedObjectRealSide = GetChangedThroughRelatedObjectRealSide();
      Employee changedThroughRelatedObjectVirtualSide = GetChangedThroughRelatedObjectVirtualSide();
      ClassWithAllDataTypes newUnchanged = GetNewUnchanged();
      ClassWithAllDataTypes newChanged = GetNewChanged ();
      Order deleted = GetDeleted();
      Location unidirectionalWithDeleted = GetUnidirectionalWithDeleted ();
      Location unidirectionalWithDeletedNew = GetUnidirectionalWithDeletedNew ();
      Order discarded = GetDiscarded();

      Assert.AreEqual (StateType.Unchanged, unchanged.State);

      Assert.AreEqual (StateType.Changed, changedThroughPropertyValue.State);
      Assert.AreNotEqual (changedThroughPropertyValue.OrderNumber,
          changedThroughPropertyValue.Properties[typeof (Order) + ".OrderNumber"].GetOriginalValue<int>());

      Assert.AreEqual (StateType.Changed, changedThroughRelatedObjects.State);
      Assert.AreNotEqual (changedThroughRelatedObjects.OrderItems.Count,
          changedThroughRelatedObjects.Properties[typeof (Order) + ".OrderItems"].GetOriginalValue<ObjectList<OrderItem>> ().Count);

      Assert.AreEqual (StateType.Changed, changedThroughRelatedObjectRealSide.State);
      Assert.AreNotEqual (changedThroughRelatedObjectRealSide.Employee,
          changedThroughRelatedObjectRealSide.Properties[typeof (Computer) + ".Employee"].GetOriginalValue<Employee> ());

      Assert.AreEqual (StateType.Changed, changedThroughRelatedObjectVirtualSide.State);
      Assert.AreNotEqual (changedThroughRelatedObjectVirtualSide.Computer,
          changedThroughRelatedObjectVirtualSide.Properties[typeof (Employee) + ".Computer"].GetOriginalValue<Computer> ());

      Assert.AreEqual (StateType.New, newUnchanged.State);
      Assert.AreEqual (StateType.New, newChanged.State);

      Assert.AreEqual (StateType.Deleted, deleted.State);

      Assert.AreEqual (StateType.Unchanged, unidirectionalWithDeleted.State);
      Assert.AreEqual (StateType.Changed, unidirectionalWithDeletedNew.State);

      Assert.IsTrue (discarded.IsDiscarded);
    }

    protected void FullyDeleteOrder (Order order)
    {
      for (int i = order.OrderItems.Count - 1; i >= 0; --i)
        order.OrderItems[i].Delete ();
      order.OrderTicket.Delete ();
      order.Delete ();
    }
  }
}
