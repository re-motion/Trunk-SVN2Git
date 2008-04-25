using System;
using System.Runtime.Serialization;

namespace Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes
{
  [Serializable]
  public class SerializableClass : ISerializable
  {
    private StreamingContext _Context;
    private SerializationInfo _Info;

    public SerializableClass ()
    {
    }

    protected SerializableClass (SerializationInfo info, StreamingContext context)
    {
      Info = info;
      Context = context;
    }

    public virtual void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      Info = info;
      Context = context;
    }

    public SerializationInfo Info
    {
      get { return _Info; }
      set { _Info = value; }
    }

    public StreamingContext Context
    {
      set { _Context = value; }
      get { return _Context; }
    }
  }
}