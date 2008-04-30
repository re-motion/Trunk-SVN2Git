using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.Core.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.IntegrationTests
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
