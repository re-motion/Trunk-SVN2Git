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
using Remotion.Mixins;
using System.Runtime.Serialization;

namespace Remotion.Development.UnitTesting
{
  [Serializable]
  public class MixinWithSerializationCallbacks : Mixin<object>, IDeserializationCallback
  {
    private static ISerializationEventReceiver s_receiver;

    public static void SetStaticReceiver (ISerializationEventReceiver receiver)
    {
      s_receiver = receiver;
    }

    protected virtual ISerializationEventReceiver StaticReceiver
    {
      get { return s_receiver; }
    }

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