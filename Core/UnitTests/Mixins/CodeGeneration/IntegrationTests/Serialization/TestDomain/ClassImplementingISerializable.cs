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
using Remotion.Mixins;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.Serialization.TestDomain
{
  [Serializable]
  [Uses(typeof(NullMixin))]
  public class ClassImplementingISerializable : ISerializable
  {
    public int I;

    public ClassImplementingISerializable ()
    {
    }

    public ClassImplementingISerializable (SerializationInfo info, StreamingContext context)
    {
      I = 13 + info.GetInt32("I");
    }

    public void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      info.AddValue ("I", I);
    }
  }
}