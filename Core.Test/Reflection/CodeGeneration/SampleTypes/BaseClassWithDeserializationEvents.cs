using System;
using System.Runtime.Serialization;

namespace Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes
{
  [Serializable]
  public class BaseClassWithDeserializationEvents
  {
    [NonSerialized]
    public bool OnBaseDeserializedCalled;
    [NonSerialized]
    public bool OnBaseDeserializingCalled;

    [OnDeserializing]
    private void OnDeserializing (StreamingContext context)
    {
      OnBaseDeserializingCalled = true;
    }

    [OnDeserialized]
    private void OnDeserialized (StreamingContext context)
    {
      OnBaseDeserializedCalled = true;
    }
  }
}