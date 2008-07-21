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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Cloning;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Cloning
{
  [TestFixture]
  public class IntegrationTest : ClientTransactionBaseTest
  {
    [Test]
    public void CompleteCloneStrategy ()
    {
      DomainObjectCloner cloner = new DomainObjectCloner ();
      ClientTransaction bindingTransaction = ClientTransaction.NewBindingTransaction ();
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