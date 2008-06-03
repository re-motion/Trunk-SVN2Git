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
  public interface IDuckBaseRequirements
  {
    string MethodImplementedOnBase ();
    string ProtectedMethodImplementedOnBase ();
  }

  public class DuckBaseMixin : Mixin<object, IDuckBaseRequirements>
  {
    [OverrideTarget]
    public string MethodImplementedOnBase ()
    {
      return "DuckBaseMixin.MethodImplementedOnBase-" + Base.MethodImplementedOnBase ();
    }

    [OverrideTarget]
    public string ProtectedMethodImplementedOnBase ()
    {
      return "DuckBaseMixin.ProtectedMethodImplementedOnBase-" + Base.ProtectedMethodImplementedOnBase ();
    }
  }

  public class DuckBaseMixinWithoutOverrides : Mixin<object, IDuckBaseRequirements>
  {
  }
}
