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
  [DBTable]
  [Instantiable]
  [Serializable]
  public abstract class DOImplementingISerializable : DomainObject, ISerializable
  {
    public static DOImplementingISerializable NewObject (string memberHeldAsField)
    {
      return NewObject<DOImplementingISerializable> ().With (memberHeldAsField);
    }

    private string _memberHeldAsField;

    protected DOImplementingISerializable (string memberHeldAsField)
    {
      _memberHeldAsField = memberHeldAsField;
    }

    protected DOImplementingISerializable (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
      _memberHeldAsField = info.GetString ("_memberHeldAsField") + "-Ctor";
    }

    public abstract int PropertyWithGetterAndSetter { get; set; }

    public string MemberHeldAsField
    {
      get { return _memberHeldAsField; }
      set { _memberHeldAsField = value; }
    }

    public void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      info.AddValue ("_memberHeldAsField", _memberHeldAsField + "-GetObjectData");
      BaseGetObjectData (info, context);
    }
  }
}
