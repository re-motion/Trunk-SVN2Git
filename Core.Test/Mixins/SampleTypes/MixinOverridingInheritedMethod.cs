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
  public class MixinOverridingInheritedMethod : Mixin<object, MixinOverridingInheritedMethod.IBaseMethods>
  {
    public interface IBaseMethods
    {
      string ProtectedInheritedMethod ();
      string ProtectedInternalInheritedMethod ();
      string PublicInheritedMethod ();
    }

    [OverrideTarget]
    public string ProtectedInheritedMethod ()
    {
      return "MixinOverridingInheritedMethod.ProtectedInheritedMethod-" + Base.ProtectedInheritedMethod ();
    }

    [OverrideTarget]
    public string ProtectedInternalInheritedMethod ()
    {
      return "MixinOverridingInheritedMethod.ProtectedInternalInheritedMethod-" + Base.ProtectedInternalInheritedMethod ();
    }

    [OverrideTarget]
    public string PublicInheritedMethod ()
    {
      return "MixinOverridingInheritedMethod.PublicInheritedMethod-" + Base.PublicInheritedMethod ();
    }
  }

  [Serializable]
  public class BaseClassWithInheritedMethod
  {
    protected internal virtual string ProtectedInternalInheritedMethod ()
    {
      return "BaseClassWithInheritedMethod.ProtectedInternalInheritedMethod";
    }

    protected virtual string ProtectedInheritedMethod ()
    {
      return "BaseClassWithInheritedMethod.ProtectedInheritedMethod";
    }

    public virtual string PublicInheritedMethod ()
    {
      return "BaseClassWithInheritedMethod.PublicInheritedMethod";
    }
  }

  [Uses (typeof (MixinOverridingInheritedMethod))]
  public class ClassWithInheritedMethod : BaseClassWithInheritedMethod
  {
    public string InvokeInheritedMethods ()
    {
      return ProtectedInheritedMethod ()+ "-" + ProtectedInternalInheritedMethod() + "-" +  PublicInheritedMethod();
    }
  }
}
