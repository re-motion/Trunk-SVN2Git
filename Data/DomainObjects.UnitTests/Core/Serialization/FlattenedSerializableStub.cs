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
  public class FlattenedSerializableStub : IFlattenedSerializable
  {
    public readonly string Data1;
    public readonly int Data2;
    public FlattenedSerializableStub Data3;

    public FlattenedSerializableStub (string data1, int data2)
    {
      Data1 = data1;
      Data2 = data2;
    }

    protected FlattenedSerializableStub (FlattenedDeserializationInfo info)
    {
      Data1 = info.GetValue<string> ();
      Data2 = info.GetIntValue ();
      Data3 = info.GetValueForHandle<FlattenedSerializableStub> ();
    }

    public void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddValue (Data1);
      info.AddIntValue (Data2);
      info.AddHandle (Data3);
    }
  }
}
