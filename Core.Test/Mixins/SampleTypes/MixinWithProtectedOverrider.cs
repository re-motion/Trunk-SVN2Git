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
  public class MixinWithProtectedOverrider : Mixin<MixinWithProtectedOverrider.IRequirements, MixinWithProtectedOverrider.IRequirements>
  {
    public interface IRequirements
    {
      string VirtualMethod ();
      string VirtualProperty { get; }
      event EventHandler VirtualEvent;
    }

    [OverrideTarget]
    protected string VirtualMethod ()
    {
      return "MixinWithProtectedOverrider.VirtualMethod-" + Base.VirtualMethod ();
    }

    [OverrideTarget]
    protected string VirtualProperty
    {
      get { return "MixinWithProtectedOverrider.VirtualProperty-" + Base.VirtualProperty; }
    }

    [OverrideTarget]
    protected event EventHandler VirtualEvent
    {
      add
      {
        Base.VirtualEvent += value;
        Base.VirtualEvent += ThisHandler;
      }
      remove
      {
        Base.VirtualEvent -= value;
        Base.VirtualEvent -= ThisHandler;
      }
    }

    private void ThisHandler (object sender, EventArgs e)
    {
      throw new NotImplementedException();
    }
  }
}
