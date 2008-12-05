// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
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

      Assert.IsNull (orderTicket.Order);
      Assert.IsNull (order.OrderTicket);
			Assert.IsNull (orderTicket.InternalDataContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order"]);
      Assert.AreEqual (StateType.Changed, order.State);
			Assert.AreEqual (StateType.Unchanged, order.InternalDataContainer.State);
    }

    [Test]
    public void DomainObjectWithOneToOneRelationAndNonVirtualNullProperty ()
    {
      Computer computerWithoutEmployee = Computer.GetObject (DomainObjectIDs.Computer4);
      computerWithoutEmployee.Delete ();

      Assert.IsNull (computerWithoutEmployee.Employee);
			Assert.IsNull (computerWithoutEmployee.InternalDataContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.Employee"]);
    }

    [Test]
    public void DomainObjectWithOneToOneRelationAndVirtualProperty ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.Delete ();

      OrderTicket orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);

      Assert.IsNull (orderTicket.Order);
      Assert.IsNull (order.OrderTicket);
			Assert.IsNull (orderTicket.InternalDataContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order"]);
			Assert.AreEqual (StateType.Changed, orderTicket.InternalDataContainer.State);
    }

    [Test]
    public void DomainObjectWithOneToOneRelationAndVirtualNullProperty ()
    {
      Employee employeeWithoutComputer = Employee.GetObject (DomainObjectIDs.Employee1);
      employeeWithoutComputer.Delete ();

      Assert.IsNull (employeeWithoutComputer.Computer);
    }

    [Test]
    public void DomainObjectWithOneToManyRelation ()
    {
      Employee supervisor = Employee.GetObject (DomainObjectIDs.Employee1);
      supervisor.Delete ();

      Employee subordinate1 = Employee.GetObject (DomainObjectIDs.Employee4);
      Employee subordinate2 = Employee.GetObject (DomainObjectIDs.Employee5);

      Assert.AreEqual (0, supervisor.Subordinates.Count);
      Assert.IsNull (subordinate1.Supervisor);
      Assert.IsNull (subordinate2.Supervisor);
			Assert.IsNull (subordinate1.InternalDataContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Supervisor"]);
			Assert.IsNull (subordinate2.InternalDataContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Supervisor"]);
			Assert.AreEqual (StateType.Changed, subordinate1.InternalDataContainer.State);
			Assert.AreEqual (StateType.Changed, subordinate2.InternalDataContainer.State);
    }

    [Test]
    public void DomainObjectWithEmptyOneToManyRelation ()
    {
      Employee supervisor = Employee.GetObject (DomainObjectIDs.Employee3);
      supervisor.Delete ();

      Assert.AreEqual (0, supervisor.Subordinates.Count);
    }

    [Test]
    public void DomainObjectWithManyToOneRelation ()
    {
      OrderItem orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      orderItem.Delete ();

      Order order = Order.GetObject (DomainObjectIDs.Order1);

      Assert.IsNull (orderItem.Order);
      Assert.AreEqual (1, order.OrderItems.Count);
      Assert.IsFalse (order.OrderItems.Contains (orderItem.ID));
			Assert.IsNull (orderItem.InternalDataContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order"]);
      Assert.AreEqual (StateType.Changed, order.State);
			Assert.AreEqual (StateType.Unchanged, order.InternalDataContainer.State);
    }
  }
}
