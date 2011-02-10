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
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  public class UnloadedCollectionRelationTest : ClientTransactionBaseTest
  {
    [Test]
    public void Insert ()
    {
      var newOrderItem = OrderItem.NewObject ();
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderItemCount = order.OrderItems.Count;
      UnloadService.UnloadCollectionEndPoint (ClientTransactionMock, order.OrderItems.AssociatedEndPointID, UnloadTransactionMode.ThisTransactionOnly);
      Assert.That (order.OrderItems.IsDataComplete, Is.False);

      order.OrderItems.Insert (0, newOrderItem);

      Assert.That (order.OrderItems.IsDataComplete, Is.True);
      Assert.That (order.OrderItems.Count, Is.EqualTo(orderItemCount+1));
      Assert.That (order.OrderItems[0], Is.SameAs (newOrderItem));
    }

    [Test]
    public void Remove ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderItemCount = order.OrderItems.Count;
      UnloadService.UnloadCollectionEndPoint (ClientTransactionMock, order.OrderItems.AssociatedEndPointID, UnloadTransactionMode.ThisTransactionOnly);
      Assert.That (order.OrderItems.IsDataComplete, Is.False);

      order.OrderItems.Remove (order.OrderItems[0]);

      Assert.That (order.OrderItems.IsDataComplete, Is.True);
      Assert.That (order.OrderItems.Count, Is.EqualTo (orderItemCount - 1));
    }

    [Test]
    public void Delete ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      UnloadService.UnloadCollectionEndPoint (ClientTransactionMock, order.OrderItems.AssociatedEndPointID, UnloadTransactionMode.ThisTransactionOnly);
      Assert.That (order.OrderItems.IsDataComplete, Is.False);

      order.Delete();

      Assert.That (order.OrderItems.IsDataComplete, Is.True);
      Assert.That (order.OrderItems.Count, Is.EqualTo (0));
    }

    [Test]
    public void Replace ()
    {
      var newOrderItem = OrderItem.NewObject ();
      var order = Order.GetObject (DomainObjectIDs.Order1);
      UnloadService.UnloadCollectionEndPoint (ClientTransactionMock, order.OrderItems.AssociatedEndPointID, UnloadTransactionMode.ThisTransactionOnly);
      Assert.That (order.OrderItems.IsDataComplete, Is.False);

      order.OrderItems[0] = newOrderItem;

      Assert.That (order.OrderItems.IsDataComplete, Is.True);
      Assert.That (order.OrderItems[0], Is.SameAs(newOrderItem));
    }

    [Test]
    public void SetOppositeCollection ()
    {
      var newOrderItem = OrderItem.NewObject ();
      var order = Order.GetObject (DomainObjectIDs.Order1);
      UnloadService.UnloadCollectionEndPoint (ClientTransactionMock, order.OrderItems.AssociatedEndPointID, UnloadTransactionMode.ThisTransactionOnly);
      Assert.That (order.OrderItems.IsDataComplete, Is.False);

      order.OrderItems = new ObjectList<OrderItem> (new[] { newOrderItem });

      Assert.That (order.OrderItems.IsDataComplete, Is.True);
      Assert.That (order.OrderItems, Is.EqualTo(new[]{newOrderItem}));
    }

    [Test]
    public void CollectionToArry ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      UnloadService.UnloadCollectionEndPoint (ClientTransactionMock, order.OrderItems.AssociatedEndPointID, UnloadTransactionMode.ThisTransactionOnly);
      Assert.That (order.OrderItems.IsDataComplete, Is.False);

      order.OrderItems.ToArray();

      Assert.That (order.OrderItems.IsDataComplete, Is.True);
    }

    [Test]
    public void Properties ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      UnloadService.UnloadCollectionEndPoint (ClientTransactionMock, order.OrderItems.AssociatedEndPointID, UnloadTransactionMode.ThisTransactionOnly);
      Assert.That (order.OrderItems.IsDataComplete, Is.False);

      order.Delete();
      var originalOrderItems = order.Properties[typeof (Order), "OrderItems"].GetOriginalValue<ObjectList<OrderItem>>();

      Assert.That (order.OrderItems.IsDataComplete, Is.True);
      Assert.That (order.OrderItems.Count, Is.EqualTo(0));
      Assert.That (originalOrderItems.Count, Is.GreaterThan(0));
    }
  }
}