using System;
using System.Runtime.Serialization;

namespace Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes
{
  [Serializable]
  public class SerializableClassWithPrivateGetObjectData : ISerializable
  {
    public SerializableClassWithPrivateGetObjectData ()
    {
    }

    protected SerializableClassWithPrivateGetObjectData (SerializationInfo info, StreamingContext context)
    {
    }

    void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
    {
    }
  }
}