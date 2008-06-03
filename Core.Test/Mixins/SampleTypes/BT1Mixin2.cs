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
  [Extends (typeof (BaseType1))]
  [Serializable]
  [AcceptsAlphabeticOrdering]
  public class BT1Mixin2
  {
    [OverrideTarget]
    public string VirtualMethod ()
    {
      return "Mixin2ForBT1.VirtualMethod";
    }

    [OverrideTarget]
    public string VirtualProperty
    {
      get { return "Mixin2ForBT1.VirtualProperty"; }
      // no setter
    }

    public EventHandler BackingEventField;

    [OverrideTarget]
    public virtual event EventHandler VirtualEvent
    {
      add { BackingEventField += value; }
      remove { BackingEventField -= value; }
    }
  }
}
