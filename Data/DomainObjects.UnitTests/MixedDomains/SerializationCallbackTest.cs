using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.MixedDomains
{
  [TestFixture]
  public class SerializationCallbackTest : ClientTransactionBaseTest
  {
    [Serializable]
    public class MixinWithSerializationCallbacks : ClassWithSerializationCallbacksBase
    {
      private static ISerializationEventReceiver s_receiver;

      public static void SetReceiver (ISerializationEventReceiver receiver)
      {
        s_receiver = receiver;
      }

      protected override ISerializationEventReceiver StaticReceiver
      {
        get { return s_receiver; }
      }
    }

    [Test]
    public void SerializationEvents_OnTarget ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (ClassWithSerializationCallbacks)).Clear().AddMixins (typeof (MixinWithSerializationCallbacks)).EnterScope())
      {
        ClassWithSerializationCallbacks instance =
            (ClassWithSerializationCallbacks) RepositoryAccessor.NewObject (typeof (ClassWithSerializationCallbacks)).With();

        Assert.AreNotSame (typeof (ClassWithSerializationCallbacks), ((object) instance).GetType());
        Assert.IsTrue (instance is IMixinTarget);

        new SerializationCallbackTester<ClassWithSerializationCallbacks> (new RhinoMocksRepositoryAdapter(), instance, ClassWithSerializationCallbacks.SetReceiver)
            .Test_SerializationCallbacks();
      }
    }

    [Test]
    public void DeserializationEvents_OnTarget ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (ClassWithSerializationCallbacks)).Clear().AddMixins (typeof (MixinWithSerializationCallbacks)).EnterScope())
      {
        ClassWithSerializationCallbacks instance =
            (ClassWithSerializationCallbacks) RepositoryAccessor.NewObject (typeof (ClassWithSerializationCallbacks)).With();

        Assert.AreNotSame (typeof (ClassWithSerializationCallbacks), ((object) instance).GetType());
        Assert.IsTrue (instance is IMixinTarget);

        new SerializationCallbackTester<ClassWithSerializationCallbacks> (new RhinoMocksRepositoryAdapter (), instance, ClassWithSerializationCallbacks.SetReceiver)
            .Test_DeserializationCallbacks();
      }
    }

    [Test]
    public void SerializationEvents_OnMixin ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (ClassWithSerializationCallbacks)).Clear().AddMixins (typeof (MixinWithSerializationCallbacks)).EnterScope())
      {
        ClassWithSerializationCallbacks instance =
            (ClassWithSerializationCallbacks) RepositoryAccessor.NewObject (typeof (ClassWithSerializationCallbacks)).With();

        Assert.AreNotSame (typeof (ClassWithSerializationCallbacks), ((object) instance).GetType ());
        Assert.IsTrue (instance is IMixinTarget);

        new SerializationCallbackTester<ClassWithSerializationCallbacks> (new RhinoMocksRepositoryAdapter (), instance, MixinWithSerializationCallbacks.SetReceiver)
            .Test_SerializationCallbacks ();
      }
    }

    [Test]
    public void DeserializationEvents_OnMixin ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (ClassWithSerializationCallbacks)).Clear().AddMixins (typeof (MixinWithSerializationCallbacks)).EnterScope())
      {
        ClassWithSerializationCallbacks instance =
            (ClassWithSerializationCallbacks) RepositoryAccessor.NewObject (typeof (ClassWithSerializationCallbacks)).With();

        Assert.AreNotSame (typeof (ClassWithSerializationCallbacks), ((object) instance).GetType ());
        Assert.IsTrue (instance is IMixinTarget);

        new SerializationCallbackTester<ClassWithSerializationCallbacks> (new RhinoMocksRepositoryAdapter (), instance, MixinWithSerializationCallbacks.SetReceiver)
            .Test_DeserializationCallbacks ();
      }
    }
  }
}