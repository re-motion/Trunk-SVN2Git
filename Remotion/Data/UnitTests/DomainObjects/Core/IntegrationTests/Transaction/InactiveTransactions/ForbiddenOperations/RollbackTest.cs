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
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction.InactiveTransactions.ForbiddenOperations
{
  [TestFixture]
  public class RollbackTest : InactiveTransactionsTestBase
  {
    protected override void InitializeInactiveRootTransaction ()
    {
      base.InitializeInactiveRootTransaction ();
      ClassWithAllDataTypes.NewObject ();
    }

    protected override void InitializeInactiveMiddleTransaction ()
    {
      base.InitializeInactiveMiddleTransaction ();
      ClassWithAllDataTypes.NewObject ();
    }

    protected override void InitializeActiveSubTransaction ()
    {
      base.InitializeActiveSubTransaction ();
      ClassWithAllDataTypes.NewObject ();
    }
    
    [Test]
    public void RollbackInInactiveRootTransaction_IsForbidden ()
    {
      Assert.That (InactiveRootTransaction.HasChanged (), Is.True);
      Assert.That (InactiveMiddleTransaction.HasChanged (), Is.True);
      Assert.That (ActiveSubTransaction.HasChanged (), Is.True);

      CheckForbidden (() => InactiveRootTransaction.Rollback (), "TransactionRollingBack");

      Assert.That (InactiveRootTransaction.HasChanged (), Is.True);
      Assert.That (InactiveMiddleTransaction.HasChanged (), Is.True);
      Assert.That (ActiveSubTransaction.HasChanged (), Is.True);
    }

    [Test]
    public void RollbackInInactiveMiddleTransaction_IsForbidden ()
    {
      Assert.That (InactiveRootTransaction.HasChanged (), Is.True);
      Assert.That (InactiveMiddleTransaction.HasChanged (), Is.True);
      Assert.That (ActiveSubTransaction.HasChanged (), Is.True);

      CheckForbidden (() => InactiveMiddleTransaction.Rollback (), "TransactionRollingBack");

      Assert.That (InactiveRootTransaction.HasChanged (), Is.True);
      Assert.That (InactiveMiddleTransaction.HasChanged (), Is.True);
      Assert.That (ActiveSubTransaction.HasChanged (), Is.True);
    }
  }
}