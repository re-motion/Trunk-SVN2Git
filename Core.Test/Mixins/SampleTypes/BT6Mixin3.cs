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
  public interface IBT6Mixin3
  {
    string Mixin3Method ();
  }

  public interface IBT6Mixin3Constraints : ICBT6Mixin1, ICBT6Mixin2 {}

  [Extends (typeof (BaseType6))]
  public class BT6Mixin3<This> : Mixin<This>, IBT6Mixin3
      where This : class, IBT6Mixin3Constraints
  {
    public string Mixin3Method ()
    {
      return "BT6Mixin3.Mixin3Method";
    }
  }

  [CompleteInterface (typeof (BaseType6))]
  public interface ICBT6Mixin3 : IBT6Mixin1, IBT6Mixin2, IBaseType6
  {
  }
}
