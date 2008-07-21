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
using Remotion.Data.DomainObjects.UnitTests.DomainObjects.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.IntegrationTests
{
  [TestFixture]
  public class RelationsTest : ClientTransactionBaseTest
  {
    [Test]
    public void OneToOneRelationChangeTest ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderTicket orderTicket = order.OrderTicket;

      DomainObjectRelationCheckEventReceiver orderEventReceiver = new DomainObjectRelationCheckEventReceiver (order);
      DomainObjectRelationCheckEventReceiver orderTicketEventReceiver = new DomainObjectRelationCheckEventReceiver (orderTicket);

      orderTicket.Order = null;

      Assert.IsTrue (orderEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsTrue (orderTicketEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.AreSame (orderTicket, orderEventReceiver.GetChangingRelatedDomainObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));
      Assert.AreSame (order, orderTicketEventReceiver.GetChangingRelatedDomainObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"));

      Assert.IsTrue (orderEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.IsTrue (orderTicketEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreSame (null, orderEventReceiver.GetChangedRelatedDomainObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));
      Assert.AreSame (null, orderTicketEventReceiver.GetChangedRelatedDomainObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"));
    }

  }
}
