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
using Remotion.Data.DomainObjects;
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain
{
  public class MixinOverridingPropertiesAndMethods
      : Mixin<MixinOverridingPropertiesAndMethods.IBaseRequirements, MixinOverridingPropertiesAndMethods.IBaseRequirements>
  {

    [OverrideTarget]
    public virtual string Property
    {
      get { return Base.Property + "-MixinGetter"; }
      set { Base.Property = value + "-MixinSetter"; }
    }

    [OverrideTarget]
    public virtual string GetSomething ()
    {
      return Base.GetSomething () + "-MixinMethod";
    }

    public interface IBaseRequirements
    {
      string Property { get; set; }
      string GetSomething ();
    }
  }
}
