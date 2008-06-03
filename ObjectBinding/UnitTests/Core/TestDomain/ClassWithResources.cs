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
using Remotion.Globalization;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  [BindableObject]
  [MultiLingualResources ("Remotion.ObjectBinding.UnitTests.Core.Globalization.ClassWithResources")]
  public class ClassWithResources
  {
    private string _value1;
    private string _valueWithoutResource;

    public ClassWithResources ()
    {
    }
    
    public string Value1
    {
      get { return _value1; }
      set { _value1 = value; }
    }

    public string ValueWithoutResource
    {
      get { return _valueWithoutResource; }
      set { _valueWithoutResource = value; }
    }
  }
}
