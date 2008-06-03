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
