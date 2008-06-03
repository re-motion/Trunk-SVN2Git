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

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [AttributeUsage (AttributeTargets.All, AllowMultiple = true)]
  public class AttributeWithParameters2 : Attribute
  {
    public int Field;

    private int _property;
    private int _ctor;

    public AttributeWithParameters2 (int ctor)
    {
      _ctor = ctor;
    }

    public AttributeWithParameters2 (int ctor, string dummy)
      : this (ctor)
    {
    }

    public int Property
    {
      get { return _property; }
      set { _property = value; }
    }
  }
}
