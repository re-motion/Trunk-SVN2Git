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
        + "OrderItem does not support the requested constructor with signature (System.Decimal).")]
    public void NewObject_WrongCtorArgs ()
    {
      RepositoryAccessor.NewObject (typeof (OrderItem)).With (0m);
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
