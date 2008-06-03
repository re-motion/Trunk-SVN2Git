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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Core.DomainObjects
{
  [TestFixture]
  public class TransactionalAccessorTest : ClientTransactionBaseTest
  {
    private TransactionalAccessor<T> CreateTransactionalAccessor<T> (PropertyAccessor accessor)
    {
      return (TransactionalAccessor<T>) PrivateInvoke.CreateInstanceNonPublicCtor (typeof (TransactionalAccessor<T>), accessor);
    }

    [Test]
    public void TransactionalAccessorGetValue ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      PropertyAccessor orderNumberAccessor = order.Properties[typeof (Order), "OrderNumber"];

      ClientTransaction secondTransaction = ClientTransaction.NewRootTransaction ();
      secondTransaction.EnlistDomainObject (order);

      orderNumberAccessor.SetValue (12);
      orderNumberAccessor.SetValueTx (secondTransaction, 13);

      TransactionalAccessor<int> orderNumberTx = CreateTransactionalAccessor<int> (orderNumberAccessor);
      Assert.AreEqual (12, orderNumberTx[ClientTransactionMock]);
      Assert.AreEqual (13, orderNumberTx[secondTransaction]);
    }

    [Test]
    public void TransactionalAccessorSetValue ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      PropertyAccessor orderNumberAccessor = order.Properties[typeof (Order), "OrderNumber"];

      ClientTransaction secondTransaction = ClientTransaction.NewRootTransaction ();
      secondTransaction.EnlistDomainObject (order);

      TransactionalAccessor<int> orderNumberTx = CreateTransactionalAccessor<int> (orderNumberAccessor);

      orderNumberTx[ClientTransactionMock] = 12;
      orderNumberTx[secondTransaction] = 13;

      
      Assert.AreEqual (12, orderNumberAccessor.GetValue<int>());
      Assert.AreEqual (13, orderNumberAccessor.GetValueTx<int> (secondTransaction));
    }

    [Test]
    [ExpectedException (typeof (InvalidTypeException), ExpectedMessage = 
        "Actual type 'System.Int32' of property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber' does not match expected type "
        + "'System.Double'.")]
    public void TransactionalAccessorThrowsOnInvalidT ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      PropertyAccessor orderNumberAccessor = order.Properties[typeof (Order), "OrderNumber"];

      CreateTransactionalAccessor<double> (orderNumberAccessor);
    }

    [Test]
    public void GetValueSetValueOnObject ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);

      ClientTransaction secondTransaction = ClientTransaction.NewRootTransaction ();
      secondTransaction.EnlistDomainObject (order);

      order.OrderNumberTx[ClientTransactionMock] = 7;
      order.OrderNumberTx[secondTransaction] = 56;

      Assert.AreEqual (7, order.OrderNumberTx[ClientTransactionMock]);
      Assert.AreEqual (56, order.OrderNumberTx[secondTransaction]);
    }
  }
}
