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

namespace Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes
{
  [Serializable]
  public class SerializableClass : ISerializable
  {
    private StreamingContext _Context;
    private SerializationInfo _Info;

    public SerializableClass ()
    {
    }

    protected SerializableClass (SerializationInfo info, StreamingContext context)
    {
      Info = info;
      Context = context;
    }

    public virtual void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      Info = info;
      Context = context;
    }

    public SerializationInfo Info
    {
      get { return _Info; }
      set { _Info = value; }
    }

    public StreamingContext Context
    {
      set { _Context = value; }
      get { return _Context; }
    }
  }
}
