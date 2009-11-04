// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
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

    [Test]
    public void SerializeMethods ()
    {
      TestMethodSerialization (typeof (object).GetMethod ("Equals", BindingFlags.Public | BindingFlags.Instance));
      TestMethodSerialization (typeof (Console).GetMethod ("WriteLine", new Type[] { typeof (string), typeof (object[]) }));
      TestMethodSerialization (typeof (ReflectionObjectSerializerTest).GetMethod ("PerformSerialization", BindingFlags.NonPublic | BindingFlags.Instance));

      MethodInfo[] methods =
        Array.FindAll (typeof (ReflectionObjectSerializerTest).GetMethods (), m => m.Name == "SameName");
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

    // Used by SerializeMethods test caste
    public void SameName<T> () { }
    public void SameName () { }

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

    private T PerformSerialization<T> (T value, Action<T, string, SerializationInfo> serializer, Func<string, SerializationInfo, T> deserializer)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      ArgumentUtility.CheckNotNull ("serializer", serializer);
      ArgumentUtility.CheckNotNull ("deserializer", deserializer);

      SerializationMethods<T>.Serializer = serializer;
      SerializationMethods<T>.Deserializer = deserializer;
      var tester = new SerializationTester<T> (value);
      return Serializer.SerializeAndDeserialize (tester).Value;
    }

    private void TestMethodSerialization (MethodBase method)
    {
      Assert.AreEqual (method, PerformSerialization (method, ReflectionObjectSerializer.SerializeMethodBase,
          ReflectionObjectSerializer.DeserializeMethodBase));
    }

    // This class holds a value which is manually serialized and deserialized via the delegates specified in the SerializationMethods class.
    [Serializable]
    class SerializationTester<T> : ISerializable
    {
      public readonly T Value;

      public SerializationTester (T value)
      {
        ArgumentUtility.CheckNotNull ("value", value);
        Value = value;
      }

      protected SerializationTester (SerializationInfo info, StreamingContext context)
      {
        Value = SerializationMethods<T>.Deserializer ("Value", info);
        Assert.IsNotNull (Value);
      }

      public void GetObjectData (SerializationInfo info, StreamingContext context)
      {
        SerializationMethods<T>.Serializer (Value, "Value", info);
      }
    }

    // Holds the methods used by SerializationTester for serialization.
    static class SerializationMethods<T>
    {
      public static Action<T, string, SerializationInfo> Serializer;
      public static Func<string, SerializationInfo, T> Deserializer;
    }
  }
}
