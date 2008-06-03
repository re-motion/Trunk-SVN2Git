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
  public interface IMixinWithAbstractMembers
  {
    string ImplementedMethod ();
    string ImplementedProperty ();
    string ImplementedEvent ();
  }

  [Serializable]
  public abstract class MixinWithAbstractMembers : Mixin<object, object>, IMixinWithAbstractMembers
  {
    public int I;

    public string ImplementedMethod ()
    {
      return "MixinWithAbstractMembers.ImplementedMethod-" + AbstractMethod (25);
    }

    public string ImplementedProperty ()
    {
      return "MixinWithAbstractMembers.ImplementedProperty-" + AbstractProperty;
    }

    public string ImplementedEvent ()
    {
      Func<string> func = delegate { return "MixinWithAbstractMembers.ImplementedEvent"; };
      AbstractEvent += func;
      string result = RaiseEvent ();
      AbstractEvent -= func;
      return result;
    }

    protected abstract string AbstractMethod (int i);
    protected abstract string AbstractProperty { get; }
    protected abstract event Func<string> AbstractEvent;
    protected abstract string RaiseEvent ();
  }
}
