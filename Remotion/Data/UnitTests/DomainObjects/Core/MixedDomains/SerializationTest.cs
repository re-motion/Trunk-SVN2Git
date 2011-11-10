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
      var instance = TargetClassForMixinWithState.NewObject ();
      Assert.IsTrue (((object) instance).GetType ().IsSerializable);
    }

    [Test]
    public void MixedObjectsCanBeSerializedWithoutException ()
    {
      var instance = TargetClassForMixinWithState.NewObject ();
      Serializer.Serialize (instance);
    }

    [Test]
    public void MixedObjectsCanBeDeserializedWithoutException ()
    {
      var instance = TargetClassForMixinWithState.NewObject ();
      Serializer.SerializeAndDeserialize (instance);
    }

    [Test]
    public void DomainObjectStateIsRestored ()
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

    [Test]
    public void MixinStateIsRestored ()
    {
        var instance = TargetClassForMixinWithState.NewObject ();
        Mixin.Get<MixinWithState> (instance).State = "Sto stas stat stamus statis stant";
        var deserializedObjects = Serializer.SerializeAndDeserialize (Tuple.Create (ClientTransactionScope.CurrentTransaction, instance));

        Assert.AreNotSame (Mixin.Get<MixinWithState> (instance), Mixin.Get<MixinWithState> (deserializedObjects.Item2));
        Assert.AreEqual ("Sto stas stat stamus statis stant", Mixin.Get<MixinWithState> (deserializedObjects.Item2).State);
    }

    [Test]
    public void MixinConfigurationIsRestored ()
    {
      var instance = TargetClassForMixinWithState.NewObject ();
      Assert.IsNotNull (Mixin.Get<MixinWithState> (instance));
      byte[] serializedData = Serializer.Serialize (instance);

      var deserializedInstance1 = (TargetClassForMixinWithState) Serializer.Deserialize (serializedData);
      Assert.IsNotNull (Mixin.Get<MixinWithState> (deserializedInstance1));

      using (MixinConfiguration.BuildNew().ForClass (typeof (TargetClassForMixinWithState)).AddMixins (typeof (NullMixin)).EnterScope())
      {
        var deserializedInstance2 = (TargetClassForMixinWithState) Serializer.Deserialize (serializedData);

        Assert.IsNotNull (Mixin.Get<MixinWithState> (deserializedInstance2));
        Assert.IsNull (Mixin.Get<NullMixin> (deserializedInstance2));
      }
    }
  }
}
