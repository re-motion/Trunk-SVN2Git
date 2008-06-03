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

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IBaseType2
  {
    string IfcMethod ();
  }
  
  [Serializable]
  public class BaseType2 : IBaseType2, ISerializable
  {
    public string S;

    public BaseType2 ()
    {
    }

    public BaseType2 (SerializationInfo info, StreamingContext context)
    {
      S = info.GetString ("S");
    }

    public void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      info.AddValue ("S", S);
    }

    public string IfcMethod ()
    {
      return "BaseType2.IfcMethod";
    }
  }
}
