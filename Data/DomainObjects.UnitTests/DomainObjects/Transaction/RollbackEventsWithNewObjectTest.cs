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
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Transaction
{
  [TestFixture]
  public class RollbackEventsWithNewObjectTest : ClientTransactionBaseTest
  {
    // types

    // static members and constants

    // member fields

    private Order _order1;
    private Customer _newCustomer;

    // construction and disposing

    public RollbackEventsWithNewObjectTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _newCustomer = Customer.NewObject ();
    }

    [Test]
    public void DiscardOtherObjectInDomainObjectRollingBack ()
    {
      _order1.Customer = _newCustomer;
      _order1.RollingBack += Order1_RollingBack;

      ClientTransactionMock.Rollback ();

      // expectation: no ObjectDiscardedException
    }

    [Test]
    public void ObjectDiscardedInDomainObjectRollingBackDoesNotRaiseRollingBackEvent ()
    {
      _order1.Customer = _newCustomer;
      _order1.RollingBack += Order1_RollingBack;
      _newCustomer.RollingBack += NewCustomer_RollingBack_MustNotBeCalled;

      ClientTransactionMock.Rollback ();

      // expectation: NewCustomer_RollingBack_MustNotBeCalled must not throw an AssertionException
    }

    [Test]
    public void DiscardObjectInClientTransactionRollingBack ()
    {
      _order1.Customer = _newCustomer;
      ClientTransactionMock.RollingBack += ClientTransaction_RollingBack;

      ClientTransactionMock.Rollback ();

      // expectation: no ObjectDiscardedException
    }

    [Test]
    public void RolledBackEventWithNewObject ()
    {
      MockRepository mockRepository = new MockRepository ();

      ClientTransactionMockEventReceiver clientTransactionMockEventReceiver = 
          mockRepository.CreateMock<ClientTransactionMockEventReceiver> (ClientTransactionMock);

      using (mockRepository.Ordered ())
      {
        clientTransactionMockEventReceiver.RollingBack (ClientTransactionMock, _newCustomer);
        clientTransactionMockEventReceiver.RolledBack (ClientTransactionMock);
      }

      mockRepository.ReplayAll ();

      ClientTransactionMock.Rollback ();

      mockRepository.VerifyAll ();
    }

    private void NewCustomer_RollingBack_MustNotBeCalled (object sender, EventArgs e)
    {
      throw new AssertionException ("New customer must not throw a RollingBack event, because it is already discarded.");
    }

    private void Order1_RollingBack (object sender, EventArgs e)
    {
      DeleteNewCustomer ();
    }

    private void ClientTransaction_RollingBack (object sender, ClientTransactionEventArgs args)
    {
      DeleteNewCustomer ();
    }

    private void DeleteNewCustomer ()
    {
      _newCustomer.Delete ();
    }
  }
}
