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
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transaction
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
          mockRepository.StrictMock<ClientTransactionMockEventReceiver> (ClientTransactionMock);

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
