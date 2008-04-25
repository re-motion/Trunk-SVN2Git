using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration;
using Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class SerializationEventRaiserTest
  {
    [Test]
    public void InvokeAttributedMethod_OnDeserialized ()
    {
      SerializationEventRaiser eventRaiser = new SerializationEventRaiser();

      ClassWithDeserializationEvents instance = new ClassWithDeserializationEvents ();
      Assert.IsFalse (instance.OnBaseDeserializingCalled);
      Assert.IsFalse (instance.OnBaseDeserializedCalled);
      Assert.IsFalse (instance.OnDeserializingCalled);
      Assert.IsFalse (instance.OnDeserializedCalled);
      Assert.IsFalse (instance.OnDeserializationCalled);

      eventRaiser.InvokeAttributedMethod (instance, typeof (OnDeserializedAttribute), new StreamingContext ());

      Assert.IsFalse (instance.OnBaseDeserializingCalled);
      Assert.IsTrue (instance.OnBaseDeserializedCalled);
      Assert.IsFalse (instance.OnDeserializingCalled);
      Assert.IsTrue (instance.OnDeserializedCalled);
      Assert.IsFalse (instance.OnDeserializationCalled);
    }

    [Test]
    public void InvokeAttributedMethod_OnDeserializing ()
    {
      SerializationEventRaiser eventRaiser = new SerializationEventRaiser ();

      ClassWithDeserializationEvents instance = new ClassWithDeserializationEvents ();
      Assert.IsFalse (instance.OnBaseDeserializingCalled);
      Assert.IsFalse (instance.OnBaseDeserializedCalled);
      Assert.IsFalse (instance.OnDeserializingCalled);
      Assert.IsFalse (instance.OnDeserializedCalled);
      Assert.IsFalse (instance.OnDeserializationCalled);

      eventRaiser.InvokeAttributedMethod (instance, typeof (OnDeserializingAttribute), new StreamingContext ());

      Assert.IsTrue (instance.OnBaseDeserializingCalled);
      Assert.IsFalse (instance.OnBaseDeserializedCalled);
      Assert.IsTrue (instance.OnDeserializingCalled);
      Assert.IsFalse (instance.OnDeserializedCalled);
      Assert.IsFalse (instance.OnDeserializationCalled);
    }

    [Test]
    public void InvokeAttributedMethod_UsesCache ()
    {
      ClassWithDeserializationEvents instance = new ClassWithDeserializationEvents ();
      StreamingContext context = new StreamingContext();

      MockRepository repository = new MockRepository();
      SerializationEventRaiser eventRaiserMock = repository.CreateMock<SerializationEventRaiser>();

      eventRaiserMock.InvokeAttributedMethod (instance, typeof (OnDeserializedAttribute), context);
      LastCall.CallOriginalMethod (OriginalCallOptions.CreateExpectation);

      Expect.Call (PrivateInvoke.InvokeNonPublicMethod (eventRaiserMock, "FindDeserializationMethodsWithCache", typeof (ClassWithDeserializationEvents), typeof (OnDeserializedAttribute)))
          .Return (new List<MethodInfo>());

      repository.ReplayAll();

      eventRaiserMock.InvokeAttributedMethod (instance, typeof (OnDeserializedAttribute), context);

      repository.VerifyAll();
    }

    [Test]
    public void FindDeserializationMethodsWithCache_Caching ()
    {
      SerializationEventRaiser eventRaiser = new SerializationEventRaiser();
      List<MethodInfo> methods = (List<MethodInfo>) PrivateInvoke.InvokeNonPublicMethod (
          eventRaiser, "FindDeserializationMethodsWithCache", typeof (ClassWithDeserializationEvents), typeof (OnDeserializedAttribute));
      Assert.IsNotNull (methods);
      List<MethodInfo> methods2 = (List<MethodInfo>) PrivateInvoke.InvokeNonPublicMethod (
          eventRaiser, "FindDeserializationMethodsWithCache", typeof (ClassWithDeserializationEvents), typeof (OnDeserializedAttribute));
      Assert.AreSame (methods, methods2);
    }

    [Test]
    public void RaiseOnDeserialization ()
    {
      SerializationEventRaiser eventRaiser = new SerializationEventRaiser ();

      ClassWithDeserializationEvents instance = new ClassWithDeserializationEvents ();
      Assert.IsFalse (instance.OnBaseDeserializingCalled);
      Assert.IsFalse (instance.OnBaseDeserializedCalled);
      Assert.IsFalse (instance.OnDeserializingCalled);
      Assert.IsFalse (instance.OnDeserializedCalled);
      Assert.IsFalse (instance.OnDeserializationCalled);

      eventRaiser.RaiseDeserializationEvent (instance, null);

      Assert.IsFalse (instance.OnBaseDeserializingCalled);
      Assert.IsFalse (instance.OnBaseDeserializedCalled);
      Assert.IsFalse (instance.OnDeserializingCalled);
      Assert.IsFalse (instance.OnDeserializedCalled);
      Assert.IsTrue (instance.OnDeserializationCalled);
    }
  }
}