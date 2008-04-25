using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class SerializationCallbackTest : ClientTransactionBaseTest
  {
    [Test]
    public void SerializationEvents ()
    {
      ClassWithSerializationCallbacks instance =
          (ClassWithSerializationCallbacks) RepositoryAccessor.NewObject (typeof (ClassWithSerializationCallbacks)).With();

      Assert.AreNotSame (typeof (ClassWithSerializationCallbacks), ((object)instance).GetType ());

      new SerializationCallbackTester<ClassWithSerializationCallbacks> (new RhinoMocksRepositoryAdapter (), instance, ClassWithSerializationCallbacks.SetReceiver)
          .Test_SerializationCallbacks ();
    }

    [Test]
    public void DeserializationEvents ()
    {
      ClassWithSerializationCallbacks instance =
          (ClassWithSerializationCallbacks) RepositoryAccessor.NewObject (typeof (ClassWithSerializationCallbacks)).With();

      Assert.AreNotSame (typeof (ClassWithSerializationCallbacks), ((object) instance).GetType ());

      new SerializationCallbackTester<ClassWithSerializationCallbacks> (new RhinoMocksRepositoryAdapter (), instance, ClassWithSerializationCallbacks.SetReceiver)
          .Test_DeserializationCallbacks ();
    }
  }
}