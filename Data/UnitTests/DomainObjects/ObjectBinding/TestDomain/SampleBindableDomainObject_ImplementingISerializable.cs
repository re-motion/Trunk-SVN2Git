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

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain
{
  [DBTable]
  [Serializable]
  public class SampleBindableDomainObject_ImplementingISerializable 
      : SimpleDomainObject<SampleBindableDomainObject_ImplementingISerializable>, ISerializable
  {
    protected SampleBindableDomainObject_ImplementingISerializable ()
    {
    }

    protected SampleBindableDomainObject_ImplementingISerializable (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
    }

    public virtual int IntProperty { get; set; }
    
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      BaseGetObjectData (info, context);
    }
  }
}