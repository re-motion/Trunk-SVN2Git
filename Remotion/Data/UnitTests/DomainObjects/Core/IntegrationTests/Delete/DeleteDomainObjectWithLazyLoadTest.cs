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

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Delete
{
  [TestFixture]
  public class DeleteDomainObjectWithLazyLoadTest : ClientTransactionBaseTest
  {
    [Test]
    public void DomainObjectWithOneToOneRelationAndNonVirtualProperty ()
    {
      OrderTicket orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      orderTicket.Delete ();

      Order order = Order.GetObject (DomainObjectIDs.Order1);

      Assert.That (orderTicket.Order, Is.Null);
      Assert.That (order.OrderTicket, Is.Null);
      Assert.That (orderTicket.Properties[typeof (OrderTicket), "Order"].GetRelatedObjectID(), Is.Null);
      Assert.That (order.State, Is.EqualTo (StateType.Changed));
      Assert.That (order.InternalDataContainer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void DomainObjectWithOneToOneRelationAndNonVirtualNullProperty ()
    {
      Computer computerWithoutEmployee = Computer.GetObject (DomainObjectIDs.Computer4);
      computerWithoutEmployee.Delete ();

      Assert.That (computerWithoutEmployee.Employee, Is.Null);
      Assert.That (computerWithoutEmployee.Properties[typeof (Computer), "Employee"].GetRelatedObjectID (), Is.Null);
    }

    [Test]
    public void DomainObjectWithOneToOneRelationAndVirtualProperty ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.Delete ();

      OrderTicket orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);

      Assert.That (orderTicket.Order, Is.Null);
      Assert.That (order.OrderTicket, Is.Null);
      Assert.That (orderTicket.Properties[typeof (OrderTicket), "Order"].GetRelatedObjectID (), Is.Null);
      Assert.That (orderTicket.InternalDataContainer.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void DomainObjectWithOneToOneRelationAndVirtualNullProperty ()
    {
      Employee employeeWithoutComputer = Employee.GetObject (DomainObjectIDs.Employee1);
      employeeWithoutComputer.Delete ();

      Assert.That (employeeWithoutComputer.Computer, Is.Null);
    }

    [Test]
    public void DomainObjectWithOneToManyRelation ()
    {
      Employee supervisor = Employee.GetObject (DomainObjectIDs.Employee1);
      supervisor.Delete ();

      Employee subordinate1 = Employee.GetObject (DomainObjectIDs.Employee4);
      Employee subordinate2 = Employee.GetObject (DomainObjectIDs.Employee5);

      Assert.That (supervisor.Subordinates.Count, Is.EqualTo (0));
      Assert.That (subordinate1.Supervisor, Is.Null);
      Assert.That (subordinate2.Supervisor, Is.Null);
      Assert.That (subordinate1.Properties[typeof (Employee), "Supervisor"].GetRelatedObjectID (), Is.Null);
      Assert.That (subordinate2.Properties[typeof (Employee), "Supervisor"].GetRelatedObjectID (), Is.Null);
      Assert.That (subordinate1.InternalDataContainer.State, Is.EqualTo (StateType.Changed));
      Assert.That (subordinate2.InternalDataContainer.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void DomainObjectWithEmptyOneToManyRelation ()
    {
      Employee supervisor = Employee.GetObject (DomainObjectIDs.Employee3);
      supervisor.Delete ();

      Assert.That (supervisor.Subordinates.Count, Is.EqualTo (0));
    }

    [Test]
    public void DomainObjectWithManyToOneRelation ()
    {
      OrderItem orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      orderItem.Delete ();

      Order order = Order.GetObject (DomainObjectIDs.Order1);

      Assert.That (orderItem.Order, Is.Null);
      Assert.That (order.OrderItems.Count, Is.EqualTo (1));
      Assert.That (order.OrderItems.Contains (orderItem.ID), Is.False);
      Assert.That (orderItem.Properties[typeof (OrderItem), "Order"].GetRelatedObjectID (), Is.Null);
      Assert.That (order.State, Is.EqualTo (StateType.Changed));
      Assert.That (order.InternalDataContainer.State, Is.EqualTo (StateType.Unchanged));
    }
  }
}
