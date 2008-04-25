using System;
using System.Runtime.Serialization;

namespace Remotion.Development.UnitTesting
{
  [Serializable]
  public abstract class ClassWithSerializationCallbacksBase : IDeserializationCallback
  {
    protected abstract ISerializationEventReceiver StaticReceiver { get; }

    public void OnDeserialization (object sender)
    {
      if (StaticReceiver != null)
        StaticReceiver.OnDeserialization (sender);
    }

    [OnDeserialized]
    public void OnDeserialized (StreamingContext context)
    {
      if (StaticReceiver != null)
        StaticReceiver.OnDeserialized (context);
    }

    [OnDeserializing]
    public void OnDeserializing (StreamingContext context)
    {
      if (StaticReceiver != null)
        StaticReceiver.OnDeserializing (context);
    }

    [OnSerialized]
    public void OnSerialized (StreamingContext context)
    {
      if (StaticReceiver != null)
        StaticReceiver.OnSerialized (context);
    }

    [OnSerializing]
    public void OnSerializing (StreamingContext context)
    {
      if (StaticReceiver != null)
        StaticReceiver.OnSerializing (context);
    }
  }
}