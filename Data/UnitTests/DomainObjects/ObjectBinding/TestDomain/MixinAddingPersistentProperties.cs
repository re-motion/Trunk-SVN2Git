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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain
{
  public class MixinAddingPersistentProperties : DomainObjectMixin<BindableDomainObjectWithMixedPersistentProperties>, IMixinAddingPersistentProperties
  {
    public DateTime MixedProperty
    {
      get { return Properties[typeof (MixinAddingPersistentProperties), "MixedProperty"].GetValue<DateTime>(); }
      set { Properties[typeof (MixinAddingPersistentProperties), "MixedProperty"].SetValue (value); }
    }

    [MemberVisibility (MemberVisibility.Public)]
    public DateTime PublicMixedProperty
    {
      get { return Properties[typeof (MixinAddingPersistentProperties), "PublicMixedProperty"].GetValue<DateTime> (); }
      set { Properties[typeof (MixinAddingPersistentProperties), "PublicMixedProperty"].SetValue (value); }
    }

    [MemberVisibility (MemberVisibility.Private)]
    public DateTime PrivateMixedProperty
    {
      get { return Properties[typeof (MixinAddingPersistentProperties), "PrivateMixedProperty"].GetValue<DateTime> (); }
      set { Properties[typeof (MixinAddingPersistentProperties), "PrivateMixedProperty"].SetValue (value); }
    }

    [StorageClass(StorageClass.Persistent)]
    DateTime IMixinAddingPersistentProperties.ExplicitMixedProperty
    {
      get { return Properties[typeof (MixinAddingPersistentProperties), typeof (IMixinAddingPersistentProperties) + ".PrivateMixedProperty"].GetValue<DateTime> (); }
      set { Properties[typeof (MixinAddingPersistentProperties), typeof (IMixinAddingPersistentProperties) +  ".PrivateMixedProperty"].SetValue (value); }
    }
  }
}