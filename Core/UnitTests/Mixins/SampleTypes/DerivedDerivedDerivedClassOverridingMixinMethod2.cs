/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Mixins;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public class DerivedDerivedDerivedClassOverridingMixinMethod2 : DerivedDerivedClassOverridingMixinMethod
  {
    public override string M1 ()
    {
      return "DerivedDerivedDerivedClassOverridingMixinMethod2.M1";
    }

    public override string M2 ()
    {
      return "DerivedDerivedDerivedClassOverridingMixinMethod2.M2";
    }
  }
}