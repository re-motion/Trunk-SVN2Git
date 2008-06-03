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
  public interface IBT3Mixin4
  {
    string Foo ();
  }

  [Extends (typeof (BaseType3))]
  [Serializable]
  public class BT3Mixin4 : BT3Mixin3<BaseType3, IBaseType34>, IBT3Mixin4
  {
    public string Foo ()
    {
      return "BT3Mixin4.Foo";
    }
  }
}
