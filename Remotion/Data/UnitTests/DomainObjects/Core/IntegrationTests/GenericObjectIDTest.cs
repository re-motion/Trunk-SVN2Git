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
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  public class GenericObjectIDTest : ClientTransactionBaseTest
  {
    private IObjectID<Order> _orderTypedID;
    private ObjectID _untypedID;

    public override void SetUp ()
    {
      base.SetUp ();

      _orderTypedID = DomainObjectIDs.Order1.AsObjectID<Order>();
      _untypedID = DomainObjectIDs.Order1;
    }

    [Test]
    public void ObjectID_Create_FromType ()
    {
      var value = Guid.NewGuid();

      var result = ObjectID.Create (typeof (Order), value);

      Assert.That (result, Is.TypeOf<ObjectID<Order>> ());
      Assert.That (result, Is.EqualTo (ObjectID.Create<Order> (value)));
    }

    [Test]
    public void ObjectID_Create_FromClassID ()
    {
      var value = Guid.NewGuid ();

      var result = ObjectID.Create ("Order", value);

      Assert.That (result, Is.TypeOf<ObjectID<Order>> ());
      Assert.That (result, Is.EqualTo (ObjectID.Create<Order> (value)));
    }

    [Test]
    public void ObjectID_Create_FromClassDefinition ()
    {
      var value = Guid.NewGuid ();

      var result = ObjectID.Create (GetTypeDefinition (typeof (Order)), value);

      Assert.That (result, Is.TypeOf<ObjectID<Order>> ());
      Assert.That (result, Is.EqualTo (ObjectID.Create<Order> (value)));
    }

    [Test]
    public void ObjectID_CovariantInterface ()
    {
      var result = ObjectID.Create (typeof (Order), Guid.NewGuid());

      Assert.That (result, Is.AssignableTo<IObjectID<Order>> ());
      Assert.That (result, Is.AssignableTo<IObjectID<TestDomainBase>> ());
      Assert.That (result, Is.AssignableTo<IObjectID<DomainObject>> ());
    }

    [Test]
    public void GetTypedID ()
    {
      Order order = Order.NewObject();

      var orderTypedID = order.GetTypedID ();
      var testDomainBaseTypedID1 = ((TestDomainBase) order).GetTypedID ();
      var testDomainBaseTypedID2 = order.GetTypedID<TestDomainBase> ();
      var domainObjectTypedID = ((DomainObject) order).GetTypedID ();

      Assert.That (orderTypedID, Is.EqualTo (order.ID));
      Assert.That (orderTypedID, Is.TypeOf<ObjectID<Order>> ());
      Assert.That (VariableTypeInferrer.GetVariableType (orderTypedID), Is.SameAs (typeof (IObjectID<Order>)));

      Assert.That (testDomainBaseTypedID1, Is.EqualTo (order.ID));
      Assert.That (testDomainBaseTypedID1, Is.TypeOf<ObjectID<Order>> ());
      Assert.That (VariableTypeInferrer.GetVariableType (testDomainBaseTypedID1), Is.SameAs (typeof (IObjectID<TestDomainBase>)));

      Assert.That (testDomainBaseTypedID2, Is.EqualTo (order.ID));
      Assert.That (testDomainBaseTypedID2, Is.TypeOf<ObjectID<Order>> ());
      Assert.That (VariableTypeInferrer.GetVariableType (testDomainBaseTypedID2), Is.SameAs (typeof (IObjectID<TestDomainBase>)));

      Assert.That (domainObjectTypedID, Is.EqualTo (order.ID));
      Assert.That (domainObjectTypedID, Is.TypeOf<ObjectID<Order>> ());
      Assert.That (VariableTypeInferrer.GetVariableType (domainObjectTypedID), Is.SameAs (typeof (IObjectID<DomainObject>)));
    }

    [Test]
    public void GetTypedID_Null ()
    {
      Assert.That (() => ((Order) null).GetTypedID (), Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void GetSafeTypedID ()
    {
      Order order = Order.NewObject ();

      var orderTypedID = order.GetSafeTypedID ();
      var testDomainBaseTypedID1 = ((TestDomainBase) order).GetSafeTypedID ();
      var testDomainBaseTypedID2 = order.GetSafeTypedID<TestDomainBase> ();
      var domainObjectTypedID = ((DomainObject) order).GetSafeTypedID ();

      Assert.That (orderTypedID, Is.EqualTo (order.ID));
      Assert.That (orderTypedID, Is.TypeOf<ObjectID<Order>> ());
      Assert.That (VariableTypeInferrer.GetVariableType (orderTypedID), Is.SameAs (typeof (IObjectID<Order>)));

      Assert.That (testDomainBaseTypedID1, Is.EqualTo (order.ID));
      Assert.That (testDomainBaseTypedID1, Is.TypeOf<ObjectID<Order>> ());
      Assert.That (VariableTypeInferrer.GetVariableType (testDomainBaseTypedID1), Is.SameAs (typeof (IObjectID<TestDomainBase>)));

      Assert.That (testDomainBaseTypedID2, Is.EqualTo (order.ID));
      Assert.That (testDomainBaseTypedID2, Is.TypeOf<ObjectID<Order>> ());
      Assert.That (VariableTypeInferrer.GetVariableType (testDomainBaseTypedID2), Is.SameAs (typeof (IObjectID<TestDomainBase>)));

      Assert.That (domainObjectTypedID, Is.EqualTo (order.ID));
      Assert.That (domainObjectTypedID, Is.TypeOf<ObjectID<Order>> ());
      Assert.That (VariableTypeInferrer.GetVariableType (domainObjectTypedID), Is.SameAs (typeof (IObjectID<DomainObject>)));
    }

    [Test]
    public void GetSafeTypedID_Null ()
    {
      Assert.That (((Order) null).GetSafeTypedID (), Is.Null);
    }

    [Test]
    public void ImplicitCast_FromObjectID_ToIObjectID ()
    {
      ObjectID objectID = DomainObjectIDs.Order1;

      IObjectID<DomainObject> castID = objectID;

      Assert.That (castID, Is.EqualTo (objectID));
    }

    [Test]
    public void AsObjectID ()
    {
      var result = _orderTypedID.AsObjectID();

      Assert.That (VariableTypeInferrer.GetVariableType (result), Is.SameAs (typeof (ObjectID)));
      Assert.That (result, Is.EqualTo (_orderTypedID));
    }

    [Test]
    public void ClientTransaction_GetEnlistedDomainObject_InfersResultType ()
    {
      var typedResult = TestableClientTransaction.GetEnlistedDomainObject (_orderTypedID);
      var untypedResult = TestableClientTransaction.GetEnlistedDomainObject (_untypedID);

      Assert.That (VariableTypeInferrer.GetVariableType (typedResult), Is.SameAs (typeof (Order)));
      Assert.That (VariableTypeInferrer.GetVariableType (untypedResult), Is.SameAs (typeof (DomainObject)));
    }

    [Test]
    public void LifetimeService_GetObject_InfersResultType ()
    {
      var typedResult = LifetimeService.GetObject (TestableClientTransaction, _orderTypedID, false);
      var untypedResult = LifetimeService.GetObject (TestableClientTransaction, _untypedID, false);

      Assert.That (VariableTypeInferrer.GetVariableType (typedResult), Is.SameAs (typeof (Order)));
      Assert.That (VariableTypeInferrer.GetVariableType (untypedResult), Is.SameAs (typeof (DomainObject)));
    }

    [Test]
    public void LifetimeService_GetObjects_InfersResultType ()
    {
      var typedResult = LifetimeService.GetObjects (TestableClientTransaction, _orderTypedID);
      var untypedResult = LifetimeService.GetObjects (TestableClientTransaction, _untypedID);

      Assert.That (VariableTypeInferrer.GetVariableType (typedResult), Is.SameAs (typeof (Order[])));
      Assert.That (VariableTypeInferrer.GetVariableType (untypedResult), Is.SameAs (typeof (DomainObject[])));
    }

    [Test]
    public void LifetimeService_TryGetObject_InfersResultType ()
    {
      var typedResult = LifetimeService.TryGetObject (TestableClientTransaction, _orderTypedID);
      var untypedResult = LifetimeService.TryGetObject (TestableClientTransaction, _untypedID);

      Assert.That (VariableTypeInferrer.GetVariableType (typedResult), Is.SameAs (typeof (Order)));
      Assert.That (VariableTypeInferrer.GetVariableType (untypedResult), Is.SameAs (typeof (DomainObject)));
    }

    [Test]
    public void LifetimeService_TryGetObjects_InfersResultType ()
    {
      var typedResult = LifetimeService.TryGetObjects (TestableClientTransaction, _orderTypedID);
      var untypedResult = LifetimeService.TryGetObjects (TestableClientTransaction, _untypedID);

      Assert.That (VariableTypeInferrer.GetVariableType (typedResult), Is.SameAs (typeof (Order[])));
      Assert.That (VariableTypeInferrer.GetVariableType (untypedResult), Is.SameAs (typeof (DomainObject[])));
    }

    [Test]
    public void LifetimeService_GetObjectReference_InfersResultType ()
    {
      var typedResult = LifetimeService.GetObjectReference (TestableClientTransaction, _orderTypedID);
      var untypedResult = LifetimeService.GetObjectReference (TestableClientTransaction, _untypedID);

      Assert.That (VariableTypeInferrer.GetVariableType (typedResult), Is.SameAs (typeof (Order)));
      Assert.That (VariableTypeInferrer.GetVariableType (untypedResult), Is.SameAs (typeof (DomainObject)));
    }

    [Test]
    public void Serialization ()
    {
      var value = Guid.NewGuid();
      var objectID = ObjectID.Create (typeof (Order), value);

      var deserialized = Serializer.SerializeAndDeserialize (objectID);

      Assert.That (deserialized, Is.TypeOf<ObjectID<Order>>());
      Assert.That (deserialized, Is.EqualTo (objectID));
    }
  }
}