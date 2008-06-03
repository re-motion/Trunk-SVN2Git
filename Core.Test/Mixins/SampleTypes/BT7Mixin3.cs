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
  public interface IBT7Mixin3
  {
    string One<T> (T t);
  }

  [Extends (typeof (BaseType7))]
  public class BT7Mixin3 : Mixin<object, IBT7Mixin1>, IBT7Mixin3
  {
    [OverrideTarget]
    public virtual string One<T> (T t)
    {
      return "BT7Mixin3.One(" + t + ")-" + Base.BT7Mixin1Specific() + "-" + Base.One(t);
    }
  }
}
