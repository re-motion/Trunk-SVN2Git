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
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [Extends (typeof (BaseType3))]
  [Serializable]
  public class BT3Mixin2 : Mixin<IBaseType32>
  {
    public int I;

    public BT3Mixin2()
    {
    }

    protected BT3Mixin2 (int i)
    {
      I = i;
    }

    public new IBaseType32 This
    {
      get { return base.This; }
    }
  }

  [Serializable]
  public class BT3Mixin2B : Mixin<IBaseType32>
  {
    public new IBaseType32 This
    {
      get { return base.This; }
    }
  }
}
