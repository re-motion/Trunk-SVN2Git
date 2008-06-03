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
  public interface IBT6Mixin2
  {
    string Mixin2Method ();
  }

  [Extends (typeof (BaseType6))]
  public class BT6Mixin2 : Mixin<IBaseType6>, IBT6Mixin2
  {
    public string Mixin2Method ()
    {
      return "BT6Mixin2.Mixin2Method";
    }
  }

  [CompleteInterface (typeof (BaseType6))]
  public interface ICBT6Mixin2 : IBT6Mixin2, IBaseType6
  {
  }
}
