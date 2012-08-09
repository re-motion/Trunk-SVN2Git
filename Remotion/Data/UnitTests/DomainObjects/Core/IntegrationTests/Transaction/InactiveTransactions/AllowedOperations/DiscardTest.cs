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

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction.InactiveTransactions.AllowedOperations
{
  [TestFixture]
  public class DiscardTest : InactiveTransactionsTestBase
  {
    [Test]
    [Ignore ("TODO 5001")]
    public void DiscardInactiveRootTransaction_IsAllowed ()
    {
      Assert.That (InactiveRootTransaction.IsDiscarded, Is.False);
      Assert.That (InactiveMiddleTransaction.IsDiscarded, Is.False);
      Assert.That (ActiveSubTransaction.IsDiscarded, Is.False);

      InactiveRootTransaction.Discard();

      Assert.That (InactiveRootTransaction.IsDiscarded, Is.True);
      Assert.That (InactiveMiddleTransaction.IsDiscarded, Is.True);
      Assert.That (ActiveSubTransaction.IsDiscarded, Is.True);
    }

    [Test]
    [Ignore ("TODO 5001")]
    public void CreateSubTransactionInInactiveMiddleTransaction_IsForbidden ()
    {
      Assert.That (InactiveRootTransaction.IsDiscarded, Is.False);
      Assert.That (InactiveMiddleTransaction.IsDiscarded, Is.False);
      Assert.That (ActiveSubTransaction.IsDiscarded, Is.False);

      InactiveMiddleTransaction.Discard ();

      Assert.That (InactiveRootTransaction.IsDiscarded, Is.False);
      Assert.That (InactiveMiddleTransaction.IsDiscarded, Is.True);
      Assert.That (ActiveSubTransaction.IsDiscarded, Is.True);

      Assert.That (InactiveRootTransaction.SubTransaction, Is.Null);
      Assert.That (InactiveRootTransaction.IsActive, Is.True);
    }
  }
}