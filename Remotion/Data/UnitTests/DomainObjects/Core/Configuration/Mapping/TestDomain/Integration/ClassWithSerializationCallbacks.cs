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
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Integration
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
