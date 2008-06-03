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

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  [BindableObject]
  public class ClassWithUndefinedEnumValue
  {
    private EnumWithUndefinedValue _scalar;
    private EnumWithUndefinedValue[] _array;

    public ClassWithUndefinedEnumValue ()
    {
    }

    public EnumWithUndefinedValue Scalar
    {
      get { return _scalar; }
      set { _scalar = value; }
    }

    public EnumWithUndefinedValue[] Array
    {
      get { return _array; }
      set { _array = value; }
    }
  }
}
