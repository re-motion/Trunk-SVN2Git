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
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction.InactiveTransactions.AllowedOperations
{
  [TestFixture]
  public class ResurrectTest : InactiveTransactionsTestBase
  {
    private Order _invalidOrder;

    public override void SetUp ()
    {
      base.SetUp ();

      _invalidOrder = ActiveSubTransaction.Execute (() => Order.NewObject ());
      ActiveSubTransaction.Execute (() => _invalidOrder.Delete());
    }

    [Test]
    public void ResurrectInInactiveRootTransaction_IsAllowed ()
    {
      CheckState (InactiveRootTransaction, _invalidOrder, StateType.Invalid);
      CheckState (InactiveMiddleTransaction, _invalidOrder, StateType.Invalid);
      CheckState (ActiveSubTransaction, _invalidOrder, StateType.Invalid);

      ResurrectionService.ResurrectInvalidObject (InactiveRootTransaction, _invalidOrder.ID);

      CheckState (InactiveRootTransaction, _invalidOrder, StateType.NotLoadedYet);
      CheckState (InactiveMiddleTransaction, _invalidOrder, StateType.NotLoadedYet);
      CheckState (ActiveSubTransaction, _invalidOrder, StateType.NotLoadedYet);
    }

    [Test]
    public void UnloadDataInInactiveMiddleTransaction_IsAllowed ()
    {
      CheckState (InactiveRootTransaction, _invalidOrder, StateType.Invalid);
      CheckState (InactiveMiddleTransaction, _invalidOrder, StateType.Invalid);
      CheckState (ActiveSubTransaction, _invalidOrder, StateType.Invalid);

      ResurrectionService.ResurrectInvalidObject (InactiveMiddleTransaction, _invalidOrder.ID);

      CheckState (InactiveRootTransaction, _invalidOrder, StateType.NotLoadedYet);
      CheckState (InactiveMiddleTransaction, _invalidOrder, StateType.NotLoadedYet);
      CheckState (ActiveSubTransaction, _invalidOrder, StateType.NotLoadedYet);
    }
  }
}