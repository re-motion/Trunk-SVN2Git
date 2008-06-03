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
  public class ClassWithDeserializationEvents : BaseClassWithDeserializationEvents, IDeserializationCallback
  {
    [NonSerialized]
    public bool OnDeserializationCalled;
    [NonSerialized]
    public bool OnDeserializedCalled;
    [NonSerialized]
    public bool OnDeserializingCalled;

    public void OnDeserialization (object sender)
    {
      OnDeserializationCalled = true;
    }

    [OnDeserializing]
    private void OnDeserializing (StreamingContext context)
    {
      OnDeserializingCalled = true;
    }

    [OnDeserialized]
    private void OnDeserialized (StreamingContext context)
    {
      OnDeserializedCalled = true;
    }
  }
}
