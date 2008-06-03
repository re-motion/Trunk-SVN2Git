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
  [Uses (typeof (MixinWithInheritedMethod))]
  public class ClassOverridingInheritedMixinMethod
  {
    [OverrideMixin]
    public string ProtectedInheritedMethod ()
    {
      return "ClassOverridingInheritedMixinMethod.ProtectedInheritedMethod";
    }

    [OverrideMixin]
    public string ProtectedInternalInheritedMethod ()
    {
      return "ClassOverridingInheritedMixinMethod.ProtectedInternalInheritedMethod";
    }

    [OverrideMixin]
    public string PublicInheritedMethod ()
    {
      return "ClassOverridingInheritedMixinMethod.PublicInheritedMethod";
    }
  }

  public class BaseMixinWithInheritedMethod : Mixin<object>
  {
    protected virtual string ProtectedInheritedMethod ()
    {
      return "BaseMixinWithInheritedMethod.ProtectedInheritedMethod";
    }

    protected internal virtual string ProtectedInternalInheritedMethod ()
    {
      return "BaseMixinWithInheritedMethod.ProtectedInternalInheritedMethod";
    }

    public virtual string PublicInheritedMethod ()
    {
      return "BaseMixinWithInheritedMethod.PublicInheritedMethod";
    }
  }

  public class MixinWithInheritedMethod : BaseMixinWithInheritedMethod
  {
    public string InvokeInheritedMethods ()
    {
      return ProtectedInheritedMethod () + "-" + ProtectedInternalInheritedMethod() + "-" + PublicInheritedMethod();
    }
  }
}
