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
  public interface IBT1Mixin1
  {
    string IntroducedMethod ();
    string IntroducedProperty { get; }
    event EventHandler IntroducedEvent;
  }

  public class BT1M1Attribute : Attribute {}

  [Extends (typeof (BaseType1))]
  [Serializable]
  [BT1M1Attribute]
  [AcceptsAlphabeticOrdering]
  public class BT1Mixin1 : IBT1Mixin1
  {
    [OverrideTarget]
    [BT1M1Attribute]
    public string VirtualMethod ()
    {
      return "BT1Mixin1.VirtualMethod";
    }

    public string BackingField = "BT1Mixin1.BackingField";

    [OverrideTarget]
    [BT1M1Attribute]
    public virtual string VirtualProperty
    {
      set { BackingField = value; } // no getter
    }

    public bool VirtualEventAddCalled = false;
    public bool VirtualEventRemoveCalled = false;

    [OverrideTarget]
    [BT1M1Attribute]
    public virtual event EventHandler VirtualEvent
    {
      add { VirtualEventAddCalled = true; }
      remove { VirtualEventRemoveCalled = true; }
    }


    [BT1M1Attribute]
    public string IntroducedMethod ()
    {
      return "BT1Mixin1.IntroducedMethod";
    }

    [BT1M1Attribute]
    public string IntroducedProperty
    {
      get { return "BT1Mixin1.IntroducedProperty"; }
    }

    [BT1M1Attribute]
    public event EventHandler IntroducedEvent;
  }
}
