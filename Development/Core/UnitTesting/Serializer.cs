/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting
{
  /// <summary>
  /// Provides quick serialization and deserialization functionality for unit tests.
  /// </summary>
  /// <remarks>The methods of this class use a <see cref="BinaryFormatter"/> for serialization.</remarks>
  public static class Serializer
  {
    public static T SerializeAndDeserialize<T> (T t)
    {
      ArgumentUtility.CheckNotNull ("t", t);
      return (T) Serializer.Deserialize (Serializer.Serialize ((object) t));
    }

    public static byte[] Serialize (object o)
    {
      ArgumentUtility.CheckNotNull ("o", o);

      using (MemoryStream stream = new MemoryStream ())
      {
        BinaryFormatter formatter = new BinaryFormatter ();
        formatter.Serialize (stream, o);
        return stream.ToArray();
      }
    }

    public static object Deserialize (byte[] bytes)
    {
      ArgumentUtility.CheckNotNull ("bytes", bytes);

      using (MemoryStream stream = new MemoryStream (bytes))
      {
        BinaryFormatter formatter = new BinaryFormatter ();
        return formatter.Deserialize (stream);
      }
    }

    public static byte[] XmlSerialize (object o)
    {
      using (MemoryStream stream = new MemoryStream ())
      {
        XmlSerializer serializer = new XmlSerializer (o.GetType());
        serializer.Serialize (stream, o);
        return stream.ToArray();
      }
    }

    public static T XmlDeserialize<T> (byte[] bytes)
    {
      using (MemoryStream stream = new MemoryStream (bytes))
      {
        XmlSerializer serializer = new XmlSerializer (typeof (T));
        return (T) serializer.Deserialize (stream);
      }
    }

    public static T XmlSerializeAndDeserialize<T> (T t)
    {
      return XmlDeserialize<T> (XmlSerialize (t));
    }
  }
}
