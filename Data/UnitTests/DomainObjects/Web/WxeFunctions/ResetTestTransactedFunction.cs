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
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Assertion=Remotion.Utilities.Assertion;

namespace Remotion.Data.UnitTests.DomainObjects.Web.WxeFunctions
{
  public class ResetTestTransactedFunction : WxeFunction
  {
    public ResetTestTransactedFunction (ITransactionMode transactionMode, params object[] actualParameters)
        : base (transactionMode, actualParameters)
    {
      Assertion.IsFalse (TransactionMode.AutoCommit);
    }

    private void Step1 ()
    {
      ClientTransaction transactionBefore = ClientTransactionScope.CurrentTransaction;
      Order order = Order.GetObject (new DomainObjectIDs().Order1);
      order.OrderNumber = 7;
      transactionBefore.Rollback();

      bool addedCalled = false;
      order.OrderItems.Added += delegate { addedCalled = true; };

      bool loadedCalled = false;
      ClientTransactionScope.CurrentTransaction.Loaded += delegate { loadedCalled = true; };

      Transaction.Reset ();

      Assert.AreNotEqual (transactionBefore, ClientTransactionScope.CurrentTransaction);
      Assert.IsTrue (order.CanBeUsedInTransaction);

      Assert.AreEqual (1, order.OrderNumber);

      Assert.IsFalse (addedCalled);
      order.OrderItems.Add (OrderItem.NewObject());
      Assert.AreEqual (true, addedCalled);

      loadedCalled = false;

      Order.GetObject (new DomainObjectIDs().Order2);

      Assert.AreEqual (true, loadedCalled);
    }
  }
}
