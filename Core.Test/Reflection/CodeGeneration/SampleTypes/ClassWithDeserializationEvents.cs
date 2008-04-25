using System;
using System.Runtime.Serialization;

namespace Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes
{
  [Serializable]
  public class ClassWithDeserializationEvents : BaseClassWithDeserializationEvents, IDeserializationCallback
  {
    [NonSerialized]
    public bool OnDeserializationCalled;
    [NonSerialized]
    public bool OnDeserializedCalled;
    [NonSerialized]
    public bool OnDeserializingCalled;

    public void OnDeserialization (object sender)
    {
      OnDeserializationCalled = true;
    }

    [OnDeserializing]
    private void OnDeserializing (StreamingContext context)
    {
      OnDeserializingCalled = true;
    }

    [OnDeserialized]
    private void OnDeserialized (StreamingContext context)
    {
      OnDeserializedCalled = true;
    }
  }
}