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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class DomainObjectUtilityTest : ClientTransactionBaseTest
  {
    [Test]
    public void GetNonNullClientTransaction_Current()
    {
      var order = Order.NewObject();
      Assert.That (DomainObjectUtility.GetNonNullClientTransaction (order), Is.SameAs (ClientTransaction.Current));
    }

    [Test]
    public void GetNonNullClientTransaction_Bound ()
    {
      Order order;
      var bindingTransaction = ClientTransaction.CreateBindingTransaction ();
      using (bindingTransaction.EnterNonDiscardingScope ())
      {
        order = Order.NewObject();
      }
      Assert.That (DomainObjectUtility.GetNonNullClientTransaction (order), Is.SameAs (bindingTransaction));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), 
        ExpectedMessage = "No ClientTransaction has been associated with the current thread or this object.")]
    public void GetNonNullClientTransaction_Null ()
    {
      var order = Order.NewObject ();
      using (ClientTransactionScope.EnterNullScope ())
      {
        DomainObjectUtility.GetNonNullClientTransaction (order);
      }
    }

    [Test]
    public void CheckIfObjectIsDiscarded_Valid ()
    {
      var order = Order.NewObject ();
      DomainObjectUtility.CheckIfObjectIsDiscarded (order, ClientTransaction.Current);
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException))]
    public void CheckIfObjectIsDiscarded_Discarded ()
    {
      var order = Order.NewObject ();
      order.Delete ();
      DomainObjectUtility.CheckIfObjectIsDiscarded (order, ClientTransaction.Current);
    }

    [Test]
    public void CheckIfRightTransaction_Works ()
    {
      var order = Order.NewObject ();
      DomainObjectUtility.CheckIfRightTransaction (order, ClientTransaction.Current);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = "Domain object 'Order|.*|System.Guid' cannot be used in the "
        + "given transaction as it was loaded or created in another transaction. Enter a scope for the transaction, or call EnlistInTransaction to "
        + "enlist the object in the transaction. (If no transaction was explicitly given, ClientTransaction.Current was used.)",
          MatchType = MessageMatch.Regex)]
    public void CheckIfRightTransaction_Fails ()
    {
      var order = Order.NewObject ();
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        DomainObjectUtility.CheckIfRightTransaction (order, ClientTransaction.Current);
      }
    }
  }
}