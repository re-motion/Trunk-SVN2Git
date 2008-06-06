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
using Remotion.Mixins.Definitions;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [Serializable]
  public class MixinOverridingClassMethod : Mixin<object, MixinOverridingClassMethod.IRequirements>, IMixinOverridingClassMethod
  {
    public interface IRequirements
    {
      string OverridableMethod (int i);
    }

    public new object This { get { return base.This; } }
    public new object Base { get { return base.Base; } }

    [OverrideTarget]
    public string OverridableMethod (int i)
    {
      return "MixinOverridingClassMethod.OverridableMethod-" + i;
    }

    public virtual string AbstractMethod (int i)
    {
      return "MixinOverridingClassMethod.AbstractMethod-" + i;
    }
  }

  public interface IMixinOverridingClassMethod
  {
    string AbstractMethod (int i);
  }
}
