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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Mixins;
using Remotion.Reflection;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainImplementation
{
  [TestFixture]
  public class LifetimeServiceTest : ClientTransactionBaseTest
  {
    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Mapping does not contain class 'System.Object'.")]
    public void NewObject_InvalidType ()
    {
      LifetimeService.NewObject (TestableClientTransaction, typeof (object), ParamList.Empty);
    }

    [Test]
    public void NewObject_NoCtorArgs ()
    {
      var instance = (Order) LifetimeService.NewObject (TestableClientTransaction, typeof (Order), ParamList.Empty);
      Assert.That (instance, Is.Not.Null);
      Assert.That (instance.CtorCalled, Is.True);
    }

    [Test]
    public void NewObject_WithCtorArgs ()
    {
      var order = Order.NewObject();
      var instance = (OrderItem) LifetimeService.NewObject (TestableClientTransaction, typeof (OrderItem), ParamList.Create (order));
      Assert.That (instance, Is.Not.Null);
      Assert.That (instance.Order, Is.SameAs (order));
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type Remotion.Data.UnitTests.DomainObjects.TestDomain."
                                                                           + "OrderItem does not support the requested constructor with signature (System.Decimal).")]
    public void NewObject_WrongCtorArgs ()
    {
      LifetimeService.NewObject (TestableClientTransaction, typeof (OrderItem), ParamList.Create (0m));
    }

    [Test]
    public void NewObject_InitializesMixins ()
    {
      var domainObject = LifetimeService.NewObject (TestableClientTransaction, typeof (ClassWithAllDataTypes), ParamList.Empty);
      var mixin = Mixin.Get<MixinWithAccessToDomainObjectProperties<ClassWithAllDataTypes>> (domainObject);
      Assert.That (mixin, Is.Not.Null);
      Assert.That (mixin.OnDomainObjectCreatedCalled, Is.True);
      Assert.That (mixin.OnDomainObjectCreatedTx, Is.SameAs (TestableClientTransaction));
    }

    [Test]
    public void GetObject ()
    {
      var order = (Order) LifetimeService.GetObject (TestableClientTransaction, DomainObjectIDs.Order1, false);
      Assert.That (order, Is.Not.Null);
      Assert.That (order.ID, Is.EqualTo (DomainObjectIDs.Order1));
      Assert.That (order.CtorCalled, Is.False);
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void GetObject_IncludeDeleted_False ()
    {
      Order.GetObject (DomainObjectIDs.Order1).Delete();
      LifetimeService.GetObject (TestableClientTransaction, DomainObjectIDs.Order1, false);
    }

    [Test]
    public void GetObject_IncludeDeleted_True ()
    {
      Order.GetObject (DomainObjectIDs.Order1).Delete ();
      var order = (Order) LifetimeService.GetObject (TestableClientTransaction, DomainObjectIDs.Order1, true);
      Assert.That (order, Is.Not.Null);
      Assert.That (order.ID, Is.EqualTo (DomainObjectIDs.Order1));
      Assert.That (order.State, Is.EqualTo (StateType.Deleted));
    }

    [Test]
    public void GetObject_WithInvalidObject_Throws ()
    {
      var instance = Order.NewObject ();
      instance.Delete ();
      Assert.That (instance.IsInvalid, Is.True);

      Assert.That (() => LifetimeService.GetObject (TestableClientTransaction, instance.ID, false), Throws.TypeOf<ObjectInvalidException> ());
      Assert.That (() => LifetimeService.GetObject (TestableClientTransaction, instance.ID, true), Throws.TypeOf<ObjectInvalidException> ());
    }

    [Test]
    public void TryGetObject ()
    {
      var order = (Order) LifetimeService.TryGetObject (TestableClientTransaction, DomainObjectIDs.Order1);
      Assert.That (order, Is.Not.Null);
      Assert.That (order.ID, Is.EqualTo (DomainObjectIDs.Order1));
      Assert.That (order.CtorCalled, Is.False);
    }

    [Test]
    public void TryGetObject_Deleted ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      order.Delete ();

      var orderAgain = LifetimeService.TryGetObject (TestableClientTransaction, DomainObjectIDs.Order1);

      Assert.That (orderAgain, Is.SameAs (order));
    }

    [Test]
    public void TryGetObject_Invalid ()
    {
      var instance = Order.NewObject ();
      instance.Delete ();
      Assert.That (instance.IsInvalid, Is.True);

      var instanceAgain = LifetimeService.TryGetObject (TestableClientTransaction, instance.ID);

      Assert.That (instanceAgain, Is.SameAs (instance));
    }

    [Test]
    public void TryGetObject_NotFound ()
    {
      var id = new ObjectID (typeof (Order), Guid.NewGuid());
      Assert.That (TestableClientTransaction.IsInvalid (id), Is.False);
      
      var result = LifetimeService.TryGetObject (TestableClientTransaction, id);

      Assert.That (result, Is.Null);
      Assert.That (TestableClientTransaction.IsInvalid (id), Is.True);
    }

    [Test]
    public void GetObjectReference ()
    {
      var result = LifetimeService.GetObjectReference (TestableClientTransaction, DomainObjectIDs.Order1);

      Assert.That (result, Is.InstanceOf (typeof (Order)));
      Assert.That (result.ID, Is.EqualTo (DomainObjectIDs.Order1));
      Assert.That (result.State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void GetObjectReference_WithInvalidObject ()
    {
      var instance = Order.NewObject ();
      instance.Delete();
      Assert.That (instance.IsInvalid, Is.True);

      var result = LifetimeService.GetObjectReference (TestableClientTransaction, instance.ID);

      Assert.That (result, Is.SameAs (instance));
    }

    [Test]
    public void GetObjects ()
    {
      var deletedObjectID = DomainObjectIDs.Order3;
      var deletedObject = Order.GetObject (deletedObjectID);
      deletedObject.Delete();

      Order[] orders = LifetimeService.GetObjects<Order> (TestableClientTransaction, DomainObjectIDs.Order1, DomainObjectIDs.Order2, deletedObjectID);

      Assert.That (orders, Is.EqualTo (new[] { Order.GetObject (DomainObjectIDs.Order1), Order.GetObject (DomainObjectIDs.Order2), deletedObject }));
    }

    [Test]
    public void GetObjects_WithInvalidObject_Throws ()
    {
      var instance = Order.NewObject ();
      instance.Delete ();
      Assert.That (instance.IsInvalid, Is.True);

      Assert.That (() => LifetimeService.GetObjects<Order> (TestableClientTransaction, instance.ID, instance.ID), Throws.TypeOf<ObjectInvalidException> ());
    }

    [Test]
    public void TryGetObjects ()
    {
      var notFoundObjectID = new ObjectID (typeof (Order), Guid.NewGuid());

      var deletedObjectID = DomainObjectIDs.Order3;
      var deletedObject = Order.GetObject (deletedObjectID);
      deletedObject.Delete ();

      var invalidInstance = Order.NewObject ();
      invalidInstance.Delete ();
      Assert.That (invalidInstance.IsInvalid, Is.True);

      Order[] orders = LifetimeService.TryGetObjects<Order> (
          TestableClientTransaction, DomainObjectIDs.Order1, notFoundObjectID, deletedObjectID, invalidInstance.ID);

      Assert.That (orders, Is.EqualTo (new[] { Order.GetObject (DomainObjectIDs.Order1), null, deletedObject, invalidInstance }));
    }

    [Test]
    public void DeleteObject ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Assert.That (order.State, Is.Not.EqualTo (StateType.Deleted));
      LifetimeService.DeleteObject (TestableClientTransaction, order);
      Assert.That (order.State, Is.EqualTo (StateType.Deleted));
    }

    [Test]
    public void DeleteObject_Twice ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      LifetimeService.DeleteObject (TestableClientTransaction, order);
      LifetimeService.DeleteObject (TestableClientTransaction, order);
    }
  }
}