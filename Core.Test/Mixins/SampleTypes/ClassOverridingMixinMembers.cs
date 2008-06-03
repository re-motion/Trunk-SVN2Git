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
  [Uses(typeof(MixinWithAbstractMembers))]
  [Serializable]
  public class ClassOverridingMixinMembers
  {
    [OverrideMixin]
    public virtual string AbstractMethod(int i)
    {
      return "ClassOverridingMixinMembers.AbstractMethod-" + i;
    }

    [OverrideMixin]
    public virtual string AbstractProperty
    {
      get { return "ClassOverridingMixinMembers.AbstractProperty"; }
    }

    [OverrideMixin]
    public virtual string RaiseEvent ()
    {
      return _abstractEvent ();
    }

    private Func<string> _abstractEvent;

    [OverrideMixin]
    public virtual event Func<string> AbstractEvent
    {
      add { _abstractEvent += value; }
      remove { _abstractEvent -= value; }
    }
  }
}
