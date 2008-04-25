using System;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.MixedDomains
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

        using (deserializedObjects.A.EnterDiscardingScope ())
        {
          Assert.AreEqual (5, deserializedObjects.B.OrderNumber);
          Assert.IsTrue (deserializedObjects.B.OrderItems.Contains (DomainObjectIDs.OrderItem4));
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

        Assert.AreNotSame (Mixin.Get<MixinWithState> (order), Mixin.Get<MixinWithState> (deserializedObjects.B));
        Assert.AreEqual ("Sto stas stat stamus statis stant", Mixin.Get<MixinWithState> (deserializedObjects.B).State);
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