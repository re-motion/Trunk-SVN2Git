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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction
{
  public abstract class ClientTransactionStateTransitionBaseTest : ClientTransactionBaseTest
  {
    public Order GetInvalid ()
    {
      Order invalid = Order.NewObject ();
      invalid.Delete ();
      Assert.That (invalid.State, Is.EqualTo (StateType.Invalid));
      return invalid;
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
      Order invalid = GetInvalid();

      Assert.That (unchanged.State, Is.EqualTo (StateType.Unchanged));

      Assert.That (changedThroughPropertyValue.State, Is.EqualTo (StateType.Changed));
      Assert.That (changedThroughPropertyValue.Properties[typeof (Order) + ".OrderNumber"].GetOriginalValue<int>(), Is.Not.EqualTo (changedThroughPropertyValue.OrderNumber));

      Assert.That (changedThroughRelatedObjects.State, Is.EqualTo (StateType.Changed));
      Assert.That (changedThroughRelatedObjects.Properties[typeof (Order) + ".OrderItems"].GetOriginalValue<ObjectList<OrderItem>> ().Count, Is.Not.EqualTo (changedThroughRelatedObjects.OrderItems.Count));

      Assert.That (changedThroughRelatedObjectRealSide.State, Is.EqualTo (StateType.Changed));
      Assert.That (changedThroughRelatedObjectRealSide.Properties[typeof (Computer) + ".Employee"].GetOriginalValue<Employee> (), Is.Not.EqualTo (changedThroughRelatedObjectRealSide.Employee));

      Assert.That (changedThroughRelatedObjectVirtualSide.State, Is.EqualTo (StateType.Changed));
      Assert.That (changedThroughRelatedObjectVirtualSide.Properties[typeof (Employee) + ".Computer"].GetOriginalValue<Computer> (), Is.Not.EqualTo (changedThroughRelatedObjectVirtualSide.Computer));

      Assert.That (newUnchanged.State, Is.EqualTo (StateType.New));
      Assert.That (newChanged.State, Is.EqualTo (StateType.New));

      Assert.That (deleted.State, Is.EqualTo (StateType.Deleted));

      Assert.That (unidirectionalWithDeleted.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (unidirectionalWithDeletedNew.State, Is.EqualTo (StateType.Changed));

      Assert.That (invalid.IsInvalid, Is.True);
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
