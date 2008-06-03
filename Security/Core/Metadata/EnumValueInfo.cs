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
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{

  public class EnumValueInfo : MetadataInfo
  {
    // types

    // static members

    // member fields

    private int _value;
    private string _typeName;

    // construction and disposing

    public EnumValueInfo (string typeName, string name, int value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("typeName", typeName);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      _value = value;
      _typeName = typeName;
      Name = name;
    }

    // methods and properties

    public int Value
    {
      get { return _value; }
      set { _value = value; }
    }

    public string TypeName
    {
      get
      {
        return _typeName;
      }
      set
      {
        ArgumentUtility.CheckNotNullOrEmpty ("TypeName", value);
        _typeName = value;
      }
    }

    public override string Description
    {
      get { return TypeName; }
    }
  }
}
