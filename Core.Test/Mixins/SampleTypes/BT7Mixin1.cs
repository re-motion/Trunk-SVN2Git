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
  public interface IBT7Mixin1
  {
    string One<T> (T t);
    string BT7Mixin1Specific ();
  }

  [Extends (typeof (BaseType7))]
  public class BT7Mixin1 : Mixin<BaseType7, IBaseType7> , IBT7Mixin1
  {
    [OverrideTarget]
    public virtual string One<T> (T t)
    {
      return "BT7Mixin1.One(" + t + ")-" + Base.One(t);
    }

    public string BT7Mixin1Specific ()
    {
      return "BT7Mixin1.BT7Mixin1Specific-" + Base.Three() + "-" + This.Three();
    }
  }
}
