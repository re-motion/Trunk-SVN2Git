using System;
using System.Runtime.Serialization;

namespace Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes
{
  [Serializable]
  public class SerializableClassWithoutCtor : ISerializable
  {
    public virtual void GetObjectData (SerializationInfo info, StreamingContext context)
    {
    }
  }
}