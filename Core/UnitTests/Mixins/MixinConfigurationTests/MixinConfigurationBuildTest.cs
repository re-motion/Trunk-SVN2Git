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
using NUnit.Framework;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;
using Remotion.Mixins.Context.FluentBuilders;

namespace Remotion.UnitTests.Mixins.MixinConfigurationTests
{
  [TestFixture]
  public class MixinConfigurationBuildTest
  {
    [Test]
    public void BuildNew ()
    {
      MixinConfiguration previousConfiguration = MixinConfiguration.ActiveConfiguration;
      Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
      using (MixinConfiguration.BuildNew ().ForClass<BaseType7> ().AddMixin<BT7Mixin0> ().EnterScope ())
      {
        Assert.AreNotSame (previousConfiguration, MixinConfiguration.ActiveConfiguration);
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType7)));
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ClassContexts.GetWithInheritance (typeof (BaseType7)).Mixins.ContainsKey (typeof (BT7Mixin0)));
        Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
      }
      Assert.AreSame (previousConfiguration, MixinConfiguration.ActiveConfiguration);
    }

    [Test]
    public void BuildFrom ()
    {
      MixinConfiguration previousConfiguration = MixinConfiguration.ActiveConfiguration;
      MixinConfiguration parentConfiguration = new MixinConfigurationBuilder (null)
          .ForClass <BaseType2>().AddMixin (typeof (BT2Mixin1)).BuildConfiguration();
      
      Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
      using (MixinConfiguration.BuildFrom (parentConfiguration).ForClass<BaseType7> ().AddMixin<BT7Mixin0> ().EnterScope ())
      {
        Assert.AreNotSame (previousConfiguration, MixinConfiguration.ActiveConfiguration);
        Assert.AreNotSame (parentConfiguration, MixinConfiguration.ActiveConfiguration);
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType7)));
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ClassContexts.GetWithInheritance (typeof (BaseType7)).Mixins.ContainsKey (typeof (BT7Mixin0)));
        Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType2)));
      }
      Assert.AreSame (previousConfiguration, MixinConfiguration.ActiveConfiguration);
    }

    [Test]
    public void BuildFromActive ()
    {
      MixinConfiguration previousConfiguration = MixinConfiguration.ActiveConfiguration;
      Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType4)));
      using (MixinConfiguration.BuildFromActive ().ForClass<BaseType4> ().AddMixin<BT4Mixin1> ().EnterScope ())
      {
        Assert.AreNotSame (previousConfiguration, MixinConfiguration.ActiveConfiguration);
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ClassContexts.GetWithInheritance (typeof (BaseType1)).Mixins.ContainsKey (typeof (BT1Mixin1)));
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType4)));
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ClassContexts.GetWithInheritance (typeof (BaseType4)).Mixins.ContainsKey (typeof (BT4Mixin1)));
      }
      Assert.AreSame (previousConfiguration, MixinConfiguration.ActiveConfiguration);
    }

    [Test]
    public void BuildFromActive_CausesDefaultConfigurationToBeAnalyzed ()
    {
      MixinConfiguration.SetActiveConfiguration (null);
      Assert.IsFalse (MixinConfiguration.HasActiveConfiguration);
      using (MixinConfiguration.BuildFromActive ().ForClass<BaseType1> ().Clear ().AddMixins (typeof (BT1Mixin1), typeof (BT1Mixin2)).EnterScope ())
      {
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType3)));
      }
      Assert.IsTrue (MixinConfiguration.HasActiveConfiguration);
    }

  }
}
