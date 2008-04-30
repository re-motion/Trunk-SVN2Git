using System;
using System.Runtime.Serialization;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable]
  [Serializable]
  [Instantiable]
  public abstract class ClassWithSerializationCallbacks : DomainObject, IDeserializationCallback
  {
    private static ISerializationEventReceiver s_receiver;

    public static void SetReceiver (ISerializationEventReceiver receiver)
    {
      s_receiver = receiver;
    }

    public void OnDeserialization (object sender)
    {
      if (s_receiver != null)
        s_receiver.OnDeserialization (sender);
    }

    [OnDeserialized]
    public void OnDeserialized (StreamingContext context)
    {
      if (s_receiver != null)
        s_receiver.OnDeserialized (context);
    }

    [OnDeserializing]
    public void OnDeserializing (StreamingContext context)
    {
      if (s_receiver != null)
        s_receiver.OnDeserializing (context);
    }

    [OnSerialized]
    public void OnSerialized (StreamingContext context)
    {
      if (s_receiver != null)
        s_receiver.OnSerialized (context);
    }

    [OnSerializing]
    public void OnSerializing (StreamingContext context)
    {
      if (s_receiver != null)
        s_receiver.OnSerializing (context);
    }
  }
}