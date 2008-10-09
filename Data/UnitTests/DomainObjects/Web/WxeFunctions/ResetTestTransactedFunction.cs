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
      Assert.IsTrue (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));

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
