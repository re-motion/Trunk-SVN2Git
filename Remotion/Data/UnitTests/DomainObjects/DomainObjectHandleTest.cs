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
using Remotion.Data.UnitTests.DomainObjects.Core;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects
{
  [TestFixture]
  public class DomainObjectHandleTest : StandardMappingTest
  {
    [Test]
    public void Initialization ()
    {
      var handle = new DomainObjectHandle<Order> (DomainObjectIDs.Order1);
      Assert.That (handle.ObjectID, Is.EqualTo (DomainObjectIDs.Order1));
    }

    [Test]
    public void Initialization_ThrowsWhenTypeDoesntMatchID ()
    {
      Assert.That (
          () => new DomainObjectHandle<OrderItem> (DomainObjectIDs.Order1), 
          Throws.ArgumentException.With.Message.EqualTo (
              "The class type of ObjectID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' doesn't match the handle type "
              + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem'.\r\nParameter name: objectID"));
      Assert.That (
          () => new DomainObjectHandle<DomainObject> (DomainObjectIDs.Order1),
          Throws.ArgumentException.With.Message.EqualTo (
              "The class type of ObjectID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' doesn't match the handle type "
              + "'Remotion.Data.DomainObjects.DomainObject'.\r\nParameter name: objectID"));
    }

    [Test]
    public void Cast_CanUpcast ()
    {
      var handle = new DomainObjectHandle<Order> (DomainObjectIDs.Order1);

      var result = handle.Cast<DomainObject> ();

      Assert.That (result, Is.SameAs (handle));
      Assert.That (VariableTypeInferrer.GetVariableType (result), Is.SameAs (typeof (IDomainObjectHandle<DomainObject>)));
    }

    [Test]
    public void Cast_CanDowncast ()
    {
      var handle = new DomainObjectHandle<Order> (DomainObjectIDs.Order1).Cast<DomainObject> ();

      var result = handle.Cast<Order>();

      Assert.That (result, Is.SameAs (handle));
      Assert.That (VariableTypeInferrer.GetVariableType (result), Is.SameAs (typeof (IDomainObjectHandle<Order>)));
    }

    [Test]
    public void Cast_ThrowsOnUnsupportedCast ()
    {
      var handle = new DomainObjectHandle<Order> (DomainObjectIDs.Order1);

      Assert.That (
          () => handle.Cast<OrderItem> (),
          Throws.TypeOf<InvalidCastException> ().With.Message.EqualTo (
              "The handle for object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be represented as a handle for type "
              + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem'."));
    }

    [Test]
    public void Equals_Object_True ()
    {
      var handle1 = new DomainObjectHandle<Order> (DomainObjectIDs.Order1);
      var handle2 = new DomainObjectHandle<Order> (DomainObjectIDs.Order1);

      Assert.That (handle1.Equals ((object) handle2), Is.True);
    }

    [Test]
    public void Equals_Object_False_DifferentIDs ()
    {
      var handle1 = new DomainObjectHandle<Order> (DomainObjectIDs.Order1);
      var handle2 = new DomainObjectHandle<Order> (DomainObjectIDs.Order2);

      Assert.That (handle1.Equals ((object) handle2), Is.False);
    }

    [Test]
    public void Equals_Equatable_True ()
    {
      var handle1 = new DomainObjectHandle<Order> (DomainObjectIDs.Order1);
      var handle2 = new DomainObjectHandle<Order> (DomainObjectIDs.Order1);

      Assert.That (handle1.Equals (handle2), Is.True);
    }

    [Test]
    public void Equals_Equatable_False_DifferentIDs ()
    {
      var handle1 = new DomainObjectHandle<Order> (DomainObjectIDs.Order1);
      var handle2 = new DomainObjectHandle<Order> (DomainObjectIDs.Order2);

      Assert.That (handle1.Equals (handle2), Is.False);
    }

    [Test]
    public void GetHashCode_EqualObjects ()
    {
      var handle1 = new DomainObjectHandle<Order> (DomainObjectIDs.Order1);
      var handle2 = new DomainObjectHandle<Order> (DomainObjectIDs.Order1);

      Assert.That (handle1.GetHashCode(), Is.EqualTo (handle2.GetHashCode()));
    }

    [Test]
    public new void ToString ()
    {
      var handle = new DomainObjectHandle<Order> (DomainObjectIDs.Order1);
      Assert.That (handle.ToString (), Is.EqualTo ("Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid (handle)"));
    }

    [Test]
    public void Serializable ()
    {
      var handle = new DomainObjectHandle<Order> (DomainObjectIDs.Order1);

      var result = Serializer.SerializeAndDeserialize (handle);

      Assert.That (result, Is.EqualTo (handle));
    }

    [Test]
    public void TypeProvider_OnInterface ()
    {
      Assert.That (TypeConversionProvider.Current.CanConvert (typeof (IDomainObjectHandle<Order>), typeof (string)), Is.True);
      Assert.That (TypeConversionProvider.Current.CanConvert (typeof (string), typeof (IDomainObjectHandle<Order>)), Is.True);
    }

    [Test]
    [Ignore ("TODO 4405")]
    public void TypeProvider_OnClass ()
    {
      Assert.That (TypeConversionProvider.Current.CanConvert (typeof (DomainObjectHandle<Order>), typeof (string)), Is.True);
      Assert.That (TypeConversionProvider.Current.CanConvert (typeof (string), typeof (DomainObjectHandle<Order>)), Is.True);
    }
  }
}