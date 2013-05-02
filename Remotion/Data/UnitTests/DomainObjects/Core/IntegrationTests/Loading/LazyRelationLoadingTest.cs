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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Loading
{
  [TestFixture]
  [Ignore ("TODO 2264")]
  public class LazyRelationLoadingTest : ClientTransactionBaseTest
  {
    private Order _order;

    public override void SetUp ()
    {
      base.SetUp ();

      _order = DomainObjectIDs.Order1.GetObject<Order>();
    }

    [Test]
    public void AccessingRelatedObject_ForeignKeySide_ReturnsNonloadedReference ()
    {
      Assert.That (_order.Customer.ID, Is.EqualTo (DomainObjectIDs.Customer1));
      Assert.That (_order.Customer.State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void AccessingRelatedObject_ForeignKeySide_ReturnsNonloadedReference_DataIsLoadedOnDemand ()
    {
      Assert.That (_order.Customer.State, Is.EqualTo (StateType.NotLoadedYet));

      Assert.That (_order.Customer.Name, Is.EqualTo ("Kunde 1"));

      Assert.That (_order.Customer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void AccessingRelatedObject_ForeignKeySide_NonloadedReference_CanBeUsedToRegisterEvents ()
    {
      bool propertyChanged = false;
      _order.Customer.PropertyChanged += delegate { propertyChanged = true; };

      Assert.That (_order.Customer.State, Is.EqualTo (StateType.NotLoadedYet));

      _order.Customer.EnsureDataAvailable();

      Assert.That (_order.Customer.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (propertyChanged, Is.False);

      _order.Customer.Name = "John Doe";

      Assert.That (propertyChanged, Is.True);
    }

    [Test]
    public void AccessingRelatedObject_ForeignKeySide_ExceptionOnLoading_IsTriggeredOnDemand ()
    {
      var orderWithInvalidCustomer = DomainObjectIDs.InvalidOrder.GetObject<Order>();

      Assert.That (orderWithInvalidCustomer.Customer.State, Is.EqualTo (StateType.NotLoadedYet));

      Assert.That (() => orderWithInvalidCustomer.Customer.Name, Throws.TypeOf<PersistenceException>());
    }

    [Test]
    public void AccessingRelatedObject_ForeignKeySide_ReturnsAlreadyLoadedReference_IfAlreadyLoaded ()
    {
      DomainObjectIDs.Customer1.GetObject<Customer>();

      Assert.That (_order.Customer.ID, Is.EqualTo (DomainObjectIDs.Customer1));
      Assert.That (_order.Customer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void AccessingRelatedObject_VirtualSide_ReturnsLoadedObject ()
    {
      Assert.That (_order.OrderTicket.ID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
      Assert.That (_order.OrderTicket.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void AccessingRelatedCollection_ReturnsCollectionWithIncompleteContents ()
    {
      Assert.That (_order.OrderItems.AssociatedEndPointID, Is.EqualTo (RelationEndPointID.Resolve (_order, o => o.OrderItems)));
      Assert.That (_order.OrderItems.IsDataComplete, Is.False);
    }

    [Test]
    public void AccessingRelatedCollection_ReturnsCollectionWithIncompleteContents_ContentsIsLoadedWhenNeeded ()
    {
      Assert.That (_order.OrderItems.IsDataComplete, Is.False);

      Assert.That (_order.OrderItems.Count, Is.EqualTo (2));

      Assert.That (_order.OrderItems.IsDataComplete, Is.True);
    }

    [Test]
    public void AccessingRelatedCollection_CollectionWithIncompleteContents_CanBeUsedToRegisterEvents ()
    {
      bool itemAdded = false;
      _order.OrderItems.Added += delegate { itemAdded = true; };

      Assert.That (_order.OrderItems.IsDataComplete, Is.False);

      _order.OrderItems.EnsureDataComplete();

      Assert.That (_order.OrderItems.IsDataComplete, Is.True);
      Assert.That (itemAdded, Is.False);

      _order.OrderItems.Add (OrderItem.NewObject());

      Assert.That (itemAdded, Is.True);
    }

    [Test]
    public void AccessingRelatedCollection_ExceptionOnLoading_IsTriggeredOnDemand ()
    {
      var orderWithoutOrderItems = DomainObjectIDs.OrderWithoutOrderItems.GetObject<Order>();

      Assert.That (orderWithoutOrderItems.OrderItems.IsDataComplete, Is.False);

      Assert.That (() => orderWithoutOrderItems.OrderItems.Count, Throws.TypeOf<PersistenceException>());
    }

    [Test]
    public void AccessingRelatedCollection_ReturnsAlreadyLoadedCollection_IfAlreadyLoaded ()
    {
      TestableClientTransaction.EnsureDataComplete (RelationEndPointID.Resolve (_order, o => o.OrderItems));

      Assert.That (_order.OrderItems.IsDataComplete, Is.True);
    }
  }
}