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
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains
{
  [TestFixture]
  public class SerializationTest : ClientTransactionBaseTest
  {
    [Test]
    public void MixedTypesAreSerializable ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (Order)).Clear().AddMixins (typeof (MixinAddingInterface)).EnterScope())
      {
        Order order = Order.NewObject ();
        Assert.IsTrue (((object) order).GetType ().IsSerializable);
      }
    }

    [Test]
    public void MixedObjectsCanBeSerializedWithoutException ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (Order)).Clear().AddMixins (typeof (MixinAddingInterface)).EnterScope())
      {
        Order order = Order.NewObject();
        Serializer.Serialize (order);
      }
    }

    [Test]
    public void MixedObjectsCanBeDeserializedWithoutException ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (Order)).Clear().AddMixins (typeof (MixinAddingInterface)).EnterScope())
      {
        Order order = Order.NewObject ();
        Serializer.SerializeAndDeserialize (order);
      }
    }

    [Test]
    public void DomainObjectStateIsRestored ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (Order)).Clear().AddMixins (typeof (MixinAddingInterface)).EnterScope())
      {
        Order order = Order.NewObject ();
        order.OrderNumber = 5;
        order.OrderItems.Add (OrderItem.GetObject (DomainObjectIDs.OrderItem4));
        Tuple<ClientTransaction, Order> deserializedObjects =
            Serializer.SerializeAndDeserialize (new Tuple<ClientTransaction, Order> (ClientTransactionScope.CurrentTransaction, order));

        using (deserializedObjects.Item1.EnterDiscardingScope ())
        {
          Assert.AreEqual (5, deserializedObjects.Item2.OrderNumber);
          Assert.IsTrue (deserializedObjects.Item2.OrderItems.Contains (DomainObjectIDs.OrderItem4));
        }
      }
    }

    [Test]
    public void MixinStateIsRestored ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (Order)).Clear().AddMixins (typeof (MixinWithState)).EnterScope())
      {
        Order order = Order.NewObject ();
        Mixin.Get<MixinWithState> (order).State = "Sto stas stat stamus statis stant";
        Tuple<ClientTransaction, Order> deserializedObjects =
            Serializer.SerializeAndDeserialize (new Tuple<ClientTransaction, Order> (ClientTransactionScope.CurrentTransaction, order));

        Assert.AreNotSame (Mixin.Get<MixinWithState> (order), Mixin.Get<MixinWithState> (deserializedObjects.Item2));
        Assert.AreEqual ("Sto stas stat stamus statis stant", Mixin.Get<MixinWithState> (deserializedObjects.Item2).State);
      }
    }

    [Test]
    public void MixinConfigurationIsRestored ()
    {
      byte[] serializedData;
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (Order)).Clear().AddMixins (typeof (MixinWithState)).EnterScope())
      {
        Order order = Order.NewObject ();
        Assert.IsNotNull (Mixin.Get<MixinWithState> (order));
        serializedData = Serializer.Serialize (order);
      }

      Order deserializedOrder1 = (Order) Serializer.Deserialize (serializedData);
      Assert.IsNotNull (Mixin.Get<MixinWithState> (deserializedOrder1));

      using (MixinConfiguration.BuildFromActive().ForClass (typeof (Order)).Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        Order deserializedOrder2 = (Order) Serializer.Deserialize (serializedData);
        Assert.IsNotNull (Mixin.Get<MixinWithState> (deserializedOrder2));
        Assert.IsNull (Mixin.Get<NullMixin> (deserializedOrder2));
      }
    }
  }
}
