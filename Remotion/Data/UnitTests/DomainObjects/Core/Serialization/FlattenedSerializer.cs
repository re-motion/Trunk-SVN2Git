// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
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

    public static object[] SerializeHandle (IFlattenedSerializable serializable)
    {
      FlattenedSerializationInfo info = new FlattenedSerializationInfo ();
      info.AddHandle (serializable);
      return info.GetData ();
    }

    public static T DeserializeHandle<T> (object[] data) where T : IFlattenedSerializable
    {
      FlattenedDeserializationInfo info = new FlattenedDeserializationInfo (data);
      return info.GetValueForHandle<T> ();
    }

    public static T SerializeAndDeserializeHandle<T> (T serializable) where T : IFlattenedSerializable
    {
      return DeserializeHandle<T>(SerializeHandle(serializable));
    }
  }
}
