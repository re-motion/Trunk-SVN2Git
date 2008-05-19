using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins.Utilities.Serialization;
using Remotion.Utilities;

namespace Remotion.UnitTests.Mixins.Utilities.Serialization
{
  [TestFixture]
  public class ReflectionObjectSerializerTest
  {
    [Serializable]
    class SerializationTester<T> : ISerializable
    {
      public readonly T Value;
      public static Proc<T, string, SerializationInfo> Serializer;
      public static Func<string, SerializationInfo, T> Deserializer;

      public SerializationTester (T value)
      {
        ArgumentUtility.CheckNotNull ("value", value);
        Value = value;
      }

      protected SerializationTester (SerializationInfo info, StreamingContext context)
      {
        Value = Deserializer ("Value", info);
        Assert.IsNotNull (Value);
      }

      public void GetObjectData (SerializationInfo info, StreamingContext context)
      {
        Serializer (Value, "Value", info);
      }
    }

    private static T PerformSerialization<T> (T value, Proc<T, string, SerializationInfo> serializer, Func<string, SerializationInfo, T> deserializer)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      ArgumentUtility.CheckNotNull ("serializer", serializer);
      ArgumentUtility.CheckNotNull ("deserializer", deserializer);

      SerializationTester<T>.Serializer = serializer;
      SerializationTester<T>.Deserializer = deserializer;
      SerializationTester<T> tester = new SerializationTester<T>(value);
      return Serializer.SerializeAndDeserialize (tester).Value;
    }

    [Test]
    public void SerializeTypes()
    {
      Assert.AreEqual (typeof (object),
          PerformSerialization (typeof (object), ReflectionObjectSerializer.SerializeType, ReflectionObjectSerializer.DeserializeType));
      Assert.AreEqual (typeof (Dictionary<,>.Enumerator),
          PerformSerialization (typeof (Dictionary<,>.Enumerator), ReflectionObjectSerializer.SerializeType, ReflectionObjectSerializer.DeserializeType));
      Assert.AreEqual (typeof (Dictionary<int, string>.Enumerator),
          PerformSerialization (typeof (Dictionary<int, string>.Enumerator), ReflectionObjectSerializer.SerializeType, ReflectionObjectSerializer.DeserializeType));
    }

    public void SameName<T> () { }
    public void SameName () { }

    private static void TestMethodSerialization (MethodBase method)
    {
      Assert.AreEqual (method, PerformSerialization (method, ReflectionObjectSerializer.SerializeMethodBase,
          ReflectionObjectSerializer.DeserializeMethodBase));
    }

    [Test]
    public void SerializeMethods ()
    {
      TestMethodSerialization (typeof (object).GetMethod ("Equals", BindingFlags.Public | BindingFlags.Instance));
      TestMethodSerialization (typeof (Console).GetMethod ("WriteLine", new Type[] { typeof (string), typeof (object[]) }));
      TestMethodSerialization (typeof (ReflectionObjectSerializerTest).GetMethod ("PerformSerialization", BindingFlags.NonPublic | BindingFlags.Static));

      MethodInfo[] methods =
        Array.FindAll (typeof (ReflectionObjectSerializerTest).GetMethods (), delegate (MethodInfo m) { return m.Name == "SameName"; });
      Assert.AreEqual (2, methods.Length);

      TestMethodSerialization (methods[0]);
      TestMethodSerialization (methods[1]);

      TestMethodSerialization (typeof (GenericType<>).GetMethod ("NonGenericMethod"));
      TestMethodSerialization (typeof (GenericType<>).GetMethod ("GenericMethod"));
      TestMethodSerialization (typeof (GenericType<>).GetMethod ("GenericMethod").MakeGenericMethod (typeof (ReflectionObjectSerializerTest)));
      TestMethodSerialization (typeof (GenericType<>).GetMethod ("GenericMethod").MakeGenericMethod (typeof (int)));
      TestMethodSerialization (typeof (GenericType<>).GetMethod ("GenericMethod").MakeGenericMethod (typeof (DateTime)));

      TestMethodSerialization (typeof (GenericType<object>).GetMethod ("NonGenericMethod"));
      TestMethodSerialization (typeof (GenericType<object>).GetMethod ("GenericMethod"));
      TestMethodSerialization (typeof (GenericType<object>).GetMethod ("GenericMethod").MakeGenericMethod (typeof (ReflectionObjectSerializerTest)));
      TestMethodSerialization (typeof (GenericType<object>).GetMethod ("GenericMethod").MakeGenericMethod (typeof (int)));
      TestMethodSerialization (typeof (GenericType<object>).GetMethod ("GenericMethod").MakeGenericMethod (typeof (DateTime)));

      TestMethodSerialization (typeof (GenericType<int>).GetMethod ("NonGenericMethod"));
      TestMethodSerialization (typeof (GenericType<int>).GetMethod ("GenericMethod"));
      TestMethodSerialization (typeof (GenericType<int>).GetMethod ("GenericMethod").MakeGenericMethod (typeof (ReflectionObjectSerializerTest)));
      TestMethodSerialization (typeof (GenericType<int>).GetMethod ("GenericMethod").MakeGenericMethod (typeof (int)));
      TestMethodSerialization (typeof (GenericType<int>).GetMethod ("GenericMethod").MakeGenericMethod (typeof (DateTime)));
    }

    [Test]
    public void SerializeConstructors()
    {
      TestMethodSerialization (typeof (GenericType<>).GetConstructor (Type.EmptyTypes));
      TestMethodSerialization (typeof (GenericType<int>).GetConstructor (Type.EmptyTypes));
      TestMethodSerialization (typeof (GenericType<object>).GetConstructor (Type.EmptyTypes));

      TestMethodSerialization (typeof (GenericType<>).GetConstructor (new Type[] {typeof (GenericType<>).GetGenericArguments()[0]}));
      TestMethodSerialization (typeof (GenericType<int>).GetConstructor (new Type[] {typeof (int)}));
      TestMethodSerialization (typeof (GenericType<object>).GetConstructor (new Type[] { typeof (object) }));
    }
  }
}
