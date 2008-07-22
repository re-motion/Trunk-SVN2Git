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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public class RepositoryAccessorTest : ClientTransactionBaseTest
  {
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Mapping does not contain class 'System.Object'.")]
    public void NewObject_InvalidType ()
    {
      RepositoryAccessor.NewObject (typeof (object));
    }

    [Test]
    public void NewObject_NoCtorArgs ()
    {
      Order instance = RepositoryAccessor.NewObject (typeof (Order)).With() as Order;
      Assert.IsNotNull (instance);
      Assert.IsTrue (instance.CtorCalled);
    }

    [Test]
    public void NewObject_WithCtorArgs ()
    {
      Order order = Order.NewObject();
      OrderItem instance = RepositoryAccessor.NewObject (typeof (OrderItem)).With (order) as OrderItem;
      Assert.IsNotNull (instance);
      Assert.AreSame (order, instance.Order);
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type Remotion.Data.UnitTests.DomainObjects.TestDomain."
        + "OrderItem does not support the requested constructor with signature (System.String).")]
    public void NewObject_WrongCtorArgs ()
    {
      RepositoryAccessor.NewObject (typeof (OrderItem)).With ("foo");
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void NewObject_NoTransaction ()
    {
      using (ClientTransactionScope.EnterNullScope ())
      {
        RepositoryAccessor.NewObject (typeof (Order)).With();
      }
    }

    [Test]
    public void GetObject ()
    {
      Order order = RepositoryAccessor.GetObject (DomainObjectIDs.Order1, false) as Order;
      Assert.IsNotNull (order);
      Assert.AreEqual (DomainObjectIDs.Order1, order.ID);
      Assert.IsFalse (order.CtorCalled);
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void GetObject_IncludeDeleted_False ()
    {
      Order.GetObject (DomainObjectIDs.Order1).Delete();
      RepositoryAccessor.GetObject (DomainObjectIDs.Order1, false);
    }

    [Test]
    public void GetObject_IncludeDeleted_True ()
    {
      Order.GetObject (DomainObjectIDs.Order1).Delete ();
      Order order = RepositoryAccessor.GetObject (DomainObjectIDs.Order1, true) as Order;
      Assert.IsNotNull (order);
      Assert.AreEqual (DomainObjectIDs.Order1, order.ID);
      Assert.AreEqual (StateType.Deleted, order.State);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void GetObject_NoTransaction ()
    {
      using (ClientTransactionScope.EnterNullScope ())
      {
        RepositoryAccessor.GetObject (DomainObjectIDs.Order1, false);
      }
    }

    [Test]
    public void DeleteObject ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Assert.AreNotEqual (StateType.Deleted, order.State);
      RepositoryAccessor.DeleteObject (order);
      Assert.AreEqual (StateType.Deleted, order.State);
    }

    [Test]
    public void DeleteObject_Twice ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      RepositoryAccessor.DeleteObject (order);
      RepositoryAccessor.DeleteObject (order);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread or this object.")]
    public void DeleteObject_NoTransaction ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransactionScope.EnterNullScope ())
      {
        RepositoryAccessor.DeleteObject (order);
      }
    }
  }
}
