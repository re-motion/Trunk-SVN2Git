using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Serialization
{
  public static class FlattenedSerializer
  {
    public static object[] Serialize (IFlattenedSerializable serializable)
    {
      FlattenedSerializationInfo info = new FlattenedSerializationInfo();
      info.AddValue (serializable);
      return info.GetData();
    }

    public static T Deserialize<T> (object[] data) where T : IFlattenedSerializable
    {
      FlattenedDeserializationInfo info = new FlattenedDeserializationInfo (data);
      return info.GetValue<T>();
    }

    public static T SerializeAndDeserialize<T> (T serializable) where T : IFlattenedSerializable
    {
      return Deserialize<T> (Serialize (serializable));
    }
  }
}