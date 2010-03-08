// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Web.ExecutionEngine;
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

    public Order TheOrder
    {
      get { return (Order) Variables["TheOrder"]; }
      set { Variables["TheOrder"] = value; }
    }

    private void Step1 ()
    {
      ClientTransaction transactionBefore = ClientTransactionScope.CurrentTransaction;
      Order order1 = Order.GetObject (new DomainObjectIDs().Order1);
      order1.OrderNumber = 7;
      TheOrder = order1;

      Order order2 = Order.GetObject (new DomainObjectIDs().Order2);

      transactionBefore.Rollback();

      bool addedCalled = false;
      order1.OrderItems.Added += delegate { addedCalled = true; };

      bool loadedCalled = false;
      ClientTransactionScope.CurrentTransaction.Loaded += delegate { loadedCalled = true; };

      Transaction.Reset();

      Assert.AreNotEqual (transactionBefore, ClientTransactionScope.CurrentTransaction);
      Assert.IsTrue (ClientTransaction.Current.IsEnlisted (order1));
      
      var isRootTransaction = ParentFunction == null;
      if (isRootTransaction)
        Assert.IsFalse (ClientTransaction.Current.IsEnlisted (order2));
      else
        Assert.IsTrue (ClientTransaction.Current.IsEnlisted (order2));

      Assert.AreEqual (1, order1.OrderNumber);

      Assert.IsFalse (addedCalled);
      order1.OrderItems.Add (OrderItem.NewObject());
      Assert.IsFalse (addedCalled);

      loadedCalled = false;

      Order.GetObject (new DomainObjectIDs().Order3);

      Assert.IsFalse (loadedCalled);
    }
  }
}