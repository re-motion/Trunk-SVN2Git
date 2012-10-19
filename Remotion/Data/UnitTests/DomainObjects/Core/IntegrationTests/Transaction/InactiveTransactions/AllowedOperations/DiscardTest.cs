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
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction.InactiveTransactions.AllowedOperations
{
  [TestFixture]
  public class DiscardTest : InactiveTransactionsTestBase
  {
    [Test]
    public void DiscardInactiveRootTransaction_IsAllowed ()
    {
      Assert.That (InactiveRootTransaction.IsDiscarded, Is.False);
      Assert.That (InactiveMiddleTransaction.IsDiscarded, Is.False);
      Assert.That (ActiveSubTransaction.IsDiscarded, Is.False);

      var extensionMock = CreateAndAddExtensionMock();
      using (extensionMock.GetMockRepository().Ordered())
      {
        extensionMock
            .Expect (mock => mock.TransactionDiscard (InactiveRootTransaction))
            .WhenCalled (mi => CheckTransactionHierarchy ());
        extensionMock
            .Expect (mock => mock.TransactionDiscard (InactiveMiddleTransaction))
            .WhenCalled (mi => CheckTransactionHierarchy ());
        extensionMock
            .Expect (mock => mock.TransactionDiscard (ActiveSubTransaction))
            .WhenCalled (mi => CheckTransactionHierarchy());
      }

      InactiveRootTransaction.Discard();

      extensionMock.VerifyAllExpectations();
      Assert.That (InactiveRootTransaction.IsDiscarded, Is.True);
      Assert.That (InactiveMiddleTransaction.IsDiscarded, Is.True);
      Assert.That (ActiveSubTransaction.IsDiscarded, Is.True);
    }

    [Test]
    public void DiscardInactiveMiddleTransaction_IsAllowed ()
    {
      Assert.That (InactiveRootTransaction.IsDiscarded, Is.False);
      Assert.That (InactiveMiddleTransaction.IsDiscarded, Is.False);
      Assert.That (ActiveSubTransaction.IsDiscarded, Is.False);

      var extensionMock = CreateAndAddExtensionMock ();
      using (extensionMock.GetMockRepository ().Ordered ())
      {
        extensionMock
            .Expect (mock => mock.TransactionDiscard (InactiveMiddleTransaction))
            .WhenCalled (mi => CheckTransactionHierarchy ());
        extensionMock
            .Expect (mock => mock.TransactionDiscard (ActiveSubTransaction))
            .WhenCalled (mi => CheckTransactionHierarchy ());
      }

      InactiveMiddleTransaction.Discard ();

      Assert.That (InactiveRootTransaction.IsDiscarded, Is.False);
      Assert.That (InactiveMiddleTransaction.IsDiscarded, Is.True);
      Assert.That (ActiveSubTransaction.IsDiscarded, Is.True);

      Assert.That (InactiveRootTransaction.SubTransaction, Is.Null);
      Assert.That (InactiveRootTransaction.IsActive, Is.True);
    }

    private void CheckTransactionHierarchy ()
    {
      Assert.That (ActiveSubTransaction.ParentTransaction, Is.SameAs (InactiveMiddleTransaction));
      Assert.That (InactiveMiddleTransaction.SubTransaction, Is.SameAs (ActiveSubTransaction));
      Assert.That (InactiveMiddleTransaction.ParentTransaction, Is.SameAs (InactiveRootTransaction));
      Assert.That (InactiveRootTransaction.SubTransaction, Is.SameAs (InactiveMiddleTransaction));

      Assert.That (ActiveSubTransaction.IsDiscarded, Is.False);
      Assert.That (InactiveMiddleTransaction.IsDiscarded, Is.False);
      Assert.That (InactiveRootTransaction.IsDiscarded, Is.False);

      Assert.That (ActiveSubTransaction.IsActive, Is.True);
      Assert.That (InactiveMiddleTransaction.IsActive, Is.False);
      Assert.That (InactiveRootTransaction.IsActive, Is.False);
    }

    private IClientTransactionExtension CreateAndAddExtensionMock ()
    {
      var extensionMock = MockRepository.GenerateStrictMock<IClientTransactionExtension>();
      extensionMock.Stub (stub => stub.Key).Return ("Test");
      InactiveRootTransaction.Extensions.Add (extensionMock);
      InactiveMiddleTransaction.Extensions.Add (extensionMock);
      ActiveSubTransaction.Extensions.Add (extensionMock);
      return extensionMock;
    }
  }
}