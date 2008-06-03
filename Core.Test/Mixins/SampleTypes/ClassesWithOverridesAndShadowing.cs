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
  public class TargetForOverridesAndShadowing
  {
    public virtual void Method (int i) { }

    public virtual int Property
    {
      get { return 0; }
      set { }
    }

    public virtual event EventHandler Event;
  }
  
  public class BaseWithOverrideAttributes
  {
    [OverrideTarget]
    public virtual void Method(int i)
    {
    }

    [OverrideTarget]
    public virtual int Property
    {
      get { return 0; }
      set { }
    }

    [OverrideTarget]
    public virtual event EventHandler Event;
  }

  public class DerivedWithoutOverrideAttributes : BaseWithOverrideAttributes
  {
    public override void Method (int i)
    {
    }

    public override int Property
    {
      get { return 0; }
      set { }
    }

    public override event EventHandler Event;
  }

  public class DerivedNewWithAdditionalOverrideAttributes : BaseWithOverrideAttributes
  {
    [OverrideTarget]
    public new void Method (int i)
    {
    }

    [OverrideTarget]
    public new int Property
    {
      get { return 0; }
      set { }
    }

    [OverrideTarget]
    public new event EventHandler Event;
  }

  public class BaseWithoutOverrideAttributes
  {
    public virtual void Method (int i)
    {
    }

    public virtual int Property
    {
      get { return 0; }
      set { }
    }

    public virtual event EventHandler Event;
  }

  public class DerivedNewWithOverrideAttributes : BaseWithoutOverrideAttributes
  {
    [OverrideTarget]
    public new void Method (int i)
    {
    }

    [OverrideTarget]
    public new int Property
    {
      get { return 0; }
      set { }
    }

    [OverrideTarget]
    public new event EventHandler Event;
  }

  
  public class DerivedNewWithoutOverrideAttributes : BaseWithoutOverrideAttributes
  {
    public new void Method (int i)
    {
    }

    public new int Property
    {
      get { return 0; }
      set { }
    }

    public new event EventHandler Event;
  }
}
