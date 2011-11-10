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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Relations
{
  [TestFixture]
  public class RelationsTest : ClientTransactionBaseTest
  {
    [Test]
    public void OneToOneRelationChangeTest ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderTicket orderTicket = order.OrderTicket;

      var orderEventReceiver = new DomainObjectRelationCheckEventReceiver (order);
      var orderTicketEventReceiver = new DomainObjectRelationCheckEventReceiver (orderTicket);

      orderTicket.Order = null;

      Assert.IsTrue (orderEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsTrue (orderTicketEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.AreSame (
          orderTicket, orderEventReceiver.GetChangingRelatedDomainObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"));
      Assert.AreSame (
          order, orderTicketEventReceiver.GetChangingRelatedDomainObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order"));

      Assert.IsTrue (orderEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.IsTrue (orderTicketEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreSame (null, orderEventReceiver.GetChangedRelatedDomainObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"));
      Assert.AreSame (
          null, orderTicketEventReceiver.GetChangedRelatedDomainObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order"));
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void Unidirectional_InvalidObject_CausesThrowOnAccess ()
    {
      Location location = Location.GetObject (DomainObjectIDs.Location1);
      location.Client = Client.NewObject ();
      location.Client.Delete ();

      Dev.Null = location.Client;
    }

    [Test]
    public void Unidirectional_InvalidObject_CanBeOverwritten ()
    {
      Location location = Location.NewObject ();
      location.Client = Client.NewObject ();
      location.Client.Delete ();
      location.Client = Client.NewObject ();
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void Unidirectional_DeletedObject_CausesThrowOnAccess ()
    {
      Location location = Location.NewObject ();
      location.Client = Client.GetObject (DomainObjectIDs.Client1);
      location.Client.Delete ();

      Dev.Null = location.Client;
    }

    [Test]
    public void Unidirectional_DeletedObject_CanBeOverwritten ()
    {
      Location location = Location.NewObject ();
      location.Client = Client.GetObject (DomainObjectIDs.Client1);
      location.Client.Delete ();
      location.Client = Client.NewObject ();
    }

  }
}