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
using Remotion.Data.DomainObjects.DomainImplementation.Cloning;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainImplementation.Cloning
{
  [TestFixture]
  public class IntegrationTest : ClientTransactionBaseTest
  {
    [Test]
    public void CompleteCloneStrategy ()
    {
      DomainObjectCloner cloner = new DomainObjectCloner ();
      ClientTransaction bindingTransaction = ClientTransaction.CreateBindingTransaction ();
      cloner.CloneTransaction = bindingTransaction;

      Order source = Order.GetObject (DomainObjectIDs.Order1);
      Order clone = cloner.CreateClone (source, new CompleteCloneStrategy());

      Assert.That (clone, Is.Not.SameAs (source));
      Assert.That (clone.OrderNumber, Is.EqualTo (source.OrderNumber));

      Assert.That (clone.OrderItems[0], Is.Not.SameAs (source.OrderItems[0]));
      Assert.That (clone.OrderItems[0].Product, Is.EqualTo (source.OrderItems[0].Product));
      Assert.That (clone.OrderTicket, Is.Not.SameAs (source.OrderTicket));
      Assert.That (clone.OrderTicket.FileName, Is.EqualTo (source.OrderTicket.FileName));
      Assert.That (clone.Customer, Is.Not.SameAs (source.Customer));
      Assert.That (clone.Customer.Name, Is.EqualTo (source.Customer.Name));
      Assert.That (clone.Customer.Orders.ContainsObject (clone));
    }
   
    [Test]
    public void TwoClonesWithSameContext ()
    {
      DomainObjectCloner cloner = new DomainObjectCloner ();
      CloneContext context = new CloneContext (cloner);

      Order source = Order.GetObject (DomainObjectIDs.Order1);
      Order clone1 = cloner.CreateClone (source, new CompleteCloneStrategy(), context);
      Order clone2 = cloner.CreateClone (source, new CompleteCloneStrategy(), context);

      Assert.That (clone1, Is.SameAs (clone2));
    }
  }
}
