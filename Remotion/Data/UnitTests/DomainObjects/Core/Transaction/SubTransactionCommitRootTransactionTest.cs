// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transaction
{
  [TestFixture]
  public class SubTransactionCommitRootTransactionTest : ClientTransactionBaseTest
  {
    private Order CreateNewOrder ()
    {
      var order = Order.NewObject();
      order.Official = Official.GetObject (DomainObjectIDs.Official1);
      order.Customer = Customer.GetObject (DomainObjectIDs.Customer1);
      order.OrderTicket = OrderTicket.NewObject (order);
      order.DeliveryDate = DateTime.Now;

      return order;
    }

    [Test]
    public void ChangeParent ()
    {
      var order1 = CreateNewOrder();

      var orderItem1 = OrderItem.NewObject (order1);
      var orderItem2 = OrderItem.NewObject (order1); // a 2nd orderItem is necessary

      Order order2;
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        orderItem2.Delete(); // this delete is necessary
        order2 = CreateNewOrder();
        order2.OrderItems.Add (orderItem1);
        order1.OrderTicket.Delete(); // must delete to delete order1 
        order1.Delete(); // this delete triggers resetting order1.OrderItems during commit
        ClientTransaction.Current.Commit();
        Assert.AreSame (order2, orderItem1.Order);
      }
      Assert.AreSame (order2, orderItem1.Order);
      PropertyIndexer propertyIndexer = new PropertyIndexer (orderItem1);
      PropertyAccessor propertyAccessor = propertyIndexer[typeof (OrderItem), "Order"];
      Assert.IsNotNull (propertyAccessor.GetRelatedObjectID());
    }
  }
}