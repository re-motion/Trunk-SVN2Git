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
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.TestDomain
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
