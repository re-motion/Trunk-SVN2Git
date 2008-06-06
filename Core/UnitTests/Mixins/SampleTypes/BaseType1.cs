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

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public class BT1Attribute : Attribute { }

  [Serializable]
  [BT1Attribute]
  public class BaseType1
  {
    public int I;

    [BT1Attribute]
    public virtual string VirtualMethod ()
    {
      return "BaseType1.VirtualMethod";
    }

    public virtual string VirtualMethod (string text)
    {
      return "BaseType1.VirtualMethod(" + text + ")";
    }

    private string _backingField = "BaseType1.BackingField";

    [BT1Attribute]
    public virtual string VirtualProperty
    {
      get { return _backingField; }
      set { _backingField = value; }
    }

    public object this [int index]
    {
      get { return null; }
    }

    public object this[string index]
    {
      set { }
    }

    [BT1Attribute]
    public virtual event EventHandler VirtualEvent;

    public event EventHandler ExplicitEvent
    {
      add { VirtualEvent += value; }
      remove { VirtualEvent -= value; }
    }

    internal Delegate[] GetVirtualEventInvocationList ()
    {
      if (VirtualEvent != null)
        return VirtualEvent.GetInvocationList ();
      else
        return null;
    }
  }
}
