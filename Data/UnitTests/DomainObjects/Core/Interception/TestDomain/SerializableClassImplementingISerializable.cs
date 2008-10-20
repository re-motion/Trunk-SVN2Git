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

namespace Remotion.Data.UnitTests.DomainObjects.Core.Interception.TestDomain
{
  [Serializable]
  [DBTable]
  public class SerializableClassImplementingISerializable : DomainObject, ISerializable
  {
    public bool ISerializableCtorCalled = false;
    public int I;

    public SerializableClassImplementingISerializable ()
    {
    }

    protected SerializableClassImplementingISerializable (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
      ISerializableCtorCalled = true;
      I = info.GetInt32 ("I");
    }

    public void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      info.AddValue ("I", I);
      BaseGetObjectData (info, context);
    }
  }
}